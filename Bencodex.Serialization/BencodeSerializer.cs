using Bencodex.Serialization.Converters;
using Bencodex.Serialization.Utilities;
using Bencodex.Types;

namespace Bencodex.Serialization;

public class BencodeSerializer : IBencodeTypeContext, IBencodeTypeDescriptor
{
    public BencodeSerializer()
    {
    }

    bool IBencodeTypeDescriptor.IsBinary => false;

    Type IBencodeTypeDescriptor.OwnType => GetType();

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
            var converter = GetConverter(this, type: obj.GetType());
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
            var converter = GetConverter(this, type);
            return converter.ConvertTo(typeContext: this, value, destinationType: type);
        }
    }

    BencodeConverter IBencodeTypeContext.GetConverter(IBencodeTypeDescriptor typeDescriptor, Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        var memberType = underlyingType ?? type;
        var converter = GetConverter(typeDescriptor, memberType);

        if (underlyingType != null)
        {
            return new NullableConverter(converter);
        }

        return converter;
    }

    protected virtual BencodeConverter GetConverter(IBencodeTypeDescriptor typeDescriptor, Type type)
    {
        if (typeDescriptor.IsBinary == true)
        {
            return BinaryConverter.Default;
        }

        return ConverterUtility.GetConverter(type) ?? BencodeBaseConverter.Default;
    }
}
