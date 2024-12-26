using System;
using System.Linq;
using System.Collections.Generic;

public static class EnumerableExtensions
{
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
