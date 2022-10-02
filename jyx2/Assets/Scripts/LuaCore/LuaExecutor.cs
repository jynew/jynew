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
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Jyx2
{
    public class LuaExecutor
    {
        public static async UniTask Execute(string path)
        {
            /*if (CurrentEventSource != null)
            {
                Debug.LogError("错误：一个lua未完成的情况下，执行了另一个lua");
                return;
            }*/

            
            var chunk = LuaManager.LoadLua(path);
            string luaContent = Encoding.UTF8.GetString(chunk).Trim('\n').Trim('\r');
            await ExecuteLuaAsync(luaContent, path);
        }

        public static readonly Stack<UniTaskCompletionSource<string>> CurrentEventSourceStack =
            new Stack<UniTaskCompletionSource<string>>();

        public static async UniTask ExecuteLuaAsync(string luaContent, string path = "")
        {
            //BY CG:JYX2的特殊情况，有空文件
            if (luaContent.Equals("do return end;"))
            {
                //Debug.Log("识别到空的lua文件，直接跳过:" + path);
                return;
            }

            var luaEnv = LuaManager.GetLuaEnv();

            Debug.Log("执行lua: " + path);

            
            string template =
                $"local function temp_lua_func()\r\n {luaContent}\r\n end\r\n util.coroutine_call(combine(temp_lua_func, LuaExecFinished))();\r\n";
            
            var cs = new UniTaskCompletionSource<string>();
            CurrentEventSourceStack.Push(cs);

            try
            {
                luaEnv.DoString(template);
                await cs.Task;
                Debug.Log("lua执行完毕: " + path);
            }
            catch (Exception e)
            {
                if(IsExecuting())
                {
                    //异常情况有时候不会执行到Lua代码末尾，要手动结束掉对应的UniTaskCompletionSource
                    if (CurrentEventSourceStack.Peek() == cs)
                        Jyx2LuaBridge.LuaExecFinished(template);
                }
                Debug.LogError("lua执行错误：" + e.ToString());
                Debug.LogError(e.StackTrace);
            }
        }
		
		public static bool IsExecuting()
        {
            return CurrentEventSourceStack.Count > 0;
        }
    }
}
