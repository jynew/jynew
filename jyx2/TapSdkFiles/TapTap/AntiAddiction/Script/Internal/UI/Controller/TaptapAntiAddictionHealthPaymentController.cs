using TapSDK.UI;
using TapTap.AntiAddiction.Model;
using UnityEngine.UI;
using System;

namespace TapTap.AntiAddiction.Internal 
{
    public class TaptapAntiAddictionHealthPaymentController : BasePanelController
    {
        public Text titleText;
        public Text contentText;
        public Text buttonText;
        public Button okButton;

        private Action _onOk;

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            titleText = transform.Find("Root/TitleText").GetComponent<Text>();
            contentText = transform.Find("Root/ContentText").GetComponent<Text>();
            okButton = transform.Find("Root/OKButton").GetComponent<Button>();
            buttonText = okButton.transform.Find("Text").GetComponent<Text>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            okButton.onClick.AddListener(OnOKButtonClicked);
        }

        internal void Show(PayableResult payable)
        {
            titleText.text = payable.Title;
            contentText.text = ProcessContent(payable.Content);
            var buttonText = Config.healthPayTipButtonText;
            if (!string.IsNullOrEmpty(buttonText))
                this.buttonText.text = buttonText;
        }

        internal void Show(string title, string content, string buttonText, Action onOk = null)
        {
            titleText.text = title;
            contentText.text = ProcessContent(content);
            if (!string.IsNullOrEmpty(buttonText))
                this.buttonText.text = buttonText;
            _onOk = onOk;
        }

        private string ProcessContent(string content)
        {
            return content
                ?.Replace("<font color=", "<color=")
                .Replace("<span color=", "<color=")
                .Replace("</font>", "</color>")
                .Replace("</span>", "</color>")
                .Replace("<br>", "\n");
        }

        private void OnOKButtonClicked()
        {
            _onOk?.Invoke();
            Close();
        }
    }
}