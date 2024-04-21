namespace Bencodex.Serialization;

public interface IBencodeTypeContext
{
    BencodeConverter GetConverter(IBencodeTypeDescriptor typeDescriptor, Type type);
}
