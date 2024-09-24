using System.Collections.Immutable;
using System.Numerics;
using Bencodex.Serialization.Utilities;

namespace Bencodex.Serialization.Tests;

public static partial class RandomUtility
{
    public static Dictionary<TKey, TValue?> Dictionary<TKey, TValue>()
        where TKey : notnull
        => Dictionary<TKey, TValue?>(settings: new());

    public static Dictionary<TKey, TValue?> Dictionary<TKey, TValue>(DictionarySettings<TKey, TValue> settings)
        where TKey : notnull
    {
        using var depthScope = new DepthScope();
        var count = settings.Count;
        var comparer = settings.Comparer;
        var dictionary = new Dictionary<TKey, TValue?>(count, comparer);
        for (var i = 0; i < count; i++)
        {
            var key = GetKey<TKey>();
            if (dictionary.ContainsKey(key) == true)
            {
                continue;
            }

            dictionary[key] = GetValue<TKey, TValue>(settings);
        }

        return dictionary;
    }

    public static ImmutableDictionary<TKey, TValue?> ImmutableDictionary<TKey, TValue>()
        where TKey : notnull
        => ImmutableDictionary<TKey, TValue?>(settings: new());

    public static ImmutableDictionary<TKey, TValue?> ImmutableDictionary<TKey, TValue>(DictionarySettings<TKey, TValue> settings)
        where TKey : notnull
    {
        DictionaryUtility.VerifyKeyType(typeof(TKey));

        using var depthScope = new DepthScope();
        var count = settings.Count;
        var comparer = settings.Comparer;
        var dictionary = new Dictionary<TKey, TValue?>(count, comparer);
        for (var i = 0; i < count; i++)
        {
            var key = GetKey<TKey>();
            if (dictionary.ContainsKey(key) == true)
            {
                continue;
            }

            dictionary[key] = GetValue<TKey, TValue>(settings);
        }

        var builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<TKey, TValue?>(comparer);
        builder.AddRange(dictionary);
        return builder.ToImmutable();
    }

    private static TKey GetKey<TKey>()
        where TKey : notnull
    {
        if (typeof(TKey) == typeof(string))
        {
            return (TKey)(object)String();
        }

        if (typeof(TKey) == typeof(byte[]))
        {
            return (TKey)(object)Array(Byte);
        }

        if (typeof(TKey) == typeof(object))
        {
            return Boolean() ? (TKey)(object)String() : (TKey)(object)Array(Byte);
        }

        throw new NotSupportedException();
    }

    private static TValue GetValue<TValue>()
    {
        if (typeof(TValue) == typeof(bool))
        {
            return (TValue)(object)Boolean();
        }
        else if (typeof(TValue) == typeof(byte))
        {
            return (TValue)(object)Byte();
        }
        else if (typeof(TValue) == typeof(sbyte))
        {
            return (TValue)(object)SByte();
        }
        else if (typeof(TValue) == typeof(short))
        {
            return (TValue)(object)Int16();
        }
        else if (typeof(TValue) == typeof(ushort))
        {
            return (TValue)(object)UInt16();
        }
        else if (typeof(TValue) == typeof(int))
        {
            return (TValue)(object)Int32();
        }
        else if (typeof(TValue) == typeof(uint))
        {
            return (TValue)(object)UInt32();
        }
        else if (typeof(TValue) == typeof(long))
        {
            return (TValue)(object)Int64();
        }
        else if (typeof(TValue) == typeof(ulong))
        {
            return (TValue)(object)UInt64();
        }
        else if (typeof(TValue) == typeof(BigInteger))
        {
            return (TValue)(object)BigInteger();
        }
        else if (typeof(TValue) == typeof(float))
        {
            return (TValue)(object)Single();
        }
        else if (typeof(TValue) == typeof(double))
        {
            return (TValue)(object)Double();
        }
        else if (typeof(TValue) == typeof(decimal))
        {
            return (TValue)(object)Decimal();
        }
        else if (typeof(TValue) == typeof(char))
        {
            return (TValue)(object)Char();
        }
        else if (typeof(TValue) == typeof(string))
        {
            return (TValue)(object)Word();
        }
        else if (typeof(TValue) == typeof(DateTime))
        {
            return (TValue)(object)DateTime();
        }
        else if (typeof(TValue) == typeof(TimeSpan))
        {
            return (TValue)(object)TimeSpan();
        }
        else if (typeof(TValue) == typeof(DateTimeOffset))
        {
            return (TValue)(object)DateTimeOffset();
        }

        throw new NotSupportedException();
    }

    private static TValue? GetValue<TKey, TValue>(DictionarySettings<TKey, TValue> settings)
        where TKey : notnull
    {
        var isNull = Int32() % 10 == 0 && settings.IsNullable == true;
        if (isNull == true)
        {
            return default;
        }

        if (typeof(TValue) == typeof(object))
        {
            var r = DepthValue.Value < MaxDepth ? Int32(0, 100) : int.MaxValue;
            if (r < 5)
            {
                return (TValue)(object)Dictionary<TKey, TValue>(settings);
            }
            else if (r < 10)
            {
                return (TValue)(object)ImmutableDictionary<TKey, TValue>(settings);
            }
            else
            {
                var key = ValueByType.Keys.Random();
                return (TValue)(object)ValueByType[key]();
            }
        }
        else
        {
            return (TValue)(object)ValueByType[typeof(TValue)]();
        }
    }

    public record class DictionarySettings<TKey, TValue>
        where TKey : notnull
    {
        public int Count { get; init; } = 10;

        public bool IsNullable { get; init; } = true;

        public IEqualityComparer<TKey>? Comparer { get; init; }
    }
}
