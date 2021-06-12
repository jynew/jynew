using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using GLib;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
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

        [BoxGroup("数据")] [Header("剑")] [SerializeReference]
        public SwordPart m_SwordWeapon;

        [BoxGroup("数据")] [Header("刀")] [SerializeReference]
        public KnifePart m_KnifeWeapon;

        [BoxGroup("数据")] [Header("长柄")] [SerializeReference]
        public SpearPart m_SpearWeapon;

        [BoxGroup("数据")] [Header("其他类型")] [SerializeReference]
        public List<WeaponPart> m_OtherWeapons;

        [EnumToggleButtons] [ShowInInspector] [LabelText("预览武器类型")]
        private WeaponPartType weaponType = WeaponPartType.Sword;

        public enum WeaponPartType
        {
            [LabelText("剑")] Sword = 1,

            [LabelText("刀")] Knife = 2,

            [LabelText("长柄")] Spear = 3,

            [LabelText("其他")] Other = 4,
        }

#if UNITY_EDITOR
        [ButtonGroup("操作")]
        [Button("完整预览", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void FullPreview()
        {
            if (m_View == null) return;

            var scene = EditorSceneManager.OpenScene("Assets/Scripts/Jyx2Model/ModelPreviewScene.unity",
                OpenSceneMode.Additive);

            var gameObjects = scene.GetRootGameObjects();
            gameObjects.ForEachG(delegate(GameObject o)
            {
                if (o.name == m_View.name)
                {
                    DestroyImmediate(o);
                }
            });

            viewWithWeapon = (GameObject) PrefabUtility.InstantiatePrefab(m_View, scene);
            viewWithWeapon.transform.SetAsLastSibling();
            PrefabUtility.UnpackPrefabInstance(viewWithWeapon, PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction);
            EditorGUIUtility.PingObject(viewWithWeapon);
            Selection.activeGameObject = viewWithWeapon;
            SceneView.lastActiveSceneView.LookAt(viewWithWeapon.transform.position);

            DestroyImmediate(currentWeapon);
            var weaponPart = GetWeaponPart(weaponType);
            if (weaponPart != null && weaponPart.m_PartView != null)
            {
                currentWeapon = (GameObject) PrefabUtility.InstantiatePrefab(weaponPart.m_PartView, scene);
                var parent = UnityTools.DeepFindChild(viewWithWeapon.transform, weaponPart.m_BindBone);
                currentWeapon.transform.SetParent(parent);
                currentWeapon.transform.localScale = weaponPart.m_OffsetScale;
                currentWeapon.transform.localPosition = weaponPart.m_OffsetPosition;
                currentWeapon.transform.localRotation = Quaternion.Euler(weaponPart.m_OffsetRotation);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [ButtonGroup("操作")]
        [Button("从预览场景导入武器偏移数据", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void AutoInputWeaponData()
        {
            var weaponPart = GetWeaponPart(weaponType);
            weaponPart.m_OffsetScale = currentWeapon.transform.localScale;
            weaponPart.m_OffsetPosition = currentWeapon.transform.localPosition;
            weaponPart.m_OffsetRotation = currentWeapon.transform.localEulerAngles;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private GameObject currentWeapon = null;

        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true, PreviewHeight = 600f)]
        [ShowInInspector]
        [ReadOnly]
        [HideLabel]
        [BoxGroup("完整预览", Order = 99)]
        private GameObject viewWithWeapon;
#endif

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
            if (m_SwordWeapon == null) m_SwordWeapon = new SwordPart();
            if (m_KnifeWeapon == null) m_KnifeWeapon = new KnifePart();
            if (m_SpearWeapon == null) m_SpearWeapon = new SpearPart();

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

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            AtuoBindModelData();
        }
#endif
    }

    [SerializeField]
    public class WeaponPart
    {
        // [ValueDropdown("GetListOfBones", ExpandAllMenuItems = true)]
        public int m_Id;
        public string m_BindBone;

        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)] [InlineButton("LoadDefaultView", "缺省模型")]
        public GameObject m_PartView;

        public Vector3 m_OffsetPosition;
        public Vector3 m_OffsetRotation;
        public Vector3 m_OffsetScale;

        public WeaponPart()
        {
            m_OffsetScale = Vector3.one;
        }

#if UNITY_EDITOR
        private void LoadDefaultView()
        {
            switch (m_Id)
            {
                case 1:
                {
                    m_PartView = (GameObject) AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultSword);
                    break;
                }
                case 2:
                {
                    m_PartView = (GameObject) AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultKnife);
                    break;
                }
                case 3:
                {
                    m_PartView = (GameObject) AssetDatabase.LoadMainAssetAtPath(ConStr.DefaultSpear);
                    break;
                }
            }

            AssetDatabase.SaveAssets();
        }
#endif
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