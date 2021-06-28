/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jyx2
{
    static public class UnityTools
    {
        //是否在UI上
        public static bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        public static string Vector3ToString(Vector3 v, char splitChar = '#')
        {
            return v.x + splitChar.ToString() + v.y + splitChar.ToString() + v.z;
        }

        public static Vector3 StringToVector3(string content, char splitChar = '#')
        {
            var tmp = content.Split(splitChar);
            return new Vector3(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));
        }

        public static string QuaternionToString(Quaternion q)
        {
            return $"{q.x}#{q.y}#{q.z}#{q.w}";
        }

        public static Quaternion StringToQuaternion(string content)
        {
            var tmp = content.Split('#');
            return new Quaternion(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]), float.Parse(tmp[3]));
        }



        static public void HighLightObject(GameObject obj, Color color)
        {
            if (obj == null) return;
            var outline = obj.GetComponent<JYX2Outline>();
            if(outline == null)
            {
                outline = obj.AddComponent<JYX2Outline>();
            }
            
            if (outline != null)
            {
                outline.SetOutlineProperty(color, 0.04f);
            }
        }

        /// <summary>
        /// 高亮显示所有交互物体
        /// </summary>
        static public void HighLightObjects(GameObject[] objs, Color color)
        {
            if (objs == null || objs.Length == 0) return;

            foreach (var obj in objs)
            {
                if(obj != null)
                {
                    HighLightObject(obj, color);
                }
            }
        }

        static public void DisHighLightObject(GameObject obj)
        {
            if (obj == null) return;
            var outline = obj.GetComponent<JYX2Outline>();
            if (outline != null)
            {
                GameObject.Destroy(outline);
            }
        }

        /// <summary>
        /// 清除高亮所有交互物体
        /// </summary>
        static public void DisHighLightObjects(GameObject[] objs)
        {
            if (objs == null || objs.Length == 0) return;

            foreach (var obj in objs)
            {
                if(obj != null)
                {
                    DisHighLightObject(obj);
                }
            }
        }

        /// <summary>
        /// 遍历寻找所有子节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        static public Transform DeepFindChild(Transform root, string childName)
        {
            Transform result = null;
            result = root.Find(childName);
            if (result == null)
            {
                foreach (Transform trs in root)
                {
                    result = DeepFindChild(trs, childName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
    }
}
