using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("jyx2role")]
    public class Jyx2Role : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public int Head; //头像

        [XmlAttribute]
        public int HpInc; //生命增长

        [XmlAttribute]
        public string Name; //姓名

        [XmlAttribute]
        public string Waihao; //外号

        [XmlAttribute]
        public int Sex; //性别

        [XmlAttribute]
        public int Level; //等级

        [XmlAttribute]
        public int Exp; //经验

        [XmlAttribute]
        public int Hp;

        [XmlAttribute]
        public int MaxHp;

        [XmlAttribute]
        public int DamageLevel; //受伤程度

        [XmlAttribute]
        public int PoisonLevel; //中毒程度

        [XmlAttribute]
        public int Tili; //体力

        [XmlAttribute]
        public int Xiulian; //物品修炼点

        [XmlAttribute]
        public int Weapon; //武器

        [XmlAttribute]
        public int Armor; //防具

        [XmlAttribute]
        public int MpType; //内力性质

        [XmlAttribute]
        public int Mp;

        [XmlAttribute]
        public int MaxMp;

        [XmlAttribute]
        public int Attack; //攻击力

        [XmlAttribute]
        public int Qinggong; //轻功

        [XmlAttribute]
        public int Defence; //防御力

        [XmlAttribute]
        public int Heal; //医疗

        [XmlAttribute("Poison")]
        public int UsePoison; //用毒

        [XmlAttribute]
        public int DePoison; //解毒

        [XmlAttribute]
        public int AntiPoison; //抗毒

        [XmlAttribute]
        public int Quanzhang; //拳掌

        [XmlAttribute]
        public int Yujian; //御剑

        [XmlAttribute]
        public int Shuadao; //耍刀

        [XmlAttribute]
        public int Qimen;//特殊兵器

        [XmlAttribute]
        public int Anqi; //暗器技巧

        [XmlAttribute]
        public int Wuxuechangshi; //武学常识

        [XmlAttribute]
        public int Pinde; //品德

        [XmlAttribute]
        public int AttackPoison; //攻击带毒

        [XmlAttribute]
        public int Zuoyouhubo; //左右互搏

        [XmlAttribute]
        public int Shengwang; //声望

        [XmlAttribute("Zizhi")]
        public int IQ; //资质

        [XmlAttribute]
        public int Xiulianwupin; //修炼物品

        [XmlAttribute]
        public int XiulianPoint; //修炼点数
		
		//added by eaphone at 2021/6/6
        [XmlAttribute]
        public string Dialogue; //离队对话

        [XmlElement("Wugongs")]
        public List<Jyx2RoleWugong> Wugongs; //武功

        [XmlElement("Items")]
        public List<Jyx2RoleItem> Items; //武功

		
        //立绘
        public string GetHeadAvata()
        {
            return ConfigTable.Get<Jyx2RoleHeadMapping>(Head).HeadAvata;
        }

        //模型配置
        public string GetModelAsset()
        {
            return ConfigTable.Get<Jyx2RoleHeadMapping>(Head).ModelAsset;
        }
        
        //模型
        public string GetModel()
        {
            return ConfigTable.Get<Jyx2RoleHeadMapping>(Head).Model;
        }

        public string GetWeaponMount()
        {
            return ConfigTable.Get<Jyx2RoleHeadMapping>(Head).WeaponMount;
        }

        public string GetBattleAnimator()
        {
            return ConfigTable.Get<Jyx2RoleHeadMapping>(Head).BattleAnimator;
        }

        //待适配
        public string Tag = "";
    }

    [XmlType("jyx2role_headMapping")]
    public class Jyx2RoleHeadMapping : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id;

        //立绘
        [XmlAttribute]
        public string HeadAvata;

        //模型配置
        [XmlAttribute]
        public string ModelAsset;
        
        //模型
        [XmlAttribute]
        public string Model;

        //武器挂载
        [XmlAttribute]
        public string WeaponMount;

        //战斗动作集
        [XmlAttribute]
        public string BattleAnimator;
    }

    [XmlType]
    public class Jyx2RoleWugong
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public int Level;
    }

    [XmlType]
    public class Jyx2RoleItem
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public int Count;

        public Jyx2RoleItem Clone()
        {
            return new Jyx2RoleItem() { Id = this.Id, Count = this.Count };
        }
    }
}
