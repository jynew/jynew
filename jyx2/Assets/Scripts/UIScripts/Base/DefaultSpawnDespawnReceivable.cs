using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HSUI;
using HanSquirrel.ResourceManager;

namespace HSUI
{
    public class DefaultSpawnDespawnReceivable:MonoBehaviour,ISpawnDespawnReceivable 
    {
        static Dictionary<string,List<RectTrsInfo>> initialObjInfoMap=new Dictionary<string, List<RectTrsInfo>>();
        static Dictionary<string,int> initalChildrenCountMap=new Dictionary<string, int>();
        private string type;

        struct RectTrsInfo
        {
            public string objName;
            public bool isActive;
            public Vector3 localPosition;
            public Vector3 localScale;

            public RectTrsInfo(string name, bool isActive, Vector3 localPosition, Vector3 localScale, Vector2 anchoredPosition)
            {
                this.objName = name;
                this.isActive = isActive;
                this.localPosition = localPosition;
                this.localScale = localScale;
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
                    Transform tras = child.transform;
                    info.objName = child.name;
                    info.isActive = child.gameObject.activeSelf;
                    info.localPosition = tras.localPosition;
                    info.localScale = tras.localScale;
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
                var initalChildrenCount= initalChildrenCountMap[type];
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
                }
            }
        }
    }
}
