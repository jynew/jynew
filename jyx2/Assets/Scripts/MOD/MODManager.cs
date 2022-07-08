using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2.MOD
{
    public static class MODManager
    {
        public static readonly List<ModEntry> ModEntries = new List<ModEntry>();
        public static readonly string Http = "http://42.192.48.70:3001/getPassMods";
        public static string ModsPath { get; private set; }

        public static async UniTask Init()
        {
            ModsPath = Path.Combine(Application.persistentDataPath, "mods");
            if (!Directory.Exists(ModsPath))
            {
                Directory.CreateDirectory(ModsPath);
                Debug.Log($"已创建Mods文件夹：{ModsPath}。");
            }

            if (Directory.Exists(ModsPath))
            {
                try
                {
                    UnityWebRequest request = UnityWebRequest.Get(Http);
                    await request.SendWebRequest();
                    string textString = request.downloadHandler.text;
                    var response = new Response(textString);
                    var modMetas = response.data;
                    foreach (var modMeta in modMetas)
                    {
                        var filePath = Path.Combine(ModsPath, modMeta.id);
                        var modEntry = new ModEntry(modMeta, filePath);
#if UNITY_ANDROID
                    if (modMeta.platform == "Android")
                    {
                        ModEntries.Add(modEntry); 
                    }
#endif
#if UNITY_STANDALONE_WIN
                        if (modMeta.platform == "Windows")
                        {
                            ModEntries.Add(modEntry);
                        }
#endif
#if UNITY_STANDALONE_OSX
                    if (modMeta.platform == "MacOS")
                    {
                        ModEntries.Add(modEntry);
                    }
#endif
                    }
                
                    var pathList = new List<string>();
                    FileTools.GetAllFilePath(ModsPath, pathList, new List<string>() { ".json" });
                    foreach (var jsonPath in pathList)
                    {
                        var jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);
                        var modMeta = new ModMeta(jsonString);
                        var filePath = Path.Combine(ModsPath, modMeta.id);
                        var modEntry = new ModEntry(modMeta, filePath);
                        ModEntries.Add(modEntry);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        [Serializable]
        public class ModMeta
        {
            public string platform;
            public string name;
            public string id;
            public string version;
            public string latestVersion;
            public string tags;
            public string uri;
            public List<ModMeta> dependencies = new List<ModMeta>();
            public string description;
            public string poster;
            public string createDate;
            public string updateDate;

            public bool Equals(ModMeta other)
            {
                return id.Equals(other?.id);
            }

            public static implicit operator bool(ModMeta exists)
            {
                return exists != null;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ModMeta modMeta && Equals(modMeta);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
            
            public ModMeta(string jsonString)
            {
                InitData(jsonString);
            }

            void InitData(string jsonString)
            {
                try
                {
                    var modMeta = JsonUtility.FromJson<ModMeta>(jsonString);
                    name = modMeta.name;
                    id = modMeta.id;
                    version = modMeta.version;
                    latestVersion = modMeta.latestVersion;
                    tags = modMeta.tags;
                    uri = modMeta.uri;
                    dependencies = modMeta.dependencies;
                    description = modMeta.description;
                    poster = modMeta.poster;
                    createDate = modMeta.createDate;
                    updateDate = modMeta.updateDate;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        [Serializable]
        public class Response
        {
            public int code;
            public string msg;
            public List<ModMeta> data = new List<ModMeta>();
            
            public Response(string jsonString)
            {
                InitData(jsonString);
            }

            void InitData(string jsonString)
            {
                try
                {
                    var response = JsonUtility.FromJson<Response>(jsonString);
                    code = response.code;
                    msg = response.msg;
                    data = response.data;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public class ModEntry
        {
            public ModMeta ModMeta;
            public readonly string Path;
            public readonly Dictionary<string, string> Dependencies = new Dictionary<string, string>();

            public ModEntry(ModMeta modMeta, string path)
            {
                ModMeta = modMeta;
                Path = path;
                
                if (modMeta.dependencies == null || modMeta.dependencies.Count == 0) return;

                var regex = new Regex(@"(.*)-(\d+\.\d+\.\d+).*");
                foreach (var dependency in modMeta.dependencies)
                {
                    var match = regex.Match(dependency.id);
                    if (match.Success)
                    {
                        Dependencies.Add(match.Groups[1].Value, match.Groups[2].Value);
                        continue;
                    }
                    if (!Dependencies.ContainsKey(dependency.id)) Dependencies.Add(dependency.id, null);
                }
            }

            private bool _active;
            
            public bool Active
            {
                get
                {
                    return PlayerPrefs.GetInt(ModMeta.name) == 1 || _active;
                }
                set
                {
                    PlayerPrefs.SetInt(ModMeta.name, value ? 1 : 0);
                    PlayerPrefs.Save();
                    _active = value;
                }
            }
        }
    }

}