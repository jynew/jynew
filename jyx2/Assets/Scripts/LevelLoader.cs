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
using Cysharp.Threading.Tasks;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

            //记住当前的音乐，战斗后还原
            var formalMusic = AudioManager.GetCurrentMusic();

            LoadingPanel.Create(sceneName, ()=> {

                GameObject obj = new GameObject("BattleLoader");
                var battleLoader = obj.AddComponent<BattleLoader>();
                battleLoader.m_BattleId = battleId;
                battleLoader.Callback = (rst) =>
                {
                    AudioManager.PlayMusicAtPath(formalMusic).Forget();
                    callback(rst);
                };

            });
        }
    }
}
