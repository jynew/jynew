using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class ButtonClickEvent : UnityEvent
    {
        public ButtonClickEvent() { }
    }

    public interface ISelectButton
    {
        public ButtonClickEvent onClick { get;}

        public void OnClickSelect();

        public void OnClickDeselect();
    }


    public class SelectButton : Selectable, ISelectButton,IPointerClickHandler
    {
        [SerializeField]
        private ButtonClickEvent m_ClickEvent = new ButtonClickEvent();

        [SerializeField]
        private Graphic m_SelectedGraphic;

        [SerializeField]
        private bool m_Selected;

        public ButtonClickEvent onClick => m_ClickEvent;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateGraphic(true);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateGraphic(true);
        }


        public virtual void OnClickSelect()
        {
            m_Selected = true;
            UpdateGraphic(true);
        }

        public virtual void OnClickDeselect()
        {
            m_Selected = false;
            UpdateGraphic(true);
        }

        private void UpdateGraphic(bool instant)
        {
            if (m_SelectedGraphic == null)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_SelectedGraphic.canvasRenderer.SetAlpha(m_Selected ? 1f : 0f);
            else
#endif
                m_SelectedGraphic.CrossFadeAlpha(m_Selected ? 1f : 0f, instant ? 0f : 0.1f, true);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
