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
        public Dictionary<string, object> properties = null;
        public DateTime createTime;
        public DateTime updateTime;

        private void ProcessDictionary(Dictionary<string, object> source, Dictionary<string, object> result)
        {
            foreach (var item in source)
            {
                var data = JObject.FromObject(item.Value).ToObject<Dictionary<string, object>>();
                var enumerator = data.GetEnumerator();
                enumerator.MoveNext();

                string valueType = enumerator.Current.Key;
                object value = enumerator.Current.Value;

                result[item.Key] = FirestoreHelper.NameToType(valueType, value);
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            if (fields == null && properties == null)
                return new Dictionary<string, object>();

            var result = new Dictionary<string, object>();

            if (fields != null)
                ProcessDictionary(fields, result);

            if (properties != null)
                ProcessDictionary(properties, result);

            return result;
        }

        public object GetValue(string key, Type targetType)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException($"Field or property '{key}' not found in document");

            var fieldValue = fields?.ContainsKey(key) == true ? fields[key] : properties[key];
            Dictionary<string, object> fieldData;

            if (fieldValue is JObject jObj)
                fieldData = jObj.ToObject<Dictionary<string, object>>();
            else if (fieldValue is Dictionary<string, object> dict)
                fieldData = dict;
            else
                throw new InvalidCastException($"Unexpected field type: {fieldValue.GetType()}");

            var kvp = fieldData.First();
            return FirestoreHelper.NameToType(kvp.Key, kvp.Value, targetType);
        }

        public T GetValue<T>(string key) => (T)GetValue(key, typeof(T));

        public bool TryParse<T>(out T result) where T : new()
        {
            result = default;
            if (fields == null && properties == null)
                return false;

            result = new T();
            var members = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                   .Cast<MemberInfo>()
                                   .Concat(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));

            foreach (var member in members)
            {
                if (member.Name == "DocumentId")
                {
                    if (member is FieldInfo field)
                        field.SetValue(result, Id);
                    else if (member is PropertyInfo property && property.CanWrite)
                        property.SetValue(result, Id);

                    continue;
                }

                if (ContainsKey(member.Name))
                {
                    object value = GetValue(member.Name, member is FieldInfo fieldInfo ? fieldInfo.FieldType : ((PropertyInfo)member).PropertyType);

                    if (member is FieldInfo field)
                        field.SetValue(result, value);
                    else if (member is PropertyInfo property && property.CanWrite)
                        property.SetValue(result, value);
                }
                else
                    Debug.LogWarning($"Field or property {member.Name} not found in Document");
            }

            return true;
        }

        public bool ContainsKey(string key) =>
            (fields != null && fields.ContainsKey(key)) || (properties != null && properties.ContainsKey(key));

        public override string ToString()
        {
            int fieldCount = fields?.Count ?? 0;
            int propertyCount = properties?.Count ?? 0;
            return $"{GetType()} ({name}) Field count: {fieldCount}, Property count: {propertyCount}";
        }
    }
}
