// Copyright (c) 2020 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Integration.UnityUI {
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    [AddComponentMenu("Rewired/Rewired Event System")]
    public class RewiredEventSystem : EventSystem {
        
        [Tooltip("If enabled, the Event System will be updated every frame even if other Event Systems are enabled. Otherwise, only EventSystem.current will be updated.")]
        [SerializeField]
        private bool _alwaysUpdate;
        
        ///<summary>
        ///If enabled, the Event System will be updated every frame even if other Event Systems are enabled. Otherwise, only EventSystem.current will be updated.
        ///</summary>
        public bool alwaysUpdate {
            get {
                return _alwaysUpdate;
            }
            set {
                _alwaysUpdate = value;
            }
        }
    
        protected override void Update() {
            if(alwaysUpdate) {
                EventSystem prev = current;
                if(prev != this) current = this;
                try {
                    base.Update();
                } finally {
                    if(prev != this) current = prev;
                }
            } else {
                base.Update();
            }
        }
    }
}