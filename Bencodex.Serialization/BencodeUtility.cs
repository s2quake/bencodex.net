using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using Bencodex.Serialization.Converters;
using Bencodex.Types;

namespace Bencodex.Serialization;

public static class BencodeUtility
{
    internal static readonly Dictionary<Type, BencodeConverter> ConverterByType = new()
    {
        { typeof(bool), BooleanConverter.Default },
        { typeof(byte), IntegerConverter.Default },
        { typeof(sbyte), IntegerConverter.Default },
        { typeof(short), IntegerConverter.Default },
        { typeof(ushort), IntegerConverter.Default },
        { typeof(int), IntegerConverter.Default },
        { typeof(uint), IntegerConverter.Default },
        { typeof(long), IntegerConverter.Default },
        { typeof(ulong), IntegerConverter.Default },
        { typeof(BigInteger), IntegerConverter.Default },
        { typeof(float), TextConverter.Default },
        { typeof(double), TextConverter.Default },
        { typeof(decimal), TextConverter.Default },
        { typeof(char), TextConverter.Default },
        { typeof(string), TextConverter.Default },
        { typeof(DateTime), TextConverter.Default },
        { typeof(TimeSpan), TextConverter.Default },
        { typeof(DateTimeOffset), TextConverter.Default },
    };

    public static Type[] SupportedTypes { get; } = [.. ConverterByType.Keys];

    public static bool IsBencodable(object obj)
        => obj is IBencodable;

    public static bool IsBencodable(Type type)
        => typeof(IBencodable).IsAssignableFrom(type) == true;

    public static void VerifyBencodable(object obj)
    {
        if (obj is IBencodable != true)
        {
            throw new ArgumentException(
                message: $"Parameter '{nameof(obj)}' must implement '{typeof(IBencodable)}'.",
                paramName: nameof(obj)
            );
        }
    }

    public static void VerifyBencodable(Type type)
    {
        if (typeof(IBencodable).IsAssignableFrom(type) != true)
        {
            throw new ArgumentException(
                message: $"Parameter '{nameof(type)}' must implement '{typeof(IBencodable)}'.",
                paramName: nameof(type)
            );
        }
    }

    public static bool IsSupportedType(Type type)
    {
        if (IsArrayType(type) == true)
        {
            if (type.GetElementType() is not { } elementType)
            {
                return false;
            }

            return IsSupportedType(elementType);
        }

        if (typeof(IValue).IsAssignableFrom(type) == true)
        {
            return true;
        }

        if (SupportedTypes.Contains(type) == true)
        {
            return true;
        }

        if (type.IsAbstract != true &&
            type.IsInterface != true &&
            IsStaticClass(type) != true)
        {
            return true;
        }

        return false;
    }

    public static void VerifySupportedType(Type type)
    {
        if (IsStaticClass(type) == true)
        {
            throw new NotSupportedException($"Unsupported type: '{type}'.");
        }

        if (type.IsAbstract == true || type.IsInterface == true)
        {
            throw new NotSupportedException($"Unsupported type: '{type}'.");
        }

        if (SupportedTypes.Contains(type) != true)
        {
            throw new NotSupportedException($"Unsupported type: '{type}'.");
        }
    }

