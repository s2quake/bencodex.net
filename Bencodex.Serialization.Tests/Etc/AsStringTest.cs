using System.Numerics;
using Bencodex.Types;

namespace Bencodex.Serialization.Tests.Etc;

public class AsStringTest
{
    [Fact]
    public void FieldToTextTest()
    {
        var instance1 = new FieldClass();
        var serializer = new CustomSerializer();
        var serializedValue = (Dictionary)serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var descriptors = BencodeDescriptor.GetDescriptors(instance1);
        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            var value1 = descriptor.GetValue(instance1)!;
            var value2 = descriptor.GetValue(instance2)!;
            Assert.Equal(value1, value2);
        }

        Assert.IsType<Text>(serializedValue[nameof(FieldClass.BigInteger)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.Short)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.UShort)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.Int)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.UInt)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.Long)]);
        Assert.IsType<Text>(serializedValue[nameof(FieldClass.ULong)]);
    }

    [Fact]
    public void PropertyToTextTest()
    {
        var instance1 = new PropertyClass();
        var serializer = new CustomSerializer();
        var serializedValue = (Dictionary)serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var descriptors = BencodeDescriptor.GetDescriptors(instance1);
        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            var value1 = descriptor.GetValue(instance1)!;
            var value2 = descriptor.GetValue(instance2)!;
            Assert.Equal(value1, value2);
        }

        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.BigInteger)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.Short)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.UShort)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.Int)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.UInt)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.Long)]);
        Assert.IsType<Text>(serializedValue[nameof(PropertyClass.ULong)]);
    }

    public sealed class CustomSerializer : BencodeSerializer
    {
        public CustomSerializer()
        {
        }

        protected override BencodeConverter GetConverter(Type? declaringType, Type type)
        {
            if (type == typeof(BigInteger) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong))
            {
                return CustomConverter.Default;
            }

            return base.GetConverter(declaringType, type);
        }
    }

    public sealed class CustomConverter : BencodeConverter
    {
        public static CustomConverter Default { get; } = new CustomConverter();

        public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
        {
            if (sourceType == typeof(BigInteger) ||
                sourceType == typeof(short) ||
                sourceType == typeof(ushort) ||
                sourceType == typeof(int) ||
                sourceType == typeof(uint) ||
                sourceType == typeof(long) ||
                sourceType == typeof(ulong))
            {
                return true;
            }

            return base.CanConvertFrom(typeContext, sourceType);
        }

        public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
        {
            return new Text(GetText(value));

            static string GetText(object value) => value switch
            {
                BigInteger bigInteger => $"{bigInteger:R}",
                short @short => $"{@short}",
                ushort @ushort => $"{@ushort}",
                int @int => $"{@int}",
                uint @uint => $"{@uint}",
                long @long => $"{@long}",
                ulong @ulong => $"{@ulong}",
                _ => throw new NotSupportedException(),
            };
        }

        public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
        {
            if (destinationType == typeof(BigInteger) ||
                destinationType == typeof(short) ||
                destinationType == typeof(ushort) ||
                destinationType == typeof(int) ||
                destinationType == typeof(uint) ||
                destinationType == typeof(long) ||
                destinationType == typeof(ulong))
            {
                return true;
            }

            return base.CanConvertTo(typeContext, destinationType);
        }

        public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
        {
            if (value is Text text)
            {
                if (destinationType == typeof(BigInteger))
                {
                    return BigInteger.Parse(text.Value);
                }
                else if (destinationType == typeof(short))
                {
                    return short.Parse(text.Value);
                }
                else if (destinationType == typeof(ushort))
                {
                    return ushort.Parse(text.Value);
                }
                else if (destinationType == typeof(int))
                {
                    return int.Parse(text.Value);
                }
                else if (destinationType == typeof(uint))
                {
                    return uint.Parse(text.Value);
                }
                else if (destinationType == typeof(long))
                {
                    return long.Parse(text.Value);
                }
                else if (destinationType == typeof(ulong))
                {
                    return ulong.Parse(text.Value);
                }
            }

            throw new NotSupportedException();
        }
    }

    public sealed class FieldClass
    {
        [Bencode]
        public BigInteger BigInteger = RandomUtility.BigInteger();

        [Bencode]
        public short Short = RandomUtility.Int16();

        [Bencode]
        public ushort UShort = RandomUtility.UInt16();

        [Bencode]
        public int Int = RandomUtility.Int32();

        [Bencode]
        public uint UInt = RandomUtility.UInt32();

        [Bencode]
        public long Long = RandomUtility.Int64();

        [Bencode]
        public ulong ULong = RandomUtility.UInt64();
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public BigInteger BigInteger { get; set; } = RandomUtility.BigInteger();

        [Bencode]
        public short Short { get; set; } = RandomUtility.Int16();

        [Bencode]
        public ushort UShort { get; set; } = RandomUtility.UInt16();

        [Bencode]
        public int Int { get; set; } = RandomUtility.UInt16();

        [Bencode]
        public uint UInt { get; set; } = RandomUtility.UInt32();

        [Bencode]
        public long Long { get; set; } = RandomUtility.Int64();

        [Bencode]
        public ulong ULong { get; set; } = RandomUtility.UInt64();
    }
}
