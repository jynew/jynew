using Cysharp.Threading.Tasks;
using Jyx2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/地图", fileName = "地图ID_地图名")]
    public class Jyx2ConfigMap : Jyx2ConfigBase
    {
        public static Jyx2ConfigMap Get(int id)
        {
            return GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(id);
        }
        
        [InfoBox("引用指定场景asset")]
        [LabelText("地图")]
        public AssetReference MapScene;

        [LabelText("进门音乐")]
        public AssetReferenceT<AudioClip> InMusic;
        
        [LabelText("出门音乐")]
        public AssetReferenceT<AudioClip> OutMusic;

        [InfoBox("0开局开启  1开局关闭")]
        [LabelText("进入条件")] 
        public int EnterCondition;

        [LabelText("标签")] 
        public string Tags;

        [HideInInspector] public int ForceSetLeaveMusicId = -1;
        
        public override async UniTask WarmUp()
        {
            _isWorldMap = Tags.Contains("WORLDMAP");
        }
        
        public string GetShowName()
        {
            if ("小虾米居".Equals(Name)) return GameRuntimeData.Instance.Player.Name + "居";
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

    }
}
