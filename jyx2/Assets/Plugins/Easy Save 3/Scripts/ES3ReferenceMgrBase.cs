using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ES3Internal
{
    [System.Serializable]
    [DisallowMultipleComponent]
    public abstract class ES3ReferenceMgrBase : MonoBehaviour
    {
        internal object _lock = new object();

        public const string referencePropertyName = "_ES3Ref";
        private static ES3ReferenceMgrBase _current = null;
        private static HashSet<ES3ReferenceMgrBase> mgrs = new HashSet<ES3ReferenceMgrBase>();
#if UNITY_EDITOR
        private const int CollectDependenciesDepth = 5;
        protected static bool isEnteringPlayMode = false;
        static readonly HideFlags[] invalidHideFlags = new HideFlags[] { HideFlags.DontSave, HideFlags.DontSaveInBuild, HideFlags.DontSaveInEditor, HideFlags.HideAndDontSave };
#endif

        private static System.Random rng;

        [HideInInspector]
        public bool openPrefabs = false; // Whether the prefab list should be open in the Editor.

        public List<ES3Prefab> prefabs = new List<ES3Prefab>();

        public static ES3ReferenceMgrBase Current
        {
            get
            {
                // If the reference manager hasn't been assigned, or we've got a reference to a manager in a different scene which isn't marked as DontDestroyOnLoad, look for this scene's manager.
                if (_current == null /*|| (_current.gameObject.scene.buildIndex != -1 && _current.gameObject.scene != SceneManager.GetActiveScene())*/)
                {
                    var scene = SceneManager.GetActiveScene();
                    var roots = scene.GetRootGameObjects();
                    ES3ReferenceMgr mgr = null;

                    // First, look for Easy Save 3 Manager in the top-level.
                    foreach (var root in roots)
                        if (root.name == "Easy Save 3 Manager")
                            mgr = root.GetComponent<ES3ReferenceMgr>();

                    // If the user has moved or renamed the Easy Save 3 Manager, we need to perform a deep search.
                    if (mgr == null)
                        foreach (var root in roots)
                            if ((_current = root.GetComponentInChildren<ES3ReferenceMgr>()) != null)
                                return _current;

                    mgrs.Add(_current = mgr);
                }
                return _current;
            }
        }

        public bool IsInitialised { get { return idRef.Count > 0; } }

        [SerializeField]
        public ES3IdRefDictionary idRef = new ES3IdRefDictionary();
        private ES3RefIdDictionary _refId = null;

        public ES3RefIdDictionary refId
        {
            get
            {
                if (_refId == null)
                {
                    _refId = new ES3RefIdDictionary();
                    // Populate the reverse dictionary with the items from the normal dictionary.
                    foreach (var kvp in idRef)
                        if (kvp.Value != null)
                            _refId[kvp.Value] = kvp.Key;
                }
                return _refId;
            }
            set
            {
                _refId = value;
            }
        }

        public ES3GlobalReferences GlobalReferences
        {
            get
            {
                return ES3GlobalReferences.Instance;
            }
        }

        private void Awake()
        {
            if (_current != null && _current != this)
            {
                var existing = _current;

                /* We intentionally use Current rather than _current here, as _current may contain a reference to a manager in another scene, 
                 * but Current only returns the Manager for the active scene. */
                if (Current != null)
                {
                    existing.Merge(this);
                    if (gameObject.name.Contains("Easy Save 3 Manager"))
                        Destroy(this.gameObject);
                    else
                        Destroy(this);
                    _current = existing; // Undo the call to Current, which may have set it to NULL.
                }
            }
            else
                _current = this;
            mgrs.Add(this);
        }

        private void OnDestroy()
        {
            mgrs.Remove(this);
        }

        // Merges two managers, not allowing any clashes of IDs
        public void Merge(ES3ReferenceMgrBase otherMgr)
        {
            foreach (var kvp in otherMgr.idRef)
                Add(kvp.Value, kvp.Key);
        }

        public long Get(UnityEngine.Object obj)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                if (obj == null)
                    return -1;
                long id;
                if (!mgr.refId.TryGetValue(obj, out id))
                    return -1;
                return id;
            }
            return -1;
        }

        internal UnityEngine.Object Get(long id, Type type, bool suppressWarnings=false)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                if (id == -1)
                    return null;
                UnityEngine.Object obj;
                if (!mgr.idRef.TryGetValue(id, out obj))
                {
                    if (GlobalReferences != null)
                    {
                        var globalRef = GlobalReferences.Get(id);
                        if (globalRef != null)
                            return globalRef;
                    }

                    if(type != null)
                        ES3Debug.LogWarning("Reference for " + type + " with ID " + id + " could not be found in Easy Save's reference manager. If you are loading objects dynamically (i.e. objects created at runtime), this warning is expected and can be ignored.", this);
                    else
                        ES3Debug.LogWarning("Reference with ID " + id + " could not be found in Easy Save's reference manager. If you are loading objects dynamically (i.e. objects created at runtime), this warning is expected and can be ignored.", this);
                    return null;
                }
                if (obj == null) // If obj has been marked as destroyed but not yet destroyed, don't return it.
                    return null;
                return obj;
            }
            return null;
        }

        public UnityEngine.Object Get(long id, bool suppressWarnings = false)
        {
            return Get(id, null, suppressWarnings);
        }

        public ES3Prefab GetPrefab(long id, bool suppressWarnings = false)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                foreach (var prefab in mgr.prefabs)
                    if (prefabs != null && prefab.prefabId == id)
                        return prefab;
            }
            if (!suppressWarnings)
                ES3Debug.LogWarning("Prefab with ID " + id + " could not be found in Easy Save's reference manager. Try pressing the Refresh References button on the ES3ReferenceMgr Component of the Easy Save 3 Manager in your scene.", this);
            return null;
        }

        public long GetPrefab(ES3Prefab prefabToFind, bool suppressWarnings = false)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                foreach (var prefab in prefabs)
                    if (prefab == prefabToFind)
                        return prefab.prefabId;
            }
            if (!suppressWarnings)
                ES3Debug.LogWarning("Prefab with name " + prefabToFind.name + " could not be found in Easy Save's reference manager. Try pressing the Refresh References button on the ES3ReferenceMgr Component of the Easy Save 3 Manager in your scene.", prefabToFind);
            return -1;
        }

        public long Add(UnityEngine.Object obj)
        {
            long id;
            // If it already exists in the list, do nothing.
            if (refId.TryGetValue(obj, out id))
                return id;

            if (GlobalReferences != null)
            {
                id = GlobalReferences.GetOrAdd(obj);
                if (id != -1)
                {
                    Add(obj, id);
                    return id;
                }
            }

            lock (_lock)
            {
                // Add the reference to the Dictionary.
                id = GetNewRefID();
                return Add(obj, id);
            }
        }

        public long Add(UnityEngine.Object obj, long id)
        {
            if (!CanBeSaved(obj))
                return -1;

            // If the ID is -1, auto-generate an ID.
            if (id == -1)
                id = GetNewRefID();
            // Add the reference to the Dictionary.
            lock (_lock)
            {
                idRef[id] = obj;
                if(obj != null)
                    refId[obj] = id;
            }
            return id;
        }

        public bool AddPrefab(ES3Prefab prefab)
        {
            if (!prefabs.Contains(prefab))
            {
                prefabs.Add(prefab);
                return true;
            }
            return false;
        }

        public void Remove(UnityEngine.Object obj)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                lock (mgr._lock)
                {
                    mgr.refId.Remove(obj);
                    // There may be multiple references with the same ID, so remove them all.
                    foreach (var item in mgr.idRef.Where(kvp => kvp.Value == obj).ToList())
                        mgr.idRef.Remove(item.Key);
                }
            }
        }

        public void Remove(long referenceID)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                lock (mgr._lock)
                {
                    mgr.idRef.Remove(referenceID);
                    // There may be multiple references with the same ID, so remove them all.
                    foreach (var item in mgr.refId.Where(kvp => kvp.Value == referenceID).ToList())
                        mgr.refId.Remove(item.Key);
                }
            }
        }

        public void RemoveNullOrInvalidValues()
        {
            var nullKeys = idRef.Where(pair => pair.Value == null || !CanBeSaved(pair.Value))
                                .Select(pair => pair.Key).ToList();
            foreach (var key in nullKeys)
                idRef.Remove(key);

            if (GlobalReferences != null)
                GlobalReferences.RemoveInvalidKeys();
        }

        public void Clear()
        {
            lock (_lock)
            {
                refId.Clear();
                idRef.Clear();
            }
        }

        public bool Contains(UnityEngine.Object obj)
        {
            return refId.ContainsKey(obj);
        }

        public bool Contains(long referenceID)
        {
            return idRef.ContainsKey(referenceID);
        }

        public void ChangeId(long oldId, long newId)
        {
            foreach (var mgr in mgrs)
            {
                if (mgr == null)
                    continue;

                mgr.idRef.ChangeKey(oldId, newId);
                // Empty the refId so it has to be refreshed.
                mgr.refId = null;
            }
        }

        internal static long GetNewRefID()
        {
            if (rng == null)
                rng = new System.Random();

            byte[] buf = new byte[8];
            rng.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (System.Math.Abs(longRand % (long.MaxValue - 0)) + 0);
        }

