using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ZeroGame
{
    public static class FirestoreHelper
    {
        public static Dictionary<string, object> ConvertFromFirestoreJson(string data)
        { 
            throw new NotImplementedException();
            //Dictionary<string, object> fields = null;

            //// ...

            //if (fields == null)
            //    return new Dictionary<string, object>();

            //var result = new Dictionary<string, object>();

            //foreach (var field in fields)
            //{
            //    var fieldData = JObject.FromObject(field.TValue).ToObject<Dictionary<string, object>>();
            //    var enumerator = fieldData.GetEnumerator();
            //    enumerator.MoveNext();

            //    string valueType = enumerator.Current.TKey;
            //    object value = enumerator.Current.TValue;

            //    result[field.TKey] = FirestoreHelper.NameToType(valueType, value);
            //}

            //return result;
        }
        
        public static string ConvertToFirestoreJson(Dictionary<string, object> data)
        {
            var firestoreData = new Dictionary<string, object>
            {
                ["fields"] = ConvertToFirestoreFields(data)
            };
            return JsonConvert.SerializeObject(firestoreData);

            static Dictionary<string, object> ConvertToFirestoreFields(Dictionary<string, object> data)
            {
                var fields = new Dictionary<string, object>();

                foreach (var kvp in data)
                {
                    fields[kvp.Key] = ConvertToFirestoreValue(kvp.Value);
                }

                return fields;
            }
        }

        /// <summary>
        /// Convert object to a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T NameToType<T>(string valueType, object value)
        {
            return (T)NameToType(valueType, value, typeof(T));
        }



        /// <summary>
        /// Convert object to particular typed object
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="Exception"></exception>
        public static object NameToType(string valueType, object value, Type targetType = null)
        {
            try
            {
                if (value == null) return null;

                string stringValue = value.ToString();

                return valueType switch
                {
                    "stringValue" => stringValue,
                    "integerValue" => HandleIntegerValue(stringValue, targetType),
                    "doubleValue" => HandleDoubleValue(stringValue, targetType),
                    "booleanValue" => bool.Parse(stringValue),
                    "timestampValue" => HandleTimestampValue(stringValue),
                    "mapValue" => HandleMapValue(value, targetType),
                    "arrayValue" => HandleArrayValue(value, targetType),
                    "nullValue" => null,
                    "referenceValue" => stringValue, // Document reference path
                    "geoPointValue" => HandleGeoPointValue(value),
                    _ => throw new InvalidCastException($"Unsupported Firestore type: {valueType}")
                };
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    $"Failed to convert {valueType} {(targetType != null ? $"to {targetType.Name}" : "")}",
                    ex);
            }

            static object HandleIntegerValue(string stringValue, Type targetType)
            {
                if (targetType == null)
                    return int.Parse(stringValue);

                // Handle enum conversion
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, stringValue);

                // Do not remove (object) castings. Otherwise compiler upcasting all
                // to long (biggest one)
                return Type.GetTypeCode(targetType) switch
                {
                    TypeCode.Int32 => (object)int.Parse(stringValue),
                    TypeCode.Int64 => (object)long.Parse(stringValue),
                    TypeCode.Int16 => (object)short.Parse(stringValue),
                    TypeCode.Byte => (object)byte.Parse(stringValue),
                    _ => throw new InvalidCastException($"Cannot convert integerValue to {targetType.Name}")
                };
            }

            static object HandleDoubleValue(string stringValue, Type targetType)
            {
                double doubleValue = double.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture);

                if (targetType == typeof(float) || targetType == typeof(Single))
                {
                    return (float)doubleValue;
                }

                return doubleValue;
            }

            static object HandleMapValue(object value, Type targetType)
            {
                var map = (Dictionary<string, object>)((Dictionary<string, object>)value)["fields"];

                if (targetType == null || targetType == typeof(Dictionary<string, object>))
                    return map;

                // Add custom object mapping logic here if needed
                throw new InvalidCastException($"Cannot convert mapValue to {targetType.Name}");
            }

            static object HandleArrayValue(object value, Type targetType)
            {
                // TODO: it only supports List<string> for now
                //Debug.Assert(targetType == typeof(List<string>), "Only supporting List<string>");
                if (value is not JObject jObject)
                    throw new Exception("jobject");
                if (!jObject.ContainsKey("values"))
                {
                    return new List<string>();
                }
                if (jObject["values"] is not JArray valuesArray)
                    throw new Exception("Values type: " + jObject["values"].GetType());

                var result = new List<string>();

                foreach (var item in valuesArray)
                {
                    if (item is JObject itemObj &&
                        itemObj["stringValue"] is JValue stringValue &&
                        stringValue.Type == JTokenType.String)
                    {
                        result.Add(stringValue.Value.ToString());
                    }
                    else
                    {
                        throw new InvalidCastException(
                            $"Expected {{'stringValue': '...'}} but got: {item}");
                    }
                }

                return result;

                // 4. Fallback for other cases (modify as needed)
                throw new InvalidCastException(
                    $"Unsupported array format or target type. " +
                    $"Expected JObject with 'values' array for List<string>, " +
                    $"got {value?.GetType().Name} targeting {targetType?.Name}");
            }

            static DateTime HandleTimestampValue(string timestampString)
            {
                // Parse the timestamp
                DateTime dateTime;

                // First try parsing with timezone (if present)
                if (DateTime.TryParse(timestampString, CultureInfo.InvariantCulture,
                                    DateTimeStyles.RoundtripKind, out dateTime))
                {
                    // Ensure it's in UTC format (ends with Z)
                    if (dateTime.Kind == DateTimeKind.Unspecified)
                    {
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                    return dateTime.ToUniversalTime();
                }

                // Fallback for other formats (add your specific format if needed)
                throw new FormatException($"Invalid timestamp format: {timestampString}. " +
                                       "Timestamps must be in ISO 8601 format with timezone (e.g., '2023-01-01T00:00:00Z')");
            }

            static object HandleGeoPointValue(object value)
            {
                var geoDict = (Dictionary<string, object>)value;
                return new
                {
                    Latitude = double.Parse(geoDict["latitude"].ToString()),
                    Longitude = double.Parse(geoDict["longitude"].ToString())
                };
            }
        }

        public static string TypeToName(object value)
        {
            if (value == null)
                return "nullValue";

            switch (value)
            {
                case string _: return "stringValue";
                case int _: case long _: case short _: case byte _: return "integerValue";
                case float _: case double _: case decimal _: return "doubleValue";
                case bool _: return "booleanValue";
                case DateTime _: return "timestampValue";
                case Dictionary<string, object> _: return "mapValue";
                case Enum _: return "integerValue";
                case byte[] _: return "bytesValue";

                // Handle all IEnumerable types (including List<string>)
                case IEnumerable enumerable when !(value is string):
                    return "arrayValue";
                //case GeoPoint _:  // Assuming you have a GeoPoint class
                //    return "geoPointValue";
                //case DocumentReference _:  // Firestore document reference
                //    return "referenceValue";
                default:
                    throw new ArgumentException($"Unsupported Firestore data type: {value.GetType()}");
            }
        }



        public static Dictionary<string, object> ConvertToFirestoreValue(object value)
        {
            if (value == null)
                return new Dictionary<string, object> { ["nullValue"] = null };

            if (value is IEnumerable enumerable && !(value is string))
            {
                var values = new List<object>();
                foreach (var item in enumerable)
                {
                    values.Add(ConvertToFirestoreValue(item));
                }
                return new Dictionary<string, object>
                {
                    ["arrayValue"] = new Dictionary<string, object>
                    {
                        ["values"] = values
                    }
                };
            }

            return new Dictionary<string, object> { [TypeToName(value)] = value };
        }

        public static Dictionary<string, object> ToDictionary(object obj)
        {
            if (obj == null)
                return new Dictionary<string, object>();

            var dictionary = new Dictionary<string, object>();
            var type = obj.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                try
                {
                    object value = field.GetValue(obj);
                    dictionary[field.Name] = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Field '{field.Name}': {ex.Message}");
                }
            }

            return dictionary;
        }
    }
}