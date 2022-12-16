using System;
using System.Text.RegularExpressions;
using LC.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TapSDK.UI;
using TapTap.AntiAddiction.Model;

namespace TapTap.AntiAddiction.Internal {
    public class TaptapAntiAddictionIDInputController : BasePanelController
    {
        public Button closeButton;
        public Button submitButton;

        public InputField nameInputField;
        public InputField idNumberInputField;

        public Text titleText;
        public Text descriptionText;
        public Text buttonText;

        public GameObject errorNode;

        public Text errorTipText;

        internal Action<VerificationResult> OnVerified;
        internal Action<Exception> OnException;
        internal Action OnClosed;


        private bool _isSending;

        private bool isSending
        {
            get => _isSending;
            set
            {
                if (value != _isSending)
                {
                    _isSending = value;
                    if (_isSending)
                        UIManager.Instance.OpenLoading();
                    else
                        UIManager.Instance.CloseLoading();
                }
            }
        }

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
            submitButton = transform.Find("Root/SubmitButton").GetComponent<Button>();

            titleText = transform.Find("Root/TitleText").GetComponent<Text>();
            descriptionText = transform.Find("Root/ContentText").GetComponent<Text>();
            buttonText = submitButton.transform.Find("Text").GetComponent<Text>();

            nameInputField = transform.Find("Root/NameInput").GetComponent<InputField>();
            idNumberInputField = transform.Find("Root/IDNumInput").GetComponent<InputField>();

            errorNode = transform.Find("Root/Error").gameObject;
            errorTipText = errorNode.transform.Find("Text").GetComponent<Text>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            submitButton.onClick.AddListener(OnConfirmButtonClicked);

            var config = Config.GetInputIdentifyTip();
            if (config != null)
            {
                titleText.text = config.Title;
                descriptionText.text = config.Content;
                buttonText.text = config.PositiveButtonText;
            }

            isSending = false;

            errorNode.gameObject.SetActive(false);
        }

        private bool Validate(out string name, out string idNumber, out string errorTip)
        {
            errorTip = null;
            name = nameInputField.text;
            idNumber = idNumberInputField.text;
            if (string.IsNullOrWhiteSpace(name))
            {
                errorTip = "请输入真实姓名";
                return false;
            }

            if (!IsIdNum(idNumber))
            {
                errorTip = "请输入真实的身份证号码";
                return false;
            }

            return true;
        }

        private async void OnConfirmButtonClicked()
        {
            if (isSending) return;
            string errorTip;
            string name;
            string idNumber;
            var validation = Validate(out name, out idNumber, out errorTip);
            if (validation)
            {
                var dateJson = new { name = name, idCard = idNumber };
                var json = JsonConvert.SerializeObject(dateJson, Formatting.Indented);
                try
                {
                    isSending = true;
                    var verificationResult = await Verification.VerifyKycAsync(TapTapAntiAddictionManager.UserId, json);
                    isSending = false;
                    OnVerified?.Invoke(verificationResult);
                    // 2-认证失败
                    if (verificationResult.Status != 2)
                        Close();
                }
                catch (Exception e)
                {
                    isSending = false;
                    OnException?.Invoke(e);
                }
            }
            else
            {
                ShowErrorTip(errorTip);
            }
        }


        private void OnCloseButtonClicked()
        {
            OnClosed?.Invoke();
            Close();
        }

        /// <summary>
        /// 验证身份证号(https://cloud.tencent.com/developer/article/1860685)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsIdNum(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
//             double iSum = 0;
//             // 18位验证
//             Regex rg = new Regex(@"^\d{17}(\d|x)$");
//             Match mc = rg.Match(input);
//             if (!mc.Success)
//             {
//                 return false;
//             }
//
//             // 生日验证
//             input = input.ToLower();
//             input = input.Replace("x", "a");
//             try
//             {
//                 var year = input.Substring(6, 4);
//                 var month = input.Substring(10, 2);
//                 var day = input.Substring(12, 2);
//                 DateTime.Parse(year + "-" + month + "-" + day);
//             }
//             catch
//             {
// #if UNITY_EDITOR
//                 Debug.LogErrorFormat("国内-防沉迷 身份证号非法出生日期");
// #endif
//                 return false;
//             }
//
//             // 最后一位验证
//             for (int i = 17; i >= 0; i--)
//             {
//                 iSum += (Math.Pow(2, i) % 11) *
//                         int.Parse(input[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);
//             }
//
//             if (iSum % 11 != 1)
//             {
// #if UNITY_EDITOR
//                 Debug.LogErrorFormat("国内-防沉迷 身份证号非法尾号");
// #endif
//                 return false;
//             }
//
//             return true;
        }

        public void ShowErrorTip(string content)
        {
            errorNode.gameObject.SetActive(true);
            errorTipText.text = content;
        }
    }
}