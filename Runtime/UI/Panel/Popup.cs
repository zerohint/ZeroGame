using UnityEngine;


/// <summary>
/// Panel that stacks on top of the current one instead of replacing it.
/// The panel behind stays open and stays in the navigation stack
/// </summary>
public abstract class Popup : Panel
{
    public TweenType OpenAnimation;
    public TweenType CloseAnimation;

    public override void OnShow()
    {
        base.OnShow();

        content.transform.localScale = Vector3.one;
        // popup.localScale = Vector3.one * 0.6f;
        // popup.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutElastic);
    }


    public override void OnHide()
    {
        base.OnHide();

    }

    public enum TweenType
    {
        None,
        Scale,
        Up
    }
}
