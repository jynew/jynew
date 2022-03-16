using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ES3Internal;

namespace ES3Editor
{
	[System.Serializable]
	public class AutoSaveWindow : SubWindow
	{
		public bool showAdvancedSettings = false;

		public ES3AutoSaveMgr mgr = null;

        private HierarchyItem[] hierarchy = null;
        public HierarchyItem selected = null;

        private Vector2 hierarchyScrollPosition = Vector2.zero;

        private bool sceneOpen = true;

        private string searchTerm = "";

        public AutoSaveWindow(EditorWindow window) : base("Auto Save", window){}

		public override void OnGUI()
		{
			Init();

			if(mgr == null)
			{
                EditorGUILayout.Space();
                if (GUILayout.Button("Enable Auto Save for this scene"))
                    mgr = ES3Postprocessor.AddManagerToScene().GetComponent<ES3AutoSaveMgr>();
                else
                    return;
			}

			var style = EditorStyle.Get;

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                using (var vertical = new EditorGUILayout.VerticalScope(style.areaPadded))
                {
                    //GUILayout.Label("Settings for current scene", style.heading);
                    mgr.saveEvent = (ES3AutoSaveMgr.SaveEvent)EditorGUILayout.EnumPopup("Save Event", mgr.saveEvent);
                    mgr.loadEvent = (ES3AutoSaveMgr.LoadEvent)EditorGUILayout.EnumPopup("Load Event", mgr.loadEvent);

                    EditorGUILayout.Space();

                    showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Show Advanced Settings");
                    if (showAdvancedSettings)
                    {
                        EditorGUI.indentLevel++;
                        mgr.key = EditorGUILayout.TextField("Key", mgr.key);
                        ES3SettingsEditor.Draw(mgr.settings);
                        EditorGUI.indentLevel--;
                    }
                }

                // Display the menu.
                using (var horizontal = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Scene", sceneOpen ? style.menuButtonSelected : style.menuButton))
                    {
                        sceneOpen = true;
                        OnFocus();
                    }
                    if (GUILayout.Button("Prefabs", sceneOpen ? style.menuButton : style.menuButtonSelected))
                    {
                        sceneOpen = false;
                        OnFocus();
                    }
                }

                //EditorGUILayout.HelpBox("Select the Components you want to be saved.\nTo maximise performance, only select the Components with variables which need persisting.", MessageType.None, true);

                if (hierarchy == null || hierarchy.Length == 0)
                {
                    EditorGUILayout.LabelField("Right-click a prefab and select 'Easy Save 3 > Enable Easy Save for Scene' to enable Auto Save for it.\n\nYour scene will also need to reference this prefab for it to be recognised.", style.area);
                    return;
                }

                using (var scrollView = new EditorGUILayout.ScrollViewScope(hierarchyScrollPosition, style.areaPadded))
                {
                    hierarchyScrollPosition = scrollView.scrollPosition;

                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(200)))
                    {
                        searchTerm = GUILayout.TextField(searchTerm, GUI.skin.FindStyle("ToolbarSeachTextField"));
                        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
                        {
                            // Remove focus if cleared
                            searchTerm = "";
                            GUI.FocusControl(null);
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    foreach (var go in hierarchy)
                        if (go != null)
                            go.DrawHierarchy(searchTerm.ToLowerInvariant());
                }
                if (changeCheck.changed)
                    EditorUtility.SetDirty(mgr);
            }
        }

		public void Init()
		{
            if (mgr == null)
            {
                var mgrs = Resources.FindObjectsOfTypeAll<ES3AutoSaveMgr>();
                if (mgrs.Length > 0)
                    mgr = mgrs[0];
            }

            if (hierarchy == null)
                OnFocus();
        }

        public override void OnFocus()
        {

            GameObject[] parentObjects;
            if (sceneOpen)
                parentObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            else // Prefabs
            {
                var prefabs = ES3ReferenceMgr.Current.prefabs;
                parentObjects = new GameObject[prefabs.Count];
                for (int i = 0; i < prefabs.Count; i++)
                    if(prefabs[i] != null)
                        parentObjects[i] = prefabs[i].gameObject;
            }
            hierarchy = new HierarchyItem[parentObjects.Length];
            for (int i = 0; i < parentObjects.Length; i++)
                if(parentObjects[i] != null)
                    hierarchy[i] = new HierarchyItem(parentObjects[i].transform, null, this);
        }

        public class HierarchyItem
        {
            private ES3AutoSave autoSave;
            private Transform t;
            private Component[] components = null;
            // Immediate children of this GameObject
            private HierarchyItem[] children = new HierarchyItem[0];
            private bool showComponents = false;
            //private AutoSaveWindow window;

            public HierarchyItem(Transform t, HierarchyItem parent, AutoSaveWindow window)
            {
                this.autoSave = t.GetComponent<ES3AutoSave>();
                this.t = t;
                this.components = t.GetComponents<Component>();

                children = new HierarchyItem[t.childCount];
                for (int i = 0; i < t.childCount; i++)
                    children[i] = new HierarchyItem(t.GetChild(i), this, window);

                //this.window = window;
            }

