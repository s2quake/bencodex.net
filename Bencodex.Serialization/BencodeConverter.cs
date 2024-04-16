using System.Numerics;
using Bencodex.Serialization.Converters;
using Bencodex.Types;

namespace Bencodex.Serialization;

public abstract class BencodeConverter
{
    // private static readonly BencodeConverterCollection Converters = new();
    // private static readonly Dictionary<BencodeConverter, BencodeConverter> NullableConverters = new();

    public virtual bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType) => false;

    public abstract IValue ConvertFrom(IBencodeTypeContext typeContext, object value);

    public virtual bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType) => false;

    public abstract object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType);

    public static BencodeConverter GetNullableConverter(BencodeConverter baseConverter)
    {
        return new NullableConverter(baseConverter);
    }
}
