using System.Numerics;
using Bencodex.Serialization.Converters;
using static Bencodex.Serialization.Utilities.ArrayUtility;
using static Bencodex.Serialization.Utilities.DictionaryUtility;
using static Bencodex.Serialization.Utilities.ListUtility;

namespace Bencodex.Serialization.Utilities;

public static class ConverterUtility
{
    public static readonly Dictionary<Type, BencodeConverter> ConverterByType = new()
    {
        { typeof(bool), BooleanConverter.Default },
        { typeof(byte), IntegerConverter.Default },
        { typeof(sbyte), IntegerConverter.Default },
        { typeof(short), IntegerConverter.Default },
        { typeof(ushort), IntegerConverter.Default },
        { typeof(int), IntegerConverter.Default },
        { typeof(uint), IntegerConverter.Default },
        { typeof(long), IntegerConverter.Default },
        { typeof(ulong), IntegerConverter.Default },
        { typeof(BigInteger), IntegerConverter.Default },
        { typeof(float), TextConverter.Default },
        { typeof(double), TextConverter.Default },
        { typeof(decimal), TextConverter.Default },
        { typeof(char), TextConverter.Default },
        { typeof(string), TextConverter.Default },
        { typeof(DateTime), TextConverter.Default },
        { typeof(TimeSpan), TextConverter.Default },
        { typeof(DateTimeOffset), TextConverter.Default },
    };

    public static Type[] SupportedTypes { get; } = [.. ConverterByType.Keys];

    public static BencodeConverter? GetConverter(Type type)
    {
        if (IsArray(type) == true)
        {
            return ListConverter.Default;
        }

        if (ConverterByType.ContainsKey(type) == true)
        {
            return ConverterByType[type];
        }

        if (IsDictionary(type) == true)
        {
            return DictionaryConverter.Default;
        }

        if (IsImmutableDictionary(type) == true)
        {
            return DictionaryConverter.Default;
        }

        if (IsImmutableArray(type) == true)
        {
            return ListConverter.Default;
        }

        if (IsImmutableList(type) == true)
        {
            return ListConverter.Default;
        }

        if (IsList(type) == true)
        {
            return ListConverter.Default;
        }

        return null;
    }
}
