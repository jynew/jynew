#if UNITY_2024 || UNITY_2025 || UNITY_2026 || UNITY_2027 || UNITY_2028 || UNITY_2029 || UNITY_2030
#define UNITY_2024_PLUS
#endif

#if UNITY_2023 || UNITY_2024_PLUS
#define UNITY_2023_PLUS
#endif

#if UNITY_2022 || UNITY_2023_PLUS
#define UNITY_2022_PLUS
#endif

#if UNITY_2021 || UNITY_2022_PLUS
#define UNITY_2021_PLUS
#endif

#if UNITY_2020 || UNITY_2021_PLUS
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_PLUS
#define SUPPORTS_UNITY_UI
#endif

#if (UNITY_PS4 && UNITY_2018_PLUS) || REWIRED_DEBUG_UNITY_PS4_2018_PLUS
#define UNITY_PS4_2018_PLUS
#endif

#if UNITY_2018_PLUS || UNITY_2017_4_OR_NEWER
#define PS4INPUT_NEW_PAD_API
#endif

// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0067

namespace Rewired.Utils {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Rewired.Utils.Interfaces;

    /// <exclude></exclude>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ExternalTools : IExternalTools {

        private static System.Func<object> _getPlatformInitializerDelegate;
        public static System.Func<object> getPlatformInitializerDelegate {
            get {
                return _getPlatformInitializerDelegate;
            }
            set {
                _getPlatformInitializerDelegate = value;
            }
        }

        public ExternalTools() {
#if UNITY_EDITOR
#if UNITY_2018_PLUS
            UnityEditor.EditorApplication.pauseStateChanged += OnEditorPauseStateChanged;
#else
            UnityEditor.EditorApplication.update += OnEditorUpdate;            
#endif
            _isEditorPaused = UnityEditor.EditorApplication.isPaused; // get initial state
#endif
        }

        public void Destroy() {
#if UNITY_EDITOR
#if UNITY_2018_PLUS
            UnityEditor.EditorApplication.pauseStateChanged -= OnEditorPauseStateChanged;
#else
            UnityEditor.EditorApplication.update -= OnEditorUpdate;
#endif
#endif
        }

        private bool _isEditorPaused;
        public bool isEditorPaused {
            get {
                return _isEditorPaused;
            }
        }

        private System.Action<bool> _EditorPausedStateChangedEvent;
        public event System.Action<bool> EditorPausedStateChangedEvent {
            add { _EditorPausedStateChangedEvent += value; }
            remove { _EditorPausedStateChangedEvent -= value; }
        }

#if UNITY_EDITOR
#if UNITY_2018_PLUS
        private void OnEditorPauseStateChanged(UnityEditor.PauseState state) {
            _isEditorPaused = state == UnityEditor.PauseState.Paused;
            var evt = _EditorPausedStateChangedEvent;
            if (evt != null) evt(_isEditorPaused);
        }
#else
        private void OnEditorUpdate() {
            // Watch EditorApplication.isPaused state
            bool isPaused = UnityEditor.EditorApplication.isPaused;
            if(isPaused != _isEditorPaused) {
                _isEditorPaused = isPaused;
                var evt = _EditorPausedStateChangedEvent;
                if (evt != null) evt(_isEditorPaused);
            }
        }
#endif
#endif

        public object GetPlatformInitializer() {
#if UNITY_5_PLUS
#if (!UNITY_EDITOR && UNITY_STANDALONE_WIN) || UNITY_EDITOR_WIN
            return Rewired.Utils.Platforms.Windows.Main.GetPlatformInitializer();
#elif (!UNITY_EDITOR && UNITY_STANDALONE_OSX) || UNITY_EDITOR_OSX
            return Rewired.Utils.Platforms.OSX.Main.GetPlatformInitializer();
#elif (!UNITY_EDITOR && UNITY_STANDALONE_LINUX) || UNITY_EDITOR_LINUX
            return Rewired.Utils.Platforms.Linux.Main.GetPlatformInitializer();
#elif UNITY_WEBGL && !UNITY_EDITOR
            return Rewired.Utils.Platforms.WebGL.Main.GetPlatformInitializer();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return Rewired.Utils.Platforms.Android.Main.GetPlatformInitializer();
#else
            if(_getPlatformInitializerDelegate != null) return _getPlatformInitializerDelegate();
            else return null;
#endif
#else
#if UNITY_WEBGL && !UNITY_EDITOR
            return Rewired.Utils.Platforms.WebGL.Main.GetPlatformInitializer();
#else
            if (_getPlatformInitializerDelegate != null) return _getPlatformInitializerDelegate();
            else return null;
#endif
#endif
        }

        public string GetFocusedEditorWindowTitle() {
#if UNITY_EDITOR
            UnityEditor.EditorWindow window = UnityEditor.EditorWindow.focusedWindow;
#if UNITY_2017_PLUS
            return window != null ? window.titleContent.text : string.Empty;
#else
            return window != null ? window.title : string.Empty;
#endif
#else
            return string.Empty;
#endif
        }

