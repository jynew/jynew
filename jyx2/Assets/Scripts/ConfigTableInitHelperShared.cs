using System;
using HSFrameWork.ConfigTable;
using System.Collections.Generic;
using Jyx2;

namespace Jyx2.Crossplatform.BasePojo
{
    /// <summary>
    /// 客户端和服务端必须共享此实现，否则Values可能会无法正常加载。
    /// </summary>
    public abstract class ConfigTableInitHelperShared : DefaultInitHelper
    {
        public override IEnumerable<Type> ProtoBufTypes { get { return _SharedTypes; } }
        public override bool ShowExeTimer { get { return true; } }

        /// <summary>
        /// 服务端和客户端共同需要二进制序列化的类
        /// </summary>
        protected static readonly List<Type> _SharedTypes = new List<Type>(){
                //client
                typeof(GameMap),
                typeof(System.Numerics.Vector3),
                typeof(System.Numerics.Vector2),

                //jyx2
                typeof(Jyx2Role),
                typeof(Jyx2Map),
                typeof(Jyx2Item),
                typeof(Jyx2Skill),
                typeof(Jyx2RoleHeadMapping),
                typeof(Jyx2RoleWugong),
                typeof(Jyx2RoleItem),
                typeof(Jyx2SkillLevel),
                typeof(Jyx2SkillDisplay),
                typeof(Jyx2Battle),
                typeof(Jyx2IntWrap),
                typeof(Jyx2Shop),
                typeof(Jyx2ShopItem),
            };

        /// <summary>
        /// 将XML文件中的NODE和实际的POJO Class进行关联。请注意：InitBind的顺序和添加的顺序相同。
        /// </summary>
        protected override void BuildTypeNodes()
        {
            AddTypeNode<GameMap>("gamemap");

            //jyx2
            AddTypeNode<Jyx2Role>("jyx2role");
            AddTypeNode<Jyx2Map>("jyx2map");
            AddTypeNode<Jyx2Item>("jyx2item");
            AddTypeNode<Jyx2Skill>("jyx2skill");
            AddTypeNode<Jyx2RoleHeadMapping>("jyx2role_headMapping");
            AddTypeNode<Jyx2SkillDisplay>("jyx2skillDisplay");
            AddTypeNode<Jyx2Battle>("jyx2battle");
            AddTypeNode<Jyx2Shop>("jyx2shop");
        }
    }
}
