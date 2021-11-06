using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

/// <summary>
/// 游戏视角管理器
/// </summary>
public class GameViewPortManager : MonoBehaviour
{
    public enum ViewportType
    {
        Topdown = 0, //顶视角
        Follow = 1, //跟随视角
    }
    
    public static GameViewPortManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                GameObject.DontDestroyOnLoad(obj);
                obj.name = "[GameViewPortManager]";
                _instance = obj.AddComponent<GameViewPortManager>();

                if (PlayerPrefs.HasKey("viewport_type"))
                {
                    _instance._viewportType = (ViewportType) (PlayerPrefs.GetInt("viewport_type"));
                }
                else
                {
                    _instance._viewportType = ViewportType.Topdown;
                }
            }

            return _instance;
        }
    }


    private static GameViewPortManager _instance;

    private ViewportType _viewportType = ViewportType.Follow;
    
    public void SetViewport(ViewportType viewportType)
    {
        _viewportType = viewportType;
        var vcam = GetFollowVCam();
        if (vcam != null)
        {
            vcam.gameObject.SetActive(viewportType == ViewportType.Follow);
        }
    }

    public ViewportType GetViewportType()
    {
        return _viewportType;
    }


    private CinemachineVirtualCamera _followVCam;
    public CinemachineVirtualCamera GetFollowVCam()
    {
        if (_followVCam == null)
        {
            var vcam3rdPrefab = GlobalAssetConfig.Instance.vcam3rdPrefab;
            var followCam = Instantiate(vcam3rdPrefab);
            followCam.name = "FollowPlayerVCam";
            var vcamGroup = GameObject.Find("CameraGroup");
            if (vcamGroup != null)
            {
                followCam.transform.SetParent(vcamGroup.transform);
            }
            _followVCam = followCam.GetComponent<CinemachineVirtualCamera>();
        }

        return _followVCam;
    }
    
    /// <summary>
    /// 关卡初始化调用
    /// </summary>
    public void InitForLevel(Transform player)
    {
        var vcam = GetFollowVCam();
        vcam.Follow = player;
        vcam.LookAt = player;
        
        vcam.gameObject.SetActive(_viewportType == ViewportType.Follow);
        Camera.main.clearFlags = CameraClearFlags.Skybox;
    }
}
