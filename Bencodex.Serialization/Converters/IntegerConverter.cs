using System.Reflection;
using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class IntegerConverter : BencodeConverter
{
    private static readonly Dictionary<Type, MethodInfo> ImplicitMethodByType;
    private static readonly Dictionary<Type, ConstructorInfo> ConstructorByType;

    static IntegerConverter()
    {
        ImplicitMethodByType = typeof(Integer).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(item => item.Name == "op_Implicit")
            .Where(item => item.GetParameters().Length == 1 && item.GetParameters()[0].ParameterType == typeof(Integer))
            .ToDictionary(item => item.ReturnType, item => item);
        ConstructorByType = typeof(Integer).GetConstructors()
            .Where(item => item.GetParameters().Length == 1)
            .ToDictionary(item => item.GetParameters()[0].ParameterType, item => item);
    }

    public static IntegerConverter Default { get; } = new IntegerConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (ConstructorByType.ContainsKey(sourceType) == true)
        {
            return true;
        }

        if (sourceType == typeof(byte) ||
            sourceType == typeof(sbyte))
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (ConstructorByType.TryGetValue(value.GetType(), out var constructor))
        {
            return (IValue)constructor.Invoke([value]);
        }

        if (value is byte @byte)
        {
            return new Integer(@byte);
        }

        if (value is sbyte @sbyte)
        {
            return new Integer(@sbyte);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (ImplicitMethodByType.ContainsKey(destinationType))
        {
            return true;
        }

        if (destinationType == typeof(byte) ||
            destinationType == typeof(sbyte))
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (ImplicitMethodByType.TryGetValue(destinationType, out var method))
        {
            return method.Invoke(null, [value])!;
        }

        if (value is Integer integer)
        {
            if (destinationType == typeof(byte))
            {
                return (byte)integer.Value;
            }

            if (destinationType == typeof(sbyte))
            {
                return (sbyte)integer.Value;
            }
        }

        throw new NotSupportedException();
    }
}
