using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ES3Editor
{
	public class ES3Window : EditorWindow
	{
		private SubWindow[] windows = null;

		public SubWindow currentWindow;

		[MenuItem("Window/Easy Save 3", false, 1000)]
        [MenuItem("Assets/Easy Save 3/Open Easy Save 3 Window", false, 1000)]
		public static void Init()
		{
			// Get existing open window or if none, make a new one:
			ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if(window != null)
			    window.Show();
		}

		public static void InitAndShowHome()
		{
			// Get existing open window or if none, make a new one:
			ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(HomeWindow));
            }
		}

        [MenuItem("Tools/Easy Save 3/Auto Save", false, 100)]
        public static void InitAndShowAutoSave()
		{
			// Get existing open window or if none, make a new one:
			ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(AutoSaveWindow));
            }
		}

        public static void InitAndShowReferences()
        {
            // Get existing open window or if none, make a new one:
            ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(ReferencesWindow));
            }
        }

        [MenuItem("Tools/Easy Save 3/Types", false, 100)]
        public static void InitAndShowTypes()
        {
            // Get existing open window or if none, make a new one:
            ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(TypesWindow));
            }
        }

        public static void InitAndShowTypes(System.Type type)
        {
            // Get existing open window or if none, make a new one:
            ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                var typesWindow = (TypesWindow)window.SetCurrentWindow(typeof(TypesWindow));
                typesWindow.SelectType(type);
            }
        }

        [MenuItem("Tools/Easy Save 3/Settings", false, 100)]
        public static void InitAndShowSettings()
        {
            // Get existing open window or if none, make a new one:
            ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(SettingsWindow));
            }
        }

        [MenuItem("Tools/Easy Save 3/Tools", false, 100)]
        public static void InitAndShowTools()
        {
            // Get existing open window or if none, make a new one:
            ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
            if (window != null)
            {
                window.Show();
                window.SetCurrentWindow(typeof(ToolsWindow));
            }
        }

        public void InitSubWindows()
		{
			windows = new SubWindow[]{
				new HomeWindow(this),
				new SettingsWindow(this),
				new ToolsWindow(this),
				new TypesWindow(this),
				new AutoSaveWindow(this)
				//, new ReferencesWindow(this)
			};
		}

		void OnLostFocus()
		{
			if(currentWindow != null)
				currentWindow.OnLostFocus();
		}

        private void OnFocus()
        {
            if (currentWindow != null)
                currentWindow.OnFocus();
        }

        void OnDestroy()
		{
			if(currentWindow != null)
				currentWindow.OnDestroy();
		}

		void OnEnable()
		{
			if(windows == null)
				InitSubWindows();
			// Set the window name and icon.
			var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ES3Settings.PathToEasySaveFolder()+"Editor/es3Logo16x16.png");
			titleContent = new GUIContent("Easy Save", icon);

			// Get the last opened window and open it.
			if(currentWindow == null)
			{
				var currentWindowName = EditorPrefs.GetString("ES3Editor.Window.currentWindow", windows[0].name);
				for(int i=0; i<windows.Length; i++)
				{
					if(windows[i].name == currentWindowName)
					{
						currentWindow = windows[i];
						break;
					}
				}
			}
		}

        private void OnHierarchyChange()
        {
            if (currentWindow != null)
                currentWindow.OnHierarchyChange();
        }

        void OnGUI()
		{
			var style = EditorStyle.Get;

			// Display the menu.
			EditorGUILayout.BeginHorizontal();

			for(int i=0; i<windows.Length; i++)
			{
				if(GUILayout.Button(windows[i].name, currentWindow == windows[i] ? style.menuButtonSelected : style.menuButton))
					SetCurrentWindow(windows[i]);
			}

			EditorGUILayout.EndHorizontal();

			if(currentWindow != null)
				currentWindow.OnGUI();
		}

		void SetCurrentWindow(SubWindow window)
		{
            if (currentWindow != null)
                currentWindow.OnLostFocus();
            currentWindow = window;
            currentWindow.OnFocus();
			EditorPrefs.SetString("ES3Editor.Window.currentWindow", window.name);
		}

		SubWindow SetCurrentWindow(System.Type type)
		{
			currentWindow.OnLostFocus();
			currentWindow = windows.First(w => w.GetType() == type);
			EditorPrefs.SetString("ES3Editor.Window.currentWindow", currentWindow.name);
            return currentWindow;
		}
			
		// Shows the Easy Save Home window if it's not been disabled.
		// This method is called from the Postprocessor.
		public static void OpenEditorWindowOnStart()
		{
			if(EditorPrefs.GetBool("Show ES3 Window on Start", true))
				ES3Window.InitAndShowHome();
			EditorPrefs.SetBool("Show ES3 Window on Start", false);
		}
	}

	public abstract class SubWindow
	{
		public string name;
		public EditorWindow parent;
		public abstract void OnGUI();

		public SubWindow(string name, EditorWindow parent)
		{
			this.name = name;
			this.parent = parent;
		}

		public virtual void OnLostFocus()
		{
		}

        public virtual void OnFocus()
        {
        }

		public virtual void OnDestroy()
		{
		}

        public virtual void OnHierarchyChange()
        {
        }
	}
}