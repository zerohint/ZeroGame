using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Dictionary that observable. Has actions on change
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    public event Action<TKey, TValue> OnItemAdded;
    public event Action<TKey, TValue> OnItemUpdated;
    public event Action<TKey> OnItemRemoved;
    public event Action OnDictionaryChanged;


    private readonly Dictionary<TKey, TValue> _inner = new();


    public TValue this[TKey key]
    {
        get => _inner[key];
        set
        {
            bool exists = _inner.ContainsKey(key);
            _inner[key] = value;
            if (exists) OnItemUpdated?.Invoke(key, value);
            else OnItemAdded?.Invoke(key, value);
            OnDictionaryChanged?.Invoke();
        }
    }

    public ICollection<TKey> Keys => _inner.Keys;
    public ICollection<TValue> Values => _inner.Values;
    public int Count => _inner.Count;
    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        _inner.Add(key, value);
        OnItemAdded?.Invoke(key, value);
        OnDictionaryChanged?.Invoke();
    }

    public bool Remove(TKey key)
    {
        if (_inner.Remove(key))
        {
            OnItemRemoved?.Invoke(key);
            OnDictionaryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool ContainsKey(TKey key) => _inner.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);
    public void Clear()
    {
        _inner.Clear();
        OnDictionaryChanged?.Invoke();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, TValue>)_inner).CopyTo(array, arrayIndex);
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

}