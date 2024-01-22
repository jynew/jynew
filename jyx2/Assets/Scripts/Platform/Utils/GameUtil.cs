/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// JYX工具类
/// </summary>

public static class GameUtil
{
    

    public static void GamePause(bool pause) 
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public static void BetterSetActive(this GameObject go, bool isActive)
    {
        if (go == null || go.activeSelf == isActive)
            return;
        go.SetActive(isActive);
    }


    public static Component GetOrAddComponent(Transform trans,string type)
    {
        Component com = trans.GetComponent(type);
        if (com == null) 
        {
            System.Type t = System.Type.GetType(type);
            com = trans.gameObject.AddComponent(t);
        }
        return com;
    }

    public static T GetOrAddComponent<T>(Transform trans) where T:Component 
    {
        T com = trans.GetComponent<T>();
        if (com == null)
        {
            com = trans.gameObject.AddComponent<T>();
        }
        return com;
    }
    
    public static T GetOrAddComponent<T>(this GameObject go) where T:Component
    {
        return GetOrAddComponent<T>(go.transform);
    }

    public static void LogError(string str) 
    {
        Debug.LogError(str);
    }
    
    
    public static void CallWithDelay(double time,Action action, Component attachedComponent = null)
    {
        if(time == 0)
        {
            action();
            return;
        }

        var observable = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(ms =>
        {
            action();
        });

        //避免关联对象销毁后延时逻辑仍然访问该对象的问题
        if (attachedComponent != null)
            observable.AddTo(attachedComponent);
    }

}
