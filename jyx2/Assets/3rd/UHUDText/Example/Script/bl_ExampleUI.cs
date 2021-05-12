using UnityEngine;
using UnityEngine.UI;

namespace HUDText
{
    public class bl_ExampleUI : MonoBehaviour
    {

        public bl_HUDText HUD;
        [Space(5)]
        public Text FadeSpeedText;
        public Text FloatingSpeedText;
        public Text Factor;
        public Text DelayText;
        public Text MaxUsesText;
        public GameObject CanReuseGo;

        public void FadeSpeed(float f)
        {
            HUD.FadeSpeed = f;
            FadeSpeedText.text = f.ToString("00") + "%";
        }
        public void FloatingSpeed(float f)
        {
            HUD.FloatingSpeed = f;
            FloatingSpeedText.text = f.ToString("00") + "%";
        }
        public void FactorMultipler(float f)
        {
            HUD.FactorMultiplier = f;
            Factor.text = (f * 100).ToString("00") + "%";
        }
        public void Delay(float f)
        {
            HUD.DelayStay = f;
            DelayText.text = (f * 20).ToString("00") + "%";
        }
        public void CanReuse(bool b)
        {
            HUD.CanReuse = b;
            CanReuseGo.SetActive(b);
        }
        public void MaxReuses(float m)
        {
            HUD.MaxUses = (int)m;
            MaxUsesText.text = m.ToString();
        }
    }
}