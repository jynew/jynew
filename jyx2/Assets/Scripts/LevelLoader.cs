
using System;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;

namespace Jyx2
{
    public class LevelLoader
    {
        //加载地图
        public static void LoadGameMap(GameMap map, LevelMaster.LevelLoadPara para = null, string command = "", Action callback = null)
        {
            if (para == null)
                para = new LevelMaster.LevelLoadPara(); //默认生成一份

            LevelMaster.loadPara = para;

            //存储结构
            if (GameRuntimeData.Instance != null)
            {
                //存储上一个地图
                GameRuntimeData.Instance.PrevMap = GameRuntimeData.Instance.CurrentMap;

                //切换当前地图
                if (map != null)
                {
                    GameRuntimeData.Instance.CurrentMap = map.Key;
                    //GameRuntimeData.Instance.CurrentPos = "";

                    //非战斗场景清理数据
                    if (!map.Tags.Contains("BATTLE"))
                    {
                        //MapRuntimeData.Instance.Clear();

                        //清理重复角色
                        //GameRuntimeData.Instance.CurrentTeam.RemoveAll(role => !GameRuntimeData.Instance.Team.Contains(role));

                        //从GameRuntimeData复制队伍信息到地图数据
                        //if (map.Tags.Contains("PLAYER_ONLY"))
                        //{
                        //    MapRuntimeData.Instance.AddToExploreTeam(GameRuntimeData.Instance.CurrentTeam[0]);
                        //}
                        //else
                        //{
                        //    foreach (var role in GameRuntimeData.Instance.CurrentTeam)
                        //    {
                        //        MapRuntimeData.Instance.AddToExploreTeam(role);
                        //    }
                        //}

                        //初始化队伍状态
                        //foreach (var r in GameConst.MapTeam)
                        //{
                        //    r.Hp = r.Maxhp;
                        //}
                    }
                    //存档
                    //GameRuntimeData.Instance.GameSave();
                }
            }

            if (string.IsNullOrEmpty(command))
                LoadingPanel.Create(map.Key, callback);
            else
                LoadingPanel.Create($"{map.Key}&{command}", callback);            
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="levelKey"></param>
        /// <param name="fromPosTag">是否从存档中取出生点</param>
        public static void LoadGameMap(string levelKey, LevelMaster.LevelLoadPara para = null)
        {
            if (para == null)
                para = new LevelMaster.LevelLoadPara(); //默认生成一份
            var mapKey = levelKey.Contains("&") ? levelKey.Split('&')[0] : levelKey;
            var command = levelKey.Contains("&") ? levelKey.Split('&')[1] : "";
            LoadGameMap(ConfigTable.Get<GameMap>(mapKey), para, command);
        }

        //加载战斗
        public static void LoadBattle(int battleId, Action<BattleResult> callback)
        {
            var battle = ConfigTable.Get<Jyx2Battle>(battleId);

            string sceneName = "Jyx2Battle_" + battle.MapId;
            if(!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                sceneName = "BattleScene_hufeiju";
            }

            LoadingPanel.Create(sceneName, ()=> {

                GameObject obj = new GameObject("BattleLoader");
                var battleLoader = obj.AddComponent<BattleLoader>();
                battleLoader.m_BattleId = battleId;
                battleLoader.Callback = callback;
            });
        }
    }
}
