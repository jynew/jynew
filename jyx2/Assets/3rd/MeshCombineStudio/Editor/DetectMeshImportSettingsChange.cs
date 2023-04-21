using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshCombineStudio
{
#if MCS2_DETECT_MESH_IMPORT_SETTINGS_CHANGE
    public class DetectMeshImportSettingsChange : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            // ModelImporter importer = (ModelImporter)assetImporter;

            // List<MeshCombiner> instances = MeshCombiner.instances;

            // Debug.Log("MeshCombiner instances " + instances.Count);

            MeshCombineJobManager.ResetMeshCache(); 
        }
    }
#endif
}

