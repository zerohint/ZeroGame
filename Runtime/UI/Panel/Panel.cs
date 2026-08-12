using System;
using UnityEngine;


/// <summary>
/// Scene dependent panel base
/// </summary>
public abstract class Panel : MonoBehaviour
{
    [field: SerializeField, Tooltip("When close, destroy gameobject")] public bool HardClose { get; private set; } = false;
    public event Action<Panel> OnOpened;
    public event Action<Panel> OnClosed;

    /// <summary>
    /// Is panel open
    /// </summary>
    public bool IsOpen => content.activeSelf;


    [SerializeField] protected GameObject content;



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
        if (content.activeSelf) return;

        OnShow();
        content.SetActive(true);
        OnOpened?.Invoke(this);
    }


    /// <summary>
    /// Called by <see cref="PanelManager"/> only
    /// </summary>
    internal void Hide()
    {
        if (!content.activeSelf) return;

        OnHide();
        content.SetActive(false);
        OnClosed?.Invoke(this);
    }


    protected virtual void OnDestroy()
    {

    }
}
