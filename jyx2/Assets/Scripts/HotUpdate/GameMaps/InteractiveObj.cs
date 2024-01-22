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
using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[Obsolete("不再需要直接点击物体交互了")]
public class InteractiveObj : MonoBehaviour
{
    GameObject[] m_renderGameObjects;

    void Start()
    {
        InitForRenderGameObjects();//寻找渲染的GameObject
        InitForColliders(); //初始化Coliider
    }

    void InitForRenderGameObjects()
    {
        //寻找渲染的GameObject
        List<GameObject> renderObjs = new List<GameObject>();
        if (HasRenderer(this.gameObject))
        {
            renderObjs.Add(this.gameObject);
        }
        else
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (HasRenderer(child.gameObject))
                {
                    renderObjs.Add(child.gameObject);
                }
            }
        }
        m_renderGameObjects = renderObjs.ToArray();
    }

    bool HasRenderer(GameObject obj)
    {
        return (obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<SkinnedMeshRenderer>() != null);
    }

    void InitForColliders()
    {
        //有Collider了也不需要添加
        if (gameObject.GetComponent<BoxCollider>() != null)
            return;

        if (gameObject.GetComponent<SphereCollider>() != null)
            return;

        //看是否有碰撞胶囊体，如果有，则生成一个一样的胶囊体
        if(gameObject.GetComponent<NavMeshObstacle>() != null)
        {
            var nvOb = gameObject.GetComponent<NavMeshObstacle>();
            CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
            c.center = nvOb.center;
            c.radius = nvOb.radius;
            c.height = nvOb.height;
        }
        //否则添加一个BOX collider
        else {
            var box = gameObject.AddComponent<BoxCollider>();
            box.center = new Vector3(0, 0.5f, 0);
        }

        ////否则默认添加一个MeshCollider
        //if (gameObject.GetComponent<MeshCollider>() == null)
        //{
        //    gameObject.AddComponent<MeshCollider>();
        //}
    }

    //BY CGGG：不再需要鼠标点击模式
    
    /*
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        //this.GetComponent<MeshRenderer>().material.color = Color.red;
        
        
        UnityTools.HighLightObject(this.gameObject, Color.yellow);
    }

    private void OnMouseExit()
    {
        //this.GetComponent<MeshRenderer>().material.color = Color.white;
        UnityTools.DisHighLightObject(this.gameObject);
    }
    */

    private void OnMouseUp()
    {
        if(_callback != null)
        {
            _callback(this);
        }
    }

    public void SetMouseClickCallback(Action<InteractiveObj> callback)
    {
        _callback = callback;
    }

    Action<InteractiveObj> _callback;
}
