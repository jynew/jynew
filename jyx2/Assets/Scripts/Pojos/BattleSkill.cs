
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("battleskill")]
    public class BattleSkill : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string ZhaoshiList;

        [XmlAttribute]
        public string AnimationCode;
        
        public static BattleSkill Get(string key)
        {
            return ConfigTable.Get<BattleSkill>(key);
        }
        
        //获取技能所有的招式
        public IEnumerable<BattleZhaoshi> GetZhaoshis()
        {
            foreach(var zhaoshiId in ZhaoshiList.Split(','))
            {
                yield return ConfigTable.Get<BattleZhaoshi>(zhaoshiId);
            }
        }

    }
}
