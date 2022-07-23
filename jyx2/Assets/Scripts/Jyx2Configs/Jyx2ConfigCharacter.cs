using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using ProtoBuf;
using UnityEngine;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigCharacter : Jyx2ConfigBase
    {
        //性别
        [ProtoMember(1)]
        public int Sexual;
        
        //头像
        [ProtoMember(2)]
        public int Pic;
        
        public async UniTask<Sprite> GetPic()
        {
            var _sprite = await ResLoader.LoadAsset<Sprite>($"BuildSource/head/{Pic}.png");
            return _sprite;
        }

        //品德
        [ProtoMember(3)]
        public int Pinde;

        //资质
        [ProtoMember(4)]
        public int IQ;
        
        //生命上限
        [ProtoMember(5)]
        public int MaxHp;
        
        //内力上限
        [ProtoMember(6)]
        public int MaxMp;

        //生命增长
        [ProtoMember(7)]
        public int HpInc;
        
        //开场等级
        [ProtoMember(8)]
        public int Level;
        
        //经验
        [ProtoMember(9)]
        public int Exp;
        
        //内力性质
        //0:阴 1:阳 2:调和
        [ProtoMember(10)]
        public int MpType;
        
        //攻击力
        [ProtoMember(11)]
        public int Attack;
        
        //轻功
        [ProtoMember(12)]
        public int Qinggong;
        
        //防御力
        [ProtoMember(13)]
        public int Defence;
        
        //医疗
        [ProtoMember(14)]
        public int Heal;
        
        //用毒
        [ProtoMember(15)]
        public int UsePoison;
        
        //解毒
        [ProtoMember(16)]
        public int DePoison;
        
        //抗毒
        [ProtoMember(17)]
        public int AntiPoison;
        
        //拳掌
        [ProtoMember(18)]
        public int Quanzhang;
        
        //御剑
        [ProtoMember(19)]
        public int Yujian;
        
        //耍刀
        [ProtoMember(20)]
        public int Shuadao;
        
        //特殊兵器
        [ProtoMember(21)]
        public int Qimen;
        
        //暗器技巧
        [ProtoMember(22)]
        public int Anqi;
        
        //武学常识
        [ProtoMember(23)]
        public int Wuxuechangshi;
        
        //攻击带毒
        [ProtoMember(24)]
        public int AttackPoison;
        
        //左右互搏
        [ProtoMember(25)]
        public int Zuoyouhubo;
        
        /* ------- 分割线 ------- */
        //所会武功
        [ProtoMember(26)]
        public string Skills;

        /* ------- 分割线 --------*/
        //携带道具
        [ProtoMember(27)]
        public string Items;

        //武器
        [ProtoMember(28)]
        public int Weapon;
        
        //防具
        [ProtoMember(29)]
        public int Armor;
        
        
        /* ------- 分割线 --------*/
        //队友离场对话
        [ProtoMember(30)]
        public string LeaveStoryId;

        /* ------- 分割线 --------*/
        //模型配置
        public ModelAsset Model
        {
            get
            {
                return ModelAsset.Get(Name);
            }
        }
    }

    public class Jyx2ConfigCharacterSkill
    {
        //ID
        public int Id;
        
        //等级
        public int Level;
    }
    
    public class Jyx2ConfigCharacterItem
    {
        //ID
        public int Id;
        
        //数量
        public int Count;
    }
}

