namespace Bencodex.Serialization;

public interface IBencodeTypeDescriptor
{
    bool IsBinary { get; }

    Type? OwnType { get; }
}
