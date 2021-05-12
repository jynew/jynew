using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_Text : MonoBehaviour
{
    //Use this if the Text prefabs have more than a UI component
    public CanvasGroup LayoutRoot = null;
    public Text m_Text = null;
    public RectTransform Rect;

    public Color m_Color { get; set; }
    public bl_Guidance movement { get; set; }
    public float Xcountervail { get; set; }
    public float Ycountervail { get; set; }
    public int m_Size { get; set; }
    public float m_Speed { get; set; }
    public string m_text { get; set; }
    public Transform m_Transform { get; set; }
    public float Yquickness { get; set; }
    public float YquicknessScaleFactor { get; set; }
    public float VerticalPositionOffset;
    public float FadeSpeed { get; set; }
    public float FloatSpeed { get; set; }
    public float Delay { get; set; }
    public bool isUtil { get; set; }
    public int Uses { get; set; }

    public float fadeCurveTime { get; set; }
    public float sizeCurveTime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public void PlayAnimation(int id,float speed)
    {
        Animator anim = null;
        if (this.GetComponent<Animation>() != null)
        {
            anim = this.GetComponent<Animator>();
        }
        else if (this.GetComponentInChildren<Animator>() != null)
        {
            anim = this.GetComponentInChildren<Animator>();
        }
        else
        {
            return;
        }
        anim.speed = speed;
        anim.Play(id.ToString(), 0, 0);

    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        Destroy(this.gameObject);
    }
}