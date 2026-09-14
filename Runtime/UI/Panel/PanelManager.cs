using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Scene independent panels
/// </summary>
[CreateAssetMenu(menuName = "Game/Managers/Panel Manager")]
public partial class PanelManager : SingletonSC<PanelManager>
{
    /// <summary>
    /// A root panel is stacked, home in the menu scene
    /// </summary>
    public bool IsHomeLoaded => stack.Count > 0;

    [SerializeField] private Canvas canvasPrefab;
    [SerializeField] private PanelData[] panels;
    [Header("Advanced")]
    [SerializeField, Tooltip("Adds click sound to all Button's if assigned")] private AudioClip clickSound;


    /// <summary>
    /// Navigation stack, bottom is the root panel and never closed, top is the last opened one.
    /// A screen replaces everything above the root, a <see cref="Popup"/> stacks on top of it
    /// </summary>
    private readonly List<Panel> stack = new();
    private Panel CurrPanel => stack.Count > 0 ? stack[^1] : null;
    private Canvas runtimeCanvas;
    private AudioSource uiAudioSource;

    /// <summary>
    /// The shared dim on the canvas prefab, if it has one. Optional - a project that does not
    /// dim behind its panels simply leaves it off the prefab
    /// </summary>
    private PanelDimm dimm;

    private readonly Dictionary<Type, PanelData> panelCache = new();


    /// <summary>
    /// Open panel by class name, for serialized fields that can't hold a type
    /// </summary>
    /// <param name="typeName">Panel class name, with or without namespace</param>
    public Task<Panel> OpenPanel(string typeName)
    {
        CachePanelsDictionary();

        var panelData = panelCache.Values.FirstOrDefault(pd => pd.PanelType.Name == typeName || pd.PanelType.FullName == typeName);
        if (panelData == null)
            throw new KeyNotFoundException($"[PanelManager.OpenPanel] Panel {typeName} not found in panels");

        return OpenPanelInternal(panelData);
    }


    /// <summary>
    /// Open panel by class
    /// </summary>
    /// <typeparam name="T">Panel class, must be registered in panels</typeparam>
    public async Task<T> OpenPanel<T>() where T : Panel
    {
        CachePanelsDictionary();

        if (!panelCache.TryGetValue(typeof(T), out var panelData))
            throw new KeyNotFoundException($"[PanelManager.OpenPanel] Panel {typeof(T)} not found in panels");

        var panel = await OpenPanelInternal(panelData);

        if (panel is not T typedPanel)
            throw new InvalidCastException(
                $"[PanelManager.OpenPanel] Expected {typeof(T)}, got {(panel == null ? "null" : panel.GetType())}");

        return typedPanel;
    }



    /// <summary>
    /// Close the topmost panel and reveal what is under it.
    /// TODO: connect android/ios back and esc
    /// </summary>
    public void Back()
    {
        PruneStack();
        PopFrom(stack.Count - 1);
        RefreshVisibility();
    }


    /// <summary>
    /// Close a panel and everything stacked above it
    /// </summary>
    /// <returns>False when the panel is the root one, or not open at all</returns>
    public bool ClosePanel(Panel panel)
    {
        PruneStack();
        var index = stack.IndexOf(panel);
        if (index < 0) return false;

        PopFrom(index);
        RefreshVisibility();
        return true;
    }


    /// <summary>
    /// Close all panels
    /// </summary>
    public void ClosePanels() => PopFrom(0);


    /// <summary>
    /// Close the panel, destroy its instance and release its addressable.
    /// For panels that won't be needed for a long time, reopening pays the full load cost again
    /// </summary>
    /// <typeparam name="T">Panel class, must be registered in panels</typeparam>
    /// <returns>False when the panel is not in memory anyway</returns>
    public bool FullClose<T>() where T : Panel
    {
        CachePanelsDictionary();

        if (!panelCache.TryGetValue(typeof(T), out var panelData))
            throw new KeyNotFoundException($"[PanelManager.FullClose] Panel {typeof(T)} not found in panels");

        return FullClose(panelData);
    }


