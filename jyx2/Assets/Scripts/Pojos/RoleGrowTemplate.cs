using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using System.Xml.Serialization;
using ObscuredInt=System.Int32;
using ObscuredFloat=System.Single;

namespace Jyx2
{
    [XmlType("rolegrowtemplate")]
    public class RoleGrowTemplate : BaseBean
    {
        public override string PK { get { return Key; } }

        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public float HpFactor
        {
            get { return _hpFactor; }
            set { _hpFactor = value; }
        }
        private ObscuredFloat _hpFactor;

        [XmlAttribute]
        public float Mp
        {
            get { return _mp; }
            set { _mp = value; }
        }
        private ObscuredFloat _mp;

        [XmlAttribute]
        public float Wuxing
        {
            get { return _wuxing; }
            set { _wuxing = value; }
        }
        private ObscuredFloat _wuxing;

        [XmlAttribute]
        public float Shenfa
        {
            get { return _shenfa; }
            set { _shenfa = value; }
        }
        private ObscuredFloat _shenfa;

        [XmlAttribute]
        public float Bili
        {
            get { return _bili; }
            set { _bili = value; }
        }
        private ObscuredFloat _bili;

        [XmlAttribute]
        public float Gengu
        {
            get { return _gengu; }
            set { _gengu = value; }
        }
        private ObscuredFloat _gengu;

        [XmlAttribute]
        public float Fuyuan
        {
            get { return _fuyuan; }
            set { _fuyuan = value; }
        }
        private ObscuredFloat _fuyuan;

        [XmlAttribute]
        public float Dingli
        {
            get { return _dingli; }
            set { _dingli = value; }
        }
        private ObscuredFloat _dingli;

        [XmlAttribute]
        public float Quanzhang
        {
            get { return _quanzhang; }
            set { _quanzhang = value; }
        }
        private ObscuredFloat _quanzhang;

        [XmlAttribute]
        public float Jianfa
        {
            get { return _jianfa; }
            set { _jianfa = value; }
        }
        private ObscuredFloat _jianfa;

        [XmlAttribute]
        public float Daofa
        {
            get { return _daofa; }
            set { _daofa = value; }
        }
        private ObscuredFloat _daofa;

        [XmlAttribute]
        public float Qimen
        {
            get { return _qimen; }
            set { _qimen = value; }
        }
        private ObscuredFloat _qimen;

        [XmlAttribute]
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        private string _tag;
    }
}
