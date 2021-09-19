using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Add this to the prefabs of GameObjects you want to GPU Instance at runtime.
    /// </summary>
    public class GPUInstancerPrefab : MonoBehaviour
    {
        [HideInInspector]
        public GPUInstancerPrefabPrototype prefabPrototype;
        [NonSerialized]
        public int gpuInstancerID;
        [NonSerialized]
        public PrefabInstancingState state = PrefabInstancingState.None;
        public Dictionary<string, object> variationDataList;

        protected bool _isTransformSet;
        protected Transform _instanceTransform;

        protected bool _isMatrixSet;
        protected Matrix4x4 _localToWorldMatrix;

        public virtual void AddVariation<T>(string bufferName, T value)
        {
            if (variationDataList == null)
                variationDataList = new Dictionary<string, object>();
            if (variationDataList.ContainsKey(bufferName))
                variationDataList[bufferName] = value;
            else
                variationDataList.Add(bufferName, value);
        }

        public virtual Transform GetInstanceTransform(bool forceNew = false)
        {
            if (!_isTransformSet || forceNew)
            {
                _instanceTransform = transform;
                _instanceTransform.hasChanged = false;
                _isTransformSet = true;
            }
            return _instanceTransform;
        }

        public virtual Matrix4x4 GetLocalToWorldMatrix(bool forceNew = false)
        {
            if (!_isMatrixSet || forceNew)
            {
                _localToWorldMatrix = GetInstanceTransform(forceNew).localToWorldMatrix;
                _isMatrixSet = true;
            }
            return _localToWorldMatrix;
        }

        public virtual void SetupPrefabInstance(GPUInstancerRuntimeData runtimeData, bool forceNew = false)
        {

        }
    }

    public enum PrefabInstancingState
    {
        None,
        Disabled,
        Instanced
    }
}
