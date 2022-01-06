// 金庸群侠传3D重制版
// https://github.com/jynew/jynew
// 
// 这是本开源项目文件头，所有代码均使用MIT协议。
// 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
// 
// 金庸老先生千古！
// 
// 本文件作者：东方怂天(EasternDay)
// 文件名: ISerializationCallbackReceiver.cs
// 时间: 2022-01-05-2:52 PM

// Serialization.cs
using UnityEngine;
using System.Collections.Generic;
using System;

// List<T>
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

// Dictionary<TKey, TValue>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}