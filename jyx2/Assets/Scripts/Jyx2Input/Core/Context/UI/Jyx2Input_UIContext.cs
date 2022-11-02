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

        protected bool NoValidSelect
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
        
        
        public void TrySelectMyUIObject()
        {
            if(m_LastSelect != null && CurrentSelect != m_LastSelect)
            {
                CurrentSelect = m_LastSelect;
                return;
            }
            if (m_FirstSelect != null && CurrentSelect != m_FirstSelect)
                CurrentSelect = m_FirstSelect;
        }

        public void TryStoreLastSelect()
        {
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
