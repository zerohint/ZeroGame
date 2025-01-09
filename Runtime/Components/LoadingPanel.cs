using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoSingleton<LoadingPanel>
{
    [Tooltip("Loading...{0}%"), SerializeField] private string progressFormat = "Loading...{0}%";
    [Space]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMPro.TMP_Text progressText;

    private bool IsProgressBarVisible
    {
        get
        {
            return progressBar.gameObject.activeSelf;
        }
        set
        {
            progressBar.gameObject.SetActive(value);
            progressText.gameObject.SetActive(value);
        }
    }

    public static void Show(bool progressBar = false)
    {
        Instance.gameObject.SetActive(true);
        Instance.IsProgressBarVisible = progressBar;
    }

    /// <summary>
    /// Set progress bar value
    /// </summary>
    /// <param name="progress">0-1 value</param>
    public static void SetProgress(float progress)
    {
        if(!Instance.IsProgressBarVisible)
            Debug.LogWarning("Progress bar is not visible.");

        progress = Mathf.Clamp01(progress);
        Instance.progressBar.value = progress;
        Instance.progressText.text = string.Format(Instance.progressFormat, Mathf.RoundToInt(progress * 100));
    }

    public static void Hide()
    {
        Instance.gameObject.SetActive(false);
    }
}
