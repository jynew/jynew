using System;
using System.Collections.Generic;
using System.IO;
using GLib;
using HanSquirrel;
using HanSquirrel.ResourceManager;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using UnityEngine;
using XLua;

namespace Jyx2
{
    public class LuaManager
    {
        public const string LUA_ROOT_MENU = "";
        public const bool LUAJIT_ENABLE = false;


        public LuaManager() { }

        static public bool IsInited()
        {
            return _inited;
        }

        static public object GetLuaEnv()
        {
            return luaEnv as object;
        }

        static public void Clear()
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
                HSLogManager.GetLogger("Booter").Debug("★★★★ Destroying lua state finished★★★★ ");
            }

            ClearLuaMapper();
        }

        static public void Init(bool forceReset = false)
        {
            if (forceReset)
            {
                Clear();
            }
            if (_inited) return;

            using (ExeTimer.GetEndIndent("LuaManagerImpl.Init", x => _PerfLogger.Info(x)))
            {
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

                //ConfigManager.Init();

                HSLogManager.GetLogger("Booter").Info("lua vm 装载成功");
            }
        }

        static public object[] Call(string functionName, params object[] paras)
        {
            if (!_inited)
            {
                Init();
            }

            var func = getCachedFunction(functionName);
            if (func == null)
            {
                _Logger.Error("调用了未定义的lua 函数 [{0}]", functionName);
                return null;
            }
            else
            {
                var tmp = func.Call(paras);
                return tmp;
            }
        }

        static public T Call<T>(string functionName, params object[] paras)
        {
            if (!_inited)
            {
                Init();
            }

            var func = getCachedFunction(functionName);
            if (func == null)
            {
                _Logger.Error("调用了未定义的lua 函数 [{0}]", functionName);
                return default(T);
            }
            else
            {
                var rtn = func.Call(paras);
                if (rtn.Length == 0 || rtn[0] is Boolean && (bool)rtn[0] == false) return default(T);

                return (T)rtn[0];
            }
        }

        static public object CallWithSingleReturn(string functionName, params object[] paras)
        {
            return Call(functionName, paras)[0];
        }

        static public int CallWithIntReturn(string functionName, params object[] paras)
        {
            return Convert.ToInt32(Call(functionName, paras)[0]);
        }

        static public double CallWithDoubleReturn(string functionName, params object[] paras)
        {
            return Convert.ToDouble(Call(functionName, paras)[0]);
        }

        static public void GC()
        {
            if (luaEnv != null)
            {
                var before = Call<double>("get_lua_memory_cost");
                luaEnv.GC();
                var after = Call<double>("get_lua_memory_cost");
                _PerfLogger.Info("Lua GC {0}-{1}={2}", ((int)before).ToMBKBB(), ((int)after).ToMBKBB(), ((int)(before - after)).ToMBKBB());
            }
        }

        static public byte[] LoadLua(string path)
        {
            //BY CG：在unity编辑模式下，方便调试，不用repack lua
            if (Application.isEditor)
            {
                return File.ReadAllBytes("data/lua/" + path + ".lua");
            }


            if (__luaMapper == null)
            {
                using (var input = BinaryResourceLoader.CreateStreamFromCEBinary(HSUnityEnv.CELuaPath))
                    __luaMapper = DirectProtoBufTools.Deserialize<Dictionary<string, byte[]>>(input);
            }

            //处理文件名格式
            if (!path.Contains("/"))
            {
                path = path.Replace(".", "/");
            }

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

            if (__luaMapper.ContainsKey(path))
            {
                return __luaMapper[path];
            }
            else
            {
                _Logger.Error("找不到文件 [{0}]", path);
                return null;
            }
        }


        #region private
        static private void ClearLuaMapper()
        {
            __luaMapper = null;
        }
        static private readonly string[] files = new string[]{
            "main.lua",
        };

        /// <summary> 保证LUA资源及时释放，主动胜过被动，被动胜过不做。@George </summary>
        private static readonly PassiveResourceDisposer _passiveResourceDisposer = PassiveResourceDisposer.Create(delegate
        {
            if (LuaManager.IsInited() && HSUnityEnv.NeedElegantDispose)
            {   //因为是在析构函数里面调用的缘故，好像有了这个代码后Untiy有时候在重新编译的时候会死机。
                //这个仅仅是在游戏没有运行的时候，LUA被无意初始化后才会发生，因此智能释放的概率本身很低，故此暂时先这么写代码。
                LuaManager.Clear();
                HSLogManager.GetLogger("Booter").Debug("★★★★ 智能释放Lua资源成功 ★★★★");
            }
        });

        static private IHSLogger _Logger = HSLogManager.GetLogger("Lua");
        static private IHSLogger _PerfLogger = HSLogManager.GetLogger("Perf");

        static private bool _inited = false;
        static private LuaEnv luaEnv;
        static private void LoadLuaFiles()
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

        static private Dictionary<string, byte[]> __luaMapper = null;

        static private Dictionary<string, LuaFunction> _cachedFunc = new Dictionary<string, LuaFunction>();
        static private LuaFunction getCachedFunction(string name)
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

