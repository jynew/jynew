#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
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

#if UNITY_PS4

        public Vector3 PS4Input_GetLastAcceleration(int id) {
#if PS4INPUT_NEW_PAD_API
            return UnityEngine.PS4.PS4Input.PadGetLastAcceleration(id);
#else
            return UnityEngine.PS4.PS4Input.GetLastAcceleration(id);
#endif
        }

        public Vector3 PS4Input_GetLastGyro(int id) {
#if PS4INPUT_NEW_PAD_API
            return UnityEngine.PS4.PS4Input.PadGetLastGyro(id);
#else
            return UnityEngine.PS4.PS4Input.GetLastGyro(id);
#endif
        }

        public Vector4 PS4Input_GetLastOrientation(int id) {
#if PS4INPUT_NEW_PAD_API
            return UnityEngine.PS4.PS4Input.PadGetLastOrientation(id);
#else
            return UnityEngine.PS4.PS4Input.GetLastOrientation(id);
#endif
        }

        public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id) {
            UnityEngine.PS4.PS4Input.GetLastTouchData(id, out touchNum, out touch0x, out touch0y, out touch0id, out touch1x, out touch1y, out touch1id);
        }

        public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType) {
            UnityEngine.PS4.PS4Input.ConnectionType connectionTypeEnum;
            UnityEngine.PS4.PS4Input.GetPadControllerInformation(id, out touchpixelDensity, out touchResolutionX, out touchResolutionY, out analogDeadZoneLeft, out analogDeadZoneright, out connectionTypeEnum);
            connectionType = (int)connectionTypeEnum;
        }

        public void PS4Input_PadSetMotionSensorState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetMotionSensorState(id, bEnable);
        }

        public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetTiltCorrectionState(id, bEnable);
        }

        public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetAngularVelocityDeadbandState(id, bEnable);
        }

        public void PS4Input_PadSetLightBar(int id, int red, int green, int blue) {
            UnityEngine.PS4.PS4Input.PadSetLightBar(id, red, green, blue);
        }

        public void PS4Input_PadResetLightBar(int id) {
            UnityEngine.PS4.PS4Input.PadResetLightBar(id);
        }

        public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor) {
            UnityEngine.PS4.PS4Input.PadSetVibration(id, largeMotor, smallMotor);
        }

        public void PS4Input_PadResetOrientation(int id) {
            UnityEngine.PS4.PS4Input.PadResetOrientation(id);
        }

        public bool PS4Input_PadIsConnected(int id) {
            return UnityEngine.PS4.PS4Input.PadIsConnected(id);
        }

        public void PS4Input_GetUsersDetails(int slot, object loggedInUser) {
            if(loggedInUser == null) throw new System.ArgumentNullException("loggedInUser");
#if PS4INPUT_NEW_PAD_API
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.GetUsersDetails(slot);
#else
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.PadGetUsersDetails(slot);
#endif
            Rewired.Platforms.PS4.Internal.LoggedInUser retUser = loggedInUser as Rewired.Platforms.PS4.Internal.LoggedInUser;
            if(retUser == null) throw new System.ArgumentException("loggedInUser is not the correct type.");

            retUser.status = user.status;
            retUser.primaryUser = user.primaryUser;
            retUser.userId = user.userId;
            retUser.color = user.color;
            retUser.userName = user.userName;
            retUser.padHandle = user.padHandle;
            retUser.move0Handle = user.move0Handle;
            retUser.move1Handle = user.move1Handle;
#if UNITY_PS4_2018_PLUS
            retUser.aimHandle = user.aimHandle;
#endif
        }

        public int PS4Input_GetDeviceClassForHandle(int handle) {
#if UNITY_PS4_2018_PLUS
            return (int)UnityEngine.PS4.PS4Input.GetDeviceClassForHandle(handle);
#else
            return -1;
#endif
        }

        public string PS4Input_GetDeviceClassString(int intValue) {
#if UNITY_PS4_2018_PLUS
            return ((UnityEngine.PS4.PS4Input.DeviceClass)intValue).ToString();
#else
            return null;
#endif
        }

        public int PS4Input_PadGetUsersHandles2(int maxControllers, int[] handles) {
#if UNITY_PS4_2018_PLUS
            return UnityEngine.PS4.PS4Input.PadGetUsersHandles2(maxControllers, handles);
#else
            return 0;
#endif
        }

