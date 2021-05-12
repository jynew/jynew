using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Linq;

public class ResourceImporterAndTools : AssetPostprocessor
{
    public static bool AutoSet = false;

    [MenuItem("SkillEffect/打开自动修改开关")]
    public static void OpenAutoSetModel()
    {
        AutoSet = true;
    }

    [MenuItem("SkillEffect/关闭自动修改开关")]
    public static void AutoSetModel()
    {
        AutoSet = false;
    }

    //模型导入之前调用,自动设置人物模型部分属性
    public void OnPreprocessModel()
    {
        if (!AutoSet)
            return;

        if (this.assetPath.Contains(PathConst.NanModelPtah) || this.assetPath.Contains(PathConst.NvModelPtah))
        {
            ModelImporter impor = this.assetImporter as ModelImporter;
            impor.meshCompression = ModelImporterMeshCompression.Medium;
            impor.importCameras = false;
            impor.importLights = false;
            impor.importMaterials = true;
            impor.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
            impor.materialLocation = ModelImporterMaterialLocation.External;
            impor.materialName = ModelImporterMaterialName.BasedOnTextureName;
            //impor.animationType = ModelImporterAnimationType.Human;
        }
        else if (this.assetPath.Contains(PathConst.NanModelAnimPtah) || this.assetPath.Contains(PathConst.NvModelAnimPtah))
        {
            if (this.assetPath.EndsWith("@rig.FBX") || this.assetPath.EndsWith("@copy.FBX"))
                return;

            ModelImporter avataModel = null;
            if (this.assetPath.Contains(PathConst.NanModelAnimPtah))
            {
                avataModel = ModelImporter.GetAtPath(PathConst.NanModelAnimPtah + "nan@copy.FBX") as ModelImporter;
            }
            else if (this.assetPath.Contains(PathConst.NvModelAnimPtah))
            {
                avataModel = ModelImporter.GetAtPath(PathConst.NvModelAnimPtah + "nv@copy.FBX") as ModelImporter;
            }

            AvatarMask mask = null;
            if (this.assetPath.Contains(PathConst.NanModelAnimPtah))
            {
                mask = AssetDatabase.LoadAssetAtPath<AvatarMask>(PathConst.NanModelAnimPtah + "NanAvataMask.mask");
            }
            else if (this.assetPath.Contains(PathConst.NvModelAnimPtah))
            {
                mask = AssetDatabase.LoadAssetAtPath<AvatarMask>(PathConst.NvModelAnimPtah + "NvAvataMask.mask");
            }

            ModelImporter impor = this.assetImporter as ModelImporter;
            impor.meshCompression = ModelImporterMeshCompression.Medium;
            impor.importCameras = false;
            impor.importLights = false;
            impor.importMaterials = false;
            impor.animationType = ModelImporterAnimationType.Human;
            impor.sourceAvatar = avataModel.sourceAvatar;
            ModelImporterClipAnimation[] allAnim = new ModelImporterClipAnimation[impor.defaultClipAnimations.Length];
            for (int i = 0; i < impor.defaultClipAnimations.Length; i++)
            {
                ModelImporterClipAnimation temp = new ModelImporterClipAnimation();
                temp.name = impor.defaultClipAnimations[i].name;
                temp.firstFrame = impor.defaultClipAnimations[i].firstFrame;
                temp.lastFrame = impor.defaultClipAnimations[i].lastFrame;
                temp.lockRootHeightY = true;
                temp.lockRootPositionXZ = true;
                temp.lockRootRotation = true;
                temp.keepOriginalPositionY = true;
                temp.keepOriginalPositionXZ = true;
                temp.keepOriginalOrientation = true;
                temp.maskType = ClipAnimationMaskType.CopyFromOther;
                temp.maskSource = mask;
                if (this.assetPath.Contains("Idle") || this.assetPath.Contains("Run"))
                {
                    temp.loopTime = true;
                }
                allAnim[i] = temp;
            }
            impor.clipAnimations = allAnim;
        }
        else if (this.assetPath.Contains(PathConst.WeaponModPath) || assetPath.Contains(PathConst.OldWeaponModelPath))
        {
            ModelImporter impor = this.assetImporter as ModelImporter;
            impor.importAnimation = false;
            impor.meshCompression = ModelImporterMeshCompression.Medium;
            impor.importCameras = false;
            impor.importLights = false;
            impor.importMaterials = true;
            impor.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
            impor.materialLocation = ModelImporterMaterialLocation.External;
            impor.materialName = ModelImporterMaterialName.BasedOnTextureName;
        }
        else if (this.assetPath.Contains(PathConst.SceneModPath))
        {
            ModelImporter impor = this.assetImporter as ModelImporter;
            impor.importAnimation = false;
            impor.meshCompression = ModelImporterMeshCompression.Medium;
            impor.importCameras = false;
            impor.importLights = false;
            impor.importMaterials = true;
            impor.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
            impor.materialLocation = ModelImporterMaterialLocation.External;
            impor.materialName = ModelImporterMaterialName.BasedOnTextureName;
        }
    }
    

