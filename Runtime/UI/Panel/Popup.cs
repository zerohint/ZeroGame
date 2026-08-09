using UnityEngine;


/// <summary>
/// Panel that stacks on top of the current one instead of replacing it.
/// The panel behind stays open and stays in the navigation stack
/// </summary>
public abstract class Popup : Panel
{
    public override void Show()
    {
        base.Show();
        PopupFadeIn(transform);
    }
}
