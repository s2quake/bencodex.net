using System.Collections;

namespace Bencodex.Serialization.Tests.General;

public class TextArrayNullableTest
{
    public static object?[][] TestData =>
    [
        [nameof(FieldClass.Text), RandomUtility.NullableObjectArray(RandomUtility.String)],
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
        public float?[] Float
            = RandomUtility.NullableArray(RandomUtility.Single);

        [Bencode]
        public double?[] Double
            = RandomUtility.NullableArray(RandomUtility.Double);

        [Bencode]
        public decimal?[] Decimal
            = RandomUtility.NullableArray(RandomUtility.Decimal);

        [Bencode]
        public DateTime?[] DateTime
            = RandomUtility.NullableArray(RandomUtility.DateTime);

        [Bencode]
        public DateTimeOffset?[] DateTimeOffset
            = RandomUtility.NullableArray(RandomUtility.DateTimeOffset);

        [Bencode]
        public TimeSpan?[] TimeSpan
            = RandomUtility.NullableArray(RandomUtility.TimeSpan);

        [Bencode]
        public char?[] Char
            = RandomUtility.NullableArray(RandomUtility.Char);

        [Bencode]
        public string?[] Text
            = RandomUtility.NullableObjectArray(RandomUtility.String);
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public float?[] Float { get; set; }
            = RandomUtility.NullableArray(RandomUtility.Single);

        [Bencode]
        public double?[] Double { get; set; }
            = RandomUtility.NullableArray(RandomUtility.Double);

        [Bencode]
        public decimal?[] Decimal { get; set; }
            = RandomUtility.NullableArray(RandomUtility.Decimal);

        [Bencode]
        public DateTime?[] DateTime { get; set; }
            = RandomUtility.NullableArray(RandomUtility.DateTime);

        [Bencode]
        public DateTimeOffset?[] DateTimeOffset { get; set; }
            = RandomUtility.NullableArray(RandomUtility.DateTimeOffset);

        [Bencode]
        public TimeSpan?[] TimeSpan { get; set; }
            = RandomUtility.NullableArray(RandomUtility.TimeSpan);

        [Bencode]
        public char?[] Char { get; set; }
            = RandomUtility.NullableArray(RandomUtility.Char);

        [Bencode]
        public string?[] Text { get; set; }
            = RandomUtility.NullableObjectArray(RandomUtility.String);
    }
}
