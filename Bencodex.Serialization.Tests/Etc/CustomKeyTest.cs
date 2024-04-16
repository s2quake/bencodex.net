using Bencodex.Types;

namespace Bencodex.Serialization.Tests.Etc;

public class CustomKeyTest
{
    [Fact]
    public void KeyTest()
    {
        var instance = new TestClass();
        var serializer = new BencodeSerializer();
        var serializedValue = (Dictionary)serializer.Serialize(instance);

        Assert.Equal(instance.TextKeyField, GetString(serializedValue["_text"]));
        Assert.Equal(instance.TextKeyProperty, GetString(serializedValue["Text"]));
        Assert.Equal(instance.BinaryKeyField, GetString(serializedValue[new Binary("key_field"u8.ToArray())]));
        Assert.Equal(instance.BinaryKeyProperty, GetString(serializedValue[new Binary("key_property"u8.ToArray())]));

        static string GetString(IValue value)
        {
            if (value is Text text)
            {
                return text.Value;
            }

            throw new NotSupportedException();
        }
    }

    public sealed class TestClass
    {
        [Bencode("_text")]
        public string TextKeyField = RandomUtility.String();

        [Bencode(new byte[] { 0x6b, 0x65, 0x79, 0x5f, 0x66, 0x69, 0x65, 0x6c, 0x64, })]
        public string BinaryKeyField = RandomUtility.String();

        [Bencode("Text")]
        public string TextKeyProperty { get; set; } = RandomUtility.String();

        [Bencode(new byte[] { 0x6b, 0x65, 0x79, 0x5f, 0x70, 0x72, 0x6f, 0x70, 0x65, 0x72, 0x74, 0x79, })]
        public string BinaryKeyProperty { get; set; } = RandomUtility.String();
    }
}
