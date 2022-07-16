using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    [ProtoInclude(3, typeof(Jyx2ConfigBattle))]
    [ProtoInclude(4, typeof(Jyx2ConfigCharacter))]
    [ProtoInclude(5, typeof(Jyx2ConfigItem))]
    [ProtoInclude(6, typeof(Jyx2ConfigMap))]
    [ProtoInclude(7, typeof(Jyx2ConfigShop))]
    [ProtoInclude(8, typeof(Jyx2ConfigSkill))]
    [ProtoInclude(9, typeof(Jyx2ConfigExtra))]
    [ProtoInclude(10, typeof(Jyx2ConfigSettings))]
    abstract public class Jyx2ConfigBase
    {
        //ID
        [ProtoMember(1)]
        public int Id;
        
        //名称
        [ProtoMember(2)]
        public string Name;
    }
}