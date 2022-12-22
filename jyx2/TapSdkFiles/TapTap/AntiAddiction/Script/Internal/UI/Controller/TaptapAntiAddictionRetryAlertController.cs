using System;
using UnityEngine.UI;
using TapSDK.UI;
using TapTap.AntiAddiction;

namespace TapTap.AntiAddiction.Internal {
    public class TaptapAntiAddictionRetryAlertController : BasePanelController
    {
        public Text messageText;
        public Text buttonText;
        public Button retryButton;

        private Action _onRetry;

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            messageText = transform.Find("Root/BackgroundImage/MessageText").GetComponent<Text>();
            retryButton = transform.Find("Root/BackgroundImage/RetryButton").GetComponent<Button>();
            buttonText = retryButton.transform.Find("Text").GetComponent<Text>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();
            buttonText.text = TapTapAntiAddictionManager.LocalizationItems.Current.Retry;
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void OnRetryClicked()
        {
            _onRetry?.Invoke();
            Close();
        }

        internal void Show(string message, Action onRetry)
        {
            messageText.text = message;
            _onRetry = onRetry;
        }
    }
}