#if UNITY_PS4_2018_PLUS

        private readonly UnityEngine.PS4.PS4Input.ControllerInformation _controllerInformation = new UnityEngine.PS4.PS4Input.ControllerInformation();

        public void PS4Input_GetSpecialControllerInformation(int id, int padIndex, object controllerInformation) {
            if(controllerInformation == null) throw new System.ArgumentNullException("controllerInformation");
            Rewired.Platforms.PS4.Internal.ControllerInformation tControllerInformation = controllerInformation as Rewired.Platforms.PS4.Internal.ControllerInformation;
            if(tControllerInformation == null) throw new System.ArgumentException("controllerInformation is not the correct type.");
            UnityEngine.PS4.PS4Input.ControllerInformation c = _controllerInformation;
            UnityEngine.PS4.PS4Input.GetSpecialControllerInformation(id, padIndex, ref c);
            tControllerInformation.padControllerInformation.touchPadInfo.pixelDensity = c.padControllerInformation.touchPadInfo.pixelDensity;
            tControllerInformation.padControllerInformation.touchPadInfo.resolutionX = c.padControllerInformation.touchPadInfo.resolutionX;
            tControllerInformation.padControllerInformation.touchPadInfo.resolutionY = c.padControllerInformation.touchPadInfo.resolutionY;
            tControllerInformation.padControllerInformation.stickInfo.deadZoneLeft = c.padControllerInformation.stickInfo.deadZoneLeft;
            tControllerInformation.padControllerInformation.stickInfo.deadZoneRight = c.padControllerInformation.stickInfo.deadZoneRight;
            tControllerInformation.padControllerInformation.connectionType = c.padControllerInformation.connectionType;
            tControllerInformation.padControllerInformation.connectedCount = c.padControllerInformation.connectedCount;
            tControllerInformation.padControllerInformation.connected = c.padControllerInformation.connected;
            tControllerInformation.padControllerInformation.deviceClass = (int)c.padControllerInformation.deviceClass;
            tControllerInformation.padDeviceClassExtendedInformation.deviceClass = (int)c.padDeviceClassExtendedInformation.deviceClass;
            tControllerInformation.padDeviceClassExtendedInformation.capability = c.padDeviceClassExtendedInformation.capability;
            tControllerInformation.padDeviceClassExtendedInformation.quantityOfSelectorSwitch = c.padDeviceClassExtendedInformation.quantityOfSelectorSwitch;
            tControllerInformation.padDeviceClassExtendedInformation.maxPhysicalWheelAngle = c.padDeviceClassExtendedInformation.maxPhysicalWheelAngle;
        }

        public Vector3 PS4Input_SpecialGetLastAcceleration(int id) {
            return UnityEngine.PS4.PS4Input.SpecialGetLastAcceleration(id);
        }

        public Vector3 PS4Input_SpecialGetLastGyro(int id) {
            return UnityEngine.PS4.PS4Input.SpecialGetLastGyro(id);
        }

        public Vector4 PS4Input_SpecialGetLastOrientation(int id) {
            return UnityEngine.PS4.PS4Input.SpecialGetLastOrientation(id);
        }

        public int PS4Input_SpecialGetUsersHandles(int maxNumberControllers, int[] handles) {
            return UnityEngine.PS4.PS4Input.SpecialGetUsersHandles(maxNumberControllers, handles);
        }

        public int PS4Input_SpecialGetUsersHandles2(int maxNumberControllers, int[] handles) {
            return UnityEngine.PS4.PS4Input.SpecialGetUsersHandles2(maxNumberControllers, handles);
        }

        public bool PS4Input_SpecialIsConnected(int id) {
            return UnityEngine.PS4.PS4Input.SpecialIsConnected(id);
        }

        public void PS4Input_SpecialResetLightSphere(int id) {
            UnityEngine.PS4.PS4Input.SpecialResetLightSphere(id);
        }

        public void PS4Input_SpecialResetOrientation(int id) {
            UnityEngine.PS4.PS4Input.SpecialResetOrientation(id);
        }

        public void PS4Input_SpecialSetAngularVelocityDeadbandState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.SpecialSetAngularVelocityDeadbandState(id, bEnable);
        }

        public void PS4Input_SpecialSetLightSphere(int id, int red, int green, int blue) {
            UnityEngine.PS4.PS4Input.SpecialSetLightSphere(id, red, green, blue);
        }

        public void PS4Input_SpecialSetMotionSensorState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.SpecialSetMotionSensorState(id, bEnable);
        }

        public void PS4Input_SpecialSetTiltCorrectionState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.SpecialSetTiltCorrectionState(id,  bEnable);
        }

        public void PS4Input_SpecialSetVibration(int id, int largeMotor, int smallMotor) {
            UnityEngine.PS4.PS4Input.SpecialSetVibration(id, largeMotor, smallMotor);
        }

        // Aim

        public Vector3 PS4Input_AimGetLastAcceleration(int id) {
            return UnityEngine.PS4.PS4Input.AimGetLastAcceleration(id);
        }

        public Vector3 PS4Input_AimGetLastGyro(int id) {
            return UnityEngine.PS4.PS4Input.AimGetLastGyro(id);
        }

        public Vector4 PS4Input_AimGetLastOrientation(int id) {
            return UnityEngine.PS4.PS4Input.AimGetLastOrientation(id);
        }

        public int PS4Input_AimGetUsersHandles(int maxNumberControllers, int[] handles) {
            return UnityEngine.PS4.PS4Input.AimGetUsersHandles(maxNumberControllers, handles);
        }

        public int PS4Input_AimGetUsersHandles2(int maxNumberControllers, int[] handles) {
            return UnityEngine.PS4.PS4Input.AimGetUsersHandles2(maxNumberControllers, handles);
        }

        public bool PS4Input_AimIsConnected(int id) {
            return UnityEngine.PS4.PS4Input.AimIsConnected(id);
        }

        public void PS4Input_AimResetLightSphere(int id) {
            UnityEngine.PS4.PS4Input.AimResetLightSphere(id);
        }

        public void PS4Input_AimResetOrientation(int id) {
            UnityEngine.PS4.PS4Input.AimResetOrientation(id);
        }

        public void PS4Input_AimSetAngularVelocityDeadbandState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.AimSetAngularVelocityDeadbandState(id, bEnable);
        }

        public void PS4Input_AimSetLightSphere(int id, int red, int green, int blue) {
            UnityEngine.PS4.PS4Input.AimSetLightSphere(id, red, green, blue);
        }

        public void PS4Input_AimSetMotionSensorState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.AimSetMotionSensorState(id, bEnable);
        }

        public void PS4Input_AimSetTiltCorrectionState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.AimSetTiltCorrectionState(id, bEnable);
        }

        public void PS4Input_AimSetVibration(int id, int largeMotor, int smallMotor) {
            UnityEngine.PS4.PS4Input.AimSetVibration(id, largeMotor, smallMotor);
        }

        // Move

        public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index) {
            return UnityEngine.PS4.PS4Input.GetLastMoveAcceleration(id, index);
        }

        public Vector3 PS4Input_GetLastMoveGyro(int id, int index) {
            return UnityEngine.PS4.PS4Input.GetLastMoveGyro(id, index);
        }

        public int PS4Input_MoveGetButtons(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveGetButtons(id, index);
        }

        public int PS4Input_MoveGetAnalogButton(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveGetAnalogButton(id, index);
        }

        public bool PS4Input_MoveIsConnected(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveIsConnected(id, index);
        }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles) {
            return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers, primaryHandles, secondaryHandles);
        }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles) {
            return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers, primaryHandles);
        }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers) {
            return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers);
        }

        public System.IntPtr PS4Input_MoveGetControllerInputForTracking() {
            return UnityEngine.PS4.PS4Input.MoveGetControllerInputForTracking();
        }

        public int PS4Input_MoveSetLightSphere(int id, int index, int red, int green, int blue) {
            return UnityEngine.PS4.PS4Input.MoveSetLightSphere(id, index, red, green, blue);
        }

        public int PS4Input_MoveSetVibration(int id, int index, int motor) {
            return UnityEngine.PS4.PS4Input.MoveSetVibration(id, index, motor);
        }

