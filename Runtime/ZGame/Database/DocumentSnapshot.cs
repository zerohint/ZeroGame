using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace ZeroGame.DB
{
    public class DocumentSnapshot
    {
        public string Id => name.Split('/')[^1];

        public string name;
        public Dictionary<string, object> fields = null;
        public DateTime createTime;
        public DateTime updateTime;

        public Dictionary<string, object> ToDictionary()
        {
            // TODO: also can be used: FirestoreHelper.ConvertFromFirestoreJson
            if (fields == null)
                return new Dictionary<string, object>();

            var result = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                var fieldData = JObject.FromObject(field.Value).ToObject<Dictionary<string, object>>();
                var enumerator = fieldData.GetEnumerator();
                enumerator.MoveNext();

                string valueType = enumerator.Current.Key;
                object value = enumerator.Current.Value;

                result[field.Key] = FirestoreHelper.NameToType(valueType, value);
            }

            return result;
        }

        public object GetValue(string key, Type targetType)
        {
            if (!ContainsField(key))
                throw new KeyNotFoundException($"Field '{key}' not found in document");

            // Handle both JObject and direct Dictionary cases
            var fieldValue = fields[key];
            Dictionary<string, object> fieldData;

            if (fieldValue is JObject jObj)
                fieldData = jObj.ToObject<Dictionary<string, object>>();
            else if (fieldValue is Dictionary<string, object> dict)
                fieldData = dict;
            else
                throw new InvalidCastException($"Unexpected field type: {fieldValue.GetType()}");

            // Get the first (and only) key-value pair
            var kvp = fieldData.First();
            return FirestoreHelper.NameToType(kvp.Key, kvp.Value, targetType);
        }

        public T GetValue<T>(string key) => (T)GetValue(key, typeof(T));


        public bool TryParse<T>(out T result) where T : new()
        {
            result = default;
            if (fields == null)
                return false;

            result = new T();
            var fieldsInType = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fieldsInType)
            {
                if(field.Name == "DocumentId")
                {
                    field.SetValue(result, Id);
                    continue;
                }

                if (ContainsField(field.Name))
                {
                    object value = GetValue(field.Name, field.FieldType);
                    //Debug.Log($"{field.Name}: {field.FieldType} and return is " + (value == null ? "null" : value.GetType()));
                    field.SetValue(result, value);
                }
                else
                    Debug.LogWarning($"Field {field.Name} not found in Document");
            }

            return true; // Success
        }

        public bool ContainsField(string field) => fields != null && fields.ContainsKey(field);

        public override string ToString()
        {
            return $"{GetType()} ({name}) Field count: {fields.Count}";
        }
    }
}