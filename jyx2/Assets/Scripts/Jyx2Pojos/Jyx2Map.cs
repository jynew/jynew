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
    [XmlType("jyx2map")]
    public class Jyx2Map : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id; //ID

        [XmlAttribute]
        public string Name; //名称

        [XmlAttribute]
        public int OutMusic; //出门音乐

        [XmlAttribute]
        public int InMusic; //进门音乐

        [XmlAttribute]
        public int TransportToMap; //跳转场景

        [XmlAttribute]
        public int EnterCondition; //进入条件
    }
}
#endif