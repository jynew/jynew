using i18n.TranslatorDef;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Jyx2.InputCore
{
    public class ControllerButtonNotice:MonoBehaviour
    {
        [SerializeField]
        private Jyx2PlayerAction m_Action;

        [SerializeField]
        private AxisRange m_AxisRange;

        [SerializeField]
        private TextMeshProUGUI m_ActionTxt;

        [SerializeField]
        private string m_FormatText = string.Empty;

        private void OnEnable()
        {
            Jyx2_Input.OnControllerChange += OnControllerChange;
            RefreshActionText();
        }

        private void OnDisable()
        {
            Jyx2_Input.OnControllerChange -= OnControllerChange;
        }

        private void OnControllerChange(Controller newController)
        {
            RefreshActionText();
        }

        private void ClearActionText()
        {
            m_ActionTxt.text = string.Empty;
        }

        private void RefreshActionText()
        {
            var lastController = Jyx2_Input.GetLastActiveController();
            if (lastController == null || lastController.type == ControllerType.Mouse)
            {
                ClearActionText();
                return;
            }
            var buttonName = Jyx2_Input.GetActionButtonName(m_Action, m_AxisRange);
            if (string.IsNullOrEmpty(m_FormatText))
                m_ActionTxt.text = Jyx2_Input.GetActionButtonName(m_Action, m_AxisRange);
            else
                m_ActionTxt.text = string.Format(m_FormatText.GetContent("ControllerNotice"), buttonName);
        }
    }
}
