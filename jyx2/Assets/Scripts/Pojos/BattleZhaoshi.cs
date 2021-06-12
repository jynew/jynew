
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;
using UnityEngine;

namespace Jyx2
{
    [XmlType("battlezhaoshi")]
    public class BattleZhaoshi : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;

        [XmlAttribute("CoverType")]
        public int _coverType;

        public SkillCoverType CoverType
        {
            get { return (SkillCoverType)_coverType; }
        }

        [XmlAttribute]
        public int CastSize;

        [XmlAttribute]
        public float DamageFactor;

        [XmlAttribute]
        public int CoverSize;

        [XmlAttribute]
        public int Cooldown;


        [XmlAttribute]
        public string Icon;

    }
}
