using HSFrameWork.ConfigTable;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Jyx2
{
    [XmlType("trigger")]
    public class Trigger : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string KeyParam;

        [XmlAttribute]
        public string CommonParam;

        public string[] CommonParams
        {
            get
            {
                return CommonParam.Split(',');
            }
        }

        [XmlAttribute]
        public string Rule;

        [XmlAttribute]
        public string Desc;

        [XmlAttribute]
        public string Tag;

        public static Trigger Get(string pk)
        {
            return ConfigTable.Get<Trigger>(pk);
        }
    }
}
