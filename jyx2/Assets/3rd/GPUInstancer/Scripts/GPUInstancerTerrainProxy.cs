using UnityEngine;

namespace GPUInstancer
{
    [ExecuteInEditMode]
    public class GPUInstancerTerrainProxy : MonoBehaviour
    {
        public GPUInstancerDetailManager detailManager;
        public GPUInstancerTreeManager treeManager;
        public bool beingDestroyed;
        public int terrainSelectedToolIndex = -1;

        private void OnDestroy()
        {
            beingDestroyed = true;
        }
    }
}
