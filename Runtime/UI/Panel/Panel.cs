using System;
using UnityEngine;


/// <summary>
/// Scene dependent panel base
/// </summary>
public abstract class Panel : MonoBehaviour
{
    public event Action<Panel> OnOpened;
    public event Action<Panel> OnClosed;

    /// <summary>
    /// Is panel open
    /// </summary>
    public bool IsOpen => content.activeSelf;


    [SerializeField] private GameObject content;



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


    /// <summary>
    /// Called by <see cref="PanelManager"/> only
    /// </summary>
    public virtual void Show()
    {
        if (content.activeSelf) return;

        content.SetActive(true);
        OnOpened?.Invoke(this);
    }

    /// <summary>
    /// Called by <see cref="PanelManager"/> only
    /// </summary>
    public virtual void Hide()
    {
        if (!content.activeSelf) return;

        content.SetActive(false);
        OnClosed?.Invoke(this);
    }


    /// <summary>
    /// TODO: common tweens
    /// </summary>
    /// <param name="popup"></param>
    protected static void PopupFadeIn(Transform popup)
    {
        popup.localScale = Vector3.one;
        // popup.localScale = Vector3.one * 0.6f;
        // popup.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutElastic);
    }



    protected virtual void OnDestroy()
    {

    }
}
