using System.Reflection;
using Bencodex.Serialization.Converters;
using Bencodex.Types;

namespace Bencodex.Serialization;

internal sealed class BencodeMemberDescriptor : BencodeDescriptor
{
    private static readonly Dictionary<Type, BencodeConverter> NullableConverters = new();
    private readonly MemberInfo _memberInfo;

    public BencodeMemberDescriptor(MemberInfo memberInfo)
    {
        if (memberInfo is not PropertyInfo && memberInfo is not FieldInfo)
        {
            var message =
                $"'{memberInfo}' must be the " +
                $"'{typeof(PropertyInfo)}' or the '{typeof(FieldInfo)}'.";
            throw new ArgumentException(message, nameof(memberInfo));
        }

        if (GetMemberType(memberInfo) is { } memberType &&
            BencodeUtility.IsSupportedType(memberType) != true)
        {
            throw new ArgumentException(
                message: $"The member type '{memberType}' is not supported.",
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

    public override BencodeConverter GetConverter(IBencodeTypeContext typeContext)
    {
        var underlyingType = Nullable.GetUnderlyingType(MemberType);
        var memberType = underlyingType ?? MemberType;
        var converter = BencodeUtility.ConverterByType.ContainsKey(memberType) == true ?
            BencodeUtility.ConverterByType[memberType] :
            base.GetConverter(typeContext);

        if (underlyingType != null)
        {
            return new NullableConverter(converter);
        }

        return converter;
    }

    private static Type GetMemberType(MemberInfo memberInfo) => memberInfo switch
    {
        PropertyInfo propertyInfo => propertyInfo.PropertyType,
        FieldInfo fieldInfo => fieldInfo.FieldType,
        _ => throw new NotSupportedException(),
    };
}
