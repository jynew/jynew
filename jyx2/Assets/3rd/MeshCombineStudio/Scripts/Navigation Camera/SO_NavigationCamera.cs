using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    // [CreateAssetMenu(fileName = "NavigationCamera_Data", menuName = "ScriptableObjects/MeshCombineStudio/NavigationCamera_Data", order = 1)]
    public class SO_NavigationCamera : ScriptableObject
    {
        public float mouseSensitity = 1;

        public float speedUpLerpMulti = 1;
        public float speedDownLerpMulti = 15;
        public float speedSlow = 1;
        public float speedNormal = 10;
        public float speedFast = 25;
        public float mouseScrollWheelMulti = 25;
    }
}
