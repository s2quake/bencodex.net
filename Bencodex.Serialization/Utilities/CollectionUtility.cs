using System.Collections;

namespace Bencodex.Serialization.Utilities;

public static class CollectionUtility
{
    internal static int GetCount(object obj)
    {
        if (obj is ICollection collection)
        {
            return collection.Count;
        }

        var type = obj.GetType();
        if (typeof(IReadOnlyCollection<>).IsAssignableFrom(type) == true)
        {
            var genericArguments = type.GetGenericArguments();
            var genericCollectionType = typeof(IReadOnlyCollection<>).MakeGenericType(genericArguments);
            var propertyInfo = genericCollectionType.GetProperty(nameof(IReadOnlyCollection<object>.Count));
            if (propertyInfo is null)
            {
                throw new InvalidOperationException("The property 'Count' is not found.");
            }

            return (int)propertyInfo.GetValue(obj)!;
        }

        return 0;
    }
}
