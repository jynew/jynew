using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeautifyEffect
{
    public class Demo1 : MonoBehaviour
    {
        float deltaTime = 0.0f;
        bool benchmarkEnabled;
        GUIStyle style;
        Rect rect;


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(0))
            {
                Beautify.instance.enabled = !Beautify.instance.enabled;
                UpdateText();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                benchmarkEnabled = !benchmarkEnabled;
            }
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            if (Input.GetKeyDown(KeyCode.B)) Beautify.instance.Blink(0.1f);
        }

        void UpdateText()
        {
            if (Beautify.instance.enabled)
            {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify ON";
            }
            else
            {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify OFF";
            }
        }

        void OnGUI()
        {
            if (!benchmarkEnabled)
                return;

            int w = Screen.width, h = Screen.height;
            if (style == null)
            {
                style = new GUIStyle();
                rect = new Rect(0, 0, w, h * 4 / 100);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = h * 4 / 100;
                style.normal.textColor = Color.white;
            }
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }


    }
}
