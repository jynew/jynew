using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class DisabledMeshRenderer : MonoBehaviour
    {
        [HideInInspector] public MeshCombiner meshCombiner;
        public CachedGameObject cachedGO;
    }
}
