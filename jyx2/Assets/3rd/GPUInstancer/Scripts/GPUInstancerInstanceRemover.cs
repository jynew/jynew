using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerInstanceRemover : MonoBehaviour
    {
        public bool useBounds = false;
        public List<Collider> selectedColliders;
        public bool removeFromDetailManagers = true;
        public bool removeFromTreeManagers = true;
        public bool removeFromPrefabManagers = true;
        public bool removeAtUpdate = false;
        public float offset = 0;
        public bool onlyRemoveSelectedPrototypes = false;
        public List<GPUInstancerPrototype> selectedPrototypes;

        private void Reset()
        {
            selectedColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        }

        private void Start()
        {
            foreach (GPUInstancerManager manager in GPUInstancerManager.activeManagerList)
            {
                if ((!removeFromDetailManagers && manager is GPUInstancerDetailManager)
                    || (!removeFromTreeManagers && manager is GPUInstancerTreeManager)
                    || (!removeFromPrefabManagers && manager is GPUInstancerPrefabManager))
                    continue;
                //GPUInstancerAPI.InitializeGPUInstancer(manager, false);
                foreach (Collider collider in selectedColliders)
                {
                    if (useBounds)
                        GPUInstancerAPI.RemoveInstancesInsideBounds(manager, collider.bounds, offset, onlyRemoveSelectedPrototypes ? selectedPrototypes : null);
                    else
                        GPUInstancerAPI.RemoveInstancesInsideCollider(manager, collider, offset, onlyRemoveSelectedPrototypes ? selectedPrototypes : null);
                }
            }
        }

        private void Update()
        {
            if (removeAtUpdate)
                Start();
        }
    }
}
