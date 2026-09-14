using UnityEngine;


/// <summary>
/// The one dim quad every panel shares, sitting on the panel canvas.
///
/// A panel used to carry its own dim Image, which went wrong as soon as two of them were up
/// at once: the dims stacked and doubled, and the lower panel's dim covered the panel above
/// it. So there is exactly one, it belongs to the canvas rather than to any panel, and
/// <see cref="PanelManager"/> slides it to just underneath the lowest panel that asked to be
/// dimmed behind - see <see cref="Panel.DimBehind"/>.
///
/// Put this on a full-canvas child of the canvas prefab, with a <see cref="CanvasGroup"/> and
/// a stretched black Image on it. It is found by type, so it does not need wiring up
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PanelDimm : MonoBehaviour
{
    [SerializeField, Range(0f, 1f), Tooltip("How dark it gets")] private float alpha = 0.9f;
    [SerializeField, Tooltip("Fade time in seconds. 0 snaps")] private float fadeSeconds = 0.25f;


    /// <summary>Is the dim up, or on its way up</summary>
    public bool IsShown { get; private set; }


    /// <summary>
    /// Resolved on demand, not in Awake: this component starts life on an inactive object
    /// most of the time, and Awake does not run on one of those
    /// </summary>
    private CanvasGroup Group => group ??= GetComponent<CanvasGroup>();
    private CanvasGroup group;

    private float target;


    /// <summary>
    /// Fade in and slide to just below <paramref name="panel"/>, so the panel is lit and
    /// everything stacked under it is not
    /// </summary>
    public void ShowUnder(Transform panel)
    {
        if (panel.IsExists() && panel.parent == transform.parent)
        {
            // taking the panel's own index pushes the panel itself up one, which is the
            // point - the dim has to be drawn before the panel it is lighting
            var index = panel.GetSiblingIndex();

            // SetSiblingIndex removes before it inserts, so every index above the dim's
            // current one shifts down by one on the way. Without this the dim lands one
            // slot too high and covers the very panel it was asked to sit under
            if (transform.GetSiblingIndex() < index) index--;

            transform.SetSiblingIndex(index);
        }

        // coming up from nothing, so start from nothing - the prefab is authored at whatever
        // alpha was convenient to see in the editor, and that would snap rather than fade
        if (!gameObject.activeSelf) Group.alpha = 0f;

        IsShown = true;
        target = alpha;
        gameObject.SetActive(true);
        Group.blocksRaycasts = true;

        if (fadeSeconds <= 0f) Group.alpha = target;
    }


    public void Hide()
    {
        if (!IsShown) return;

        IsShown = false;
        target = 0f;
        Group.blocksRaycasts = false;

        if (fadeSeconds <= 0f)
        {
            Group.alpha = 0f;
            gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (Mathf.Approximately(Group.alpha, target))
        {
            // turned off only once it has actually faded out, so the fade is not cut short
            // by the object going inactive on the frame Hide was called
            if (!IsShown && gameObject.activeSelf) gameObject.SetActive(false);
            return;
        }

        // unscaled: a panel is very often what put the game on hold in the first place
        var step = fadeSeconds <= 0f ? 1f : Time.unscaledDeltaTime / fadeSeconds;
        Group.alpha = Mathf.MoveTowards(Group.alpha, target, step);
    }
}
