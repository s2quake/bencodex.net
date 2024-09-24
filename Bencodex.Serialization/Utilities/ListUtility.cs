using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;

namespace Bencodex.Serialization.Utilities;

public static class ListUtility
{
    public static bool IsImmutableList(Type type)
        => IsImmutableList(type, out var _);

    public static bool IsImmutableList(Type type, [MaybeNullWhen(false)] out Type elementType)
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

    public static bool IsList(Type type)
        => IsList(type, out var _);

    public static bool IsList(Type type, [MaybeNullWhen(false)] out Type elementType)
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

    public static object ToImmutableList(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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
        var methodInfo = TypeUtility.GetCreateRangeMethod(typeof(ImmutableList), methodName, typeof(IEnumerable<>));
        var genericMethodInfo = methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }

    public static object ToList(IBencodeTypeContext typeContext, IValue obj, Type destinationType)
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
}