    public static bool IsStaticClass(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) == null && type.IsAbstract == true && type.IsSealed == true;
    }

    public static bool IsArrayType(Type type)
        => IsArrayType(type, out var _);

    public static bool IsArrayType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (typeof(Array).IsAssignableFrom(type) == true)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        elementType = null;
        return false;
    }

    public static bool IsDictionaryType(Type type)
    {
        if (type.IsGenericType == true)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(Dictionary<,>))
            {
                var genericArguments = type.GetGenericArguments();
                var keyType = genericArguments[0];
                return IsKeyType(keyType);
            }
        }

        return false;
    }

    public static bool IsImmutableDictionaryType(Type type)
    {
        if (type.IsGenericType == true)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ImmutableDictionary<,>))
            {
                var genericArguments = type.GetGenericArguments();
                var keyType = genericArguments[0];
                return IsKeyType(keyType);
            }
        }

        return false;
    }

    public static bool IsImmutableArrayType(Type type)
        => IsImmutableArrayType(type, out var _);

    public static bool IsImmutableArrayType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (type.IsGenericType == true)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ImmutableArray<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        elementType = null;
        return false;
    }

    public static bool IsImmutableListType(Type type)
        => IsImmutableListType(type, out var _);

    public static bool IsImmutableListType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (type.IsGenericType == true)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ImmutableList<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        elementType = null;
        return false;
    }

    public static bool IsListType(Type type)
        => IsListType(type, out var _);

    public static bool IsListType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (type.IsGenericType == true)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        elementType = null;
        return false;
    }

    internal static int GetCollectionCount(object obj)
    {
        if (obj is ICollection collection)
        {
            return collection.Count;
        }

        var type = obj.GetType();
        if (IsAssignableFrom(typeof(IReadOnlyCollection<>), type) == true)
        {
            var genericArguments = type.GetGenericArguments();
            var genericCollectionType = typeof(IReadOnlyCollection<>).MakeGenericType(genericArguments);
            var propertyInfo = genericCollectionType.GetProperty(nameof(IReadOnlyCollection<object>.Count));
            if (propertyInfo is null)
            {
                throw new InvalidOperationException("The property 'Count' is not found.");
            }

            return (int)propertyInfo.GetValue(obj)!;
        }

        return 0;
    }

    internal static bool IsAssignableFrom(Type baseType, Type? type)
    {
        if (type is null)
        {
            return false;
        }

        if (baseType.IsGenericTypeDefinition == true && type.IsGenericType == true)
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length != 2)
            {
                return false;
            }

            if (IsKeyType(genericArguments[0]) != true)
            {
                return false;
            }

            return baseType.MakeGenericType(genericArguments).IsAssignableFrom(type);
        }

        return baseType.IsAssignableFrom(type);
    }

    internal static object ToArray(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var list = (List)obj;
        var elementType = destinationType.GetElementType()!;
        var array = Array.CreateInstance(elementType, list.Count);
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var converter = typeContext.GetConverter(typeDescriptor, elementType);
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            var itemValue = item == null || item is Null ? null : converter.ConvertTo(typeContext, item, elementType);
            array.SetValue(itemValue, i);
        }

        return array;
    }

    internal static object ToImmutableList(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var list = (List)obj;
        var elementType = destinationType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var listInstance = (IList)Activator.CreateInstance(listType)!;
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var converter = typeContext.GetConverter(typeDescriptor, elementType);
        foreach (var item in list)
        {
            var itemValue = item == null || item is Null ? null : converter.ConvertTo(typeContext, item, elementType);
            listInstance.Add(itemValue);
        }

        var methodName = nameof(ImmutableList.CreateRange);
        var methodInfo = GetCreateRangeMethod(typeof(ImmutableList), methodName, typeof(IEnumerable<>));
        var genericMethodInfo = methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }

    internal static object ToImmutableArray(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var list = (List)obj;
        var elementType = destinationType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var listInstance = (IList)Activator.CreateInstance(listType)!;
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var converter = typeContext.GetConverter(typeDescriptor, elementType);
        foreach (var item in list)
        {
            var itemValue = item == null || item is Null ? null : converter.ConvertTo(typeContext, item, elementType);
            listInstance.Add(itemValue);
        }

        var methodName = nameof(ImmutableArray.CreateRange);
        var methodInfo = GetCreateRangeMethod(typeof(ImmutableArray), methodName, typeof(IEnumerable<>));
        var genericMethodInfo = methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }

    internal static object ToList(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var list = (List)obj;
        var elementType = destinationType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var listInstance = (IList)Activator.CreateInstance(listType)!;
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var converter = typeContext.GetConverter(typeDescriptor, elementType);
        foreach (var item in list)
        {
            var itemValue = item == null || item is Null ? null : converter.ConvertTo(typeContext, item, elementType);
            listInstance.Add(itemValue);
        }

        return listInstance;
    }

    internal static IValue ToBencodeDictionary(IBencodeTypeContext typeContext, object obj)
    {
        var capacity = GetCollectionCount(obj);
        var enumerable = (IEnumerable)obj;
        var enumerator = enumerable.GetEnumerator();
        var genericArguments = obj.GetType().GetGenericArguments();
        var genericPairType = typeof(KeyValuePair<,>).MakeGenericType(genericArguments);
        var keyPropertyInfo = genericPairType.GetProperty(nameof(KeyValuePair<object, object>.Key))!;
        var valuePropertyInfo = genericPairType.GetProperty(nameof(KeyValuePair<object, object>.Value))!;
        var itemInfoList = new List<DictionaryItemInfo>(capacity);

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            var (key, keyType) = GetKey(typeContext, keyPropertyInfo, current);
            var (value, valueType) = GetValue(typeContext, valuePropertyInfo, current);
            itemInfoList.Add(new DictionaryItemInfo
            {
                KeyType = keyType,
                ValueType = valueType,
                Key = key,
                Value = value,
            });
        }

        var dictionaryInfo = new DictionaryInfo
        {
            Type = obj.GetType().AssemblyQualifiedName!,
            Items = [.. itemInfoList],
        };
        var typeDescriptor = new BencodeTypeDescriptor(obj.GetType());
        var dictionaryConverter = typeContext.GetConverter(typeDescriptor, typeof(DictionaryInfo));

        return dictionaryConverter.ConvertFrom(typeContext, dictionaryInfo);
    }

    internal static object ToImmutableDictionary(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var dictionaryConverter = typeContext.GetConverter(typeDescriptor, typeof(DictionaryInfo));
        var dictionaryInfo = (DictionaryInfo)dictionaryConverter.ConvertTo(typeContext, obj, typeof(DictionaryInfo))!;
        var dictionaryType = GetType(dictionaryInfo.Type);
        var capacity = dictionaryInfo.Items.Length;
        var genericPairType = typeof(KeyValuePair<,>).MakeGenericType(destinationType.GetGenericArguments());
        var genericListType = typeof(List<>).MakeGenericType(genericPairType);
        var list = (IList)Activator.CreateInstance(genericListType, args: [capacity])!;

        for (var i = 0; i < dictionaryInfo.Items.Length; i++)
        {
            var itemInfo = dictionaryInfo.Items[i];
            var key = GetKey(typeContext, itemInfo);
            var value = GetValue(typeContext, itemInfo);
            var pair = Activator.CreateInstance(genericPairType, args: [key, value])!;
            list.Add(pair);
        }

        var emptyFieldInfo = dictionaryType.GetField(nameof(ImmutableDictionary<object, object>.Empty), BindingFlags.Public | BindingFlags.Static)!;
        var emptyValue = emptyFieldInfo.GetValue(null)!;
        var addRangeMethodInfo = emptyFieldInfo.FieldType.GetMethod(nameof(ImmutableDictionary<object, object>.AddRange))!;
        return addRangeMethodInfo.Invoke(emptyValue, [list])!;
    }

    internal static object ToDictionary(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
    {
        var typeDescriptor = new BencodeTypeDescriptor(destinationType);
        var dictionaryConverter = typeContext.GetConverter(typeDescriptor, typeof(DictionaryInfo));
        var dictionaryInfo = (DictionaryInfo)dictionaryConverter.ConvertTo(typeContext, obj, typeof(DictionaryInfo))!;
        var dictionaryType = GetType(dictionaryInfo.Type);
        var capacity = dictionaryInfo.Items.Length;
        var genericPairType = typeof(KeyValuePair<,>).MakeGenericType(destinationType.GetGenericArguments());
        var genericListType = typeof(List<>).MakeGenericType(genericPairType);
        var list = (IList)Activator.CreateInstance(genericListType, args: [capacity])!;

        for (var i = 0; i < dictionaryInfo.Items.Length; i++)
        {
            var itemInfo = dictionaryInfo.Items[i];
            var key = GetKey(typeContext, itemInfo);
            var value = GetValue(typeContext, itemInfo);
            var pair = Activator.CreateInstance(genericPairType, args: [key, value])!;
            list.Add(pair);
        }

        return Activator.CreateInstance(dictionaryType, list)!;
    }

    internal static IKey GetKey(object key)
    {
        if (key is string keyString)
        {
            return new Text(keyString);
        }

        if (key is byte[] keyBytes)
        {
            return new Binary(keyBytes);
        }

        throw new InvalidOperationException($"The key type '{key.GetType()}' is not supported.");
    }

    internal static BencodeConverter? GetConverter(Type type)
    {
        if (IsArrayType(type) == true)
        {
            return ListConverter.Default;
        }

        if (ConverterByType.ContainsKey(type) == true)
        {
            return ConverterByType[type];
        }

        if (IsDictionaryType(type) == true)
        {
            return DictionaryConverter.Default;
        }

        if (IsImmutableDictionaryType(type) == true)
        {
            return DictionaryConverter.Default;
        }

        if (IsImmutableArrayType(type) == true)
        {
            return ListConverter.Default;
        }

        if (IsImmutableListType(type) == true)
        {
            return ListConverter.Default;
        }

        if (IsListType(type) == true)
        {
            return ListConverter.Default;
        }

        return null;
    }

    private static (IKey, string) GetKey(IBencodeTypeContext typeContext, PropertyInfo propertyInfo, object obj)
    {
        var key = propertyInfo.GetValue(obj)!;
        var keyType = key.GetType();
        var typeDescriptor = new BencodeTypeDescriptor(propertyInfo);
        var keyConverter = typeContext.GetConverter(typeDescriptor, keyType);
        var k = (IKey)keyConverter.ConvertFrom(typeContext, key);
        var t = keyType.AssemblyQualifiedName!;
        return (k, t);
    }

    private static (IValue, string?) GetValue(IBencodeTypeContext typeContext, PropertyInfo propertyInfo, object obj)
    {
        var value = propertyInfo.GetValue(obj);
        if (value == null)
        {
            return (Null.Value, null);
        }

        var valueType = value.GetType();
        var typeDescriptor = new BencodeTypeDescriptor(propertyInfo);
        var valueConverter = typeContext.GetConverter(typeDescriptor, valueType);
        var v = valueConverter.ConvertFrom(typeContext, value);
        var t = valueType.AssemblyQualifiedName;
        return (v, t);
    }

    private static object GetKey(IBencodeTypeContext typeContext, DictionaryItemInfo itemInfo)
    {
        var keyType = GetType(itemInfo.KeyType);
        var typeDescriptor = new BencodeTypeDescriptor(typeof(DictionaryInfo));
        var keyConverter = typeContext.GetConverter(typeDescriptor, keyType);
        return keyConverter.ConvertTo(typeContext, itemInfo.Key, keyType);
    }

    private static object? GetValue(IBencodeTypeContext typeContext, DictionaryItemInfo itemInfo)
    {
        if (itemInfo.Value is null)
        {
            return null;
        }

        var valueType = GetType(itemInfo.ValueType!);
        var typeDescriptor = new BencodeTypeDescriptor(typeof(DictionaryInfo));
        var valueConverter = typeContext.GetConverter(typeDescriptor, valueType);
        return valueConverter.ConvertTo(typeContext, itemInfo.Value, valueType);
    }

    private static bool IsKeyType(Type type)
    {
        if (type == typeof(object))
        {
            return true;
        }

        if (type == typeof(string))
        {
            return true;
        }

        if (type == typeof(byte[]))
        {
            return true;
        }

        return false;
    }

    private static Type GetType(string assemblyQualifiedName)
    {
        if (Type.GetType(assemblyQualifiedName) is { } type)
        {
            return type;
        }

        throw new ArgumentException($"The type '{assemblyQualifiedName}' is not found.", nameof(assemblyQualifiedName));
    }

    private static MethodInfo GetCreateRangeMethod(Type type, string methodName, Type parameterType)
    {
        var parameterName = parameterType.Name;
        var bindingFlags = BindingFlags.Public | BindingFlags.Static;
        var methodInfos = type.GetMethods(bindingFlags);

        for (var i = 0; i < methodInfos.Length; i++)
        {
            var methodInfo = methodInfos[i];
            var parameters = methodInfo.GetParameters();
            if (methodInfo.Name == methodName &&
                parameters.Length == 1 &&
                parameters[0].ParameterType.Name == parameterName)
            {
                return methodInfo;
            }
        }

        throw new NotSupportedException("The method is not found.");
    }

    private struct DictionaryItemInfo
    {
        [Bencode]
        public string KeyType { get; set; }

        [Bencode]
        public string? ValueType { get; set; }

        [Bencode]
        public IKey Key { get; set; }

        [Bencode]
        public IValue Value { get; set; }
    }

    private struct DictionaryInfo
    {
        [Bencode]
        public string Type { get; set; }

        [Bencode]
        public DictionaryItemInfo[] Items { get; set; }
    }
}
