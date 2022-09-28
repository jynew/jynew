using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.ResourceManagement;
using Sirenix.Utilities;
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
        
        private Dictionary<string, ModItem> _items = new Dictionary<string, ModItem>();

        /// <summary>
        /// 获取已安装的Mod列表
        /// </summary>
        /// <returns></returns>
        public override async UniTask<Dictionary<string, ModItem>> GetInstalledMods()
        {
            if (SteamClient.IsValid)
            {
                var query = Steamworks.Ugc.Query.All.WhereUserSubscribed();
                var result = await query.GetPageAsync(1);
                
                Debug.Log($"Found {result.Value.TotalCount} subscribed items");
                
                foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
                {
                    if (entry.IsInstalled)
                    {
                        Debug.Log($"Found Installed Item: {entry.Title}");
                        
                        var modPath = Path.Combine(entry.Directory, XmlFileName);
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
                        _items.Add(xmlObj.ModId, modItem);
                    }
                }
            }
            return _items;
        }

        /// <summary>
        /// 获取Mod文件夹路径
        /// </summary>
        /// <param name="modId"></param>
        /// <returns></returns>
        public override string GetModDirPath(string modId)
        {
            if (_items.ContainsKey(modId))
            {
                return _items[modId].Directory;
            }
            return "";
        }
        
        /// <summary>
        /// 加载Mod
        /// </summary>
        /// <param name="modId"></param>
        public override async UniTask LoadMod(string modId)
        {
            var modDirPath = GetModDirPath(modId);
            await ResLoader.LoadMod(modId, modDirPath);
        }
    }
}