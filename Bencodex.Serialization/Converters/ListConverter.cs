using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class ListConverter : BencodeConverter
{
    public static ListConverter Default { get; } = new ListConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (IsSupportedType(sourceType, out var elementType) == true)
        {
            var typeDescriptor = new BencodeTypeDescriptor(sourceType);
            var converter = typeContext.GetConverter(typeDescriptor, elementType);
            return converter.CanConvertFrom(typeContext, elementType);
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        var valueType = value.GetType();
        if (IsSupportedType(valueType, out var elementType) == true)
        {
            var length = BencodeUtility.GetCollectionCount(value);
            var itemList = new List<IValue>(length);
            var typeDescriptor = new BencodeTypeDescriptor(valueType);
            var converter = typeContext.GetConverter(typeDescriptor, elementType);
            foreach (var item in (IEnumerable)value)
            {
                var itemValue = item != null ? converter.ConvertFrom(typeContext, item) : Null.Value;
                itemList.Add(itemValue);
            }

            return new List(itemList);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (IsSupportedType(destinationType, out var elementType) == true)
        {
            var typeDescriptor = new BencodeTypeDescriptor(destinationType);
            var converter = typeContext.GetConverter(typeDescriptor, elementType);
            return converter.CanConvertFrom(typeContext, elementType);
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is not List list)
        {
            throw new NotSupportedException();
        }

        if (BencodeUtility.IsArrayType(destinationType) == true)
        {
            return BencodeUtility.ToArray(typeContext, value, destinationType);
        }

        if (BencodeUtility.IsImmutableListType(destinationType) == true)
        {
            return BencodeUtility.ToImmutableList(typeContext, list, destinationType);
        }

        if (BencodeUtility.IsImmutableArrayType(destinationType) == true)
        {
            return BencodeUtility.ToImmutableArray(typeContext, list, destinationType);
        }

        if (BencodeUtility.IsListType(destinationType) == true)
        {
            return BencodeUtility.ToList(typeContext, list, destinationType);
        }

        throw new NotSupportedException();
    }

    private static bool IsSupportedType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (BencodeUtility.IsArrayType(type, out elementType) == true)
        {
            return true;
        }

        if (BencodeUtility.IsImmutableListType(type, out elementType) == true)
        {
            return true;
        }

        if (BencodeUtility.IsImmutableArrayType(type, out elementType) == true)
        {
            return true;
        }

        if (BencodeUtility.IsListType(type, out elementType) == true)
        {
            return true;
        }

        return false;
    }
}
