using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneTools
{
    [MenuItem("Tools/场景/打开烘培")]
    public static void OpenBake()
    {
        GameObject bakePoint = GameObject.Find("BakePoint");
        if (bakePoint == null)
        {
            Debug.LogError("此场景无烘培节点！");
            return;
        }

        bakePoint.SetActive(true);
    }

    [MenuItem("Tools/场景/关闭烘培")]
    public static void CloseBake()
    {
        GameObject bakePoint = GameObject.Find("BakePoint");
        if (bakePoint == null)
        {
            Debug.LogError("此场景无烘培节点！");
            return;
        }

        bakePoint.SetActive(false);
    }

    [MenuItem("Assets/TextureToJpg")]
    public static void TextureToJpg()
    {
        Object[] selectObjs = Selection.objects;
        for (int i = 0; i < selectObjs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(selectObjs[i]);
            string savePath = path.Remove(path.LastIndexOf(".")) +"_temp1.jpg";
            Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
           
            if (texture2D != null)
            {
                byte[] bytes = texture2D.EncodeToJPG();
                if (File.Exists(savePath))
                    File.Delete(savePath);
                FileStream file = File.Open(savePath, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(bytes);
                file.Close();
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/TextureToPng")]
    public static void TextureToPng()
    {
        Object[] selectObjs = Selection.objects;
        for (int i = 0; i < selectObjs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(selectObjs[i]);
            string savePath = path.Remove(path.LastIndexOf(".")) + "_temp1.png";
            Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (texture2D != null)
            {
                byte[] bytes = texture2D.EncodeToPNG();
                if (File.Exists(savePath))
                    File.Delete(savePath);
                FileStream file = File.Open(savePath, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(bytes);
                file.Close();
            }
        }
        AssetDatabase.Refresh();
    }
}
