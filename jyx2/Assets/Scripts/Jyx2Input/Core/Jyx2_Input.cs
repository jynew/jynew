using System;
using System.Collections;
using System.Collections.Generic;
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

        private static RController m_CurController;


        private const int SystemPlayerId = 0;
        
        private static RPlayer m_RewiredPlayer;

        private static RPlayer CurrentPlayer
        {
            get
            {
                if (m_RewiredPlayer == null)
                {
                    m_RewiredPlayer = Rewired.ReInput.players.GetPlayer(SystemPlayerId);
                }
                return m_RewiredPlayer;
            }
        }

        private static bool IsPlayerValid => CurrentPlayer != null;

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

        public static bool GetButton(Jyx2PlayerAction actionId)
        {
            return GetButton((int)actionId);
        }

        #endregion
        public static RController GetLastActiveController()
        {
            if (!IsPlayerValid)
                return null;
            return CurrentPlayer.controllers.GetLastActiveController();
        }

        public static void OnUpdate()
        {
            if (!IsPlayerValid)
                return;
            var lastController = GetLastActiveController();
            if(m_CurController != lastController)
            {
                m_CurController = lastController;
                OnControllerChange?.Invoke(m_CurController);
            }
        }
    }
}