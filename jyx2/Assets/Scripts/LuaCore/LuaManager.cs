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
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XLua;
using Jyx2.MOD;
using Jyx2.Middleware;

namespace Jyx2
{
    public class LuaManager
    {
        public const string LUA_ROOT_MENU = "";
        public const bool LUAJIT_ENABLE = false;


        public LuaManager() { }

        private static bool IsInited()
        {
            return _inited;
        }

        public static object GetLuaEnv()
        {
            return luaEnv as object;
        }

        private static void Clear()
        {

            _inited = false;

            //ConfigManager.Reset();

            if (_cachedFunc != null && _cachedFunc.Count > 0)
            {
                foreach (var cf in _cachedFunc.Values)
                    cf.Dispose(); //Added By George

                _cachedFunc.Clear();
                //HSUtils.Log("Destroying lua cachedFunc finished.");
            }

            if (luaEnv != null)
            {
                luaEnv.FullGc();
                //HSUtils.Log ("★★★Destroying lua state..");
                luaEnv = null;
            }

            ClearLuaMapper();
        }

        public static void Init(bool forceReset = false)
        {
            if (forceReset)
            {
                Clear();
            }
            if (_inited) return;

            
            luaEnv = new LuaEnv();

            //jit interpreter
            if (LuaManager.LUAJIT_ENABLE)
            {
                luaEnv.DoString("\nif jit then\n\n  jit.off();jit.flush()\n\nend");
            }

            //load lua base files
            foreach (var f in files)
            {
                luaEnv.DoString(LoadLua(LuaManager.LUA_ROOT_MENU + f.Replace(".lua", "")), f);
            }

            _inited = true;
            LoadLuaFiles();


        }

        private static object[] Call(string functionName, params object[] paras)
        {
            if (!_inited)
            {
                Init();
            }

            var func = getCachedFunction(functionName);
            if (func == null)
            {
                return null;
            }
            else
            {
                var tmp = func.Call(paras);
                return tmp;
            }
        }

        private static T Call<T>(string functionName, params object[] paras)
        {
            if (!_inited)
            {
                Init();
            }

            var func = getCachedFunction(functionName);
            if (func == null)
            {
                return default(T);
            }
            else
            {
                var rtn = func.Call(paras);
                if (rtn.Length == 0 || rtn[0] is Boolean && (bool)rtn[0] == false) return default(T);

                return (T)rtn[0];
            }
        }

        public static object CallWithSingleReturn(string functionName, params object[] paras)
        {
            return Call(functionName, paras)[0];
        }

        public static int CallWithIntReturn(string functionName, params object[] paras)
        {
            return Convert.ToInt32(Call(functionName, paras)[0]);
        }

        public static double CallWithDoubleReturn(string functionName, params object[] paras)
        {
            return Convert.ToDouble(Call(functionName, paras)[0]);
        }

        public static void GC()
        {
            if (luaEnv != null)
            {
                var before = Call<double>("get_lua_memory_cost");
                luaEnv.GC();
                var after = Call<double>("get_lua_memory_cost");
            }
        }

        public static byte[] LoadLua(string path)
        {
            //BY CG：在unity编辑模式下，方便调试，不用repack lua
/*            if (Application.isEditor)
            {
                return File.ReadAllBytes("Assets/BuildSource/Lua/" + path + ".lua");
            }*/

            //处理文件名格式
            if (!path.Contains("/"))
            {
                path = path.Replace(".", "/");
            }

            /*
            if (LuaManager.LUAJIT_ENABLE)
            {
                path = "/luajit/" + path;
            }
            else
            {
                path = "/lua/" + path;
            }

            if (!path.EndsWith(".lua"))
            {
                path += ".lua";
            }
            */

            path = path.Split('/').Last();
            
            if (__luaMapper.ContainsKey(path))
            {
                string code = Encoding.UTF8.GetString(__luaMapper[path]);
                
                Debug.Log(code);
                
                return __luaMapper[path];
            }
            else
            {
                Debug.LogError($"找不到文件 [{path}]");
                return null;
            }
        }


        #region private
        private static void ClearLuaMapper()
        {
            __luaMapper = null;
        }

        public static async UniTask InitLuaMapper()
        {
            /*            if (Application.isEditor) //编辑器模式下不需要缓存，直接读取文件
                            return;*/
            var overridePaths = await MODLoader.LoadOverrideList("Assets/BuildSource/Lua");
            
            var assets = await MODLoader.LoadAssets<TextAsset>(overridePaths);

            __luaMapper = new Dictionary<string, byte[]>();
            foreach (var a in assets)
            {
                __luaMapper[a.name] = Encoding.UTF8.GetBytes(a.text);
            }
        }
        
        private static readonly string[] files = new string[]{
            "main.lua",
        };
        
        
        private static bool _inited = false;
        private static LuaEnv luaEnv;
        private static void LoadLuaFiles()
        {
            LuaTable files = Call<LuaTable>("main_getLuaFiles");
            foreach (var luaFile in files.Cast<List<string>>())
            {
                //HSUtils.Log(luaFile);
                luaEnv.DoString(LoadLua(LuaManager.LUA_ROOT_MENU + luaFile.Replace(".lua", "")), luaFile);
            }
            files.Dispose();
            files = null;
        }

        private static Dictionary<string, byte[]> __luaMapper = null;

        private static Dictionary<string, LuaFunction> _cachedFunc = new Dictionary<string, LuaFunction>();
        private static LuaFunction getCachedFunction(string name)
        {
            if (_cachedFunc.ContainsKey(name))
            {
                return _cachedFunc[name];
            }
            var func = luaEnv.Global.Get<LuaFunction>(name); // TODO:XLua fix
            _cachedFunc.Add(name, func);
            return func;
        }
        #endregion
    }
}