        public bool IsEditorSceneViewFocused() {
#if UNITY_EDITOR
            ArrayList sceneViews = UnityEditor.SceneView.sceneViews;
            if (sceneViews == null) return false;
            string focusedWindowTitle = GetFocusedEditorWindowTitle();
            for (int i = 0; i < sceneViews.Count; i++) {
                UnityEditor.SceneView sceneView = sceneViews[i] as UnityEditor.SceneView;
                if (sceneView == null) continue;
#if UNITY_2017_PLUS
                if (sceneView.titleContent.text == focusedWindowTitle) return true;
#else
                if (sceneView.title == focusedWindowTitle) return true;
#endif
            }
            return false;
#else
            return false;
#endif
        }

        // Linux Tools
#if UNITY_5_PLUS && UNITY_STANDALONE_LINUX
        public bool LinuxInput_IsJoystickPreconfigured(string name) {
            return UnityEngine.Input.IsJoystickPreconfigured(name);
        }
#else
        public bool LinuxInput_IsJoystickPreconfigured(string name) {
            return false;

        }
#endif

        // Xbox One Tools

#if UNITY_XBOXONE

        public event System.Action<uint, bool> XboxOneInput_OnGamepadStateChange {
            add { XboxOneInput.OnGamepadStateChange += new XboxOneInput.OnGamepadStateChangeEvent(value); }
            remove { XboxOneInput.OnGamepadStateChange -= new XboxOneInput.OnGamepadStateChangeEvent(value); }
        }

        public int XboxOneInput_GetUserIdForGamepad(uint id) { return XboxOneInput.GetUserIdForGamepad(id); }

        public ulong XboxOneInput_GetControllerId(uint unityJoystickId) { return XboxOneInput.GetControllerId(unityJoystickId); }

        public bool XboxOneInput_IsGamepadActive(uint unityJoystickId) { return XboxOneInput.IsGamepadActive(unityJoystickId); }

        public string XboxOneInput_GetControllerType(ulong xboxControllerId) { return XboxOneInput.GetControllerType(xboxControllerId); }

        public uint XboxOneInput_GetJoystickId(ulong xboxControllerId) { return XboxOneInput.GetJoystickId(xboxControllerId); }
        
        private bool _xboxOne_gamepadDLLException;

        public void XboxOne_Gamepad_UpdatePlugin() {
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
            if(_xboxOne_gamepadDLLException) return;
            try {
                Ext_Gamepad_UpdatePlugin();
            } catch {
                UnityEngine.Debug.LogError("Rewired: An exception occurred updating vibration. Gamepad vibration will not function. Did you install the required Gamepad.dll dependency? See Special Platforms - Xbox One in the documentation for information.");
                _xboxOne_gamepadDLLException = true;
            }
#endif
        }

        public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel) {
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
            if(_xboxOne_gamepadDLLException) return false;
            try {
                return Ext_Gamepad_SetGamepadVibration(xboxOneJoystickId, leftMotor, rightMotor, leftTriggerLevel, rightTriggerLevel);
            } catch {
                return false;
            }
#else
            return false;
#endif
        }

        public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS) {
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
            if(_xboxOne_gamepadDLLException) return;
            Rewired.Platforms.XboxOne.XboxOneGamepadMotorType motor = (Rewired.Platforms.XboxOne.XboxOneGamepadMotorType)motorInt;
            try {
                switch(motor) {
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.LeftMotor:
                        Ext_Gamepad_PulseVibrateLeftMotor(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.RightMotor:
                        Ext_Gamepad_PulseVibrateRightMotor(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.LeftTriggerMotor:
                        Ext_Gamepad_PulseVibrateLeftTrigger(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.RightTriggerMotor:
                        Ext_Gamepad_PulseVibrateRightTrigger(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    default: throw new System.NotImplementedException();
                }
            } catch {
            }
#endif
        }

#if !REWIRED_XBOXONE_DISABLE_VIBRATION

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "UpdatePlugin")]
        private static extern void Ext_Gamepad_UpdatePlugin();

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "SetGamepadVibration")]
        private static extern bool Ext_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel);

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsLeftMotor")]
        private static extern void Ext_Gamepad_PulseVibrateLeftMotor(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsRightMotor")]
        private static extern void Ext_Gamepad_PulseVibrateRightMotor(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsLeftTrigger")]
        private static extern void Ext_Gamepad_PulseVibrateLeftTrigger(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);
        
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsRightTrigger")]
        private static extern void Ext_Gamepad_PulseVibrateRightTrigger(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);

#endif
#else
        public event System.Action<uint, bool> XboxOneInput_OnGamepadStateChange;

        public int XboxOneInput_GetUserIdForGamepad(uint id) { return 0; }

        public ulong XboxOneInput_GetControllerId(uint unityJoystickId) { return 0; }

        public bool XboxOneInput_IsGamepadActive(uint unityJoystickId) { return false; }

        public string XboxOneInput_GetControllerType(ulong xboxControllerId) { return string.Empty; }

        public uint XboxOneInput_GetJoystickId(ulong xboxControllerId) { return 0; }

        public void XboxOne_Gamepad_UpdatePlugin() { }

        public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel) { return false; }

        public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS) { }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR

        const int API_LEVEL_HONEYCOMB = 9;
        const int API_LEVEL_KITKAT = 19;

        public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids) {

            vids = new List<int>();
            pids = new List<int>();

            try {
                if(GetAndroidAPILevel() < API_LEVEL_KITKAT) return;

                AndroidJavaClass android_view_InputDevice = new AndroidJavaClass("android.view.InputDevice");

                int[] ids = null;
                using(AndroidJavaObject jniArray = android_view_InputDevice.CallStatic<AndroidJavaObject>("getDeviceIds")) {
                    if(jniArray != null) {
                        ids = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jniArray.GetRawObject());
                    }
                }

                if(ids == null) return;
                for(int i = 0; i < ids.Length; i++) {
                    try {
                        using(AndroidJavaObject jo = android_view_InputDevice.CallStatic<AndroidJavaObject>("getDevice", ids[i])) {
                            if(jo == null) continue;
                            vids.Add(jo.Call<int>("getVendorId"));
                            pids.Add(jo.Call<int>("getProductId"));
                        }
                    } catch {
                    }
                }
            } catch {
            }
        }

        public int GetAndroidAPILevel() {
            try {
                // Get the Android SDK version
                int apiLevel = API_LEVEL_HONEYCOMB;
                using(var version = new AndroidJavaClass("android.os.Build$VERSION")) {
                    apiLevel = version.GetStatic<int>("SDK_INT");
                }
                return apiLevel;
            } catch {
                return -1;
            }
        }
