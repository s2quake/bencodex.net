using System.Collections;
using System.Numerics;

namespace Bencodex.Serialization.Tests.General;

public class IntegerArrayNullableTest
{
    public static object?[][] TestData =>
    [
        [nameof(FieldClass.BigInteger), RandomUtility.NullableArray(RandomUtility.BigInteger)],
        [nameof(FieldClass.Short), RandomUtility.NullableArray(RandomUtility.Int16)],
        [nameof(FieldClass.UShort), RandomUtility.NullableArray(RandomUtility.UInt16)],
        [nameof(FieldClass.Int), RandomUtility.NullableArray(RandomUtility.Int32)],
        [nameof(FieldClass.UInt), RandomUtility.NullableArray(RandomUtility.UInt32)],
        [nameof(FieldClass.Long), RandomUtility.NullableArray(RandomUtility.Int64)],
        [nameof(FieldClass.ULong), RandomUtility.NullableArray(RandomUtility.UInt64)],
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
        public BigInteger?[] BigInteger = [];

        [Bencode]
        public short?[] Short = [];

        [Bencode]
        public ushort?[] UShort = [];

        [Bencode]
        public int?[] Int = [];

        [Bencode]
        public uint?[] UInt = [];

        [Bencode]
        public long?[] Long = [];

        [Bencode]
        public ulong?[] ULong = [];
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public BigInteger?[] BigInteger { get; set; } = [];

        [Bencode]
        public short?[] Short { get; set; } = [];

        [Bencode]
        public ushort?[] UShort { get; set; } = [];

        [Bencode]
        public int?[] Int { get; set; } = [];

        [Bencode]
        public uint?[] UInt { get; set; } = [];

        [Bencode]
        public long?[] Long { get; set; } = [];

        [Bencode]
        public ulong?[] ULong { get; set; } = [];
    }
}
