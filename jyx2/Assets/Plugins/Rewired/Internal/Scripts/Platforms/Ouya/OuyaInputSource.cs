// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

#if UNITY_ANDROID && !UNITY_EDITOR && REWIRED_OUYA

namespace Rewired.Platforms.Ouya {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using tv.ouya.console.api;
    using Rewired;
    using Rewired.Platforms.Custom;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class OuyaInputSource : CustomInputSource {

        private bool _initialized;

        public override bool isReady {
            get {
                if(!_initialized) {
                    TryInitialize(); // initialize the platform late because Ouya is not ready in one cycle
                }
                return _initialized;
            }
        }


        public OuyaInputSource() : base(20) { // Ouya = 20
            for(int i = 0; i < OuyaController.MAX_CONTROLLERS; i++) {
                AddJoystick(new OyuaJoystick("Controller " + (i + 1).ToString(), i, JoystickConnectedStateChanged));
            }
        }

        public override void Update() {
            if(!_initialized) return; // initialize the platform late because Ouya is not ready in one cycle

            // Update joysticks
            IList<Platforms.Custom.CustomInputSource.Joystick> joysticks = GetJoysticks();
            int joystickCount = joysticks.Count;
            for(int i = 0; i < joystickCount; i++) {
                joysticks[i].Update();
            }
        }

        private void JoystickConnectedStateChanged(int playerId, bool isConnected) {
            // Fire events
            if(isConnected) OnJoystickConnected();
            else OnJoystickDisconnected();
        }

        private bool TryInitialize() {
            if(!OuyaSDK.isIAPInitComplete()) return false;
            _initialized = true;
            return true;
        }

#region IDisposable Implementation

        private bool _disposed;

        public override void Dispose() {
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OuyaInputSource() {
            Dispose(false);
        }

        protected override void Dispose(bool disposing) {
            if(_disposed)
                return;

            if(disposing) {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

#endregion

#region Private Classes

        private sealed class OyuaJoystick : CustomInputSource.Joystick {

            // Consts

            private const int _axisCount = 6;
            private const int _buttonCount = 13;

            // Private Variables

            private int _playerId;
            private bool _isConnectedPrev;

            private System.Action<int, bool> joystickConnectedChangedDelegate;

            // Constructors

            public OyuaJoystick(string name, int playerId, System.Action<int, bool> joystickConnectedChangedDelegate)
                : base(name, (long?)playerId, 0, _axisCount, _buttonCount) {
                _playerId = playerId;
                this.joystickConnectedChangedDelegate = joystickConnectedChangedDelegate;
                _customName = name; // set custom controller name so it shows the controller id
            }

            // Public Methods

            public override void Update() {
                bool justConnected = false;

                // Check if joystick is connected and fire events for state changes
                _isConnected = OuyaSDK.OuyaInput.IsControllerConnected(_playerId);

                // Check for connection state changes
                if(_isConnected != _isConnectedPrev) { // joystick was connected or disconnected

                    _isConnectedPrev = _isConnected;

                    if(!_isConnected) { // just disconnected
                        joystickConnectedChangedDelegate(_playerId, _isConnected); // send events
                        return; // done
                    } else { // just connected
                        justConnected = true; // We will send the event at the end of the method
                    }
                }

                // Get Button
                IList<Platforms.Custom.CustomInputSource.Button> buttons = Buttons;
                buttons[0].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_O);
                buttons[1].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_U);
                buttons[2].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_Y);
                buttons[3].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_A);
                buttons[4].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_L1);
                buttons[5].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_R1);
                buttons[6].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_L3);
                buttons[7].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_R3);
                buttons[8].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_DPAD_UP);
                buttons[9].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_DPAD_RIGHT);
                buttons[10].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_DPAD_DOWN);
                buttons[11].value = OuyaSDK.OuyaInput.GetButton(_playerId, OuyaController.BUTTON_DPAD_LEFT);

                // Menu button - Never returns button event, only down and up in the same frame
                buttons[12].value =
                    OuyaSDK.OuyaInput.GetButtonDown(_playerId, OuyaController.BUTTON_MENU) |
                    OuyaSDK.OuyaInput.GetButtonUp(_playerId, OuyaController.BUTTON_MENU);

                // Axes
                IList<CustomInputSource.Axis> axes = Axes;

                // Smoothed Axes
                //axes[0].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_LS_X);
                //axes[1].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_LS_Y);
                //axes[2].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_RS_X);
                //axes[3].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_RS_Y);
                //axes[4].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_L2);
                //axes[5].value = OuyaSDK.OuyaInput.GetAxis(_playerId, OuyaController.AXIS_R2);

                // Raw Axes
                axes[0].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_LS_X);
                axes[1].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_LS_Y);
                axes[2].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_RS_X);
                axes[3].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_RS_Y);
                axes[4].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_L2);
                axes[5].value = OuyaSDK.OuyaInput.GetAxisRaw(_playerId, OuyaController.AXIS_R2);

                if(justConnected) { // send events on connection after getting latest values
                    UpdateJoystickInfo();
                    joystickConnectedChangedDelegate(_playerId, _isConnected);
                }
            }

            private void UpdateJoystickInfo() {
                OuyaController ouyaController = OuyaController.getControllerByPlayer(_playerId);
                if(ouyaController == null) return;
                _deviceName = ouyaController.getDeviceName();
            }
        }
#endregion
    }
}
#endif