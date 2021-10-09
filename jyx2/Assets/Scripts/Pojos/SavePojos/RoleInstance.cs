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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSFrameWork.Common;
using HSFrameWork.SPojo;
using UnityEngine;
using UniRx;
using Jyx2;
using HSFrameWork.ConfigTable;
using Random = UnityEngine.Random;


namespace Jyx2
{
    public class RoleInstance : SaveablePojo, IComparable<RoleInstance>
    {
        public RoleInstance()
        {
        } //手动调用的话，需要调用BindKey

        public RoleInstance(string roleKey)
        {
            Key = roleKey;
            BindKey();
            Recover();
        }

        public void BindKey()
        {
            //for jyx2,自动适配为小虾米
            if (Key == "主角")
            {
                Key = "0";
            }

            _data = ConfigTable.Get<Jyx2Role>(Key);

            if (_data == null)
            {
                Key = "10000"; //自动设置为空人物
                _data = new Jyx2Role() {Id = "10000"};
                _data.Wugongs = new List<Jyx2RoleWugong>();
                _data.Items = new List<Jyx2RoleItem>();
            }

            _data.Exp = 0; //已经有初始等级。经验值应该抵消，设置为0.

            //初始化武功列表
            //Wugongs.Clear();			
            if (Wugongs.Count == 0)
            {
                foreach (var wugong in _data.Wugongs)
                {
                    if (wugong.Id == 0)
                        continue;
                    Wugongs.Add(new WugongInstance(wugong));
                }
            }

            //没有设置武功，默认增加一个普通攻击
            if (Wugongs.Count == 0 && _data.Wugongs != null && _data.Wugongs.Count > 0)
            {
                Wugongs.Add(new WugongInstance(_data.Wugongs[0]));
            }

            //初始化属性增强
            AddEffect(this.GetWeapon(), 1);
            AddEffect(this.GetArmor(), 1);
            AddEffect(this.GetXiulianItem(), 1);
            getOrigin = false;

            //每次战斗前reset一次
            ResetForBattle();
        }

        public void ResetForBattle()
        {
            ResetZhaoshis();
            ResetItems();
        }

        public void Recover()
        {
            SetHPAndRefreshHudBar(MaxHp);

            Mp = MaxMp;
            Tili = GameConst.MaxTili;

            Hurt = 0;
            Poison = 0;
        }

        public int HpInc
        {
            get { return Data.HpInc; }
        }

        public string Waihao
        {
            get { return Data.Waihao; }
        } //外号

        public string Key
        {
            get { return Get("Key"); }
            set { Save("Key", value); }
        }

        public int GetJyx2RoleId()
        {
            try
            {
                return int.Parse(Key);
            }
            catch
            {
                Debug.LogError("错误的JYX2人物ID：" + Key);
                return -1;
            }
        }

        public string Name
        {
            get { return Get("Name", Data.Name); }
            set { Save("Name", value); }
        }

        public int Sex
        {
            get { return Get("Sex", Data.Sex); }
            set { Save("Sex", value); }
        }

        #region JYX2等级相关

        public int Level //等级
        {
            get { return Get("Level", Data.Level); }
            set { Save("Level", value); }
        }

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
            return GameConst._levelUpExpList[level - 1];
        }

        public int GetLevelUpExp()
        {
            return GameConst._levelUpExpList[Level - 1];
        }


        public void LevelUp()
        {
            Exp -= GameConst._levelUpExpList[this.Level - 1];
            Level++;
            Tili = GameConst.MAX_ROLE_TILI;
            MaxHp += Data.HpInc * 3 + Tools.GetRandomInt(0, 6);
            SetHPAndRefreshHudBar(this.MaxHp);
            MaxMp += 20 + Tools.GetRandomInt(0, 6);
            Mp = MaxMp;

            Hurt = 0;
            Poison = 0;

            getOrigin = true;
            Attack += Tools.GetRandomInt(0, 7);
            Qinggong += Tools.GetRandomInt(0, 7);
            Defence += Tools.GetRandomInt(0, 7);
            getOrigin = false;

            Heal = checkUp(Heal, 0, 3);
            DePoison = checkUp(DePoison, 0, 3);
            UsePoison = checkUp(UsePoison, 0, 3);

            Quanzhang = checkUp(Quanzhang, 10, 3);
            Yujian = checkUp(Yujian, 10, 3);
            Shuadao = checkUp(Shuadao, 10, 3);
            Qimen = checkUp(Qimen, 10, 3);

            this.Limit();

            Debug.Log($"{this.Name}升到{this.Level}级！");
        }

