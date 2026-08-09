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
        /// Name to show on inspector panel
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


        /// <summary>
        /// Load addressable and instantiate it.
        /// Concurrent calls share one task instead of instantiating twice
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Task<Panel> Load(Transform parent)
        {
            if (IsInstanced)
                return Task.FromResult(instanced);

            return loadTask ??= LoadInternal(parent);
        }


        /// <exception cref="Exception"></exception>
        private async Task<Panel> LoadInternal(Transform parent)
        {
            try
            {
                if (!asyncHandle.IsValid())
                    asyncHandle = panelAddressable.LoadAssetAsync();
                await asyncHandle.Task;

                if (asyncHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"[PanelData.Load] Failed to load panel: " + ToString());
                    Release();
                    return null;
                }

                var go = UnityEngine.Object.Instantiate(asyncHandle.Result, parent);
                if (!go.TryGetComponent(out instanced))
                {
                    UnityEngine.Object.Destroy(go);
                    throw new Exception($"[PanelData.Load] No {nameof(Panel)} component on prefab: " + ToString());
                }
                return instanced;
            }
            finally
            {
                loadTask = null;
            }
        }


        /// <summary>
        /// On game being destroyed
        /// In editor, game closed. I'm not sure needed on build
        /// </summary>
        public void OnDestroy() => Release();


        /// <summary>
        /// Back to the never-loaded state.
        /// Matters on a ScriptableObject, it outlives play mode in the editor
        /// </summary>
        private void Release()
        {
            if (instanced.IsExists())
                UnityEngine.Object.Destroy(instanced.gameObject);
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
