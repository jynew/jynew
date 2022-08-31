using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2
{

    /// <summary>
    /// 物品存储扩展数据
    /// </summary>
    [Serializable]
    public class ItemExtSaveData
    {
        [SerializeField] public int getTime; //获取时间
        [SerializeField] public int count; //数量
    }
}

