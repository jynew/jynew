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
    }
}