        void Limit()
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
            //
            // Attack = Tools.Limit(Attack, 0, GameConst.MAX_ROLE_ATTACK);
            // Defence = Tools.Limit(Defence, 0, GameConst.MAX_ROLE_DEFENCE);
            // Qinggong = Tools.Limit(Qinggong, 0, GameConst.MAX_ROLE_QINGGONG);
            //
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

            foreach (var wugong in Wugongs)
            {
                wugong.Level = Tools.Limit(wugong.Level, 0, GameConst.MAX_SKILL_LEVEL);
            }
        }

        int checkUp(int value, int limit, int max_inc)
        {
            if (value > limit)
            {
                value += 1 + Tools.GetRandomInt(0, max_inc);
            }

            return value;
        }

        public int Exp //经验
        {
            get { return Get("Exp", Data.Exp); }
            set { Save("Exp", value); }
        }

        public int ExpGot; //战斗中获得的经验

        #endregion

        public int Hp
        {
            get { return Get("Hp", Data.Hp); }
            set { Save("Hp", value); }
        }

        public int MaxHp
        {
            get { return Get("MaxHp", Data.MaxHp); }
            set { Save("MaxHp", value); }
        }

        public int Hurt //受伤程度
        {
            get { return Get("Hurt", Data.DamageLevel); }
            set { Save("Hurt", value); }
        }

        public int Poison //中毒程度
        {
            get { return Get("Poison", Data.PoisonLevel); }
            set { Save("Poison", value); }
        }

        public int Tili //体力
        {
            get { return Get("Tili", Data.Tili); }
            set { Save("Tili", value); }
        }

        public int ExpForMakeItem //物品修炼点
        {
            get { return Get("ExpForMakeItem", Data.Xiulian); }
            set { Save("ExpForMakeItem", value); }
        }

        public int Weapon //武器
        {
            get { return Get("Weapon", Data.Weapon); }
            set { Save("Weapon", value); }
        }

        public Jyx2Item GetWeapon()
        {
            if (Weapon == -1) return null;
            return ConfigTable.Get<Jyx2Item>(Weapon);
        }

        public int Armor //防具
        {
            get { return Get("Armor", Data.Armor); }
            set { Save("Armor", value); }
        }

        public Jyx2Item GetArmor()
        {
            if (Armor == -1) return null;
            return ConfigTable.Get<Jyx2Item>(Armor);
        }

        public int MpType //内力性质
        {
            get { return Get("MpType", Data.MpType); }
            set { Save("MpType", value); }
        }

        public int Mp
        {
            get { return Get("Mp", Data.Mp); }
            set { Save("Mp", value); }
        }

        public int MaxMp
        {
            get { return Get("MaxMp", Data.MaxMp); }
            set { Save("MaxMp", value); }
        }

        public int Attack //攻击力
        {
            get
            {
                return getOrigin ? Get("Attack", Data.Attack) : Tools.Limit(Get("Attack", Data.Attack), 0, MaxAttack);
            }
            set { Save("Attack", value); }
        }

        public int Qinggong //轻功
        {
            get
            {
                return getOrigin
                    ? Get("Qinggong", Data.Qinggong)
                    : Tools.Limit(Get("Qinggong", Data.Qinggong), 0, MaxQinggong);
            }
            set { Save("Qinggong", value); }
        }

        public int Defence //防御力
        {
            get
            {
                return getOrigin
                    ? Get("Defence", Data.Defence)
                    : Tools.Limit(Get("Defence", Data.Defence), 0, MaxDefence);
            }
            set { Save("Defence", value); }
        }

        public int Heal //医疗
        {
            get { return Get("Heal", Data.Heal); }
            set { Save("Heal", value); }
        }

        public int UsePoison //用毒
        {
            get { return Get("UsePoison", Data.UsePoison); }
            set { Save("UsePoison", value); }
        }

        public int DePoison //解毒
        {
            get { return Get("DePoison", Data.DePoison); }
            set { Save("DePoison", value); }
        }

        public int AntiPoison //抗毒
        {
            get { return Get("AntiPoison", Data.AntiPoison); }
            set { Save("AntiPoison", value); }
        }

        public int Quanzhang //拳掌
        {
            get { return Get("Quanzhang", Data.Quanzhang); }
            set { Save("Quanzhang", value); }
        }

        public int Yujian //御剑
        {
            get { return Get("Yujian", Data.Yujian); }
            set { Save("Yujian", value); }
        }

        public int Shuadao //耍刀
        {
            get { return Get("Shuadao", Data.Shuadao); }
            set { Save("Shuadao", value); }
        }

        public int Qimen //特殊兵器
        {
            get { return Get("Qimen", Data.Qimen); }
            set { Save("Qimen", value); }
        }

        public int Anqi //暗器技巧
        {
            get { return Get("Anqi", Data.Anqi); }
            set { Save("Anqi", value); }
        }

        public int Wuxuechangshi //武学常识
        {
            get { return Get("Wuxuechangshi", Data.Wuxuechangshi); }
            set { Save("Wuxuechangshi", value); }
        }

        public int Pinde //品德
        {
            get { return Get("Pinde", Data.Pinde); }
            set { Save("Pinde", value); }
        }

        public int AttackPoison //攻击带毒
        {
            get { return Get("AttackPoison", Data.AttackPoison); }
            set { Save("AttackPoison", value); }
        }

        public int Zuoyouhubo //左右互搏
        {
            get { return Get("Zuoyouhubo", Data.Zuoyouhubo); }
            set { Save("Zuoyouhubo", value); }
        }

        public int Shengwang //声望
        {
            get { return Get("Shengwang", Data.Shengwang); }
            set { Save("Shengwang", value); }
        }

        public int IQ //资质
        {
            get { return Get("IQ", Data.IQ); }
            set { Save("IQ", value); }
        }

        public int Xiulianwupin //修炼物品
        {
            get { return Get("Xiulianwupin", Data.Xiulianwupin); }
            set { Save("Xiulianwupin", value); }
        }

        public Jyx2Item GetXiulianItem()
        {
            if (Xiulianwupin == -1) return null;
            return ConfigTable.Get<Jyx2Item>(Xiulianwupin);
        }

        public int ExpForItem //修炼点数
        {
            get { return Get("ExpForItem", Data.XiulianPoint); }
            set { Save("ExpForItem", value); }
        }

        //武功
        public List<WugongInstance> Wugongs
        {
            get { return GetList<WugongInstance>("Wugongs"); }
            set { SaveList("Wugongs", value); }
        }

        /// <summary>
        /// 战斗中使用的招式
        /// </summary>
        private List<BattleZhaoshiInstance> Zhaoshis;


        /// <summary>
        /// 获取该角色所有的招式，（如果有医疗、用毒、解毒，也封装成招式）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BattleZhaoshiInstance> GetZhaoshis(bool forceAttackZhaoshi)
        {
            //金庸DOS版逻辑，体力大于20才可以使用技能
            if (this.Tili >= 20)
            {
                foreach (var zhaoshi in Zhaoshis)
                {
                    yield return zhaoshi;
                }
            }

            if (forceAttackZhaoshi)
                yield break;

            //金庸DOS版逻辑，用毒、解毒、医疗
            if (this.UsePoison > 0 && this.Tili >= 3) yield return new PoisonZhaoshiInstance(this.UsePoison);
            if (this.DePoison > 0 && this.Tili >= 5) yield return new DePoisonZhaoshiInstance(this.DePoison);
            if (this.Heal > 0 && this.Tili >= 5) yield return new HealZhaoshiInstance(this.Heal);
        }

        public void ResetZhaoshis()
        {
            if (Zhaoshis == null)
            {
                Zhaoshis = new List<BattleZhaoshiInstance>();
            }
            else
            {
                Zhaoshis.Clear();
            }

            foreach (var wugong in Wugongs)
            {
                Zhaoshis.Add(new BattleZhaoshiInstance(wugong));
            }
        }

        #region JYX2道具相关

        //重置身上的物品
        public void ResetItems()
        {
            _items = new List<Jyx2RoleItem>();

            //配置表中添加的物品
            foreach (var item in Data.Items)
            {
                if (item.Id < 0) continue;
                _items.Add(item.Clone()); //这里对于NPC来说，每场战斗都会补满道具
            }
        }

        //身上携带的道具
        public List<Jyx2RoleItem> Items
        {
            get
            {
                if (_items == null)
                {
                    ResetItems();
                }

                return _items;
            }
            set { _items = value; }
        }

        private List<Jyx2RoleItem> _items;


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
            if (count < 0)
            {
                Items.Remove(item);
            }

            if (item != null)
            {
                item.Count += count;
            }
            else
            {
                Items.Add(new Jyx2RoleItem() {Id = itemId, Count = count});
            }
        }


        public bool CanUseItem(int itemId)
        {
            return CanUseItem(ConfigTable.Get<Jyx2Item>(itemId));
        }

        /// <summary>
        /// 判断角色是否可以使用道具
        /// 
        /// 对应kyscpp：bool GameUtil::canUseItem(Role* r, Item* i)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanUseItem(Jyx2Item item)
        {
            if (item == null) return false;

            //剧情类无人可以使用
            if (item.ItemType == 0)
                return false;

            else if (item.ItemType == 1 || item.ItemType == 2)
            {
                if (item.ItemType == 2)
                {
                    //有仅适合人物，直接判断
                    if (item.OnlySuitableRole >= 0)
                    {
                        return item.OnlySuitableRole == int.Parse(this.Key);
                    }

                    //内力属性判断
                    if ((this.MpType == 0 || this.MpType == 1) && (item.NeedMPType == 0 || item.NeedMPType == 1))
                    {
                        if (this.MpType != item.NeedMPType)
                        {
                            return false;
                        }
                    }
                }

                //若有相关武学，满级则为假，未满级为真
                //若已经学满武学，则为假
                //此处注意，如果有可制成物品的秘籍，则武学满级之后不会再制药了，请尽量避免这样的设置
                if (item.Wugong > 0)
                {
                    int level = GetWugongLevel(item.Wugong);
                    //if (level >= 0 && level < GameConst.MAX_WUGONG_LEVEL)
                    //{
                    //    return true;
                    //}
                    if (level < 0 && this.Wugongs.Count >= GameConst.MAX_ROLE_WUGONG_COUNT)
                    {
                        return false;
                    }

                    if (level == GameConst.MAX_WUGONG_LEVEL)
                    {
                        return false;
                    }
                }

                //上面的判断未确定则进入下面的判断链
                return testAttr(this.Attack, item.ConditionAttack)
                       && testAttr(this.Qinggong, item.ConditionQinggong)
                       && testAttr(this.Heal, item.ConditionHeal)
                       && testAttr(this.UsePoison, item.ConditionPoison)
                       && testAttr(this.DePoison, item.ConditionDePoison)
                       && testAttr(this.Quanzhang, item.ConditionQuanzhang)
                       && testAttr(this.Yujian, item.ConditionYujian)
                       && testAttr(this.Shuadao, item.ConditionShuadao)
                       && testAttr(this.Qimen, item.ConditionQimen)
                       && testAttr(this.Anqi, item.ConditionAnqi)
                       && testAttr(this.Mp, item.ConditionMp)
                       && testAttr(this.IQ, item.ConditionIQ);
            }
            else if (item.ItemType == 3)
            {
                //药品类所有人可以使用
                return true;
            }
            else if (item.ItemType == 4)
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

        //getOrigin的意思是获取源数据，即get方法不使用limit
        private bool getOrigin;
        private int MaxAttack = GameConst.MAX_ROLE_ATTACK;
        private int MaxQinggong = GameConst.MAX_ROLE_QINGGONG;
        private int MaxDefence = GameConst.MAX_ROLE_DEFENCE;

        private void AddEffect(Jyx2Item item, int AddType)
        {
            if (item == null) return;
            MaxAttack += item.Attack * AddType;
            MaxDefence += item.Defence * AddType;
            MaxQinggong += (item.Qinggong>0?item.Qinggong:0) * AddType;
        }

        private GameRuntimeData runtime
        {
            get { return GameRuntimeData.Instance; }
        }

        /// <summary>
        /// 炼制物品
        /// </summary>
        /// <param name="item"></param>
        public string LianZhiItem(Jyx2Item practiseItem)
        {
            if (practiseItem == null)
                return "";
            
            if (practiseItem.GenerateItems[0].Id > 0 && ExpForMakeItem >= practiseItem.GenerateItemNeedExp &&
                runtime.HaveItemBool(practiseItem.GenerateItemNeedCost))
            {
                List<Jyx2RoleItem> makeItemList = new List<Jyx2RoleItem>();
                for (int i = 0; i < 5; i++)
                {
                    var generateItem = practiseItem.GenerateItems[i];
                    if (generateItem.Id >= 0)
                    {
                        makeItemList.Add(generateItem);
                    }
                }

                int index = Random.Range(0, makeItemList.Count);
                var item = makeItemList[index];
                runtime.AddItem(item.Id, item.Count);
                runtime.AddItem(practiseItem.GenerateItemNeedCost, -1);
                return $"{GetXiulianItem().Name} 炼出 {ConfigTable.Get<Jyx2Item>(item.Id.ToString()).Name}×{item.Count}";
            }

            return "";
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="item"></param>
        public void UseItem(Jyx2Item item)
        {
            if (item == null)
                return;

            if (item.EquipmentType == 0 || item.EquipmentType == 1)
                AddEffect(item, 1);

            this.Tili += item.AddTili;
            this.SetHPAndRefreshHudBar(this.Hp + item.AddHp);
            this.MaxHp += item.AddMaxHp;
            this.Mp += item.AddMp;
            this.MaxMp += item.AddMaxMp;
            this.Poison += item.ChangePoisonLevel;
            this.Heal += item.Heal;
            this.DePoison += item.DePoison;
            this.AntiPoison += item.AntiPoison;
            this.UsePoison += item.UsePoison;

            getOrigin = true;
            this.Attack += item.Attack;
            this.Defence += item.Defence;
            this.Qinggong += item.Qinggong;
            
            getOrigin = false;

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

            if (item.AttackFreq == 1)
            {
                this.Zuoyouhubo = 1;
            }

            int need_item_exp = GetFinishedExpForItem(item);
            if (this.ExpForItem >= need_item_exp)
            {
                this.LearnMagic(item.Wugong);
                this.ExpForItem -= need_item_exp;
            }

            this.Limit();
        }

        /// <summary>
        /// 卸下物品（装备,修炼）
        /// </summary>
        /// <param name="item"></param>
        public void UnequipItem(Jyx2Item item)
        {
            if (item == null)
                return;

            AddEffect(item, -1);

            item.User = -1;
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

            getOrigin = true;
            this.Attack -= item.Attack;
            this.Defence -= item.Defence;
            this.Qinggong -= item.Qinggong;
            getOrigin = false;

            this.Quanzhang -= item.Quanzhang;
            this.Yujian -= item.Yujian;
            this.Shuadao -= item.Shuadao;
            this.Qimen -= item.Qimen;

            this.Pinde -= item.AddPinde;
            this.AttackPoison -= item.AttackPoison;
            if (item.ChangeMPType == 2)
            {
                this.MpType = 2;
            }

            if (item.AttackFreq == 1)
            {
                this.Zuoyouhubo = 1;
            }

            int need_item_exp = GetFinishedExpForItem(item);
            if (this.ExpForItem >= need_item_exp)
            {
                this.LearnMagic(item.Wugong);
                this.ExpForItem -= need_item_exp;
            }

            this.Limit();
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
        public int GetFinishedExpForItem(Jyx2Item item)
        {
            if (item == null || item.ItemType != 2 || item.NeedExp < 0)
            {
                return GameConst.MAX_EXP;
            }

            int multiple = 7 - this.IQ / 15;
            if (multiple <= 0)
            {
                multiple = 1;
            }

            //有关联武学的，如已满级则不可修炼
            if (item.Wugong > 0)
            {
                int magic_level_index = GetWugongLevel(item.Wugong);
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

        //捏脸数据
        public string HeadAvata
        {
            get { return Get("HeadAvata", Data.GetHeadAvata()); }
            set { Save("HeadAvata", value); }
        }

        //模型组成数据
        public string ModelAvata
        {
            get { return Get("ModelAvata", Data.GetModel()); }
            set { Save("ModelAvata", value); }
        }

        public string ModelAsset
        {
            get { return Get(nameof(ModelAsset), Data.GetModelAsset()); }
            set { Save(nameof(ModelAsset), value); }
        }

        public ModelAsset Model;

        public string GetBattleAnimator()
        {
            return Data.GetBattleAnimator();
        }

        public Jyx2Role Data
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

        private Jyx2Role _data;

        public MapRole View;

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

        //是否已经行动
        public bool isActed = false;
        public bool isWaiting = false; //正在等待

        public void EnterBattle()
        {
            if (_isInBattle) return;
            
            _isInBattle = true;
            _currentSkill = null;

            View.LazyInitAnimator();

            if (Wugongs.Count > 0)
            {
                //默认使用第一个武功
                SwitchAnimationToSkill(Wugongs[0]);    
            }
        }

        public void SetHPAndRefreshHudBar(int hp)
        {
            Hp = hp;
            View?.MarkHpBarIsDirty();
        }

        private WugongInstance _currentSkill = null;

        public void SwitchAnimationToSkill(WugongInstance skill)
        {
            if (skill == null || _currentSkill == skill) return;

            //切换武功
            //View.ChangeWeaponTo(skill.GetWeaponCode(), Sex);

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

        //public void BattleAction()
        //{
        //    Loom.QueueOnMainThread((o) => { DoBattleAction(); }, null);
        //}

        ////本角色行动
        //void DoBattleAction()
        //{
        //    sp = 0;//清零集气槽
        //    BattleHelper.Instance.SetCurrentRole(this);
        //    BattleHelper.Instance.poisonEffect(this);

        //    this.ExpGot += 1;

        //    if (IsAI())
        //    {
        //        //隐藏所有格子
        //        BattleboxHelper.Instance.HideAllBlocks();
        //        BattleHelper.Instance.GetBattleActionFromAI(this, rst => { ExecuteAction(rst); });
        //        BattleHelper.Instance.SwitchStatesTo(BattleHelper.BattleViewStates.AI);
        //        BattleHelper.Instance.CreateAIAction(this, action =>
        //        {
        //            if (action.MoveTo != null)
        //            {
        //                BattleHelper.Instance.AIMove(this, action.MoveTo, delegate
        //                {
        //                    if (action.Skill != null && action.SkillTo != null)
        //                        BattleHelper.Instance.AIAttack(this, action.Skill, action.SkillTo);
        //                    else
        //                        BattleHelper.Instance.OnRestButtonClicked();
        //                });
        //            }
        //            else
        //            {
        //                BattleHelper.Instance.OnRestButtonClicked();
        //            }
        //        });
        //    }
        //    else
        //    {
        //        BattleHelper.Instance.GetBattleActionFromPlayer(this, rst => { ExecuteAction(rst); });
        //    }
        //}

        ////执行指令
        //void ExecuteAction(BattleAction action)
        //{
        //    //坐标位置改变
        //    this.Pos = action.MoveTo;
        //}

        //获得行动力
        public int GetMoveAbility()
        {
            if (Tili < 10)
                return 0; //金庸DOS版逻辑，体力小于10无法移动
            int speed = this.Qinggong;

            if (this.Weapon >= 0)
            {
                speed += this.GetWeapon().Qinggong;
            }

            if (this.Armor >= 0)
            {
                speed += this.GetArmor().Qinggong;
            }

            return speed / 15 + 1;
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

        //JYX2的休息逻辑，对应kyscpp  BattleScene::actRest
        public void OnRest()
        {
            Tili = Tools.Limit(Tili + 5, 0, GameConst.MaxTili);
            int tmpHp = Hp;
            Hp = Tools.Limit((int) (Hp + MaxHp * 0.05), 0, MaxHp);
            View?.MarkHpBarIsDirty();

            int tmpMp = Mp;
            Mp = Tools.Limit((int) (Mp + MaxMp * 0.05), 0, MaxMp);

            if (Hp > tmpHp)
                this.View.ShowAttackInfo($"<color=green>+{Hp - tmpHp}</color>");
            if (Mp > tmpMp)
                this.View.ShowAttackInfo($"<color=blue>+{Mp - tmpMp}</color>");
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


            WugongInstance w = new WugongInstance(magicId);
            Wugongs.Add(w);
            ResetZhaoshis();
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
    }
}