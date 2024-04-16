using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

public sealed class NullableConverter : BencodeConverter
{
    private readonly BencodeConverter _baseConverter;

    public NullableConverter(BencodeConverter baseConverter)
    {
        _baseConverter = baseConverter;
    }

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (Nullable.GetUnderlyingType(sourceType) is { } underlyingType)
        {
            return _baseConverter.CanConvertFrom(typeContext, underlyingType);
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (value is null)
        {
            return Null.Value;
        }
        else
        {
            return _baseConverter.ConvertFrom(typeContext, value);
        }
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (Nullable.GetUnderlyingType(destinationType) is { } underlyingType)
        {
            return _baseConverter.CanConvertTo(typeContext, underlyingType);
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is Null)
        {
            return null!;
        }
        else if (Nullable.GetUnderlyingType(destinationType) is { } underlyingType)
        {
            return _baseConverter.ConvertTo(typeContext, value, underlyingType);
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}