#if UNITY_EDITOR
        public static HashSet<UnityEngine.Object> CollectDependencies(UnityEngine.Object obj, HashSet<UnityEngine.Object> dependencies = null, int depth = CollectDependenciesDepth)
        {
            return CollectDependencies(new UnityEngine.Object[] { obj }, dependencies, depth);
        }

        /*
         * Collects all top-level dependencies of an object.
         * For GameObjects, it will traverse all children.
         * For Components or ScriptableObjects, it will get all serialisable UnityEngine.Object fields/properties as dependencies.
         */
        public static HashSet<UnityEngine.Object> CollectDependencies(UnityEngine.Object[] objs, HashSet<UnityEngine.Object> dependencies = null, int depth = CollectDependenciesDepth)
        {
            if (depth < 0)
                return dependencies;

            if (dependencies == null)
                dependencies = new HashSet<UnityEngine.Object>();

            foreach (var obj in objs)
            {
                if (obj == null)
                    continue;

                var type = obj.GetType();

                // Skip types which don't need processing
                if (type == typeof(ES3ReferenceMgr) || type == typeof(ES3AutoSaveMgr) || type == typeof(ES3AutoSave) || type == typeof(ES3InspectorInfo))
                    continue;

                // Add the prefab to the manager but don't process it. We'll use this to work out what prefabs to add to the prefabs list later.
                if (type == typeof(ES3Prefab))
                {
                    dependencies.Add(obj);
                    continue;
                }

                // If it's a GameObject, get the GameObject's Components and collect their dependencies.
                if (type == typeof(GameObject))
                {
                    var go = (GameObject)obj;
                    // If we've not already processed this GameObject ...
                    if (dependencies.Add(go))
                    {
                        // Get the dependencies of each Component in the GameObject.
                        CollectDependencies(go.GetComponents<Component>(), dependencies, depth - 1);
                        // Get the dependencies of each child in the GameObject.
                        foreach (Transform child in go.transform)
                            CollectDependencies(child.gameObject, dependencies, depth); // Don't decrement child, as we consider this a top-level object.
                    }
                }
                // Else if it's a Component or ScriptableObject, add the values of any UnityEngine.Object fields as dependencies.
                else
                    CollectDependenciesFromFields(obj, dependencies, depth - 1);
            }

            return dependencies;
        }

        private static void CollectDependenciesFromFields(UnityEngine.Object obj, HashSet<UnityEngine.Object> dependencies, int depth)
        {
            // If we've already collected dependencies for this, do nothing.
            if (!dependencies.Add(obj))
                return;

            if (depth < 0)
                return;

            var type = obj.GetType();

            if (isEnteringPlayMode && type == typeof(UnityEngine.UI.Text))
                return;

            try
            {
                // SerializedObject is expensive, so for known classes we manually gather references.

                if (type == typeof(Animator) || obj is Transform || type == typeof(CanvasRenderer) || type == typeof(Mesh) || type == typeof(AudioClip) || type == typeof(Rigidbody) || obj is Texture || obj is HorizontalOrVerticalLayoutGroup)
                    return;

                if (obj is Graphic)
                {
                    var m = (Graphic)obj;
                    dependencies.Add(m.material);
                    dependencies.Add(m.defaultMaterial);
                    dependencies.Add(m.mainTexture);

                    if (type == typeof(Text))
                    {
                        var text = (Text)obj;
                        dependencies.Add(text.font);
                    }
                    else if (type == typeof(Image))
                    {
                        var img = (Image)obj;
                        dependencies.Add(img.sprite);
                    }
                    return;
                }

                if (type == typeof(Mesh))
                {
                    if (UnityEditor.AssetDatabase.Contains(obj))
                        dependencies.Add(obj);
                    return;
                }

                if (type == typeof(Material))
                {
                    dependencies.Add(((Material)obj).shader);
                    return;
                }

                if (type == typeof(MeshFilter))
                {
                    dependencies.Add(((MeshFilter)obj).sharedMesh);
                    return;
                }

                if (type == typeof(MeshCollider))
                {
                    var mc = (MeshCollider)obj;
                    dependencies.Add(mc.sharedMesh);
                    dependencies.Add(mc.sharedMaterial);
                    dependencies.Add(mc.attachedRigidbody);
                    return;
                }

                if (type == typeof(Camera))
                {
                    var c = (Camera)obj;
                    dependencies.Add(c.targetTexture);
                    return;
                }

                if (type == typeof(SkinnedMeshRenderer))
                    dependencies.Add(((SkinnedMeshRenderer)obj).sharedMesh); // Don't return. Let this fall through to the if(obj is renderer) call.
                else if (type == typeof(SpriteRenderer))
                    dependencies.Add(((SpriteRenderer)obj).sprite); // Don't return. Let this fall through to the if(obj is renderer) call.
                else if (type == typeof(ParticleSystemRenderer))
                    dependencies.Add(((ParticleSystemRenderer)obj).mesh); // Don't return. Let this fall through to the if(obj is renderer) call.

                if (obj is Renderer)
                {
                    dependencies.UnionWith(((Renderer)obj).sharedMaterials);
                    return;
                }
            }
            catch { }

            var so = new UnityEditor.SerializedObject(obj);
            if (so == null)
                return;

            var property = so.GetIterator();
            if (property == null)
                return;

            // Iterate through each of this object's properties.
            while (property.NextVisible(true))
            {
                try
                {
                    // If it's an array which contains UnityEngine.Objects, add them as dependencies.
                    if (property.isArray && property.propertyType != UnityEditor.SerializedPropertyType.String)
                    {
                        for (int i = 0; i < property.arraySize; i++)
                        {
                            var element = property.GetArrayElementAtIndex(i);

                            // If the array contains UnityEngine.Object types, add them to the dependencies.
                            if (element.propertyType == UnityEditor.SerializedPropertyType.ObjectReference)
                            {
                                var elementValue = element.objectReferenceValue;
                                var elementType = elementValue.GetType();

                                // If it's a GameObject, use CollectDependencies so that Components are also added.
                                if (elementType == typeof(GameObject))
                                    CollectDependencies(elementValue, dependencies, depth - 1);
                                else
                                    CollectDependenciesFromFields(elementValue, dependencies, depth - 1);
                            }
                            // Otherwise this array does not contain UnityEngine.Object types, so we should stop.
                            else
                                break;
                        }
                    }
                    // Else if it's a normal UnityEngine.Object field, add it.
                    else if (property.propertyType == UnityEditor.SerializedPropertyType.ObjectReference)
                    {
                        var propertyValue = property.objectReferenceValue;
                        if (propertyValue == null)
                            continue;

                        // If it's a GameObject, use CollectDependencies so that Components are also added.
                        if (propertyValue.GetType() == typeof(GameObject))
                            CollectDependencies(propertyValue, dependencies, depth - 1);
                        else
                            CollectDependenciesFromFields(propertyValue, dependencies, depth - 1);
                    }
                }
                catch { }
            }
        }

        // Called in the Editor when this Component is added.
        private void Reset()
        {
            // Ensure that Component can only be added by going to Assets > Easy Save 3 > Add Manager to Scene.
            if (gameObject.name != "Easy Save 3 Manager")
            {
                UnityEditor.EditorUtility.DisplayDialog("Cannot add ES3ReferenceMgr directly", "Please go to 'Assets > Easy Save 3 > Add Manager to Scene' to add an Easy Save 3 Manager to your scene.", "Ok");
                DestroyImmediate(this);
            }
        }
