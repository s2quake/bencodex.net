using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Bencodex.Serialization.Tests;

public static partial class RandomUtility
{
    public const int AttemptCount = 10;
    public const int MaxDepth = 3;

    private static readonly string[] Words;

    private static readonly Dictionary<Type, Func<object>> ValueByType = new()
    {
        { typeof(bool), () => Boolean() },
        { typeof(byte), () => Byte() },
        { typeof(sbyte), () => SByte() },
        { typeof(short), () => Int16() },
        { typeof(ushort), () => UInt16() },
        { typeof(int), () => Int32() },
        { typeof(uint), () => UInt32() },
        { typeof(long), () => Int64() },
        { typeof(ulong), () => UInt64() },
        { typeof(BigInteger), () => BigInteger() },
        { typeof(float), () => Single() },
        { typeof(double), () => Double() },
        { typeof(decimal), () => Decimal() },
        { typeof(char), () => Char() },
        { typeof(string), () => String() },
        { typeof(DateTime), () => DateTime() },
        { typeof(TimeSpan), () => TimeSpan() },
        { typeof(DateTimeOffset), () => DateTimeOffset() },
    };

    private static readonly ThreadLocal<int> DepthValue = new();

    static RandomUtility()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = string.Join(".", typeof(RandomUtility).Namespace, "Resources", "words.txt");
        var resourceStream = assembly.GetManifestResourceStream(resourceName)!;
        using var stream = new StreamReader(resourceStream);
        var text = stream.ReadToEnd();
        var i = 0;
        using var sr1 = new StringReader(text);
        while (sr1.ReadLine() is string line1)
        {
            i++;
        }

        Words = new string[i];
        i = 0;
        using var sr2 = new StringReader(text);
        while (sr2.ReadLine() is string line2)
        {
            Words[i++] = line2;
        }
    }

    public static sbyte SByte()
    {
        var bytes = new byte[1];
        System.Random.Shared.NextBytes(bytes);
        return (sbyte)bytes[0];
    }

    public static byte Byte()
    {
        var bytes = new byte[1];
        System.Random.Shared.NextBytes(bytes);
        return bytes[0];
    }

    public static short Int16()
    {
        var bytes = new byte[2];
        System.Random.Shared.NextBytes(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    public static ushort UInt16()
    {
        var bytes = new byte[2];
        System.Random.Shared.NextBytes(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public static int Int32() => Int32(int.MinValue, int.MaxValue);

    public static int Int32(int minValue, int maxValue)
    {
        return System.Random.Shared.Next(minValue, maxValue);
    }

    public static uint UInt32()
    {
        var bytes = new byte[4];
        System.Random.Shared.NextBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static long Int64()
    {
        var bytes = new byte[8];
        System.Random.Shared.NextBytes(bytes);
        return BitConverter.ToInt64(bytes, 0);
    }

    public static ulong UInt64()
    {
        var bytes = new byte[8];
        System.Random.Shared.NextBytes(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static float Single()
    {
#if NET6_0_OR_GREATER
        return System.Random.Shared.NextSingle();
#else
        return (float)System.Random.Shared.NextDouble() * System.Random.Shared.Next();
#endif
    }

    public static double Double() => System.Random.Shared.NextDouble();

    public static decimal Decimal() => (decimal)System.Random.Shared.NextDouble();

    public static BigInteger BigInteger() => new BigInteger(System.Random.Shared.NextInt64());

    public static string Word() => Word(item => true);

    public static string Word(Func<string, bool> predicate)
    {
        for (var i = 0; i < AttemptCount; i++)
        {
            var index = Int32(0, Words.Length);
            var item = Words[index];
            if (predicate(item) == true)
            {
                return item;
            }
        }

        throw new InvalidOperationException("No value was found that matches the condition.");
    }

    public static char Char() => (char)UInt16();

    public static DateTime DateTime()
    {
        var minValue = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var maxValue = new DateTime(2050, 12, 31, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var value = System.Random.Shared.NextInt64(minValue, maxValue) / 10000000L * 10000000L;
        return new DateTime(value, DateTimeKind.Utc);
    }

    public static DateTimeOffset DateTimeOffset()
    {
        var minValue = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var maxValue = new DateTime(2050, 12, 31, 0, 0, 0, DateTimeKind.Utc).Ticks;
        var value = System.Random.Shared.NextInt64(minValue, maxValue) / 10000000L * 10000000L;
        return new DateTimeOffset(value, System.TimeSpan.Zero);
    }

    public static TimeSpan TimeSpan()
    {
        return new TimeSpan(System.Random.Shared.NextInt64(new TimeSpan(365, 0, 0, 0).Ticks));
    }

    public static int Length() => Length(0, 10);

    public static int Length(int maxLength) => Length(1, maxLength);

    public static int Length(int minLength, int maxLength) => System.Random.Shared.Next(minLength, maxLength);

    public static bool Boolean() => Int32(0, 2) == 0;

    public static T Enum<T>()
        where T : Enum
    {
        if (Attribute.GetCustomAttribute(typeof(Enum), typeof(FlagsAttribute)) is FlagsAttribute)
        {
            throw new InvalidOperationException("flags enum is not supported.");
        }

        var values = System.Enum.GetValues(typeof(T));
        var index = Int32(0, values.Length);
        return (T)values.GetValue(index)!;
    }

    public static string String()
    {
        var sb = new StringBuilder();
        var count = Int32(1, 10);
        for (var i = 0; i < count; i++)
        {
            if (sb.Length != 0)
            {
                sb.Append(' ');
            }

            sb.Append(Word());
        }

        return sb.ToString();
    }

    public static T[] Array<T>(Func<T> generator)
    {
        var length = Length();
        var items = new T[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = generator();
        }

        return items;
    }

    public static List<T> List<T>(Func<T> generator)
    {
        var length = Length();
        var items = new T[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = generator();
        }

        return new List<T>(items);
    }

    public static ImmutableArray<T> ImmutableArray<T>(Func<T> generator)
    {
        var length = Length();
        var items = new T[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = generator();
        }

        return System.Collections.Immutable.ImmutableArray.Create(items);
    }

    public static ImmutableList<T> ImmutableList<T>(Func<T> generator)
    {
        var length = Length();
        var items = new T[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = generator();
        }

        return System.Collections.Immutable.ImmutableList.Create(items);
    }

    public static T? RandomOrDefault<T>(this IEnumerable<T> enumerable)
        => RandomOrDefault(enumerable, item => true);

    public static T? RandomOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var items = enumerable.Where(predicate).ToArray();
        if (items.Length == 0)
        {
            return default!;
        }

        var count = items.Length;
        var index = Int32(0, count);
        return items[index];
    }

    public static T Random<T>(this IEnumerable<T> enumerable)
        => Random(enumerable, item => true);

    public static T Random<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        if (enumerable.Any() == false)
        {
            throw new InvalidOperationException("there is no random item that matches the condition.");
        }

        return RandomOrDefault(enumerable, predicate)!;
    }

    internal sealed class DepthScope : IDisposable
    {
        public DepthScope() => DepthValue.Value++;

        public void Dispose() => DepthValue.Value--;
    }
}