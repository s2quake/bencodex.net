using Bencodex.Types;

namespace Bencodex.Serialization;

public static class BencodeConvert
{
    public static IValue Serialize(object? obj)
    {
        if (obj != null)
        {
            var serializer = new BencodeSerializer();
            return serializer.Serialize(obj);
        }

        return Null.Value;
    }

    public static object? Deserialize(IValue value, Type? type)
    {
        if (type != null && type != typeof(Null))
        {
            var serializer = new BencodeSerializer();
            return serializer.Deserialize(value, type);
        }

        return null;
    }
}
