using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2.ResourceManagement;
using UnityEngine;

namespace Jyx2.MOD
{
    public abstract class MODProviderBase
    {
        //名字
        public string Name;

        //xml类
        [XmlType]
        public class ModItem
        {
            [XmlAttribute]
            public string ModId;
            
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public string Version;
            
            [XmlAttribute]
            public string Author;
            
            [XmlAttribute]
            public string Description;
            
            [XmlAttribute]
            public string Directory;
            
            [XmlAttribute]
            public string PreviewImageUrl;
        }

        public static Dictionary<string, ModItem> Items { get; set; } = new Dictionary<string, ModItem>();
        
        //获取所有安装的Mod
        public virtual async UniTask GetInstalledMods() {}

        /// <summary>
        /// 获取Mod文件夹路径
        /// </summary>
        /// <param name="modId"></param>
        /// <returns></returns>
        public string GetModDirPath(string modId)
        {
            modId = modId.ToLower();
            if (Items.ContainsKey(modId))
            {
                return Items[modId].Directory;
            }
            return "";
        }
        
        /// <summary>
        /// 获取ModItem
        /// </summary>
        /// <param name="modPath"></param>
        /// <returns></returns>
        public ModItem GetModItem(string modPath)
        {
            ModItem modItem = new ModItem();
            
            if (File.Exists(modPath))
            {
                var xmlContent = File.ReadAllText(modPath);
         
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    try
                    {
                        modItem = Tools.DeserializeXML<ModItem>(xmlContent);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
            return modItem;
        }
        
        /// <summary>
        /// 加载指定的Mod
        /// </summary>
        /// <param name="modId"></param>
        public async UniTask LoadMod(string modId)
        {
            var modDirPath = GetModDirPath(modId);
            await ResLoader.LoadMod(modId, modDirPath);
        }
    }
}