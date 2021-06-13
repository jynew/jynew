using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Jyx2.Middleware;

public sealed class AddScenesToBuildTool
{
    //private static readonly string __SCENES_DIR = Application.dataPath + "/Jyx2Scenes";
    private static readonly string __ASSETS_SCENES_DIR = "/sources/product/scene";

    // 需要搜索的场景路径
    private static readonly string[] __SCENES_DIR =
    {
        "/Jyx2Scenes",
        "/Jyx2BattleScene",
    };

    // 默认需要添加的场景, 就是不在上面路径里的
    private static string[] _scenesList =
    {
    };

    [MenuItem("Tools/场景/AddScenesToBuild")]
    public static void AddScenesToBuild()
    {
        for (int i = 0; i < __SCENES_DIR.Length; ++i)
        {
            if (!Directory.Exists(Application.dataPath + __SCENES_DIR[i]))
            {
                return;
            }
        }

        string _pattern = "*.unity";

        List<string> _files = new List<string>();

        for (int i = 0; i < __SCENES_DIR.Length; ++i)
        {
            FileTools.GetAllFilePath(Application.dataPath + __SCENES_DIR[i], _files, new List<string>()
            {
                ".unity"
            });
        }

        EditorBuildSettingsScene[] _buildSettingScene = new EditorBuildSettingsScene[_files.Count + _scenesList.Length];

        for (int i = 0; i < _files.Count; ++i)
        {
            // Debug.Log(_files[i]);
            string _fileName = Path.GetFileName(_files[i]);

            string _dirName = Path.GetDirectoryName(_files[i]);

            if (!string.IsNullOrEmpty(_dirName))
            {
                int _index = _dirName.Replace('\\', '/').LastIndexOf('/') + 1;

                if (_index > 0)
                {
                    _dirName = _dirName.Substring(_index);

                    // Debug.Log("_dirName: " + _dirName);
                    // Debug.Log(_fileName);

                    _fileName = _dirName + '/' + _fileName;

                    _buildSettingScene[i] = GetBuildSettingScene(_fileName);
                }
            }
        }

        for (int i = 0; i < _scenesList.Length; ++i)
        {
            _buildSettingScene[_files.Count + i] = new EditorBuildSettingsScene(_scenesList[i], true);
        }

        //将GameStart挪到第一个
        for (int i = 0; i < _buildSettingScene.Length; ++i)
        {
            if (_buildSettingScene[i].path.EndsWith("0_GameStart.unity"))
            {
                var tmp = _buildSettingScene[i];
                _buildSettingScene[i] = _buildSettingScene[0];
                _buildSettingScene[0] = tmp;
                break;
            }
        }
        
        // 设置场景
        EditorBuildSettings.scenes = _buildSettingScene;

        Debug.Log("场景添加完毕");
    }

    public static EditorBuildSettingsScene GetBuildSettingScene(string _sceneName)
    {
        string _sceneAssetPath = "Assets/" + _sceneName;
        // Debug.Log(_sceneAssetPath);

        return new EditorBuildSettingsScene(_sceneAssetPath, true);
    }
}