    /// <inheritdoc cref="FullClose{T}"/>
    public bool FullClose(Panel panel)
    {
        CachePanelsDictionary();

        if (!panel.IsExists() || !panelCache.TryGetValue(panel.GetType(), out var panelData))
            throw new KeyNotFoundException($"[PanelManager.FullClose] Panel {panel} not found in panels");

        return FullClose(panelData);
    }


    private bool FullClose(PanelData panelData)
    {
        if (panelData.IsLoading)
        {
            Debug.LogError($"[PanelManager.FullClose] Can't unload while loading: {panelData}");
            return false;
        }

        if (!panelData.IsInstanced)
            return false;

        ClosePanel(panelData.instanced); // no-op when it's not stacked, already closed
        panelData.Unload();
        return true;
    }


    /// <summary>
    /// Load the wanted panel if needed, then stack it
    /// </summary>
    private async Task<Panel> OpenPanelInternal(PanelData panelData)
    {
        if (panelData.IsInstanced && CurrPanel == panelData.instanced)
        {
            return CurrPanel;
        }

        if (!panelData.IsInstanced)
        {
            if (!runtimeCanvas.IsExists())
            {
                runtimeCanvas = Instantiate(canvasPrefab);
                // in children, not on the root: a world space canvas usually wants its click
                // sound on a child placed where the sound should come from, not on the board
                uiAudioSource = runtimeCanvas.GetComponentInChildren<AudioSource>(true);
                // true: the dim is authored switched off, so an active-only search misses it
                dimm = runtimeCanvas.GetComponentInChildren<PanelDimm>(true);
                DontDestroyOnLoad(runtimeCanvas);
            }

            Loadings.Instance.Show(runtimeCanvas.transform);
            await panelData.Load(runtimeCanvas.transform);
            Loadings.Instance.Hide(runtimeCanvas.transform);

            if (!panelData.IsInstanced)
            {
                Debug.LogError($"LOAD RETURNED BUT NO INSTANCE {panelData.PanelType}");
                return null;
            }

            // straight away: the panel is fully instantiated by the time Load returns, so
            // there is nothing to wait for. It used to be put off by a second, which is a
            // second of a panel whose buttons are silent - and on a build with no
            // TheSingleton to run the delay on, silent for good
            ApplyButtonSounds(panelData.instanced.transform);
        }

        Push(panelData.instanced);
        return panelData.instanced;
    }



    /// <summary>
    /// The click a button makes. Every <see cref="Button"/> is wired to this by
    /// <see cref="ApplyButtonSounds"/>, and it is public for the presses that are not a
    /// Button at all - a controller shortcut opening a panel, a gesture the patient needs
    /// an acknowledgement for - so the whole UI answers with one sound instead of each
    /// caller carrying a clip of its own.
    ///
    /// Quiet, not an error, when no click sound is assigned: this is feedback, and a
    /// project that wants a silent UI says so by leaving the clip empty
    /// </summary>
    public void PlayClick()
    {
        if (clickSound == null) return;

        // the canvas is only instantiated when the first panel opens, so a click asked for
        // before that has nothing to play through. Falling back to a one-shot at the
        // listener keeps those callers audible rather than silently doing nothing
        if (uiAudioSource.IsExists())
            uiAudioSource.PlayOneShot(clickSound);
        else
            AudioSource.PlayClipAtPoint(clickSound, ListenerPoint());
    }


    private static Vector3 ListenerPoint()
    {
        var listener = FindFirstObjectByType<AudioListener>();
        if (listener.IsExists()) return listener.transform.position;

        var camera = Camera.main;
        return camera.IsExists() ? camera.transform.position : Vector3.zero;
    }


    /// <summary>
    /// Make every <see cref="Button"/> under <paramref name="t"/> click.
    ///
    /// This is the only place a UI click sound is wired up - no button and no button script
    /// carries a clip of its own - so one clip in one place is the whole UI's voice. Call it
    /// on anything made of Buttons that <see cref="PanelManager"/> did not load itself: a
    /// menu standing in a scene, a list whose rows are instantiated after the panel was.
    ///
    /// Safe to call twice on the same buttons - the listener is taken off before it is put
    /// on, so a pooled row that is re-registered clicks once rather than once per rebuild
    /// </summary>
    public void ApplyButtonSounds(Transform t)
    {
        if (clickSound == null || !t.IsExists()) return;

        // true: a panel's content is very often still switched off when this runs, and an
        // active-only search would leave every button on it silent
        foreach (var btn in t.GetComponentsInChildren<Button>(true))
        {
            btn.onClick.RemoveListener(PlayClick);
            btn.onClick.AddListener(PlayClick);
        }
    }


