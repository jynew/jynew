using System.Collections;
using System.Collections.Generic;
using Hanjiasongshu.ThreeD.XML;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CustomOutlooking))]
public class CustomOutlookingEditor : Editor {

    private SerializedObject obj;//序列化
    private CustomOutlooking myScript;


    private int bodyIndex = 0;
    private int hairIndex = 0;
    private int weaponIndex = 0;
    private int xiakeIndex = 0;

    void OnEnable()
    {
        obj = new SerializedObject(target);
    }
    
    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        myScript = (CustomOutlooking)target;

        xiakeIndex = GetRoleModelSet().IndexOf(myScript.m_ModelId);
        weaponIndex = GetWeaponNameSet().IndexOf(myScript.m_WeaponName);


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("开始配装");

        var xiakeopts = GetRoleModelSet();
        xiakeIndex = EditorGUILayout.Popup(new GUIContent("侠客模型"), xiakeIndex, xiakeopts.ToArray());
        if (xiakeIndex > 0)
        {
            var oldxiakeName = xiakeopts[xiakeIndex];
            myScript.m_ModelId = oldxiakeName;
        }
 
        var weaponopts = GetWeaponNameSet();
        weaponIndex = EditorGUILayout.Popup(new GUIContent("Weapon"), weaponIndex, weaponopts.ToArray());
        if (weaponIndex > 0)
        {
            var weaponName = weaponopts[weaponIndex];
            //Debug.Log("选择的武器是：" + weaponName);
            myScript.m_WeaponName = weaponName;
        }
        
        if (GUILayout.Button("实装配置"))
        {
            myScript.OnChange();
            EditorSceneManager.MarkAllScenesDirty();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("关闭XRay！警告：编辑器模式关闭的是sharedMeterials，也就是一关关掉所有，一定要记得开"))
        {
            myScript.CloseXRay();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("打开XRay"))
        {
            myScript.OpenXRay();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("角色着地"))
        {
            PlaceOnTheGround();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("重载配置(修改xml后动态点击生效)"))
        {
            RoleModelSet.Clear();
            WeaponSet.Clear();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("其他");
        DrawDefaultInspector();
    }


    public List<string> GetRoleModelSet()
    {
        var rst = new List<string>();
        rst.Add("选择角色模型");

        var allModels = RoleModelSet.Get();
        foreach (var hair in allModels.List)
        {
            rst.Add(hair.GameName);
        }
        return rst;
    }
    
    public List<string> GetWeaponNameSet()
    {
        var rst = new List<string>();
        rst.Add("选择武器");

        var list = myScript.WeaponSetObj.NanWeaponList; //: myScript.WeaponSetObj.NvWeaponList;
        rst.Add("空手");
        foreach (var weapon in list)
        {
            rst.Add(weapon.Name);
        }

        return rst;
    }

    public void PlaceOnTheGround()
    {
        //define ray to cast downwards waypoint position
        Ray ray = new Ray(myScript.transform.position + new Vector3(0, 2f, 0), -Vector3.up);
        RaycastHit hit;
        //cast ray against ground, if it hit:
        
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
        {
            Debug.Log("hit object = " + hit.transform.name);
            //position waypoint to hit point
            myScript.transform.position = hit.point;
        }
    }
}
