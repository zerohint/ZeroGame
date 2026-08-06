using System;
using UnityEngine;


/// <summary>
/// Scene dependent panel base
/// </summary>
public class Panel : MonoBehaviour
{
    public event Action<Panel> OnOpened;
    public event Action<Panel> OnClosed;

    /// <summary>
    /// Is panel open
    /// </summary>
    public bool IsOpen => content.activeSelf;

    public PanelID PanelID => (PanelID) panelId;

    [field: SerializeField] private string panelId;

    [SerializeField] private GameObject content;

    /// <summary>
    /// Open panel shortcut for serialized fields
    /// </summary>
    /// <param name="panelId"></param>
    public void OpenPanel(string panelId) => PanelManager.Instance.OpenPanel((PanelID)panelId);

    public virtual void Open()
    {
        content.SetActive(true);
        OnOpened?.Invoke(this);
    }

    public virtual void Close()
    {
        if (PanelID == PanelNames.HOME) return;

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



// public enum PanelType
// {
//     // Home Scene Panels
//     Home = 0,
//     Ban = 1,
//     Leaderboard = 3,
//     Friends = 4,
//     BattlePass = 5,
//     SpinWheel = 6,
//     Store = 10,
//     Chat = 14,
//     Soon = 16,
//     Pairing = 2,
//     Lobby = 7,
//     Seasons = 8,
//     Royalty = 11,
//     VoiceLobby = 13,
//     CongratsPanel = 15,

//     // Game Scene Panels
// }
