
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//WeaponType映射表
/*
 * BigSword 0
 * Bow 1
 * Dagger 2 匕首  -跑动动作用剑的
 * DoubleKnife 3
 * Gudgel 4
 * Gun 5
 * HFH 6（黄飞鸿？)
 * HidWea 7 暗器 -跑动动作用空手
 * Leg 8  -跑动动作用空手的，受击也是
 * Lute 9 琴/琵琶
 * Palm 10
 * Scourge 11 鞭子
 * Shield 12 盾牌
 * Sinknif 13 单刀
 * SinSword 14 长剑
 * Spear 15 长矛
 * 
 * 
 */
namespace Jyx2
{
    public enum WeaponAnimationCode
    {
        BigSword = 0,
        Bow = 1,
        Dagger = 2,
        DoubleKnife = 3,
        Cudgel = 4,
        Gun = 5,
        HFH = 6,
        HidWea = 7,
        Leg = 8,
        Lute = 9,
        Palm = 10,
        Scourge = 11,
        Shield = 12,
        Sinknif = 13,
        SinSword = 14,
        Spear = 15,
    }
}
