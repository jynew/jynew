using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ES3Internal;

namespace ES3Editor
{
	public class SettingsWindow : SubWindow
	{
		public ES3Defaults editorSettings = null;
		public ES3SerializableSettings settings = null;
		public SerializedObject so = null;
		public SerializedProperty assemblyNamesProperty = null;

        private Vector2 scrollPos = Vector2.zero;

		public SettingsWindow(EditorWindow window) : base("Settings", window){}

		public override void OnGUI()
		{
			if(settings == null || editorSettings == null || assemblyNamesProperty == null)
				Init();

            var style = EditorStyle.Get;

            var labelWidth = EditorGUIUtility.labelWidth;


            EditorGUI.BeginChangeCheck();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, style.area))
            {
                scrollPos = scrollView.scrollPosition;

                EditorGUIUtility.labelWidth = 160;

                GUILayout.Label("Runtime Settings", style.heading);

                using (new EditorGUILayout.VerticalScope(style.area))
                {
                    ES3SettingsEditor.Draw(settings);
                }

                GUILayout.Label("Debug Settings", style.heading);

                using (new EditorGUILayout.VerticalScope(style.area))
                {
                    EditorGUIUtility.labelWidth = 100;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Log Info");
                        editorSettings.logDebugInfo = EditorGUILayout.Toggle(editorSettings.logDebugInfo);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Log Warnings");
                        editorSettings.logWarnings = EditorGUILayout.Toggle(editorSettings.logWarnings);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Log Errors");
                        editorSettings.logErrors = EditorGUILayout.Toggle(editorSettings.logErrors);
                    }

                    EditorGUILayout.Space();
                }

                GUILayout.Label("Editor Settings", style.heading);

                using (new EditorGUILayout.VerticalScope(style.area))
                {
                    EditorGUIUtility.labelWidth = 170;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Auto Update References");
                        editorSettings.autoUpdateReferences = EditorGUILayout.Toggle(editorSettings.autoUpdateReferences);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Use Global References");

                        var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                        bool useGlobalReferences = !symbols.Contains("ES3GLOBAL_DISABLED");
                        if(EditorGUILayout.Toggle(useGlobalReferences) != useGlobalReferences)
                        {
                            // Remove the existing symbol even if we're disabling global references, just incase it's already in there.
                            symbols = symbols.Replace("ES3GLOBAL_DISABLED;", ""); // With semicolon
                            symbols = symbols.Replace("ES3GLOBAL_DISABLED", "");  // Without semicolon

                            // Add the symbol if useGlobalReferences is currently true, meaning that we want to disable it.
                            if (useGlobalReferences)
                                symbols = "ES3GLOBAL_DISABLED;" + symbols;

                            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);

                            if(useGlobalReferences)
                                EditorUtility.DisplayDialog("Global references disabled for build platform", "This will only disable Global References for this build platform. To disable it for other build platforms, open that platform in the Build Settings and uncheck this box again.", "Ok");
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Add All Prefabs to Manager");
                        editorSettings.addAllPrefabsToManager = EditorGUILayout.Toggle(editorSettings.addAllPrefabsToManager);
                    }

                    EditorGUILayout.Space();
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(editorSettings);

            EditorGUIUtility.labelWidth = labelWidth; // Set the label width back to default
		}

		public void Init()
		{
            editorSettings = ES3Settings.defaultSettingsScriptableObject;

			settings = editorSettings.settings;
			/*so = new SerializedObject(editorSettings);
			var settingsProperty = so.FindProperty("settings");
			assemblyNamesProperty = settingsProperty.FindPropertyRelative("assemblyNames");*/
			
		}
	}

}
