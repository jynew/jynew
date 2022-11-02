using Jyx2;
using Jyx2.Util;
using Rewired;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2.InputCore
{
    public class InputContextManager : Singleton<InputContextManager>
    {
        private List<IJyx2_InputContext> m_Contexts = new List<IJyx2_InputContext>();
        

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


        public override void DeInit()
        {
            base.DeInit();
            m_Contexts.Clear();
        }

        public override void Init()
        {
            base.Init();
        }

        public void AddInputContext(IJyx2_InputContext inputContext)
        {
            if (inputContext == null)
                return;
            if (m_Contexts.Contains(inputContext))
            {
                Debug.LogError("Try add duplicate context:" + inputContext);
                return;
            }
            m_Contexts.Add(inputContext);
        }

        public void RemoveInputContext(IJyx2_InputContext inputContext)
        {
            if (inputContext == null)
                return;
            m_Contexts.Remove(inputContext);
        }


        public void Update()
        {
            if (IsLoading)
                return;
            Jyx2_Input.OnUpdate();
            CurrentContext?.OnUpdate();
        }
    }
}
