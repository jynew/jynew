using HSFrameWork.ConfigTable;
using System.Xml.Serialization;

namespace Jyx2
{
    [XmlType("kv")]
    public class KeyValue : BaseBean
    {
        public override string PK
        {
            get { return Key; }
        }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string Value;
    }
}
