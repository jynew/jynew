using System.Collections.Generic;
using UnityEngine;
using ES3Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace ES3Internal
{
    public class ES3Prefab : MonoBehaviour
    {
        public long prefabId = GetNewRefID();
        /*
         * We need to store references to all dependencies of the prefab because only supported scripts will be serialised.
         * This means that although supported scripts have their dependencies added to the reference manager when we load the prefab, 
         * there will not be any dependencies in the reference manager for scripts which are not supported.  So it will not be possible to save any reference to these.
         */
        public ES3RefIdDictionary localRefs = new ES3RefIdDictionary();

        public void Awake()
        {
            // Add the references to the reference list when this prefab is instantiated.
            var mgr = ES3ReferenceMgrBase.Current;

            if (mgr == null)
                return;

            foreach (var kvp in localRefs)
                if (kvp.Key != null)
                    mgr.Add(kvp.Key);
        }

        public long Get(UnityEngine.Object obj)
        {
            long id;
            if (localRefs.TryGetValue(obj, out id))
                return id;
            return -1;
        }

        public long Add(UnityEngine.Object obj)
        {
            long id;
            if (localRefs.TryGetValue(obj, out id))
                return id;

            if (!ES3ReferenceMgr.CanBeSaved(obj))
                return -1;
            id = GetNewRefID();
            localRefs.Add(obj, id);
            return id;
        }

        public Dictionary<string, string> GetReferences()
        {
            var localToGlobal = new Dictionary<string, string>();

            var refMgr = ES3ReferenceMgr.Current;

            if (refMgr == null)
                return localToGlobal;

            foreach (var kvp in localRefs)
            {
                long id = refMgr.Add(kvp.Key);
                localToGlobal.Add(kvp.Value.ToString(), id.ToString());
            }
            return localToGlobal;
        }

        public void ApplyReferences(Dictionary<long, long> localToGlobal)
        {
            if (ES3ReferenceMgrBase.Current == null)
                return;

            foreach (var localRef in localRefs)
            {
                long globalId;
                if (localToGlobal.TryGetValue(localRef.Value, out globalId))
                    ES3ReferenceMgrBase.Current.Add(localRef.Key, globalId);
            }
        }

        public static long GetNewRefID()
        {
            return ES3ReferenceMgrBase.GetNewRefID();
        }
#if UNITY_EDITOR
        public void GeneratePrefabReferences()
        {
#if UNITY_2018_3_OR_NEWER
            if (this.gameObject.scene.name != null || UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
#else
            if (this.gameObject.scene.name != null)
#endif
                return;

            // Create a new reference list so that any objects which are no longer dependencies are removed.
            var tempLocalRefs = new ES3RefIdDictionary();

            // Get dependencies of children also.
            var transforms = GetComponentsInChildren<Transform>();
            var gos = new GameObject[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
                gos[i] = transforms[i].gameObject;

            // Add the GameObject's dependencies to the reference list.
            foreach (var obj in ES3ReferenceMgr.CollectDependencies(gos))
            {
                var dependency = (UnityEngine.Object)obj;
                if (obj == null || !ES3ReferenceMgr.CanBeSaved(dependency))
                    continue;

                var id = Get(dependency);
                // If we're adding a new reference, do an Undo.RecordObject to ensure it persists.
                if (id == -1)
                {
                    Undo.RecordObject(this, "Update Easy Save 3 Prefab");
                    EditorUtility.SetDirty(this);
                }
                tempLocalRefs.Add(dependency, id == -1 ? GetNewRefID() : id);
            }

            localRefs = tempLocalRefs;
        }
#endif
    }
}

/*
 * 	Create a blank ES3Type for ES3Prefab as it does not require serialising/deserialising when stored as a Component.
 */
namespace ES3Types
{
    [UnityEngine.Scripting.Preserve]
    public class ES3Type_ES3Prefab : ES3Type
    {
        public static ES3Type Instance = null;

        public ES3Type_ES3Prefab() : base(typeof(ES3Prefab)) { Instance = this; }

        public override void Write(object obj, ES3Writer writer)
        {
        }

        public override object Read<T>(ES3Reader reader)
        {
            return null;
        }
    }

    /*
     * 	Use this ES3Type to serialise the .
     */
    public class ES3Type_ES3PrefabInternal : ES3Type
    {
        public static ES3Type Instance = new ES3Type_ES3PrefabInternal();

        public ES3Type_ES3PrefabInternal() : base(typeof(ES3Type_ES3PrefabInternal)) { Instance = this; }

        public override void Write(object obj, ES3Writer writer)
        {
            ES3Prefab es3Prefab = (ES3Prefab)obj;

            writer.WriteProperty("prefabId", es3Prefab.prefabId.ToString(), ES3Type_string.Instance);
            writer.WriteProperty("refs", es3Prefab.GetReferences());
        }

        public override object Read<T>(ES3Reader reader)
        {
            var prefabId = reader.ReadRefProperty();

            if (ES3ReferenceMgrBase.Current == null)
                return null;

            var es3Prefab = ES3ReferenceMgrBase.Current.GetPrefab(prefabId);
            if (es3Prefab == null)
                throw new MissingReferenceException("Prefab with ID " + prefabId + " could not be found.\nPress the 'Refresh References' button on the ES3ReferenceMgr Component of the Easy Save 3 Manager in the scene to refresh prefabs.");


#if UNITY_EDITOR
            // Use PrefabUtility.InstantiatePrefab if we're saving in the Editor and the application isn't playing.
            // This keeps the connection to the prefab, which is useful for Editor scripts saving data outside of runtime.
            var instance = Application.isPlaying ? GameObject.Instantiate(es3Prefab.gameObject) : PrefabUtility.InstantiatePrefab(es3Prefab.gameObject);
#else
            var instance = GameObject.Instantiate(es3Prefab.gameObject);
#endif
            var instanceES3Prefab = ((GameObject)instance).GetComponent<ES3Prefab>();
            if (instanceES3Prefab == null)
                throw new MissingReferenceException("Prefab with ID " + prefabId + " was found, but it does not have an ES3Prefab component attached.");

            ReadInto<T>(reader, instance);

            return instanceES3Prefab.gameObject;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            // Load as ES3Refs and convert to longs.
            var localToGlobal_refs = reader.ReadProperty<Dictionary<ES3Ref, ES3Ref>>(ES3Type_ES3RefDictionary.Instance);
            var localToGlobal = new Dictionary<long, long>();
            foreach (var kvp in localToGlobal_refs)
                localToGlobal.Add(kvp.Key.id, kvp.Value.id);

            if (ES3ReferenceMgrBase.Current == null)
                return;

            ((GameObject)obj).GetComponent<ES3Prefab>().ApplyReferences(localToGlobal);
        }
    }
}