#endif

#else
        public Vector3 PS4Input_GetLastAcceleration(int id) { return Vector3.zero; }

        public Vector3 PS4Input_GetLastGyro(int id) { return Vector3.zero; }

        public Vector4 PS4Input_GetLastOrientation(int id) { return Vector4.zero; }

        public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id) { touchNum = 0; touch0x = 0; touch0y = 0; touch0id = 0; touch1x = 0; touch1y = 0; touch1id = 0; }

        public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType) { touchpixelDensity = 0f; touchResolutionX = 0; touchResolutionY = 0; analogDeadZoneLeft = 0; analogDeadZoneright = 0; connectionType = 0; }

        public void PS4Input_PadSetMotionSensorState(int id, bool bEnable) { }

        public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable) { }

        public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable) { }

        public void PS4Input_PadSetLightBar(int id, int red, int green, int blue) { }

        public void PS4Input_PadResetLightBar(int id) { }

        public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor) { }

        public void PS4Input_PadResetOrientation(int id) { }

        public bool PS4Input_PadIsConnected(int id) { return false; }

        public void PS4Input_GetUsersDetails(int slot, object loggedInUser) { }

        public int PS4Input_GetDeviceClassForHandle(int handle) { return -1; }

        public string PS4Input_GetDeviceClassString(int intValue) { return null; }

        public int PS4Input_PadGetUsersHandles2(int maxControllers, int[] handles) { return 0; }

