using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;

namespace Jyx2.MOD.ModV2
{
    /// <summary>
    /// 手动安装的MOD
    /// </summary>
    public class GameModManualInstalled : GameModBase
    {
        public bool IsBroken;
        public string Dir;
        public override async UniTask<AssetBundle> LoadModAb()
        {
            var path = Path.Combine(Dir, $"{Info.Id.ToLower()}_mod");
            return await AssetBundle.LoadFromFileAsync(path);
        }

        public override async UniTask<AssetBundle> LoadModMap()
        {
            var path = Path.Combine(Dir, $"{Info.Id.ToLower()}_maps");
            return await AssetBundle.LoadFromFileAsync(path);
        }

        public override string GetDesc()
        {
            return base.GetDesc();
        }

        protected override string Tag => "手动安装";
    }
    
    public class GameModManualInstalledLoader : GameModLoader
    {
        //手动安装的目录列表
        public string[] ManualInstalledDir => new[]
        {
#if UNITY_STANDALONE_WIN
            Path.Combine(Application.dataPath, "mods"),
#endif

#if UNITY_ANDROID
            Path.Combine("/storage/emulated/0/", "jynew/mods"),
#endif
            Path.Combine(Application.persistentDataPath, "mods")
        };

        public override async UniTask<List<GameModBase>> LoadMods()
        {
            List<GameModBase> rst = new List<GameModBase>();
            foreach (var dir in ManualInstalledDir)
            {
                if (!Directory.Exists(dir)) continue;

                List<string> modPaths = new List<string>();
                FileTools.GetAllFilePath(dir, modPaths, new List<string>()
                        {
                        ".xml"
                        });

                if (modPaths.Count == 0)
                {
                    continue;
                }

                foreach (var modPath in modPaths)
                {
                    var content = File.ReadAllText(modPath).Trim();
                    if (string.IsNullOrEmpty(content)) continue;
                    var modInfo = Tools.DeserializeXML<GameModInfo>(content);
                    if (modInfo == null) continue;



                    var mod = new GameModManualInstalled() {Info = modInfo, Dir = Directory.GetParent(modPath)?.FullName};
                    rst.Add(mod);
                }
            }

            return rst;
        }

        //暂时没有使用，以后看看需求
        private static string GetAndroidExternalFilesDir()
        {
            using (AndroidJavaClass unityPlayer = 
                    new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject context = 
                        unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Get all available external file directories (emulated and sdCards)
                    AndroidJavaObject[] externalFilesDirectories = 
                        context.Call<AndroidJavaObject[]>
                        ("getExternalFilesDirs", (object)null);
                    AndroidJavaObject emulated = null;
                    AndroidJavaObject sdCard = null;
                    for (int i = 0; i < externalFilesDirectories.Length; i++)
                    {
                        AndroidJavaObject directory = externalFilesDirectories[i];
                        using (AndroidJavaClass environment = 
                                new AndroidJavaClass("android.os.Environment"))
                        {
                            // Check which one is the emulated and which the sdCard.
                            bool isRemovable = environment.CallStatic<bool>
                                ("isExternalStorageRemovable", directory);
                            bool isEmulated = environment.CallStatic<bool>
                                ("isExternalStorageEmulated", directory);
                            if (isEmulated)
                                emulated = directory;
                            else if (isRemovable && isEmulated == false)
                                sdCard = directory;
                        }
                    }
                    // Return the sdCard if available
                    if (sdCard != null)
                        return sdCard.Call<string>("getAbsolutePath");
                    else
                        return emulated.Call<string>("getAbsolutePath");
                }
            }
        }
    }
}
