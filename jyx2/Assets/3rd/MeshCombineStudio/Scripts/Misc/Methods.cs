using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace MeshCombineStudio
{
    public enum CustomHideFlags
    {
        HideInHierarchy = 1,
        HideInInspector = 2,
        DontSaveInEditor = 4,
        NotEditable = 8,
        DontSaveInBuild = 16,
        DontUnloadUnusedAsset = 32,
    }

    static public class Methods
    {
        public static HideFlags CustomToHideFlags(CustomHideFlags customHideFlags)
        {
            HideFlags hideFlags = HideFlags.None;

            if ((customHideFlags & CustomHideFlags.HideInHierarchy) != 0) hideFlags |= HideFlags.HideInHierarchy;
            if ((customHideFlags & CustomHideFlags.HideInInspector) != 0) hideFlags |= HideFlags.HideInInspector;
            if ((customHideFlags & CustomHideFlags.DontSaveInEditor) != 0) hideFlags |= HideFlags.DontSaveInEditor;
            if ((customHideFlags & CustomHideFlags.NotEditable) != 0) hideFlags |= HideFlags.NotEditable;

            if ((customHideFlags & CustomHideFlags.DontSaveInBuild) != 0) hideFlags |= HideFlags.DontSaveInBuild;
            if ((customHideFlags & CustomHideFlags.DontUnloadUnusedAsset) != 0) hideFlags |= HideFlags.DontUnloadUnusedAsset;

            return hideFlags;
        }

        public static CustomHideFlags HideFlagsToCustom(HideFlags hideFlags)
        {
            CustomHideFlags customHideFlags = 0;
            if ((hideFlags & HideFlags.HideInHierarchy) != 0) customHideFlags |= CustomHideFlags.HideInHierarchy;
            if ((hideFlags & HideFlags.HideInInspector) != 0) customHideFlags |= CustomHideFlags.HideInInspector;
            if ((hideFlags & HideFlags.DontSaveInEditor) != 0) customHideFlags |= CustomHideFlags.DontSaveInEditor;
            if ((hideFlags & HideFlags.NotEditable) != 0) customHideFlags |= CustomHideFlags.NotEditable;

            if ((hideFlags & HideFlags.DontSaveInBuild) != 0) customHideFlags |= CustomHideFlags.DontSaveInBuild;
            if ((hideFlags & HideFlags.DontUnloadUnusedAsset) !=0) customHideFlags |= CustomHideFlags.DontUnloadUnusedAsset;

            return customHideFlags;
        }

        public static int GetFirstLayerOfLayerMask(LayerMask layerMask)
        {
            for (int i = 0; i < 32; i++)
            {
                int layer = 1 << i;
                if ((i & layerMask) != 0) return layer;
            }

            return -1;
        }

        public static bool IsLayerInLayerMask(LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static void SetMeshRenderersActive(FastList<MeshRenderer> mrs, bool active)
        {
            for (int i = 0; i < mrs.Count; i++)
            {
                mrs.items[i].enabled = active;
            }
        }

        public static void SetCachedGOSActive(FastList<CachedGameObject> cachedGOS, bool active)
        {
            for (int i = 0; i < cachedGOS.Count; i++)
            {
                cachedGOS.items[i].mr.enabled = active;
            }
        }

        static public void SetTag(GameObject go, string tag)
        {
            Transform[] tArray = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < tArray.Length; i++) { tArray[i].tag = tag; }
        }

        static public void SetTagWhenCollider(GameObject go, string tag)
        {
            Transform[] tArray = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < tArray.Length; i++)
            {
                if (tArray[i].GetComponent<Collider>() != null) tArray[i].tag = tag;
            }
        }

        static public void SetTagAndLayer(GameObject go, string tag, int layer)
        {
            // Debug.Log("Layer " + layer);
            Transform[] tArray = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < tArray.Length; i++) { tArray[i].tag = tag; tArray[i].gameObject.layer = layer; }
        }

        static public void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;
            Transform[] tArray = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < tArray.Length; i++) tArray[i].gameObject.layer = layer;
        }

        static public bool LayerMaskContainsLayer(int layerMask, int layer)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        static public int GetFirstLayerInLayerMask(int layerMask)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((layerMask & Mathw.bits[i]) != 0) return i;
            }

            return -1;
        }

        static public bool Contains(string compare, string name)
        {
            List<string> cuts = new List<string>();
            int index;

            do
            {
                index = name.IndexOf("*");

                if (index != -1)
                {
                    if (index != 0) { cuts.Add(name.Substring(0, index)); }
                    if (index != name.Length - 1) { name = name.Substring(index + 1); }
                    else break;
                }
            }
            while (index != -1);

            cuts.Add(name);

            for (int i = 0; i < cuts.Count; i++)
            {
                //Debug.Log(cuts.items[i] +" " + compare);
                if (!compare.Contains(cuts[i])) return false;
            }
            //Debug.Log("Passed");
            return true;
        }

        static public T[] Search<T>(GameObject parentGO = null)
        {
            GameObject[] gos = null;
            if (parentGO == null) {
                gos = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            }

            else gos = new GameObject[] { parentGO };

            if (gos == null) return null;

            if (typeof(T) == typeof(GameObject))
            {
                List<GameObject> list = new List<GameObject>();
                for (int i = 0; i < gos.Length; i++)
                {
                    Transform[] transforms = gos[i].GetComponentsInChildren<Transform>(true);
                    for (int j = 0; j < transforms.Length; j++) list.Add(transforms[j].gameObject);
                }
                return list.ToArray() as T[];
            }
            else
            {
                if (parentGO == null)
                {
                    List<T> list = new List<T>();
                    for (int i = 0; i < gos.Length; i++)
                    {
                        list.AddRange(gos[i].GetComponentsInChildren<T>(true));
                    }
                    return list.ToArray();
                }
                else return parentGO.GetComponentsInChildren<T>(true);
            }
        }

        static public FastList<GameObject> GetAllRootGameObjects()
        {
            FastList<GameObject> list = new FastList<GameObject>();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.isLoaded) list.AddRange(scene.GetRootGameObjects());
            }

            return list;
        }

        static public T[] SearchParent<T>(GameObject parentGO, bool searchInActiveGameObjects) where T : Component
        {
            if (parentGO == null) return SearchAllScenes<T>(searchInActiveGameObjects).ToArray();

            if (!searchInActiveGameObjects && !parentGO.activeInHierarchy) return null;

            if (typeof(T) == typeof(GameObject))
            {
                var ts = parentGO.GetComponentsInChildren<Transform>(searchInActiveGameObjects);
                GameObject[] gos = new GameObject[ts.Length];
                for (int i = 0; i < gos.Length; i++) gos[i] = ts[i].gameObject;
                return gos as T[];
            }

            return parentGO.GetComponentsInChildren<T>(searchInActiveGameObjects);
        }

        static public T[] SearchScene<T>(UnityEngine.SceneManagement.Scene scene, bool searchInActiveGameObjects) where T : Component
        {
            if (!scene.isLoaded) return null;

            var gos = scene.GetRootGameObjects();

            var list = new FastList<T>();

            foreach (var go in gos) list.AddRange(SearchParent<T>(go, searchInActiveGameObjects));

            return list.ToArray();
        }

        static public FastList<T> SearchAllScenes<T>(bool searchInActiveGameObjects) where T : Component
        {
            var list = new FastList<T>();

            FastList<GameObject> gos = GetAllRootGameObjects();

            for (int i = 0; i < gos.Count; i++)
            {
                var result = SearchParent<T>(gos.items[i], searchInActiveGameObjects);

                list.AddRange(result);
            }

            return list;
        }

        static public T Find<T>(GameObject parentGO, string name) where T : UnityEngine.Component
        {
            T[] gos = SearchParent<T>(parentGO, true);

            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i].name == name) return gos[i];
            }
            return null;
        }

        static public void SetCollidersActive(Collider[] colliders, bool active, string[] nameList)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                for (int j = 0; j < nameList.Length; j++)
                {
                    if (colliders[i].name.Contains(nameList[j])) colliders[i].enabled = active;
                }
            }
        }

        static public void SelectChildrenWithMeshRenderer(GameObject[] parentGOs)
        {
#if UNITY_EDITOR
            FastList<MeshRenderer> mrList = new FastList<MeshRenderer>(1024);

            for (int i = 0; i < parentGOs.Length; i++)
            {
                MeshRenderer[] mrs = parentGOs[i].GetComponentsInChildren<MeshRenderer>();
                mrList.AddRange(mrs);
            }

            GameObject[] gos = new GameObject[mrList.Count];

            for (int i = 0; i < mrList.Count; i++) gos[i] = mrList.items[i].gameObject;

            UnityEditor.Selection.objects = gos;
#endif
        }

        static public void SelectChildrenWithMeshRenderer(Transform t)
        {
#if UNITY_EDITOR
            MeshRenderer[] mrs = t.GetComponentsInChildren<MeshRenderer>();

            GameObject[] gos = new GameObject[mrs.Length];

            for (int i = 0; i < mrs.Length; i++) gos[i] = mrs[i].gameObject;

            UnityEditor.Selection.objects = gos;
#endif
        }

        static public void DestroyChildren(Transform t)
        {
            while (t.childCount > 0)
            {
                Transform child = t.GetChild(0);
                child.parent = null;
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        static public void Destroy(GameObject go)
        {
            if (go == null) return;

#if UNITY_EDITOR
                GameObject.DestroyImmediate(go);
#else
                GameObject.Destroy(go);
#endif
        }

        static public void Destroy(Component c)
        {
            if (c == null) return;

#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(c);
#else
            UnityEngine.Object.Destroy(c);
#endif
        }

        static public void SetChildrenActive(Transform t, bool active)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                child.gameObject.SetActive(active);
            }
        }

        static public void SnapBoundsAndPreserveArea(ref Bounds bounds, float snapSize, Vector3 offset)
        {
            Vector3 newCenter = Mathw.Snap(bounds.center, snapSize) + offset;
            bounds.size += Mathw.Abs(newCenter - bounds.center) * 2;
            bounds.center = newCenter;
        }

        static public void ListRemoveAt<T>(List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        static public void CopyComponent(Component component, GameObject target)
        {
            Type type = component.GetType();
            target.AddComponent(type);
            PropertyInfo[] propInfo = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var property in propInfo)
            {
                property.SetValue(target.GetComponent(type), property.GetValue(component, null), null);
            }
        }

        static public Transform GetChildRootTransform(Transform t, Transform rootT)
        {
            var mcsDynamic = t.GetComponentInParent<MCSDynamicObject>();

            if (mcsDynamic)
            {
                return mcsDynamic.transform;
            }

            return rootT;
        }
    }
}