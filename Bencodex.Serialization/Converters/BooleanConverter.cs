using Bencodex.Types;
using Boolean = Bencodex.Types.Boolean;

namespace Bencodex.Serialization.Converters;

internal sealed class BooleanConverter : BencodeConverter
{
    public static BooleanConverter Default { get; } = new BooleanConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (sourceType == typeof(bool))
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (destinationType == typeof(bool))
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (value is bool @bool)
        {
            return new Boolean(@bool);
        }

        throw new NotSupportedException();
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (destinationType == typeof(bool) && value is Boolean boolean)
        {
            return boolean.Value;
        }

        throw new NotSupportedException();
    }
}
