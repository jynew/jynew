using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using GLib;
using Hanjiasongshu.ThreeD.XML;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Jyx2
{
    [CreateAssetMenu(fileName = "NewModelAsset", menuName = "Model Asset")]
    
    public class ModelAsset : ScriptableObject
    {
        [BoxGroup("数据", false)]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [OnValueChanged("AtuoBindModelData")]
        public GameObject m_View;
        
        [BoxGroup("数据")]
        [Header("剑")]
        [SerializeReference]
        public SwordPart m_SwordWeapon;
        
        [BoxGroup("数据")]
        [Header("刀")]
        [SerializeReference]
        public KnifePart m_KnifeWeapon;
        
        [BoxGroup("数据")]
        [Header("长柄")]
        [SerializeReference]
        public SpearPart m_SpearWeapon;
        
        [BoxGroup("数据")]
        [Header("其他类型")]
        [SerializeReference]
        public List<WeaponPart> m_OtherWeapons;
        
        [EnumToggleButtons]
        [ShowInInspector]
        [LabelText("预览武器类型")]
        private WeaponPartType weaponType = WeaponPartType.Sword;
        
        public enum WeaponPartType
        {
            [LabelText("剑")]
            Sword = 1, 
            
            [LabelText("刀")]
            Knife = 2, 
            
            [LabelText("长柄")]
            Spear = 3,
            
            [LabelText("其他")]
            Other = 4,
        }

        [ButtonGroup("操作")]
        [Button("完整预览", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void FullPreview()
        {
            if (m_View == null) return;
            
            var scene = SceneManager.GetActiveScene();
            // if (!scene.isLoaded) return;

            var gameObjects = scene.GetRootGameObjects();
            gameObjects.ForEachG(delegate(GameObject o)
            {
                if (o.name == m_View.name)
                {
                    DestroyImmediate(o);
                }
            });
            
            viewWithWeapon = (GameObject)PrefabUtility.InstantiatePrefab(m_View, scene);
            PrefabUtility.UnpackPrefabInstance(viewWithWeapon, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            
            DestroyImmediate(currentWeapon);
            var weaponPart = GetWeaponPart(weaponType);
            if (weaponPart != null && weaponPart.m_PartView != null)
            {
                currentWeapon = (GameObject)PrefabUtility.InstantiatePrefab(weaponPart.m_PartView, scene);
                var parent = UnityTools.DeepFindChild(viewWithWeapon.transform, weaponPart.m_BindBone);
                currentWeapon.transform.SetParent(parent);
                currentWeapon.transform.localScale = weaponPart.m_OffsetScale;
                currentWeapon.transform.localPosition = weaponPart.m_OffsetPosition;
                currentWeapon.transform.localRotation = Quaternion.Euler(weaponPart.m_OffsetRotation);
            }
        }

        [ButtonGroup("操作")]
        [Button("从场景预览导入武器数据", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void AutoInputWeaponData()
        {
            var weaponPart = GetWeaponPart(weaponType);
            weaponPart.m_OffsetScale = currentWeapon.transform.localScale;
            weaponPart.m_OffsetPosition = currentWeapon.transform.localPosition;
            weaponPart.m_OffsetRotation = currentWeapon.transform.localEulerAngles;
        }
        
        private GameObject currentWeapon = null;

        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true, PreviewHeight = 600f)]
        [ShowInInspector]
        [ReadOnly]
        [HideLabel]
        [BoxGroup("完整预览", Order = 99)]
        private GameObject viewWithWeapon;

        /// <summary>
        /// 获取武器模型配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public WeaponPart GetWeaponPart(WeaponPartType type)
        {
            int index = (int) type;
            return GetWeaponPart(index.ToString());
        }
        
        /// <summary>
        /// 获取武器模型配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
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