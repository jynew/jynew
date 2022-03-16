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
using UnityEngine.AddressableAssets;
using XLua;

namespace Jyx2
{
    public class LuaExecutor
    {
        static bool _executing = false;
        
        public static void Execute(string path, Action callback = null)
        {
            if (_executing)
            {
                Debug.LogError("错误：在一个lua未结束的时候，启动另一个lua线程, path=" + path);
                return;
            }
            
            var chunk = LuaManager.LoadLua(path);
            string luaContent = Encoding.UTF8.GetString(chunk).Trim('\n').Trim('\r');
            ExecuteLuaAsync(luaContent, callback, path);
        }

        public static void ExecuteLuaAsync(string luaContent, Action callback = null, string path = "")
        {
            //BY CG:JYX2的特殊情况，有空文件
            if (luaContent.Equals("do return end;"))
            {
                //Debug.Log("识别到空的lua文件，直接跳过:" + path);
                callback?.Invoke();
                return;
            }

            var luaEnv = LuaManager.GetLuaEnv() as LuaEnv;

            Debug.Log("执行lua: " + path);

            _executing = true;
            Loom.RunAsync(() =>
            {
                try
                {
                    luaEnv.DoString(luaContent);
                    Debug.Log("lua执行完毕: " + path);
                }
                catch (Exception e)
                {
                    Debug.LogError("lua执行错误：" + e.ToString());
                    Debug.LogError(e.StackTrace);
                }
                
                _executing = false;
                if (callback != null)
                {
                    Loom.QueueOnMainThread(_ => { callback(); }, null);
                }
            });
        }
		
		public static bool isExcutling(){
			return _executing;
		}
    }
}
