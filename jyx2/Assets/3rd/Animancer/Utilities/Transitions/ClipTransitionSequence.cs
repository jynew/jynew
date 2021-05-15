// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>A group of <see cref="ClipState.Transition"/>s which play one after the other.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">Transitions</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/ClipTransitionSequence
    /// 
    [Serializable]
    public class ClipTransitionSequence : ClipState.Transition, ISerializationCallbackReceiver
    {
        /************************************************************************************************************************/

        [SerializeField]
#if UNITY_EDITOR
        [Editor.DrawAfterEvents]
#endif
        private ClipState.Transition[] _Others;

        /// <summary>[<see cref="SerializeField"/>] The transitions to play in order after this first one.</summary>
        public ref ClipState.Transition[] Others => ref _Others;

        /************************************************************************************************************************/

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ClipState.Transition previous = this;

            for (int i = 0; i < _Others.Length; i++)
            {
                var current = _Others[i];
                previous.Events.OnEnd += () => AnimancerEvent.CurrentState.Layer.Play(current);
                previous = current;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /************************************************************************************************************************/
    }
}
