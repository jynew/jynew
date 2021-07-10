using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AnimationTools
{
    [MenuItem("Tools/动画工具/一键配置所有动作为JYX2标准")]
    static public void ForceCheckAllJyx2Animations()
    {
        
        foreach (var file in Directory.GetFiles("Assets/BuildSource/Animations"))
        {
            if (!file.EndsWith(".anim")) continue;

            
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);

            if (clip != null)
            {
                AnimationClipSettings acs = AnimationUtility.GetAnimationClipSettings(clip);
                acs.keepOriginalOrientation = true;
                acs.keepOriginalPositionY = true;
                acs.keepOriginalPositionXZ = true;

                acs.loopBlendOrientation = true;
                acs.loopBlendPositionY = true;
                acs.loopBlendPositionXZ = true;
                
                if (clip.name.Contains("站立") || clip.name.Contains("待机") || clip.name.Contains("移动") || clip.name.Contains("跑图"))
                {
                    acs.loopTime = true;
                }
                else
                {
                    acs.loopTime = false;
                }
                
                AnimationUtility.SetAnimationClipSettings(clip, acs);
            }
            AssetDatabase.SaveAssets();
            Debug.Log(clip);
        }
    }
}
