using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerFloatingOriginHandler : MonoBehaviour
    {
        public List<GPUInstancerManager> gPUIManagers;

        private Vector3 _previousPosition;

        void OnEnable()
        {
            _previousPosition = transform.position;
            transform.hasChanged = false;
        }

        void Update()
        {
            if(transform.hasChanged && transform.position != _previousPosition)
            {
                foreach (GPUInstancerManager manager in gPUIManagers)
                {
                    if (manager != null)
                        GPUInstancerAPI.SetGlobalPositionOffset(manager, transform.position - _previousPosition);
                }
                _previousPosition = transform.position;
                transform.hasChanged = false;
            }
        }
    }
}