// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/07/2018
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoboRyanTron.SceneReference
{
    /// <summary>
    /// Class used to serialize a reference to a scene asset that can be used
    /// at runtime in a build, when the asset can no longer be directly
    /// referenced. This caches the scene name based on the SceneAsset to use
    /// at runtime to load.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Exception that is raised when there is an issue resolving and
        /// loading a scene reference.
        /// </summary>
        public class SceneLoadException : Exception
        {
            public SceneLoadException(string message) : base(message)
            {}
        }
        
#if UNITY_EDITOR
        public UnityEditor.SceneAsset Scene;
#endif

        [Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
        public string SceneName;

        [SerializeField]
        private int sceneIndex = -1;

        [SerializeField]
        private bool sceneEnabled;

        private void ValidateScene()
        {
            if (string.IsNullOrEmpty(SceneName))
                throw new SceneLoadException("No scene specified.");
            
            if (sceneIndex < 0)
                throw new SceneLoadException("Scene " + SceneName + " is not in the build settings");
            
            if (!sceneEnabled)
                throw new SceneLoadException("Scene " + SceneName + " is not enabled in the build settings");
        }
        
        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            SceneManager.LoadScene(SceneName, mode);
        }
        
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (Scene != null)
            {
                string sceneAssetPath = UnityEditor.AssetDatabase.GetAssetPath(Scene);
                string sceneAssetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(sceneAssetPath);
                
                UnityEditor.EditorBuildSettingsScene[] scenes = 
                    UnityEditor.EditorBuildSettings.scenes;

                sceneIndex = -1;
                for (int i = 0; i < scenes.Length; i++)
                {
                    if (scenes[i].guid.ToString() == sceneAssetGUID)
                    {
                        sceneIndex = i;
                        sceneEnabled = scenes[i].enabled;
                        if (scenes[i].enabled)
                            SceneName = Scene.name;
                        break;
                    }
                }
            }
            else
            {
                SceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize() {}
    }
}