using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Serialization.Utilities;
using Bencodex.Types;
using static Bencodex.Serialization.Utilities.ArrayUtility;
using static Bencodex.Serialization.Utilities.ListUtility;

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
            var length = CollectionUtility.GetCount(value);
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

        if (IsArray(destinationType) == true)
        {
            return ToArray(typeContext, value, destinationType);
        }

        if (IsImmutableList(destinationType) == true)
        {
            return ToImmutableList(typeContext, list, destinationType);
        }

        if (IsImmutableArray(destinationType) == true)
        {
            return ToImmutableArray(typeContext, list, destinationType);
        }

        if (IsList(destinationType) == true)
        {
            return ToList(typeContext, list, destinationType);
        }

        throw new NotSupportedException();
    }

    private static bool IsSupportedType(Type type, [MaybeNullWhen(false)] out Type elementType)
    {
        if (IsArray(type, out elementType) == true)
        {
            return true;
        }

        if (IsImmutableList(type, out elementType) == true)
        {
            return true;
        }

        if (IsImmutableArray(type, out elementType) == true)
        {
            return true;
        }

        if (IsList(type, out elementType) == true)
        {
            return true;
        }

        return false;
    }
}