    [MenuItem("SkillEffect/从模型创建预制/创建人物部件预制")]
    public static void CreateBodyHairPrefabInMenu()
    {
        string[] allPath = new string[] { PathConst.NanModelPtah, PathConst.NvModelPtah };
        for (int j = 0; j < allPath.Length; j++)
        {
            //创建人物预制
            if (!Directory.Exists(allPath[j])) continue;

            DirectoryInfo direction = new DirectoryInfo(allPath[j]);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            int flieCount = files.Length;
            for (int i = 0; i < flieCount; i++)
            {
                EditorUtility.DisplayCancelableProgressBar("创建人物预制", "正在查找模型" + files[i].Name + "......", 1.0f / flieCount * i);
                CreateBodyHairPrefabFromModel(files[i].FullName);
            }
        }
        UnityEngine.Debug.Log("创建人物部件预制完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/从模型创建预制/按文件夹或文件创建自定义人物部件预制")]
    public static void CreateBodyHairPrefabByFile()
    {
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        for (int i = 0; i < objectsCount; i++)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(objects[i]);
            EditorUtility.DisplayCancelableProgressBar("创建部件预知", "正在查找模型" + selectPath + "......", 1.0f / objectsCount * i);

            if (!selectPath.Contains("NanModel") && !selectPath.Contains("NvModel")) continue;

            //创建人物预制
            if (Directory.Exists(selectPath))
            {
                DirectoryInfo direction = new DirectoryInfo(selectPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                int flieCount = files.Length;
                for (int j = 0; j < flieCount; j++)
                {
                    var filePath = files[j].FullName;
                    CreateBodyHairPrefabFromModel(filePath);
                }
            }
            else if (File.Exists(selectPath))
            {
                CreateBodyHairPrefabFromModel(selectPath);
            }
        }
        UnityEngine.Debug.Log("生成自定义人物部件完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static void CreateBodyHairPrefabFromModel(string modelPath)
    {
        if (!modelPath.EndsWith(".FBX") || modelPath.Contains("@rig")) return;

        string path = modelPath.Replace(@"\", "/");
        path = path.Replace(Application.dataPath.Replace("Assets", ""), "");
        GameObject obj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        if (path.Contains("Nan"))
        {
            CreatePrefab(PathConst.NanPrePath, obj, false);
        }
        else if (path.Contains("Nv"))
        {
            CreatePrefab(PathConst.NvPrePath, obj, false);
        }
        GameObject.DestroyImmediate(obj);
    }

    
    [MenuItem("SkillEffect/从模型创建预制/创建武器预制")]
    public static void CreateWeaponPrefabInMenu()
    {
        //创建武器预制
        if (Directory.Exists(PathConst.WeaponModPath))
        {
            DirectoryInfo direction = new DirectoryInfo(PathConst.WeaponModPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            int flieCount = files.Length;
            for (int i = 0; i < flieCount; i++)
            {
                EditorUtility.DisplayCancelableProgressBar("创建武器预制", "正在查找模型" + files[i].Name + "......", 1.0f / flieCount * i);
                CreateWeaponPrefabFromModel(files[i].FullName, PathConst.WeaponPrefabPath);
            }
        }

        UnityEngine.Debug.Log("生成武器预制完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/从模型创建预制/按文件夹或文件创建武器预制")]
    public static void CreateWeaponBySelect()
    {
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        for (int i = 0; i < objectsCount; i++)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(objects[i]);
            EditorUtility.DisplayCancelableProgressBar("创建武器预制", "正在查找模型" + selectPath + "......", 1.0f / objectsCount * i);
            //创建武器预制
            if (Directory.Exists(selectPath) && selectPath.Contains("WeaponModel"))
            {
                DirectoryInfo direction = new DirectoryInfo(selectPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                int flieCount = files.Length;
                for (int j = 0; j < flieCount; j++)
                {
                    CreateWeaponPrefabFromModel(files[j].FullName, PathConst.WeaponPrefabPath);
                }
            }
            else if (File.Exists(selectPath) && selectPath.Contains("WeaponModel"))
            {
                CreateWeaponPrefabFromModel(selectPath, PathConst.WeaponPrefabPath);
            }
        }
        UnityEngine.Debug.Log("生成武器预制完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/从模型创建预制/按文件夹或文件创建老侠客武器预制")]
    public static void CreateOldWeaponBySelect()
    {
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        for (int i = 0; i < objectsCount; i++)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(objects[i]);
            EditorUtility.DisplayCancelableProgressBar("创建武器预制", "正在查找模型" + selectPath + "......", 1.0f / objectsCount * i);
            //创建武器预制
            if (Directory.Exists(selectPath) && selectPath.Contains("WeaponModel"))
            {
                DirectoryInfo direction = new DirectoryInfo(selectPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                int flieCount = files.Length;
                for (int j = 0; j < flieCount; j++)
                {
                    CreateWeaponPrefabFromModel(files[j].FullName, PathConst.OldWeaponPrefabPath);
                }
            }
            else if (File.Exists(selectPath) && selectPath.Contains("WeaponModel"))
            {
                CreateWeaponPrefabFromModel(selectPath, PathConst.OldWeaponPrefabPath);
            }
        }
        UnityEngine.Debug.Log("生成武器预制完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static void CreateWeaponPrefabFromModel(string modelPath, string filePath)
    {
        if (modelPath.EndsWith(".FBX"))
        {
            string path = modelPath.Replace(@"\", "/");
            path = path.Replace(Application.dataPath.Replace("Assets", ""), "");
            GameObject obj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            CreatePrefab(filePath, obj, false);
            GameObject.DestroyImmediate(obj);
        }
    }


    [MenuItem("Assets/检测模型文件/检测所选模型文件的骨骼是否和标准骨骼一致")]
    public static void CheckBoneAmount()
    {
        EditorUtility.ClearProgressBar();
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        if (objectsCount < 1)
        {
            return;
        }

        var examplePath = "";
        foreach (var obj in objects)
        {
            var tempPath = AssetDatabase.GUIDToAssetPath(obj);
            if (File.Exists(tempPath) && tempPath.EndsWith(".FBX"))
            {
                examplePath = tempPath;
                break;
            }
        }

        if (string.IsNullOrEmpty(examplePath)) return;

        List<Transform> standardBone = new List<Transform>();
        if (examplePath.Contains("Nan"))
        {
            TryGetStandardBone("nan", standardBone);
        }
        else if (examplePath.Contains("Nv"))
        {
            TryGetStandardBone("nv", standardBone);
        }

        if (standardBone.Count == 0) return;

        for (int i = 0; i < objectsCount; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("检测骨骼", "正在提取" + objects[i] + "......", 1.0f / objectsCount * i);
            string path = AssetDatabase.GUIDToAssetPath(objects[i]);

            if (File.Exists(path) && path.EndsWith(".FBX"))
            {
                CheckModelBoneEqualBone(path, standardBone);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log("检测结束");
    }

    private static void TryGetStandardBone(string str, List<Transform> allBone)
    {
        var dir = str.Equals("nan")? PathConst.NanModelAnimPtah:PathConst.NvModelAnimPtah;

        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(dir + str + "@rig.FBX");
        Transform trs = obj.transform.Find("Bip001");
        Transform[] allTrs = trs.GetComponentsInChildren<Transform>(true);
        for (int j = 0; j < allTrs.Length; j++)
        {
            allBone.Add(allTrs[j]);
        }
    }

    private static void CheckModelBoneEqualBone(string path, List<Transform> standardbone)
    {
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Transform trs = obj.transform.Find("Bip001");
        Transform[] allTrs = trs.GetComponentsInChildren<Transform>(true);
        List<Transform> targetBone = new List<Transform>();
        for (int j = 0; j < allTrs.Length; j++)
        {
            targetBone.Add(allTrs[j]);
        }

        EqualList(obj, standardbone, targetBone);
    }

    enum BoneEqual
    {
        None = 0,//正常
        Count = 1,//数量不一致
        Layer = 2,//层级不一样
    }

    static BoneEqual EqualList(GameObject obj, List<Transform> refBone, List<Transform> targetBone)
    {
        List<string> tempRef = new List<string>();
        for (int i = 0; i < refBone.Count; i++)
        {
            tempRef.Add(refBone[i].name);
        }

        List<string> tempTarget = new List<string>();
        for (int i = 0; i < targetBone.Count; i++)
        {
            tempTarget.Add(targetBone[i].name);
        }

        if (refBone.Count != targetBone.Count)
        {
            UnityEngine.Debug.LogError(obj.name + "  骨骼数量不一致：  " + refBone.Count + "   " + targetBone.Count);
            if (refBone.Count > targetBone.Count)
            {
                tempRef.RemoveAll((res => tempTarget.Contains(res)));
                for (int i = 0; i < tempRef.Count; i++)
                {
                    UnityEngine.Debug.LogError(obj.name + "  少了骨骼： " + tempRef[i]);
                }
            }
            else if (refBone.Count < targetBone.Count)
            {
                tempTarget.RemoveAll((res => tempRef.Contains(res)));
                for (int i = 0; i < tempTarget.Count; i++)
                {
                    UnityEngine.Debug.LogError(obj.name + "  多了骨骼： " + tempTarget[i]);
                }
            }
            return BoneEqual.Count;
        }

        for (int i = 0; i < refBone.Count; i++)
        {
            if (refBone[i].name != targetBone[i].name || (refBone[i].parent.name != targetBone[i].parent.name && refBone[i].name != "Bip001" && targetBone[i].name != "Bip001"))
            {
                UnityEngine.Debug.LogError(obj.name + "  骨骼层级不一样： " + refBone[i].name + "  " + targetBone[i].name);
                return BoneEqual.Layer;
            }
        }

        return BoneEqual.None;
    }
    
    [MenuItem("Assets/检测模型文件/按文件夹或文件检查材质球个数是否大于1")]
    public static void CheckMaterialCountMoreThanOne()
    {
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        for (int i = 0; i < objectsCount; i++)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(objects[i]);
            EditorUtility.DisplayCancelableProgressBar("创建部件预知", "正在查找模型" + selectPath + "......", 1.0f / objectsCount * i);

            if (!(selectPath.Contains("NanModel") || selectPath.Contains("NvModel"))) continue;
            
            //创建人物预制
            if (Directory.Exists(selectPath))
            {
                DirectoryInfo direction = new DirectoryInfo(selectPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                int flieCount = files.Length;
                for (int j = 0; j < flieCount; j++)
                {
                    PrintMaterialsCountMoreThanOne(files[j].FullName);
                }
            }
            else if (File.Exists(selectPath))
            {
                PrintMaterialsCountMoreThanOne(selectPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static void PrintMaterialsCountMoreThanOne(string modelPath)
    {
        if (modelPath.EndsWith(".FBX") && !modelPath.Contains("@rig"))
        {
            string path = modelPath.Replace(@"\", "/");
            path = path.Replace(Application.dataPath.Replace("Assets", ""), "");
            GameObject obj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            Material[] materials = obj.GetComponentInChildren<Renderer>().sharedMaterials;
            if (materials.Length > 1)
            {
                UnityEngine.Debug.LogError(path + "此模型材质球个数为：" + materials.Length + "个");
            }

            GameObject.DestroyImmediate(obj);
        }
    }


    [MenuItem("SkillEffect/从模型创建预制/创建标准骨骼预制")]
    public static void CreateStandPrefab()
    {
        string[] allPath = new string[] { PathConst.StandardNvBonePath, PathConst.StandardNanBonePath };
        for (int i = 0; i < allPath.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("创建标准骨骼预知", "正在查找模型" + allPath[i] + "......", 1.0f / allPath.Length * i);
            GameObject temp = AssetDatabase.LoadAssetAtPath<GameObject>(allPath[i]);
            if (temp == null)
                continue;

            GameObject obj = GameObject.Instantiate(temp);
            List<Transform> trsList = new List<Transform>();
            for (int j = 0; j < obj.transform.childCount; j++)
            {
                Transform trs = obj.transform.GetChild(j);
                trsList.Add(trs);
            }

            for (int j = 0; j < trsList.Count; j++)
            {
                if (trsList[j].name != "Bip001")
                {
                    GameObject.DestroyImmediate(trsList[j].gameObject);
                }
            }
            trsList.Clear();
            if (allPath[i].Contains("Nan"))
            {
                CreatePrefab(PathConst.NanPrePath, obj);
            }
            else if (allPath[i].Contains("Nv"))
            {
                CreatePrefab(PathConst.NvPrePath, obj);
            }
            GameObject.DestroyImmediate(obj);
        }
        UnityEngine.Debug.Log("生成人物预知完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }


    [MenuItem("Assets/按文件夹或文件提取动画")]
    public static void CreateAnimClipByFile()
    {
        string[] objects = Selection.assetGUIDs;
        int objectsCount = objects.Length;
        for (int j = 0; j < objectsCount; j++)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(objects[j]);
            EditorUtility.DisplayCancelableProgressBar("创建部件预知", "正在查找模型" + selectPath + "......", 1.0f / objectsCount * j);
            if (!(selectPath.Contains("NanAnim") || selectPath.Contains("NvAnim"))) continue;

            if (Directory.Exists(selectPath))
            {
                DirectoryInfo direction = new DirectoryInfo(selectPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                int flieCount = files.Length;
                for (int i = 0; i < flieCount; i++)
                {
                    EditorUtility.DisplayCancelableProgressBar("创建动画", "正在查找模型" + files[i].Name + "......", 1.0f / flieCount * i);

                    CreateAnimClipFromFBX(files[i].FullName);
                }
            }
            else if (File.Exists(selectPath))
            {
                CreateAnimClipFromFBX(selectPath);
            }
        }
        UnityEngine.Debug.Log("提取动画完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SkillEffect/动画/创建所有动画")]
    public static void CreateAnimClipInMenu()
    {
        //创建人物预制
        if (Directory.Exists(PathConst.ModelPath))
        {
            DirectoryInfo direction = new DirectoryInfo(PathConst.ModelPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            int flieCount = files.Length;
            for (int i = 0; i < flieCount; i++)
            {
                EditorUtility.DisplayCancelableProgressBar("创建人物动画", "正在查找模型" + files[i].Name + "......", 1.0f / flieCount * i);

                CreateAnimClipFromFBX(files[i].FullName);
            }
        }

        UnityEngine.Debug.Log("生成人物动画完成！");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }


    private static void CreateAnimClipFromFBX(string fbxPath)
    {
        if (!fbxPath.EndsWith(".FBX") || !fbxPath.Contains("@") || 
            fbxPath.EndsWith("rig.FBX") || fbxPath.EndsWith("@copy.FBX")) return;

        string path = fbxPath.Replace(@"\", "/");
        path = path.Replace(Application.dataPath.Replace("Assets", ""), "");

        AnimationClip newClip = new AnimationClip();
        AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
        if (oldClip == null)
            return;

        EditorUtility.CopySerialized(oldClip, newClip);

        string[] strs = fbxPath.Replace(".FBX", "").Split('@');
        newClip.name = strs[1];

        string animDirectory = "";
        if (path.Contains("Nan"))
        {
            animDirectory = PathConst.NanAnimPath;
        }
        else if (path.Contains("Nv"))
        {
            animDirectory = PathConst.NvAnimPath;
        }
        else
        {
            newClip.name = strs[0] + strs[1];
            animDirectory = PathConst.XiakeAnimPath;
        }

        if (!Directory.Exists(animDirectory))
            Directory.CreateDirectory(animDirectory);

        string animationPath = animDirectory + newClip.name + ".anim";
        if (File.Exists(animationPath))
        {
            File.Delete(animationPath);
        }

        AssetDatabase.CreateAsset(newClip, animationPath);
    }

    private static void CreatePrefab(string path, GameObject obj, bool anmiator = true)
    {
        string name;
        if (obj.name.Contains("@rig(Clone)"))
        {
            name = obj.name.Replace("@rig(Clone)", "");
        }
        else
        {
            name = obj.name.Replace("(Clone)", "");
        }

        if (anmiator)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                anim = obj.AddComponent<Animator>();
            }
            RuntimeAnimatorController controll = AssetDatabase.LoadMainAssetAtPath(PathConst.CommonAnimControllerPath) as RuntimeAnimatorController;
            anim.runtimeAnimatorController = controll;
        }
        else
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim != null)
            {
                GameObject.DestroyImmediate(anim);
            }
        }
      
        string prefabPath = path + name + ".prefab";
        if (File.Exists(prefabPath))
        {
            File.Delete(prefabPath);
        }

        Transform[] allGameobjts = obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allGameobjts.Length; i++)
        {
            if (allGameobjts[i] != null)
            {
                allGameobjts[i].gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
        PrefabUtility.CreatePrefab(prefabPath, obj, ReplacePrefabOptions.ConnectToPrefab);
    }
    
}
