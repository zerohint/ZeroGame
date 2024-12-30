/// TODO: Find a way to make this class generic and serializable. Probably using a custom editor.

using UnityEngine;

/// <summary>
/// Singleton manager scriptable object.
/// </summary>
/// [CreateAssetMenu(fileName = "ManagerName", menuName = "Game/Managers/ManagerName")]
public abstract class SingletonSC<T> : SingletonSCNonGeneric where T : SingletonSC<T>
{
    public static T Instance => TheSingleton.GetManager<T>();
}

/// <summary>
/// Non generic singleton manager scriptable object for serialization purposes.
/// DON'T USE THIS CLASS DIRECTLY. USE SingletonSC INSTEAD.
/// </summary>
public abstract class SingletonSCNonGeneric : ScriptableObject
{
    /// <summary>
    /// Called at awake.
    /// </summary>
    public virtual void Initialize() { }
}
