using Sirenix.OdinInspector;
using UnityEngine;

namespace Jyx2.InputCore.UI
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class Jyx2Input_UIContext: Jyx2InputContextBase
    {
        [SerializeField]
        private GameObject m_FirstSelect;

        [SerializeField]
        private GameObject m_LastSelect;

        [SerializeField]
        [LabelText("手动控制激活时的UI对象选择")]
        private bool m_ManualControlSelect = false;

        private int m_EnableFrame = int.MaxValue;
        private bool IsValidFrame => m_EnableFrame <= Time.frameCount;

        public override bool CanUpdate => IsValidFrame;

        protected override void OnEnable()
        {
            base.OnEnable();
            //避免相同按键在同一帧多次响应多个UIContext
            m_EnableFrame = Time.frameCount + 1;
        }

        public bool NoValidSelect
        {
            get
            {
                if (CurrentSelect == null)
                    return true;
                if (!CurrentSelect.activeInHierarchy)
                    return true;
                if (!CurrentSelect.transform.IsChildOf(transform))
                    return true;
                return false;
            }
        }

        public bool IsFirstSelectValid => m_FirstSelect != null && m_FirstSelect.activeInHierarchy;

        public bool IsLastSelectValid => m_LastSelect != null && m_LastSelect.activeInHierarchy;
        
        
        public void TrySelectMyUIObject(bool forceSelect = false)
        {
            if (m_ManualControlSelect && !forceSelect)
                return;
            if (IsLastSelectValid && CurrentSelect != m_LastSelect)
            {
                CurrentSelect = m_LastSelect;
                return;
            }
            if (IsFirstSelectValid && CurrentSelect != m_FirstSelect)
                CurrentSelect = m_FirstSelect;
        }

        public void TryStoreLastSelect()
        {
            if (m_ManualControlSelect)
                return;
            var selectObject = CurrentSelect;
            if (selectObject == null)
            {
                m_LastSelect = null;
                return;
            }
            if (selectObject.transform.IsChildOf(transform))
            {
                m_LastSelect = selectObject;
            }
        }
    }
}
