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
using Cysharp.Threading.Tasks;
using UnityEngine;
using XLua;
using Jyx2.ResourceManagement;
using Jyx2.Middleware;
using Jyx2.MOD;
using System.IO;
using Jyx2.MOD.ModV2;

namespace Jyx2
{
    public class LuaManager
    {
        public const bool LUAJIT_ENABLE = false;

        /// <summary>
        /// 是否在editor中调试时热加载lua（可以不重启游戏进行编辑）
        /// </summary>
        public const bool HOTRELOAD_LUA_IN_EDITOR = true;

        public LuaManager() { }

        private static bool IsInited()
        {
            return _inited;
        }

        public static LuaEnv GetLuaEnv()
        {
            return luaEnv;
        }

        public static void Clear()
        {
            LuaMod_DeInit();
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
                luaEnv.Dispose();
                //luaEnv.FullGc();
                //HSUtils.Log ("★★★Destroying lua state..");
                luaEnv = null;
            }

            ClearLuaMapper();
        }

        public static void Init(string rootLuaFileText)
        {
            if (_inited) return;

            
            luaEnv = new LuaEnv();

            luaEnv.AddLoader((ref string filename) =>
            {
                //require时默认去baseasset下加载lua
                var luaFile = ResLoader.LoadAssetSync<TextAsset>($"Assets/BuildSource/Lua/{filename}.lua");
                if (luaFile != null)
                    return Encoding.UTF8.GetBytes(luaFile.text);
                return null;
            });

            //jit interpreter
            if (LuaManager.LUAJIT_ENABLE)
            {
                luaEnv.DoString("\nif jit then\n\n  jit.off();jit.flush()\n\nend");
            }


            if (!string.IsNullOrEmpty(rootLuaFileText))
            {
                luaEnv.DoString(rootLuaFileText);    
            }
            
            //load lua base files
            /*foreach (var f in files)
            {
                luaEnv.DoString(LoadLua(LuaManager.LUA_ROOT_MENU + f.Replace(".lua", "")), f);
            }*/

            _inited = true;
            //LoadLuaFiles();
        }

        public static void PreloadLua()
        {
            if (RuntimeEnvSetup.CurrentModConfig == null)
            {
                Debug.LogError("错误：没初始化运行环境就调用了preloadlua");
                return;
            }

            if (RuntimeEnvSetup.CurrentModConfig.PreloadedLua != null)
            {
                UniTask.Void(async () =>
                {
                    foreach (var lua in RuntimeEnvSetup.CurrentModConfig.PreloadedLua)
                    {
                        Debug.Log($"preloading {lua}...");
                        if (_luaMapper.ContainsKey(lua))
                        {
                            
                            await LuaExecutor.Execute(lua);
                        }
                        
                        Debug.Log($"load {lua} finished.");
                    }
                    
                    Debug.Log("PreloadLua finished");
                });
            }
        }

        private static object[] Call(string functionName, params object[] paras)
        {
            if (!_inited)
            {
                Init(null);
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
                Init(null);
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
            //在unity编辑模式下，方便调试，不用repack lua
            if (HOTRELOAD_LUA_IN_EDITOR && Application.isEditor)
            {
                var curMod = RuntimeEnvSetup.CurrentModConfig.ModId;
                return File.ReadAllBytes($"Assets/Mods/{curMod}/Lua/" + path + ".lua");
            }

            //处理文件名格式
            if (!path.Contains("/"))
            {
                path = path.Replace(".", "/");
            }

            path = path.Split('/').Last();
            
            if (_luaMapper.ContainsKey(path))
            {
                string code = Encoding.UTF8.GetString(_luaMapper[path]);
                
                Debug.Log(code);
                
                return _luaMapper[path];
            }
            else
            {
                Debug.LogError($"找不到文件 [{path}]");
                return null;
            }
        }

        public static void LuaMod_Init()
        {
            Call("LuaMod_Init");
        }

        public static void LuaMod_DeInit()
        {
            Call("LuaMod_DeInit");
        }

        #region private
        private static void ClearLuaMapper()
        {
            _luaMapper = null;
        }

        public static async UniTask InitLuaMapper()
        {
            var assets = await ResLoader.LoadAssets<TextAsset>("Assets/Lua/");

            _luaMapper = new Dictionary<string, byte[]>();
            foreach (var a in assets)
            {
                _luaMapper[a.name] = Encoding.UTF8.GetBytes(a.text);
            }
            
            /*var mod = RuntimeEnvSetup.GetCurrentMod();
            if (mod is GameModEditor)
            {
                await InitMapperFromEditor();
            }
            else
            {
                var assets = await ResLoader.LoadAssets<TextAsset>("Assets/Lua/");

                _luaMapper = new Dictionary<string, byte[]>();
                foreach (var a in assets)
                {
                    _luaMapper[a.name] = Encoding.UTF8.GetBytes(a.text);
                }
            }*/
        }

        private static async UniTask InitMapperFromEditor()
        {
            try
            {
                var curModDir = RuntimeEnvSetup.CurrentModConfig.ModRootDir + "/Lua";
                var luaFilePaths = new List<string>();
                var luafilter = new List<string>() { ".lua" };
                FileTools.GetAllFilePath(curModDir, luaFilePaths, luafilter);

                Debug.Log("加载当前mod的lua重载文件, 路径:" + curModDir);

                if (luaFilePaths.Count == 0)
                    return;

                foreach (var path in luaFilePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    if(_luaMapper.ContainsKey(fileName))
                    {
                        Debug.LogFormat("Lua文件[{0}]的逻辑将会被AB外的同名文件重载掉", fileName);
                    }
                    _luaMapper[fileName] = await FileTools.ReadAllBytesAsync(path);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("读取Mod文件夹的Lua文件异常:" + ex.ToString());
            }
        }


        
        private static bool _inited = false;
        private static LuaEnv luaEnv;

        private static Dictionary<string, byte[]> _luaMapper = null;

        private static Dictionary<string, LuaFunction> _cachedFunc = new Dictionary<string, LuaFunction>();
        private static LuaFunction getCachedFunction(string name)
        {
            if (_cachedFunc.ContainsKey(name))
            {
                return _cachedFunc[name];
            }
            var func = luaEnv.Global.Get<LuaFunction>(name); // TODO:XLua fix
            if(func != null)
                _cachedFunc.Add(name, func);
            else
            {
                Debug.LogWarning("尝试获取不存在的Lua函数, function name :" + name);
            }
            return func;
        }
        #endregion
    }
}

