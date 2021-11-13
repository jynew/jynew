using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/地图", fileName = "地图ID_地图名")]
    public class Jyx2ConfigMap : Jyx2ConfigBase
    {
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

        public override async UniTask WarmUp()
        {
            
        }
    }
}
