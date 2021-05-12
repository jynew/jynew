using System;
using UnityEngine;

namespace SkillEffect
{
    public class GhostItem : MonoBehaviour
    {
        //持续时间
        public float duration;
        //销毁时间
        public float deleteTime;

        public MeshRenderer meshRenderer;

        void Update()
        {
            float tempTime = deleteTime - Time.time;
            if (tempTime <= 0)
            {//到时间就销毁
                GameObject.Destroy(this.gameObject);
            }
            else if (meshRenderer.material)
            {
                float rate = tempTime / duration;//计算生命周期的比例
                Color cal = meshRenderer.material.GetColor("_RimColor");
                cal.a *= rate;//设置透明通道
                meshRenderer.material.SetColor("_RimColor", cal);
            }

        }
    }
}