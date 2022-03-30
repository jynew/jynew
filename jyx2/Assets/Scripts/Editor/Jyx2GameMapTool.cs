using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.DemiLib;
using Jyx2Configs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Jyx2GameMapTool : Editor
{

    [MenuItem("Tools/金庸群侠传3D/处理当前地图连接")]
    public static void ProcessGameMapsTransportors()
    {
        Debug.Log("ProcessGameMapsTransportors called");
        CurrentMapTrans().Forget();
    }
    
    
    [MenuItem("Tools/金庸群侠传3D/处理所有当前地图连接")]
    public static async void ProcessAllGameMapsTransportors()
    {
        Debug.Log("ProcessGameMapsTransportors called");

        string path = "Assets/BuildSource/Configs/Maps";
        var assets = AssetDatabase.FindAssets($"*", new string[] {path});

        foreach (var uuid in assets)
        {
            var loadPath = AssetDatabase.GUIDToAssetPath(uuid);
            var map = AssetDatabase.LoadAssetAtPath<Jyx2ConfigMap>(loadPath);
            await map.OnAutoSetTransport();
        }
    }


    public static async UniTask CurrentMapTrans()
    {
        /*await GameConfigDatabase.Instance.Init();
        foreach (var zone in FindObjectsOfType<MapTeleportor>())
        {
            if (zone.m_GameMap == null)
            {
                //zone.m_GameMap = GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(zone.TransportMapId);
            }
        }*/
    }
}
