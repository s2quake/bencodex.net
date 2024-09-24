using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Bencodex.Types;

namespace Bencodex.Serialization.Utilities;

public static class DictionaryUtility
{
    public static bool IsDictionary(Type type)
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

    public static bool IsImmutableDictionary(Type type)
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

    public static IValue ToBencodeDictionary(IBencodeTypeContext typeContext, object obj)
    {
        var isSupportedByteArray = IsSupportedByteArray(obj);
        var capacity = CollectionUtility.GetCount(obj);
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
            var (key, keyType) = GetKey(typeContext, keyPropertyInfo, current, isSupportedByteArray);
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

    public static object ToImmutableDictionary(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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

    public static object ToDictionary(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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

    public static bool IsSupportedByteArray(object obj)
    {
        var comparer = GetComparer(obj);
        var x1 = new byte[] { 1, 2, 3 };
        var x2 = new byte[] { 1, 2, 3 };
        var y1 = new byte[] { };
        var y2 = new byte[] { };

        if (comparer is IEqualityComparer<object> objectComparer)
        {
            return objectComparer.Equals(x1, x2) && objectComparer.Equals(x2, x1) &&
                objectComparer.Equals(y1, y2) && objectComparer.Equals(y2, y1);
        }
        else if (comparer is IEqualityComparer<byte[]> bytesComparer)
        {
            return bytesComparer.Equals(x1, x2) && bytesComparer.Equals(x2, x1) &&
                bytesComparer.Equals(y1, y2) && bytesComparer.Equals(y2, y1);
        }

        return false;
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

    internal static bool IsKeyType(Type type)
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

    internal static void VerifyKeyType(Type type)
    {
        if (IsKeyType(type) != true)
        {
            throw new ArgumentException($"The key type '{type}' is not supported.");
        }
    }

    internal static object GetComparer(object obj)
    {
        var type = obj.GetType();
        if (IsDictionary(type) == true)
        {
            var propertyName = nameof(Dictionary<object, object>.Comparer);
            var genericArguments = type.GetGenericArguments();
            var genericDictionaryType = typeof(Dictionary<,>).MakeGenericType(genericArguments);
            var propertyInfo = genericDictionaryType.GetProperty(propertyName);
            if (propertyInfo is null)
            {
                throw new InvalidOperationException($"The property '{propertyName}' is not found.");
            }

            return propertyInfo.GetValue(obj)!;
        }

        if (IsImmutableDictionary(type) == true)
        {
            var propertyName = nameof(ImmutableDictionary<object, object>.KeyComparer);
            var genericArguments = type.GetGenericArguments();
            var genericDictionaryType = typeof(ImmutableDictionary<,>).MakeGenericType(genericArguments);
            var propertyInfo = genericDictionaryType.GetProperty(propertyName);
            if (propertyInfo is null)
            {
                throw new InvalidOperationException($"The property '{propertyName}' is not found.");
            }

            return propertyInfo.GetValue(obj)!;
        }

        throw new NotSupportedException("The object is not a dictionary.");
    }

    private static (IKey, string) GetKey(IBencodeTypeContext typeContext, PropertyInfo propertyInfo, object obj, bool isSupportedByteArray)
    {
        var key = propertyInfo.GetValue(obj)!;
        var keyType = key.GetType();
        var isBinary = keyType == typeof(byte[]);
        if (isSupportedByteArray != true && isBinary == true)
        {
            throw new InvalidOperationException();
        }

        var typeDescriptor = new BencodeTypeDescriptor(propertyInfo, isBinary);
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
        var typeDescriptor = new BencodeTypeDescriptor(propertyInfo, isBinary: false);
        var valueConverter = typeContext.GetConverter(typeDescriptor, valueType);
        var v = valueConverter.ConvertFrom(typeContext, value);
        var t = valueType.AssemblyQualifiedName;
        return (v, t);
    }

    private static object GetKey(IBencodeTypeContext typeContext, DictionaryItemInfo itemInfo)
    {
        var keyType = GetType(itemInfo.KeyType);
        var isBinary = keyType == typeof(byte[]);
        var typeDescriptor = new BencodeTypeDescriptor(typeof(DictionaryInfo), isBinary);
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

    private static Type GetType(string assemblyQualifiedName)
    {
        if (Type.GetType(assemblyQualifiedName) is { } type)
        {
            return type;
        }

        throw new ArgumentException($"The type '{assemblyQualifiedName}' is not found.", nameof(assemblyQualifiedName));
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
