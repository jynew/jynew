using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Jyx2.MOD.ModV2
{
    public class GameModEditor : GameModBase
    {
        public override UniTask<AssetBundle> LoadModAb()
        {
            throw new System.NotImplementedException();
        }

        public override UniTask<AssetBundle> LoadModMap()
        {
            throw new System.NotImplementedException();
        }

        public override string GetDesc()
        {
            return $"[{Tag}]{RootConfig.ModName}({Id.ToLower()})";
        }
        
        protected override string Tag => "Editor";

        public MODRootConfig RootConfig;
    }


    public class GameModEditorLoader : GameModLoader
    {
        public override async UniTask<List<GameModBase>> LoadMods()
        {
            List<GameModBase> rst = new List<GameModBase>();
#if UNITY_EDITOR
            var find = AssetDatabase.FindAssets("ModSetting", new string[] {"Assets/Mods/"});

            foreach (var assetId in find)
            {
                var configFile = AssetDatabase.LoadAssetAtPath<MODRootConfig>(AssetDatabase.GUIDToAssetPath(assetId));
                if (configFile == null) continue;
                
                GameModInfo info = new GameModInfo() {Id = configFile.ModId, Name = configFile.ModName};
                rst.Add(new GameModEditor() {Info = info, RootConfig = configFile});
            }
#endif
            return rst;
        }
        
        public List<GameModBase> LoadModsSync()
        {
            List<GameModBase> rst = new List<GameModBase>();
#if UNITY_EDITOR
            var find = AssetDatabase.FindAssets("ModSetting", new string[] {"Assets/Mods/"});

            foreach (var assetId in find)
            {
                var configFile = AssetDatabase.LoadAssetAtPath<MODRootConfig>(AssetDatabase.GUIDToAssetPath(assetId));
                if (configFile == null) continue;
                
                GameModInfo info = new GameModInfo() {Id = configFile.ModId, Name = configFile.ModName};
                rst.Add(new GameModEditor() {Info = info, RootConfig = configFile});
            }
#endif
            return rst;
        }
    }
}