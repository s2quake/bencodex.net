using System.Text;
using Bencodex.Types;
using Binary = Bencodex.Types.Binary;

namespace Bencodex.Serialization.Converters;

internal sealed class BinaryConverter : BencodeConverter
{
    private static readonly Dictionary<Type, Func<object, byte[]>> ConvertFromByType = new()
    {
        [typeof(string)] = value => Encoding.UTF8.GetBytes((string)value),
        [typeof(float)] = value => BitConverter.GetBytes((float)value),
        [typeof(double)] = value => BitConverter.GetBytes((double)value),
        [typeof(decimal)] = value => decimal.GetBits((decimal)value).SelectMany(BitConverter.GetBytes).ToArray(),
        [typeof(char)] = value => BitConverter.GetBytes((char)value),
        [typeof(DateTime)] = value => BitConverter.GetBytes(((DateTime)value).ToBinary()),
        [typeof(DateTimeOffset)] = value => BitConverter.GetBytes(((DateTimeOffset)value).ToUnixTimeMilliseconds()),
        [typeof(TimeSpan)] = value => BitConverter.GetBytes(((TimeSpan)value).Ticks),
    };

    private static readonly Dictionary<Type, Func<byte[], object>> ConvertToByType = new()
    {
        [typeof(string)] = value => value,
        [typeof(float)] = value => BitConverter.ToSingle(value, 0),
        [typeof(double)] = value => BitConverter.ToDouble(value, 0),
        [typeof(decimal)] = value => new decimal(Enumerable.Range(0, value.Length / 4).Select(i => BitConverter.ToInt32(value, i * 4)).ToArray()),
        [typeof(char)] = value => BitConverter.ToChar(value, 0),
        [typeof(DateTime)] = value => DateTime.FromBinary(BitConverter.ToInt64(value, 0)),
        [typeof(DateTimeOffset)] = value => DateTimeOffset.FromUnixTimeMilliseconds(BitConverter.ToInt64(value, 0)),
        [typeof(TimeSpan)] = value => TimeSpan.FromTicks(BitConverter.ToInt64(value, 0)),
    };

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (ConvertFromByType.ContainsKey(sourceType) == true)
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (ConvertFromByType.TryGetValue(value.GetType(), out var converter) == true)
        {
            var byteArray = converter(value);
            return new Binary(byteArray);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (ConvertToByType.ContainsKey(destinationType) == true)
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (ConvertToByType.TryGetValue(destinationType, out var converter) == true &&
            value is Binary binary)
        {
            return converter(binary.ToByteArray());
        }

        throw new NotSupportedException();
    }
}
