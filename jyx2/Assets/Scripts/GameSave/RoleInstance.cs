/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System;
using System.Collections.Generic;
using IFix;
using Jyx2.Middleware;
using UnityEngine;
using UniRx;
using NUnit.Framework;
using Random = UnityEngine.Random;
using XLua;

namespace Jyx2
{
    [Serializable]
    public class RoleInstance : IComparable<RoleInstance>
    {
    #region 存档数据定义
        [SerializeField] public int Key; //ID
        [SerializeField] public string Name; //姓名

        [SerializeField] public int Sex; //性别
        [SerializeField] public int Level = 1; //等级
        [SerializeField] public int Exp; //经验

        [SerializeField] public int Attack; //攻击力
        [SerializeField] public int Qinggong; //轻功
        [SerializeField] public int Defence; //防御力
        [SerializeField] public int Heal; //医疗
        [SerializeField] public int UsePoison; //用毒
        [SerializeField] public int DePoison; //解毒
        [SerializeField] public int AntiPoison; //抗毒
        [SerializeField] public int Quanzhang; //拳掌
        [SerializeField] public int Yujian; //御剑
        [SerializeField] public int Shuadao; //耍刀
        [SerializeField] public int Qimen; //特殊兵器
        [SerializeField] public int Anqi; //暗器技巧
        [SerializeField] public int Wuxuechangshi; //武学常识
        [SerializeField] public int Pinde; //品德
        [SerializeField] public int AttackPoison; //攻击带毒
        [SerializeField] public int Zuoyouhubo; //左右互搏
        [SerializeField] public int Shengwang; //声望
        [SerializeField] public int IQ; //资质
        [SerializeField] public int HpInc; //生命增长


        [SerializeField] public int ExpForItem; //修炼点数
        [SerializeField] public List<SkillInstance> Wugongs = new List<SkillInstance>(); //武功
        [SerializeField] public List<CsRoleItem> Items = new List<CsRoleItem>(); //道具

        [SerializeField] public int Mp;
        [SerializeField] public int MaxMp;
        [SerializeField] public int MpType; //内力性质
        [SerializeField] public int Hp;
        [SerializeField] public int MaxHp;
        [SerializeField] public int Hurt; //受伤程度
        [SerializeField] public int Poison; //中毒程度
        [SerializeField] public int Tili; //体力
        [SerializeField] public int ExpForMakeItem; //物品修炼点

        [SerializeField] public int Weapon; //武器
        [SerializeField] public int Armor; //防具
        [SerializeField] public int Xiulianwupin = -1; //修炼物品


        [SerializeField] public int CurrentSkill = 0; //当前技能
        #endregion

        public RoleInstance()
        {
        }

        public RoleInstance(int roleId)
        {
            Key = roleId;
            BindKey();
            InitData();
            Recover();
        }

        public void BindKey()
        {
            _data = LuaToCsBridge.CharacterTable[Key];

            if (_data == null)
            {
                Assert.Fail("无法获取角色配置，配置不存在，Id:" + Key);
            }

            //初始化武功列表，只有武功数量为0时才需要初始化
            if (Wugongs.Count == 0)
            {
                foreach (var _skill in _data.Skills)
                {
                    Wugongs.Add(new SkillInstance(_skill));
                }
            }

            //每次战斗前reset一次
            ResetForBattle();
        }


#if !INJECTFIX_PATCH_ENABLE
        void InitData()
        {
            //CG 初始化
            Name = Data.Name;
            Sex = (int)Data.Sexual;
            Level = Data.Level;
            Exp = Data.Exp;
            Hp = Data.MaxHp;
            PreviousRoundHp = Hp;
            MaxHp = Data.MaxHp;
            Mp = Data.MaxMp;
            MaxMp = Data.MaxMp;
            Tili = GameConst.MAX_ROLE_TILI;
            Weapon = Data.Weapon;
            Armor = Data.Armor;
            MpType = Data.MpType;
            Attack = Data.Attack;
            Qinggong = Data.Qinggong;
            Defence = Data.Defence;
            Heal = Data.Heal;
            UsePoison = Data.UsePoison;
            DePoison = Data.DePoison;
            AntiPoison = Data.AntiPoison;
            Quanzhang = Data.Quanzhang;
            Yujian = Data.Yujian;
            Shuadao = Data.Shuadao;
            Qimen = Data.Qimen;
            Anqi = Data.Anqi;
            Wuxuechangshi = Data.Wuxuechangshi;
            Pinde = Data.Pinde;
            AttackPoison = Data.AttackPoison;
            Zuoyouhubo = Data.Zuoyouhubo;
            IQ = Data.IQ;
            HpInc = Data.HpInc;

            ResetItems();
        }
#else
        [IFix.Patch]
        void InitData()
        {
            //CG 初始化
            Name = Data.Name;
            Sex = (int)Data.Sexual;
            Level = Data.Level;
            Exp = Data.Exp;
            Hp = Data.MaxHp;
            PreviousRoundHp = Hp;
            MaxHp = Data.MaxHp;
            Mp = Data.MaxMp;
            MaxMp = Data.MaxMp + 1000;
            Tili = GameConst.MAX_ROLE_TILI;
            Weapon = Data.Weapon;
            Armor = Data.Armor;
            MpType = Data.MpType;
            Attack = Data.Attack;
            Qinggong = Data.Qinggong;
            Defence = Data.Defence;
            Heal = Data.Heal;
            UsePoison = Data.UsePoison;
            DePoison = Data.DePoison;
            AntiPoison = Data.AntiPoison;
            Quanzhang = Data.Quanzhang;
            Yujian = Data.Yujian;
            Shuadao = Data.Shuadao;
            Qimen = Data.Qimen;
            Anqi = Data.Anqi;
            Wuxuechangshi = Data.Wuxuechangshi;
            Pinde = Data.Pinde;
            AttackPoison = Data.AttackPoison;
            Zuoyouhubo = Data.Zuoyouhubo;
            IQ = Data.IQ;
            HpInc = Data.HpInc;

            ResetItems();
        }
#endif

