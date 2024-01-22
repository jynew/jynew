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

            //文件中函数的调用方式
            if (path.Contains("."))
            {
                var tmp = path.Split('.');
                var file = tmp[0];
                var func = tmp[1];
                var chunk = LuaManager.LoadLua(file);
                string luaContent = Encoding.UTF8.GetString(chunk).Trim('\n').Trim('\r');
                luaContent += $"\n{func}()";
                await ExecuteLuaAsync(luaContent, path);
            }
            //简单文件的调用方式
            else
            {
                var chunk = LuaManager.LoadLua(path);
                string luaContent = Encoding.UTF8.GetString(chunk).Trim('\n').Trim('\r');
            
                await ExecuteLuaAsync(luaContent, path);
            }
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

            //Debug.Log("执行lua: " + path);

            string template =
                $"local function temp_lua_func()\r\n {luaContent}\r\n end\r\n util.coroutine_call(combine(temp_lua_func, LuaExecFinished))();\r\n";
            
            var cs = new UniTaskCompletionSource<string>();
            CurrentEventSourceStack.Push(cs);

            try
            {
                luaEnv.DoString(template);
                await cs.Task;
                //Debug.Log("lua执行完毕: " + path);
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

        public static void Clear()
        {
            if (CurrentEventSourceStack.Count > 0)
                CurrentEventSourceStack.Clear();
        }

#region c# api to call Lua functions
        //封装对lua侧模块的普通呼叫，为不同参数个数分别封装泛型
        public static void CallLua(string funName)
        {
            //Debug.Log("Call Lua Function: " + funName);
            try
            {
                LuaToCsBridge.cs_calllua.Action<string>(funName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
            return;
        }
        public static Rst CallLua<Rst>(string funName)
        {
            //Debug.Log("Call Lua Function: " + funName);
            Rst rst;
            try
            {
                rst = LuaToCsBridge.cs_calllua.Func<string, Rst>(funName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
            return rst;
        }
        public static Rst CallLua<Rst, T>(string funName, T par)
        {
            //Debug.Log("Call Lua Function: " + funName);
            Rst rst;
            try
            {
                rst = LuaToCsBridge.cs_calllua.Func<string, T, Rst>(funName, par);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
            return rst;
        }
        public static Rst CallLua<Rst, T1, T2, T3, T4>(string funName, T1 par1, T2 par2, T3 par3, T4 par4)
        {
            //Debug.Log("Call Lua Function: " + funName);
            Rst rst;
            try
            {
                rst = LuaToCsBridge.cs_calllua.Func<string, T1, T2, T3, T4, Rst>(funName, par1, par2, par3, par4);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
            return rst;
        }
        //封装对lua侧异步模块的呼叫
        public static UniTask<Rst> CallLuaAsync<Rst,T>(string funName, T par)
        {
            //Debug.Log("Call Lua Function: " + funName);
            var utcs = new UniTaskCompletionSource<Rst>();
            //用来完成UniTask的回调
            void callback(bool success, Rst lrst, string err)
            {
                if (success)
                {
                    utcs.TrySetResult(lrst);
                }
                else
                {
                    utcs.TrySetCanceled();
                    Debug.LogError(err);
                }
            }

            try
            {//调用lua侧函数
                LuaToCsBridge.cs_await.Action<string, Action<bool, Rst, string>, T>(funName, callback, par);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                utcs.TrySetException(ex);
            }
            return utcs.Task;
        }
#endregion
    }
}
