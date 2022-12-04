using System;
using i18n.TranslatorDef;
using Jyx2;
using ProtoBuf;
using UnityEngine;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigMap : Jyx2ConfigBase
    {
        //地图
        [ProtoMember(1)]
        public string MapScene;

        //进门音乐
        [ProtoMember(2)]
        public int InMusic;
        
        //出门音乐
        [ProtoMember(3)]
        public int OutMusic;
        
        //跳转场景
        [ProtoMember(4)]
        public string TransportToMap;
        
        //进入条件
        //0-开局开启，1-开局关闭
        [ProtoMember(5)]
        public int EnterCondition;
        
        //标签
        [ProtoMember(6)]
        public string Tags;
        
        
        /// <summary>
        /// 获取标签数据，以半角的冒号分割
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string GetTagValue(string tag)
        {
            try
            {
                foreach (var tmp in Tags.Split(','))
                {
                    if (tmp.StartsWith(tag))
                    {
                        var s = tmp.Split(':');
                        if (s.Length == 2)
                        {
                            return s[1];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            return string.Empty;
        }
        
        /// <summary>
        /// 获取跳转场景数据，以半角的冒号分割
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string GetTransportToMapValue(string transportToMap)
        {
            try
            {
                foreach (var tmp in TransportToMap.Split(','))
                {
                    if (tmp.StartsWith(transportToMap))
                    {
                        var s = tmp.Split(':');
                        if (s.Length == 2)
                        {
                            return s[1];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            return string.Empty;
        }

        public int ForceSetLeaveMusicId = -1;

        public string GetShowName()
        {
            //---------------------------------------------------------------------------
            //if ("小虾米居".Equals(Name)) return GameRuntimeData.Instance.Player.Name + "居";
            //---------------------------------------------------------------------------
            //特定位置的翻译【小地图左上角的主角居显示】
            //---------------------------------------------------------------------------
            if (GlobalAssetConfig.Instance.defaultHomeName.Equals(Name)) return GameRuntimeData.Instance.Player.Name + "居".GetContent(nameof(Jyx2ConfigMap));
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            return Name;
        }
        
        //获得开场地图
        public static Jyx2ConfigMap GetGameStartMap()
        {
            foreach(var map in GameConfigDatabase.Instance.GetAll<Jyx2ConfigMap>())
            {
                if (map.Tags.Contains("START"))
                {
                    return map;
                }
            }
            return null;
        }
        
        public static Jyx2ConfigMap GetMapBySceneName(string sceneName)
        {
            foreach(var map in GameConfigDatabase.Instance.GetAll<Jyx2ConfigMap>())
            {
                if (map.MapScene.Equals(sceneName))
                {
                    return map;
                }
            }
            return null;
        }
        
        public static Jyx2ConfigMap GetMapByName(string name)
        {
            foreach(var map in GameConfigDatabase.Instance.GetAll<Jyx2ConfigMap>())
            {
                if (map.Name.Equals(name))
                {
                    return map;
                }
            }
            return null;
        }
    }
}
