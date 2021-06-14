using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Jyx2
{

    public class JYX2LuaEvnContext
    {
        public int currentItemId;
    }

    public class LuaExecutor
    {
        static public JYX2LuaEvnContext currentLuaContext = null;

        static bool _executing = false;

        public static void Execute(string path, Action callback = null, JYX2LuaEvnContext context = null)
        {
            if (_executing)
            {
                Debug.LogError("错误：在一个lua未结束的时候，启动另一个lua线程, path=" + path);
                return;
            }

            currentLuaContext = context;

            var chunk = LuaManager.LoadLua(path);
            string luaContent = Encoding.UTF8.GetString(chunk).Trim('\n').Trim('\r');
            ExecuteLua(luaContent, callback, path);
        }

        public static void ExecuteLua(string luaContent, Action callback = null, string path = "")
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

                currentLuaContext = null;
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