        public void ResetForBattle()
        {
            ResetSkillCasts();
            //停止在BindKey时刷新物品
            //ResetItems();
        }

        public void Recover()
        {
            SetHPAndRefreshHudBar(MaxHp);

            Mp = MaxMp;
            Tili = GameConst.MAX_ROLE_TILI;

            Hurt = 0;
            Poison = 0;
        }

        public int GetJyx2RoleId()
        {
            return Key;
        }
        public string GetPic()
        {
            return $"BuildSource/head/{Data.Pic}.png";
        }
        //模型配置
        public ModelAsset Model
        {
            get
            {
                //如果配置了，则从配置表读取，否则根据名字读取
                if (!string.IsNullOrWhiteSpace(Data.ModelFileKey))
                {
                    return ModelAsset.Get(Data.ModelFileKey);
                }
                else
                {
                    return ModelAsset.Get(Data.Name);    
                }
            }
        }
        #region JYX2等级相关



        //JYX2
        public bool CanLevelUp()
        {
            if (this.Level >= 1 && this.Level < GameConst.MAX_ROLE_LEVEL)
            {
                if (this.Exp >= getLevelUpExp(this.Level))
                {
                    return true;
                }
            }

            return false;
        }

        int getLevelUpExp(int level)
        {
            //防止越界，限制下
            level = Mathf.Clamp(level, 0, GameConst._levelUpExpList.Count);
            return GameConst._levelUpExpList[level - 1];
        }

        public int GetLevelUpExp()
        {
            //防止越界，限制下
            int lv = Mathf.Clamp(Level, 0, GameConst._levelUpExpList.Count);
            return GameConst._levelUpExpList[lv - 1];
        }


        /// <summary>
        /// 升级属性计算公式可以参考：https://github.com/ZhanruiLiang/jinyong-legend
        ///
        /// 
        /// </summary>
        /// <returns></returns>
        public void LevelUp()
        {
            Level++;
            Tili = GameConst.MAX_ROLE_TILI;
            MaxHp += (HpInc + Random.Range(0, 3)) * 3;
            SetHPAndRefreshHudBar(this.MaxHp);
            //当0 <= 资质 < 30, a = 2;
            //当30 <= 资质 < 50, a = 3;
            //当50 <= 资质 < 70, a = 4;
            //当70 <= 资质 < 90, a = 5;
            //当90 <= 资质 <= 100, a = 6;
            //a = random(a) + 1;
            int a = Random.Range(0, (int)Math.Floor((double)(IQ - 10) / 20) + 2) + 1;
            MaxMp += (9 - a) * 4;
            Mp = MaxMp;

            Hurt = 0;
            Poison = 0;

            Attack += a;
            Qinggong += a;
            Defence += a;

            Heal = checkUp(Heal, 20, 3);
            DePoison = checkUp(DePoison, 20, 3);
            UsePoison = checkUp(UsePoison, 20, 3);

            Quanzhang = checkUp(Quanzhang, 20, 3);
            Yujian = checkUp(Yujian, 20, 3);
            Shuadao = checkUp(Shuadao, 20, 3);
            Anqi = checkUp(Anqi, 20, 3);

            this.LimitAllAttrs();

            Debug.Log($"{this.Name}升到{this.Level}级！");
        }

