using Hanjiasongshu.ThreeD.XML;
using HanSquirrel.ResourceManager;
using Jyx2.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomOutlooking : MonoBehaviour
{
    [HideInInspector]
    public string m_ModelId = "Xia_HFH";

    [HideInInspector]
    public string m_WeaponName = "";

    private string BipKey = "Bip001";

    //左手武器模型
    private GameObject m_LeftWeaponModel;
    //右手武器模型
    private GameObject m_RightWeaponModel;
    //背上武器模型
    private GameObject m_BackWeaponModel;

    public WeaponSet WeaponSetObj
    {
        get
        {
            return WeaponSet.Get();
        }
    }
   
    public RoleModelSet RoleModelSetObj
    {
        get
        {
            return RoleModelSet.Get();
        }
    }



    public void OnChange(Action callback = null)
    {
        if (Application.isPlaying)
        {
            //销毁所有的孩子
            HSUnityTools.DestroyChildren(transform);
        }
        else
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                var go = transform.GetChild(i).gameObject;
                go.SetActive(false);
                DestroyImmediate(go);
            }
            transform.DetachChildren();

            //适应老的代码。。清理残留的合并Mesh
            var oldMesh = GetComponent<SkinnedMeshRenderer>();
            if (oldMesh != null)
            {
                DestroyImmediate(oldMesh);
            }
        }

        string path = "";

        if (m_ModelId.StartsWith("@"))
        {
            path = m_ModelId.TrimStart('@');
        }
        else
        {
            var roleModelSet = RoleModelSetObj.GetByName(m_ModelId);
            if (roleModelSet == null)
            {
                Debug.LogError("找不到模型:" + m_ModelId);
                return;
            }
            var modelPath = roleModelSet.FilePath;
            path = modelPath.TrimStart('@') + ".prefab";
        }

        Jyx2ResourceHelper.SpawnPrefab(path, (res) =>
        {
            if(res == null)
            {
                Debug.LogError("找不到模型资源：" + path);
                return;
            }
            if (res != null)
            {
                res.transform.SetParent(gameObject.transform, false);
                res.transform.localPosition = Vector3.zero;
            }

            var animator = GetComponent<Animator>();
            if(animator != null)
                animator.enabled = false;

            if(callback != null)
            {
                callback();
            }
        });
    }



    public void BindWeapon()
    {
        Transform leftParent = gameObject.transform.Find(BipKey + "/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/LeftWeaponMount");
        Transform rightParent = gameObject.transform.Find(BipKey + "/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/RightWeaponMount");
        Transform backParent = gameObject.transform.Find(BipKey + "/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/BackWeaponmount");

        if (leftParent != null && leftParent.childCount > 0)
        {
            m_LeftWeaponModel = leftParent.GetChild(0).gameObject;
        }

        if (rightParent != null && rightParent.childCount > 0)
        {
            m_RightWeaponModel = rightParent.GetChild(0).gameObject;
        }

        if (backParent != null && backParent.childCount > 0)
        {
            m_BackWeaponModel = backParent.GetChild(0).gameObject;
        }
    }

    public void ClearWeapon()
    {
        HSUnityTools.Destroy(m_LeftWeaponModel);
        HSUnityTools.Destroy(m_RightWeaponModel);
        HSUnityTools.Destroy(m_BackWeaponModel);
    }

    public void LoadWeapon(WeaponData weapon)
    {
        LoadWeapon(weapon, gameObject);
    }

    public void LoadWeapon(WeaponData weapon, GameObject parent)
    {
        //TODO 回头重新实现


        /*
        BindWeapon();
        ClearWeapon();

        string weaponPath = PathConst.WeaponPrefabPath;
        if (weapon != null && transform.childCount > 0)
        {
            Transform leftParent = transform.GetChild(0).Find(BipKey + "/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/LeftWeaponMount");
            Transform rightParent = transform.GetChild(0).Find(BipKey + "/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/RightWeaponMount");
            //HSUnityTools.DestroyChildren(leftParent);
            //HSUnityTools.DestroyChildren(rightParent);
            if (leftParent != null && weapon.Type == WeaponType.Left)
            {
                weaponPath = weaponPath + weapon.LeftName + ".prefab";
                GameObject leftObj = null;

                leftObj = ResourceLoader.LoadPrefabCached(weaponPath);

                m_LeftWeaponModel = GameObject.Instantiate(leftObj);
                m_LeftWeaponModel.transform.SetParent(leftParent, false);
                m_LeftWeaponModel.transform.localPosition = new Vector3(weapon.PosX, weapon.PosY, weapon.PosZ);
                m_LeftWeaponModel.transform.localEulerAngles = new Vector3(weapon.LeftRotX, weapon.LeftRotY, weapon.LeftRotZ);
            }
            else if (rightParent != null && weapon.Type == WeaponType.Right)
            {
                weaponPath = weaponPath + weapon.RightName + ".prefab";
                GameObject rightObj = null;

                rightObj = ResourceLoader.LoadPrefabCached(weaponPath);

                m_RightWeaponModel = GameObject.Instantiate(rightObj);
                m_RightWeaponModel.transform.SetParent(rightParent, false);
                m_RightWeaponModel.transform.localPosition = new Vector3(weapon.PosX, weapon.PosY, weapon.PosZ);
                m_RightWeaponModel.transform.localEulerAngles = new Vector3(weapon.RightRotX, weapon.RightRotY, weapon.RightRotZ);
            }
            else if (leftParent != null && rightParent != null && weapon.Type == WeaponType.Double)
            {
                string leftWeaponPath = weaponPath + weapon.LeftName + ".prefab";
                string rightWeaponPath = weaponPath + weapon.RightName + ".prefab";
                GameObject leftObj = null;
                GameObject rightObj = null;

                leftObj = ResourceLoader.LoadPrefabCached(leftWeaponPath);
                rightObj = ResourceLoader.LoadPrefabCached(rightWeaponPath);

                m_LeftWeaponModel = GameObject.Instantiate(leftObj);
                m_LeftWeaponModel.transform.SetParent(leftParent, false);
                m_LeftWeaponModel.transform.localPosition = new Vector3(weapon.PosX, weapon.PosY, weapon.PosZ);
                m_LeftWeaponModel.transform.localEulerAngles = new Vector3(weapon.LeftRotX, weapon.LeftRotY, weapon.LeftRotZ);

                m_RightWeaponModel = GameObject.Instantiate(rightObj);
                m_RightWeaponModel.transform.SetParent(rightParent, false);
                m_RightWeaponModel.transform.localPosition = new Vector3(weapon.PosX, weapon.PosY, weapon.PosZ);
                m_RightWeaponModel.transform.localEulerAngles = new Vector3(weapon.RightRotX, weapon.RightRotY, weapon.RightRotZ);
            }
            var animator = GetComponent<Animator>();
            if (animator != null) animator.SetInteger("WeaponType", (int)weapon.WeaponType);
        }
        */
    }

    //编辑器模式关闭的是所有材质的XRay，记得要打开
    //运行时之关闭自身（材质有实例化）
    public void CloseXRay()
    {
        SetXRayFactor(0.0f);
    }

    public void OpenXRay()
    {
        SetXRayFactor(1.0f);
    }

    void SetXRayFactor(float value)
    {
        BindWeapon();
        SetXRayFactor(gameObject, value);
        if (m_LeftWeaponModel != null)
        {
            SetXRayFactor(m_LeftWeaponModel, value);
        }
        if (m_RightWeaponModel != null)
        {
            SetXRayFactor(m_RightWeaponModel, value);
        }
        if (m_BackWeaponModel != null)
        {
            SetXRayFactor(m_BackWeaponModel, value);
        }
    }

    void SetXRayFactor(GameObject obj, float value)
    {
        if (obj == null) return;

        var mesh = obj.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
#if UNITY_EDITOR
            foreach (var meshMaterial in mesh.sharedMaterials)
            {
                meshMaterial.SetFloat("_XRayFactor", value);
            }
#else
        foreach (var meshMaterial in mesh.materials)
        {
            meshMaterial.SetFloat("_XRayFactor", value);
        }
#endif
        }

        var smesh = obj.GetComponent<SkinnedMeshRenderer>();
        if (smesh != null)
        {
#if UNITY_EDITOR
            foreach (var meshMaterial in smesh.sharedMaterials)
            {
                meshMaterial.SetFloat("_XRayFactor", value);
            }
#else
        foreach (var meshMaterial in smesh.materials)
        {
            meshMaterial.SetFloat("_XRayFactor", value);
        }
#endif
        }
    }
}
