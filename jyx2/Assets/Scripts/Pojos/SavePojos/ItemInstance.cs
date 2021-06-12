using HSFrameWork.SPojo;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2
{
    public class ItemInstance : SaveablePojo
    {
        public ItemInstance() { }

        public ItemInstance(bool ignoreSubmit) : base(ignoreSubmit) { }

        public ItemInstance(string itemKey)
        {
            Key = itemKey;
        }

        //Key
        public string Key
        {
            get { return Get("Key", string.Empty); }
            set { Save("Key", value); }
        }

        public int Count
        {
            get { return Get("Count", 1); }
            set { Save("Count", value); }
        }

        //归属角色
        public string Owner
        {
            get { return Get("Owner", string.Empty); }
            set { Save("Owner", value); }
        }

        public double BornMF
        {
            get { return Get("BornMF", 0); }
            set { Save("BornMF", value); }
        }

        public List<TriggerInstance> AdditionTriggers
        {
            get { return GetList<TriggerInstance>("AdditionTriggers"); }
            set { SaveList("AdditionTriggers", value); }
        }

        public ItemType Type
        {
            get
            {
                return (ItemType)ItemData.Type;
            }
        }

        public Item ItemData
        {
            get
            {
                if (_item == null) _item = Item.Get(Key);
                return _item;
            }
        }
        private Item _item;

        public Dictionary<string, TriggerInstance> Triggers
        {
            get
            {
                if (_triggers == null)
                {
                    _triggers = new Dictionary<string, TriggerInstance>();
                    foreach(var kv in ItemData.Triggers)
                    {
                        _triggers.Add(kv.Key, new TriggerInstance(kv.Key, kv.Value));
                    }
                    foreach (var t in AdditionTriggers)
                    {
                        if (_triggers.ContainsKey(t.Key))
                        {
                            _triggers[t.Key].KeyArgv += t.KeyArgv;
                        }
                        else
                        {
                            _triggers.Add(t.Key, t);
                        }
                    }
                }
                return _triggers;
            }
        }

        private Dictionary<string, TriggerInstance> _triggers;

        public void ResetTriggers()
        {
            _triggers = null;
        }

        public ItemRare GetRare()
        {
            if (ItemData.IsLegend)
            {
                return ItemRare.Red;
            }

            if (AdditionTriggers.Count > 0)
            {
                switch (AdditionTriggers.Count)
                {
                    case 1:
                            return ItemRare.Blue;
                    case 2:
                            return ItemRare.Green;
                    case 3:
                            return ItemRare.Orange;
                    case 4:
                            return ItemRare.Purple;
                }
            }
            
            return ItemRare.White;
        }

        public bool IsEuippment
        {
            get
            {
                return Type == ItemType.Weapon || Type == ItemType.Armor || Type == ItemType.Accessories ||
                       Type == ItemType.Head || Type == ItemType.Waist || Type == ItemType.Foot;
            }
        }

        public ItemDropTemplate GetItemInstanceDropTemplate()
        {
            return ItemData.GetDropTemplate();
        }

        public static void AddRandomTrigger(ItemInstance item)
        {
            if (!item.IsEuippment) return;
            ItemDropTemplate template = item.GetItemInstanceDropTemplate();//item.getDropTemplate();
            if (template == null) return;

            double teamMf = 1;//GameRuntimeData.Instance.TeamMF;
            item.BornMF = (int)(teamMf * 100);

            var triggers = template.GenerateItemTriggers(teamMf, 0.5f);

            if (triggers != null && triggers.Count > 0)
            {
                item.AdditionTriggers.AddRange(triggers);
            }
        }

        public static ItemInstance Generate(string itemKey, bool setRandomTrigger = false, bool ignoreSubmit = false)
        {
            ItemInstance rst = new ItemInstance(ignoreSubmit)
            {
                Key = itemKey,
            };
            rst.Count = 1;

            //只有装备才需要随机词条、不可分解等
            if (rst.IsEuippment && setRandomTrigger) AddRandomTrigger(rst);
            return rst;
        }

        public string GetDesc()
        {
            string rst = $"<size=32>{ItemData.Name}</size>\n\n";
            if (ItemData != null) rst += ItemData.Desc;
            if (Triggers != null && Triggers.Count > 0)
            {
                rst += "\n\n";
                foreach (var t in Triggers.Values)
                {
                    if (t.TriggerData == null) continue;
                    rst += t.Desc + "\n";
                }
            }
            return rst;
        }
    }
}
