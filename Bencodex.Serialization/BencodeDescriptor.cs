using Bencodex.Types;

namespace Bencodex.Serialization;

public abstract class BencodeDescriptor
{
    private static readonly Dictionary<Type, BencodeDescriptorCollection> DescriptorsByType = [];
    private static readonly object LockObject = new();

    public abstract IKey Key { get; }

    public abstract string Name { get; }

    public abstract Type MemberType { get; }

    public abstract Type DeclaringType { get; }

    public abstract bool IsBinary { get; }

    public static BencodeDescriptorCollection GetDescriptors(object obj)
        => GetDescriptors(obj.GetType());

    public static BencodeDescriptorCollection GetDescriptors(Type type)
    {
        lock (LockObject)
        {
            if (DescriptorsByType.TryGetValue(type, out var descriptors) != true)
            {
                descriptors = new BencodeDescriptorCollection(type);
                DescriptorsByType.Add(type, descriptors);
            }

            return descriptors;
        }
    }

    public abstract object? GetValue(object obj);

    public abstract void SetValue(object obj, object? value);

    public virtual BencodeConverter GetConverter(IBencodeTypeContext typeContext)
        => typeContext.GetConverter(DeclaringType, MemberType);

    internal IValue SerilizeValue(IBencodeTypeContext typeContext, object? obj)
    {
        if (obj is null)
        {
            return Null.Value;
        }

        if (obj is IValue value && MemberType.IsAssignableFrom(obj.GetType()))
        {
            return value;
        }

        return ConvertFrom(typeContext, obj);
    }

    internal object? DeserializeValue(IBencodeTypeContext typeContext, IValue value)
    {
        if (value is Null)
        {
            return null;
        }

        if (typeof(IValue).IsAssignableFrom(MemberType) == true)
        {
            return value;
        }

        return ConvertTo(typeContext, value);
    }

    private IValue ConvertFrom(IBencodeTypeContext typeContext, object? value)
    {
        var converter = GetConverter(typeContext);
        var converterContext = new BencodeTypeContext(typeContext, this);
        if (value != null &&
            converter.CanConvertFrom(converterContext, MemberType) == true &&
            converter.ConvertFrom(converterContext, value) is IValue v)
        {
            return v;
        }

        throw new NotSupportedException();
    }

    private object? ConvertTo(IBencodeTypeContext typeContext, IValue value)
    {
        var converter = GetConverter(typeContext);
        var converterContext = new BencodeTypeContext(typeContext, this);
        if (converter.CanConvertTo(converterContext, MemberType) == true)
        {
            return converter.ConvertTo(converterContext, value, MemberType);
        }

        throw new NotSupportedException();
    }
}
