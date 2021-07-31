using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jyx2Editor.AssetToolkits
{
    public class TextureProcesser : MonoBehaviour
    {
        private const string AssetRoot = "Assets";

        [MenuItem("Tools/检查工具/贴图格式检查")]
        private static void CheckAllTexture()
        {
            var guids = AssetDatabase.FindAssets("t:Texture", new string[] {AssetRoot});
            var index = 0;
            var length = guids.Length;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                index++;
                EditorUtility.DisplayProgressBar("提示", $"玩命检查中:{path}", index / (float) length);
                var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                //check etc1 on pc
                if (null == textureImporter) continue;

                var defaultPlatformTextureSettings = textureImporter.GetDefaultPlatformTextureSettings();
                var windowPlatformTextureSettings = textureImporter.GetPlatformTextureSettings("Standalone");
                if (windowPlatformTextureSettings.overridden)
                {
                    if (windowPlatformTextureSettings.format == TextureImporterFormat.ETC_RGB4)
                    {
                        Debug.LogError($"Standalone平台不支持etc1压缩格式的贴图，路径：{path}");
                    }
                }
                else
                {
                    if (defaultPlatformTextureSettings.format == TextureImporterFormat.ETC_RGB4)
                    {
                        Debug.LogError($"Standalone平台不支持etc1压缩格式的贴图，路径：{path}");
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "检查完毕！请看控制台信息输出！", "OK");
        }
    }
}