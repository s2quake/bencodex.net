using static Bencodex.Serialization.Utilities.TypeUtility;

namespace Bencodex.Serialization;

public static class ThrowUtility
{
    public static void ThrowArgumentExceptionIf(Func<bool> condition, string message, string? paramName = null)
    {
        if (condition() == true)
        {
            throw new ArgumentException(message, paramName);
        }
    }

    public static void ThrowIfNotBencodable(object obj, string? message = null, string? paramName = null)
    {
        if (IsBencodable(obj) != true)
        {
            throw new ArgumentException(
                message ?? $"Parameter '{nameof(obj)}' must implement '{typeof(IBencodable)}'.",
                paramName ?? nameof(obj)
            );
        }
    }

    public static void ThrowIfNotBencodable(Type type, string? message = null, string? paramName = null)
    {
        if (IsBencodable(type) != true)
        {
            throw new ArgumentException(
                message ?? $"Parameter '{nameof(type)}' must implement '{typeof(IBencodable)}'.",
                paramName ?? nameof(type)
            );
        }
    }
}
