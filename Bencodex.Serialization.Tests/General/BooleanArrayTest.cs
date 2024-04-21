using System.Collections;

namespace Bencodex.Serialization.Tests.General;

public class BooleanArrayTest
{
    public static object[][] TestData =>
    [
        [nameof(FieldClass.Boolean), RandomUtility.Array(RandomUtility.Boolean)],
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
        public bool[] Boolean = RandomUtility.Array(RandomUtility.Boolean);
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public bool[] Boolean { get; set; } = RandomUtility.Array(RandomUtility.Boolean);
    }
}
