using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jyx2
{
    public class RotateBlockParticles : MonoBehaviour
    {
        public void AdjustRotation(Transform from, Transform to)
        {
            var direction = (to.position - from.position).normalized;
            direction.y = 0;
            transform.forward = direction;
        }
    }
}