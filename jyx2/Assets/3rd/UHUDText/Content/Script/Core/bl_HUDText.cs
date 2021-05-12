using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class bl_HUDText : MonoBehaviour {
    /// <summary>
    /// The Canvas Root of scene.
    /// </summary>
    [Tooltip("Canvas Root of scene.")]
    public Transform CanvasParent;
    public GameType m_Type = GameType.Mode3D;

    /// <summary>
    /// UI Prefab to instatantiate
    /// </summary>
    public GameObject TextPrefab;
    [Space(10)]
    public float FadeSpeed;
    public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1f, 1f), new Keyframe(3f, 0f) });
    //Start Animation type
    public TextAnimationType m_TextAnimationType = TextAnimationType.PingPong;
    //Speed factor to move / floating the text
    public float FloatingSpeed;
    //Distance between target and uiCamera / playerCamera for hide the text
    public float HideDistance;
    //multiplier of each value
    public float FactorMultiplier = 0.25f;
    //time to text wait for start to move.
    public float DelayStay = 0.3f;
    //can text re-use? if not, text just appear and move with any change.
    public bool CanReuse = true;
    //if can re-use, how many times can reuse?
    public int MaxUses = 5;
    [Range(0,180)]
    public float MaxViewAngle;
    //Destroy text when target destroy / death?
    public bool DestroyTextOnDeath = true;

    //Privates
   [SerializeField] private List<bl_Text> texts = new List<bl_Text>();
    private Camera MCamera = null;

    //Get the uiCamera / PlayerCamera
    Camera m_Cam
    {
        get
        {
            if (MCamera == null)
            {
                MCamera = (Camera.main != null) ? Camera.main : Camera.current;
            }

            return MCamera;
        }
    }

    /// <summary>
    /// Disable all text when this script gets disabled.
    /// </summary>
    void OnDisable()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] != null)
            {
                if (texts[i].Rect != null)
                {
                    Destroy(texts[i].Rect.gameObject);
                }
                texts[i] = null;
                texts.Remove(texts[i]);
            }
        }
        texts.Clear();
        bl_UHTUtils.GetHUDText = null;
    }


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (m_Cam == null || Time.timeScale == 0.0f)           
        {
            return;
        }
        HudTextControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void HudTextControl()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            //cache the current text
            bl_Text temporal = texts[i];

            if (temporal == null)
            {
                texts.RemoveAt(i);
                return;
            }
            //when target is destroyed then remove it from list.
            if (temporal.m_Transform == null)
            {
                //When player / Object death, destroy all last text.
                if (DestroyTextOnDeath && temporal.Rect.gameObject != null)
                {
                    Destroy(temporal.Rect.gameObject);
                    texts[i] = null;
                }
                texts.Remove(temporal);
                return;
            }

            //Wait for delay time, if this is pass, then is not more util and can move.
            temporal.isUtil = (Time.time < temporal.Delay && temporal.Uses < MaxUses);

            if (!temporal.isUtil)
            {
                float alpha = FadeCurve.Evaluate(temporal.fadeCurveTime);
                temporal.fadeCurveTime += ((Time.deltaTime * temporal.FadeSpeed) / 100);
                //fade text
                Color _color = temporal.m_Color;
                _color.a = alpha;
                temporal.m_Color = _color;
            }

            //if Text have more than a target graphic
            //add a canvas group in the root for fade all
            if (temporal.LayoutRoot != null)
            {
                temporal.LayoutRoot.alpha = texts[i].m_Color.a;
            }
            //if complete fade, remove and destroy text.
            if (texts[i].m_Color.a <= FadeCurve.keys[FadeCurve.keys.Length - 1].value)
            {
                Destroy(texts[i].Rect.gameObject);
                texts[i] = null;
                texts.Remove(texts[i]);
            }
            else//if UI visible
            {
                //Convert Word Position in screen position for UI
                int mov = ScreenPosition(texts[i].m_Transform);

                //only move is already pass the daly time 
                if (!temporal.isUtil)
                {
                    bl_Text m_Text = texts[i];
                    m_Text.Yquickness += Time.deltaTime * temporal.YquicknessScaleFactor;
                    float floatSpeed = temporal.FloatSpeed;
                    switch (texts[i].movement)
                    {
                        case bl_Guidance.Up:
                            m_Text.Ycountervail += (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            break;
                        case bl_Guidance.Down:
                            m_Text.Ycountervail -= (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            break;
                        case bl_Guidance.Left:
                            m_Text.Xcountervail -= ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;
                        case bl_Guidance.Right:
                            m_Text.Xcountervail += ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;
                        case bl_Guidance.RightUp:
                            m_Text.Ycountervail += (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            m_Text.Xcountervail += ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;
                        case bl_Guidance.RightDown:
                            m_Text.Ycountervail -= (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            m_Text.Xcountervail += ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;
                        case bl_Guidance.LeftUp:
                            m_Text.Ycountervail += (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            m_Text.Xcountervail -= ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;
                        case bl_Guidance.LeftDown:
                            m_Text.Ycountervail -= (((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier) * temporal.Yquickness;
                            m_Text.Xcountervail -= ((Time.deltaTime * floatSpeed) * mov) * FactorMultiplier;
                            break;

                    }
                }
                //Get center up of target               
                Vector3 position = Vector3.zero;
                if(m_Type == GameType.Mode3D)
                {
                    position = temporal.m_Transform.GetComponent<Collider>().bounds.center + (((Vector3.up * temporal.m_Transform.GetComponent<Collider>().bounds.size.y) * 0.5f));
                }
                else
                {
                    Collider2D c = temporal.m_Transform.GetComponent<Collider2D>();
                    position = c.bounds.center + (((Vector3.up * c.bounds.size.y) * 0.5f));
                }
                position.y += temporal.VerticalPositionOffset;

                Vector3 front = position - MCamera.transform.position;
                //its in camera view
                if ((front.magnitude <= HideDistance) && (Vector3.Angle(MCamera.transform.forward, position - MCamera.transform.position) <= MaxViewAngle))
                {
                    //Convert position to view port
                    Vector2 v = MCamera.WorldToViewportPoint(position);
                    //Calculate font size depend of distance.
                    int fontSize = ((int)((mov / 4) * 1) + temporal.m_Size) / 2;
                    //Clamp size for avoid problems of failed to generate dynamic font texture.
                    fontSize = Mathf.Clamp(fontSize, 1, 100);
                    temporal.m_Text.fontSize = fontSize;

                    temporal.m_Text.text = temporal.m_text;

                    //Calculate the movement 
                    Vector2 v2 = new Vector2((v.x * 0.5f) + temporal.Xcountervail, -((v.y) - temporal.Ycountervail));
                    //Apply to Text
                    temporal.Rect.anchorMax = v;
                    temporal.Rect.anchorMin = v;

                    temporal.Rect.anchoredPosition = v2;
                    temporal.m_Text.color = texts[i].m_Color;
                }
                else
                {
                    temporal.m_Color -= new Color(0f, 0f, 0f, (Time.deltaTime * temporal.FadeSpeed) / 25f);
                }
            }
        }
    }

    #region Constructors
    /// <summary>
    /// Simple way
    /// </summary>
    /// <param name="text"></param>
    /// <param name="trans"></param>
    public void NewText(string text, Transform trans)
    {
        NewText(text, trans, bl_Guidance.Up);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="trans"></param>
    public void NewText(string text, Transform trans, Color color)
    {
        NewText(text, trans, color, 8, 20f, 1, 2.2f, bl_Guidance.Up);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="trans"></param>
    /// <param name="place"></param>
    public void NewText(string text, Transform trans, bl_Guidance place)
    {
        NewText(text, trans, Color.white, 8, 20f, 0, 2.2f, place);
    }

    public void NewText(string text, Transform trans, Color color, int size, float speed, float yAcceleration, float yAccelerationScaleFactor, bl_Guidance movement)
    {
        HUDTextInfo info = new HUDTextInfo(trans,text);
        info.Side = movement;
        info.Size = size;
        info.Speed = speed;
        info.VerticalAceleration = yAcceleration;
        info.VerticalFactorScale = yAccelerationScaleFactor;
        info.Color = color;
        NewText(info);
    }
    #endregion

    /// <summary>
    /// send a new event, to create a new floating text
    /// </summary>
    public void NewText(HUDTextInfo info)
    {
        //override animation type
        TextAnimationType tat = (info.AnimationType == TextAnimationType.None) ? m_TextAnimationType : info.AnimationType;


        if (CanReuse && HaveUtil(info.CacheTransform) != null)
        {
            bl_Text t = HaveUtil(info.CacheTransform);

            //check for new lines
            //if you want add a new line to the current text simple send a text with the subfix '\n' in the end
            string text = t.m_text + info.Text;
            bool hasOtherLine = false;
            int containCount = 0;
            if (info.Text.Contains("\n") && !string.IsNullOrEmpty(t.m_text))
            {
                string[] tArray = text.Split('\n');
                containCount = tArray.Length;
                hasOtherLine = true;
                text = string.Join("\n", tArray);
            }

            t.m_text = (hasOtherLine) ? text : info.Text;
            t.m_Color = info.Color;
            t.FadeSpeed = (info.FadeSpeed > 0) ? info.FadeSpeed : FadeSpeed;
            t.VerticalPositionOffset = (hasOtherLine) ? 2.2f * containCount : t.VerticalPositionOffset;
            t.Delay = (Time.time + (DelayStay / 2)) + info.ExtraDelayTime;
            t.FloatSpeed = (FloatingSpeed + info.ExtraFloatSpeed);
            t.Uses++;
            t.PlayAnimation((int)tat,info.AnimationSpeed);
            return;
        }
        GameObject prefab = (info.TextPrefab == null) ? TextPrefab : info.TextPrefab;
        //Create new text info to instantiate 
        GameObject go = Instantiate(prefab) as GameObject;
        bl_Text item = go.GetComponent<bl_Text>();
        item.m_Speed = info.Speed;
        item.FadeSpeed = (info.FadeSpeed > 0) ? info.FadeSpeed : FadeSpeed;
        item.m_Color = info.Color;
        item.m_Transform = info.CacheTransform;
        item.m_text = info.Text;
        item.m_Size = info.Size;
        item.movement = info.Side;
        item.FloatSpeed = (FloatingSpeed + info.ExtraFloatSpeed);
        item.VerticalPositionOffset = info.VerticalPositionOffset;
        item.Yquickness = info.VerticalAceleration;
        item.YquicknessScaleFactor = info.VerticalFactorScale;
        item.Delay = (Time.time + DelayStay) + info.ExtraDelayTime;
        item.PlayAnimation((int)tat, info.AnimationSpeed);
        
        go.transform.SetParent(CanvasParent, false);
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        texts.Add(item);
    }

    /// <summary>
    /// Check if the request transform already have a text above
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    private bl_Text HaveUtil(Transform trans)
    {
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i].m_Transform == trans)
            {
                if (texts[i].isUtil && texts[i].Uses <= MaxUses)
                {
                    return texts[i];
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int ScreenPosition(Transform t)
    {
        int p = 0;
        if (m_Type == GameType.Mode3D)
        {
            p = (int)(m_Cam.WorldToScreenPoint(t.GetComponent<Collider>().bounds.center + (((Vector3.up * t.GetComponent<Collider>().bounds.size.y) * 0.5f))).y - this.m_Cam.WorldToScreenPoint(t.GetComponent<Collider>().bounds.center - (((Vector3.up * t.GetComponent<Collider>().bounds.size.y) * 0.5f))).y);
        }
        else if (m_Type == GameType.Mode2D)
        {
            Collider2D c = t.GetComponent<Collider2D>();
            if (c == null) { Debug.LogWarning("This transform, doesn't have a 2D collider, try to use Mode3D type instead."); return 0; }

            p = (int)(m_Cam.WorldToScreenPoint(c.bounds.center + (((Vector3.up * c.bounds.size.y) * 0.5f))).y - this.m_Cam.WorldToScreenPoint(c.bounds.center - (((Vector3.up * c.bounds.size.y) * 0.5f))).y);
        }

        return p;
    }

    [System.Serializable]
    public enum TextAnimationType
    {
        PingPong = 0,
        HorizontalSmall = 1,
        BigToSmall = 2,
        SmallToNormal = 3,
        None = 99,
    }

    [System.Serializable]
    public enum GameType
    {
        Mode3D,
        Mode2D,
    }
}