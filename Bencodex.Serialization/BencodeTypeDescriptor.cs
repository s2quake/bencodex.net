using System.Reflection;

namespace Bencodex.Serialization;

internal sealed class BencodeTypeDescriptor : IBencodeTypeDescriptor
{
    public BencodeTypeDescriptor(BencodeDescriptor descriptor)
    {
        IsBinary = descriptor.IsBinary;
        OwnType = descriptor.DeclaringType;
    }

    public BencodeTypeDescriptor(MemberInfo memberInfo)
    {
        IsBinary = false;
        OwnType = memberInfo.DeclaringType ??
            throw new ArgumentException(
            message: $"'{memberInfo}' must have a declaring type.",
            paramName: nameof(memberInfo));
    }

    public BencodeTypeDescriptor(Type ownType)
    {
        IsBinary = false;
        OwnType = ownType;
    }

    public bool IsBinary { get; }

    public Type? OwnType { get; }
}
