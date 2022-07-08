using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigExtra : Jyx2ConfigBase
    {
        //武器
        [ProtoMember(1)]
        public int Weapon;
        
        //武功
        [ProtoMember(2)]
        public int Wugong;
        
        //加成攻击
        [ProtoMember(3)]
        public int ExtraAttack;
    }
}