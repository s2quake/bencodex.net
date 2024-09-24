// using System.Collections;
// using System.Collections.Immutable;

// namespace Bencodex.Serialization.Tests.Dictionaries;

// public class KeyObjectValueObjectTest
// {
//     public static IEnumerable<object[]> TestData =>
//     [
//         [nameof(FieldClass.Dictionary), RandomUtility.Dictionary<object, object>(isNullable: false)],
//         [nameof(FieldClass.ImmtableDictionary), RandomUtility.ImmutableDictionary<object, object>(isNullable: false)],
//     ];

//     [Fact]
//     public void FieldTest()
//     {
//         var instance1 = new FieldClass();
//         var serializer = new BencodeSerializer();
//         var serializedValue = serializer.Serialize(instance1);
//         var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
//         var descriptors = BencodeDescriptor.GetDescriptors(instance1);
//         for (var i = 0; i < descriptors.Count; i++)
//         {
//             var descriptor = descriptors[i];
//             var value1 = (IEnumerable)descriptor.GetValue(instance1)!;
//             var value2 = (IEnumerable)descriptor.GetValue(instance2)!;
//             Assert.Equal(value1, value2);
//         }
//     }

//     [Fact]
//     public void PropertyTest()
//     {
//         var instance1 = new PropertyClass();
//         var serializer = new BencodeSerializer();
//         var serializedValue = serializer.Serialize(instance1);
//         var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
//         var descriptors = BencodeDescriptor.GetDescriptors(instance1);
//         for (var i = 0; i < descriptors.Count; i++)
//         {
//             var descriptor = descriptors[i];
//             var value1 = (IEnumerable)descriptor.GetValue(instance1)!;
//             var value2 = (IEnumerable)descriptor.GetValue(instance2)!;
//             Assert.Equal(value1, value2);
//         }
//     }

//     [Theory]
//     [MemberData(nameof(TestData))]
//     public void SetFieldTest(string name, IEnumerable expectedValue)
//     {
//         var descriptors = BencodeDescriptor.GetDescriptors(typeof(FieldClass));
//         var descriptor = descriptors[name];
//         var instance1 = new FieldClass();
//         descriptor.SetValue(instance1, expectedValue);

//         var serializer = new BencodeSerializer();
//         var serializedValue = serializer.Serialize(instance1);
//         var instance2 = (FieldClass)serializer.Deserialize(serializedValue, typeof(FieldClass))!;
//         var actualValue1 = (IEnumerable)descriptor.GetValue(instance1)!;
//         var actualValue2 = (IEnumerable)descriptor.GetValue(instance2)!;

//         Assert.Equal(expectedValue, actualValue2);
//         Assert.Equal(actualValue1, actualValue2);
//     }

//     [Theory]
//     [MemberData(nameof(TestData))]
//     public void SetPropertyTest(string name, IEnumerable expectedValue)
//     {
//         var descriptors = BencodeDescriptor.GetDescriptors(typeof(PropertyClass));
//         var descriptor = descriptors[name];
//         var instance1 = new PropertyClass();
//         descriptor.SetValue(instance1, expectedValue);

//         var serializer = new BencodeSerializer();
//         var serializedValue = serializer.Serialize(instance1);
//         var instance2 = (PropertyClass)serializer.Deserialize(serializedValue, typeof(PropertyClass))!;
//         var actualValue1 = (IEnumerable)descriptor.GetValue(instance1)!;
//         var actualValue2 = (IEnumerable)descriptor.GetValue(instance2)!;

//         Assert.Equal(expectedValue, actualValue2);
//         Assert.Equal(actualValue1, actualValue2);
//     }

//     public sealed class FieldClass
//     {
//         [Bencode]
//         public Dictionary<object, object> Dictionary
//             = RandomUtility.Dictionary<object, object>(isNullable: false)!;

//         [Bencode]
//         public ImmutableDictionary<object, object> ImmtableDictionary
//             = RandomUtility.ImmutableDictionary<object, object>(isNullable: false)!;
//     }

//     public sealed class PropertyClass
//     {
//         [Bencode]
//         public Dictionary<object, object> Dictionary { get; set; }
//             = RandomUtility.Dictionary<object, object>(isNullable: false)!;

//         [Bencode]
//         public ImmutableDictionary<object, object> ImmtableDictionary { get; set; }
//             = RandomUtility.ImmutableDictionary<object, object>(isNullable: false)!;
//     }
// }
