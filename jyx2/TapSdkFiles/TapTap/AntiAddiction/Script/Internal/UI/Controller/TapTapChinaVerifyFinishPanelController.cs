using System;
using System.Collections;
using TapSDK.UI;
using TapTap.AntiAddiction.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TapTap.AntiAddiction.Internal {
    public class TapTapChinaVerifyFinishPanelController : BasePanelController
    {
        public Text titleText;
        public Text contentText;
        public Text closeTipText;
        public Button okButton;
        public Button closeButton;

        private float _closeTime = 5.0f;
        private float _elapse;

        private Action OnOk;

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            titleText = transform.Find("Root/TitleText").GetComponent<Text>();
            contentText = transform.Find("Root/ContentText").GetComponent<Text>();
            closeTipText = transform.Find("Root/CloseTipText").GetComponent<Text>();
            okButton = transform.Find("Root/OKButton").GetComponent<Button>();
            closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            okButton.onClick.AddListener(OnOKButtonClicked);
            closeButton.onClick.AddListener(OnOKButtonClicked);

            _elapse = 0;

            StartCoroutine(Countdown());
        }

        internal void Show(PayableResult payable, Action onOk = null)
        {
            // if (payable != null)
            // {
            //     titleText.text = payable.Title;
            //     contentText.text = payable.Content
            //         ?.Replace("<font color=", "<color=")
            //         .Replace("</font>", "</color>")
            //         .Replace("<br>", "\n");
            // }
            OnOk = onOk;
        }

        private void OnOKButtonClicked()
        {
            OnOk?.Invoke();
            Close();
        }

        IEnumerator Countdown()
        {
            int remainTime = 0;
            do
            {
                remainTime = Mathf.CeilToInt(_closeTime - _elapse);
                closeTipText.text = $"即将关闭当前页面({remainTime}s)";
                yield return new WaitForSeconds(1);
                _elapse++;

            } while (remainTime > 0);

            OnOk?.Invoke();
            Close();
        }
    }
}