using Jyx2.InputCore.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jyx2.InputCore
{
    /// <summary>
    /// Wrapper for Rewired
    /// </summary>

    using RPlayer = Rewired.Player;
    using RController = Rewired.Controller;

    public static class Jyx2_Input
    {
        public static event Action<RController> OnControllerChange;

        public static event Action<bool> OnPlayerInputStateChange;

        private static RController m_CurController;


        private const int SystemPlayerId = 0;
        
        private static RPlayer m_RewiredPlayer;

        private static RPlayer CurrentPlayer
        {
            get
            {
                if (!ReInput.isReady)
                    return null;
                if (m_RewiredPlayer == null)
                {
                    m_RewiredPlayer = Rewired.ReInput.players.GetPlayer(SystemPlayerId);
                }
                return m_RewiredPlayer;
            }
        }

        private static bool IsPlayerValid => CurrentPlayer != null;

        public static bool IsPlayerContext => InputContextManager.Instance.IsPlayerContext;

        public static bool GetAnyButtonDown()
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetAnyButtonDown();
        }

        #region poll actions
        public static bool GetButtonDown(int actionId)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButtonDown(actionId);
        }

        //主要给Axis模拟按键用，比如GetButtonDown("MoveHorizontal") 表示向右 GetNegativeButtonDown("MoveHorizontal") 表示向左
        public static bool GetNegativeButtonDown(int actionId)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetNegativeButtonDown(actionId);
        }

        public static bool GetButtonUp(int actionId)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButtonUp(actionId);
        }
        
        public static float GetAxis(int actionId)
        {
            if (!IsPlayerValid)
                return 0;
            return CurrentPlayer.GetAxis(actionId);
        }

        public static float GetAxisRaw(int actionId)
        {
            if (!IsPlayerValid)
                return 0;
            return CurrentPlayer.GetAxisRaw(actionId);
        }

        public static Vector2 GetAxis2DRaw(int xActionId, int yActionId)
        {
            if (!IsPlayerValid)
                return Vector2.zero;
            return CurrentPlayer.GetAxis2DRaw(xActionId, yActionId);
        }


        public static Vector2 GetAxis2D(int xActionId, int yActionId)
        {
            if (!IsPlayerValid)
                return Vector2.zero;
            return CurrentPlayer.GetAxis2D(xActionId, yActionId);
        }

        public static bool GetButton(int actionId)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButton(actionId);
        }

        public static bool GetButtonDown(Jyx2PlayerAction actionId)
        {
            return GetButtonDown((int)actionId);
        }

        public static bool GetNegativeButtonDown(Jyx2PlayerAction actionId)
        {
            return GetNegativeButtonDown((int)actionId);
        }

        public static bool GetButtonUp(Jyx2PlayerAction actionId)
        {
            return GetButtonUp((int)actionId);
        }

        public static float GetAxis(Jyx2PlayerAction actionId)
        {
            return GetAxis((int)actionId);
        }

        public static Vector2 GetAxis2D(Jyx2PlayerAction xActionId, Jyx2PlayerAction yActionId)
        {
            return GetAxis2D((int)xActionId, (int)yActionId);
        }

        public static float GetAxisRaw(Jyx2PlayerAction actionId)
        {
            return GetAxisRaw((int)actionId);
        }

        public static Vector2 GetAxis2DRaw(Jyx2PlayerAction xActionId, Jyx2PlayerAction yActionId)
        {
            return GetAxis2DRaw((int)xActionId, (int)yActionId);
        }

        public static bool GetButton(Jyx2PlayerAction actionId)
        {
            return GetButton((int)actionId);
        }

        public static bool GetButtonDown(string buttonName)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButtonDown(buttonName);
        }

        public static bool GetButton(string buttonName)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButton(buttonName);
        }

        public static bool GetButtonUp(string buttonName)
        {
            if (!IsPlayerValid)
                return false;
            return CurrentPlayer.GetButtonUp(buttonName);
        }

        #endregion

        #region UI Direction 
        
        public static bool IsUIMoveLeft()
        {
           return  GetNegativeButtonDown(Jyx2ActionConst.UIHorizontal);
        }

        public static bool IsUIMoveRight()
        {
            return GetButtonDown(Jyx2ActionConst.UIHorizontal);
        }

        public static bool IsUIMoveUp()
        {
            return GetButtonDown(Jyx2ActionConst.UIVertical);
        }

        public static bool IsUIMoveDown()
        {
            return GetNegativeButtonDown(Jyx2ActionConst.UIVertical);
        }

        #endregion

        public static RController GetLastActiveController()
        {
            if (!IsPlayerValid)
                return null;
            var lastController = CurrentPlayer.controllers.GetLastActiveController();
            return lastController;
        }

        public static void OnUpdate()
        {
            if (!IsPlayerValid)
                return;
            UpdateControllerState();
        }

        private static void UpdateControllerState()
        {
            var lastController = GetLastActiveController();
            if(m_CurController != lastController)
            {
                m_CurController = lastController;
                OnControllerChange?.Invoke(lastController);
#if UNITY_EDITOR
                if (lastController != null)
                {
                    //Debug.Log("NewController:" + lastController.name);
                }
#endif
            }
        }

        internal static void FirePlayerInputChangeEvent(bool isActive)
        {
            OnPlayerInputStateChange?.Invoke(isActive);
        }

        public static string GetAllJoyStickJsonData()
        {
            Dictionary<string, IList<ControllerElementIdentifier>> m_AllElements = new Dictionary<string, IList<ControllerElementIdentifier>>();
            var allJoySticks = ReInput.controllers.Joysticks;
            foreach (var joyStick in allJoySticks)
            {
                m_AllElements[joyStick.hardwareIdentifier] = joyStick.ElementIdentifiers;
            }
            var serializeSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            var json = JsonConvert.SerializeObject(m_AllElements, serializeSettings);
            return json;
        }

        public static string GetKeyBoardElementJsonData()
        {
            var keyBoard = ReInput.controllers.Keyboard;
            var allElements = keyBoard.ElementIdentifiers.Select(e => keyBoard.GetKeyCodeById(e.id).ToString());
            var serializeSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            var json = JsonConvert.SerializeObject(allElements, serializeSettings);
            return json;
        }


        public static string GetActionButtonName(Jyx2PlayerAction actionID, AxisRange axisRange = AxisRange.Positive)
        {
            if (!IsPlayerValid)
                return "?";
            return CurrentPlayer.GetPlayerButtonName(actionID, axisRange);
        }
    }
}