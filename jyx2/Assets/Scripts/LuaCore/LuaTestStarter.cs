using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
using Jyx2;

[CreateAssetMenu(menuName = "金庸重制版/LUA测试器", fileName = "LuaTestStarter")]
public class LuaTestStarter : ScriptableObject
{
#if UNITY_EDITOR
    [LabelText("要测试的Lua文件")] public List<TextAsset> luaFiles;
    [Button("测试Lua代码")]
    public void luaTest()
    {
        LuaManager.Init("");
        var luaEnv = LuaManager.GetLuaEnv();
        foreach (var file in luaFiles)
        {
            luaEnv.DoString(Encoding.UTF8.GetBytes(file.text));
        }
    }
#endif
}
