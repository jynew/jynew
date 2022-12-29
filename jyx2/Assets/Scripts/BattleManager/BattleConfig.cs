using System;
using System.Collections.Generic;
using Jyx2;
using UnityEngine;

namespace Jyx2
{
    public class BattleConfig : LBattleConfig
    {
        public int Id {get;set;}
        public string Name{get;set;}

        //地图
        public string MapScene{get;set;}
        
        //获得经验
        public int Exp{get;set;}
        
        //音乐
        public int Music{get;set;}
        
        //队友
        public List<int> TeamMates{get;set;}

        //自动队友
        public List<int> AutoTeamMates{get;set;}

        //敌人
        public List<int> Enemies{get;set;}

        //动态生成队友
        public List<RoleInstance> DynamicTeammate {get;set;}
        //动态生成的敌人
        public List<RoleInstance> DynamicEnemies {get;set;}

        public void InitForDynamicData()
        {
            DynamicTeammate = new List<RoleInstance>();
            DynamicEnemies = new List<RoleInstance>();
        }
        
        public BattleConfig()
        {
        }

        public BattleConfig(LBattleConfig b)
        {

            Id = b.Id;
            Name = b.Name;
            MapScene = b.MapScene;
            Exp = b.Exp;
            Music = b.Music;
            TeamMates = b.TeamMates;
            AutoTeamMates = b.AutoTeamMates;
            Enemies = b.Enemies;

            DynamicTeammate = b.DynamicTeammate;
            DynamicEnemies = b.DynamicEnemies;
        }
    }
}
