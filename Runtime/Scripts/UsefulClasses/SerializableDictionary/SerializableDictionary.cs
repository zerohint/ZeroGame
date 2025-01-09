//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
//{
//    [SerializeField] private List<TKey> keys = new List<TKey>();
//    [SerializeField] private List<TValue> values = new List<TValue>();

//    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

//    public void OnBeforeSerialize()
//    {
//        keys.Clear();
//        values.Clear();
//        foreach (var pair in dictionary)
//        {
//            keys.Add(pair.Key);
//            values.Add(pair.Value);
//        }
//    }

//    public void OnAfterDeserialize()
//    {
//        dictionary = new Dictionary<TKey, TValue>();
//        for (int i = 0; i < keys.Count; i++)
//        {
//            if (i < values.Count)
//            {
//                dictionary[keys[i]] = values[i];
//            }
//        }
//    }

//    public void Add(TKey key, TValue value)
//    {
//        dictionary[key] = value;
//    }

//    public bool TryGetValue(TKey key, out TValue value)
//    {
//        return dictionary.TryGetValue(key, out value);
//    }

//    public bool ContainsKey(TKey key)
//    {
//        return dictionary.ContainsKey(key);
//    }

//    public TValue this[TKey key]
//    {
//        get => dictionary[key];
//        set => dictionary[key] = value;
//    }

//    public Dictionary<TKey, TValue> ToDictionary()
//    {
//        return new Dictionary<TKey, TValue>(dictionary);
//    }

//    public List<TKey> Keys => keys;
//    public List<TValue> Values => values;
//}
