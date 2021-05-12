using System;
using UnityEngine;

[Serializable]
public class ExposedMapRole
{
    [SerializeField]
    public ExposedReference<MapRole> m_MapRole;
}