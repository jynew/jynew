using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(CFX_SpawnSystem))]
public class CFX_SpawnSystemEditor : Editor
{
	public override void OnInspectorGUI()
	{
		(this.target as CFX_SpawnSystem).hideObjectsInHierarchy = GUILayout.Toggle((this.target as CFX_SpawnSystem).hideObjectsInHierarchy, "Hide Preloaded Objects in Hierarchy");
		
		GUI.SetNextControlName("DragDropBox");
		EditorGUILayout.HelpBox("Drag GameObjects you want to preload here!\n\nTIP:\nUse the Inspector Lock at the top right to be able to drag multiple objects at once!", MessageType.None);
		
		for(int i = 0; i < (this.target as CFX_SpawnSystem).objectsToPreload.Length; i++)
		{
			GUILayout.BeginHorizontal();
			
			(this.target as CFX_SpawnSystem).objectsToPreload[i] = (GameObject)EditorGUILayout.ObjectField((this.target as CFX_SpawnSystem).objectsToPreload[i], typeof(GameObject), true);
			EditorGUILayout.LabelField(new GUIContent("times","Number of times to copy the effect\nin the pool, i.e. the max number of\ntimes the object will be used\nsimultaneously"), GUILayout.Width(40));
			int nb = EditorGUILayout.IntField("", (this.target as CFX_SpawnSystem).objectsToPreloadTimes[i], GUILayout.Width(50));
			if(nb < 1)
				nb = 1;
			(this.target as CFX_SpawnSystem).objectsToPreloadTimes[i] = nb;
			
			if(GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(24)))
			{
				ArrayUtility.RemoveAt<GameObject>(ref (this.target as CFX_SpawnSystem).objectsToPreload, i);
				ArrayUtility.RemoveAt<int>(ref (this.target as CFX_SpawnSystem).objectsToPreloadTimes, i);
			}
			
			GUILayout.EndHorizontal();
		}
		
		if(Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated)
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			
			if(Event.current.type == EventType.DragPerform)
			{
				foreach(Object o in DragAndDrop.objectReferences)
				{
					if(o is GameObject)
					{
						bool already = false;
						foreach(GameObject otherObj in (this.target as CFX_SpawnSystem).objectsToPreload)
						{
							if(o == otherObj)
							{
								already = true;
								Debug.LogWarning("CFX_SpawnSystem: Object has already been added: " + o.name);
								break;
							}
						}
						
						if(!already)
						{
							ArrayUtility.Add<GameObject>(ref (this.target as CFX_SpawnSystem).objectsToPreload, (GameObject)o);
							ArrayUtility.Add<int>(ref (this.target as CFX_SpawnSystem).objectsToPreloadTimes, 1);
						}
					}
				}
			}
		}
	}
}
