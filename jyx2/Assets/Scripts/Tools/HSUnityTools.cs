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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using Lean.Pool;

namespace Jyx2.Middleware
{
    /// <summary>
    /// 释放游戏物体工具类
    /// </summary>
    static public class HSUnityTools
    {
        public static void Destroy(GameObject obj)
        {
            if (obj == null) return;
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                //if a part of a prefab instance then get the instance handle
                UnityEngine.Object prefabInstance = PrefabUtility.GetPrefabInstanceHandle(obj);
                //destroy the handle
                GameObject.DestroyImmediate(prefabInstance);
                GameObject.DestroyImmediate(obj.gameObject, true);
            }
            else
            {
                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(obj.gameObject);
                else
                    GameObject.Destroy(obj.gameObject);
            }
#else
            GameObject.Destroy(obj.gameObject);
#endif
        }

        public static void Destroy(Transform obj)
        {
            if (obj == null) return;
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                //if a part of a prefab instance then get the instance handle
                UnityEngine.Object prefabInstance = PrefabUtility.GetPrefabInstanceHandle(obj);
                //destroy the handle
                GameObject.DestroyImmediate(prefabInstance);
                GameObject.DestroyImmediate(obj.gameObject, true);
            }
            else
            {
                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(obj.gameObject);
                else
                    GameObject.Destroy(obj.gameObject);
            }
#else
            GameObject.Destroy(obj.gameObject);
#endif
        }

        /// <summary>
        /// 销毁一个物体的所有子物体
        /// 如果为对象池物体则返回对象池
        /// </summary>
        /// <param name="obj"></param>
        public static void DestroyChildren(GameObject obj)
        {
            DestroyChildren(obj.transform);
        }

        /// <summary>
        /// 销毁一个物体的所有子物体
        /// 如果为对象池物体则返回对象池
        /// 
        /// by CG:如果不延迟释放，则会crash:UNITY BUG：UI::CanvasRenderer::SyncDirtyElements
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyChildren(Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                var go = transform.GetChild(i).gameObject;
                go.SetActive(false);
                LeanPool.Despawn(go);
            }
            transform.DetachChildren();
        }

        /// <summary>
        /// 销毁一个脚本所在物体的所有子物体
        /// 如果为对象池物体则返回对象池
        /// 
        /// by CG:如果不延迟释放，则会crash:UNITY BUG：UI::CanvasRenderer::SyncDirtyElements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        //by CG:如果不延迟释放，则会crash:UNITY BUG：UI::CanvasRenderer::SyncDirtyElements
        public static void SafeDestroyGameObject<T>(T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(false);
            LeanPool.Despawn(obj.gameObject);
        }

        /// <summary>
        /// 销毁一个物体
        /// 如果为对象池物体则返回对象池
        /// by CG:如果不延迟释放，则会crash:UNITY BUG：UI::CanvasRenderer::SyncDirtyElements
        /// </summary>
        /// <param name="obj"></param>
        public static void SafeDestroyGameObject(GameObject obj)
        {
            obj.SetActive(false);
            LeanPool.Despawn(obj);
        }

        /// <summary>
        /// 销毁一个物体
        /// 如果为对象池物体则返回对象池
        /// by CG:如果不延迟释放，则会crash:UNITY BUG：UI::CanvasRenderer::SyncDirtyElements
        /// </summary>
        /// <param name="t"></param>
        public static void SafeDestroyGameObject(Transform t)
        {
            SafeDestroyGameObject(t.gameObject);
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static void ChangeLayersRecursively(Transform trans, string name)
        {
            //CG TODO，重构
            //if (MapUI.Instance != null && MapUI.Instance.BigMapCanvas == trans.gameObject && name != "DontShow") {
            //    name = "BigMap";
            //}

            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                ChangeLayersRecursively(child, name);
            }
        }

        public static void ChangeLayer(Transform trans, string name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
        }

        public static void TurnRaycastTargetOff(Transform trans, bool isOn)
        {
            foreach (var item in trans.GetComponentsInChildren<Graphic>())
            {
                item.GetComponent<Graphic>().raycastTarget = isOn;
            }
        }


        /// <summary>
        /// 获取一个特效的时长
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static float ParticleSystemLength(Transform transform)
        {
            var pts = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0f;
            foreach (var p in pts)
            {
                if (p.enableEmission)
                {
                    /*if (p.loop)
                    {
                        return -1f;
                    }*/
                    float dunration = 0f;
                    if (p.emissionRate <= 0)
                    {
                        dunration = p.startDelay + p.startLifetime;
                    }
                    else
                    {
                        dunration = p.startDelay + Mathf.Max(p.duration, p.startLifetime);
                    }
                    if (dunration > maxDuration)
                    {
                        maxDuration = dunration;
                    }
                }
            }
            return maxDuration;
        }
    }
}

