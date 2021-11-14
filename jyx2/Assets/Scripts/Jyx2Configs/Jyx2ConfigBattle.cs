using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/战斗", fileName = "战斗ID")]
    public class Jyx2ConfigBattle : Jyx2ConfigBase
    {
        public static Jyx2ConfigBattle Get(int id)
        {
            return GameConfigDatabase.Instance.Get<Jyx2ConfigBattle>(id);
        }
        
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
        public List<Jyx2ConfigCharacter> AutoTeamMates;
        
        [BoxGroup("战斗人物设置")] [LabelText("敌人")] [SerializeReference]
        public List<Jyx2ConfigCharacter> Enemies;

        public override async UniTask WarmUp()
        {
            
        }
    }
}