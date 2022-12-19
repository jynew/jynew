using System;
using System.Collections;
using System.Collections.Generic;
using Jyx2;
using UIWidgets;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 想用Hunter6 的笔名做一个短篇mods,所以加一些事件管理Editor，方便快速开发和调试事件
/// </summary>
public class GameEventEditorWindow : EditorWindow
{
    [MenuItem("项目快速导航/游戏事件快速调试",priority = 102)]
    public static void StartWindow()
    {
        GetWindow<GameEventEditorWindow>().Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(5);
        if (GameEventManager.Inst == null)
        {
            GUILayout.Label("需要进入游戏和启动事件后，才能正常显示事件调试");
            return;
        }
        GUILayout.Label("若当前事件错误，可用[x]关闭，先调试其他事件");
        GUILayout.BeginHorizontal();
      //  EditorGUILayout.LabelField("当前事件Id",GameEventManager.Inst.CurrEvent);
        EditorGUILayout.LabelField("当前事件Id", GetCurrEventStr());
        if (GUILayout.Button("x"))
        {
            CloseCurrEvent();
        }

        GUILayout.EndHorizontal();
        // if (GUILayout.Button("跳到事件（Mods-hunter6-TL-X"))
        // {
        //     GoToModsLuaFile();
        // }
        
        GUILayout.Space(10);
        
    }

    void CloseCurrEvent()
    {
        //删除当前事件
       // Jyx2LuaBridge.EveDel();
        GameEventManager.Inst.SetCurrentGameEvent(null);
        Debug.Log("清空了。。。当前事件 ");
        //Eve();//打印看看是否还有事件
    }

    string GetCurrEventStr()
    {
        var eve = GameEventManager.GetCurrentGameEvent();
        if (eve == null)
        {
            return "nil";
        }
        else
        {
            string npcStr = "no target";
             if (eve.m_EventTargets.Length > 0)
             {
                 npcStr = eve.m_EventTargets[0].name;
             }
             return $"index={eve.name}-{npcStr}-{eve.m_InteractiveEventId}";;
        }
    }

    void GoToModsLuaFile()
    {
    }
}
