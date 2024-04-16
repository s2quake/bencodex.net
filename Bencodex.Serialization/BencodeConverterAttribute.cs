namespace Bencodex.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BencodeConverterAttribute : Attribute
{
    public BencodeConverterAttribute(Type converterType)
    {
        ConverterType = converterType;
    }

    public BencodeConverterAttribute(string converterTypeName)
    {
        ConverterType = Type.GetType(converterTypeName);
    }

    public Type? ConverterType { get; }
}
