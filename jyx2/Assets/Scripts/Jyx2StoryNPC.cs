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
