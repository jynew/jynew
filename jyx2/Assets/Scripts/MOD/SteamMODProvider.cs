#if !UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Steamworks;
using UnityEngine;

namespace Jyx2.MOD
{
    public class SteamMODProvider: MODProviderBase
    {
        public SteamMODProvider()
        {
            try
            {
                SteamClient.Init(2098790);
            } 
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 获取已安装的Mod列表
        /// </summary>
        /// <returns></returns>
        public override async UniTask GetInstalledMods()
        {
            if (SteamClient.IsValid)
            {
                var query = Steamworks.Ugc.Query.All.WhereUserSubscribed();
                var result = await query.GetPageAsync(1);
                
                Debug.Log($"[SteamMODProvider] Found {result.Value.TotalCount} subscribed items");
                
                foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
                {
                    if (entry.IsInstalled)
                    {
                        Debug.Log($"[SteamMODProvider] Found Installed Item: {entry.Title}");
                        
                        List<string> modPaths = new List<string>();
                        FileTools.GetAllFilePath(entry.Directory, modPaths, new List<string>()
                        {
                            ".xml"
                        });
                        if (modPaths.Count == 0)
                        {
                            Debug.LogWarning("[SteamMODProvider] Mod xml file not found, ModName:" + entry.Title);
                            continue;
                        }
                        foreach (var modPath in modPaths)
                        {
                            var xmlObj = GetModItem(modPath);
                            var modItem = new ModItem
                            {
                                ModId = xmlObj.ModId,
                                Name = entry.Title ?? xmlObj.Name,
                                Version = xmlObj.Version,
                                Author = entry.Owner.Name ?? xmlObj.Author,
                                Description = entry.Description ?? xmlObj.Description,
                                Directory = entry.Directory,
                                PreviewImageUrl = entry.PreviewImageUrl ?? xmlObj.PreviewImageUrl
                            };
                            Items[xmlObj.ModId.ToLower()] = modItem;
                        }
                    }
                }
            }
        }
    }
}
#endif