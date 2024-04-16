using System.Collections;
using Bencodex.Serialization.Converters;
using BooleanConverter = Bencodex.Serialization.Converters.BooleanConverter;

namespace Bencodex.Serialization;

// internal sealed class BencodeConverterCollection : IEnumerable<BencodeConverter>
// {
//     private readonly Dictionary<Type, BencodeConverter> _converterByType = [];
//     private readonly BencodeConverter[] _baseConverters =
//     [
//         new BooleanConverter(),
//         new IntegerConverter(),
//         new TextConverter(),
//         new ListConverter(),
//         new DictionaryConverter(),
//         new BinaryConverter(),
//     ];

//     public int Count => _converterByType.Count;

//     public BencodeConverter this[IBencodeTypeContext typeContext, Type type]
//     {
//         get
//         {
//             if (_converterByType.ContainsKey(type) != true)
//             {
//                 if (FindBaseConverter(typeContext, type) is BencodeConverter baseConverter)
//                 {
//                     _converterByType.Add(type, baseConverter);
//                 }
//                 else
//                 {
//                     var converterType = AttributeUtility.GetConverterType(type);
//                     var converter = (BencodeConverter)Activator.CreateInstance(converterType)!;
//                     _converterByType.Add(type, converter);
//                 }
//             }

//             return _converterByType[type];
//         }
//     }

//     public IEnumerator<BencodeConverter> GetEnumerator()
//         => _converterByType.Values.GetEnumerator();

//     IEnumerator IEnumerable.GetEnumerator()
//         => _converterByType.Values.GetEnumerator();

//     private BencodeConverter? FindBaseConverter(IBencodeTypeContext typeContext, Type type)
//     {
//         foreach (var converter in _baseConverters)
//         {
//             if (converter.CanConvertFrom(typeContext, type) == true)
//             {
//                 return converter;
//             }
//         }

//         return null;
//     }
// }
