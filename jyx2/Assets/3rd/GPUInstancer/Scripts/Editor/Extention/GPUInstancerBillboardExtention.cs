using UnityEngine;

namespace GPUInstancer.Extension
{
    public abstract class GPUInstancerBillboardExtension
    {
        public abstract string GetTitle();

        public abstract string GetButtonText();

        public abstract GameObject GenerateBillboard(GameObject gameObject);

        public abstract Mesh GetBillboardMesh(GameObject gameObject);

        public abstract Material GetBillboardMaterial(GameObject gameObject);

        public abstract bool IsInLODGroup(GameObject gameObject);

        public abstract bool IsBillboardAdded(GameObject gameObject);
    }
}
