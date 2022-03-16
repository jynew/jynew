using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal partial class GrassPainter
    {
        /*
         * Grass instances management
         */

        #region grass instance list attaching

        private static bool CanAttachGrassLoader = false;
        private static string CannotAttachGrassReason = null;

        public static void CheckIfCanAttachGrassLoader()
        {
            CanAttachGrassLoader = false;
            CannotAttachGrassReason = null;

            var gameObject = Selection.activeGameObject;
            if (gameObject == null)
            {
                CannotAttachGrassReason = StringTable.Get(C.Warning_SelectAGameObject);
                CanAttachGrassLoader = false;
                return;
            }
            var possibleMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (possibleMeshFilter != null)
            {
                CannotAttachGrassReason = StringTable.Get(C.Warning_CannotAttachGrassLoaderToAGameObjectWithMeshFilter);
                CanAttachGrassLoader = false;
                return;
            }

            var grassLoader = gameObject.GetComponent<GrassLoader>();
            if (grassLoader == null)
            {
                CanAttachGrassLoader = true;
                return;
            }
            if (grassLoader.grassInstanceList == null)
            {
                CanAttachGrassLoader = true;
                return;
            }

            CannotAttachGrassReason = StringTable.Get(C.Warning_AlreadyAttached);
            CanAttachGrassLoader = false;
        }
        
        public static void AttachGrassLoader()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (!confirmed)
            {
                return;
            }

            AttachGrassLoaderToGameObject(Selection.activeGameObject);
        }

        private static void AttachGrassLoaderToGameObject(GameObject obj)
        {
            var gameObject = obj;
            if (gameObject == null)
            {
                return;
            }

            var relativePath = EditorUtility.SaveFilePanelInProject(
                StringTable.Get(C.ChooseGrassPointCloudDataFilePath),
                "Grasses", "asset", "");
            if (string.IsNullOrEmpty(relativePath))
            {
                return;
            }

            var grassLoader = gameObject.GetComponent<GrassLoader>();
            if (grassLoader == null)
            {
                grassLoader = gameObject.AddComponent<GrassLoader>();
            }
            if (grassLoader.grassInstanceList == null)
            {
                grassLoader.grassInstanceList = ScriptableObject.CreateInstance<GrassList>();
                grassLoader.grassInstanceList.grasses = new List<GrassStar>();
                grassLoader.grassInstanceList.quads = new List<GrassQuad>();
            }

            AssetDatabase.CreateAsset(grassLoader.grassInstanceList, relativePath);
        }
        
        public static void CreateGrassContainer()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (!confirmed)
            {
                return;
            }

            if (MTEContext.TheGrassLoader)
            {
                string loaderObjName = MTEContext.TheGrassLoader.gameObject.name;
                EditorUtility.DisplayDialog(
                    StringTable.Get(C.Warning),
                    string.Format(StringTable.Get(C.Warning_GrassLoaderExists), loaderObjName),
                    StringTable.Get(C.Yes), StringTable.Get(C.No));
                return;
            }

            var gameObject = new GameObject("GrassContainer");
            AttachGrassLoaderToGameObject(gameObject);
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
        }
        #endregion

        List<Vector2> grassPositions = new List<Vector2>();

        readonly List<GrassItem> removeList = new List<GrassItem>(256);
        private DetailListBox<GrassDetail> detailListBox;
    }
}
