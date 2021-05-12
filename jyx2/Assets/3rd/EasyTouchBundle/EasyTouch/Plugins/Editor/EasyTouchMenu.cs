using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.EventSystems;
using HedgehogTeam.EasyTouch;

public static class EasyTouchMenu{

	[MenuItem ("GameObject/EasyTouch/EasyTouch", false, 0)]
	static void  AddEasyTouch(){

		Selection.activeObject = EasyTouch.instance.gameObject;
	}
				
}

/*

[MenuItem ("Window/GameAnalytics/Folder Structure/Revert to original", false, 601)]
static void RevertFolders ()
{
	if (!Directory.Exists(Application.dataPath + "/Plugins/GameAnalytics/"))
	{
		Debug.LogWarning("Folder structure incompatible, are you already using original folder structure, or have you manually changed the folder structure?");
		return;
	}
	
	if (!Directory.Exists(Application.dataPath + "/GameAnalytics/"))
		AssetDatabase.CreateFolder("Assets", "GameAnalytics");
	if (!Directory.Exists(Application.dataPath + "/GameAnalytics/Plugins"))
		AssetDatabase.CreateFolder("Assets/GameAnalytics", "Plugins");
	
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/Android", "Assets/GameAnalytics/Plugins/Android");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/Components", "Assets/GameAnalytics/Plugins/Components");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/Examples", "Assets/GameAnalytics/Plugins/Examples");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/Framework", "Assets/GameAnalytics/Plugins/Framework");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/iOS", "Assets/GameAnalytics/Plugins/iOS");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/Playmaker", "Assets/GameAnalytics/Plugins/Playmaker");
	AssetDatabase.MoveAsset("Assets/Plugins/GameAnalytics/WebPlayer", "Assets/GameAnalytics/Plugins/WebPlayer");
	
	AssetDatabase.MoveAsset("Assets/Editor/GameAnalytics", "Assets/GameAnalytics/Editor");
	
	AssetDatabase.DeleteAsset("Assets/Plugins/GameAnalytics");
	AssetDatabase.DeleteAsset("Assets/Editor/GameAnalytics");
	
	Debug.Log("Project must be reloaded when reverting folder structure.");
	EditorApplication.OpenProject(Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length, "Assets".Length));
}
*/

/*
#if true

#endif*/

/*
	[MenuItem ("Window/Easy Touch/Folder Structure/Switch to JS", false, 100)]
	static void JsFolders(){
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

	[MenuItem ("Window/EasyTouch/Folder Structure/Revert to original", false, 200)]
	static void CFolders(){

		if (!Directory.Exists(Application.dataPath + "/Plugins/EasyTouch/")){
			Debug.LogWarning("Folder structure incompatible, are you already using original folder structure, or have you manually changed the folder structure?");
			return;
		}

		AssetDatabase.MoveAsset("Assets/Plugins/EasyTouch/Components","Assets/EasyTouchBundle/EasyTouch/Plugins/Components");
		AssetDatabase.MoveAsset("Assets/Plugins/EasyTouch/Engine","Assets/EasyTouchBundle/EasyTouch/Plugins/Engine");

		AssetDatabase.DeleteAsset("Assets/Plugins/EasyTouch");

		Debug.Log("Project must be reloaded when reverting folder structure.");
		EditorApplication.OpenProject(Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length, "Assets".Length));
	}*/