using System.Collections;
using System.Collections.Immutable;

namespace Bencodex.Serialization.Tests.General;

public class DictionaryTest
{
    public static IEnumerable<object[]> TestData =>
    [
        [nameof(FieldClass.Dictionary1), RandomUtility.Dictionary(isNullable: false)],
        [nameof(FieldClass.Dictionary2), RandomUtility.Dictionary()],
        [nameof(FieldClass.ImmtableDictionary1), RandomUtility.ImmutableDictionary(isNullable: false)],
        [nameof(FieldClass.ImmtableDictionary2), RandomUtility.ImmutableDictionary()],
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
        public Dictionary<object, object> Dictionary1
            = RandomUtility.Dictionary(isNullable: false)!;

        [Bencode]
        public Dictionary<object, object?> Dictionary2
            = RandomUtility.Dictionary();

        [Bencode]
        public ImmutableDictionary<object, object> ImmtableDictionary1
            = RandomUtility.ImmutableDictionary(isNullable: false)!;

        [Bencode]
        public ImmutableDictionary<object, object?> ImmtableDictionary2
            = RandomUtility.ImmutableDictionary();
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public Dictionary<object, object> Dictionary1 { get; set; }
            = RandomUtility.Dictionary(isNullable: false)!;

        [Bencode]
        public Dictionary<object, object?> Dictionary2 { get; set; }
            = RandomUtility.Dictionary();

        [Bencode]
        public ImmutableDictionary<object, object> ImmtableDictionary1 { get; set; }
            = RandomUtility.ImmutableDictionary(isNullable: false)!;

        [Bencode]
        public ImmutableDictionary<object, object?> ImmtableDictionary2 { get; set; }
            = RandomUtility.ImmutableDictionary();
    }
}