#else
        public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids) {
            vids = new List<int>();
            pids = new List<int>();
        }

        public int GetAndroidAPILevel() {
            return -1;
        }
#endif

        #region Windows Standalone

#if UNITY_2021_PLUS

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || (UNITY_EDITOR_WIN)

        public void WindowsStandalone_ForwardRawInput(System.IntPtr rawInputHeaderIndices, System.IntPtr rawInputDataIndices, uint indicesCount, System.IntPtr rawInputData, uint rawInputDataSize) {
#if UNITY_2022_PLUS
            UnityEngine.Windows.Input.ForwardRawInput(rawInputHeaderIndices, rawInputDataIndices, indicesCount, rawInputData, rawInputDataSize);
#elif UNITY_2021_2_OR_NEWER
            Rewired.Internal.Windows.Functions.ForwardRawInput(rawInputHeaderIndices, rawInputDataIndices, indicesCount, rawInputData, rawInputDataSize);
#else
            throw new System.NotImplementedException();
#endif
        }

#else
        public void WindowsStandalone_ForwardRawInput(System.IntPtr rawInputHeaderIndices, System.IntPtr rawInputDataIndices, uint indicesCount, System.IntPtr rawInputData, uint rawInputDataSize) {}
#endif

#endif

        #endregion

        #region Unity UI


#if SUPPORTS_UNITY_UI

        public bool UnityUI_Graphic_GetRaycastTarget(object graphic) {
            if (graphic as UnityEngine.UI.Graphic == null) return false;
#if UNITY_5_2_PLUS
            return (graphic as UnityEngine.UI.Graphic).raycastTarget;
#else
            return true;
#endif
        }
        public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value) {
            if (graphic as UnityEngine.UI.Graphic == null) return;
#if UNITY_5_2_PLUS
            (graphic as UnityEngine.UI.Graphic).raycastTarget = value;
#endif
        }
#else
        public bool UnityUI_Graphic_GetRaycastTarget(object graphic) { return true; }
        public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value) { }
#endif

        #endregion

        #region Touch

        public bool UnityInput_IsTouchPressureSupported {
            get {
#if UNITY_5_3_PLUS
                return UnityEngine.Input.touchPressureSupported;
#else
                return false;
#endif
            }
        }

        public float UnityInput_GetTouchPressure(ref UnityEngine.Touch touch) {
#if UNITY_5_3_PLUS
            return touch.pressure;
#else
            return touch.phase != UnityEngine.TouchPhase.Ended &&
                touch.phase != UnityEngine.TouchPhase.Canceled
                ? 1.0f : 0.0f;
#endif
        }

        public float UnityInput_GetTouchMaximumPossiblePressure(ref UnityEngine.Touch touch) {
#if UNITY_5_3_PLUS
            return touch.maximumPossiblePressure;
#else
            return 1.0f;
#endif
        }

        #endregion

        #region Controller Templates

        public IControllerTemplate CreateControllerTemplate(System.Guid typeGuid, object payload) {
            return Rewired.Internal.ControllerTemplateFactory.Create(typeGuid, payload);
        }

        public System.Type[] GetControllerTemplateTypes() {
            return Rewired.Internal.ControllerTemplateFactory.templateTypes;
        }

        public System.Type[] GetControllerTemplateInterfaceTypes() {
            return Rewired.Internal.ControllerTemplateFactory.templateInterfaceTypes;
        }

        #endregion
    }
}