        /// <summary>
        /// 限制属性范围
        /// 
        /// Attack、Defence、Qinggong为最终状态：原始属性 + 此刻使用的装备属性的总值
        /// 
        /// </summary>
        void LimitAllAttrs()
        {
            Exp = Tools.Limit(Exp, 0, GameConst.MAX_EXP);
            ExpForItem = Tools.Limit(ExpForItem, 0, GameConst.MAX_EXP);
            ExpForMakeItem = Tools.Limit(ExpForMakeItem, 0, GameConst.MAX_EXP);
            Poison = Tools.Limit(Poison, 0, GameConst.MAX_POISON);
            MaxHp = Tools.Limit(MaxHp, 0, GameConst.MAX_ROLE_HP);
            MaxMp = Tools.Limit(MaxMp, 0, GameConst.MAX_ROLE_MP);
            Hp = Tools.Limit(Hp, 0, MaxHp);
            Mp = Tools.Limit(Mp, 0, MaxMp);
            Tili = Tools.Limit(Tili, 0, GameConst.MAX_ROLE_TILI);

            // 获取装备增加的属性，这些属性不受最大值限制
            var equipAttack = GetWeaponProperty("Attack") + GetArmorProperty("Attack");
            var equipDefence = GetWeaponProperty("Defence") + GetArmorProperty("Defence");
            var equipQinggong = GetWeaponProperty("Qinggong") + GetArmorProperty("Qinggong");
            // 限制属性
            Attack = LimitAttr(Attack, 0, GameConst.MAX_ROLE_ATTACK, equipAttack);
            Defence = LimitAttr(Defence, 0, GameConst.MAX_ROLE_DEFENCE, equipDefence);
            Qinggong = LimitAttr(Qinggong, 0, GameConst.MAX_ROLE_QINGGONG, equipQinggong);

            UsePoison = Tools.Limit(UsePoison, 0, GameConst.MAX_USE_POISON);
            DePoison = Tools.Limit(DePoison, 0, GameConst.MAX_DEPOISON);
            Heal = Tools.Limit(Heal, 0, GameConst.MAX_HEAL);
            AntiPoison = Tools.Limit(AntiPoison, 0, GameConst.MAX_ANTIPOISON);

            Quanzhang = Tools.Limit(Quanzhang, 0, GameConst.MAX_ROLE_WEAPON_ATTR);
            Yujian = Tools.Limit(Yujian, 0, GameConst.MAX_ROLE_WEAPON_ATTR);
            Shuadao = Tools.Limit(Shuadao, 0, GameConst.MAX_ROLE_WEAPON_ATTR);
            Qimen = Tools.Limit(Qimen, 0, GameConst.MAX_ROLE_WEAPON_ATTR);
            Anqi =Tools.Limit(Anqi, 0, GameConst.MAX_ROLE_WEAPON_ATTR);

            IQ = Tools.Limit(IQ, 0, GameConst.MAX_ROLE_ZIZHI);
            Pinde = Tools.Limit(Pinde, 0, GameConst.MAX_ROLE_PINDE);
            Shengwang = Tools.Limit(Shengwang, 0, GameConst.MAX_ROLE_SHENGWANG);
            AttackPoison = Tools.Limit(AttackPoison, 0, GameConst.MAX_ROLE_ATK_POISON);
            Hurt = Tools.Limit(Hurt, 0, GameConst.MAX_HURT);

            foreach (var wugong in Wugongs)
            {
                wugong.Level = Tools.Limit(wugong.Level, 0, GameConst.MAX_SKILL_LEVEL);
            }
        }
        /// <summary>
        /// 角色专用的属性范围限制
        /// </summary>
        int LimitAttr(int attr, int minValue, int maxValue, int extraValue)
        {
            int attrNeat = attr - extraValue;
            //净值大于最大值，限制净值为最大值
            if (attrNeat > maxValue)
            {
                return maxValue + extraValue;
            }
            //净值小于最小值而且附加值为正,限制净值为最小值
            if (attrNeat < minValue && extraValue > 0)
            {
                return minValue + extraValue;
            }
            //净值小于最小值且附加值为负,限制总值为最小值
            if (attrNeat < minValue && extraValue < 0)
            {
                return minValue;
            }
            //以上都没有，则认为属性符合要求
            return attr;
        }

