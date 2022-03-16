using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/地图", fileName = "地图ID_地图名")]
    public class Jyx2ConfigMap : Jyx2ConfigBase
    {
        public static Jyx2ConfigMap Get(int id)
        {
            
            return GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(id);
        }
        
        [InfoBox("引用指定场景asset")]
        [LabelText("地图")]
        public AssetReference MapScene;

        [LabelText("进门音乐")]
        public AssetReferenceT<AudioClip> InMusic;
        
        [LabelText("出门音乐")]
        public AssetReferenceT<AudioClip> OutMusic;

        [InfoBox("0开局开启  1开局关闭")]
        [LabelText("进入条件")] 
        public int EnterCondition;

        [LabelText("标签")] 
        public string Tags;

        [HideInInspector] public int ForceSetLeaveMusicId = -1;
        
        public override async UniTask WarmUp()
        {
            _isWorldMap = Tags.Contains("WORLDMAP");
            _isNoNavAgent = Tags.Contains("NONAVAGENT");
        }
        
        public string GetShowName()
        {
            //---------------------------------------------------------------------------
            //if ("小虾米居".Equals(Name)) return GameRuntimeData.Instance.Player.Name + "居";
            //---------------------------------------------------------------------------
            //特定位置的翻译【小地图左上角的主角居显示】
            //---------------------------------------------------------------------------
            if (GlobalAssetConfig.Instance.defaultHomeName.Equals(Name)) return GameRuntimeData.Instance.Player.Name + "居".GetContent(nameof(Jyx2ConfigMap));
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            return Name;
        }
        
        //获得开场地图
        public static Jyx2ConfigMap GetGameStartMap()
        {
            foreach(var map in GameConfigDatabase.Instance.GetAll<Jyx2ConfigMap>())
            {
                if (map.Tags.Contains("START"))
                {
                    return map;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 是否是大地图
        /// </summary>
        /// <returns></returns>
        public bool IsWorldMap() { return _isWorldMap;}
        private bool _isWorldMap;
        
        /// <summary>
        /// 是否不能寻路
        /// </summary>
        /// <returns></returns>
        public bool IsNoNavAgent() { return _isNoNavAgent;}
        private bool _isNoNavAgent;

#if UNITY_EDITOR


        static Jyx2ConfigMap LoadInEditor(int id)
        {
            string path = "Assets/BuildSource/Configs/Maps";
            var asset = AssetDatabase.FindAssets($"{id}_*", new string[] {path});
            var loadPath = AssetDatabase.GUIDToAssetPath(asset[0]);
            return AssetDatabase.LoadAssetAtPath<Jyx2ConfigMap>(loadPath);
        }
        
        [Button("自动设置地图连接点")]
        public async UniTask OnAutoSetTransport()
        {
            var map = MapScene;

            EditorSceneManager.OpenScene($"Assets/Jyx2Scenes/{map.editorAsset.name}.unity");

            
            
            Debug.Log("processing...");
            //await GameConfigDatabase.Instance.Init();

            var levelMasterbooster = FindObjectOfType<LevelMasterBooster>();
            if (levelMasterbooster.m_GameMap == null)
            {
                levelMasterbooster.m_GameMap = this;
                levelMasterbooster.m_IsBattleMap = false;
            }
            
            foreach (var zone in FindObjectsOfType<MapTeleportor>())
            {
                if (zone.m_GameMap == null)
                {
                    //zone.m_GameMap = Jyx2ConfigMap.LoadInEditor(zone.TransportMapId);
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            
            Debug.Log("ok");
        }
#endif
    }
}
