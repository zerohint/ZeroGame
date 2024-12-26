using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;

namespace Redellion.Editor
{
    [InitializeOnLoad]
    public static class ToolbarUtilities
    {
        private static readonly bool active = true;
        private static readonly ToolbarZone toolbarPosition = ToolbarZone.ToolbarZoneRightAlign; 
        private static ScriptableObject _toolbar;
        private static string[] _scenePaths;
        private static string[] _sceneNames;

        static ToolbarUtilities()
        {
            if(CheckNamespaceExist("Quantum")){
                active = false;
                return;
            }

            EditorApplication.delayCall += () =>
            {
                EditorApplication.update -= Update;
                EditorApplication.update += Update;
            };
        }

        private static bool CheckNamespaceExist(string name)
            => AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetTypes().Any(type => type.Namespace == name));

        private static void Update()
        {
            if (!active) return;

            if (_toolbar == null)
            {
                Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

                UnityEngine.Object[] toolbars = UnityEngine.Resources.FindObjectsOfTypeAll(
                    editorAssembly.GetType("UnityEditor.Toolbar")
                );
                _toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (_toolbar != null)
                {
                    var root = _toolbar
                        .GetType()
                        .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(_toolbar);
                    var mRoot = rawRoot as VisualElement;
                    
                    RegisterCallback(
                        toolbarPosition.ToString(),
                        OnGUI
                    );

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);
                        if(toolbarZone.Children().ToList().Find(element=> element.Children().ToList().Find(element=> element.childCount == 1 && element.Children().First() is IMGUIContainer) != null) != null) return;
                        if (toolbarZone != null)
                        {
                            var parent = new VisualElement()
                            {
                                style = { flexGrow = 1, flexDirection = FlexDirection.Row, }
                            };
                            var container = new IMGUIContainer();
                            container.onGUIHandler += () =>
                            {
                                cb?.Invoke();
                            };
                            parent.Add(container);
                            toolbarZone.Add(parent);
                        }
                    }
                }
            }

            if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length)
            {
                List<string> scenePaths = new();
                List<string> sceneNames = new();

                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == null || scene.path.StartsWith("Assets") == false)
                        continue;

                    string scenePath = Application.dataPath + scene.path.Substring(6);

                    scenePaths.Add(scenePath);
                    sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
                }

                _scenePaths = scenePaths.ToArray();
                _sceneNames = sceneNames.ToArray();
            }
        }

        private static void OnGUI()
        {
            if (!active) return;

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                {
                    string activeSceneName = EditorSceneManager.GetActiveScene().name;
                    int activeSceneIndex = Array.FindIndex(_sceneNames, s=> s == activeSceneName);

                    int newSceneIndex = EditorGUILayout.Popup(
                        activeSceneIndex,
                        _sceneNames,
                        GUILayout.Width(200.0f)
                    );

                    if (newSceneIndex != activeSceneIndex && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(
                            _scenePaths[newSceneIndex],
                            OpenSceneMode.Single
                        );
                    }
                }
            }
        }
    
        public enum ToolbarZone {
            ToolbarZoneRightAlign,
            ToolbarZoneLeftAlign
        }
    }
}