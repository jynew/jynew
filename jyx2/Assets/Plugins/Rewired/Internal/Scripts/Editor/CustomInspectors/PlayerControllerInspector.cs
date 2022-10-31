// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor {

    using UnityEngine;
    using UnityEditor;
    using Rewired;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [CustomEditor(typeof(Rewired.Components.PlayerController))]
    public sealed class PlayerControllerInspector : CustomInspector_External {

        private void OnEnable() {
            internalEditor = new PlayerControllerInspector_Internal(this);
            internalEditor.OnEnable();
        }
    }
}