using System;
using HSFrameWork.ConfigTable;
using System.Collections.Generic;
//using XLua;
using HanSquirrel.ResourceManager;
using GLib;
using HSFrameWork.Common;
using Jyx2.Crossplatform.BasePojo;

namespace Jyx2
{
    //[Hotfix]
    public class HSConfigTableInitHelperPhone : ConfigTableInitHelperShared
    {
        /// <summary> desKey不放在InitHelper里面是因为担心会被忽略。 </summary>
        public static IInitHelper Create()
        {
            return new HSConfigTableInitHelperPhone();
        }

        protected HSConfigTableInitHelperPhone() { }

        public override IEnumerable<Type> ProtoBufTypes
        {
            get
            {
                return _ClientAllTypes != null ? _ClientAllTypes :
                    (_ClientAllTypes = new List<Type>(_SharedTypes).AddRangeG(_ClientExtraTypes));
            }
        }

        public override Func<byte[]> LoadConfigTableData
        {
            get
            {
                return () => BinaryResourceLoader.LoadBinary(HSUnityEnv.CEValuesPath);
            }
        }

        private static List<Type> _ClientAllTypes;

        /// <summary>
        /// 客户端额外需要二进制序列化的类。
        /// </summary>
        private static readonly List<Type> _ClientExtraTypes = new List<Type>(){};
    }
}
