using System;
using ProtoBuf;

namespace Jyx2Configs
{
    [Serializable]
    [ProtoContract]
    [ProtoInclude(2, typeof(Jyx2ConfigBase))]
    abstract public class Jyx2ConfigBase
    {
        //ID
        public int Id;
        
        //名称
        public string Name;
    }
}