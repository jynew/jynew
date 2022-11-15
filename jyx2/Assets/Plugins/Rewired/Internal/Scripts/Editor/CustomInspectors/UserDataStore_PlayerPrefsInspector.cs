// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor {

    using UnityEngine;
    using UnityEditor;
    using Rewired;
    using Rewired.Data;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [CustomEditor(typeof(UserDataStore_PlayerPrefs))]
    public sealed class UserDataStore_PlayerPrefsInspector : UnityEditor.Editor {

        private bool showDebugOptions;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            showDebugOptions = EditorGUILayout.Foldout(showDebugOptions, new GUIContent("Debug Options"));

            if(showDebugOptions) {
                GUILayout.Space(15);
                if(GUILayout.Button(new GUIContent("Clear All PlayerPrefs Data", "This will clear all player prefs data! Use this with caution!"))) {
                    if(EditorUtility.DisplayDialog("Clear All PlayerPrefs Data", "WARNING: This will delete ALL PlayerPrefs data in your project, not just the keys used by Rewired (because there is no way to search for keys by prefix). This cannot be undone! Are you sure?", "DELETE", "Cancel")) {
                        PlayerPrefs.DeleteAll();
                    }
                }
                GUILayout.Space(15);
            }
        }


    }
}