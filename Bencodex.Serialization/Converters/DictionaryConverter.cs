using Bencodex.Types;
using static Bencodex.Serialization.Utilities.DictionaryUtility;

namespace Bencodex.Serialization.Converters;

internal sealed class DictionaryConverter : BencodeConverter
{
    public static DictionaryConverter Default { get; } = new DictionaryConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (IsImmutableDictionary(sourceType) ||
            IsDictionary(sourceType))
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        var sourceType = value.GetType();
        if (IsImmutableDictionary(sourceType) ||
            IsDictionary(sourceType))
        {
            return ToBencodeDictionary(typeContext, value);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (IsImmutableDictionary(destinationType) ||
            IsDictionary(destinationType))
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is not Dictionary dictionary)
        {
            throw new NotSupportedException();
        }

        if (IsImmutableDictionary(destinationType) == true)
        {
            return ToImmutableDictionary(typeContext, dictionary, destinationType);
        }
        else if (IsDictionary(destinationType) == true)
        {
            return ToDictionary(typeContext, dictionary, destinationType);
        }

        throw new NotSupportedException();
    }
}
