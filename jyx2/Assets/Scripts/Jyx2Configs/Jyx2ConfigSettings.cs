using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigSettings : Jyx2ConfigBase
    {
        //值
        [ProtoMember(1)]
        public string Value;
    }
}