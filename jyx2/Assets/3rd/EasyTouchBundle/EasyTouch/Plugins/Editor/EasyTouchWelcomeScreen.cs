using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;

[InitializeOnLoad]
public class EasyTouchWelcomeScreen : EditorWindow {

	private const string VERSION = "5.0.0";
	private const string PLAYMAKERADDONVERSION = "1.0,0";
	private const string SUPPORTURL = "http://www.thehedgehogteam.com/Forum/";
	private const string OFFICIALTOPIC = "http://forum.unity3d.com/threads/released-easy-touch.135192/";
	private const float width = 500;
	private const float height = 440;

	private const string PREFSHOWATSTARTUP = "EasyTouch.ShowWelcomeScreen";

	private static bool showAtStartup;
	private static GUIStyle imgHeader;
	private static bool interfaceInitialized;
	private static Texture supportIcon;
	private static Texture cIcon;
	private static Texture jsIcon;
	private static Texture playmakerIcon;
	private static Texture topicIcon;


	[MenuItem("Tools/Easy Touch/Welcome Screen", false, 0)]
	[MenuItem("Window/Easy Touch/Welcome Screen", false, 0)]
	public static void OpenWelcomeWindow(){
		GetWindow<EasyTouchWelcomeScreen>(true);
	}

	[MenuItem ("Tools/Easy Touch/Folder Structure/Switch to JS", false, 100)]
	public static void JsFolders(){
		// EasyTouch is here
		if (!Directory.Exists(Application.dataPath + "/EasyTouchBundle/EasyTouch/Plugins/")){
			Debug.LogWarning("Folder structure incompatible, did you already switch to JS folder structure, or have you manually changed the folder structure?");
			return;
		}
		
		// Create Structure
		if (!Directory.Exists(Application.dataPath + "/Plugins/"))
			AssetDatabase.CreateFolder("Assets", "Plugins");
		if (!Directory.Exists(Application.dataPath + "/Plugins/EasyTouch"))
			AssetDatabase.CreateFolder("Assets/Plugins", "EasyTouch");
		
		AssetDatabase.MoveAsset("Assets/EasyTouchBundle/EasyTouch/Plugins/Components","Assets/Plugins/EasyTouch/Components");
		AssetDatabase.MoveAsset("Assets/EasyTouchBundle/EasyTouch/Plugins/Engine","Assets/Plugins/EasyTouch/Engine");
		
		// Refresh database
		AssetDatabase.Refresh();
	}
	
	[MenuItem ("Tools/Easy Touch/Folder Structure/Revert to original", false, 200)]
	public static void CFolders(){
		
		if (!Directory.Exists(Application.dataPath + "/Plugins/EasyTouch/")){
			Debug.LogWarning("Folder structure incompatible, are you already using original folder structure, or have you manually changed the folder structure?");
			return;
		}
		
		AssetDatabase.MoveAsset("Assets/Plugins/EasyTouch/Components","Assets/EasyTouchBundle/EasyTouch/Plugins/Components");
		AssetDatabase.MoveAsset("Assets/Plugins/EasyTouch/Engine","Assets/EasyTouchBundle/EasyTouch/Plugins/Engine");
		
		AssetDatabase.DeleteAsset("Assets/Plugins/EasyTouch");
		
		Debug.Log("Project must be reloaded when reverting folder structure.");
		EditorApplication.OpenProject(Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length, "Assets".Length));
	}

	static EasyTouchWelcomeScreen(){
		//EditorApplication.playmodeStateChanged -= OnPlayModeChanged;
		//EditorApplication.playmodeStateChanged += OnPlayModeChanged;
		
		//showAtStartup = EditorPrefs.GetBool(PREFSHOWATSTARTUP, true);
		
		//if (showAtStartup){
		//	EditorApplication.update -= OpenAtStartup;
		//	EditorApplication.update += OpenAtStartup;
		//}
	}

	static void OpenAtStartup(){
		OpenWelcomeWindow();
		EditorApplication.update -= OpenAtStartup;
	}

	static void OnPlayModeChanged(){
		EditorApplication.update -= OpenAtStartup;
		//EditorApplication.playmodeStateChanged -= OnPlayModeChanged;
	}
	
	void OnEnable(){
		titleContent = new GUIContent("Welcome To Easy Touch"); 

		maxSize = new Vector2(width, height);
		minSize = maxSize;
	}	

	public void OnGUI(){

		InitInterface();
		GUI.Box(new Rect(0, 0, width, 60), "", imgHeader);
		GUI.Label( new Rect(width-90,45,200,20),new GUIContent("Version : " +VERSION));
		GUILayoutUtility.GetRect(position.width, 64);

		GUILayout.BeginVertical();

		if (Button(playmakerIcon,"Install PlayMaker add-on","Requires PlayMaker 1.8 or higher.")){
			InstallPlayMakerAddOn();
		}

		if (Button(jsIcon,"Switch to JS","Switch Easy Touch folder structure to be used with Java Script.")){
			JsFolders();
		}

		if (Button(cIcon,"Revert to original","Revert Easy Touch folder structure to original (C#).")){
			CFolders();
		}

		if (Button(supportIcon,"Community Forum","You need your invoice number to register, if you are a new user.")){
			Application.OpenURL(SUPPORTURL);
		}

		if (Button(supportIcon,"Official topic","Official topic on Unity Forum.",3)){
			Application.OpenURL(OFFICIALTOPIC);
		}

		bool show = GUILayout.Toggle(showAtStartup, "Show at startup");
		if (show != showAtStartup){
			showAtStartup = show;
			EditorPrefs.SetBool(PREFSHOWATSTARTUP, showAtStartup);
		}

		GUILayout.EndVertical();

	}

	void InitInterface(){

		if (!interfaceInitialized){
			imgHeader = new GUIStyle();
			imgHeader.normal.background = (Texture2D)Resources.Load("EasyTouchWelcome");
			imgHeader.normal.textColor = Color.white;

			supportIcon = (Texture)Resources.Load("btn_Support") as Texture;
			playmakerIcon = (Texture)Resources.Load("btn_PlayMaker") as Texture;
			cIcon = (Texture)Resources.Load("btn_cs") as Texture;
			jsIcon = (Texture)Resources.Load("btn_js") as Texture;

			interfaceInitialized = true;
		}
	}
	
	bool Button(Texture texture, string heading, string body, int space=10){

		GUILayout.BeginHorizontal();
		
		GUILayout.Space(54);
		GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(48));
		GUILayout.Space(10);
		
		GUILayout.BeginVertical();
		GUILayout.Space(1);
		GUILayout.Label(heading, EditorStyles.boldLabel);
		GUILayout.Label(body);
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
			
		bool returnValue = false;
		if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)){
			returnValue = true;
		}
		
		GUILayout.Space(space);

		return returnValue;
	}

	private void InstallPlayMakerAddOn(){
			
		string package = "Assets/EasyTouchBundle/Install/PlayMakerAddOn.unitypackage";
		
		try
		{
			AssetDatabase.ImportPackage(package, true);
		}
		catch (Exception)
		{
			Debug.LogError("Failed to import package: " + package);
			throw;
		}

	}
}
