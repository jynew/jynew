using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using UnityEngine;

namespace Jyx2
{
    [XmlType("jyx2shop")]
    public class Jyx2Shop : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID
		
        [XmlAttribute]
        public int Trigger;

        [XmlElement]
        public List<Jyx2ShopItem> ShopItems; //商店所有物品
    }

    [XmlType]
    public class Jyx2ShopItem
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public int Count;

        [XmlAttribute]
        public int Price;//价格

        public Jyx2ShopItem Clone()
        {
            return new Jyx2ShopItem() { Id = this.Id, Count = this.Count ,Price = this.Price};
        }
    }
}
