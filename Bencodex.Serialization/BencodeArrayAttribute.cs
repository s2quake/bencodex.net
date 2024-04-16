namespace Bencodex.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BencodeArrayAttribute : BencodeAttributeBase
{
    public BencodeArrayAttribute()
        : base(typeof(BencodeArrayMemberDescriptor).AssemblyQualifiedName!)
    {
    }

    public BencodeArrayAttribute(object key)
        : base(typeof(BencodeArrayMemberDescriptor).AssemblyQualifiedName!, key)
    {
        Key = key;
    }
}