            public void MergeDown(ES3AutoSave autoSave)
            {
                if (this.autoSave != autoSave)
                {
                    if (this.autoSave != null)
                    {
                        autoSave.componentsToSave.AddRange(autoSave.componentsToSave);
                        Object.DestroyImmediate(this.autoSave);
                    }
                    this.autoSave = autoSave;
                }

                foreach (var child in children)
                    MergeDown(autoSave);
            }

            public void DrawHierarchy(string searchTerm)
            {
                bool containsSearchTerm = false;
                if (t != null)
                {
                    if (containsSearchTerm = t.name.ToLowerInvariant().Contains(searchTerm))
                    {
                        GUIContent saveIcon;
                        EditorGUIUtility.SetIconSize(new Vector2(16, 16));

                        if (HasSelectedComponents())
                            saveIcon = new GUIContent(t.name, EditorStyle.Get.saveIconSelected, "There are Components on this GameObject which will be saved.");
                        else
                            saveIcon = new GUIContent(t.name, EditorStyle.Get.saveIconUnselected, "No Components on this GameObject will be saved");

                        GUIStyle style = GUI.skin.GetStyle("Foldout");
                        if (Selection.activeTransform == t)
                        {
                            style = new GUIStyle(style);
                            style.fontStyle = FontStyle.Bold;
                        }

                        var open = EditorGUILayout.Foldout(showComponents, saveIcon, style);
                        if (open)
                        {
                            // Ping the GameObject if this was previously closed
                            if (showComponents != open)
                                EditorGUIUtility.PingObject(t.gameObject);
                            DrawComponents();
                        }
                        showComponents = open;

                        EditorGUI.indentLevel += 1;
                    }
                }

                // Draw children
                if(children != null)
                    foreach (var child in children)
                        if(child != null)
                            child.DrawHierarchy(searchTerm);

                if(containsSearchTerm)
                    EditorGUI.indentLevel-=1;
            }

            public void DrawComponents()
            {
                EditorGUI.indentLevel+=3;
                using (var scope = new EditorGUILayout.VerticalScope())
                {
                    bool toggle;
                    toggle = EditorGUILayout.ToggleLeft("active", autoSave != null ? autoSave.saveActive : false);
                    if((autoSave = (toggle && autoSave == null) ? t.gameObject.AddComponent<ES3AutoSave>() : autoSave) != null)
                        autoSave.saveActive = toggle;

                    toggle = EditorGUILayout.ToggleLeft("hideFlags", autoSave != null ? autoSave.saveHideFlags : false);
                    if ((autoSave = (toggle && autoSave == null) ? t.gameObject.AddComponent<ES3AutoSave>() : autoSave) != null)
                        autoSave.saveHideFlags = toggle;

                    toggle = EditorGUILayout.ToggleLeft("layer", autoSave != null ? autoSave.saveLayer : false);
                    if ((autoSave = (toggle && autoSave == null) ? t.gameObject.AddComponent<ES3AutoSave>() : autoSave) != null)
                        autoSave.saveLayer = toggle;

                    toggle = EditorGUILayout.ToggleLeft("name", autoSave != null ? autoSave.saveName : false);
                        if ((autoSave = (toggle && autoSave == null) ? t.gameObject.AddComponent<ES3AutoSave>() : autoSave) != null)
                        autoSave.saveName = toggle;

                    toggle = EditorGUILayout.ToggleLeft("tag", autoSave != null ? autoSave.saveTag : false);
                    if ((autoSave = (toggle && autoSave == null) ? t.gameObject.AddComponent<ES3AutoSave>() : autoSave) != null)
                        autoSave.saveTag = toggle;

                    foreach (var component in components)
                    {
                        if (component == null)
                            continue;

                        using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                        {
                            bool saveComponent = false;
                            if (autoSave != null)
                                saveComponent = autoSave.componentsToSave.Contains(component);

                            var newValue = EditorGUILayout.ToggleLeft(EditorGUIUtility.ObjectContent(component, component.GetType()), saveComponent);
                            // If the checkbox has changed, we want to save or not save a Component
                            if (newValue != saveComponent)
                            {
                                if (autoSave == null)
                                {
                                    autoSave = t.gameObject.AddComponent<ES3AutoSave>();
                                    autoSave.saveChildren = false;
                                }
                                // If we've unchecked the box, remove the Component from the array.
                                if (newValue == false)
                                    autoSave.componentsToSave.Remove(component);
                                // Else, add it to the array.
                                else
                                    autoSave.componentsToSave.Add(component);
                            }
                            if(GUILayout.Button(EditorGUIUtility.IconContent("_Popup"), new GUIStyle("Label")))
                                ES3Window.InitAndShowTypes(component.GetType());
                        }
                    }
                }

                if(autoSave != null && (autoSave.componentsToSave == null || autoSave.componentsToSave.Count == 0) && !autoSave.saveActive && !autoSave.saveChildren && !autoSave.saveHideFlags && !autoSave.saveLayer && !autoSave.saveName && !autoSave.saveTag)
                {
                    Undo.DestroyObjectImmediate(autoSave);
                    autoSave = null;
                }
                EditorGUI.indentLevel-=3;
            }

            public bool HasSelectedComponents()
            {
                if (autoSave != null)
                    foreach (var component in components)
                        if (component != null && autoSave.componentsToSave.Contains(component))
                            return true;
                return false;
            }
        }
    }

}
