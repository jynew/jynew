/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jyx2StoryNPC : MonoBehaviour
{
    /// <summary>
    /// 开始的动作trigger
    /// </summary>
    public string startAnimationTrigger = "stand";

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(startAnimationTrigger))
        {
            var animator = GetComponent<Animator>();
            animator.SetTrigger(startAnimationTrigger);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
