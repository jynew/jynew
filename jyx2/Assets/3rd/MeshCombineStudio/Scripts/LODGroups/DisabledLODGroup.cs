using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class DisabledLODGroup : MonoBehaviour
    {
        [HideInInspector] public MeshCombiner meshCombiner;
        public LODGroup lodGroup;
    }
}
