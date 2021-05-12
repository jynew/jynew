using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jyx2;
public class Jyx2SkillEditorEnemy : MonoBehaviour, ISkillCastTarget
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("hit");
        }
    }

    public void ShowDamage()
    {

       
    }
}
