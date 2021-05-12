///* TILE WORLD CREATOR TileWorldTileStore
// * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
// * 
// * Create awesome tile worlds in seconds.
// *
// * 
// * Documentation: http://tileworldcreator.doofortyfour.com
// * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
// * Web: http://www.doorfortyfour.com
// * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
// * share your awesome work you've made with TileWorldCreator
//*/
//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Xml.Serialization;

//namespace TileWorld
//{

//    public class TileWorldTileStore : EditorWindow
//    {

//        static string url = "http://tileworldcreator.doorfortyfour.com/availabletiles.xml";
//        static WWW www;


//        [System.Serializable]
//        public class FileInfo
//        {
//            [System.Serializable]
//            public class Package
//            {
//                public string name;
//                public string imgLink;
//                public string storeLink;

//                public Package() { }
//                public Package(string _imgLink, string _storeLink)
//                {
//                    imgLink = _imgLink;
//                    storeLink = _storeLink;
//                }
//            }

//            public List<Package> packages;

//            public FileInfo()
//            {
//                packages = new List<Package>();
//            }
//        }

//        static public FileInfo fileInfo;
//        static string content;
//        static bool downloadReady = false;
//        static List<Texture2D> images = new List<Texture2D>();
//        static int index;
//        Vector2 scrollPosition;
//        Texture2D storebannner;

//        void OnEnable()
//        {
//            minSize = new Vector2(320f, 555f);
//            maxSize = new Vector2(320f, 555.1f);

//            LoadResources();
//        }

//        public static void InitStore()
//        { 

//            downloadReady = false;
//            images = new List<Texture2D>();

//            index = 0;

//            // download file
//            Download(url);

//            // Get existing open window or if none, make a new one:
//            TileWorldTileStore window = (TileWorldTileStore)EditorWindow.GetWindow(typeof(TileWorldTileStore));
//        }

//        void OnGUI()
//        {
//            // DEBUG
//            /*
//            if (GUILayout.Button("Save"))
//            {
//                var _n = new FileInfo();
//                _n.packages.Add(new FileInfo.Package("img","http://www.google.com"));
//                _n.packages.Add(new FileInfo.Package("img2", "store2"));
//                AddData<FileInfo>(_n);
       
//                FileStream _fs = new FileStream(Path.Combine(Application.dataPath, "availabletiles.xml"), FileMode.Create, FileAccess.Write);
//                StreamWriter _sw = new StreamWriter(_fs);

//                _sw.Write(content);

//                _sw.Flush();
//                _sw.Close();
//            }
//            */

//            GUILayout.BeginVertical();
//            GUILayout.Label(storebannner);
//            GUILayout.Label("Here you'll find new tile packages for download, which are compatible with TileWorldCreator.", "Box");
//            GUILayout.EndVertical();

//            if (GUILayout.Button("Refresh"))
//            {
//                InitStore();
//            }

//            if (downloadReady)
//            {
//                ShowAvailableTiles();
//            }
//            else
//            {
//                GUILayout.Label("loading...");
//            }

//            Repaint();
//        }

//        void ShowAvailableTiles()
//        {
//            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

//            for (int i = 0; i < fileInfo.packages.Count; i++)
//            {
//                if (images.Count > 0 && i < images.Count)
//                {
//                    if (GUILayout.Button(images[i]))
//                    {
//                        Application.OpenURL(fileInfo.packages[i].storeLink);
//                    }
//                }
//                else
//                {
//                    if (GUILayout.Button(fileInfo.packages[i].name))
//                    {
//                        Application.OpenURL(fileInfo.packages[i].storeLink);
//                    }
//                }
//            }

//            GUILayout.EndScrollView();
//        }

//        static void DownloadTextures(int _index)
//        {
//            Download(fileInfo.packages[_index].imgLink);
//        }

//        // download file in editor
//        static void Download(string _url)
//        {
//            www = new WWW(_url);

//    #if UNITY_EDITOR
//            if (!EditorApplication.isPlaying)
//                EditorApplication.update = WaitForDownload;
//    #else
//             StartCoroutine(WaitForDownloadIE());
//    #endif

//        }

//    #if UNITY_EDITOR
//        static void WaitForDownload()
//        {
//            if (www.isDone)
//            {
//                EditorApplication.update = null;
//                LoadComplete();
//            }
//        }
//    #endif

//        static IEnumerator WaitForDownloadIE()
//        {
//            yield return www;
//            LoadComplete();
//        }

//        static void LoadComplete()
//        {
//            if (www.error != null)
//            {
//                Debug.Log(www.error.ToString());
//            }
//            else
//            {
//                if (www.texture.width == 8)
//                {
//                    fileInfo = GetData<FileInfo>(www.text);
//                    // store ready
//                    downloadReady = true;

//                    DownloadTextures(0);
//                }
//                else
//                {
//                    images.Add(www.texture);

//                    //download next texture
//                    if (index + 1 < fileInfo.packages.Count)
//                    {
//                        index++;
//                        DownloadTextures(index);
//                    }
//                }
//            }
//        }

//        static T GetData<T>(string _data)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(T));
//            StringReader _sr = new StringReader(_data);

//            return (T)serializer.Deserialize(_sr);
//        }

//        static void AddData<T>(T _data)
//        {
//            StringWriter _sw = new StringWriter();

//            XmlSerializer _serializer = new XmlSerializer(typeof(T));
//            _serializer.Serialize(_sw, _data);
//            content += _sw;
//            _sw.Close();
//        }

//        void LoadResources()
//        {
//            var _path = ReturnInstallPath.GetInstallPath("Editor", this); // GetInstallPath();

//            storebannner = AssetDatabase.LoadAssetAtPath(_path + "Res/storebanner.png", typeof(Texture2D)) as Texture2D;
//        }
//    }
//}
