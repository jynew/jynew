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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using HSFrameWork.SPojo;
using Jyx2;
using UnityEngine;

namespace Jyx2
{
    /// <summary>
    /// 这里是整个游戏的存档数据结构根节点
    ///
    /// </summary>
    public class GameRuntimeData : SaveablePojo
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
        public static GameRuntimeData Create(int index)
        {
            _instance = Saveable.Facade.CreateRoot<GameRuntimeData>();
            _instance.SaveIndex = index;
            _instance.InitAllRole();
            return _instance;
        }
        #endregion


        #region JYX2

        //新建一个存档
        public static GameRuntimeData CreateNew()
        {
            _instance = Saveable.Facade.CreateRoot<GameRuntimeData>();
            _instance.SaveIndex = 100; // 不重要
            var runtime = _instance;

            runtime.TeamLevel = 1;
            _instance.InitAllRole();

            var player = runtime.GetRole(0);
            //主角入当前队伍
            runtime.Team.Add(runtime.GetRole(0));

#if JYX2_TEST
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
                }
            }

#endif
            return runtime;
        }

        void InitAllRole() 
        {
            //创建所有角色
            foreach (var r in ConfigTable.GetAll<Jyx2Role>())
            {
                var role = new RoleInstance(r.Id);
                _instance.AllRoles.Add(role);
            }
        }

        #endregion

        #region 游戏保存和读取

        public int SaveIndex = 0;

        public const string ARCHIVE_FILE_NAME = "archive_{0}.dat";
        public const string ARCHIVE_FILE_DIR = "Save";
        public const string ARCHIVE_SUMMARY_PREFIX = "save_summaryinfo_";

        public void GameSave(int index = -1)
        {
            if (index != -1)
                SaveIndex = index;

            Debug.Log("存档中.. index = " + SaveIndex);
            SaveToFile(SaveIndex);
            Debug.Log("存档结束");

            var summaryInfo = GenerateSaveSummaryInfo();
            PlayerPrefs.SetString(ARCHIVE_SUMMARY_PREFIX + SaveIndex, summaryInfo);
        }

        public void SaveToFile(int fileIndex)
        {
            string fileName = string.Format(ARCHIVE_FILE_NAME, fileIndex);
            string sFolderPath = Application.persistentDataPath + "/" + ARCHIVE_FILE_DIR;
            if (!Directory.Exists(sFolderPath))
            {
                Directory.CreateDirectory(sFolderPath);
            }
            string sPath = sFolderPath + "/" + fileName;

            using (MemoryStream ms = new MemoryStream())
            {
                //写入存档内容
                TDES tdesTool = new TDES();
                tdesTool.Init(ConStr.DES_KEY);

                Hashtable hsData = Save();
                byte[] strBuffer = System.Text.Encoding.Default.GetBytes(hsData.toJson());
                byte[] encryptDatas = tdesTool.Encrypt(strBuffer);
                ms.Write(BitConverter.GetBytes(encryptDatas.Length), 0, sizeof(int));
                ms.Write(encryptDatas, 0, encryptDatas.Length);

                File.WriteAllBytes(sPath, ms.GetBuffer());
            }
        }

        static string GetArchiveFilePath(int fileIndex)
        {
            string fileName = string.Format(ARCHIVE_FILE_NAME, fileIndex);
            string sFolderPath = Application.persistentDataPath + "/" + ARCHIVE_FILE_DIR;
            string sPath = sFolderPath + "/" + fileName;
            return sPath;
        }

		// fix load empty savedata will init all role with internal data
		// modified by eaphone at 2021/06/01
        public static GameRuntimeData LoadArchive(int fileIndex)
        {	
            try
            {
                GameRuntimeData tagArchive = null;
				var summaryInfoKey = ARCHIVE_SUMMARY_PREFIX + fileIndex;
				if (PlayerPrefs.HasKey(summaryInfoKey))
				{
					string sPath = GetArchiveFilePath(fileIndex);
					if (File.Exists(sPath))
					{
						using (FileStream fs = File.OpenRead(sPath))
						{
							byte[] buffer1 = new byte[sizeof(int)];
							fs.Read(buffer1, 0, sizeof(int));
							int dataLen = BitConverter.ToInt32(buffer1, 0);
							byte[] archiveData = new byte[dataLen];
							fs.Read(archiveData, 0, dataLen);

							TDES tdesTool = new TDES();
							tdesTool.Init(ConStr.DES_KEY);

							byte[] decryptData = tdesTool.Decrypt(archiveData);
							string txtData = System.Text.Encoding.Default.GetString(decryptData, 0, decryptData.Length);
							Hashtable hsData = txtData.hashtableFromJson();
							tagArchive = Saveable.Facade.LoadRoot<GameRuntimeData>(hsData);

							fs.Close();
						}
					}
					_instance = tagArchive;//记录单例
					_instance.SaveIndex = fileIndex;
					_instance.InitAllRole();
                }
                return tagArchive;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        private string GenerateSaveSummaryInfo()
        {
            string mapName = "";
            if (!string.IsNullOrEmpty(CurrentMap))
            {
				// modified by eaphone at 2021/05/22
                mapName=LevelMaster.Instance.GetCurrentGameMap().GetShowName();
            }

            return $"{TeamLevel}级,{mapName},队伍:{Team.Count}人";
        }

        #endregion
        
        #region 游戏运行时数据

        //主角
        public RoleInstance Player
        {
            get
            {
                if (Team != null && Team.Count > 0) return Team[0];
                return null;
            }
        }

        //主角队伍
        public List<RoleInstance> Team
        {
            get { return GetList<RoleInstance>("Team"); }
            set { SaveList("Team", value); }
        }

        public bool JoinRoleToTeam(int roleId)
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
            
            Team.Add(role);
            return true;
        }

        public void LeaveTeam(int roleId) 
        {
            var role = GetRole(roleId);
            if (role == null)
            {
                Debug.LogError("调用了不存在的role加入队伍，roleid =" + roleId);
                return;
            }
            if (GetRoleInTeam(roleId) ==null) 
            {
                Debug.LogError("role is not in main team，roleid =" + roleId);
                return;
            }
            Team.Remove(role);
			role.Recover();
			for(var index=0;index< AllRoles.Count;index++){
				if(AllRoles[index].Key==role.Key){
					AllRoles[index]=role;
				}
			}
			
			StoryEngine.Instance.DisplayPopInfo(role.Name + "离队。");
        }

        public RoleInstance GetRoleInTeam(int roleId)
        {
            return Team.Find(r => r.Key == roleId.ToString());
        }

        public bool IsRoleInTeam(int roleId)
        {
            return GetRoleInTeam(roleId) != null;
        }

        //JYX2，所有的角色都放到存档里
        public List<RoleInstance> AllRoles
        {
            get { return GetList<RoleInstance>("AllRoles"); }
            set { SaveList("AllRoles", value); }
        }

        public RoleInstance GetRole(int roleId)
        {
            return AllRoles.Find(r => r.Key == roleId.ToString());
        }

        //当前队伍
        public List<RoleInstance> CurrentTeam
        {
            get {
               var list = GetList<RoleInstance>("CurrentTeam");
                if(list.Count == 0)
                {
                    list.Add(Team[0]);
                }
                return list;
            }
            set { SaveList("CurrentTeam", value); }
        }

        //主键值对数据
        public SaveableStrDictionary KeyValues
        {
            get { return GetPojoAutoCreate<SaveableStrDictionary>("KeyValues"); }
            set { SavePojo("KeyValues", value); }
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


        //之前所在的地图
        public string PrevMap
        {
            get { return Get("PrevMap", string.Empty); }
            set { Save("PrevMap", value); }
        }

        //当前所处地图
        public string CurrentMap
        {
            get { return Get("CurrentMap", string.Empty); }
            set { Save("CurrentMap", value); }
        }

        //当前位置
        public string CurrentPos
        {
            get { return Get("CurrentPos", string.Empty); }
            set { Save("CurrentPos", value); }
        }
		
        //当前位置
        public string CurrentOri
        {
            get { return Get("CurrentOri", string.Empty); }
            set { Save("CurrentOri", value); }
        }

        //世界位置
        public string WorldPosition
        {
            get { return Get("WorldPosition", string.Empty); }
            set { Save("WorldPosition", value); }
        }
        public string WorldRotation
        {
            get { return Get("WorldRotation", string.Empty); }
            set { Save("WorldRotation", value); }
        }


        //船的世界位置
        public string BoatWorldPos
        {
            get { return Get("BoatWorldPos", string.Empty); }
            set { Save("BoatWorldPos", value); }
        }

        //船的朝向
        public string BoatRotate
        {
            get { return Get("BoatRotate", string.Empty); }
            set { Save("BoatRotate", value); }
        }

        //是否在船上
        public int OnBoat
        {
            get { return Get("OnBoat", 0); }
            set { Save("OnBoat", value); }
        }

        //队伍经验
        public int TeamExp
        {
            get { return Get("TeamExp", 1); }
            set { Save("TeamExp", value); }
        }

        //队伍等级
        public int TeamLevel
        {
            get { return Get("TeamLevel", 1); }
            set { Save("TeamLevel", value); }
        }
        

        public int GetMoney()
        {
            return GetItemCount(GameConst.MONEY_ID);
        }
        


        //JYX2物品，{ID，数量}
        public SaveableNumberDictionary<int> Items
        {
            get { return GetPojoAutoCreate<SaveableNumberDictionary<int>>("Items"); }
            set { SavePojo("Items", value); }
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
                Items[id] = count;
            }
            else
            {
                Items[id] += count;
                if(Items[id] == 0)
                {
                    Items.Remove(id);
                }else if(Items[id] < 0)
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
                return Items[id.ToString()];
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

		public SaveableNumberDictionary<int> EventCounter
		{
			get {return GetPojoAutoCreate<SaveableNumberDictionary<int>>("EventCounter");}
			set {SavePojo("EventCounter", value);}
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
		
		public SaveableNumberDictionary<int> MapPic
		{
			get {return GetPojoAutoCreate<SaveableNumberDictionary<int>>("MapPic");}
			set {SavePojo("MapPic", value);}
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
                return str.ToStrDict();
            }
                
            return null;
        }

        public void SetSceneInfo(string scene, Dictionary<string, string> info)
        {
            if (info == null)
                return;
            string key = "scene_" + scene;
            string str = info.toJson();
            KeyValues[key] = str;
        }

        /// <summary>
        /// 获取场景进入条件码
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public int GetSceneEntranceCondition(string scene)
        {
            scene = scene.Split('&')[0]; //处理一些特殊格式的

            var gamemap = ConfigTable.Get<GameMap>(scene);
            if(gamemap != null)
            {
                //大地图
                if (gamemap.Tags == "WORLDMAP")
                    return 0;

                //已经有地图打开的纪录
                string key = "SceneEntraceCondition_" + gamemap.Jyx2MapId;
                if (KeyValues.ContainsKey(key))
                {
                    return int.Parse(GetKeyValues(key));
                }

                //否则取配置表初始值
                var map = ConfigTable.Get<Jyx2Map>(gamemap.Jyx2MapId);
                if (map != null)
                {
                    return map.EnterCondition;
                }
            }
            
            return -1;
        }

        /// <summary>
        /// 设置场景进入条件码
        /// </summary>
        /// <param name="scene"></param>
        public void SetSceneEntraceCondition(string scene,int value)
        {
            string key = "SceneEntraceCondition_" + scene;
            SetKeyValues(key, value.ToString());
        }

        #endregion
    }
}
