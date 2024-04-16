using System.Globalization;
using System.Text.RegularExpressions;
using Bencodex.Types;

namespace Bencodex.Serialization.Converters;

internal sealed class TextConverter : BencodeConverter
{
    private static readonly Dictionary<Type, Func<object, string>> ConvertFromByType = new()
    {
        [typeof(float)] = value => $"{value:R}",
        [typeof(double)] = value => $"{value:R}",
        [typeof(decimal)] = value => $"{value}",
        [typeof(char)] = value => Regex.Escape($"{value}"),
        [typeof(string)] = value => (string)value,
        [typeof(DateTime)] = value => $"{value:O}",
        [typeof(TimeSpan)] = value => $"{value:c}",
        [typeof(DateTimeOffset)] = value => $"{value:O}",
    };

    private static readonly Dictionary<Type, Func<string, object>> ConvertToByType = new()
    {
        [typeof(float)] = value => float.Parse(value),
        [typeof(double)] = value => double.Parse(value),
        [typeof(decimal)] = value => decimal.Parse(value),
        [typeof(char)] = value => Regex.Unescape(value)[0],
        [typeof(string)] = value => value,
        [typeof(DateTime)] = value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
        [typeof(TimeSpan)] = value => TimeSpan.Parse(value),
        [typeof(DateTimeOffset)] = value => DateTimeOffset.Parse(value),
    };

    public static TextConverter Default { get; } = new TextConverter();

    public override bool CanConvertFrom(IBencodeTypeContext typeContext, Type sourceType)
    {
        if (ConvertFromByType.ContainsKey(sourceType) == true)
        {
            return true;
        }

        return base.CanConvertFrom(typeContext, sourceType);
    }

    public override IValue ConvertFrom(IBencodeTypeContext typeContext, object value)
    {
        if (ConvertFromByType.TryGetValue(value.GetType(), out var converter) == true)
        {
            var text = converter(value);
            return new Text(text);
        }

        throw new NotSupportedException();
    }

    public override bool CanConvertTo(IBencodeTypeContext typeContext, Type destinationType)
    {
        if (ConvertToByType.ContainsKey(destinationType) == true)
        {
            return true;
        }

        return base.CanConvertTo(typeContext, destinationType);
    }

    public override object ConvertTo(IBencodeTypeContext typeContext, IValue value, Type destinationType)
    {
        if (ConvertToByType.TryGetValue(destinationType, out var converter) == true &&
            value is Text text)
        {
            return converter(text.Value);
        }

        throw new NotSupportedException();
    }
}
