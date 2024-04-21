using Bencodex.Types;

namespace Bencodex.Serialization;

public abstract class BencodeConverter
{
    public virtual bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType) => false;

    public abstract IValue ConvertFrom(IBencodeTypeContext typeContext, object value);

    public virtual bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType) => false;

    public abstract object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType);
}