        int checkUp(int value, int limit, int max_inc)
        {
            if (value >= limit)
            {
                value += Random.Range(0, max_inc);
            }

            return value;
        }

        private static Type _thisType = Type.GetType("Jyx2.RoleInstance");
        /// <summary>
        /// 增加角色属性的API
        /// </summary>
        /// <param name="attrName">属性名</param>
        /// <param name="delta">属性增量</param>
        /// <returns>属性实际增量</returns>
        public int AddAttr(string attrName, int delta)
        {
            var attr = _thisType.GetField(attrName);
            if (attr == null)
            {
                Debug.LogError($"人物属性 {attrName} 不存在");
                return 0;
            }
            int oldValue = (int)attr.GetValue(this);
            attr.SetValue(this, oldValue + delta);
            LimitAllAttrs();
            return (int)attr.GetValue(this) - oldValue;
        }

        public int ExpGot; //战斗中获得的经验
        public int PreviousRoundHp; //上一回合的生命值
        #endregion

        public LItemConfig GetWeapon()
        {
            if (Weapon == -1) return null;
            return LuaToCsBridge.ItemTable[Weapon];
        }

        public LItemConfig GetArmor()
        {
            if (Armor == -1) return null;
            return LuaToCsBridge.ItemTable[Armor];
        }


        public LItemConfig GetXiulianItem()
        {
            if (Xiulianwupin == -1) return null;
            return LuaToCsBridge.ItemTable[Xiulianwupin];
        }


        /// <summary>
        /// 战斗中使用的招式
        /// </summary>
        private List<SkillCastInstance> Skills;


        /// <summary>
        /// 获取该角色所有的招式，（如果有医疗、用毒、解毒，也封装成招式）
        /// </summary>
        /// <returns></returns>
        public List<SkillCastInstance> GetSkillsList(bool forceAttackSkill)
        {
            List<SkillCastInstance> skills = new List<SkillCastInstance>();
            //金庸DOS版逻辑，体力大于等于10且有武功最低等级所需内力值才可以使用技能
            if (this.Tili >= 10)
            {
                foreach (var skill in Skills)
                {
                    if (this.Mp >= skill.Data.GetSkill().MpCost)
                        skills.Add(skill);
                }
            }

            if (forceAttackSkill)
                return skills;

            //金庸DOS版逻辑，用毒、解毒、医疗
            if (this.UsePoison >= 20 && this.Tili >= 10) skills.Add( new PoisonSkillCastInstance(this.UsePoison));
            if (this.DePoison >= 20 && this.Tili >= 10) skills.Add(new DePoisonSkillCastInstance(this.DePoison));
            if (this.Heal >= 20 && this.Tili >= 50) skills.Add(new HealSkillCastInstance(this.Heal));

            return skills;
        }

        public void ResetSkillCasts()
        {
            if (Skills == null)
            {
                Skills = new List<SkillCastInstance>();
            }
            else
            {
                Skills.Clear();
            }

            foreach (var wugong in Wugongs)
            {
                Skills.Add(new SkillCastInstance(wugong));
            }
        }

        #region JYX2道具相关

        //重置身上的物品
        public void ResetItems()
        {
            Items.Clear();
            //配置表中添加的物品
            foreach (var item in Data.Items)
            {
                Items.Add(new CsRoleItem(item));
            }
        }

        public bool HaveItemBool(int itemId)
        {
            return Items.FindIndex(it => it.Id == itemId) != -1;
        }

        /// <summary>
        /// 为角色添加物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void AddItem(int itemId, int count)
        {
            var item = Items.Find(it => it.Id == itemId);

            if (item != null)
            {
                item.Count += count;

                //fix issue of using one removed the entire item
                if (count <  0 && item.Count <= 0)
                    Items.Remove(item);
            }
            else
            {
                Items.Add(new CsRoleItem()
                        {
                        Id = itemId,
                        Count = count
                        });
            }
        }


        public bool CanUseItem(int itemId)
        {
            return CanUseItem(LuaToCsBridge.ItemTable[itemId]);
        }

