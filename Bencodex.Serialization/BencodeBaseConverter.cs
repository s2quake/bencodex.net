using Bencodex.Types;

namespace Bencodex.Serialization;

internal sealed class BencodeBaseConverter : BencodeConverter
{
    public static BencodeBaseConverter Default { get; } = new BencodeBaseConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
        => true;

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        var descriptors = BencodeDescriptor.GetDescriptors(value);
        var keyValueList = new List<KeyValuePair<IKey, IValue>>(descriptors.Count);
        foreach (var descriptor in descriptors)
        {
            var memberValue = descriptor.GetValue(value);
            var k = descriptor.Key;
            var v = descriptor.SerilizeValue(typeContext, memberValue);
            keyValueList.Add(new KeyValuePair<IKey, IValue>(k, v));
        }

        return new Dictionary(keyValueList);
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
        => true;

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is Dictionary dictionary)
        {
            var obj = Activator.CreateInstance(destinationType)!;
            var descriptors = BencodeDescriptor.GetDescriptors(destinationType);
            foreach (var descriptor in descriptors)
            {
                if (dictionary.TryGetValue(descriptor.Key, out var itemValue) == true)
                {
                    var value1 = descriptor.GetValue(obj);
                    var value2 = descriptor.DeserializeValue(typeContext, itemValue);
                    if (Equals(value1, value2) != true)
                    {
                        descriptor.SetValue(obj, value2);
                    }
                }
            }

            return obj;
        }

        throw new NotSupportedException();
    }
}
