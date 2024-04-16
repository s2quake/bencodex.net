namespace Bencodex.Serialization.Tests.General;

public class BinaryTest
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
            var value1 = descriptor.GetValue(instance1)!;
            var value2 = descriptor.GetValue(instance2)!;
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
        [Bencode(IsBinary = true)]
        public float Float
            = RandomUtility.Single();

        [Bencode(IsBinary = true)]
        public double Double
            = RandomUtility.Double();

        [Bencode(IsBinary = true)]
        public decimal Decimal
            = RandomUtility.Decimal();

        [Bencode(IsBinary = true)]
        public DateTime DateTime
            = RandomUtility.DateTime();

        [Bencode(IsBinary = true)]
        public DateTimeOffset DateTimeOffset
            = RandomUtility.DateTimeOffset();

        [Bencode(IsBinary = true)]
        public TimeSpan TimeSpan
            = RandomUtility.TimeSpan();

        [Bencode(IsBinary = true)]
        public char Char
            = RandomUtility.Char();

        [Bencode(IsBinary = true)]
        public string Text
            = RandomUtility.String();
    }

    public sealed class PropertyClass
    {
        [Bencode(IsBinary = true)]
        public float Float { get; set; }
            = RandomUtility.Single();

        [Bencode(IsBinary = true)]
        public double Double { get; set; }
            = RandomUtility.Double();

        [Bencode(IsBinary = true)]
        public decimal Decimal { get; set; }
            = RandomUtility.Decimal();

        [Bencode(IsBinary = true)]
        public DateTime DateTime { get; set; }
            = RandomUtility.DateTime();

        [Bencode(IsBinary = true)]
        public DateTimeOffset DateTimeOffset { get; set; }
            = RandomUtility.DateTimeOffset();

        [Bencode(IsBinary = true)]
        public TimeSpan TimeSpan { get; set; }
            = RandomUtility.TimeSpan();

        [Bencode(IsBinary = true)]
        public char Char { get; set; }
            = RandomUtility.Char();

        [Bencode(IsBinary = true)]
        public string Text { get; set; }
            = RandomUtility.String();
    }
}
