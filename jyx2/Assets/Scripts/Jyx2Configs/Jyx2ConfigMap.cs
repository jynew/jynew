using System;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;

namespace Jyx2Configs
{
    public class Jyx2ConfigMap : Jyx2ConfigBase
    {
        public static Jyx2ConfigMap Get(int id)
        {
            
            return GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(id);
        }
        
        //地图
        public string MapScene;

        //进门音乐
        public int InMusic;
        
        //出门音乐
        public int OutMusic;
        
        //进入条件
        //0-开局开启，1-开局关闭
        public int EnterCondition;
        
        //标签
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

        public int ForceSetLeaveMusicId = -1;
        
        public override async UniTask WarmUp()
        {
            _isWorldMap = Tags.Contains("WORLDMAP");
            _isNoNavAgent = Tags.Contains("NONAVAGENT");
        }
        
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
        
        /// <summary>
        /// 是否是大地图
        /// </summary>
        /// <returns></returns>
        public bool IsWorldMap() { return _isWorldMap;}
        private bool _isWorldMap;
        
        /// <summary>
        /// 是否不能寻路
        /// </summary>
        /// <returns></returns>
        public bool IsNoNavAgent() { return _isNoNavAgent;}
        private bool _isNoNavAgent;
    }
}
