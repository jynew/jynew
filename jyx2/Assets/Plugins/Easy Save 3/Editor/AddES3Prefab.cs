using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ES3Internal;
using System.Linq;

namespace ES3Editor
{
	public class AddES3Prefab : Editor 
	{
        [MenuItem("GameObject/Easy Save 3/Enable Easy Save for Prefab(s)", false, 1001)]
        [MenuItem("Assets/Easy Save 3/Enable Easy Save for Prefab(s)", false, 1001)]
        public static void Enable()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                return;

            foreach (var obj in Selection.gameObjects)
            {
                // Don't add the Component to a GameObject which already has it.
                if (obj == null  || (obj.GetComponent<ES3Prefab>() != null))
                    continue;

                var go = obj;

                #if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.GetPrefabInstanceStatus(go) != PrefabInstanceStatus.NotAPrefab)
                {
                    go = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go);
                    if (go == null)
                        continue;
                }
                #else
			    if(PrefabUtility.GetPrefabType(go) != PrefabType.Prefab)
			    {
				    go = (GameObject)PrefabUtility.GetPrefabParent(go);
				    if(go == null)
					    continue;
			    }
                #endif

                var es3Prefab = Undo.AddComponent<ES3Prefab>(go);
                es3Prefab.GeneratePrefabReferences();

                if (ES3ReferenceMgr.Current != null)
                {
                    ES3ReferenceMgr.Current.AddPrefab(es3Prefab);
                    EditorUtility.SetDirty(ES3ReferenceMgr.Current);
                }
            }
		}

		[MenuItem("GameObject/Easy Save 3/Enable Easy Save for Prefab", true, 1001)]
		[MenuItem("Assets/Easy Save 3/Enable Easy Save for Prefab", true, 1001)]
		public static bool Validate()
		{
            return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
		}
	}
}
