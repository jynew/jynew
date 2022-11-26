using i18n.TranslatorDef;
using Jyx2.UINavigation;
using Jyx2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jyx2
{
    public class ChatUISelectionItem : MonoBehaviour, IDataContainer<string>, INavigable
    {
        [SerializeField]
        private Text m_SelectionContent;

        [SerializeField]
        private Button m_ItemBtn;

        private Action<int> _OnItemClick;

        private int m_Index = -1;


        void Awake()
        {
            m_ItemBtn.onClick.AddListener(OnItemBtnClick);
        }

        void OnDestroy()
        {
            m_ItemBtn.onClick.RemoveListener(OnItemBtnClick);
            _OnItemClick = null;
        }

        public void SetData(string text)
        {
            m_SelectionContent.text = text;
        }

        public void SetIndex(int idx)
        {
            m_Index = idx;
        }

        public void SetClickCallBack(Action<int> callback)
        {
            _OnItemClick = callback;
        }

        void OnItemBtnClick()
        {
            _OnItemClick?.Invoke(m_Index);
        }

        public void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null)
        {
            if (m_ItemBtn == null)
                return;
            Navigation navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = up?.GetSelectable();
            navigation.selectOnDown = down?.GetSelectable();
            navigation.selectOnLeft = left?.GetSelectable();
            navigation.selectOnRight = right?.GetSelectable();
            m_ItemBtn.navigation = navigation;
        }

        public Selectable GetSelectable()
        {
            return m_ItemBtn;
        }

        public void Select(bool notifyEvent = false)
        {
            m_ItemBtn.onClick?.Invoke();
        }
    }
}
