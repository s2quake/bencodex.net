using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Bencodex.Types;

namespace Bencodex.Serialization;

public sealed class BencodeDescriptorCollection : IEnumerable<BencodeDescriptor>
{
    private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly OrderedDictionary _descriptorByName;
    private readonly Dictionary<IKey, string> _nameByKey = [];

    public BencodeDescriptorCollection(Type type)
    {
        var memberInfos = GetMemberInfos(type);
        var capacity = memberInfos.Length;
        var descriptorByName = new OrderedDictionary(capacity);
        var nameByKey = new Dictionary<IKey, string>(capacity);

        foreach (var memberInfo in memberInfos)
        {
            var attribute = GetAttribute(memberInfo);
            var descriptorType = attribute.DescriptorType;
            if (descriptorType.IsSubclassOf(typeof(BencodeDescriptor)) != true)
            {
                var message =
                    $"The '{descriptorType}' type must be a subclass of the '{typeof(BencodeDescriptor)}'.";
                throw new NotSupportedException(message);
            }

            var args = new object?[] { memberInfo };
            var descriptor = (BencodeDescriptor)Activator.CreateInstance(descriptorType, args)!;
            descriptorByName.Add(descriptor.Name, descriptor);
            nameByKey.Add(descriptor.Key, descriptor.Name);
        }

        _descriptorByName = descriptorByName;
        _nameByKey = nameByKey;
    }

    public int Count => _descriptorByName.Count;

    public BencodeDescriptor this[int index] => (BencodeDescriptor)_descriptorByName[index]!;

    public BencodeDescriptor this[string name] => (BencodeDescriptor)_descriptorByName[name]!;

    public BencodeDescriptor this[IKey key] => (BencodeDescriptor)_descriptorByName[_nameByKey[key]]!;

    public bool TryGetDescriptor(string name, out BencodeDescriptor descriptor)
    {
        if (_descriptorByName.Contains(name))
        {
            descriptor = (BencodeDescriptor)_descriptorByName[name]!;
            return true;
        }
        else
        {
            descriptor = null!;
            return false;
        }
    }

    public IEnumerator<BencodeDescriptor> GetEnumerator()
    {
        foreach (var item in _descriptorByName.Values)
        {
            yield return (BencodeDescriptor)item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var item in _descriptorByName.Values)
        {
            yield return item;
        }
    }

    private static MemberInfo[] GetMemberInfos(Type type)
    {
        var memberInfos = type.GetMembers(_bindingFlags)
                              .Where(AttributeUtility.IsEnabled)
                              .Where(item => TryGetAttribute(item, out _))
                              .ToArray();

        return memberInfos;
    }

    private static bool TryGetAttribute(MemberInfo memberInfo, out BencodeAttributeBase value)
    {
        var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(BencodeAttributeBase));
        if (attribute is BencodeAttributeBase bencodableAttribute)
        {
            value = bencodableAttribute;
            return true;
        }
        else
        {
            value = null!;
            return false;
        }
    }

    private static BencodeAttributeBase GetAttribute(MemberInfo memberInfo)
    {
        var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(BencodeAttributeBase));
        if (attribute is BencodeAttributeBase bencodableAttribute)
        {
            return bencodableAttribute;
        }

        throw new NotSupportedException();
    }
}
