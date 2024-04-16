namespace Bencodex.Serialization;

public sealed class BencodeTypeContext : IBencodeTypeContext
{
    private readonly IBencodeTypeContext _typeContext;

    public BencodeTypeContext(IBencodeTypeContext typeContext)
    {
        _typeContext = typeContext;
    }

    public BencodeTypeContext(IBencodeTypeContext typeContext, BencodeDescriptor descriptor)
    {
        _typeContext = typeContext;
        IsBinary = descriptor.IsBinary;
    }

    public bool IsBinary { get; }

    BencodeConverter IBencodeTypeContext.GetConverter(Type? declaringType, Type type)
        => _typeContext.GetConverter(declaringType, type);
}
