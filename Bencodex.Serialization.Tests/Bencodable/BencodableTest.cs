using Bencodex.Types;

namespace Bencodex.Serialization.Tests.Bencodable;

public class BencodableTest
{
    [Fact]
    public void PropertyTest()
    {
        var instance1 = new BencodableClass();
        var serializer = new BencodeSerializer();
        var serializedValue = serializer.Serialize(instance1);
        var instance2 = (BencodableClass)serializer.Deserialize(serializedValue, typeof(BencodableClass))!;

        Assert.Equal(instance1.Value, instance2.Value);
    }

    public sealed class BencodableClass : IBencodable
    {
        public BencodableClass()
        {
        }

        public BencodableClass(IValue value)
        {
            if (value is Dictionary dictionary)
            {
                Value = (int)((Integer)dictionary[nameof(Value)]).Value;
            }
            else
            {
                throw new ArgumentException("Invalid value type", nameof(value));
            }
        }

        public int Value { get; set; } = RandomUtility.Int32();

        IValue IBencodable.Bencoded => Dictionary.Empty.Add(nameof(Value), Value);
    }
}
