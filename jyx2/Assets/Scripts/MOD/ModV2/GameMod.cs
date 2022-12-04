using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2.MOD.ModV2
{
    /// <summary>
    /// 游戏MOD基类
    ///
    /// </summary>
    public abstract class GameModBase
    {
        public string Id => Info.Id;
        public virtual string Title => Info.Name;
        
        public GameModInfo Info;
        public abstract UniTask<AssetBundle> LoadModAb();
        public abstract UniTask<AssetBundle> LoadModMap();

        public virtual async UniTask<Sprite> LoadPic()
        {
            return Resources.Load<Sprite>("icon");
        }

        public virtual string GetContent()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<color=green>来源:{Tag}</color>");
            sb.AppendLine($"ID:{Info.Id}");
            sb.AppendLine($"作者:{Info.Author}");
            sb.AppendLine($"版本:{Info.Version}");
            sb.AppendLine($"匹配客户端版本:{Info.ClientVersion}");
            sb.AppendLine($"更新时间:{Info.CreateTime}");
            sb.AppendLine($"简介:{Info.Desc}");


            return sb.ToString();
        }

        public virtual string GetDesc()
        {
            return
                $"[{Tag}]{Info.Name}({Id})";
        }

        protected abstract string Tag { get; }

    }

    /// <summary>
    /// 游戏MOD加载器基类
    /// </summary>
    public abstract class GameModLoader
    {
        public abstract UniTask<List<GameModBase>> LoadMods();

        public virtual void Init()
        {
            //do nothing..
        }
    }
    

    /// <summary>
    /// 游戏MOD基础信息
    /// </summary>
    [XmlType]
    public class GameModInfo
    {
        [XmlAttribute] public string Id;
        [XmlAttribute] public string Name;
        [XmlAttribute] public string Author;
        [XmlAttribute] public string Version;
        [XmlAttribute] public string ClientVersion;
        [XmlAttribute] public string ModMD5;
        [XmlAttribute] public string MapsMD5;
        [XmlAttribute] public string CreateTime;
        [XmlAttribute] public string Desc;
        [XmlAttribute] public string Welcome;
    }
}