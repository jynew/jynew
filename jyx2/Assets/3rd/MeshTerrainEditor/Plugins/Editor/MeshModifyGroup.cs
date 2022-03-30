using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    public struct MeshModifyGroup
    {
        public bool valid;

        public GameObject gameObject;
        public Mesh mesh;
        public MeshCollider meshCollider;
        public List<int> vIndex;
        public List<float> vDistance;

        public MeshModifyGroup(GameObject gameObject, Mesh mesh, MeshCollider meshCollider, List<int> vIndex, List<float> vDistance)
        {
            this.gameObject = gameObject;
            this.mesh = mesh;
            this.meshCollider = meshCollider;
            this.vIndex = vIndex;
            this.vDistance = vDistance;
            this.valid = true;
        }

    }
}