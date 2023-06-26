using Jyx2;
using Jyx2.InputCore.UI;
using Jyx2.Util;
using Rewired;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2.InputCore
{
#if DEVELOP_TAPTAP
    /// 导入3.16.5版本的TapSDK时，由于TapSDK中定义了Singleton，这里的Singleton就需要指定Jyx2.Util
    public class InputContextManager : Jyx2.Util.Singleton<InputContextManager>
#else
    public class InputContextManager : Singleton<InputContextManager>
#endif
    {
        private List<IJyx2_InputContext> m_Contexts = new List<IJyx2_InputContext>();

        public bool IsPlayerContext => CurrentContext is Jyx2_PlayerInput;

        public IJyx2_InputContext CurrentContext
        {
            get
            {
                if (m_Contexts.Count > 0)
                    return m_Contexts[m_Contexts.Count - 1];
                return null;
            }
        }

        private bool IsLoading => Jyx2_UIManager.Instance.IsUIOpen(nameof(LoadingPanel));

        public override void Init()
        {
            base.Init();
            Jyx2_Input.OnControllerChange += OnControllerChange;
        }

        public override void DeInit()
        {
            base.DeInit();
            m_Contexts.Clear();
            Jyx2_Input.OnControllerChange -= OnControllerChange;
        }

        private void OnControllerChange(Controller currentController)
        {
            if (currentController == null)
                return;
            if(currentController.type == ControllerType.Mouse)
            {
                return;
            }
            if (CurrentContext is Jyx2Input_UIContext uiContext && uiContext.NoValidSelect)
            {
                uiContext.TrySelectMyUIObject();
            }
        }


        public void AddInputContext(IJyx2_InputContext newContext, bool insertToHead = false)
        {
            if (newContext == null)
                return;
            if (m_Contexts.Contains(newContext))
            {
                Debug.LogError("Try add duplicate context:" + newContext);
                return;
            }

            TryStoreLastSelectObject(CurrentContext);

            var prevContext = CurrentContext;

            if(insertToHead)
            {
                m_Contexts.Insert(0, newContext);
            }
            else
            {
                m_Contexts.Add(newContext);
            }
            
            if (newContext is Jyx2Input_UIContext uiContext)
            {
                uiContext.TrySelectMyUIObject();
            }
            if(prevContext is Jyx2_PlayerInput)
            {
                Jyx2_Input.FirePlayerInputChangeEvent(false);
            }
        }

        public void RemoveInputContext(IJyx2_InputContext inputContext)
        {
            if (inputContext == null)
                return;

            TryStoreLastSelectObject(inputContext);

            m_Contexts.Remove(inputContext);

            if (CurrentContext is Jyx2_PlayerInput)
            {
                Jyx2_Input.FirePlayerInputChangeEvent(true);
            }

            TrySelectCurrentContextUIObject();
        }
        
        private void TryStoreLastSelectObject(IJyx2_InputContext contextToStore)
        {
            if (CurrentContext == contextToStore && contextToStore is Jyx2Input_UIContext uiContext)
            {
                uiContext.TryStoreLastSelect();
            }
        }
        

        private void TrySelectCurrentContextUIObject()
        {
            if (m_Contexts.Count > 0 && CurrentContext is Jyx2Input_UIContext newUIContext)
            {
                newUIContext.TrySelectMyUIObject();
            }
        }


        public void Update()
        {
            if (IsLoading)
                return;
            Jyx2_Input.OnUpdate();
            if (CurrentContext != null && CurrentContext.CanUpdate)
            {
                CurrentContext.OnUpdate();
            }
        }
    }
}
