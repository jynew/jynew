
using System.Xml.Serialization;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using ObscuredInt=System.Int32;
using ObscuredFloat=System.Single;

namespace Jyx2
{
    [XmlType("role")]
    public class Role : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;
        
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Sex;

        [XmlAttribute]
        public int Level;

        [XmlAttribute]
        public string GrowTemplate;

        [XmlAttribute]
        public string Weapon;

        [XmlAttribute]
        public string Model;

        [XmlAttribute]
        public string HeadAvata;

        [XmlAttribute]
        public string Skill;

        [XmlAttribute]
        public string Tag;

        [XmlAttribute]
        public int MaxHp;

        [XmlAttribute]
        public int Wuxing
        {
            get { return _wuxing; }
            set { _wuxing = value; }
        }
        private ObscuredInt _wuxing;

        [XmlAttribute]
        public int Shenfa
        {
            get { return _shenfa; }
            set { _shenfa = value; }
        }
        private ObscuredInt _shenfa;

        [XmlAttribute]
        public int Bili
        {
            get { return _bili; }
            set { _bili = value; }
        }
        private ObscuredInt _bili;

        [XmlAttribute]
        public int Gengu
        {
            get { return _gengu; }
            set { _gengu = value; }
        }
        private ObscuredInt _gengu;

        [XmlAttribute]
        public int Fuyuan
        {
            get { return _fuyuan; }
            set { _fuyuan = value; }
        }
        private ObscuredInt _fuyuan;

        [XmlAttribute]
        public int Dingli
        {
            get { return _dingli; }
            set { _dingli = value; }
        }
        private ObscuredInt _dingli;

        [XmlAttribute]
        public int Quanzhang
        {
            get { return _quanzhang; }
            set { _quanzhang = value; }
        }
        private ObscuredInt _quanzhang;

        [XmlAttribute]
        public int Jianfa
        {
            get { return _jianfa; }
            set { _jianfa = value; }
        }
        private ObscuredInt _jianfa;

        [XmlAttribute]
        public int Daofa
        {
            get { return _daofa; }
            set { _daofa = value; }
        }
        private ObscuredInt _daofa;

        [XmlAttribute]
        public int Qimen
        {
            get { return _qimen; }
            set { _qimen = value; }
        }
        private ObscuredInt _qimen;

        public static Role Get(string pk)
        {
            return ConfigTable.Get<Role>(pk);
        }

        public RoleGrowTemplate GetGrowTemplate()
        {
            return ConfigTable.Get<RoleGrowTemplate>(GrowTemplate);
        }
    }
}
