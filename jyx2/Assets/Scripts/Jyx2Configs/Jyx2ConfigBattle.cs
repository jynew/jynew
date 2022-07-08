using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigBattle : Jyx2ConfigBase
    {
        //地图
        [ProtoMember(1)]
        public string MapScene;
        
        //获得经验
        [ProtoMember(2)]
        public int Exp;
        
        //音乐
        [ProtoMember(3)]
        public int Music; //音乐
        
        //队友
        [ProtoMember(4)]
        public string TeamMates;

        //自动队友
        [ProtoMember(5)]
        public string AutoTeamMates;

        //敌人
        [ProtoMember(6)]
        public string Enemies;
    }
}