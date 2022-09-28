using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;

namespace Jyx2.MOD
{
    public abstract class MODProviderBase
    {
        //名字
        public string Name;
        
        //xml文件名
        public string XmlFileName = "mod.xml";
        
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
        
        //获取所有安装的Mod
        public virtual UniTask<Dictionary<string, ModItem>> GetInstalledMods() { return new UniTask<Dictionary<string, ModItem>>(); }
        
        //获取Mod文件夹路径
        public virtual string GetModDirPath(string modId) { return ""; }
        
        /// <summary>
        /// 获取ModItem
        /// </summary>
        /// <param name="modPath"></param>
        /// <returns></returns>
        public virtual ModItem GetModItem(string modPath)
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
        
        //加载指定的Mod
        public virtual UniTask LoadMod(string modId) { return new UniTask(); }
    }
}