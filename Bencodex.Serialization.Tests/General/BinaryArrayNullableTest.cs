using System.Collections;
using System.Collections.Immutable;

namespace Bencodex.Serialization.Tests.General;

public class BinaryArrayNullableTest
{
    public static object?[][] TestData =>
    [
        [
            nameof(FieldClass.Items),
            RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.Array(RandomUtility.Byte))),
        ],
        [
            nameof(FieldClass.ImmutableArray),
            RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableArray(RandomUtility.Byte))),
        ],
        [
            nameof(FieldClass.ImmutableList),
            RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableList(RandomUtility.Byte))),
        ],
        [
            nameof(FieldClass.List),
            RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.List(RandomUtility.Byte))),
        ],
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
    public void SetFieldTest(string name, Array? expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(FieldClass));
        var descriptor = descriptors[name];
        var instance1 = new FieldClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var actualValue1 = (Array?)descriptor.GetValue(instance1);
        var actualValue2 = (Array?)descriptor.GetValue(instance2);

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetPropertyTest(string name, Array? expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(PropertyClass));
        var descriptor = descriptors[name];
        var instance1 = new PropertyClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var actualValue1 = (Array?)descriptor.GetValue(instance1);
        var actualValue2 = (Array?)descriptor.GetValue(instance2);

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    public sealed class FieldClass
    {
        [Bencode]
        public byte[][]? Items
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.Array(RandomUtility.Byte)));

        [Bencode]
        public ImmutableArray<byte>[]? ImmutableArray
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableArray(RandomUtility.Byte)));

        [Bencode]
        public ImmutableList<byte>[]? ImmutableList
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableList(RandomUtility.Byte)));

        [Bencode]
        public List<byte>[]? List
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.List(RandomUtility.Byte)));
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public byte[][]? Items { get; set; }
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.Array(RandomUtility.Byte)));

        [Bencode]
        public ImmutableArray<byte>[]? ImmutableArray { get; set; }
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableArray(RandomUtility.Byte)));

        [Bencode]
        public ImmutableList<byte>[]? ImmutableList { get; set; }
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.ImmutableList(RandomUtility.Byte)));

        [Bencode]
        public List<byte>[]? List { get; set; }
            = RandomUtility.NullableObject(() => RandomUtility.Array(() => RandomUtility.List(RandomUtility.Byte)));
    }
}
