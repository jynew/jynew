using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class SelectButtonGroup : UIBehaviour
    {
        [SerializeField]
        private List<SelectButton> m_Btns = new List<SelectButton>();

        [SerializeField]
        private SelectButton m_CurSelect;

        private Dictionary<SelectButton, UnityAction> m_BtnCallBacks = new Dictionary<SelectButton, UnityAction>();

        public int CurButtonIndex
        {
            get
            {
                if (m_CurSelect == null)
                    return -1;
                return m_Btns.IndexOf(m_CurSelect);
            }
        }

        public List<SelectButton> Buttons
        {
            get
            {
                return m_Btns;
            }
        }


        public class OnButtonSelectEvent : UnityEvent<int> { }

        public OnButtonSelectEvent OnButtonSelect = new OnButtonSelectEvent();


        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < m_Btns.Count; ++i)
            {
                AddButtonSelectListener(m_Btns[i]);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for(int i = 0;i < m_Btns.Count; ++i)
            {
                RemoveButtonSelectListener(m_Btns[i]);
            }
        }

        public void ClearAllButtonns(bool destroy = true)
        {
            for(int i = m_Btns.Count - 1; i >= 0; --i)
            {
                RemoveButtonSelectListener(m_Btns[i]);
                if(destroy)
                {
                    Destroy(m_Btns[i].gameObject);
                }
            }
            m_Btns.Clear();
            m_CurSelect = null;
        }


        public void RegisterButton(SelectButton btn)
        {
            if (btn == null)
                return;
            AddButtonSelectListener(btn);
            if(!m_Btns.Contains(btn))
                m_Btns.Add(btn);
        }

        public void UnRegisterButton(SelectButton btn)
        {
            RemoveButtonSelectListener(btn);
            m_Btns.Remove(btn);
        }



        private void AddButtonSelectListener(SelectButton btn)
        {
            if (!m_BtnCallBacks.ContainsKey(btn))
            {
                UnityAction callback = () => { SelectButton(btn); };
                btn.onClick.AddListener(callback);
                m_BtnCallBacks.Add(btn, callback);
            }
        }


        private void RemoveButtonSelectListener(SelectButton btn)
        {
            if (m_BtnCallBacks.ContainsKey(btn))
            {
                var callback = m_BtnCallBacks[btn];
                btn.onClick.RemoveListener(callback);
                m_BtnCallBacks.Remove(btn);
            }
            if(m_CurSelect == btn)
            {
                m_CurSelect = null;
            }
        }

        public void SelectIndex(int idx)
        {
            if (idx < 0 || idx >= m_Btns.Count)
                return;
            SelectButton(m_Btns[idx], true);
        }

        public void ClearSelect()
        {
            if(m_CurSelect != null)
            {
                m_CurSelect.OnClickDeselect();
                m_CurSelect = null;
            }
        }


        private void SelectButton(SelectButton targetSelect, bool forceSelect = false)
        {
            var curIdx = m_CurSelect != null ? m_Btns.IndexOf(m_CurSelect) : -1;
            var idx = m_Btns.IndexOf(targetSelect);
            if(curIdx != idx || forceSelect)
            {
                m_CurSelect?.OnClickDeselect();
                targetSelect.OnClickSelect();
                m_CurSelect = targetSelect;
                OnButtonSelect?.Invoke(idx);
            }
        }
    }
}