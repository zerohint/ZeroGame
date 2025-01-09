using UnityEngine;

/// <summary>
/// Singleton structure, nested in TheSingleton structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoSingletonSerializable where T : MonoSingleton<T>
{
    public static T Instance => TheSingleton.GetSingleton<T>();
}

/// <summary>
/// Non generic singleton object for serialization purposes.
/// DON'T USE THIS CLASS DIRECTLY. USE MonoSingleton INSTEAD.
/// </summary>
public abstract class MonoSingletonSerializable : MonoBehaviour { }