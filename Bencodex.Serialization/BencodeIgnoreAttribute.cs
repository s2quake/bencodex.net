namespace Bencodex.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BencodeIgnoreAttribute : Attribute
{
}
