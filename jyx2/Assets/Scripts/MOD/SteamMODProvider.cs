using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.ResourceManagement;
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
        
        private Dictionary<string, Steamworks.Ugc.Item> _items = new Dictionary<string, Steamworks.Ugc.Item>();

        /// <summary>
        /// 获取安装的MOD列表
        /// </summary>
        /// <returns></returns>
        public override async UniTask<List<string>> GetInstalledMods()
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
                        _items.Add(entry.Title, entry);
                    }
                }
            }
            return _items.Keys.ToList();
        }
        
        /// <summary>
        /// 获取MOD的文件夹路径
        /// </summary>
        /// <param name="modName"></param>
        /// <returns></returns>
        public string GetModPath(string modName)
        {
            if (_items.ContainsKey(modName))
            {
                return _items[modName].Directory;
            }
            return null;
        }
 
        /// <summary>
        /// 加载MOD
        /// </summary>
        /// <param name="modId"></param>
        public override async UniTask LoadMod(string modId)
        {
            await ResLoader.LoadMod(modId, GetModPath(modId));
        }
    }
}