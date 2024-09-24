using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;

namespace Bencodex.Serialization.Utilities;

public static class ArrayUtility
{
    public static bool IsArray(Type type)
        => IsArray(type, out var _);

    public static bool IsArray(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (typeof(Array).IsAssignableFrom(type) == true)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        elementType = null;
        return false;
    }

    public static bool IsImmutableArray(Type type)
        => IsImmutableArray(type, out var _);

    public static bool IsImmutableArray(Type type, [MaybeNullWhen(false)] out Type elementType)
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

    public static object ToArray(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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

    public static object ToImmutableArray(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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
        var methodInfo = TypeUtility.GetCreateRangeMethod(typeof(ImmutableArray), methodName, typeof(IEnumerable<>));
        var genericMethodInfo = methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }
}
