using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("jyx2battle")]
    public class Jyx2Battle : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //名称

        [XmlAttribute]
        public int MapId; //地图编号

        [XmlAttribute]
        public int Exp; //经验

        [XmlAttribute]
        public int Music; //音乐

        [XmlElement]
        public List<Jyx2IntWrap> TeamMates; //队友

        [XmlElement]
        public List<Jyx2IntWrap> AutoTeamMates; //自动队友

        public bool HasAutoTeamMates()
        {
            return AutoTeamMates[0].Value >= 0;
        }

        [XmlElement]
        public List<Jyx2IntWrap> Enemies; //敌人
    }

    [XmlType]
    public class Jyx2IntWrap
    {
        [XmlAttribute]
        public int Value;
    }
}
