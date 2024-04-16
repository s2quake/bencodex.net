namespace Bencodex.Serialization;

public struct BencodeSerializerSettings
{
    private Func<string, object>? _keyConvention;

    public static BencodeSerializerSettings Default { get; }

    public Func<string, object> KeyConvention
    {
        readonly get => _keyConvention ?? (item => item);
        set => _keyConvention = value;
    }
}
