using System.Reflection;
using Bencodex.Types;

namespace Bencodex.Serialization;

internal sealed class BencodeArrayMemberDescriptor : BencodeDescriptor
{
    private readonly MemberInfo _memberInfo;

    public BencodeArrayMemberDescriptor(MemberInfo memberInfo)
    {
        if (memberInfo is not PropertyInfo && memberInfo is not FieldInfo)
        {
            var message =
                $"'{memberInfo}' must be the " +
                $"'{typeof(PropertyInfo)}' or the '{typeof(FieldInfo)}'.";
            throw new ArgumentException(message, nameof(memberInfo));
        }

        var memberType = GetMemberType(memberInfo);
        if (typeof(Array).IsAssignableFrom(memberType) != true)
        {
            throw new ArgumentException(
                message: $"The member type '{memberType}' must be an array.",
                paramName: nameof(memberInfo));
        }

        var elementType = memberType.GetElementType();
        if (elementType == null)
        {
            throw new ArgumentException(
                message: $"The member type '{memberType}' does not have an element type.",
                paramName: nameof(memberInfo));
        }

        if (BencodeUtility.IsSupportedType(elementType) != true)
        {
            var message =
                $"The element type '{elementType}' or " +
                $"member type '{memberType}' is not supported.";
            throw new ArgumentException(
                message: message,
                paramName: nameof(memberInfo));
        }

        Key = BencodeUtility.GetKey(AttributeUtility.GetKey(memberInfo));
        Name = memberInfo.Name;
        _memberInfo = memberInfo;
        IsBinary = AttributeUtility.IsBinary(memberInfo);
    }

    public override IKey Key { get; }

    public override string Name { get; }

    public override Type MemberType => GetMemberType(_memberInfo);

    public override Type DeclaringType => _memberInfo.DeclaringType!;

    public override bool IsBinary { get; }

    public override object? GetValue(object obj) => _memberInfo switch
    {
        PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
        FieldInfo fieldInfo => fieldInfo.GetValue(obj),
        _ => throw new NotSupportedException(),
    };

    public override void SetValue(object obj, object? value)
    {
        if (_memberInfo is PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(obj, value);
        }
        else if (_memberInfo is FieldInfo fieldInfo)
        {
            fieldInfo.SetValue(obj, value);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static Type GetMemberType(MemberInfo memberInfo) => memberInfo switch
    {
        PropertyInfo propertyInfo => propertyInfo.PropertyType,
        FieldInfo fieldInfo => fieldInfo.FieldType,
        _ => throw new NotSupportedException(),
    };
}
