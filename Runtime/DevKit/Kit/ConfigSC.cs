using UnityEngine;

namespace Redellion.DevKit
{
    /// <summary>
    /// General data holder
    /// </summary>
    [CreateAssetMenu(fileName = resourcesName, menuName = "Redellion/Config")]
    public class ConfigSC : ScriptableObject
    {
        #region Singleton
        private const string resourcesName = "RedellionConfig";
        private static ConfigSC instance;
        internal static ConfigSC Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ConfigSC>(resourcesName);
                    Debug.Assert(instance != null, $"{nameof(ConfigSC)} not found in Resources by {resourcesName} name");
                }
                return instance;
            }
        }
        #endregion

        public DevKitSingleton kitSingletonPrefab;

        //[SerializeField] internal API.RedellionApiSettings apiSettings;
        //[SerializeField] internal ReporterSettings reporterSettings;
    }
}
