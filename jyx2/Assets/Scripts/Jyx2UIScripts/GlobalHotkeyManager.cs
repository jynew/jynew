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

/// <summary>
/// 全局的热键管理，将对应热键进行堆栈式管理。
/// </summary>
[Obsolete("使用Rewired完全替代，勿再使用")]
public class GlobalHotkeyManager : MonoBehaviour
{
    public static GlobalHotkeyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "[GlobalHotkeyManager]";
                _instance = obj.AddComponent<GlobalHotkeyManager>();
            }
            return _instance;
        }
    }

    private static GlobalHotkeyManager _instance;

    private GlobalHotkeyManager()
    {
    }


    public bool isLock=false;
    
    class HotkeyRegist
    {
        public MonoBehaviour mono;
        public KeyCode key;
        public Action callback;

        public HotkeyRegist(MonoBehaviour m, KeyCode k, Action c)
        {
            mono = m;
            key = k;
            callback = c;
        }
    }

    private Dictionary<KeyCode, Stack<HotkeyRegist>> _hotkeys = new Dictionary<KeyCode, Stack<HotkeyRegist>>();
    

    public bool RegistHotkey(MonoBehaviour mono, KeyCode key, Action callback)
    {
        isLock = true;
        if(!_hotkeys.ContainsKey(key))
            _hotkeys.Add(key, new Stack<HotkeyRegist>());

        var stack = _hotkeys[key];
        foreach (var s in stack)
        {
            if (s.mono == mono)
            {
                Debug.LogWarning($"重复注册了热键! mono={mono}, key={key}");
                return false;
            }
        }

        stack.Push(new HotkeyRegist(mono, key, callback));
        isLock = false;
        return true;
    }

    public void UnRegistHotkey(MonoBehaviour mono, KeyCode key)
    {
        if(!_hotkeys.ContainsKey(key) || _hotkeys[key].Count==0)
            return;
        isLock = true;
        var stack = _hotkeys[key];
        var node = stack.Peek();
        
        if (node != null && node.mono == mono && node.key == key)
        {
            stack.Pop();
        }
        else
        {
            if (node != null)
            {
                Debug.LogWarning($"UnregistHotkey失败，顶部元素不是mono={mono} key={key}，而是mono={node.mono} key={node.key}");
            }
            else
            {
                Debug.LogWarning($"UnregistHotkey失败，顶部元素不是mono={mono} key={key}，为空");    
            }

            _hotkeys.Clear(); //只要出错，就全部清空
        }

        isLock = false;
    }

    void Update_Obsolete()
    {
        if (_hotkeys.Count == 0 || isLock)
            return;
        
        List<KeyCode> key = new List<KeyCode>(_hotkeys.Keys);
        for (int i=0;i< key.Count;i++)
        {
            if (Input.GetKeyDown(key[i]))
            {
                var stack = _hotkeys[key[i]];
                if (stack.Count == 0)
                    continue;
                
                var node = stack.Peek();
                if (node != null)
                {
                    node.callback();
                }
            }
        }
    }
}
