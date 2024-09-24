namespace Bencodex.Serialization.Tests;

public sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
{
    public static ByteArrayEqualityComparer Default { get; } = new ByteArrayEqualityComparer();

    public bool Equals(byte[]? x, byte[]? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        int hash = 17;
        foreach (byte element in obj)
        {
            hash = hash * 31 + element;
        }

        return hash;
    }
}
