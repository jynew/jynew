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
using UnityEngine;

namespace Jyx2
{
    public enum BattleResult
    {
        Win,
        Lose,
        InProgress,
    }
    
    public class BattleFieldModel
    {
        //战斗结果

        //行动集气
        const float ActionSp = 1000f;

        //参与战斗的角色
        public List<RoleInstance> Roles = new List<RoleInstance>();

        public List<RoleInstance> AliveRoles
        {
            get
            {
                var roleList = Roles.FindAll(role => !role.IsDead());
                roleList.Sort();
                return roleList;
            }
        }

        //队友
        public List<RoleInstance> Teammates
        {
            get
            {
                return Roles.FindAll((role) => role.team == 0);
            }
        }

        //敌人
        public List<RoleInstance> Enemys
        {
            get
            {
                return Roles.FindAll((role) => role.team > 0);
            }
        }

        //战斗结果回调
        public Action<BattleResult> Callback;

        //初始化战场
        public void InitBattleModel() 
        {
            ResetAct();//重置角色行动条
            SortRole();//角色排序
        }

        //增加一个战斗角色
        public void AddBattleRole(RoleInstance role, BattleBlockVector pos, int team, bool isAI)
        {
            role.BattleModel = this;
            role.Pos = pos;
            role.team = team;
            role.isAI = isAI;
            role.isActed = false;
            role.isWaiting = false;
            if (!Roles.Contains(role)) Roles.Add(role);
        }

        public bool BlockHasRole(int x, int y)
        {
            return BlockRoleTeam(x, y) != -1;
        }

        public int BlockRoleTeam(int x, int y)
        {
            var role = GetAliveRole(new BattleBlockVector(x, y));
            if (role != null) return role.team;
            return -1;
        }

        public RoleInstance GetAliveRole(BattleBlockVector vec)
        {
            foreach(var r in Roles)
            {
                if (r.IsDead()) continue;
                if(r.Pos.Equals(vec))
                {
                    return r;
                }
            }
            return null;
        }

        //战斗是否结束
        public BattleResult GetBattleResult()
        {
            Dictionary<int, int> teamCount = new Dictionary<int, int>();
            foreach(var role in Roles)
            {
                if (role.IsDead()) continue;
                
                if(!teamCount.ContainsKey(role.team))
                    teamCount.Add(role.team, 0);

                teamCount[role.team]++;
            }

            //战斗进行中
            if (teamCount.Keys.Count > 1)
                return BattleResult.InProgress;

            //我方有角色，胜利
            if (teamCount.ContainsKey(0))
                return BattleResult.Win;

            //敌方有角色，失败
            return BattleResult.Lose;
        }
        
        //角色排序
        void SortRole() 
        {
            if (!GameConst.SEMI_REAL)
            {
                Roles.Sort((roleA, roleB) =>
                {
                    if (roleA.Qinggong != roleB.Qinggong)
                    {
                        return roleB.Qinggong.CompareTo(roleA.Qinggong);
                    }
                    else
                    {
                        return roleA.GetJyx2RoleId().CompareTo(roleB.GetJyx2RoleId());
                    }
                });
            }
            else 
            {
                Roles.Sort((roleA, roleB) =>
                {
                    if (roleA.sp != roleB.sp)
                    {
                        return roleB.sp.CompareTo(roleA.sp);
                    }
                    else
                    {
                        return roleA.GetJyx2RoleId().CompareTo(roleB.GetJyx2RoleId());
                    }
                });
            }
        }

        void ResetAct() 
        {
            foreach (var item in Roles)
            {
                item.isActed = false;
                item.sp = 0;
            }
        }

        //角色选择之后
        public void OnActioned(RoleInstance role) 
        {
            if (role.isWaiting) 
            {
                role.isWaiting = false;
                return;
            }
            role.isActed = true;
            role.sp -= GameConst.ACTION_SP;
            //插入到末尾
            Roles.Remove(role);
            Roles.Add(role);
        }

        //角色等待
        public void ActWait(RoleInstance role) 
        {
            if (GameConst.SEMI_REAL)//半即时制 不能等待
                return;
            int index = -1;
            //插入到第一个已经行动过的前面
            for (int i = 0; i < Roles.Count; i++)
            {
                if (Roles[i].isActed) 
                {
                    index = i;
                    break;
                }
            }
            if (index <= 0)
                index = Roles.Count;
            role.isWaiting = true;
            Roles.Insert(index,role);
            Roles.RemoveAt(0);
        }

        RoleInstance GetNextRoleInTurnBase()//回合制
        {
            RoleInstance role = Roles[0];

            while (role.IsDead())
            {
                if(role.View != null)
                {
                    GameObject.Destroy(role.View.gameObject);
                }
                Roles.RemoveAt(0);
                role = Roles[0];
            }

            if (role.isActed) //全部都行动过了
            {
                ResetAct();
                SortRole();
                role = Roles[0];
                return role;
            }
            return role;
        }

        RoleInstance GetNextRoleInSemiReal()//半即时制
        {
            //无限循环，找下一个行动角色
            while (true)
            {
                foreach (var role in Roles)
                {
                    if (role.IsDead()) continue;
                    if (role.sp > ActionSp)
                        return role;
                }

                //依次增加
                foreach (var role in Roles)
                {
                    if (role.IsDead()) continue;
                    role.TimeRun();
                }
            }
        }

        //获得下一个行动的角色
        public RoleInstance GetNextActiveRole()
        {
            RoleInstance role;
            if (!GameConst.SEMI_REAL)//纯回合制的情况
                role = GetNextRoleInTurnBase();
            else
                role = GetNextRoleInSemiReal();
            return role;
        }

        //判断角色是不是最后一个行动的 如果是最后一个行动的 其实不能等待
        public bool IsLastRole(RoleInstance role) 
        {
            int index = Roles.IndexOf(role);
            index++;
            if (index >= Roles.Count)
                return true;
            RoleInstance nextRole = Roles[index];
            return nextRole.isActed;
        }
    }
}
