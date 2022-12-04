#if UNITY_STANDALONE
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.Util;
using Steamworks;
using UnityEngine;

namespace Jyx2.MOD.ModV2
{
    /// <summary>
    /// steam创意工坊MOD
    /// </summary>
    public class GameModSteamWorkshop : GameModBase
    {
        public bool IsBroken;
        public string Dir;
        public Steamworks.Ugc.Item SteamItem;
            
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
            if (IsBroken && Info != null)
            {
                return $"[{Tag}]<color=red>老旧版本,可能无法启动, {SteamItem.Title}({Id})</color>";
            }
            else if (IsBroken && Info == null)
            {
                return $"[{Tag}]<color=red>已损坏, {SteamItem.Title}</color>";
            }
            else
            {
                return
                    $"[{Tag}] {Info.Name}({Id})";
            }
        }

        public override UniTask<Sprite> LoadPic()
        {
            var url = SteamItem.PreviewImageUrl;
            return url.DownloadSprite();
        }

        public override string GetContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"作者:{SteamItem.Owner.Name}");
            sb.AppendLine($"评分:{(int)(SteamItem.Score * 100)}/100");
            sb.AppendLine($"赞/踩:{SteamItem.VotesUp}/{SteamItem.VotesDown}");
            sb.AppendLine($"创建时间:{SteamItem.Created:yyyy/MM/dd HH:mm:ss}");
            sb.AppendLine($"更新时间:{SteamItem.Updated:yyyy/MM/dd HH:mm:ss}");
            sb.AppendLine($"订阅数:{SteamItem.NumSubscriptions}");
            if (SteamItem.Tags != null && SteamItem.Tags.Length > 0)
            {
                sb.AppendLine($"标签:{string.Join(",", SteamItem.Tags)}");
            }

            sb.AppendLine();
            sb.AppendLine($"{SteamItem.Description}");
            
            return sb.ToString();
        }

        public override string Title => SteamItem.Title;

        protected override string Tag => "创意工坊";
    }


    public class GameModSteamWorkshopLoader : GameModLoader
    {
        public const uint SteamAppId = 2098790;
        
        public override void Init()
        {
            try
            {
                if (!SteamClient.IsValid)
                    SteamClient.Init(SteamAppId);
            }
            catch (Exception e)
            {
                Debug.LogError("steam没有启动或登录，所以steamWorkShopLoader不可用。启动steam后重启游戏方可生效。");
            }
        }

        //适配老的创意工坊版本
        [XmlType("ModItem")]
        public class OldModItem
        {
            [XmlAttribute] public string ModId;
        }
        
        public override async UniTask<List<GameModBase>> LoadMods()
        {
            List<GameModBase> rst = new List<GameModBase>();
            if (!SteamClient.IsValid) return rst;
            
            var query = Steamworks.Ugc.Query.All.WhereUserSubscribed();
            var result = await query.GetPageAsync(1);
            foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
            {
                if (!entry.IsInstalled) continue;
                var dir = entry.Directory;
                
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
                    var content = File.ReadAllText(modPath).Trim();
                    if (string.IsNullOrEmpty(content)) continue;
                    var modInfo = Tools.DeserializeXML<GameModInfo>(content);
                    var mod = new GameModSteamWorkshop() {Dir = entry.Directory, SteamItem = entry};
                    if (modInfo == null)
                    {
                        //尝试看老版本是否能载入
                        var oldModItem = Tools.DeserializeXML<OldModItem>(content);
                        if (oldModItem != null)
                        {
                            mod.Info = new GameModInfo() {Id = oldModItem.ModId.ToLower()};
                        }
                        else
                        {
                            mod.Info = null;    
                        }

                        mod.IsBroken = true;
                    }
                    else
                    {
                        mod.Info = modInfo;
                    }
                    
                    rst.Add(mod);
                }
            }
            return rst;
        }
    }
}
#endif