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
using Jyx2Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2
{
    public class LevelLoader
    {
        //加载地图
        public static void LoadGameMap(Jyx2ConfigMap map, LevelMaster.LevelLoadPara para = null, Action callback = null)
        {
            LevelMaster.loadPara = para != null ? para : new LevelMaster.LevelLoadPara(); //默认生成一份

            var runtime = GameRuntimeData.Instance;
            
            //存储结构
            if (runtime != null)
            {
                //存储上一个地图
                runtime.PrevMap = runtime.CurrentMap;

                //切换当前地图
                if (map != null)
                {
                    runtime.CurrentMap = map.Id.ToString();
                }
            }

            DoLoad(map, callback).Forget();
        }

        static async UniTask DoLoad(Jyx2ConfigMap map, Action callback)
        {
            LevelMaster.SetCurrentMap(map);
            await LoadingPanel.Create(map.MapScene);
            callback?.Invoke();
        }
        
        //加载战斗
        public static void LoadBattle(int battleId, Action<BattleResult> callback)
        {
            var battle = GameConfigDatabase.Instance.Get<Jyx2ConfigBattle>(battleId);
            if (battle == null)
            {
                Debug.LogError($"战斗id={battleId}未定义");
                return;
            }
            
            DoloadBattle(battle, callback).Forget();
        }

        private static async UniTask DoloadBattle(Jyx2ConfigBattle battle, Action<BattleResult> callback)
        {
            var formalMusic = AudioManager.GetCurrentMusic(); //记住当前的音乐，战斗后还原

            LevelMaster.IsInBattle = true;
            await LoadingPanel.Create(battle.MapScene);
                
            GameObject obj = new GameObject("BattleLoader");
            var battleLoader = obj.AddComponent<BattleLoader>();
            battleLoader.m_BattleId = battle.Id;
                
            //播放之前的地图音乐
            battleLoader.Callback = (rst) =>
            {
                LevelMaster.IsInBattle = false;
                AudioManager.PlayMusic(formalMusic);
                callback(rst);
            };
        }
        
        
    }
}