        /// <summary>
        /// 判断角色是否可以使用道具
        /// 
        /// 对应kyscpp：bool GameUtil::canUseItem(Role* r, Item* i)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanUseItem(LItemConfig item)
        {
            if (item == null) return false;

            //剧情类无人可以使用
            if (item.ItemType == 0)
                return false;

            else if ((int)item.ItemType == 1 || (int)item.ItemType == 2)
            {

                if ((int)item.ItemType == 2)
                {   
                    //若为可习得技能的武学
                    if (item.Skill>=0)
                    {
                        //若武学等级大于一，说明已习得武学，直接装备
                        int level = GetWugongLevel(item.Skill);
                        if (level > 0)
                        {
                            return true;
                        }
                        //若无相关武学，开始装备条件判断
                        //有仅适合人物，直接判断
                        if (item.OnlySuitableRole >= 0)
                        {
                            return item.OnlySuitableRole == this.Key;
                        }

                        //内力属性判断
                        if ((this.MpType == 0 || this.MpType == 1) && (item.NeedMPType == 0 || (int)item.NeedMPType == 1))
                        {
                            if (this.MpType != (int)item.NeedMPType)
                            {
                                return false;
                            }
                        }
                        //若已经学满武学，则为假
                        if (level < 0 || this.Wugongs.Count >= GameConst.MAX_SKILL_COUNT)
                        {
                            return false;
                        }
                    }
                    //若不是练武学技能的秘籍
                    else
                    {

                        //有仅适合人物，直接判断
                        if (item.OnlySuitableRole >= 0)
                        {
                            return item.OnlySuitableRole == this.Key;
                        }

                        //内力属性判断
                        if ((this.MpType == 0 || this.MpType == 1) && (item.NeedMPType == 0 || (int)item.NeedMPType == 1))
                        {
                            if (this.MpType != (int)item.NeedMPType)
                            {
                                return false;
                            }
                        }
                    }



                }



                //上面的判断未确定则进入下面的判断链
                return testAttr(this.Attack - GetWeaponProperty("Attack") - GetArmorProperty("Attack"), item.ConditionAttack)
                    && testAttr(this.Qinggong - GetWeaponProperty("Qinggong") - GetArmorProperty("Qinggong"), item.ConditionQinggong)
                    && testAttr(this.Heal, item.ConditionHeal)
                    && testAttr(this.UsePoison, item.ConditionPoison)
                    && testAttr(this.DePoison, item.ConditionDePoison)
                    && testAttr(this.Quanzhang, item.ConditionQuanzhang)
                    && testAttr(this.Yujian, item.ConditionYujian)
                    && testAttr(this.Shuadao, item.ConditionShuadao)
                    && testAttr(this.Qimen, item.ConditionQimen)
                    && testAttr(this.Anqi, item.ConditionAnqi)
                    && testAttr(this.MaxMp, item.ConditionMp)
                    && testAttr(this.IQ, item.ConditionIQ);
            }
            else if ((int)item.ItemType == 3)
            {
                //药品类所有人可以使用
                return true;
            }
            else if ((int)item.ItemType == 4)
            {
                //暗器类不可以使用
                return false;
            }

            return false;
        }


        bool testAttr(int v, int v_need)
        {
            if (v_need > 0 && v < v_need)
            {
                return false;
            }

            if (v_need < 0 && v > -v_need)
            {
                return false;
            }

            return true;
        }


        private GameRuntimeData runtime
        {
            get { return GameRuntimeData.Instance; }
        }

