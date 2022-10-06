using UnityEditor;
using UnityEngine;


public class FixModTool
{
    [MenuItem("Game Tools/修复启动MOD")]
    static void FixLaunchMod()
    {
        PlayerPrefs.DeleteKey("CURRENT_MOD_ID");
        Debug.Log("ok");
    }
}
