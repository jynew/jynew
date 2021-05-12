using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2
{
    /// <summary>
    /// 属性获取
    /// </summary>
    public class AttributeHelper
    {
        public AttributeHelper(RoleInstance Owner)
        {
            owner = Owner;
        }

        private RoleInstance owner = null;

        public int this[string key]
        {
            get
            {
                if (owner != null)
                {
                    switch (key)
                    {
                        case "hp":
                            return owner.Hp;
                        //case "maxhp":
                        //    {
                        //        int initialHP = (int)(owner.Data.MaxHp + LevelFactor.GetFactor(owner.Level) * owner.Data.GetGrowTemplate().HpFactor);
                        //        int maxhp = initialHP;

                        //        if (owner.Data.Tag != null && owner.Data.Tag == "BOSS")
                        //        {
                        //            maxhp *= 5;
                        //        }
                        //        else if (owner.Data.Tag != null && owner.Data.Tag == "NPC")
                        //        {
                        //            maxhp = (int)(maxhp * 0.5);
                        //        }
                        //        return maxhp;
                        //    }
                        //case "mp":
                        //    return owner.mp;
                        //case "maxmp":
                        //    {
                        //        int maxmp = (int)(owner.RoleData.maxmp + (owner.level - 1) * owner.RoleData.GetGrowTemplate().Mp);
                        //        if (owner.RoleData.tag != null && owner.RoleData.tag == "BOSS")
                        //        {
                        //            maxmp *= 15;
                        //        }
                        //        else if (owner.RoleData.tag != null && owner.RoleData.tag == "NPC")
                        //        {
                        //            maxmp = (int)(maxmp * 0.8);
                        //        }
                        //        return maxmp;
                        //    }
                        //case "gengu":
                        //    return (int)(owner.Data.Gengu + (owner.Level - 1) * owner.Data.GetGrowTemplate().Gengu);
                        //case "bili":
                        //    return (int)(owner.Data.Bili + (owner.Level - 1) * owner.Data.GetGrowTemplate().Bili);
                        //case "fuyuan":
                        //    return (int)(owner.Data.Fuyuan + (owner.Level - 1) * owner.Data.GetGrowTemplate().Fuyuan);
                        //case "shenfa":
                        //    return (int)(owner.Data.Shenfa + (owner.Level - 1) * owner.Data.GetGrowTemplate().Shenfa);
                        //case "dingli":
                        //    return (int)(owner.Data.Dingli + (owner.Level - 1) * owner.Data.GetGrowTemplate().Dingli);
                        //case "wuxing":
                        //    return (int)(owner.Data.Wuxing + (owner.Level - 1) * owner.Data.GetGrowTemplate().Wuxing);
                        //case "quanzhang":
                        //    return (int)(owner.Data.Quanzhang + (owner.Level - 1) * owner.Data.GetGrowTemplate().Quanzhang);
                        //case "jianfa":
                        //    return (int)(owner.Data.Jianfa + (owner.Level - 1) * owner.Data.GetGrowTemplate().Jianfa);
                        //case "daofa":
                        //    return (int)(owner.Data.Daofa + (owner.Level - 1) * owner.Data.GetGrowTemplate().Daofa);
                        //case "qimen":
                        //    return (int)(owner.Data.Qimen + (owner.Level - 1) * owner.Data.GetGrowTemplate().Qimen);
                        case "sex":
                            return owner.Data.Sex;
                        //case "wuxue":
                        //    return owner.wuxue;
                        default:
                            return 0;
                    }
                }
                return 0;
            }
        }
    }

    /// <summary>
    /// 属性后处理
    /// </summary>
    public class AttributeFinalHelper
    {
        public AttributeFinalHelper(RoleInstance Owner)
        {
            owner = Owner;
        }

        private RoleInstance owner = null;

        public int this[string key]
        {
            get
            {
                //int rst = owner.Attributes[key] + owner.GetAdditionAttribute(key);
                //if (rst <= 0)
                //    rst = 0;
                //return rst;
                return 0;
            }
        }
    }
}
