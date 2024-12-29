using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroGame
{
    // TODO: SCDBSC should be internal
    [CreateAssetMenu(fileName = "SCDB", menuName = "Redellion/SCDB")]
    public class SCDBSC : ScriptableObject
    {
        private const string FILE_NAME = "SCDB";

        private static SCDBSC _instance;
        internal static SCDBSC Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SCDBSC>(FILE_NAME);
                    if (_instance == null)
                    {
                    #if UNITY_EDITOR
                        _instance = GenerateInstance();
                    #else
                        throw new System.Exception($"RedbSC {FILE_NAME} not found in Resources folder!");
                    #endif
                    }
                }
                return _instance;
            }
        }

        public List<ScriptableObject> scriptables;


#if UNITY_EDITOR
        private static SCDBSC GenerateInstance()
        {
            //https://stackoverflow.com/questions/50564577/creating-a-scriptable-object-in-the-unity-editor
            throw new System.NotImplementedException();
        }

        public ScriptableObject CreateSC(System.Type type)
        {
            ScriptableObject sc = ScriptableObject.CreateInstance(type);
            //sc.guid = UnityEditor.GUID.Generate().ToString();
            scriptables.Add(sc);
            AssetDatabase.AddObjectToAsset(sc, this);
            AssetDatabase.SaveAssets();
            return sc;
        }

        public void DeleteSC(ScriptableObject sc)
        {
            scriptables.Remove(sc); 
            AssetDatabase.RemoveObjectFromAsset(sc);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
