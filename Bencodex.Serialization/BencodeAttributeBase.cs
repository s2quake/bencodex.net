namespace Bencodex.Serialization;

public abstract class BencodeAttributeBase : Attribute
{
    private readonly string _descriptorTypeName;

    protected BencodeAttributeBase(string descriptorTypeName)
    {
        _descriptorTypeName = descriptorTypeName;
    }

    protected BencodeAttributeBase(string descriptorTypeName, object key)
    {
        _descriptorTypeName = descriptorTypeName;
        Key = key;
    }

    public object Key { get; set; } = DBNull.Value;

    public bool IsBinary { get; set; }

    internal Type DescriptorType
        => Type.GetType(_descriptorTypeName) ??
            throw new InvalidOperationException($"'{_descriptorTypeName}' not found.");
}
