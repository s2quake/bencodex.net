namespace Bencodex.Serialization;

public sealed class BencodeTypeContext : IBencodeTypeContext
{
    private readonly IBencodeTypeContext _typeContext;

    public BencodeTypeContext(IBencodeTypeContext typeContext, BencodeDescriptor descriptor)
    {
        _typeContext = typeContext;
        IsBinary = descriptor.IsBinary;
        OwnType = descriptor.DeclaringType;
    }

    public bool IsBinary { get; }

    public Type OwnType { get; }

    BencodeConverter IBencodeTypeContext.GetConverter(IBencodeTypeDescriptor typeDescriptor, Type type)
        => _typeContext.GetConverter(typeDescriptor, type);
}
