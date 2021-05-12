using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jyx2Player : MonoBehaviour
{

    public static Jyx2Player GetPlayer()
    {
        return LevelMaster.Instance.GetPlayer();
    }

    public bool IsOnBoat;

    NavMeshAgent _navMeshAgent;
    Jyx2Boat _boat;


    public void GetInBoat(Jyx2Boat boat)
    {
        IsOnBoat = true;
        _boat = boat;

        _navMeshAgent.updatePosition = false;
        transform.position = boat.transform.position;
        transform.rotation = boat.transform.rotation;

        SetHide(true);
        _navMeshAgent.areaMask = GetWaterNavAreaMask();
        _navMeshAgent.updatePosition = true;
    }

    public bool GetOutBoat()
    {
        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(transform.position, out myNavHit, 3.5f, GetNormalNavAreaMask()))
        {
            //比水平面还低
            if(myNavHit.position.y < 5f)
            {
                return false;
            }

            SetHide(false);
            _navMeshAgent.areaMask = GetNormalNavAreaMask();
            IsOnBoat = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetHide(bool isHide)
    {
        foreach(var r in transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            r.enabled = !isHide;
        }
    }

    /// <summary>
    /// 获取水路行走mask
    /// </summary>
    /// <returns></returns>
    int GetWaterNavAreaMask()
    {
        return (0 << 0) + (0 << 1) + (1 << 2) + (1 << 3);
    }

    /// <summary>
    /// 获取普通的陆地行走mask
    /// </summary>
    /// <returns></returns>
    int GetNormalNavAreaMask()
    {
        return (1 << 0) + (0 << 1) + (1 << 2) + (0 << 3);
    }


    public void Init()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _boat = FindObjectOfType<Jyx2Boat>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (IsOnBoat)
        {
            _boat.transform.position = this.transform.position;
            _boat.transform.rotation = this.transform.rotation;
        }
    }

    //保存世界信息
    public void RecordWorldInfo()
    {
        var runtime = GameRuntimeData.Instance;
        runtime.WorldPosition = UnityTools.Vector3ToString(this.transform.position);
        runtime.BoatWorldPos = UnityTools.Vector3ToString(_boat.transform.position);
        runtime.BoatRotate = UnityTools.QuaternionToString(_boat.transform.rotation);
        runtime.OnBoat = IsOnBoat ? 1 : 0;
    }

    public void LoadWorldInfo()
    {
        var runtime = GameRuntimeData.Instance;
        var pos = UnityTools.StringToVector3(runtime.WorldPosition); //大地图读取当前位置
        PlayerSpawnAt(pos);

        if (!string.IsNullOrEmpty(runtime.BoatWorldPos))
        {
            _boat.transform.position = UnityTools.StringToVector3(runtime.BoatWorldPos);
            _boat.transform.rotation = UnityTools.StringToQuaternion(runtime.BoatRotate);

            if (runtime.OnBoat == 1)
            {
                _boat.GetInBoat();
            }
        }
    }

    void PlayerSpawnAt(Vector3 spawnPos)
    {
        _navMeshAgent.enabled = false;
        Debug.Log("load pos = " + spawnPos);
        transform.position = spawnPos;
        _navMeshAgent.enabled = true;
    }
}
