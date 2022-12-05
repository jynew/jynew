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
using System.IO;
using System.Text;
using i18n.TranslatorDef;
using Jyx2.Middleware;
using Jyx2.MOD;
using Jyx2Configs;
using UnityEngine;

namespace Jyx2
{

    public struct GameSaveSummary
    {
        public string Summary;
        public string ModId;
        public string ModName;

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ModId);
        }

        public static GameSaveSummary Load(int index)
        {
            var summaryInfoFilePath = GetSummaryFilePath(index);
            

            GameSaveSummary rst = new GameSaveSummary();
            //适配之前的存档
            try
            {
                rst.Summary = ES3.Load<string>("summary", summaryInfoFilePath);
                rst.ModId = ES3.Load<string>("modId", summaryInfoFilePath);
                rst.ModName = ES3.Load<string>("modName", summaryInfoFilePath);
            }
            catch (Exception e)
            {
                
                // TODO:没读取到则兼容旧版本，下个版本请删除代码
                try
                {
                    var oldSummaryInfoFilePath = RuntimeEnvSetup.CurrentModId + "_" +
                                                 string.Format(ARCHIVE_SUMMARY_FILE_NAME, index);
                    rst.Summary = ES3.Load<string>("summary", oldSummaryInfoFilePath);
                    rst.ModId = ES3.Load<string>("modId", oldSummaryInfoFilePath);
                    rst.ModName = ES3.Load<string>("modName", oldSummaryInfoFilePath);
                }
                catch (Exception ee)
                {
                    // ignored
                }
            }
            
            return rst;
        }

        public static void Save(int index, GameSaveSummary summary)
        {
            var summaryInfoFilePath = GetSummaryFilePath(index);
            ES3.Save("summary", summary.Summary, summaryInfoFilePath);
            ES3.Save("modId", summary.ModId, summaryInfoFilePath);
            ES3.Save("modName", summary.ModName, summaryInfoFilePath);
        }

        public string GetBrief()
        {
            if (string.IsNullOrEmpty(ModName))
            {
                return $"{Summary}";   
            }
            else
            {
                return $"{Summary} <size=20>[{ModName}]</size>";    
            }
        }

        public static string GetSummaryFilePath(int index)
        {
            var modDir = RuntimeEnvSetup.CurrentModId.ToLower();
            return modDir + "/" + string.Format(ARCHIVE_SUMMARY_FILE_NAME, index);
        }
        
        const string ARCHIVE_SUMMARY_FILE_NAME = "archive_summary_{0}.dat";
    }
    
    /// <summary>
    /// 这里是整个游戏的存档数据结构根节点
    ///
    /// </summary>
    [Serializable]
    public class GameRuntimeData 
    {
        #region 单例
        public static GameRuntimeData Instance {
            get
            {
                if(_instance == null)
                {
                    CreateNew();
                }
                return _instance;
            }
        }
        private static GameRuntimeData _instance;

        #endregion

        #region 存档数据定义
        //存档设置一个独立版本号，用来检查兼容性
        const int RUNTIME_VERSION_LATEST = 1;
        public int RuntimeVersion = RUNTIME_VERSION_LATEST;

        //JYX2，所有的角色都放到存档里
        [SerializeField] public Dictionary<int,RoleInstance> AllRoles = new Dictionary<int,RoleInstance>();
        
        //当前玩家队伍
        [SerializeField] private List<int> TeamId = new List<int>();
        [SerializeField] public SubMapSaveData SubMapData; //当前所处子地图存储数据
        [SerializeField] public WorldMapSaveData WorldData; //世界地图信息
        
        [SerializeField] public Dictionary<string, string> KeyValues = new Dictionary<string, string>(); //主键值对数据
        [SerializeField] public Dictionary<string, (int, int)> Items = new Dictionary<string, (int, int)>(); //JYX2物品，{ID，数量，获取时间}
        [SerializeField] public Dictionary<string, int> ItemUser= new Dictionary<string, int>(); //物品使用人，{物品ID，人物ID}
        [SerializeField] public Dictionary<string, int> ShopItems= new Dictionary<string, int>(); //小宝商店物品，{ID，数量}
        [SerializeField] public Dictionary<string, int> EventCounter = new Dictionary<string, int>();
        [SerializeField] public Dictionary<string, int> MapPic = new Dictionary<string, int>();
        [SerializeField] private List<int> ItemAdded = new List<int>(); //已经添加的角色物品

        #endregion

        #region JYX2

        //新建一个存档
        public static GameRuntimeData CreateNew()
        {
            _instance = new GameRuntimeData();
            var runtime = _instance;

            _instance.InitAllRole();

            var player = runtime.GetRole(0);
            
            //主角入当前队伍
            runtime.JoinRoleToTeam(0);

#if UNITY_EDITOR
            //可自由实现新的语法
            var content = File.ReadAllLines("CreateTeamDebug.txt");
            
            //初始技能
            foreach(var line in content)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;
                if (line.StartsWith("//")) continue;
                if (line.StartsWith("skills=")) //初始技能
                {
                    var tmp = line.Replace("skills=", "").Split('|');
                    foreach(var skill in tmp)
                    {
                        int skillId = int.Parse(skill.Split(',')[0]);
                        string level = skill.Split(',')[1];
                        player.LearnMagic(skillId);
                        var s = player.Wugongs.Find(p => p.Key == skillId);
                        if (s != null)
                        {
                            s.Level = int.Parse(level);
                        }
                    }
                }else if (line.StartsWith("teammates=")) //初始队友
                {
                    var tmp = line.Replace("teammates=", "").Split('|');
                    foreach (var roleId in tmp)
                    {
                        int role = int.Parse(roleId);
                        GameRuntimeData.Instance.JoinRoleToTeam(role);
                    }
                }else if (line.StartsWith("items")) //初始物品
                {
                    var tmp = line.Replace("items=", "").Split('|');
                    foreach (var item in tmp)
                    {
                        int itemId = int.Parse(item.Split(',')[0]);
                        int count = int.Parse(item.Split(',')[1]);
                        GameRuntimeData.Instance.AddItem(itemId, count);
                    }
                }else if (line.StartsWith("props")) //初始属性
                {
                    var tmp = line.Replace("props=", "").Split('|');
                    foreach (var prop in tmp)
                    {
                        string name = prop.Split(',')[0];
                        int value = int.Parse(prop.Split(',')[1]);
                        player.GetType().GetField(name).SetValue(player, value);
                    }
                }
            }

#endif
            return runtime;
        }

        void InitAllRole() 
        {
            //创建所有角色
            foreach (var r in GameConfigDatabase.Instance.GetAll<Jyx2ConfigCharacter>())
            {
                var role = new RoleInstance(r.Id);
                _instance.AllRoles.Add(r.Id, role);
            }
        }

        #endregion

        #region 游戏保存和读取


        public const string ARCHIVE_FILE_NAME = "archive_{0}.dat";

        public static string GetArchiveFile(int index)
        {
            var modDir = RuntimeEnvSetup.CurrentModId.ToLower();
            return modDir + "/" + string.Format(ARCHIVE_FILE_NAME, index);
        }


        public void GameSave(int index = -1)
        {
            Debug.Log("存档中.. index = " + index);
            SaveToFile(index);
            Debug.Log("存档结束");
        }

        public static DateTime? GetSaveDate(int index)
		{
            var summaryInfoFilePath = GameSaveSummary.GetSummaryFilePath(index);
            if (ES3.FileExists(summaryInfoFilePath))
            {
                return (DateTime?)ES3.GetTimestamp(summaryInfoFilePath);
            }

            // TODO:没读取到则兼容旧版本，下个版本请删除代码
            var oldSummaryInfoFilePath = RuntimeEnvSetup.CurrentModId + "_" + $"archive_summary_{index}.dat";
            if (ES3.FileExists(oldSummaryInfoFilePath))
            {
                return (DateTime?)ES3.GetTimestamp(oldSummaryInfoFilePath);
            }

            return null;
        }

	private void SaveToFile(int fileIndex)
        {
            //保存存档简介
            GameSaveSummary summary = new GameSaveSummary()
            {
                Summary = GenerateSaveSummaryInfo(),
                ModId = RuntimeEnvSetup.CurrentModId,
                ModName = RuntimeEnvSetup.CurrentModId,
            };
            GameSaveSummary.Save(fileIndex, summary);
            
            //存档
            var path = GetArchiveFile(fileIndex);
            ES3.Save("RuntimeVersion", _instance.RuntimeVersion, path);//用一个key单独存储版本号
            //ES3.Save("ModId", RuntimeEnvSetup.CurrentModId, path);//用一个key储存Mod名称
            ES3.Save("ModArchiveVersion", RuntimeEnvSetup.CurrentModConfig.ModArchiveVersion, path);//用一个key储存Mod存档版本号
            ES3.Save(nameof(GameRuntimeData), this, path);
        }

        public static GameRuntimeData LoadArchive(int fileIndex)
        {
            var path = GetArchiveFile(fileIndex);

            int archiveRtVersion = -1;
            //string archiveModId = "";
            int archiveModVersion = -1;

            GameRuntimeData runtime;

            if (ES3.FileExists(path))
            {
                //检查存档版本，进行兼容
                archiveRtVersion = ES3.Load<int>("RuntimeVersion", path, -1);
                //archiveModId = ES3.Load<int>("ModId", path, "");
                archiveModVersion = ES3.Load<int>("ModArchiveVersion", path, -1);
                runtime = ES3.Load<GameRuntimeData>(nameof(GameRuntimeData), path);
            }
            else
            {
                throw new Exception($"找不到存档文件 {fileIndex}");
            }
           
            //Debug.Log($"Runtime Version: {archiveVersion}");

            var IsOutdated = (archiveRtVersion < RUNTIME_VERSION_LATEST || archiveModVersion < RuntimeEnvSetup.CurrentModConfig.ModArchiveVersion);

            if (IsOutdated)
            {
                UpdateOldArchive(runtime, archiveRtVersion, archiveModVersion);
            }
 
            _instance = runtime;
            return runtime;
        }

        private static void UpdateOldArchive(GameRuntimeData runtime, int oldRtVersion, int oldModVersion)
        {
            if (oldRtVersion == -1)
            {
                //To Do: Runtime更新后在此填写修复代码
                runtime.RuntimeVersion = RUNTIME_VERSION_LATEST;
            }
            if (oldModVersion  < RuntimeEnvSetup.CurrentModConfig.ModArchiveVersion)
                //Lua事件，mod作者可以通过api监听此事件来进行存档更新
            Jyx2LuaBridge.DispatchLuaEvent("OnArchiveOutdated", runtime, oldModVersion);
        }

        private string GenerateSaveSummaryInfo()
        {
            string mapName = LevelMaster.GetCurrentGameMap().GetShowName();
            return $"{Player.Level}级,{mapName},队伍:{GetTeamMembersCount()}人";
        }

        #endregion
        
        #region 游戏运行时数据

        //主角
        public RoleInstance Player
        {
            get { return GetRole(0); }
        }

        //获取队伍角色Id列表
        public List<int> GetTeamId()
        {
            return TeamId;
        }

        //获取队伍所有角色
        public IEnumerable<RoleInstance> GetTeam()
        {
            foreach (var id in TeamId)
            {
                yield return AllRoles[id];
            }
        }

        //获取队伍人数
        public int GetTeamMembersCount()
        {
            return TeamId.Count;
        }

        public bool JoinRoleToTeam(int roleId,bool showGetItem = false)
        {
            if (GetRoleInTeam(roleId) != null)
            {
                Debug.LogError($"错误，角色重复入队：id = {roleId}");
                return false;
            }
            
            var role = GetRole(roleId);
            if(role == null)
            {
                Debug.LogError($"调用了不存在的role加入队伍，id = {roleId}");
                return false;
            }
            
            //获得角色身上的道具
            foreach (var item in role.Items)
            {
                if (!ItemAdded.Contains(item.Id))
                {
                    if (item.Count == 0) item.Count = 1;
                    AddItem(item.Id, item.Count);
                    ItemAdded.Add(item.Id);
                    if (item.Count > 0 && showGetItem)
                    {
                        //---------------------------------------------------------------------------
                        //StoryEngine.DisplayPopInfo("得到物品:" + item.Item.Name + "×" + Math.Abs(item.Count));
                        //---------------------------------------------------------------------------
                        //特定位置的翻译【得到物品提示】
                        //---------------------------------------------------------------------------
                        StoryEngine.DisplayPopInfo("得到物品:".GetContent(nameof(GameRuntimeData)) + GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(item.Id).Name + "×" + Math.Abs(item.Count));
                        //---------------------------------------------------------------------------
                        //---------------------------------------------------------------------------
                    }
                    item.Count = 0;
                }
            }

            //清空角色身上的装备
            role.Weapon = -1;
            role.Armor = -1;
            role.Xiulianwupin = -1;

            role.Items.Clear();   
            
            TeamId.Add(roleId);
            return true;
        }

        public bool LeaveTeam(int roleId) 
        {
            var role = GetRole(roleId);
            if (role == null)
            {
                Debug.LogError("调用了不存在的role加入队伍，roleid =" + roleId);
                return false;
            }
            if (GetRoleInTeam(roleId) ==null) 
            {
                Debug.LogError("role is not in main team，roleid =" + roleId);
                return false;
            }

            //卸下角色身上的装备，清空修炼
            role.UnequipItem(role.GetWeapon());
            role.Weapon = -1;
            role.UnequipItem(role.GetArmor());
            role.Armor = -1;
            SetItemUser(role.Xiulianwupin, -1);
            role.ExpForItem = 0;
            role.Xiulianwupin = -1;

            TeamId.Remove(roleId);
            role.Recover();
            return true;
        }

        public RoleInstance GetRoleInTeam(int roleId)
        {
            if (TeamId.Contains(roleId))
            {
                return AllRoles[roleId];
            }

            return null;
        }

        public bool IsRoleInTeam(int roleId)
        {
            return TeamId.Contains(roleId);
        }



        public RoleInstance GetRole(int roleId)
        {
            if(!AllRoles.ContainsKey(roleId))
            {
                Debug.LogError("无法获取RoleInstance, roleId: " + roleId);
                return null;
            }
            return AllRoles[roleId];
        }



        #region KeyValues

        public string GetKeyValues(string k)
        {
            return KeyValues[k];
        }

        public void SetKeyValues(string k, string v)
        {
            KeyValues[k] = v;
        }

        public void RemoveKey(string k)
        {
            KeyValues.Remove(k);
        }

        public bool KeyExist(string key)
        {
            if (KeyValues.ContainsKey(key) && KeyValues[key] != null)
                return true;
            return false;
        }
        #endregion



        public int GetMoney()
        {
            return GetItemCount(GameConst.MONEY_ID);
        }
        


        public bool HaveItemBool(int itemId)
        {
            return Items.ContainsKey(itemId.ToString());
        }

        //JYX2增加物品
        public void AddItem(string id, int count)
        {
            if (!Items.ContainsKey(id))
            {
                if(count < 0)
                {
                    Debug.LogError("扣了不存在的物品,id=" + id + ",count=" + count);
                    return;
                }
                Items[id] = (count, Tools.GetSecondsSince1970());
            }
            else
            {
                Items[id] = (Items[id].Item1 + count, Tools.GetSecondsSince1970());
                if(Items[id].Item1 == 0)
                {
                    Items.Remove(id);
                }else if(Items[id].Item1 < 0)
                {
                    Debug.LogError("物品扣成负的了,id=" + id + ",count=" + count);
                    Items.Remove(id);
                }
            }
        }

        //JYX2增加物品
        public void AddItem(int id, int count)
        {
            AddItem(id.ToString(), count);
        }


        public int GetItemCount(int id)
        {
            if (Items.ContainsKey(id.ToString()))
                return Items[id.ToString()].Item1;
            return 0;
        }

        public void RoleGetItem(int roleId, int itemId, int count)
        {
            //实现的逻辑是，如果这个角色在队里，则直接加到角色身上。否则加到一个存档记录里，在之后生成这个角色的时候再进行添加
            var teamRole = GetRole(roleId);
            if (teamRole != null)
            {
                teamRole.AddItem(itemId, count);
            }
        }

        //设置物品使用人
        public void SetItemUser(int itemId, int roleId)
        {
            ItemUser[itemId.ToString()] = roleId;
        }

        //获取物品使用人
        public int GetItemUser(int id)
        {
            if (ItemUser.ContainsKey(id.ToString()))
                return ItemUser[id.ToString()];
            return -1;
        }


        public void ModifyEvent(int scene, int eventId, int interactiveEventId, int useItemEventId, int enterEventId)
        {
            string key = "evt_" + scene + "_" + eventId;
            KeyValues[key] = string.Format("{0}_{1}_{2}", interactiveEventId, useItemEventId, enterEventId);
        }

        public string GetModifiedEvent(int scene,int eventId)
        {
            string key = "evt_" + scene + "_" + eventId;
            if (KeyValues.ContainsKey(key))
                return KeyValues[key];
            return null;
        }



        public void AddEventCount(int scene, int eventId, int eventName, int num)
        {
            string key=(string.Format("{0}_{1}_{2}", scene, eventId, eventName));
            if(EventCounter.ContainsKey(key)){
                EventCounter[key]+=num;
            }else{
                EventCounter[key]=num;
            }
        }
        
        public int GetEventCount(int scene, int eventId, int eventName)
        {
            string key=(string.Format("{0}_{1}_{2}", scene, eventId, eventName));
            if(EventCounter.ContainsKey(key)){
                return EventCounter[key];
            }
            return 0;
        }


        public void SetMapPic(int scene, int eventId, int pic)
        {
            string key=(string.Format("{0}_{1}", scene, eventId));
            if(MapPic.ContainsKey(key) && pic==-1){
                MapPic.Remove(key);
            }else{
                MapPic[key]=pic;
            }
        }
        
        public int GetMapPic(int scene, int eventId)
        {
            string key=(string.Format("{0}_{1}", scene, eventId));
            if(MapPic.ContainsKey(key)){
                return MapPic[key];
            }
            return -1;
        }


        //JYX2场景相关记录
        public Dictionary<string,string> GetSceneInfo(string scene)
        {
            string key = "scene_" + scene;
            if (KeyValues.ContainsKey(key))
            {
                string str = KeyValues[key];
                var rst = ES3.Deserialize<Dictionary<string, string>>(Encoding.UTF8.GetBytes(str));
                return rst;
            }
                
            return null;
        }

        public void SetSceneInfo(string scene, Dictionary<string, string> info)
        {
            if (info == null)
                return;
            string key = "scene_" + scene;

            var str = Encoding.UTF8.GetString(ES3.Serialize(info));
            KeyValues[key] = str;
        }

        /// <summary>
        /// 获取场景进入条件码
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public int GetSceneEntranceCondition(int mapId)
        {
            var gameMap = GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(mapId);
            if (gameMap == null) return -1;
            
            //大地图
            if (gameMap.Tags.Contains("WORLDMAP"))
                return 0;

            //已经有地图打开的纪录
            string key = "SceneEntraceCondition_" + gameMap.Id;
            if (KeyValues.ContainsKey(key))
            {
                return int.Parse(GetKeyValues(key));
            }

            //否则取配置表初始值
            return gameMap.EnterCondition;
        }

        /// <summary>
        /// 设置场景进入条件码
        /// </summary>
        public void SetSceneEntraceCondition(int mapId,int value)
        {
            string key = "SceneEntraceCondition_" + mapId;
            SetKeyValues(key, value.ToString());
        }
        #endregion

        private DateTime _startDate;
        public DateTime startDate{
            get {
                if (_startDate.Year == 1)
                {
                    _startDate = DateTime.Now;
                }
                return _startDate;
            }
            set { _startDate = value; }
        }
    }
}
