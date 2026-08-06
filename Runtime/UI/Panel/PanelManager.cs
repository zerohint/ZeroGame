using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Game/Managers/Panel Manager")]
public class PanelManager : SingletonSC<PanelManager>
{
    public bool IsHomeLoaded { get; private set; } = false;

    [SerializeField] private Canvas canvasPrefab;
    [SerializeField] private PanelData[] panels;

    private Panel CurrPanel => lastOpenedPanel.IsExists() && lastOpenedPanel.IsOpen ? lastOpenedPanel : null;
    private Panel lastOpenedPanel;
    private Canvas runtimeCanvas;


    /// <summary>
    /// Open panel
    /// </summary>
    /// <param name="panelId"></param>
    public async void OpenPanel(PanelID panelId)
    {
        try
        {
            await OpenPanel<Panel>(panelId);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }


    /// <summary>
    /// Open panel
    /// TODO: while there is T, panelId is not mandatory
    /// </summary>
    /// <param name="panelId"></param>
    public async Task<T> OpenPanel<T>(PanelID panelId) where T : Panel
    {
        if (CurrPanel != null && CurrPanel.PanelID == panelId)
            return CurrPanel as T;

        if (CurrPanel.IsExists())
            CurrPanel.Close();

        var panelData = panels.FirstOrDefault(p => p.ID == panelId);
        if (panelData == null)
        {
            Debug.LogError($"[PanelManager.OpenPanel] Panel {panelId} not found.");
            return null;
        }

        if (panelData.instanced == null)
        {
            if (!runtimeCanvas.IsExists())
                runtimeCanvas = Instantiate(canvasPrefab);
            Loadings.Instance.Show(runtimeCanvas.transform);
            await panelData.Open(runtimeCanvas.transform);
            Loadings.Instance.Hide(runtimeCanvas.transform);
        }

        CurrPanel.Open();

        lastOpenedPanel = panelData.instanced;
        if (panelId == PanelNames.HOME)
            IsHomeLoaded = true;
        return panelData.instanced as T;
    }


    private void OnDestroy()
    {
        foreach (var panel in panels)
            panel.OnDestroy();
    }



    private void OnValidate()
    {
        foreach (var panel in panels)
            panel.OnValidate();
        // TODO: check duplicate panels
    }




    [Serializable]
    private class PanelData
    {
        public PanelID ID => (PanelID)panelID;
        [SerializeField] private string panelID;
        public AssetReferenceGameObject panelAddressable;
        [NonSerialized] public Panel instanced;
        [NonSerialized] public AsyncOperationHandle<GameObject> asyncHandle;

        public async Task<Panel> Open(Transform parent)
        {
            asyncHandle = panelAddressable.LoadAssetAsync();
            await asyncHandle.Task;

            if (asyncHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[PanelData.Open] Failed to load panel: " + ToString());
                return null;
            }
            var prefab = asyncHandle.Result;
            var go = Instantiate(prefab, parent);
            if(go.TryGetComponent(out instanced))
                return instanced;
            else
                throw new Exception("Wrong type");
        }


        /// <summary>
        /// On game being destroyed
        /// In editor, game closed. I'm not sure needed on build
        /// </summary>
        public void OnDestroy()
        {
            if (asyncHandle.IsValid())
            {
                Addressables.Release(asyncHandle);
                if (instanced.IsExists())
                    Destroy(instanced.gameObject);
            }
        }

        public void OnValidate()
        {
            // TODO: load adressable, set id, id readonly
        }


        public override string ToString()
        {
            return $"{base.ToString()} {ID}";
        }

        // [Serializable]
        // public class AssetReferencePanel : AssetReferenceT<Panel>
        // {
        //     public AssetReferencePanel(string guid) : base(guid) { }
        // }
    }
}
