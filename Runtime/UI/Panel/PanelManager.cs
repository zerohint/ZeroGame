using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


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

    /// <summary>
    /// Navigation stack, bottom is the root panel and never closed, top is the last opened one.
    /// A screen replaces everything above the root, a <see cref="Popup"/> stacks on top of it
    /// </summary>
    private readonly List<Panel> stack = new();
    private Panel CurrPanel => stack.Count > 0 ? stack[^1] : null;
    private Canvas runtimeCanvas;

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

        return OpenPanel(panelData);
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

        return await OpenPanel(panelData) as T;
    }


    /// <summary>
    /// Close the topmost panel and reveal what is under it.
    /// The root panel is never closed, in the menu scene closing back down lands on home
    /// </summary>
    /// <returns>False when there is nothing left to close</returns>
    public bool Back()
    {
        PruneStack();
        if (stack.Count <= 1) return false;

        PopFrom(stack.Count - 1);
        RefreshVisibility();
        return true;
    }


    /// <summary>
    /// Close a panel and everything stacked above it
    /// </summary>
    /// <returns>False when the panel is the root one, or not open at all</returns>
    public bool ClosePanel(Panel panel)
    {
        PruneStack();
        var index = stack.IndexOf(panel);
        if (index <= 0) return false;

        PopFrom(index);
        RefreshVisibility();
        return true;
    }


    /// <summary>
    /// Load the wanted panel if needed, then stack it
    /// </summary>
    private async Task<Panel> OpenPanel(PanelData panelData)
    {
        if (panelData.IsInstanced && CurrPanel == panelData.instanced)
            return CurrPanel;

        if (!panelData.IsInstanced)
        {
            if (!runtimeCanvas.IsExists())
            {
                runtimeCanvas = Instantiate(canvasPrefab);
                DontDestroyOnLoad(runtimeCanvas);
            }
            Loadings.Instance.Show(runtimeCanvas.transform);
            await panelData.Load(runtimeCanvas.transform);
            Loadings.Instance.Hide(runtimeCanvas.transform);

            if (!panelData.IsInstanced) // failed, already logged by PanelData
                return null;
        }

        Push(panelData.instanced);
        return panelData.instanced;
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
    /// Hide and drop everything from the given index upwards
    /// </summary>
    private void PopFrom(int index)
    {
        for (int i = stack.Count - 1; i >= index; i--)
        {
            stack[i].Hide();
            stack.RemoveAt(i);
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
    }


    /// <summary>
    /// Panels live under a runtime canvas, a scene load destroys them behind our back
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
