using UnityEngine;
using System;

[Serializable]
public class HUDTextInfo
{
    public Transform CacheTransform;
    public string Text;
    public bl_Guidance Side = bl_Guidance.LeftUp;
    public int Size;
    public Color Color;
    public float Speed;
    public float VerticalAceleration;
    public float VerticalFactorScale;
    public bl_HUDText.TextAnimationType AnimationType = bl_HUDText.TextAnimationType.None;
    public float VerticalPositionOffset;
    public float AnimationSpeed;
    public float ExtraDelayTime;
    public float ExtraFloatSpeed;

    //Overrides
    public GameObject TextPrefab;
    public float FadeSpeed;

    public HUDTextInfo(Transform transform,string text)
    {
        this.CacheTransform = transform;
        this.Text = text;

        Size = 12;
        Color = Color.white;
        Speed = 10;
        VerticalAceleration = 1;
        VerticalFactorScale = 10;
        TextPrefab = null;
        AnimationType = bl_HUDText.TextAnimationType.None;
        VerticalPositionOffset = 1f;
        AnimationSpeed = 1;
        FadeSpeed = -1;
    }
}