    /// <summary>
    /// Put the panel on top of the stack.
    /// A screen drops everything above the root first, a <see cref="Popup"/> keeps it
    /// </summary>
    private void Push(Panel panel)
    {
        PruneStack();

        var index = stack.IndexOf(panel);
        if (index >= 0) // already stacked somewhere below, move it to the top
            PopFrom(index);

        if (panel is not Popup && stack.Count > 1)
            PopFrom(1);

        stack.Add(panel);
        panel.transform.SetAsLastSibling();
        RefreshVisibility();
    }


    /// <summary>
    /// Hide and drop everything from the given index upwards.
    /// A <see cref="Panel.HardClose"/> panel is unloaded straight through <see cref="PanelData"/>,
    /// not via <see cref="FullClose(PanelData)"/>, to avoid Hide/ClosePanel calling back into this
    /// </summary>
    private void PopFrom(int index)
    {
        if (index < 0) return;

        CachePanelsDictionary();

        for (int i = stack.Count - 1; i >= index; i--)
        {
            var panel = stack[i];
            stack.RemoveAt(i);
            panel.Hide();

            if (panel.HardClose && panelCache.TryGetValue(panel.GetType(), out var panelData))
                panelData.Unload();
        }
    }


    /// <summary>
    /// Everything down to the topmost screen is visible, popups don't hide what is behind them
    /// </summary>
    private void RefreshVisibility()
    {
        var topScreen = 0;
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            if (stack[i] is Popup) continue;

            topScreen = i;
            break;
        }

        for (int i = 0; i < stack.Count; i++)
        {
            if (i >= topScreen)
                stack[i].Show();
            else
                stack[i].Hide();
        }

        RefreshDimm(topScreen);
    }


    /// <summary>
    /// Put the shared dim just under the lowest visible panel that asked for it, and take
    /// everything below that out of reach.
    ///
    /// The panels under a dim are still on screen - a popup does not hide what it sits on -
    /// so they cannot be turned off, but they must not be operable either: a control the
    /// user can still reach through a dim is one they will hit while aiming at the popup
    /// </summary>
    private void RefreshDimm(int topScreen)
    {
        var dimFrom = -1;
        for (int i = topScreen; i < stack.Count; i++)
        {
            if (!stack[i].DimBehind) continue;

            dimFrom = i;
            break;
        }

        for (int i = 0; i < stack.Count; i++)
            stack[i].SetInteractable(dimFrom < 0 || i >= dimFrom);

        if (!dimm.IsExists()) return;

        if (dimFrom < 0) dimm.Hide();
        else dimm.ShowUnder(stack[dimFrom].transform);
    }



    /// <summary>
    /// Check is panels exists at stack
    /// </summary>
    private void PruneStack()
    {
        for (int i = stack.Count - 1; i >= 0; i--)
            if (!stack[i].IsExists())
                stack.RemoveAt(i);
    }


    private void CachePanelsDictionary()
    {
        if (panelCache.Count != 0 || panels == null) return;

        foreach (var pd in panels)
        {
            if (pd.PanelType == null)
            {
                Debug.LogError($"[PanelManager] Panel type is not baked, revalidate the manager asset: {pd}");
                continue;
            }

            if (panelCache.ContainsKey(pd.PanelType))
            {
                Debug.LogError($"[PanelManager] Duplicate panel type: {pd.PanelType}");
                continue;
            }

            panelCache.Add(pd.PanelType, pd);
        }
    }


    private void OnDestroy()
    {
        panelCache.Clear();
        stack.Clear();

        if (panels == null) return;
        foreach (var panel in panels)
            panel.OnDestroy();
    }



    private void OnValidate()
    {
        panelCache.Clear();

        if (panels == null) return;
        foreach (var panel in panels)
            panel.OnValidate();
    }
}
