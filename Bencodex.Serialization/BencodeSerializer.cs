using Bencodex.Types;

namespace Bencodex.Serialization;

public class BencodeSerializer : IBencodeTypeContext
{
    public BencodeSerializer()
    {
    }

    bool IBencodeTypeContext.IsBinary => false;

    public IValue Serialize(object? obj)
    {
        if (obj is null)
        {
            return Null.Value;
        }
        else if (obj is IBencodable bencodable)
        {
            return bencodable.Bencoded;
        }
        else
        {
            var converter = GetConverter(declaringType: null, type: obj.GetType());
            return converter.ConvertFrom(this, obj);
        }
    }

    public object? Deserialize(IValue value, Type type)
    {
        if (value is Null)
        {
            return null;
        }
        else if (typeof(IBencodable).IsAssignableFrom(type) == true)
        {
            var types = new Type[] { typeof(IValue) };
            var parameters = new object?[] { value };
            var constructor = type.GetConstructor(types) ?? throw new InvalidOperationException(
                    $"The type '{type}' does not have a constructor that takes an IValue parameter.");
            return constructor.Invoke(parameters);
        }
        else
        {
            var converter = GetConverter(declaringType: null, type);
            return converter.ConvertTo(typeContext: this, value, destinationType: type);
        }
    }

    BencodeConverter IBencodeTypeContext.GetConverter(Type? declaringType, Type type)
        => GetConverter(declaringType, type);

    protected virtual BencodeConverter GetConverter(Type? declaringType, Type type)
        => BencodeBaseConverter.Default;
}
