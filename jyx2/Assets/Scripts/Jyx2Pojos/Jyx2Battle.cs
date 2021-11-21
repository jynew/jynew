/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


#if JYX2_USE_HSFRAMEWORK

namespace Jyx2
{
    [XmlType("jyx2battle")]
    public class Jyx2Battle : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //名称

        [XmlAttribute]
        public int MapId; //地图编号

        [XmlAttribute]
        public int Exp; //经验

        [XmlAttribute]
        public int Music; //音乐

        [XmlElement]
        public List<Jyx2IntWrap> TeamMates; //队友

        [XmlElement]
        public List<Jyx2IntWrap> AutoTeamMates; //自动队友

        public bool HasAutoTeamMates()
        {
            return AutoTeamMates[0].Value >= 0;
        }

        [XmlElement]
        public List<Jyx2IntWrap> Enemies; //敌人
    }

    [XmlType]
    public class Jyx2IntWrap
    {
        [XmlAttribute]
        public int Value;
    }
}
#endif