using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [DefaultExecutionOrder(-99999999)]
    [ExecuteInEditMode]
    public abstract class MCS_RemoveTris : MonoBehaviour
    {
        HashSet<GameObjectLayer> gos = new HashSet<GameObjectLayer>();

        bool hasRegistered; 

        void Awake()
        {
            Register(true);
        }

        void OnEnable()
        {
            Register(false); 
        }

        void Register(bool first)
        {
            if (hasRegistered) return;
            
            if (first)
            {
                if (MeshCombiner.instances.Count == 0) return;

                for (int i = 0; i < MeshCombiner.instances.Count; i++)
                {
                    Init(MeshCombiner.instances[i]);
                }
            }
            else MeshCombiner.onInit += Init;

            hasRegistered = true;

        }

        void Init(MeshCombiner meshCombiner)
        {
            meshCombiner.onCombiningStart += OnCombine;
            meshCombiner.onCombiningAbort += OnCombineReady;
            meshCombiner.onCombiningReady += OnCombineReady;
        }

        void OnDisable()
        {
            Unregister();
        }

        void OnDestroy()
        {
            Unregister();
        }

        void Unregister()
        {
            if (!hasRegistered) return;
            hasRegistered = false;
            OnCombineReady(null);

            MeshCombiner.onInit -= Init;

            for (int i = 0; i < MeshCombiner.instances.Count; i++)
            {
                MeshCombiner meshCombiner = MeshCombiner.instances[i];
                meshCombiner.onCombiningStart -= OnCombine;
                meshCombiner.onCombiningAbort -= OnCombineReady;
                meshCombiner.onCombiningReady -= OnCombineReady;
            }
        }

        void OnCombine(MeshCombiner meshCombiner)
        {
            if (gos.Count > 0)
            {
                OnCombineReady(null);
            }

            int layer;

            if (this is MCS_RemoveTrisBelowSurface) layer = Methods.GetFirstLayerInLayerMask(meshCombiner.surfaceLayerMask);
            else layer = Methods.GetFirstLayerInLayerMask(meshCombiner.overlapLayerMask);

            if (layer == -1) return;

            Transform[] ts = GetComponentsInChildren<Transform>();

            for (int i = 0; i < ts.Length; i++)
            {
                GameObject go = ts[i].gameObject;
                gos.Add(new GameObjectLayer(go));
                go.layer = layer;
            }
        }
        void OnCombineReady(MeshCombiner meshCombiner)
        {
            foreach(var goLayer in gos)
            {
                goLayer.RestoreLayer();
            }

            gos.Clear();
        }
    }

    public struct GameObjectLayer
    {
        public GameObject go;
        public int layer;

        public GameObjectLayer(GameObject go)
        {
            this.go = go;
            layer = go.layer;
        }

        public void RestoreLayer()
        {
            go.layer = layer;
        }
    }
}
