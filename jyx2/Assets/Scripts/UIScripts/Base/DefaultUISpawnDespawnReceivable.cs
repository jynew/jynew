using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HSUI;
using HanSquirrel.ResourceManager;

namespace HSUI
{
    public class DefaultUISpawnDespawnReceivable : BaseUI, ISpawnDespawnReceivable
    {
        static Dictionary<string, List<RectTrsInfo>> initialObjInfoMap = new Dictionary<string, List<RectTrsInfo>>();
        static Dictionary<string, int> initalChildrenCountMap = new Dictionary<string, int>();
        private string type;

        struct RectTrsInfo
        {
            public string objName;
            public bool isActive;
            public Vector3 localPosition;
            public Vector3 localScale;
            public Vector2 anchoredPosition;
            public Vector2 anchoredMin;
            public Vector2 anchoredMax;
            public Vector2 pivot;
            public Vector2 sizeDelta;
            public string textComponentText;
            public Color textComponentColor;
            public int textComponentFontSize;
            public float textComponentFontLineSpacing;
            public Color outlineColor;
            public bool buttonEnabled;

            public RectTrsInfo(string name, bool isActive, Vector3 localPosition, Vector3 localScale, Vector2 anchoredPosition)
            {
                this.objName = name;
                this.isActive = isActive;
                this.localPosition = localPosition;
                this.localScale = localScale;
                this.anchoredPosition = anchoredPosition;
                this.anchoredMin = new Vector2(0.5f, 0.5f);
                this.anchoredMax = new Vector2(0.5f, 0.5f);
                this.pivot = new Vector2(0.5f, 0.5f);
                this.sizeDelta = new Vector2(120, 120);
                this.textComponentText = "default";
                this.textComponentColor = Color.white;
                this.textComponentFontSize = 20;
                this.textComponentFontLineSpacing = 10;
                this.outlineColor = Color.black;
                this.buttonEnabled = false;
            }
        }


        public void InitInfo<T>(T classType)
        {
            type = classType.GetType().ToString();
            if (!initialObjInfoMap.ContainsKey(type))
            {

                var initialObjInfo = new List<RectTrsInfo>();
                Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);
                var initalChildrenCount = allChildren.Length;
                foreach (var child in allChildren)
                {
                    RectTrsInfo info = new RectTrsInfo();
                    RectTransform rect = child.GetComponent<RectTransform>();
                    info.objName = child.name;
                    info.isActive = child.gameObject.activeSelf;
                    info.localPosition = rect.localPosition;
                    info.localScale = rect.localScale;
                    info.anchoredPosition = rect.anchoredPosition;
                    info.anchoredMin = rect.anchorMin;
                    info.anchoredMax = rect.anchorMax;
                    info.sizeDelta = rect.sizeDelta;
                    info.pivot = rect.pivot;
                    if (child.GetComponent<Text>() != null)
                    {
                        info.textComponentText = child.GetComponent<Text>().text;
                        info.textComponentColor = child.GetComponent<Text>().color;
                        info.textComponentFontSize = child.GetComponent<Text>().fontSize;
                        info.textComponentFontLineSpacing = child.GetComponent<Text>().lineSpacing;
                    }
                    else
                    {
                        info.textComponentText = "default";
                        info.textComponentColor = Color.white;
                    }
                    if (child.GetComponent<Outline>() != null)
                    {
                        info.outlineColor = child.GetComponent<Outline>().effectColor;
                    }
                    else
                    {
                        info.outlineColor = Color.black;
                    }
                    if (child.GetComponent<Button>() != null && child.GetComponent<Button>().enabled == true)
                    {
                        info.buttonEnabled = true;
                    }
                    else
                    {
                        info.buttonEnabled = false;
                    }
                    initialObjInfo.Add(info);
                }
                initialObjInfoMap.Add(type, initialObjInfo);
                initalChildrenCountMap.Add(type, initalChildrenCount);
            }
        }

        public void OnDespawn()
        {

        }

        public void OnSpawn()
        {
            if (initialObjInfoMap.ContainsKey(type))
            {
                var initialObjInfo = initialObjInfoMap[type];
                var initalChildrenCount = initalChildrenCountMap[type];
                Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);
                if (initalChildrenCount != allChildren.Length)
                {
                    Debug.LogError("回收对象时发现该对象层级关系发生变化！");
                    return;
                }
                for (int i = 0; i < allChildren.Length; i++)
                {
                    Transform child = allChildren[i];
                    RectTrsInfo info = initialObjInfo[i];
                    RectTransform rect = child.GetComponent<RectTransform>();
                    child.name = info.objName;
                    child.gameObject.SetActive(info.isActive);
                    rect.localPosition = info.localPosition;
                    rect.localScale = info.localScale;
                    rect.anchoredPosition = info.anchoredPosition;
                    rect.anchorMin = info.anchoredMin;
                    rect.anchorMax = info.anchoredMax;
                    rect.sizeDelta = info.sizeDelta;
                    rect.pivot = info.pivot;

                    if (child.GetComponent<Text>() != null)
                    {
                        child.GetComponent<Text>().text = info.textComponentText;
                        child.GetComponent<Text>().color = info.textComponentColor;
                        child.GetComponent<Text>().fontSize = info.textComponentFontSize;
                        child.GetComponent<Text>().lineSpacing = info.textComponentFontLineSpacing;


                    }
                    if (child.GetComponent<Outline>() != null)
                    {
                        child.GetComponent<Outline>().effectColor = info.outlineColor;
                    }

                    if (child.GetComponent<Button>() != null)
                    {
                        child.GetComponent<Button>().enabled = info.buttonEnabled;
                    }
                }
            }
        }
    }
}
