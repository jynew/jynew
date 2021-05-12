using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Red_UVScroller : MonoBehaviour
{
    public int targetMaterialSlot = 0;
    public float speedY = 0.5f;
    public float speedX = 0.0f;
    public Renderer m_MeshRenderer;

    private float timeWentX = 0.0f;
    private float timeWentY = 0.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeWentY += Time.deltaTime * speedY;
        timeWentX += Time.deltaTime * speedX;
        m_MeshRenderer.materials[targetMaterialSlot].SetTextureOffset("_MainTex", new Vector2(timeWentX, timeWentY));
    }

    void OnEnable()
    {
        m_MeshRenderer.materials[targetMaterialSlot].SetTextureOffset("_MainTex", new Vector2(0, 0));
        timeWentX = 0;
        timeWentY = 0;
    }
}