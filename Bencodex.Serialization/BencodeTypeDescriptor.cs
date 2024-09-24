using System.Reflection;

namespace Bencodex.Serialization;

internal sealed class BencodeTypeDescriptor : IBencodeTypeDescriptor
{
    public BencodeTypeDescriptor(BencodeDescriptor descriptor)
    {
        IsBinary = descriptor.IsBinary;
        OwnType = descriptor.DeclaringType;
    }

    public BencodeTypeDescriptor(MemberInfo memberInfo, bool isBinary)
    {
        IsBinary = isBinary;
        OwnType = memberInfo.DeclaringType ??
            throw new ArgumentException(
            message: $"'{memberInfo}' must have a declaring type.",
            paramName: nameof(memberInfo));
    }

    public BencodeTypeDescriptor(Type ownType)
        : this(ownType, isBinary: false)
    {
    }

    public BencodeTypeDescriptor(Type ownType, bool isBinary)
    {
        IsBinary = isBinary;
        OwnType = ownType;
    }

    public bool IsBinary { get; }

    public Type? OwnType { get; }
}
