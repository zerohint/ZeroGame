using System;
using UnityEngine;


/// <summary>
/// Scene dependent panel base
/// </summary>
public abstract class Panel : MonoBehaviour
{
    [field: SerializeField, Tooltip("When close, destroy gameobject")] public bool HardClose { get; private set; } = false;

    /// <summary>
    /// Darken whatever is behind this panel.
    ///
    /// The dim is one quad owned by <see cref="PanelManager"/> and shared by every panel,
    /// not an Image on the prefab - two stacked panels that each carried their own would
    /// dim twice, and the lower one would dim the panel above it. See <see cref="PanelDimm"/>
    /// </summary>
    [field: SerializeField, Tooltip("Darken whatever is behind this panel. The dim is shared " +
        "and lives on the canvas - don't put a dim Image on the prefab")]
    public bool DimBehind { get; private set; } = false;

    public event Action<Panel> OnOpened;
    public event Action<Panel> OnClosed;

    /// <summary>
    /// Is panel open
    /// </summary>
    public bool IsOpen => shown;

    /// <summary>
    /// May the user operate this panel right now.
    ///
    /// False for a panel that is still on screen but sitting behind a dim: it is visible, so
    /// it cannot simply be turned off, but a control the user can still reach through the dim
    /// is one they will hit by accident. Set by <see cref="PanelManager"/>
    /// </summary>
    public bool Interactable { get; private set; } = true;


    [SerializeField] protected GameObject content;


    /// <summary>
    /// Has <see cref="PanelManager"/> opened this panel, as opposed to
    /// <see cref="content"/> merely happening to be switched on.
    ///
    /// The two are not the same thing on a freshly loaded panel: a prefab is nearly always
    /// authored with its content on - that is how it is worked on in the editor - so the
    /// first <see cref="Show"/> used to find it already active, return early, and never
    /// raise <see cref="OnOpened"/>. A panel that fills itself in from that event was
    /// therefore empty the first time it was opened and correct every time after, because
    /// by then a <see cref="Hide"/> had turned the content off for the next Show to turn
    /// back on. So the open state is tracked rather than read off the content
    /// </summary>
    private bool shown;



    /// <summary>
    /// Open panel by class name shortcut for used by serialized fields
    /// </summary>
    /// <param name="panelClassName"></param>
    public void OpenPanel(string panelClassName) => _ = PanelManager.Instance.OpenPanel(panelClassName);

    /// <summary>
    /// Close this panel and reveal what is under it.
    /// Navigation is owned by <see cref="PanelManager"/>, don't call <see cref="Hide"/> to close a panel
    /// </summary>
    public void Close() => PanelManager.Instance.ClosePanel(this);

    public virtual void OnShow() { }
    public virtual void OnHide() { }


    /// <summary>
    /// Called by <see cref="PanelManager"/> only
    /// </summary>
    internal void Show()
    {
        if (shown) return;

        shown = true;
        OnShow();

        // a panel with no content assigned is an authoring mistake, but it is one the
        // manager used to die on mid-navigation - leaving the stack half rebuilt and every
        // panel under it stuck. Said once, loudly, and the panel is left as it stands
        if (!content.IsExists())
            Debug.LogError($"[{GetType().Name}] has no content assigned, so it can't be " +
                           $"opened or closed. Assign it on the prefab.", this);
        else
            content.SetActive(true);

        OnOpened?.Invoke(this);
    }


    /// <summary>
    /// Called by <see cref="PanelManager"/> only
    /// </summary>
    internal void Hide()
    {
        if (!shown)
        {
            // never opened, so there is no close to announce - but a prefab authored with
            // its content on must still not be left standing on the canvas
            if (content.IsExists()) content.SetActive(false);
            return;
        }

        shown = false;
        OnHide();
        if (content.IsExists()) content.SetActive(false);
        OnClosed?.Invoke(this);
    }


    /// <summary>
    /// Called by <see cref="PanelManager"/> only. See <see cref="Interactable"/>
    /// </summary>
    internal void SetInteractable(bool value)
    {
        if (Interactable == value) return;

        Interactable = value;
        OnInteractableChanged(value);
    }


    /// <summary>
    /// Take the panel's controls out of reach, or put them back.
    ///
    /// The default stops pointer input through the <see cref="CanvasGroup"/> on
    /// <see cref="content"/>, if there is one. Override for a panel that is not operated by
    /// pointers - a world space panel touched with a tracked hand has colliders to turn off,
    /// and a CanvasGroup means nothing to it
    /// </summary>
    protected virtual void OnInteractableChanged(bool value)
    {
        if (content.IsExists() && content.TryGetComponent<CanvasGroup>(out var group))
            group.blocksRaycasts = value;
    }


    protected virtual void OnDestroy()
    {

    }
}
