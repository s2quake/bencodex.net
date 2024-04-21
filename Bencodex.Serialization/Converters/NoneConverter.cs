using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class NoneConverter : BencodeConverter
{
    public static NoneConverter Defaula { get; } = new NoneConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
        => typeof(IValue).IsAssignableFrom(sourceType) == true;

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value) => value switch
    {
        IValue => (IValue)value,
        _ => throw new NotSupportedException(),
    };

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
        => typeof(IValue).IsAssignableFrom(destinationType) == true;

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (typeof(IValue).IsAssignableFrom(destinationType) == true)
        {
            return value;
        }

        throw new NotSupportedException();
    }
}
