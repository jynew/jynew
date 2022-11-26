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
    public class ArchiveItem:MonoBehaviour,IDataContainer<int>,INavigable
    {
        private int m_ArchiveIndex = -1;

        [SerializeField]
        private Text m_TitleTxt;

        [SerializeField]
        private Text m_SummaryTxt;

        [SerializeField]
        private Text m_DateTxt;

        [SerializeField]
        private Button m_ItemBtn;

        private Action<int> _OnItemClick;


        void Awake()
        {
            m_ItemBtn.onClick.AddListener(OnItemBtnClick);
        }

        void OnDestroy()
        {
            m_ItemBtn.onClick.RemoveListener(OnItemBtnClick);
            _OnItemClick = null;
        }

        public void SetData(int idx)
        {
            m_ArchiveIndex = idx;
            RefreshView();
        }

        public void SetClickCallBack(Action<int> callback)
        {
            _OnItemClick = callback;
        }

        void RefreshView()
        {
            if (m_ArchiveIndex == -1)
                return;

            string archiveIdxStr = GameConst.GetUPNumber(m_ArchiveIndex + 1).GetContent(nameof(SavePanel));
            string archiveName = "存档".GetContent(nameof(SavePanel));
            m_TitleTxt.text = archiveName + archiveIdxStr;

            var summaryText = GameSaveSummary.Load(m_ArchiveIndex).GetBrief();
            summaryText = string.IsNullOrEmpty(summaryText) ? "空档位".GetContent(nameof(SavePanel)) : summaryText;
            m_SummaryTxt.text = summaryText;

            var dateText = GameRuntimeData.GetSaveDate(m_ArchiveIndex)?.ToLocalTime().ToString("yyyy年M月d日 H时m分");
            m_DateTxt.text = dateText;
        }

        void OnItemBtnClick()
        {
            _OnItemClick?.Invoke(m_ArchiveIndex);
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
            if(notifyEvent)
            {
                m_ItemBtn.onClick?.Invoke();
            }
            EventSystem.current.SetSelectedGameObject(m_ItemBtn.gameObject);
        }
    }
}
