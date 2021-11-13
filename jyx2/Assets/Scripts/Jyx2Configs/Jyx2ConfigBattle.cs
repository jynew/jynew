using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/战斗", fileName = "战斗ID")]
    public class Jyx2ConfigBattle : Jyx2ConfigBase
    {
        [InfoBox("引用指定战斗场景asset")]
        [LabelText("地图")]
        public AssetReference MapScene;

        [LabelText("获得经验")] 
        public int Exp;
        
        [LabelText("音乐")]
        public AssetReferenceT<AudioClip> Music; //音乐

        [BoxGroup("战斗人物设置")] [LabelText("队友")] [SerializeReference]
        public List<Jyx2ConfigCharacter> TeamMates;
        
        [BoxGroup("战斗人物设置")] [LabelText("自动队友")] [SerializeReference]
        public List<Jyx2ConfigCharacter> AudioTeamMates;
    }
}