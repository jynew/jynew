using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using Hanjiasongshu;
using HanSquirrel.ResourceManager;
using UnityEngine;

namespace Hanjiasongshu.ThreeD.XML
{
    [XmlRoot("root")]
    public class WeaponSet
    {
        [XmlElement("WeaponNode")]
        public List<WeaponData> List { get; set; }


        public static WeaponSet CreateFromXml(string xmlPath)
        {
            //var xml = File.ReadAllText(xmlPath);
            var xml = ResourceLoader.LoadAsset<TextAsset>(xmlPath).text;

            var obj = Tools.DeserializeXML<WeaponSet>(xml);
            obj.Init();
            return obj;
        }

        static WeaponSet _weaponSet = null;
        public static WeaponSet Get()
        {
            if(_weaponSet == null)
            {
                _weaponSet = CreateFromXml(PathConst.WeaponSetPath);
            }
            return _weaponSet;
        }

        public static void Clear()
        {
            _weaponSet = null;
        }


        [XmlIgnore]
        private Dictionary<int, WeaponData> m_AllWeaponDic = new Dictionary<int, WeaponData>();
        [XmlIgnore]
        public List<WeaponData> NanWeaponList = new List<WeaponData>();
        [XmlIgnore]
        public List<WeaponData> NvWeaponList = new List<WeaponData>();


        public void Init()
        {
            m_AllWeaponDic.Clear();

            NanWeaponList.Clear();
            NvWeaponList.Clear();
            for (int i = 0; i < List.Count; i++)
            {
                WeaponData data = List[i];
                if (data.Sex == 0)
                {
                    NanWeaponList.Add(data);
                }
                else
                {
                    NvWeaponList.Add(data);
                }
                m_AllWeaponDic.Add(List[i].ID, List[i]);
            }
        }

        public WeaponData GetById(int id)
        {
            WeaponData data = null;
            m_AllWeaponDic.TryGetValue(id, out data);
            return data;
        }

        public WeaponData GetByName(string name, int sex = 0)
        {
            return List.FirstOrDefault(x => x.Name == name && x.Sex == sex);
        }

        public WeaponData GetByWeaponTypeAndSex(WeaponEnum weaponType, int sex = 0)
        {
            return List.FirstOrDefault(x => (x.WeaponType == weaponType && x.Sex == sex));
        }
    }

    [XmlType("WeaponNode")]
    public class WeaponData
    {
        //武器名称
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        //武器类型，单双手
        [XmlAttribute("WeaponType")]
        public WeaponEnum WeaponType { get; set; }
        //武器名称
        [XmlAttribute("LeftName")]
        public string LeftName { get; set; }
        [XmlAttribute("RightName")]
        public string RightName { get; set; }
        //性别
        [XmlAttribute("Sex")]
        public int Sex { get; set; }
        //武器类型，单双手
        [XmlAttribute("Type")]
        public WeaponType Type { get; set; }
        //位置
        [XmlAttribute("PosX")]
        public float PosX { get; set; }
        [XmlAttribute("PosY")]
        public float PosY { get; set; }
        [XmlAttribute("PosZ")]
        public float PosZ { get; set; }
        //武器旋转
        [XmlAttribute("LeftRotX")]
        public float LeftRotX { get; set; }
        [XmlAttribute("LeftRotY")]
        public float LeftRotY { get; set; }
        [XmlAttribute("LeftRotZ")]
        public float LeftRotZ { get; set; }
        [XmlAttribute("RightRotX")]
        public float RightRotX { get; set; }
        [XmlAttribute("RightRotY")]
        public float RightRotY { get; set; }
        [XmlAttribute("RightRotZ")]
        public float RightRotZ { get; set; }
    }

    public enum WeaponType
    {
        None = 0,
        Left = 1,//左手
        Right = 2,//右手
        Double = 3,//双手
    }

    public enum WeaponEnum
    {
        TakeOff = - 1, //脱掉
        BigSword = 0,//大剑
        Bow = 1,//弓箭
        Dagger = 2,//刺法
        DouKinf = 3,//双刀
        Cudgel = 4,//棍法
        Gun = 5,//火枪
        HFH = 6,//黄飞鸿
        HidWea = 7,//暗器
        Leg = 8,//腿法
        Lute = 9,//琴或琵琶
        Palm = 10,//掌法
        Scourge = 11,//鞭法
        Shield = 12,//盾
        SinKnif = 13,//单刀
        SinSword = 14,//单手剑
        Spear = 15,//枪
    }

}
