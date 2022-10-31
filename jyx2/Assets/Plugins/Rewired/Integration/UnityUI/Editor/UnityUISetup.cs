// Copyright (c) 2021 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Integration.UnityUI {
    using UnityEngine;
    using UnityEditor;

    [InitializeOnLoad]
    public class UnityUISetup {

        const int SCRIPT_EXECUTION_ORDER = -1000;

        private static bool isBatchMode { get { return UnityEditorInternal.InternalEditorUtility.inBatchMode; } }

        static UnityUISetup() {
            if (isBatchMode) return;
            SetScriptExeutionOrder();
        }

        static void SetScriptExeutionOrder() {
            MonoScript ms = System.Array.Find(MonoImporter.GetAllRuntimeMonoScripts(), x => object.ReferenceEquals(x.GetClass(), typeof(RewiredEventSystem)));
            if (ms == null) return;

            if (MonoImporter.GetExecutionOrder(ms) == 0) {
                MonoImporter.SetExecutionOrder(ms, SCRIPT_EXECUTION_ORDER);
                Debug.Log("Rewired: Script execution order on " + typeof(RewiredEventSystem).Name + " set to " + SCRIPT_EXECUTION_ORDER + ".");
            }
        }
    }
}