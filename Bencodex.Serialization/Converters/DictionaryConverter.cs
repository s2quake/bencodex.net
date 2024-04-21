using System.Collections.Immutable;
using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class DictionaryConverter : BencodeConverter
{
    public static DictionaryConverter Default { get; } = new DictionaryConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (typeof(ImmutableDictionary<object, object>) == sourceType ||
            typeof(ImmutableDictionary<object, object?>) == sourceType ||
            typeof(Dictionary<object, object>) == sourceType ||
            typeof(Dictionary<object, object?>) == sourceType)
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        var destinationType = value.GetType();
        if (typeof(ImmutableDictionary<object, object>) == destinationType ||
            typeof(ImmutableDictionary<object, object?>) == destinationType ||
            typeof(Dictionary<object, object>) == destinationType ||
            typeof(Dictionary<object, object?>) == destinationType)
        {
            return BencodeUtility.ToBencodeDictionary(typeContext, value);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (typeof(ImmutableDictionary<object, object>) == destinationType ||
            typeof(ImmutableDictionary<object, object?>) == destinationType ||
            typeof(Dictionary<object, object>) == destinationType ||
            typeof(Dictionary<object, object?>) == destinationType)
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is Dictionary dictionary)
        {
            if (typeof(ImmutableDictionary<object, object>) == destinationType ||
                typeof(ImmutableDictionary<object, object?>) == destinationType)
            {
                return BencodeUtility.ToImmutableDictionary(typeContext, dictionary, destinationType);
            }
            else if (typeof(Dictionary<object, object>) == destinationType ||
                    typeof(Dictionary<object, object?>) == destinationType)
            {
                return BencodeUtility.ToDictionary(typeContext, dictionary, destinationType);
            }
        }

        throw new NotSupportedException();
    }
}
