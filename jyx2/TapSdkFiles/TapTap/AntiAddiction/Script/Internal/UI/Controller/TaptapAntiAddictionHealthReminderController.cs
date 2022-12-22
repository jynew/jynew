using System;
using UnityEngine.UI;
using TapSDK.UI;

namespace TapTap.AntiAddiction.Internal {
    public class TaptapAntiAddictionHealthReminderController : BasePanelController
    {
        public Text titleText;
        public Text contentText;
        public Button switchAccountButton;
        public Button okButton;

        private Action OnOk { get; set; }
        private Action OnSwitchAccount { get; set; }

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            titleText = transform.Find("Root/TitleText").GetComponent<Text>();
            contentText = transform.Find("Root/ContentText").GetComponent<Text>();
            switchAccountButton = transform.Find("Root/SwitchAccountButton").GetComponent<Button>();
            okButton = transform.Find("Root/OKButton").GetComponent<Button>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            switchAccountButton.onClick.AddListener(OnSwitchAccountButtonClicked);
            okButton.onClick.AddListener(OnOKButtonClicked);
        }

        internal void Show(PlayableResult playable, Action onOk, Action onSwitchAccount)
        {
            OnOk = onOk;
            OnSwitchAccount = onSwitchAccount;
            titleText.text = playable.Title;
            int remainTime = Math.Max(0, playable.RemainTime / 60);
            remainTime = Math.Min(PlayableResult.MaxRemainTime, remainTime);
            // 替换富文本标签
            contentText.text = playable.Content
                ?.Replace("<font color=", "<color=")
                .Replace("</font>", "</color>")
                .Replace("<span color=", "<color=")
                .Replace("</span>", "</color>")
                .Replace("<br>", "\n")
                // 设置剩余时间
                .Replace("# ${remaining} #", remainTime.ToString());

            switchAccountButton.gameObject.SetActive(onSwitchAccount != null);

            var buttonText = okButton.transform.Find("Text").GetComponent<Text>();
            var switchButtonText = switchAccountButton.transform.Find("Text").GetComponent<Text>();
            if (TapTapAntiAddictionManager.AntiAddictionConfig.region == Region.Vietnam)
            {
                if (onOk == null && onSwitchAccount != null)
                {
                    buttonText.text =
                        Config.Current.UIConfig.HealthReminderVietnam.buttonSwitch;
                    OnOk = onSwitchAccount;
                    OnSwitchAccount = null;
                    switchAccountButton.gameObject.SetActive(false);
                }
                else
                {
                    buttonText.text =
                        Config.Current.UIConfig.HealthReminderVietnam.buttonExit;
                    switchButtonText.text =
                        Config.Current.UIConfig.HealthReminderVietnam.buttonSwitch;
                }
            }
            else
            {
                buttonText.text = playable.CanPlay
                    ? TapTapAntiAddictionManager.LocalizationItems.Current.EnterGame
                    : TapTapAntiAddictionManager.LocalizationItems.Current.ExitGame;
            }

        }

        private void OnOKButtonClicked()
        {
            OnOk?.Invoke();
            Close();
        }

        private void OnSwitchAccountButtonClicked()
        {
            OnSwitchAccount?.Invoke();
            Close();
        }
    }
}