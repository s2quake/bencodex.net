namespace Bencodex.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BencodeAttribute : BencodeAttributeBase
{
    public BencodeAttribute()
        : base(typeof(BencodeMemberDescriptor).AssemblyQualifiedName!)
    {
    }

    public BencodeAttribute(object key)
        : base(typeof(BencodeMemberDescriptor).AssemblyQualifiedName!, key)
    {
        Key = key;
    }
}
