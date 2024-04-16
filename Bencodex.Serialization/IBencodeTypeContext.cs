namespace Bencodex.Serialization;

public interface IBencodeTypeContext
{
    bool IsBinary { get; }

    BencodeConverter GetConverter(Type? declaringType, Type type);
}
