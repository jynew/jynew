using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("jyx2map")]
    public class Jyx2Map : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //名称

        [XmlAttribute]
        public int OutMusic; //出门音乐

        [XmlAttribute]
        public int InMusic; //进门音乐

        [XmlAttribute]
        public int TransportToMap; //跳转场景

        [XmlAttribute]
        public int EnterCondition; //进入条件
    }
}
