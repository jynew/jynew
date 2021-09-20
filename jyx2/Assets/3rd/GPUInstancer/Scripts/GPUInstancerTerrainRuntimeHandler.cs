using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Helper script that can be added to the Terrains to automatically add remove Terrains to Tree Manager.
    /// There should be only one Tree Manager in the scene and every terrain this script is attached to should have the same Tree prototypes.
    /// If you are using multiple Tree Managers with different Terrain trees use the AddTerrainToManager and RemoveTerrainFromManager API methods to specify the Tree Managers instead of this script.
    /// </summary>
    public class GPUInstancerTerrainRuntimeHandler : MonoBehaviour
    {
        [HideInInspector]
        public Terrain terrain;

        private static GPUInstancerTreeManager _treeManager;

        private void Awake()
        {
            terrain = GetComponent<Terrain>();
            if (_treeManager == null)
            {
                _treeManager = FindObjectOfType<GPUInstancerTreeManager>();
            }
        }

        private void Reset()
        {
            if (GetComponent<Terrain>() == null)
            {
                Debug.LogError("GPUInstancerTerrainRuntimeHandler can only be added to a Terrain!");
                DestroyImmediate(this);
            }
        }

        private void OnEnable()
        {
            if (_treeManager != null)
            {
                _treeManager.AddTerrain(terrain);
            }
        }

        private void OnDisable()
        {
            if (_treeManager != null)
            {
                _treeManager.RemoveTerrain(terrain);
                terrain.treeDistance = _treeManager.terrainSettings.maxTreeDistance;
            }
        }
    }
}
