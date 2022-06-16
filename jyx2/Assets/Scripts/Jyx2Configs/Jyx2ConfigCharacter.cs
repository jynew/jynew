using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using UnityEngine;

namespace Jyx2Configs
{
    public class Jyx2ConfigCharacter : Jyx2ConfigBase
    {
        //性别
        public int Sexual;
        
        //头像
        public int Pic;
        
        public async UniTask<Sprite> GetPic()
        {
            var _sprite = await MODLoader.LoadAsset<Sprite>($"Assets/BuildSource/head/{Pic}.png");
            return _sprite;
        }

        //品德
        public int Pinde;

        //资质
        public int IQ;
        
        //生命上限
        public int MaxHp;
        
        //内力上限
        public int MaxMp;

        //生命增长
        public int HpInc;
        
        //开场等级
        public int Level;
        
        //经验
        public int Exp;
        
        //内力性质
        //0:阴 1:阳 2:调和
        public int MpType;
        
        //攻击力
        public int Attack;
        
        //轻功
        public int Qinggong;
        
        //防御力
        public int Defence;
        
        //医疗
        public int Heal;
        
        //用毒
        public int UsePoison;
        
        //解毒
        public int DePoison;
        
        //抗毒
        public int AntiPoison;
        
        //拳掌
        public int Quanzhang;
        
        //御剑
        public int Yujian;
        
        //耍刀
        public int Shuadao;
        
        //特殊兵器
        public int Qimen;
        
        //暗器技巧
        public int Anqi;
        
        //武学常识
        public int Wuxuechangshi;
        
        //攻击带毒
        public int AttackPoison;
        
        //左右互搏
        public int Zuoyouhubo;
        
        /* ------- 分割线 ------- */
        //所会武功
        public string Skills;

        /* ------- 分割线 --------*/
        //携带道具
        public string Items;

        //武器
        public int Weapon;
        
        //防具
        public int Armor;
        
        
        /* ------- 分割线 --------*/
        //队友离场对话
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