#if UNITY_2018_PLUS

        public void PS4Input_GetSpecialControllerInformation(int id, int padIndex, object controllerInformation) { }

        public Vector3 PS4Input_SpecialGetLastAcceleration(int id) { return Vector3.zero; }

        public Vector3 PS4Input_SpecialGetLastGyro(int id) { return Vector3.zero; }

        public Vector4 PS4Input_SpecialGetLastOrientation(int id) { return Vector4.zero; }

        public int PS4Input_SpecialGetUsersHandles(int maxNumberControllers, int[] handles) { return 0; }

        public int PS4Input_SpecialGetUsersHandles2(int maxNumberControllers, int[] handles) { return 0; }

        public bool PS4Input_SpecialIsConnected(int id) { return false; }

        public void PS4Input_SpecialResetLightSphere(int id) { }

        public void PS4Input_SpecialResetOrientation(int id) { }

        public void PS4Input_SpecialSetAngularVelocityDeadbandState(int id, bool bEnable) { }

        public void PS4Input_SpecialSetLightSphere(int id, int red, int green, int blue) { }

        public void PS4Input_SpecialSetMotionSensorState(int id, bool bEnable) { }

        public void PS4Input_SpecialSetTiltCorrectionState(int id, bool bEnable) { }

        public void PS4Input_SpecialSetVibration(int id, int largeMotor, int smallMotor) { }

        // Aim

        public Vector3 PS4Input_AimGetLastAcceleration(int id) { return Vector3.zero; }

        public Vector3 PS4Input_AimGetLastGyro(int id) { return Vector3.zero; }

        public Vector4 PS4Input_AimGetLastOrientation(int id) { return Vector4.zero; }

        public int PS4Input_AimGetUsersHandles(int maxNumberControllers, int[] handles) { return 0; }

        public int PS4Input_AimGetUsersHandles2(int maxNumberControllers, int[] handles) { return 0; }

        public bool PS4Input_AimIsConnected(int id) { return false; }

        public void PS4Input_AimResetLightSphere(int id) { }

        public void PS4Input_AimResetOrientation(int id) { }

        public void PS4Input_AimSetAngularVelocityDeadbandState(int id, bool bEnable) { }

        public void PS4Input_AimSetLightSphere(int id, int red, int green, int blue) { }

        public void PS4Input_AimSetMotionSensorState(int id, bool bEnable) { }

        public void PS4Input_AimSetTiltCorrectionState(int id, bool bEnable) { }

        public void PS4Input_AimSetVibration(int id, int largeMotor, int smallMotor) { }

        // Move

        public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index) { return Vector3.zero; }

        public Vector3 PS4Input_GetLastMoveGyro(int id, int index) { return Vector3.zero; }

        public int PS4Input_MoveGetButtons(int id, int index) { return 0; }

        public int PS4Input_MoveGetAnalogButton(int id, int index) { return 0; }

        public bool PS4Input_MoveIsConnected(int id, int index) { return false; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles) { return 0; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles) { return 0; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers) { return 0; }

        public System.IntPtr PS4Input_MoveGetControllerInputForTracking() { return System.IntPtr.Zero; }

        public int PS4Input_MoveSetLightSphere(int id, int index, int red, int green, int blue) { return 0; }

        public int PS4Input_MoveSetVibration(int id, int index, int motor) { return 0; }

#endif

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

#if UNITY_2021_1_OR_NEWER

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || (UNITY_EDITOR_WIN)

        public void WindowsStandalone_ForwardRawInput(System.IntPtr rawInputHeaderIndices, System.IntPtr rawInputDataIndices, uint indicesCount, System.IntPtr rawInputData, uint rawInputDataSize) {
#if UNITY_2021_2_OR_NEWER
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