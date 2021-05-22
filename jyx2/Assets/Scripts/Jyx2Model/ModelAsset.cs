using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using GLib;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Jyx2
{
    [CreateAssetMenu(fileName = "NewModelAsset", menuName = "Model Asset")]
    public class ModelAsset : ScriptableObject
    {
        [OnValueChanged("AtuoBindModelData")]
        [InlineEditor(InlineEditorModes.LargePreview)]
        public GameObject m_View;
        
        [Header("剑")]
        [SerializeReference]
        public SwordPart m_SwordWeapon;
        
        [Header("刀")]
        [SerializeReference]
        public KnifePart m_KnifeWeapon;
        
        [Header("长柄")]
        [SerializeReference]
        public SpearPart m_SpearWeapon;
        
        [Header("其他类型")]
        [SerializeReference]
        public List<WeaponPart> m_OtherWeapons;

        //获取武器模型
        public WeaponPart GetWeaponPart(string type)
        {
            switch (type)
            {
                case "1":
                {
                    return m_SwordWeapon;
                }
                case "2":
                {
                    return m_KnifeWeapon;
                }
                case "3":
                {
                    return m_SpearWeapon;
                }
                default:
                {
                    return m_OtherWeapons?.Find(delegate(WeaponPart part) { return part.m_Id.ToString() == type; });
                }
            }
        }

        /// <summary>
        /// 自动绑定模型配置
        /// </summary>
        public void AtuoBindModelData()
        {
            if (m_View == null) return;
            var animator = m_View.GetComponent<Animator>();
            
            //非人型清空武器配置
            if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
            {
                m_SwordWeapon = null;
                m_KnifeWeapon = null;
                m_SpearWeapon = null;
                return;
            }
            
            //自动绑定武器配置
            m_SwordWeapon = m_SwordWeapon == null ? new SwordPart() : m_SwordWeapon;
            m_SwordWeapon.m_PartView = m_SwordWeapon.m_PartView == null ? (GameObject)AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultSword) : m_SwordWeapon.m_PartView;
            
            m_KnifeWeapon = m_KnifeWeapon == null ? new KnifePart() : m_KnifeWeapon;
            m_KnifeWeapon.m_PartView = m_KnifeWeapon.m_PartView == null ? (GameObject)AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultKnife) : m_KnifeWeapon.m_PartView;

            m_SpearWeapon = m_SpearWeapon == null ? new SpearPart() : m_SpearWeapon;
            m_SpearWeapon.m_PartView = m_SpearWeapon.m_PartView == null ? (GameObject)AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultSpear) : m_SpearWeapon.m_PartView;

            //自动绑定右手骨骼信息
            foreach (var bone in animator.avatar.humanDescription.human)
            {
                if (bone.humanName == "RightHand")
                {
                    m_SwordWeapon.m_BindBone = bone.boneName;
                    m_KnifeWeapon.m_BindBone = bone.boneName;
                    m_SpearWeapon.m_BindBone = bone.boneName;
                    break;
                }
            }
        }
        
        /*// 配置表转换Asset脚本，模型配置确认没问题则可以删除
        // [MenuItem("Exchange/Test")]
        public static void ExchangeAll()
        {
            foreach (Jyx2RoleHeadMapping mapping in ConfigTable.GetAll<Jyx2RoleHeadMapping>())
            {
                var exists = File.Exists(Application.dataPath + $"/BuildSource/Jyx2RoleModelAssets/{mapping.ModelAsset}.asset");
                if (exists) continue;

                var asset = ModelAsset.CreateInstance<ModelAsset>();
                var model = (GameObject)AssetDatabase.LoadMainAssetAtPath(mapping.Model);
                asset.m_View = model;
            
                if(!string.IsNullOrEmpty(mapping.WeaponMount))
                {
                    var paras = mapping.WeaponMount.Split('|');
                    int index = 0;
                    string id = paras[index++];
                    string prefab = paras[index++];
                    string bindObj = paras[index++];
                    float scale = float.Parse(paras[index++]);
                    Vector3 pos = UnityTools.StringToVector3(paras[index++], ',');
                    Vector3 rot = UnityTools.StringToVector3(paras[index++], ',');
                    if (id == "1")
                    {
                        asset.m_SwordWeapon = new SwordPart()
                        {
                            m_PartView = (GameObject)AssetDatabase.LoadMainAssetAtPath(prefab),
                            m_BindBone = bindObj,
                            m_OffsetPosition = pos,
                            m_OffsetRotation = rot,
                            m_OffsetScale = new Vector3(scale,scale,scale)
                        };
                    }
                    if (id == "2")
                    {
                        asset.m_KnifWeapon = new KnifPart()
                        {
                            m_PartView = (GameObject)AssetDatabase.LoadMainAssetAtPath(prefab),
                            m_BindBone = bindObj,
                            m_OffsetPosition = pos,
                            m_OffsetRotation = rot,
                            m_OffsetScale = new Vector3(scale,scale,scale)
                        };
                    }
                }
                
                AssetDatabase.CreateAsset(asset, $"Assets/BuildSource/Jyx2RoleModelAssets/{mapping.ModelAsset}.asset");
            }
        }
        */
    }
    
    [SerializeField]
    public class WeaponPart
    {
        // [ValueDropdown("GetListOfBones", ExpandAllMenuItems = true)]
        public int m_Id;
        public string m_BindBone;
        
        [InlineEditor(InlineEditorModes.LargePreview)]
        public GameObject m_PartView;
        
        public Vector3 m_OffsetPosition;
        public Vector3 m_OffsetRotation;
        public Vector3 m_OffsetScale;
        
        public WeaponPart()
        {
            m_OffsetScale = Vector3.one;
        }

        // private List<string> _boneList;
        // private IEnumerable<string> GetListOfSkills()
        // {
        //     if()
        //     return _boneList;
        // }
    }
    
    [SerializeField]
    public class SwordPart : WeaponPart
    {
        public SwordPart()
        {
            base.m_Id = 1;
        }
    }
    
    [SerializeField]
    public class KnifePart : WeaponPart
    {
        public KnifePart()
        {
            base.m_Id = 2;
        }
    }
    
    [SerializeField]
    public class SpearPart : WeaponPart
    {
        public SpearPart()
        {
            base.m_Id = 3;
        }
    }
}