namespace Bencodex.Serialization.Tests.General;

public class TextTest
{
    public static object[][] TestData =>
    [
        [nameof(FieldClass.Float), RandomUtility.Single()],
        [nameof(FieldClass.Double), RandomUtility.Double()],
        [nameof(FieldClass.Decimal), RandomUtility.Decimal()],
        [nameof(FieldClass.DateTime), RandomUtility.DateTime()],
        [nameof(FieldClass.DateTimeOffset), RandomUtility.DateTimeOffset()],
        [nameof(FieldClass.TimeSpan), RandomUtility.TimeSpan()],
        [nameof(FieldClass.Char), RandomUtility.Char()],
        [nameof(FieldClass.Text), RandomUtility.String()],
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
            var value1 = descriptor.GetValue(instance1);
            var value2 = descriptor.GetValue(instance2);
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
            var value1 = descriptor.GetValue(instance1);
            var value2 = descriptor.GetValue(instance2);
            Assert.Equal(value1, value2);
        }
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetFieldTest(string name, object expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(FieldClass));
        var descriptor = descriptors[name];
        var instance1 = new FieldClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
        var actualValue1 = descriptor.GetValue(instance1);
        var actualValue2 = descriptor.GetValue(instance2);

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void SetPropertyTest(string name, object expectedValue)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(typeof(PropertyClass));
        var descriptor = descriptors[name];
        var instance1 = new PropertyClass();
        descriptor.SetValue(instance1, expectedValue);

        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
        var actualValue1 = descriptor.GetValue(instance1);
        var actualValue2 = descriptor.GetValue(instance2);

        Assert.Equal(expectedValue, actualValue2);
        Assert.Equal(actualValue1, actualValue2);
    }

    public sealed class FieldClass
    {
        [Bencode]
        public float Float
            = RandomUtility.Single();

        [Bencode]
        public double Double
            = RandomUtility.Double();

        [Bencode]
        public decimal Decimal
            = RandomUtility.Decimal();

        [Bencode]
        public DateTime DateTime
            = RandomUtility.DateTime();

        [Bencode]
        public DateTimeOffset DateTimeOffset
            = RandomUtility.DateTimeOffset();

        [Bencode]
        public TimeSpan TimeSpan
            = RandomUtility.TimeSpan();

        [Bencode]
        public char Char
            = RandomUtility.Char();

        [Bencode]
        public string Text
            = RandomUtility.String();
    }

    public sealed class PropertyClass
    {
        [Bencode]
        public float Float { get; set; }
            = RandomUtility.Single();

        [Bencode]
        public double Double { get; set; }
            = RandomUtility.Double();

        [Bencode]
        public decimal Decimal { get; set; }
            = RandomUtility.Decimal();

        [Bencode]
        public DateTime DateTime { get; set; }
            = RandomUtility.DateTime();

        [Bencode]
        public DateTimeOffset DateTimeOffset { get; set; }
            = RandomUtility.DateTimeOffset();

        [Bencode]
        public TimeSpan TimeSpan { get; set; }
            = RandomUtility.TimeSpan();

        [Bencode]
        public char Char { get; set; }
            = RandomUtility.Char();

        [Bencode]
        public string Text { get; set; }
            = RandomUtility.String();
    }
}
