using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Hanjiasongshu;
using HanSquirrel.ResourceManager;
using UnityEngine;

namespace Hanjiasongshu.ThreeD.XML
{
    [XmlRoot("root")]
    public class RoleModelSet
    {
        [XmlElement("XiakeNode")]
        public List<XiakeNode> List { get; set; }

        public static RoleModelSet CreateFromXml(string xmlPath)
        {
            //var xml = File.ReadAllText(xmlPath);
            var xml = ResourceLoader.LoadAsset<TextAsset>(xmlPath).text;
            var obj = Tools.DeserializeXML<RoleModelSet>(xml);
            obj.Init();
            return obj;
        }

        static RoleModelSet _roleModelSet = null;
        public static RoleModelSet Get()
        {
            if (_roleModelSet == null)
            {
                _roleModelSet = CreateFromXml(PathConst.RoleModelSetPath);
            }
            return _roleModelSet;
        }

        public static void Clear()
        {
            _roleModelSet = null;
        }

        public void Init() { }

        public string GetFilePathByName(string gameName)
        {
            foreach (var xiakeNode in List)
            {
                if (xiakeNode.GameName == gameName)
                    return xiakeNode.FilePath;
            }

            return "";
        }

        public XiakeNode GetByName(string name)
        {
            foreach (var xiakeNode in List)
            {
                if (xiakeNode.GameName == name)
                    return xiakeNode;
            }

            return null;
        }
    }

    [XmlType("XiakeNode")]
    public class XiakeNode
    {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }
        [XmlAttribute("GameName")]
        public string GameName { get; set; }

        [XmlAttribute("Sex")]
        public int Sex { get; set; }

        /// <summary>
        /// 0:H2
        /// 1:原生配置的
        /// </summary>
        [XmlAttribute("AvataType")]
        public int AvataType = 0;
    }

}