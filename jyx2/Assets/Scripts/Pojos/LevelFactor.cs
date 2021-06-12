using System.Xml.Serialization;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using ObscuredInt=System.Int32;
using ObscuredFloat=System.Single;

namespace Jyx2
{
    /// <summary> 跨平台 </summary>
    [XmlType("levelfactor")]
    public class LevelFactor : BaseBean
    {
        public override string PK
        {
            get { return Level.ToString(); }
        }

        [XmlAttribute]
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        private ObscuredInt _level = 0;

        [XmlAttribute]
        public float Factor
        {
            get { return _factor; }
            set { _factor = value; }
        }
        private ObscuredFloat _factor = 0f;

        [XmlAttribute]
        public int ExpPerTili
        {
            get { return _expPerTili; }
            set { _expPerTili = value; }
        }
        private ObscuredInt _expPerTili = 0;

        [XmlAttribute]
        public int GuajiFactor
        {
            get { return _guajiFactor; }
            set { _guajiFactor = value; }
        }
        private ObscuredInt _guajiFactor = 0;

        [XmlAttribute]
        public int LevelUpExp
        {
            get { return _levelUpExp; }
            set { _levelUpExp = value; }
        }
        private ObscuredInt _levelUpExp = 0;

        [XmlAttribute]
        public int TotalExp
        {
            get { return _totalExp; }
            set { _totalExp = value; }
        }
        private ObscuredInt _totalExp = 0;

        public static LevelFactor Get(int level)
        {
            return ConfigTable.Get<LevelFactor>(level);
        }

        public static float GetFactor(int level)
        {
            return Get(level).Factor;
        }

        public static int GetTotalExp(int level)
        {
            return Get(level).TotalExp;
        }

        //指定等级升级以经验
        public static int GetLevelupExp(int level)
        {
            return Get(level).LevelUpExp;
        }
    }
}
