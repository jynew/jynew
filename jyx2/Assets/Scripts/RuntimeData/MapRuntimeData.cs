
using System.Collections.Generic;
using System.Linq;
using HSFrameWork.SPojo;

namespace Jyx2
{
    public class MapRuntimeData : SaveablePojo
    {
        public static MapRuntimeData Instance
        {
            get
            {
                return GameRuntimeData.Instance.MapRuntimeData;
            }
        }

        //主键值对数据
        public SaveableStrDictionary KeyValues
        {
            get { return GetPojoAutoCreate<SaveableStrDictionary>("KeyValues"); }
            set { SavePojo("KeyValues", value); }
        }

        public List<RoleInstance> ExploreTeam
        {
            get;
            private set;
        }

        public List<RoleInstance> NPCList
        {
            get { return Roles.Except(ExploreTeam).ToList(); }
        }

        public int ExploreSkillPoint
        {
            get { return Get("ExploreSkillPoint", 0); }
            set { Save("ExploreSkillPoint", value); }
        }

        public List<string> ActiveExploreSkill;

        public List<RoleInstance> Roles
        {
            get { return GetList<RoleInstance>("Roles"); }
            private set { SaveList("Roles", value); }
        }

        public float ExploreSpeed = 5;
        public float ExploreAngularSpeed = 360;
        public float ExploreAcceleration = 15;

        public override void Clear()
        {
            base.Clear();
            ExploreTeam = new List<RoleInstance>();
            ActiveExploreSkill = new List<string>();
        }

        public void AddMapRole(RoleInstance role)
        {
            if (!Roles.Contains(role))
                Roles.Add(role);
        }

        public void AddToExploreTeam(RoleInstance role)
        {
            if (!ExploreTeam.Contains(role)/* && ExploreTeam.Count < 5*/) ExploreTeam.Add(role);
        }
    }
}
