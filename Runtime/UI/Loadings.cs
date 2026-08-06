using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General loading screen manager.
/// </summary>
public class Loadings : MonoSingleton<Loadings>
{
    [SerializeField] private GameObject loadingPrefab;

    private readonly Dictionary<Transform, GameObject> activeLoadings = new();


    /// <summary>
    /// Show loading screen on the given transform (or fullscreen if null).
    /// </summary>
    /// <param name="t"></param>
    public void Show(Transform t = null)
    {
        if (t == null) t = transform;

        if (activeLoadings.ContainsKey(t))
        {
            Debug.LogWarning($"Loading already active for this transform ({t.gameObject.name})", t);
            return;
        }

        var loadingInstance = Instantiate(loadingPrefab, t);
        //panelLoading.transform.SetAsLastSibling();
        activeLoadings[t] = loadingInstance;
    }


    /// <summary>
    /// Hide the loading screen on the given transform (or fullscreen if null).
    /// </summary>
    /// <param name="t"></param>
    public void Hide(Transform t = null)
    {
        if (t == null) t = transform;

        if (activeLoadings.TryGetValue(t, out var loadingInstance))
        {
            Destroy(loadingInstance);
            activeLoadings.Remove(t);
        }
        else
        {
            Debug.LogWarning($"No active loading found for this transform ({t.gameObject.name})", t);
        }
    }
}



