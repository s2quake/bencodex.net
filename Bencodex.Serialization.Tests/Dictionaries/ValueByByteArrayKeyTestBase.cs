using System.Collections;
using System.Collections.Immutable;

namespace Bencodex.Serialization.Tests.Dictionaries;

public abstract class ValueByByteArrayKeyTestBase<TValue>
{
    private static readonly RandomUtility.DictionarySettings<byte[], TValue> Settings
        = new() { IsNullable = false, Comparer = ByteArrayEqualityComparer.Default };

    public static IEnumerable<object[]> TestData =>
    [
        [nameof(FieldClass.Dictionary), RandomUtility.Dictionary<byte[], TValue>(Settings)],
        [nameof(FieldClass.ImmtableDictionary), RandomUtility.ImmutableDictionary<byte[], TValue>(Settings)],
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
            var value1 = (IReadOnlyDictionary<byte[], TValue>)descriptor.GetValue(instance1)!;
            var value2 = (IReadOnlyDictionary<byte[], TValue>)descriptor.GetValue(instance2)!;
            var key1 = value1.Keys;
            var key2 = value2.Keys;
            Assert.Equal(key1, key2, ByteArrayEqualityComparer.Default);
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
    public void SetFieldTest(string name, IEnumerable expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(FieldClass));
        var descriptor = descriptors[name];
        var instance1 = new FieldClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var actualValue1 = (IEnumerable)descriptor.GetValue(instance1)!;
        var actualValue2 = (IEnumerable)descriptor.GetValue(instance2)!;

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetPropertyTest(string name, IEnumerable expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(PropertyClass));
        var descriptor = descriptors[name];
        var instance1 = new PropertyClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var actualValue1 = (IEnumerable)descriptor.GetValue(instance1)!;
        var actualValue2 = (IEnumerable)descriptor.GetValue(instance2)!;

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    public sealed class FieldClass
    {
        [Bencode]
        public Dictionary<byte[], TValue> Dictionary
            = RandomUtility.Dictionary<byte[], TValue>(Settings)!;

        [Bencode]
        public ImmutableDictionary<byte[], TValue> ImmtableDictionary
            = RandomUtility.ImmutableDictionary<byte[], TValue>(Settings)!;
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public Dictionary<byte[], TValue> Dictionary { get; set; }
            = RandomUtility.Dictionary<byte[], TValue>(Settings)!;

        [Bencode]
        public ImmutableDictionary<byte[], TValue> ImmtableDictionary { get; set; }
            = RandomUtility.ImmutableDictionary<byte[], TValue>(Settings)!;
    }
}
