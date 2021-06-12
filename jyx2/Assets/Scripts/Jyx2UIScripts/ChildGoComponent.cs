using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGoComponent : MonoBehaviour
{
    Transform instantiatePrefab;
    List<Transform> childGoList = new List<Transform>();
    List<Transform> usingGoList = new List<Transform>();
    Action<Transform> onCreate;
    public void Init(Transform trans,Action<Transform> onCreate = null) 
    {
        instantiatePrefab = trans;
        this.onCreate = onCreate;
        HideAllChild();
    }

    void HideAllChild() 
    {
        for (int i = 0; i < childGoList.Count; i++)
        {
            Transform trans = childGoList[i];
            trans.gameObject.SetActive(false);
        }
    }

    void InsureChildCount(int needNum) 
    {
        if (childGoList.Count >= needNum)
            return;
        int count = needNum - childGoList.Count;
        for (int i = 0; i < count; i++)
        {
            Transform trans = Instantiate(instantiatePrefab);
            trans.SetParent(transform);
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.SetAsLastSibling();
            childGoList.Add(trans);
            onCreate?.Invoke(trans);
        }
    }

    public void RefreshChildCount(int count) 
    {
        usingGoList.Clear();
        InsureChildCount(count);
        Transform trans;
        for (int i = 0; i < childGoList.Count; i++)
        {
            trans = childGoList[i];
            trans.gameObject.SetActive(i < count);
            if (i < count)
                usingGoList.Add(trans);
        }
    }

    public List<Transform> GetUsingTransList() 
    {
        return usingGoList;
    } 
}
