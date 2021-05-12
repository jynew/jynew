using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SkillEffect
{
    public class GhostShadow : MonoBehaviour
    {
        //持续时间
        public float m_fDuration = 2f;
        //创建新残影间隔
        public float m_fInterval = 0.1f;
        //X-ray
        private Shader m_GhostShader;

        //开关残影效果
        public bool m_bOpenGhost = false;

        //边缘颜色强度
        [Range(-1, 2)]
        public float m_fIntension = 1;

        //边缘颜色
        public Color m_Color = Color.blue;

        //网格数据
        SkinnedMeshRenderer m_MeshRender;

        private float lastTime = 0;

        void Start()
        {
            m_GhostShader = Shader.Find("SkillEffect/GhostShadow");
        }

        void Update()
        {
            if (!m_bOpenGhost)
                return;

            if (Time.time - lastTime < m_fInterval)
            {//残影间隔时间
                return;
            }
            lastTime = Time.time;

            if (m_MeshRender == null)
            {
                //获取身上的Mesh
                m_MeshRender = this.gameObject.GetComponent<SkinnedMeshRenderer>();
                if (m_MeshRender == null) return;
            }
//            for (int i = 0; i < m_MeshRender.Length; i++)
//            {
                Mesh mesh = new Mesh();
                try
                {
                    m_MeshRender.BakeMesh(mesh);
                }
                catch (Exception e)
                {
                    Debug.LogError($"严重错误：{e}");
                }

                GameObject go = new GameObject();
                go.hideFlags = HideFlags.HideAndDontSave;

                GhostItem item = go.AddComponent<GhostItem>();//控制残影消失
                item.duration = m_fDuration;
                item.deleteTime = Time.time + m_fDuration;

                MeshFilter filter = go.AddComponent<MeshFilter>();
                filter.mesh = mesh;

                MeshRenderer meshRen = go.AddComponent<MeshRenderer>();

                meshRen.material = m_MeshRender.material;
                meshRen.material.shader = m_GhostShader;//设置xray效果
                meshRen.material.SetColor("_RimColor", m_Color);
                meshRen.material.SetFloat("_RimIntensity", m_fIntension);//颜色强度传入shader中

                go.transform.localScale = m_MeshRender.transform.localScale;
                go.transform.position = m_MeshRender.transform.position;
                go.transform.rotation = m_MeshRender.transform.rotation;

                item.meshRenderer = meshRen;
//            }
        }
    }
}