using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Jyx2.MOD.ModV2
{
    /// <summary>
    /// 游戏MOD基类
    ///
    /// </summary>
    public abstract class GameModBase
    {
        public string Id => Info.Id;
        public GameModInfo Info;
        public abstract UniTask<AssetBundle> LoadModAb();
        public abstract UniTask<AssetBundle> LoadModMap();

        public virtual string GetDesc()
        {
            return
                $"[{Tag}]{Id} {Info.Name} 作者:{Info.Author} 版本:{Info.Version} 匹配客户端版本:{Info.ClientVersion} 更新时间:{Info.CreateTime}";
        }

        protected abstract string Tag { get; }

    }

    /// <summary>
    /// 游戏MOD加载器基类
    /// </summary>
    public abstract class GameModLoader
    {
        public abstract UniTask<List<GameModBase>> LoadMods();
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
    }
}