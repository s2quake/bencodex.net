using System.Reflection;

namespace Bencodex.Serialization;

internal static class AttributeUtility
{
    public static object GetKey(MemberInfo memberInfo)
    {
        var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(BencodeAttribute));
        if (attribute is BencodeAttribute bencodeAttribute && bencodeAttribute.Key != DBNull.Value)
        {
            return bencodeAttribute.Key;
        }

        return memberInfo.Name;
    }

    public static bool IsBinary(MemberInfo memberInfo)
    {
        var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(BencodeAttribute));
        if (attribute is BencodeAttribute bencodeAttribute)
        {
            return bencodeAttribute.IsBinary;
        }

        return false;
    }

    public static bool IsEnabled(MemberInfo memberInfo)
    {
        if (Attribute.GetCustomAttribute(memberInfo, typeof(BencodeIgnoreAttribute)) != null)
        {
            return false;
        }

        return Attribute.GetCustomAttribute(memberInfo, typeof(BencodeAttribute)) != null;
    }

    public static Type GetConverterType(Type type)
    {
        var attribute = Attribute.GetCustomAttribute(type, typeof(BencodeConverterAttribute));
        if (attribute is BencodeConverterAttribute bencodeConverterAttribute)
        {
            return bencodeConverterAttribute.ConverterType ?? throw new InvalidOperationException();
        }

        return typeof(BencodeBaseConverter);
    }
}
