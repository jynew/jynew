using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [ExecuteInEditMode]
    public class MeshCombinerData : MonoBehaviour
    {
        public Dictionary<Collider, CachedGameObject> colliderLookup = new Dictionary<Collider, CachedGameObject>();
        public Dictionary<LODGroup, CachedGameObject> lodGroupLookup = new Dictionary<LODGroup, CachedGameObject>();

        [HideInInspector] public List<GameObject> combinedGameObjects = new List<GameObject>();
        [HideInInspector] public List<CachedGameObject> foundObjects = new List<CachedGameObject>();
        [HideInInspector] public List<CachedLodGameObject> foundLodObjects = new List<CachedLodGameObject>();
        [HideInInspector] public List<LODGroup> foundLodGroups = new List<LODGroup>();
        [HideInInspector] public List<Collider> foundColliders = new List<Collider>();

        void OnValidate()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        public void ClearAll()
        {
            combinedGameObjects.Clear();
            foundObjects.Clear();
            foundLodObjects.Clear();
            foundLodGroups.Clear();
            foundColliders.Clear();

            colliderLookup.Clear();
            lodGroupLookup.Clear();
        }
    }

    
    //[Serializable]
    //public struct FoundLODGroup
    //{
    //    public LODGroup lodGroup;
    //    public CachedGameObject cachedGO;

    //    public FoundLODGroup(LODGroup lodGroup, CachedGameObject cachedGO)
    //    {
    //        this.lodGroup = lodGroup;
    //        this.cachedGO = cachedGO;
    //    }
    //}

    //[Serializable]
    //public struct FoundCollider
    //{
    //    public Collider collider;
    //    public CachedGameObject cachedGO;

    //    public FoundCollider(Collider collider, CachedGameObject cachedGO)
    //    {
    //        this.collider = collider;
    //        this.cachedGO = cachedGO;
    //    }
    //}
}
