using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;



/// <summary>
/// JYX工具类
/// </summary>

public class GameUtil
{
    /// <summary>
    /// 选择角色
    /// </summary>
    /// <param name="roles"></param>
    /// <param name="callback">如果放弃，则返回null</param>
    static public void SelectRole(List<RoleInstance> roles, Action<RoleInstance> callback)
    {
        //选择使用物品的人
        List<string> selectionContent = new List<string>();
        foreach (var role in roles)
        {
            selectionContent.Add(role.Name);
        }
        selectionContent.Add("取消");
        var storyEngine = StoryEngine.Instance;
        storyEngine.BlockPlayerControl = true;
        
        SelectRoleParams selectParams = new SelectRoleParams();
        selectParams.roleList = roles;
        selectParams.title = "选择使用的人";
        selectParams.callback = (cbParam) => 
        {
            storyEngine.BlockPlayerControl = false;
            if (cbParam.selectList.Count <= 0)
            {
                callback(null);
                return;
            }
            var selectRole = cbParam.selectList[0];//默认只会选择一个
            callback(selectRole);
        };

        Jyx2_UIManager.Instance.ShowUI("SelectRolePanel", selectParams);
    }

    /// <summary>
    /// 显示冒泡文字
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="duration"></param>
    static public void DisplayPopinfo(string msg, float duration =2f)
    {
        StoryEngine.Instance.DisplayPopInfo(msg, duration);
    }

    static public void ShowFullSuggest(string content, string title = "", Action cb = null) 
    {
        Jyx2_UIManager.Instance.ShowUI("FullSuggestUIPanel", content, title, cb);
    }

    static public void GamePause(bool pause) 
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    static public Component GetOrAddComponent(Transform trans,string type)
    {
        Component com = trans.GetComponent(type);
        if (com == null) 
        {
            System.Type t = System.Type.GetType(type);
            com = trans.gameObject.AddComponent(t);
        }
        return com;
    }

    static public T GetOrAddComponent<T>(Transform trans) where T:Component 
    {
        T com = trans.GetComponent<T>();
        if (com == null)
        {
            com = trans.gameObject.AddComponent<T>();
        }
        return com;
    }

    static public void LogError(string str) 
    {
        Debug.LogError(str);
    }
    
    
    static public void CallWithDelay(double time,Action action)
    {
        if(time == 0)
        {
            action();
            return;
        }

        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(ms =>
        {
            action();
        });
    }
}
