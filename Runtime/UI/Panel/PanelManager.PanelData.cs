using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class PanelManager
{
    [Serializable]
    private class PanelData
    {
        /// <summary>
        /// Name to show on inspector panel and log
        /// </summary>
        [SerializeField, HideInInspector] private string displayName = "Undefined";

        /// <summary>
        /// Dictionary key, baked from the prefab on validate.
        /// Kept as a name because Unity can't serialize <see cref="Type"/>
        /// </summary>
        public Type PanelType => panelType ??= Type.GetType(panelTypeName ?? string.Empty);
        [SerializeField, HideInInspector] private string panelTypeName;
        [NonSerialized] private Type panelType;

        [SerializeField] private AssetReferenceGameObject panelAddressable;

        [NonSerialized] public Panel instanced;
        [NonSerialized] private AsyncOperationHandle<GameObject> asyncHandle;
        [NonSerialized] private Task<Panel> loadTask;

        public bool IsInstanced => instanced.IsExists();
        public bool IsLoading => loadTask != null;


        /// <summary>
        /// Load addressable and instantiate it.
        /// Concurrent calls share one task instead of instantiating twice
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public async Task<Panel> Load(Transform parent)
        {
            if (IsInstanced)
                return instanced;

            if (loadTask != null)
                return await loadTask;

            var task = LoadInternal(parent);
            loadTask = task;

            try
            {
                return await task;
            }
            finally
            {
                if (ReferenceEquals(loadTask, task))
                    loadTask = null;
            }
        }

        private async Task<Panel> LoadInternal(Transform parent)
        {
            if (!asyncHandle.IsValid())
                asyncHandle = panelAddressable.LoadAssetAsync();

            await asyncHandle.Task;


            if (asyncHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[PanelData.Load] Failed to load panel: " + ToString());
                Unload();
                return null;
            }

            var go = Instantiate(asyncHandle.Result, parent);

            if (!go.TryGetComponent(out instanced))
            {
                Destroy(go);
                throw new Exception($"[PanelData.LoadInternal] No {nameof(Panel)} component on prefab: " + displayName);
            }

            return instanced;
        }


        /// <summary>
        /// On game being destroyed
        /// In editor, game closed. I'm not sure needed on build
        /// </summary>
        public void OnDestroy() => Unload();


        /// <summary>
        /// Destroy the instance and release the addressable, back to the never-loaded state.
        /// Reopening pays the full load cost again.
        /// Also matters on a ScriptableObject, it outlives play mode in the editor
        /// </summary>
        public void Unload()
        {
            if (instanced.IsExists())
                Destroy(instanced.gameObject);
            instanced = null;
            loadTask = null;

            if (asyncHandle.IsValid())
                panelAddressable.ReleaseAsset();
            asyncHandle = default;
        }


        public void OnValidate()
        {
#if UNITY_EDITOR
            panelTypeName = null;
            panelType = null;
            displayName = "Undefined";

            if (panelAddressable == null || !panelAddressable.RuntimeKeyIsValid())
                return;

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(panelAddressable.AssetGUID);
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null || !prefab.TryGetComponent<Panel>(out var panel))
            {
                Debug.LogError($"[PanelData.OnValidate] No {nameof(Panel)} component on: {path}");
                return;
            }

            panelTypeName = panel.GetType().AssemblyQualifiedName;
            displayName = panel.GetType().ToString();
#endif
        }


        public override string ToString()
        {
            return $"{nameof(PanelData)}({panelTypeName ?? "unbaked"})";
        }

        // [Serializable]
        // public class AssetReferencePanel : AssetReferenceT<Panel>
        // {
        //     public AssetReferencePanel(string guid) : base(guid) { }
        // }
    }
}
