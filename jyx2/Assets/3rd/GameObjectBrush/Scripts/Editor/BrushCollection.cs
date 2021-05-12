using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameObjectBrush
{
    [CreateAssetMenu(fileName = "New BrushCollection", menuName = "Tools/Gameobject Brush/Create BrushCollection")]
    public class BrushCollection : ScriptableObject
    {

        [SerializeField] private List<BrushObject> _brushes = new List<BrushObject>();
        public List<BrushObject> brushes {
            get {
                if (_brushes == null) {
                    _brushes = new List<BrushObject>();
                }
                return _brushes;
            }
            set {
                if (value == null) {
                    value = new List<BrushObject>();
                }
                _brushes = value;
            }
        }

        [HideInInspector] public BrushObject primarySelectedBrush = null;                             //The currently selected/viewes brush (has to be public in order to be accessed by the FindProperty method)
        [HideInInspector] public List<BrushObject> selectedBrushes = new List<BrushObject>();         //all other selected brushes

        [HideInInspector, NonSerialized] public List<GameObject> spawnedObjects = new List<GameObject>();

        protected static string lastBrushCollection_EditPrefsKey = "GOB_LastUsedBrushCollection";
        public static string defaultBrushCollectionName = "Default Brush Collection";
        public static string newBrushCollectionName = "New Brush Collection";

        /// <summary>
        /// Applies the spawned/cached objects of this brush collection
        /// </summary>
        public void ApplyCachedObjects() {
            spawnedObjects.Clear();
        }
        /// <summary>
        /// Deletes all previously spawned objects
        /// </summary>
        public void DeleteSpawnedObjects() {
            foreach (GameObject obj in spawnedObjects) {
                DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
        }

        /// <summary>
        /// Removes all brushes from this brush collection
        /// </summary>
        public void RemoveAllBrushes() {
            brushes.Clear();
            primarySelectedBrush = null;
            selectedBrushes.Clear();
        }

        /// <summary>
        /// Saves the collection to disk
        /// </summary>
        public void Save() {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// Get the Asset GUID of this scriptable object
        /// </summary>
        /// <returns></returns>
        public string GetGUID() {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        }



        /// <summary>
        /// Iterates over the list of current brushes and adds the name of each brush to a string.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentBrushesString() {
            string str = "";
            foreach (BrushObject brush in selectedBrushes) {
                if (str != "") {
                    str += " ,";
                }
                str += brush.brushObject.name;
            }
            return str;
        }
        /// <summary>
        /// Get the greatest brush size value from the current brushes list
        /// </summary>
        /// <returns></returns>
        public float GetMaximumBrushSizeFromCurrentBrushes() {
            float maxBrushSize = 0f;
            foreach (BrushObject brush in selectedBrushes) {
                if (brush.brushSize > maxBrushSize) {
                    maxBrushSize = brush.brushSize;
                }
            }
            return maxBrushSize;
        }
        /// <summary>
        /// Gets the global index of this brush collection.
        /// The index refferres to the index of this brush collection in the array returned by "AssetDatabase.FindAssets("t:BrushCollection");"
        /// </summary>
        /// <returns></returns>
        public int GetIndex() {
            string[] guids = GetAllBrushCollectionGUIDs();
            for(int i = 0; i < guids.Length; i++) {
                if (guids[i] == GetGUID()) {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the last used brush collection to be this brush collection
        /// (this is done to save this collection as the currently open one)
        /// </summary>
        public void SetLastUsedBrushCollection() {
            EditorPrefs.SetString(lastBrushCollection_EditPrefsKey, GetGUID());
        }

        /// <summary>
        /// Checks if a .asses with the same name already ecists in the default brush collection dir
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckIfBrushCollectionAlreadyExists(string name) {
            string path = GetDefaultBrushCollectionDir() + name + ".asset";
            return AssetDatabase.AssetPathToGUID(path) != "";
        }
        /// <summary>
        /// Returnes the last used brush collection, if none it returns a new one
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<int, BrushCollection> GetLastUsedBrushCollection() {

            //try to find the last used brush collection and return it
            if (EditorPrefs.HasKey(lastBrushCollection_EditPrefsKey)) {
                string guid = EditorPrefs.GetString(lastBrushCollection_EditPrefsKey, "");
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BrushCollection lastUsedCollection = AssetDatabase.LoadAssetAtPath<BrushCollection>(path);

                //return found one or create one
                if (lastUsedCollection != null) {
                    return new KeyValuePair<int, BrushCollection>(lastUsedCollection.GetIndex(), lastUsedCollection);
                }
            }

            //create one if none found
            BrushCollection brushCollection = CreateInstance(defaultBrushCollectionName);
            return new KeyValuePair<int, BrushCollection>(brushCollection.GetIndex(), brushCollection);
        }
        /// <summary>
        /// Get the path to the directory where all the brushes are saved by default
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultBrushCollectionDir() {

            //Find parent dir of the GameObjectBrush extension
            string relPathToScript = "Scripts\\Editor\\" + GetClassName();
            string absPath = Directory.GetFiles(Application.dataPath, GetClassName(), SearchOption.AllDirectories)[0];
            string relPath = "Assets" + absPath.Substring(Application.dataPath.Length);
            relPath = relPath.Replace("Scripts" + Path.DirectorySeparatorChar + GetClassName(), "");
            relPath = relPath.Remove(relPath.Length - relPathToScript.Length);           //remove path that points to this script from the GameObjectBrush Folder

            //create DIR if not found
            string dirPath = relPath + "Brushes";
            if (!AssetDatabase.IsValidFolder(dirPath)) {
                string path = AssetDatabase.CreateFolder(dirPath, "Brushes");
                AssetDatabase.SaveAssets();
            }
            relPath += "Brushes" + Path.DirectorySeparatorChar;
            return relPath;
        }
        /// <summary>
        /// Gets all guids of all brushCollections present in this project
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllBrushCollectionGUIDs() {
            return AssetDatabase.FindAssets("t:BrushCollection");
        }
        /// <summary>
        /// Returns a struct of all BrushCollection assets found in this project
        /// </summary>
        /// <returns></returns>
        public static BrushCollectionList GetBrushCollectionsInProject() {
            string[] guids = GetAllBrushCollectionGUIDs();
            return new BrushCollectionList(guids);
        }


        /// <summary>
        /// Creates a new BrushList asset
        /// </summary>
        /// <returns></returns>
        public new static BrushCollection CreateInstance(string name) {
            string path = GetDefaultBrushCollectionDir() + name + ".asset";

            //create asset
            BrushCollection collection = CreateInstance<BrushCollection>();
            AssetDatabase.CreateAsset(collection, path);
            AssetDatabase.SaveAssets();

            //update last used one to be this collection (this is done to save this collection as the currently open one)
            collection.SetLastUsedBrushCollection();

            //switch to newly created brush collection
            GameObjectBrushEditor.SwitchToBrushCollection(collection);

            return collection;
        }
        /// <summary>
        /// Gets the class name as string
        /// </summary>
        /// <returns></returns>
        public static string GetClassName() {
            //stringify classname and remove namespace
            return (typeof(BrushCollection).ToString() + ".cs").Replace("GameObjectBrush.", "");
        }



        public struct BrushCollectionList {
            public List<BrushCollection> brushCollections;

            public BrushCollectionList(string[] guids) {
                brushCollections = new List<BrushCollection>();
                for (int i = 0; i < guids.Length; i++) {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    brushCollections.Add(AssetDatabase.LoadAssetAtPath<BrushCollection>(path));
                }
            }


            /// <summary>
            /// Gets the name of each brush collection as an array
            /// </summary>
            /// <returns></returns>
            public string[] GetNameList() {
                List<string> names = new List<string>();
                for(int i = 0; i < brushCollections.Count; i++) {
                    if (brushCollections[i] != null) {
                        names.Add(brushCollections[i].name);
                    } else {
                        brushCollections.Remove(brushCollections[i]);
                    }
                }
                return names.ToArray();
            }
        }
    }
}