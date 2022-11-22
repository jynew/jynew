// Copyright (c) 2021 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || (UNITY_EDITOR_WIN)

namespace Rewired.Internal.Windows {

    public static class Functions {

#if UNITY_2021_2_OR_NEWER
        public static void ForwardRawInput(System.IntPtr rawInputHeaderIndices, System.IntPtr rawInputDataIndices, uint indicesCount, System.IntPtr rawInputData, uint rawInputDataSize) {
            unsafe {
                UnityEngine.Windows.Input.ForwardRawInput((uint*)rawInputHeaderIndices, (uint*)rawInputDataIndices, indicesCount, (byte*)rawInputData, rawInputDataSize);
            }
        }
#endif

    }
}

#endif
