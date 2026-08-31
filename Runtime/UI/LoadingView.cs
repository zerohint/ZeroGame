using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One instantiated loading screen. Progress is optional: wire progressParent (plus a slider,
/// a label, or both) to get a bar, leave them empty to keep the plain spinner. The parent stays
/// hidden until someone calls <see cref="SetProgress"/>, so <see cref="Loadings.Show"/> alone
/// behaves exactly as before. <see cref="Loadings"/> owns the instances of this component.
/// </summary>
public class LoadingView : MonoBehaviour
{
    [Tooltip("Loading...{0}%"), SerializeField] private string progressFormat = "{0}%";
    [Space]
    [Tooltip("Optional. Holds the bar and the label. Hidden until SetProgress is called.")]
    [SerializeField] private GameObject progressParent;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    /// <summary>True when this loading screen is able to show progress at all.</summary>
    public bool HasProgress => progressParent.IsExists();


    private void Awake()
    {
        if (progressParent.IsExists())
            progressParent.SetActive(false);
        else if (progressSlider.IsExists() || progressText.IsExists())
            Debug.LogError($"{name}: progress slider/text are assigned but progressParent is not, so the progress view can never be shown or hidden.", this);
    }


    /// <summary>
    /// Show the progress view and update it. Does nothing on a loading screen that was set up
    /// without a progress parent - progress is opt-in, per prefab.
    /// </summary>
    /// <param name="progress">0-1 value</param>
    public void SetProgress(float progress)
    {
        if (!progressParent.IsExists())
            return;

        progressParent.SetActive(true);

        progress = Mathf.Clamp01(progress);

        if (progressSlider.IsExists())
            progressSlider.value = progress;

        if (progressText.IsExists())
            progressText.text = string.Format(progressFormat, Mathf.RoundToInt(progress * 100f));
    }
}
