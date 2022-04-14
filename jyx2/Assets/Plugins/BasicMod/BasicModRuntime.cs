using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HanSquirrel.OpenSource
{
    public static class BasicModConfig
    {
        ///显示详细日志
        public static bool Verbose { get; set; }

        /// 游戏里面是固定的目录作为MOD的根目录，所有MOD都直接下载到这个目录下面。
        public static readonly string RootDir =
#if UNITY_EDITOR
            $"ModOutput/{EditorUserBuildSettings.activeBuildTarget}";
#elif UNITY_STANDALONE_WIN
             new DirectoryInfo(Application.streamingAssetsPath).Parent.Parent.FullName+ "/ModDeploy";
            ///不直接用"ModDeploy"是因为Untiy里面Build&Run的时候，当前路径并非游戏EXE的路径
#else
             Application.persistentDataPath + "/ModDeploy";
#endif
    }

    public static class BasicModRuntime
    {
        private static readonly Dictionary<string, string> _DirMap = new Dictionary<string, string>();

        static BasicModRuntime() => Addressables.ResourceManager.InternalIdTransformFunc = x => Redirect(x.InternalId);

        ///从任意目录安装MOD
        public static void RegisterMod(string path)
        {
            if (_Inited)
                throw new Exception("程序编写错误：BasicModRuntime在InitAsync之后，无法InstallMod");

            var di     = new DirectoryInfo(path);
            var normal = $"{BasicModConfig.RootDir}/{di.Name}";
            _DirMap[normal] = di.FullName;

            if (Directory.Exists(normal))
                Debug.LogWarning($"Mod[{di.Name}]在标准目录存在，使用外部目录覆盖[{path}]");
            else
                Debug.Log($"Mod[{di.Name}]安装成功 [{path}]");
        }

        private static bool _Inited;

        /// 使用例子：
        ///  BasicModRuntime.RegisterMod(@"D:\SVNGG\JYXModTest\FakeSteamMods\Icons");
        ///  await Addressables.InitializeAsync().Task;
        ///  await BasicModRuntime.InitModsAsync("Icons");
        public static async Task InitModsAsync(params string[] mods)
        {
            if (_Inited)
                throw new Exception("程序编写错误：BasicModRuntime.InitModsAsync无法重入");

            _Inited = true;
            foreach (var mod in mods)
            {
                var catalog = Redirect($"{BasicModConfig.RootDir}/{mod}/catalog.json");
                if (!File.Exists(catalog))
                    throw new Exception($"文件不存在，请检查MOD是否正常安装 [{catalog}]");

                await Addressables.LoadContentCatalogAsync(catalog).Task;
            }

            Debug.LogFormat("MOD初始化成功[{0}]", string.Join(", ", mods));
        }

        private static string Redirect(string path)
        {
            if (TryGetModName(path, out var modName, out var subPath))
            {
                if (!_Inited)
                    throw new Exception("程序编写错误：BasicModRuntime.InitModsAsync没有被调用");

                if (_DirMap.TryGetValue($"{BasicModConfig.RootDir}/{modName}", out var newPath))
                {
                    newPath += subPath;
                    if (BasicModConfig.Verbose)
                        Debug.Log($"[{path}] >> [{newPath}]");
                    return newPath;
                }
            }

            return path;
        }

        private static bool TryGetModName(string path, out string modName, out string subPath)
        {
            modName = subPath = null;
            if (!path.StartsWith(BasicModConfig.RootDir))
                return false;

            var sub    = path.Substring(BasicModConfig.RootDir.Length + 1);
            var index0 = sub.IndexOf("/", StringComparison.InvariantCulture);
            var index1 = sub.IndexOf("\\", StringComparison.InvariantCulture);
            var index  = Math.Min(index0 == -1 ? int.MaxValue : index0, index1 == -1 ? int.MaxValue : index1);
            if (index == int.MaxValue)
                return false;
            //找到第一个/或者第一个\ 也可以用split

            modName = sub.Substring(0, index);
            subPath = sub.Substring(index);
            return true;
        }
    }
}
