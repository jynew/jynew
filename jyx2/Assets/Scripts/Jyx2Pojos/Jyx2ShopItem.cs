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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


using UnityEngine;

#if JYX2_USE_HSFRAMEWORK

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
#endif