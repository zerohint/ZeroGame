using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// List that observable. Has actions on change
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class ObservableList<T> : IList<T>
{
    private readonly List<T> _innerList = new();

    public event Action OnListChanged;
    public event Action<T> OnItemAdded;
    public event Action<T> OnItemRemoved;
    public event Action<T> OnItemUptated;


    public T this[int index]
    {
        get => _innerList[index];
        set
        {
            _innerList[index] = value;
            OnItemUptated?.Invoke(value);
        }
    }

    public int Count => _innerList.Count;
    public bool IsReadOnly => false;

    public void Add(T item)
    {
        _innerList.Add(item);
        OnItemAdded?.Invoke(item);
        OnListChanged?.Invoke();
    }

    public bool Remove(T item)
    {
        var result = _innerList.Remove(item);
        if (result)
        {
            OnItemRemoved?.Invoke(item);
            OnListChanged?.Invoke();
        }
        return result;
    }

    public void Clear()
    {
        _innerList.Clear();
        OnListChanged?.Invoke();
    }

    public bool Contains(T item) => _innerList.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => _innerList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _innerList.GetEnumerator();
    public int IndexOf(T item) => _innerList.IndexOf(item);
    public void Insert(int index, T item)
    {
        _innerList.Insert(index, item);
        OnListChanged?.Invoke();
    }
    public int FindIndex(Predicate<T> match) => _innerList.FindIndex(match);
    public void RemoveAt(int index)
    {
        _innerList.RemoveAt(index);
        OnListChanged?.Invoke();
    }
    public bool Exists(Predicate<T> match) => _innerList.Exists(match);
    public T Find(Predicate<T> match) => _innerList.Find(match);
    public void RemoveAll(Predicate<T> match)
    {
        _innerList.RemoveAll(match);
        OnListChanged?.Invoke();
    }
}
