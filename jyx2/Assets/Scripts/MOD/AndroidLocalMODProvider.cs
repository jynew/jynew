#if UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using AndroidAuxiliary.Plugins.Auxiliary.AndroidBridge;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.Android;

namespace Jyx2.MOD
{
    public class AndroidMODProvider : MODProviderBase
    {
        private string ModDir => "/storage/emulated/0/Wuxia_MODs";

        public override async UniTask<Dictionary<string, ModItem>> GetInstalledMods()
        {
            if (!Directory.Exists(ModDir))
            {
                Debug.LogError("[AndroidLocalMODProvider] Mods Directory not found");
                Directory.CreateDirectory(ModDir);
                return null;
            }

            // 调用弹窗显示
            AndroidTools.ShowToast($"Mod读取路径为:{ModDir}");

            var modPaths = new List<string>();
            FileTools.GetAllFilePath(ModDir, modPaths, new List<string>()
            {
                ".xml"
            });
            if (modPaths.Count == 0)
            {
                Debug.LogError("[AndroidLocalMODProvider] Mod xml file not found");
                return null;
            }

            foreach (var modPath in modPaths)
            {
                var xmlObj = GetModItem(modPath);
                var modItem = new ModItem
                {
                    ModId = xmlObj.ModId,
                    Name = xmlObj.Name,
                    Version = xmlObj.Version,
                    Author = xmlObj.Author,
                    Description = xmlObj.Description,
                    Directory = xmlObj.Directory ?? Path.Combine(ModDir, xmlObj.ModId),
                    PreviewImageUrl = xmlObj.PreviewImageUrl
                };
                _items.Add(xmlObj.ModId.ToLower(), modItem);
            }

            return _items;
        }

        /// <summary>
        /// 初始化用于获取安卓权限
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void GetAndroidPermissions()
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
            // 获取文件权限
            AndroidTools.GetFileAccessPermission();
        }
    }
}
#endif