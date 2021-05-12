using HSFrameWork.ConfigTable;
using System.Xml.Serialization;

namespace Jyx2
{
    [XmlType("attribute")]
    public class Attribute : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string Name;

        public static Attribute Get(string pk)
        {
            return ConfigTable.Get<Attribute>(pk);
        }
    }
}
