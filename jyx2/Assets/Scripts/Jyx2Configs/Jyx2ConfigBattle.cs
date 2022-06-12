using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Jyx2Configs
{
    public class Jyx2ConfigBattle : Jyx2ConfigBase
    {
        public static Jyx2ConfigBattle Get(int id)
        {
            return GameConfigDatabase.Instance.Get<Jyx2ConfigBattle>(id);
        }
        
        //地图
        public string MapScene;
        
        //获得经验
        public int Exp;
        
        //音乐
        public int Music; //音乐
        
        //队友
        public string TeamMates;

        //自动队友
        public string AutoTeamMates;

        //敌人
        public string Enemies;

        public override async UniTask WarmUp()
        {
        }
    }
}