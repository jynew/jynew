using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Jyx2.MOD
{
    public class MODManager : MonoBehaviour
    {
        public static string ModsPath { get; private set; }
        internal static bool Initialized;
        
        void Start()
        {
            try
            {
                _Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void _Start()
        {
            if (!Initialize())
            {
                Debug.Log("初始化出错！");
                return;
            }

            if (Directory.Exists(ModsPath))
            {
                var mods = new Dictionary<string, ModEntry>();
                var pathList = new List<string>();
                FileTools.GetAllFilePath(ModsPath, pathList, new List<string>() { ".json" });
                foreach (var jsonPath in pathList)
                {
                    var modMeta = new ModMeta(jsonPath);
                    var filePath = Path.Combine(ModsPath, modMeta.id);
                    var modEntry = new ModEntry(modMeta, filePath);
                    mods.Add(modMeta.id, modEntry);
                    if (!File.Exists(filePath))
                        StartCoroutine(DownloadMod(modMeta.uri, filePath));
                }
                
                if (mods.Count > 0)
                {
                    foreach (var modEntry in mods.Values)
                        modEntry.Active = true;
                }
            }
        }

        public static bool Initialize()
        {
            if (Initialized) return true;
            Initialized = true;
            ModsPath = Path.Combine(Application.persistentDataPath, "mods");;
            if (!Directory.Exists(ModsPath))
            {
                Directory.CreateDirectory(ModsPath);
                Debug.Log($"已创建Mods文件夹：{ModsPath}。");
            }

            return Initialized;
        }
        
        private static IEnumerator DownloadMod(string uri, string path)
        {
            var uwr = UnityWebRequest.Get(uri);
            uwr.downloadHandler = new DownloadHandlerFile(path);
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError(uwr.error);
            else
                Debug.Log("File successfully downloaded and saved to " + path);
        }

        [Serializable]
        public class ModMeta
        {
            public string jsonPath;
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
            
            public ModMeta(string path)
            {
                jsonPath = path;
                InitData(path);
            }

            void InitData(string path)
            {
                string jsonTest = File.ReadAllText(path, Encoding.UTF8);
                try
                {
                    var modMeta = JsonUtility.FromJson<ModMeta>(jsonTest);
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
        
        public class ModEntry
        {
            public ModMeta ModMeta;
            public readonly string Path;
            public readonly Dictionary<string, string> Dependencies = new Dictionary<string, string>();
            public bool Loaded => File.Exists(Path);

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
            
            private bool _mActive;

            public bool Active
            {
                get => _mActive;
                set 
                {
                    if (value && Loaded)
                    {
                        _mActive = true;
                        Debug.Log("已激活MOD！");
                    }
                }
            }
        }
    }

}