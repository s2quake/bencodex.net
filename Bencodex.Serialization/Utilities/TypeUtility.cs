using System.Reflection;
using Bencodex.Types;

namespace Bencodex.Serialization.Utilities;

public static class TypeUtility
{
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
        if (ArrayUtility.IsArray(type) == true)
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

        if (ConverterUtility.SupportedTypes.Contains(type) == true)
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

        if (ConverterUtility.SupportedTypes.Contains(type) != true)
        {
            throw new NotSupportedException($"Unsupported type: '{type}'.");
        }
    }

    public static bool IsStaticClass(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) == null && type.IsAbstract == true && type.IsSealed == true;
    }

    internal static MethodInfo GetCreateRangeMethod(Type type, string methodName, Type parameterType)
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
}
