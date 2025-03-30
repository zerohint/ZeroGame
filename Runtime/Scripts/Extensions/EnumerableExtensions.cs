using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class EnumerableExtensions
{
    /// <summary>
    /// Get row of 2D matrix
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static T[] GetRow<T>(this T[,] array, int row)
    {
        if (!typeof(T).IsPrimitive)
            throw new InvalidOperationException("Not supported for managed types.");

        if (array == null)
            throw new ArgumentNullException("array");

        int cols = array.GetUpperBound(1) + 1;
        T[] result = new T[cols];

        int size;

        if (typeof(T) == typeof(bool))
            size = 1;
        else if (typeof(T) == typeof(char))
            size = 2;
        else
            size = Marshal.SizeOf<T>();

        Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

        return result;
    }

    /// <summary>
    /// Get column of 2D matrix
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static T[] GetColumn<T>(this T[,] array, int column)
    {
        if (!typeof(T).IsPrimitive)
            throw new InvalidOperationException("Not supported for managed types.");

        if (array == null)
            throw new ArgumentNullException("array");

        int rows = array.GetUpperBound(0) + 1;
        T[] result = new T[rows];

        int size;

        if (typeof(T) == typeof(bool))
            size = 1;
        else if (typeof(T) == typeof(char))
            size = 2;
        else
            size = Marshal.SizeOf<T>();

        for (int i = 0; i < rows; i++)
            Buffer.BlockCopy(array, (i * array.GetLength(1) + column) * size, result, i * size, size);

        return result;
    }

    /// <summary>
    /// Get maximum element by comparing selectors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TComparable"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="elementSelector"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T GetMax<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> elementSelector)
    where TComparable : IComparable<TComparable>
    {
        if (enumerable == null || !enumerable.Any())
            throw new InvalidOperationException("Collection is empty or null.");

        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Collection is empty.");

        T maxElement = enumerator.Current;
        TComparable maxValue = elementSelector(maxElement);

        while (enumerator.MoveNext())
        {
            var currentElement = enumerator.Current;
            var currentValue = elementSelector(currentElement);

            if (currentValue.CompareTo(maxValue) > 0)
            {
                maxElement = currentElement;
                maxValue = currentValue;
            }
        }

        return maxElement;
    }

    /// <summary>
    /// Get minimum element by comparing selectors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TComparable"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="elementSelector"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T GetMin<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> elementSelector)
    where TComparable : IComparable<TComparable>
    {
        if (enumerable == null || !enumerable.Any())
            throw new InvalidOperationException("Collection is empty or null.");

        using var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Collection is empty.");

        T minElement = enumerator.Current;
        TComparable minValue = elementSelector(minElement);

        while (enumerator.MoveNext())
        {
            var currentElement = enumerator.Current;
            var currentValue = elementSelector(currentElement);

            if (currentValue.CompareTo(minValue) < 0)
            {
                minElement = currentElement;
                minValue = currentValue;
            }
        }

        return minElement;
    }



    /// <summary>
    /// Return values shuffled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        T[] array = collection.ToArray();
        T temp;
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, array.Length);
            temp = array[rnd];
            array[rnd] = array[i];
            array[i] = temp;
        }
        return array;
    }


    /// <summary>
    /// Select random number of elements from collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<T> SelectSample<T>(this IEnumerable<T> collection, int count)
    {
        T[] shuffledArray = collection.Shuffle().ToArray();
        for (int i = 0; i < count; i++)
        {
            yield return shuffledArray[i];
        }
    }

    /// <summary>
    /// Create list from collection until stopValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="stopValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static List<T> ToListUntil<T>(this IEnumerable<T> source, T stopValue)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        List<T> result = new();

        foreach (var item in source)
        {
            if (EqualityComparer<T>.Default.Equals(item, stopValue))
                break;
            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// Get sub array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> SubArray<T>(this IEnumerable<T> array, int startIndex, int length)
    {
        if (array == null)
            throw new ArgumentNullException("array");

        int num = array.Count();
        if (num == 0)
        {
            if (startIndex != 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (length != 0)
                throw new ArgumentOutOfRangeException("length");
            yield break;
        }

        if (startIndex < 0 || startIndex >= num)
            throw new ArgumentOutOfRangeException("startIndex");
        if (length < 0 || length > num - startIndex)
            throw new ArgumentOutOfRangeException("length");

        for (int i = startIndex; i < length; i++)
        {
            yield return array.ElementAt(i);
        }
    }



    /// <summary>
    /// Get nearest one to a point.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="list"></param>
    /// <param name="position">Position of point Ex. GetPos()</param>
    /// <param name="positionSelector">Way to getting position of element Ex. (agent) => agent.GetPos()</param>
    /// <param name="condition">Taking into action condition Ex. (agent) => agent.isHostile()</param>
    /// <returns></returns>
    public static TResult GetNearest<TResult>(this IEnumerable<TResult> collection, UnityEngine.Vector3 position, Func<TResult, UnityEngine.Vector3> positionSelector, Func<TResult, bool> condition = null) where TResult : class
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        TResult nearest = null;
        float nearestDistSqr = float.MaxValue;
        foreach (var item in collection)
        {
            if (!item.IsExists())
                continue;

            if (condition != null && !condition.Invoke(item))
                continue;

            float distSqr = UnityEngine.Vector3.SqrMagnitude(positionSelector.Invoke(item) - position);
            if (distSqr < nearestDistSqr)
            {
                nearest = item;
                nearestDistSqr = distSqr;
            }
        }
        return nearest;
    }
}