        /// <summary>
        /// 炼制物品
        /// 计算公式可以参考：https://github.com/ZhanruiLiang/jinyong-legend
        /// </summary>
        /// <param name="item"></param>
        public string LianZhiItem(LItemConfig practiseItem)
        {
            if (practiseItem == null)
                return "";
            if (practiseItem.GenerateItems == "")
                return "";
            if (practiseItem.GenerateItemNeedCost == -1)
                return "";
            if (!runtime.HaveItemBool(practiseItem.GenerateItemNeedCost))
                return "";
            var GenerateItemList = new List<CsRoleItem>();
            var GenerateItemArr = practiseItem.GenerateItems.Split('|');
            foreach (var GenerateItem in GenerateItemArr)
            {
                var GenerateItemArr2 = GenerateItem.Split(',');
                if (GenerateItemArr2.Length != 2) continue;
                var characterItem = new CsRoleItem();
                characterItem.Id = int.Parse(GenerateItemArr2[0]);
                characterItem.Count = int.Parse(GenerateItemArr2[1]);
                GenerateItemList.Add(characterItem);
            }
            int GenerateItemNeedCount = runtime.Items[practiseItem.GenerateItemNeedCost.ToString()].Item1;
            int GenerateItemNeedExp = (7 - IQ / 15) * practiseItem.GenerateItemNeedExp;

            if (ExpForMakeItem >= GenerateItemNeedExp && GenerateItemNeedCount  >= GenerateItemList.Count)
            {
                //随机选择练出的物品
                var pickItem = Jyx2.Middleware.Tools.GetRandomElement(GenerateItemList);

                //已经有物品
                if (runtime.HaveItemBool(pickItem.Id))
                {
                    runtime.AddItem(pickItem.Id, 1);
                }
                else
                {
                    runtime.AddItem(pickItem.Id, 1 + Random.Range(0, 3));
                }

                runtime.AddItem(practiseItem.GenerateItemNeedCost, -pickItem.Count);
                ExpForMakeItem = 0;
                return $"{Name} 制造出 {LuaToCsBridge.ItemTable[pickItem.Id].Name}\n";
            }

            return "";
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="item"></param>
        public void UseItem(LItemConfig item)
        {
            if (item == null)
                return;

            this.Tili += item.AddTili;
            //吃药机制
            //参考：https://github.com/ZhanruiLiang/jinyong-legend
            int add = item.AddHp - this.Hurt / 2 + Random.Range(0, 10);
            if (add <= 0)
            {
                add = 5 + Random.Range(0, 5);
            }
            this.Hurt -= item.AddHp / 4;
            this.SetHPAndRefreshHudBar(this.Hp + add);
            this.MaxHp += item.AddMaxHp;
            this.Mp += item.AddMp;
            this.MaxMp += item.AddMaxMp;
            this.Poison += item.ChangePoisonLevel / 2;
            this.Heal += item.Heal;
            this.DePoison += item.DePoison;
            this.AntiPoison += item.AntiPoison;
            this.UsePoison += item.UsePoison;

            this.Attack += item.Attack;
            this.Defence += item.Defence;
            this.Qinggong += item.Qinggong;

            this.Quanzhang += item.Quanzhang;
            this.Yujian += item.Yujian;
            this.Shuadao += item.Shuadao;
            this.Qimen += item.Qimen;
            this.Anqi += item.Anqi;

            this.Pinde += item.AddPinde;
            this.AttackPoison += item.AttackPoison;

            if (item.ChangeMPType == 2)
            {
                this.MpType = 2;
            }

            if (item.Zuoyouhubo == 1)
            {
                this.Zuoyouhubo = 1;
            }

            if (CanFinishedItem())
            {
                if (item.Skill != null)
                {
                    this.LearnMagic(item.Skill);
                }

                this.ExpForItem = 0;
            }

            this.LimitAllAttrs();
        }

        /// <summary>
        /// 卸下物品（装备）
        /// </summary>
        /// <param name="item"></param>
        public void UnequipItem(LItemConfig item)
        {
            if (item == null)
                return;

            //在卸载装备流程中清理角色的装备属性值
            if (Weapon == item.Id)
            {
                Weapon = -1;
            }
            else if (Armor == item.Id)
            {
                Armor = -1;
            }
            else
            {
                return;
            }

            runtime.SetItemUser(item.Id, -1);
            this.Tili -= item.AddTili;
            this.SetHPAndRefreshHudBar(this.Hp - item.AddHp);
            this.MaxHp -= item.AddMaxHp;
            this.Mp -= item.AddMp;
            this.MaxMp -= item.AddMaxMp;
            this.Poison -= item.ChangePoisonLevel;
            this.Heal -= item.Heal;
            this.DePoison -= item.DePoison;
            this.AntiPoison -= item.AntiPoison;
            this.UsePoison -= item.UsePoison;

            this.Attack -= item.Attack;
            this.Defence -= item.Defence;
            this.Qinggong -= item.Qinggong;

            this.Quanzhang -= item.Quanzhang;
            this.Yujian -= item.Yujian;
            this.Shuadao -= item.Shuadao;
            this.Qimen -= item.Qimen;

            this.Pinde -= item.AddPinde;
            this.AttackPoison -= item.AttackPoison;

            this.LimitAllAttrs();
        }

        public bool CanFinishedItem()
        {
            if (this.ExpForItem >= GetFinishedExpForItem())
            {
                return true;
            }

            return false;
        }

        public int GetFinishedExpForItem()
        {
            return GetFinishedExpForItem(GetXiulianItem());
        }

        /// <summary>
        /// 获得修炼所需经验
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetFinishedExpForItem(LItemConfig item)
        {
            if (item == null || (int)item.ItemType != 2 || item.NeedExp < 0)
            {
                return GameConst.MAX_EXP;
            }

            int multiple = 7 - this.IQ / 15;
            if (multiple <= 0)
            {
                multiple = 1;
            }

            //有关联武学的，如已满级则不可修炼
            if (item.Skill != null)
            {
                int magic_level_index = GetWugongLevel(item.Skill);
                if (magic_level_index == GameConst.MAX_SKILL_LEVEL)
                {
                    return GameConst.MAX_EXP;
                }

                //初次修炼和从1级升到2级的是一样的
                if (magic_level_index > 0)
                {
                    multiple *= magic_level_index;
                }
            }
            else
            {
                multiple *= 2;
            }

            return item.NeedExp * multiple;
        }

        #endregion

        public int GetWugongLevel(int wugongId)
        {
            foreach (var wugong in Wugongs)
            {
                if (wugong.Key == wugongId)
                    return wugong.GetLevel();
            }

            return 0;
        }


        public LRoleConfig Data
        {
            get
            {
                if (_data == null)
                {
                    BindKey();
                }

                return _data;
            }
        }

        private LRoleConfig _data;

        public BattleRole View { get; set; }

        #region 战斗相关

        public BattleFieldModel BattleModel;

        //是否在战斗中
        private bool _isInBattle = false;

        //所属队伍，主角方为0
        public int team;

        //集气数量
        public float sp;

        //AI
        public bool isAI;

        private BattleBlockVector _pos;

        //位置
        public BattleBlockVector Pos
        {
            get { return _pos; }
            set
            {
                if (_pos == value)
                    return;
                _pos = value;
                UpdateViewPostion();
            }
        }

        public void UpdateViewPostion()
        {
            BattleBlockData posData = BattleboxHelper.Instance.GetBlockData(Pos.X, Pos.Y);
            View.SetPosition(posData.WorldPos);
        }

        //移动过的格子数
        public int movedStep = 0;

        //是否已经行动
        public bool isActed = false;
        public bool isWaiting = false; //正在等待

        public void EnterBattle()
        {
            if (_isInBattle) return;

            _isInBattle = true;

            View.LazyInitAnimator();

            //修复当前武功
            if (CurrentSkill >= Wugongs.Count)
            {
                CurrentSkill = 0;
            }
            _currentSkill = Wugongs[CurrentSkill];
            SwitchAnimationToSkill(_currentSkill, true);
        }

        public void SetHPAndRefreshHudBar(int hp)
        {
            Hp = hp;
            View?.MarkHpBarIsDirty();
        }

        private SkillInstance _currentSkill = null;

        public void SwitchAnimationToSkill(SkillInstance skill, bool force = false)
        {
            if (skill == null || (_currentSkill == skill && !force)) return;

            //切换武学待机动作
            View.SwitchSkillTo(skill);

            _currentSkill = skill;
        }

        public void LeaveBattle()
        {
            _isInBattle = false;
        }


        public void TimeRun()
        {
            IncSp();
        }

        //集气槽增长 根据轻功来增加
        public void IncSp()
        {
            sp += this.Qinggong / 4; //1f;
        }

        //获得行动力
        //参考：https://github.com/ZhanruiLiang/jinyong-legend
        public int GetMoveAbility()
        {
            if (Tili <= 5)
                return 0; //金庸DOS版逻辑，体力小于等于5无法移动
            int speed = this.Qinggong;

            speed = speed / 15 - this.Hurt / 40;

            if (speed < 0)
            {
                speed = 0;
            }
            return speed;
        }

        //是否是AI控制
        bool IsAI()
        {
            return isAI;
        }

        public int CompareTo(RoleInstance other)
        {
            int result = this.team.CompareTo(other.team);
            return result;
        }

        #endregion

        #region 状态相关

        public bool IsDead()
        {
            return Hp <= 0;
        }

        public void Resurrect()
        {
            SetHPAndRefreshHudBar(MaxHp);
        }

        //是否晕眩
        private bool _isStun = false;

        /// <summary>
        /// 晕眩
        /// </summary>
        /// <param name="duration">等于0时不晕眩；大于0时晕眩{duration}秒；小于0时永久晕眩</param>
        public void Stun(float duration = -1)
        {
            //记录晕眩状态
            if (duration > 0)
            {
                _isStun = true;
                View.ShowStun();
                int frame = Convert.ToInt32(duration * 60);
                Observable.TimerFrame(frame, FrameCountType.FixedUpdate)
                    .Subscribe(ms => { StopStun(); });
            }
            //永久晕眩（需要手动停止晕眩）
            else if (duration < 0)
            {
                _isStun = true;
                View.ShowStun();
            }
        }

        public void StopStun()
        {
            _isStun = false;
            View.StopStun(_isInBattle);
        }

        //TODO:由于探索地图没有实例，所以晕眩状态暂时由UI决定 by Cherubinxxx
        public bool IsStun()
        {
            return _isStun;
        }

        #endregion

        //JYX2的休息逻辑，对应jinyong-legend  War_RestMenu
        public void OnRest()
        {
            int addTili = 3 + Random.Range(0, 3);
            Tili = Tools.Limit(Tili + addTili, 0, GameConst.MAX_ROLE_TILI);
            if (Tili > 30)
            {
                int addHpMp = 3 + Random.Range(0, Tili / 10 - 2);
                Hp = Tools.Limit(Hp + addHpMp, 0, MaxHp);
                Mp = Tools.Limit(Mp + addHpMp, 0, MaxMp);
            }
        }

        //学习武学逻辑，对应kyscpp int Role::learnMagic(int magic_id)
        public int LearnMagic(int magicId)
        {
            if (magicId <= 0)
                return -1;

            foreach (var skill in Wugongs)
            {
                if (skill.Key == magicId)
                {
                    if (skill.Level < GameConst.MAX_SKILL_LEVEL)
                    {
                        skill.Level += 100;
                        return 0;
                    }
                    else
                    {
                        return -2; //已经满级
                    }
                }
            }

            if (Wugongs.Count >= GameConst.MAX_SKILL_COUNT)
                return -3; //武学已满

            if (!LuaToCsBridge.SkillTable.ContainsKey(magicId))
            {
                Debug.LogError("试图添加不存在的武功, skillId: " + magicId);
                return -4; //无此武学
            }

            SkillInstance w = new SkillInstance(magicId);
            Wugongs.Add(w);
            ResetSkillCasts();
            return 0;
        }

        public string GetMPColor()
        {
            return MpType == 2 ? ColorStringDefine.Default : MpType == 1 ? ColorStringDefine.Mp_type1 : ColorStringDefine.Mp_type0;
        }

        public string GetHPColor1()
        {
            return Hurt > 20 ? ColorStringDefine.Hp_hurt_heavy : Hurt > 0 ? ColorStringDefine.Hp_hurt_light : ColorStringDefine.Default;
        }

        public string GetHPColor2()
        {
            return Poison > 0 ? ColorStringDefine.Hp_posion : ColorStringDefine.Default;
        }

        public int GetWeaponProperty(string propertyName)
        {
            return Weapon != -1 ? (int)Type.GetType("Jyx2.LItemConfig").GetProperty(propertyName).GetValue(GetWeapon(), null) : 0;
        }

        public int GetArmorProperty(string propertyName)
        {
            return Armor != -1 ? (int)Type.GetType("Jyx2.LItemConfig").GetProperty(propertyName).GetValue(GetArmor(), null) : 0;
        }

        /// <summary>
        /// 获取武器武功配合加攻击力
        ///
        /// 计算方法参考：https://github.com/ZhanruiLiang/jinyong-legend
        ///
        /// 玄铁剑+玄铁剑法 攻击+100
        /// 君子剑+玉女素心剑 攻击+50
        /// 淑女剑+玉女素心剑 攻击+50
        /// 血刀+血刀大法 攻击+50
        /// 冷月宝刀+胡家刀法 攻击+70
        /// 金蛇剑+金蛇剑法 攻击力+80
        /// 霹雳狂刀+霹雳刀法 攻击+100
        /// </summary>
        /// <param name="wugong"></param>
        /// <returns></returns>
        public int GetExtraAttack(int wugongId)
        {
            LExtraConfig extra;
            if (LuaToCsBridge.ExtraTable != null && LuaToCsBridge.ExtraTable.TryGetValue(Weapon, out extra))
            {
                if (extra.Wugong == wugongId)
                {
                    return extra.ExtraAttack;
                }
            }
            return 0;
        }


        public RoleInstance Clone()
        {
            var data = ES3.Serialize(this);
            var newRole = ES3.Deserialize<RoleInstance>(data);
            return newRole;
        }

        public bool IsPlayerRole => this == GameRuntimeData.Instance.Player;
    }
}
