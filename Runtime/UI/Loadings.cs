using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General loading screen manager.
/// </summary>
public class Loadings : MonoSingleton<Loadings>
{
    [SerializeField] private GameObject loadingPrefab;

    private readonly Dictionary<Transform, ActiveLoading> activeLoadings = new();

    private readonly struct ActiveLoading
    {
        public readonly GameObject Instance;
        /// <summary>Null when the loading prefab has no progress view on it</summary>
        public readonly LoadingView View;

        public ActiveLoading(GameObject instance)
        {
            Instance = instance;
            View = instance.GetComponentInChildren<LoadingView>(true);
        }
    }


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
        activeLoadings[t] = new ActiveLoading(loadingInstance);
    }


    /// <summary>
    /// Update the progress bar of an already shown loading screen. Optional: a loading screen
    /// keeps its progress view hidden until this is called, and a loading prefab without a
    /// <see cref="LoadingView"/> simply ignores it.
    /// </summary>
    /// <param name="progress">0-1 value</param>
    /// <param name="t">The transform the loading screen was shown on (fullscreen if null)</param>
    public void SetProgress(float progress, Transform t = null)
    {
        if (t == null) t = transform;

        if (!activeLoadings.TryGetValue(t, out var active))
        {
            Debug.LogWarning($"No active loading found for this transform ({t.gameObject.name}), cannot set progress.", t);
            return;
        }

        if (!active.View.IsExists())
            return;

        active.View.SetProgress(progress);
    }


    /// <summary>
    /// Hide the loading screen on the given transform (or fullscreen if null).
    /// </summary>
    /// <param name="t"></param>
    public void Hide(Transform t = null)
    {
        if (t == null) t = transform;

        if (activeLoadings.TryGetValue(t, out var active))
        {
            Destroy(active.Instance);
            activeLoadings.Remove(t);
        }
        else
        {
            Debug.LogWarning($"No active loading found for this transform ({t.gameObject.name})", t);
        }
    }
}
