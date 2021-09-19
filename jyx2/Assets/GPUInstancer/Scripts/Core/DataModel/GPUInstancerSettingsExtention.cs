using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    public class GPUInstancerSettingsExtension : ScriptableObject
    {
        public float extensionVersionNo;

        public virtual string GetExtensionCode()
        {
            throw new NotImplementedException();
        }
    }
}
