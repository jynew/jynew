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
using Jyx2Configs;

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
    public static async UniTask SelectRole(IEnumerable<RoleInstance> roles, Action<RoleInstance> callback)
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
        selectParams.roleList = roles.ToList();
        selectParams.title = "选择使用的人";
        selectParams.isDefaultSelect=false;
        selectParams.callback = (cbParam) => 
        {
            storyEngine.BlockPlayerControl = false;
            if (cbParam.isCancelClick == true)
            {
                return;
            }
            if (cbParam.selectList.Count <= 0)
            {
                callback(null);
                return;
            }
            var selectRole = cbParam.selectList[0];//默认只会选择一个
            callback(selectRole);
        };

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SelectRolePanel), selectParams);
    }

    /// <summary>
    /// 显示冒泡文字
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="duration"></param>
    public static void DisplayPopinfo(string msg, float duration =2f)
    {
        StoryEngine.Instance.DisplayPopInfo(msg, duration);
    }

    public static async void ShowFullSuggest(string content, string title = "", Action cb = null) 
    {
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(FullSuggestUIPanel), content, title, cb);
    }

    public static void GamePause(bool pause) 
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
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
    
    public static T GetOrAddComponent<T>(GameObject go) where T:Component
    {
        return GetOrAddComponent<T>(go.transform);
    }

    public static void LogError(string str) 
    {
        Debug.LogError(str);
    }
    
    
    public static void CallWithDelay(double time,Action action)
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

    public static async UniTask ShowYesOrNoCastrate(RoleInstance role, Action action)
    {
        if (role.Sex == 0)//男
        {
            string msg = "修炼此书必须先行挥剑自宫，你是否仍要修炼？";
            List<string> selectionContent = new List<string>() { "是(Y)", "否(N)" };
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((index) =>
            {
                if (index == 0)
                {
                    ChangeScence();
                    role.Sex = 2;
                    action();
                }
            }));
        }
        else if (role.Sex == 1)//女
        {
            DisplayPopinfo("此人不适合修炼此物品");
            return;
        }
        else if (role.Sex == 2)//太监
        {
            action();
        }
    }

    private static void ChangeScence()
    {
        //惨叫
        string path = "Assets/BuildSource/sound/nancanjiao.wav";
        if (Camera.main != null) AudioManager.PlayClipAtPoint(path, Camera.main.transform.position).Forget();

        //血色
        var blackCover = LevelMaster.Instance.transform.Find("UI/BlackCover");
        if (blackCover == null)
        {
            Debug.LogError("DarkScence error，找不到LevelMaster/UI/BlackCover");
            return;
        }

        blackCover.gameObject.SetActive(true);
        var img = blackCover.GetComponent<Image>();
        img.DOColor(Color.red, 2).OnComplete(() =>
        {
            blackCover.gameObject.SetActive(false);
        });
    }

    public static async UniTask ShowYesOrNoUseItem(Jyx2ConfigItem item, Action action)
    {
        if (GameRuntimeData.Instance.GetItemUser(item.Id) != -1)
        {
            string msg = (int)item.ItemType == 1 ? "此物品已经有人配备，是否换人配备？" : "此物品已经有人修炼，是否换人修炼？";
            List<string> selectionContent = new List<string>() { "是(Y)", "否(N)" };
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((index) =>
            {
                if (index == 0)
                {
                    action();
                }
            }));
        }
        else
        {
            action();
        }

    }
}
