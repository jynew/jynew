using UnityEngine;
using UnityEngine.UI;

namespace TapSDK.UI
{
    public class ToastPanelOpenParam : IOpenPanelParameter
    {
        public float popupTime;

        public string text;

        public ToastPanelOpenParam(string text, float popupTime)
        {
            this.text = text;
            this.popupTime = popupTime;
        }
    }
    public class ToastPanelController : BasePanelController
    {
        public Text text;

        public RectTransform background;
        
        public float fixVal;

        public string show;

        protected override void BindComponents()
        {
            base.BindComponents();
            text = transform.Find("Root/Text").GetComponent<Text>();
            background = transform.Find("Root/BGM") as RectTransform;
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();
            ToastPanelOpenParam param = this.openParam as ToastPanelOpenParam;
            if (param != null)
            {
                text.text = param.text;
                var totalLength = CalculateLengthOfText();
                var x = totalLength;
                var y = background.sizeDelta.y;
                background.sizeDelta = new Vector2(x, y);
                this.Invoke("Close", param.popupTime);
            }
        }

        private float CalculateLengthOfText()
        {
            var width = text.preferredWidth + fixVal;
            width = Mathf.Max(200, width);
            width = Mathf.Min(Screen.width, width);
            return width;
        }
    }
}
