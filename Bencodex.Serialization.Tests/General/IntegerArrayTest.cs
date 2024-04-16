using System.Collections;
using System.Numerics;

namespace Bencodex.Serialization.Tests.General;

public class IntegerArrayTest
{
    public static object[][] TestData =>
    [
        [nameof(FieldClass.BigInteger), RandomUtility.Array(RandomUtility.BigInteger)],
        [nameof(FieldClass.Short), RandomUtility.Array(RandomUtility.Int16)],
        [nameof(FieldClass.UShort), RandomUtility.Array(RandomUtility.UInt16)],
        [nameof(FieldClass.Int), RandomUtility.Array(RandomUtility.Int32)],
        [nameof(FieldClass.UInt), RandomUtility.Array(RandomUtility.UInt32)],
        [nameof(FieldClass.Long), RandomUtility.Array(RandomUtility.Int64)],
        [nameof(FieldClass.ULong), RandomUtility.Array(RandomUtility.UInt64)],
    ];

    [Fact]
    public void FieldTest()
    {
        var instance1 = new FieldClass();
        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var descriptors = BencodeDescriptor.GetDescriptors(instance1);
        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            var value1 = (IEnumerable)descriptor.GetValue(instance1)!;
            var value2 = (IEnumerable)descriptor.GetValue(instance2)!;
            Assert.Equal(value1, value2);
        }
    }

    [Fact]
    public void PropertyTest()
    {
        var instance1 = new PropertyClass();
        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var descriptors = BencodeDescriptor.GetDescriptors(instance1);
        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            var value1 = (IEnumerable)descriptor.GetValue(instance1)!;
            var value2 = (IEnumerable)descriptor.GetValue(instance2)!;
            Assert.Equal(value1, value2);
        }
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetFieldTest(string name, Array expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(FieldClass));
        var descriptor = descriptors[name];
        var instance1 = new FieldClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var actualValue1 = (Array)descriptor.GetValue(instance1)!;
        var actualValue2 = (Array)descriptor.GetValue(instance2)!;

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetPropertyTest(string name, Array expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(PropertyClass));
        var descriptor = descriptors[name];
        var instance1 = new PropertyClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var actualValue1 = (Array)descriptor.GetValue(instance1)!;
        var actualValue2 = (Array)descriptor.GetValue(instance2)!;

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    public sealed class FieldClass
    {
        [Bencode]
        public BigInteger[] BigInteger = RandomUtility.Array(RandomUtility.BigInteger);

        [Bencode]
        public char[] Char = RandomUtility.Array(RandomUtility.Char);

        [Bencode]
        public byte[] Byte = RandomUtility.Array(RandomUtility.Byte);

        [Bencode]
        public sbyte[] SByte = RandomUtility.Array(RandomUtility.SByte);

        [Bencode]
        public short[] Short = RandomUtility.Array(RandomUtility.Int16);

        [Bencode]
        public ushort[] UShort = RandomUtility.Array(RandomUtility.UInt16);

        [Bencode]
        public int[] Int = RandomUtility.Array(RandomUtility.Int32);

        [Bencode]
        public uint[] UInt = RandomUtility.Array(RandomUtility.UInt32);

        [Bencode]
        public long[] Long = RandomUtility.Array(RandomUtility.Int64);

        [Bencode]
        public ulong[] ULong = RandomUtility.Array(RandomUtility.UInt64);
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public BigInteger[] BigInteger { get; set; } = RandomUtility.Array(RandomUtility.BigInteger);

        [Bencode]
        public char[] Char { get; set; } = RandomUtility.Array(RandomUtility.Char);

        [Bencode]
        public byte[] Byte { get; set; } = RandomUtility.Array(RandomUtility.Byte);

        [Bencode]
        public sbyte[] SByte { get; set; } = RandomUtility.Array(RandomUtility.SByte);

        [Bencode]
        public short[] Short { get; set; } = RandomUtility.Array(RandomUtility.Int16);

        [Bencode]
        public ushort[] UShort { get; set; } = RandomUtility.Array(RandomUtility.UInt16);

        [Bencode]
        public int[] Int { get; set; } = RandomUtility.Array(RandomUtility.Int32);

        [Bencode]
        public uint[] UInt { get; set; } = RandomUtility.Array(RandomUtility.UInt32);

        [Bencode]
        public long[] Long { get; set; } = RandomUtility.Array(RandomUtility.Int64);

        [Bencode]
        public ulong[] ULong { get; set; } = RandomUtility.Array(RandomUtility.UInt64);
    }
}
