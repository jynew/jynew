using UnityEngine;


namespace HUDText
{
    public class bl_TestMouseEvent : MonoBehaviour
    {
        private bl_HUDText HUDRoot;
        [SerializeField] private GameObject TextPrefab;
        public GameObject m_Particle;
        public ExampleType m_Type;

        private string[] Text = new string[] { "Floating Text", "Awasome", "But you say", "Nice", "Beatiful", "Surprising", "Impossible", "This is a big text for example purpose"
    ,"\n Add extra line","\n Add other line"};
        private string[] InfoText = new string[] { "Info Text", "Info Text Here", "Create a dialogue", };

        void Awake()
        {
            HUDRoot = bl_UHTUtils.GetHUDText;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMouseDown()
        {
            switch (m_Type)
            {
                case ExampleType.Friend:
                    HUDTextInfo info = new HUDTextInfo(transform, string.Format("+{0}", Random.Range(5, 20).ToString()));
                    info.Size = Random.Range(10, 15);
                    info.Color = Color.green;
                    info.VerticalPositionOffset = 3;
                    HUDRoot.NewText(info);
                    break;
                case ExampleType.Enemy:
                    //Build the information
                    HUDTextInfo info2 = new HUDTextInfo(transform, "- " + Random.Range(50, 100));
                    info2.Color = Color.red;
                    info2.Size = Random.Range(6, 15);
                    info2.Speed = Random.Range(10, 20);
                    info2.VerticalAceleration = -1;
                    info2.VerticalFactorScale = Random.Range(1.2f, 3);
                    info2.VerticalPositionOffset = 3;
                    info2.Side = (Random.Range(0, 2) == 1) ? bl_Guidance.RightDown : bl_Guidance.LeftDown;
                    //Send the information
                    HUDRoot.NewText(info2);
                    break;
                case ExampleType.Neutral:
                    //Build the information
                    string t = Text[Random.Range(0, Text.Length)];
                    HUDTextInfo info3 = new HUDTextInfo(transform, t);
                    info3.Color = Color.white;
                    info3.Size = Random.Range(10, 13);
                    info3.Speed = Random.Range(10, 20);
                    info3.VerticalAceleration = 1;
                    info3.VerticalFactorScale = Random.Range(1.2f, 3);
                    info3.Side = bl_Guidance.Up;
                    info3.VerticalPositionOffset = 3;
                    info3.AnimationSpeed = 0.5f;
                    info3.ExtraDelayTime = 2;
                    info3.FadeSpeed = 400;
                    //Send the information
                    HUDRoot.NewText(info3);
                    break;
                case ExampleType.Info:
                    //Build the information
                    HUDTextInfo info4 = new HUDTextInfo(transform, InfoText[Random.Range(0, InfoText.Length)]);
                    info4.Color = Color.white;
                    info4.Size = Random.Range(5, 12);
                    info4.Speed = Random.Range(10, 20);
                    info4.VerticalAceleration = 1;
                    info4.VerticalPositionOffset = 5;
                    info4.VerticalFactorScale = Random.Range(1.2f, 3);
                    info4.Side = bl_Guidance.Right;
                    info4.TextPrefab = TextPrefab;
                    info4.FadeSpeed = 500;
                    info4.ExtraDelayTime = 5;
                    info4.AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall;
                    //Send the information
                    HUDRoot.NewText(info4);
                    break;
                case ExampleType.NeutralText:
                    //Build the information
                    HUDTextInfo info5 = new HUDTextInfo(transform, string.Format("Text: {0}", Random.Range(2, 20).ToString()));
                    info5.Color = Color.white;
                    info5.Size = Random.Range(10, 15);
                    info5.Speed = Random.Range(5, 14);
                    info5.VerticalAceleration = 0.5f;
                    info5.VerticalPositionOffset = 3.5f;
                    info5.VerticalFactorScale = Random.Range(1.2f, 3);
                    info5.Side = bl_Guidance.Up;
                    info5.ExtraDelayTime = 0.5f;
                    info5.AnimationType = bl_HUDText.TextAnimationType.HorizontalSmall;
                    //Send the information
                    HUDRoot.NewText(info5);
                    break;
                case ExampleType.Points:
                    //Build the information
                    HUDTextInfo info6 = new HUDTextInfo(transform, string.Format("Points: {0}", Random.Range(2, 20).ToString()));
                    info6.Color = Color.white;
                    info6.Size = 4;
                    info6.Speed = Random.Range(0.2f, 1);
                    info6.VerticalAceleration = 0.2f;
                    info6.VerticalPositionOffset = -4f;
                    info6.VerticalFactorScale = Random.Range(1.2f, 20);
                    info6.Side = bl_Guidance.Down;
                    info6.ExtraDelayTime = 0.1f;
                    info6.AnimationType = bl_HUDText.TextAnimationType.SmallToNormal;
                    info6.FadeSpeed = 300;
                    //Send the information
                    HUDRoot.NewText(info6);
                    break;
                case ExampleType.Random:
                    //Build the information
                    string sub = (Random.Range(0, 2) == 1) ? "-" : "+";
                    HUDTextInfo info7 = new HUDTextInfo(transform, string.Format("{1}{0}", Random.Range(2, 20).ToString(), sub));
                    info7.Color = (Random.Range(0, 2) == 1) ? Color.red : Color.green;
                    info7.Size = Random.Range(1, 12);
                    info7.Speed = Random.Range(0.2f, 1);
                    info7.VerticalAceleration = Random.Range(-2, 2f);
                    info7.VerticalPositionOffset = 2f;
                    info7.VerticalFactorScale = Random.Range(1.2f, 10);
                    info7.Side = (Random.Range(0, 2) == 1) ? bl_Guidance.LeftDown : bl_Guidance.RightDown;
                    info7.ExtraDelayTime = -1;
                    info7.AnimationType = bl_HUDText.TextAnimationType.PingPong;
                    info7.FadeSpeed = 200;
                    info7.ExtraFloatSpeed = -11;
                    //Send the information
                    HUDRoot.NewText(info7);
                    break;
                default:
                    Debug.Log("Unknow type");
                    break;
            }


            if (m_Particle != null)
            {
                GameObject g = (GameObject)Instantiate(m_Particle, (this.transform.position + Vector3.up), this.transform.rotation);
                Destroy(g, 1.5f);
            }
        }

        [System.Serializable]
        public enum ExampleType
        {
            Enemy,
            Friend,
            Neutral,
            NeutralText,
            Points,
            Info,
            Random,
        }
    }

}