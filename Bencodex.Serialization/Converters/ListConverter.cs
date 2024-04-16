using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class ListConverter : BencodeConverter
{
    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (typeof(Array).IsAssignableFrom(sourceType))
        {
            return IsSupportedArrayType(sourceType);
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (typeof(Array).IsAssignableFrom(destinationType))
        {
            return IsSupportedArrayType(destinationType);
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (value is Array @array)
        {
            VerifySupportedArrayType(@array.GetType());

            var itemList = new List<IValue>(@array.Length);
            var elementType = @array.GetType().GetElementType()!;
            var converter = typeContext.GetConverter(@array.GetType(), elementType);
            for (var i = 0; i < @array.Length; i++)
            {
                var arrayItem = @array.GetValue(i);
                var arrayValue = arrayItem != null ? converter.ConvertFrom(typeContext, arrayItem)! : Null.Value;
                itemList.Add(arrayValue);
            }

            return new List(itemList);
        }

        throw new NotSupportedException();
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (value is List list)
        {
            VerifySupportedArrayType(destinationType);

            if (typeof(Array).IsAssignableFrom(destinationType))
            {
                var elementType = destinationType.GetElementType()!;
                var array = Array.CreateInstance(elementType, list.Count);
                var converter = typeContext.GetConverter(@array.GetType(), elementType);
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var itemValue = item == null || item is Null ? null : converter.ConvertTo(typeContext, item, elementType);
                    array.SetValue(itemValue, i);
                }

                return array;
            }
        }

        throw new NotSupportedException();
    }

    private static bool IsSupportedArrayType(Type arrayType)
    {
        var elementType = arrayType.GetElementType()!;
        return BencodeUtility.IsSupportedType(elementType);
    }

    private static void VerifySupportedArrayType(Type arrayType)
    {
        var elementType = arrayType.GetElementType()!;
        if (BencodeUtility.IsSupportedType(elementType) != true)
        {
            throw new NotSupportedException($"Element type '{elementType}' of array type '{arrayType}' is not supported.");
        }
    }
}
