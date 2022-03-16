using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地图存储数据
/// </summary>
[Serializable]
public class WorldMapSaveData
{
    [SerializeField] public Vector3 WorldPosition; //世界位置
    [SerializeField] public Quaternion WorldRotation; //世界位置旋转
    [SerializeField] public Vector3 BoatWorldPos; //船的世界位置
    [SerializeField] public Quaternion BoatRotate; //船的朝向
    [SerializeField] public int OnBoat; //是否在船上
}

/// <summary>
/// 子地图存储数据
/// </summary>
[Serializable]
public class SubMapSaveData
{
    public SubMapSaveData(int id)
    {
        MapId = id;
    }
    public SubMapSaveData(){}
    
    [SerializeField] public int MapId; //当前所处地图
    [SerializeField] public Vector3 CurrentPos; //当前位置
    [SerializeField] public Quaternion CurrentOri; //当前方向
}
