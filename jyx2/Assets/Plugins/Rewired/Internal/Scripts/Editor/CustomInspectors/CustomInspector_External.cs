// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor {

    using UnityEngine;
    using UnityEditor;
    using Rewired;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class CustomInspector_External : UnityEditor.Editor {

        protected CustomInspector_Internal internalEditor;

        override public void OnInspectorGUI() {
            internalEditor.OnInspectorGUI();
        }

        protected void Enabled() {
            internalEditor.OnEnable();
        }
    }
}
