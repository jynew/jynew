using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeautifyEffect
{
    public class Demo2 : MonoBehaviour
    {
        int demoMode = 0;

        void Start()
        {
            UpdateDemoMode();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                demoMode++;
                if (demoMode >= 11)
                    demoMode = 0;
                UpdateDemoMode();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (demoMode > 0)
                    demoMode = 0;
                else
                    demoMode = 1;
                UpdateDemoMode();
            }
        }

        void UpdateDemoMode()
        {
            string desc = "";
            Beautify.instance.enabled = demoMode > 0;

            switch (demoMode)
            {
                case 0:
                    desc = "BEAUTIFY OFF (click to enable)";
                    break;
                case 1:
                    desc = "BEAUTIFY ON";
                    Beautify.instance.lut = false;
                    Beautify.instance.outline = false;
                    Beautify.instance.nightVision = false;
                    Beautify.instance.bloom = false;
                    Beautify.instance.anamorphicFlares = false;
                    Beautify.instance.lensDirt = false;
                    Beautify.instance.vignetting = false;
                    Beautify.instance.frame = false;
                    Beautify.instance.sunFlares = false;
                    break;
                case 2:
                    desc = "BEAUTIFY ON + vignetting";
                    Beautify.instance.vignetting = true;
                    Beautify.instance.vignettingColor = new Color(0, 0, 0, 0.05f);
                    break;
                case 3:
                    desc = "BEAUTIFY ON + vignetting + bloom";
                    Beautify.instance.bloom = true;
                    break;
                case 4:
                    desc = "BEAUTIFY ON + vignetting + sun flares";
                    Beautify.instance.sunFlares = true;
                    break;
                case 5:
                    desc = "BEAUTIFY ON + vignetting + bloom + lens dirt";
                    Beautify.instance.lensDirt = true;
                    break;
                case 6:
                    desc = "BEAUTIFY ON + vignetting + lens dirt + anamorphic flares";
                    Beautify.instance.bloom = false;
                    Beautify.instance.anamorphicFlares = true;
                    break;
                case 7:
                    desc = "BEAUTIFY ON + vignetting + lens dirt + vertical anamorphic flares";
                    Beautify.instance.anamorphicFlaresVertical = true;
                    break;
                case 8:
                    desc = "BEAUTIFY ON + vignetting + bloom + lens dirt + night vision";
                    Beautify.instance.bloom = true;
                    Beautify.instance.anamorphicFlares = false;
                    Beautify.instance.nightVision = true;
                    break;
                case 9:
                    desc = "BEAUTIFY ON + red vignetting + bloom + lens dirt + thermal vision";
                    Beautify.instance.thermalVision = true;
                    break;
                case 10:
                    desc = "BEAUTIFY ON + LUT sepia + outline + frame";
                    Beautify.instance.thermalVision = false;
                    Beautify.instance.vignetting = false;
                    Beautify.instance.lut = true;
                    Beautify.instance.outline = true;
                    Beautify.instance.bloom = false;
                    Beautify.instance.anamorphicFlares = false;
                    Beautify.instance.anamorphicFlaresVertical = false;
                    Beautify.instance.lensDirt = false;
                    Beautify.instance.frame = true;
                    break;
            }
            GameObject.Find("Beautify").GetComponent<Text>().text = desc;
        }
    }

}