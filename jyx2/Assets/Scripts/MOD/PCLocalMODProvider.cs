#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;

namespace Jyx2.MOD
{
    public class PCLocalMODProvider: MODProviderBase
    {
        private string ModDir
        {
            get
            {
#if UNITY_EDITOR
                if(!ResourceManagement.ResLoader.IsEditor())
                {
                    return Application.streamingAssetsPath;
                }
                return "mods";
#else
                return "mods";
#endif
            }
        }

        public override async UniTask GetInstalledMods()
        {
            if (!Directory.Exists(ModDir))
            {
                Debug.LogWarning("[PCLocalMODProvider] Mods Directory not found");
                Directory.CreateDirectory(ModDir);
                return;
            }
            List<string> modPaths = new List<string>();
            FileTools.GetAllFilePath(ModDir, modPaths, new List<string>()
            {
                ".xml"
            });
            if (modPaths.Count == 0)
            {
                Debug.LogError("[PCLocalMODProvider] Mod xml file not found");
                return;
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
                Items[xmlObj.ModId.ToLower()] = modItem;
            }
        }
    }
}
#endif