#endif

        internal static bool CanBeSaved(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (obj == null)
                return true;

            foreach (var flag in invalidHideFlags)
                if ((obj.hideFlags & flag) != 0 && obj.hideFlags != HideFlags.HideInHierarchy && obj.hideFlags != HideFlags.HideInInspector && obj.hideFlags != HideFlags.NotEditable)
                    if (!(obj is Mesh || obj is Material))
                        return false;

            // Exclude the Easy Save 3 Manager, and all components attached to it.
            if (obj.name == "Easy Save 3 Manager")
                return false;
#endif
            return true;
        }
    }

    [System.Serializable]
    public class ES3IdRefDictionary : ES3SerializableDictionary<long, UnityEngine.Object>
    {
        protected override bool KeysAreEqual(long a, long b)
        {
            return a == b;
        }

        protected override bool ValuesAreEqual(UnityEngine.Object a, UnityEngine.Object b)
        {
            return a == b;
        }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Serializable]
    public class ES3RefIdDictionary : ES3SerializableDictionary<UnityEngine.Object, long>
    {
        protected override bool KeysAreEqual(UnityEngine.Object a, UnityEngine.Object b)
        {
            return a == b;
        }

        protected override bool ValuesAreEqual(long a, long b)
        {
            return a == b;
        }
    }
}