using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

using ObscuredInt=System.Int32;
using ObscuredFloat=System.Single;

namespace Jyx2
{
    public enum ItemType
    {
        All = -2,
        Error = -1,
        Costa = 0,
        Weapon = 1,//武器
        Armor = 2,//防具
        Accessories = 3,//饰品
        Book = 4,
        Mission = 5,
        SpeicalSkillBook = 6,
        TalentBook = 7,
        Upgrade = 8,
        Special = 9,//特殊物品
        Canzhang = 10,//残章、残章(各种技能提升上限的材料)
        ExploreItem = 11,//探索物资
        Head = 12,
        Foot = 13,
        Waist = 14,
        Cailiao = 15,//装备材料
        XinfaBook = 16,//心法书
        IslandAutoUsed = 98,
        ExploreAutoUsed = 99,
        AutoUsed = 100,
        ShowOnly = 20,//仅用作展示，无法获得物品
        MapTreasure = 21, //探索地图随机宝箱，与出产地图等级绑定

        GongnengCost = 22, //功能型消耗道具
        GongnengForever = 23, //功能型永久道具

        WitchCraftMaterial = 24, //心诀升级的材料：器意

        JingYi = 71, //精义
        Cuiqu = 72, //装备萃取
        SPXinghun = 88,//祈福专用：人物列传及残页
        Zhenjie = 89,//武学心法真解及残页
    }

    public enum EquipmentItemType
    {
        All = 0,
        Quan = 1,
        Jian = 2,
        Dao = 3,
        Qimen = 4,
    }

    public enum ItemRare
    {
        White = 0,
        Blue = 1,
        Green = 2,
        Orange = 3,
        Purple = 4,
        Red = 5
    }

    [XmlType("item")]
    public class Item : BaseBean
    {
        public override string PK
        {
            get { return Key; }
        }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string SetItems;

        [XmlAttribute]
        public string Desc;

        [XmlAttribute]
        public string Icon;

        [XmlAttribute]
        public int Type;

        [XmlAttribute]
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private ObscuredInt _level = 0;

        [XmlAttribute]
        public int NoTarget = 0;

        public bool IsNoTarget()
        {
            return NoTarget == 1;
        }

        [XmlAttribute]
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }

        private ObscuredInt _price = 0;

        [XmlAttribute]
        public bool Drop;

        [XmlAttribute]
        public int StackSize
        {
            get { return _stackSize; }
            set { _stackSize = value; }
        }

        private ObscuredInt _stackSize = 0;

        //[XmlElement("talent")]
        //public List<TalentInstanceDelegate> talents;

        [XmlAttribute]
        public string SourceText;

        //[XmlElement("condition")]
        //public List<Condition> Requires;

        [XmlElement("trigger")]
        public List<KeyValue> Triggers
        {
            get
            {
                if (_triggers == null)
                    _triggers = new List<KeyValue>();
                return _triggers;
            }
            set
            {
                _triggers = value;
            }
        }

        [XmlIgnore]
        List<KeyValue> _triggers;

        //public bool HasTrigger(string name, string argvs)
        //{
        //    if (Triggers == null)
        //        return false;
        //    foreach (var t in Triggers)
        //    {
        //        if (t.Name == name && t.ArgvsString == argvs)
        //            return true;
        //    }
        //    return false;
        //}

        [XmlAttribute]
        public int Cooldown = 0;

        //用于定义各种物品的基础TAG，特别注意：与ItemInstance里的Tag是两码事。
        //本Tag中的所有定义属性， 在generate ItemInstance的时候会生效，因此不需要在ItemInstance的Tag中添加
        [XmlAttribute]
        public string Tag;

        [XmlIgnore]
        public string[] Tags
        {
            get
            {
                if (Tag != null)
                    return Tag.Split(new char[] { ',' });
                else
                    return null;
            }
        }

        public bool ContainsTag(string tag)
        {
            if (string.IsNullOrEmpty(Tag))
                return false;
            return Tags.Contains(tag);
        }

        [XmlAttribute]
        public string DropTemplate;

        [XmlAttribute]
        public string LengendText;

        [XmlIgnore]
        public bool IsLegend
        {
            get
            {
                return !string.IsNullOrEmpty(LengendText);
            }
        }

        public static List<Color> RareColors = new List<Color>() { Color.white, Color.blue, Color.green, Color.yellow, Color.magenta, Color.red };

        public static Item Get(string pk)
        {
            return ConfigTable.Get<Item>(pk);
        }

        public static Item RandomGetByLevel(int level)
        {
            var itemList = ConfigTable.GetAll<Item>().Where(item => { return (item.Level > level - 3 && item.Level < level + 3); }).ToList();
            if(itemList == null || itemList.Count == 0) return null;
            int index = ToolsShared.GetRandomInt(0, itemList.Count - 1);
            return itemList[level];
        }

        public ItemDropTemplate GetDropTemplate()
        {
            return ConfigTable.Get<ItemDropTemplate>(DropTemplate);
        }
    }
}
