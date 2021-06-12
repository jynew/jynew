using DG.Tweening;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapTest : MonoBehaviour
{
    public MeshRenderer m_BigMapMeshRender;
    public Button m_BigButton;
    enum ViewLevel
    {
        None,
        Near = 1,
        Far = 2,
    }

    public float m_Dis = 4;
    public float m_BigMoveSpeed = 0.5f;
    public float m_SmallMoveSpeed = 0.5f;

    private ViewLevel m_ViewLevel = ViewLevel.Far;
    private float m_ChangeValue = 0;
    private Tween m_CameraTween;
    private Tween m_ValueTween;
    private Vector3 m_StartPos = new Vector3(1.151f, 4.05f, 7.5f);
    private Vector3 m_StartRot = new Vector3(40, 180, 0);


    private float lastDistance = 0;
    private float twoTouchDistance = 0;
    Vector2 firstTouch = Vector3.zero;
    Vector2 secondTouch = Vector3.zero;
    private bool isTwoTouch = false;
    private int finger_count = 0;
    private Vector2 sum_position = Vector2.zero;

    public float m_BigMinX = -3.2f;
    public float m_BigMaxX = 3.4f;
    public float m_BigMinZ = 24.14f;
    public float m_BigMaxZ = 28.3f;

    public float m_SmallMinX = -11.15f;
    public float m_SmallMaX = 11.15f;
    public float m_SmallMinZ = -1.4f;
    public float m_SmallMaxZ = 15.85f;

    public Button m_ChangeColorBtn;
    public float m_ChangeColorTimeCost = 2;
    public Color[] m_AmbientColors = new Color[4];
    private int CurrentAmbientColor = 0;

    public SpriteRenderer[] m_Big_Cloud;
    public SpriteRenderer[] m_Big_Icons;

    public SpriteRenderer[] m_Small_Trees;
    public SpriteRenderer[] m_Small_Icons;

    public GameObject[] m_Small_Cloud_Father;
    private SpriteRenderer[] m_Small_Cloud;

    private float Tolerence = 0.0001f;

    // Use this for initialization

    private List<SpriteRenderer[]> _big_items = new List<SpriteRenderer[]>();
    private List<SpriteRenderer[]> _small_items = new List<SpriteRenderer[]>();

    private void OnEnable()
    {
        EasyTouch.On_Drag += OnDrag;
        EasyTouch.On_PinchIn += OnPinchIn;
        EasyTouch.On_PinchOut += OnPinchOut;

        var list = new List<SpriteRenderer>();
        foreach (var o in m_Small_Cloud_Father)
        {
            var smallCloud = o.GetComponentInChildren<SpriteRenderer>();
            list.Add(smallCloud);
        }
        m_Small_Cloud = list.ToArray();

        _big_items.Add(m_Big_Cloud);
        _big_items.Add(m_Big_Icons);

        _small_items.Add(m_Small_Cloud);
        _small_items.Add(m_Small_Trees);
        _small_items.Add(m_Small_Icons);

        m_ChangeColorBtn.onClick.AddListener(OnChangeColorClick);
        RenderSettings.ambientSkyColor = m_AmbientColors[CurrentAmbientColor];
    }

    private void OnChangeColorClick()
    {
        var nextColor = m_AmbientColors[CurrentAmbientColor];
        DOTween.To(() => RenderSettings.ambientSkyColor, x => RenderSettings.ambientSkyColor = x, nextColor, m_ChangeColorTimeCost);
        CurrentAmbientColor++;
        if (CurrentAmbientColor > 3) CurrentAmbientColor = 0;
    }

    void Start ()
    {
        foreach (var spriteList in _small_items)
        {
            foreach (var spriteRenderer in spriteList)
            {
                var color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 0);
            }
        }
    }

    void OnPinchIn(Gesture gesture)
    {
        Debug.Log(gesture.swipeVector);
    }

    void OnPinchOut(Gesture gesture)
    {
        Debug.Log(gesture.swipeVector);
    }

    void OnDrag(Gesture gesture)
    {
        float moveSpeed = 0;
         float minX = -3.3f;
        float maxX = 3.4f;
        float minZ = 24.14f;
        float maxZ = 28.3f;
        if (m_ViewLevel == ViewLevel.Far)
        {
            moveSpeed = m_BigMoveSpeed;
            minX = m_BigMinX;
            maxX = m_BigMaxX;
            minZ = m_BigMinZ;
            maxZ = m_BigMaxZ;
        }
        else if (m_ViewLevel == ViewLevel.Near)
        {
            moveSpeed = m_SmallMoveSpeed;
            minX = m_SmallMinX;
            maxX = m_SmallMaX;
            minZ = m_SmallMinZ;
            maxZ = m_SmallMaxZ;
        }

        if (transform.position.x - maxX <= Tolerence && transform.position.x - minX>= -Tolerence)
        {
            transform.position -= (Vector3.left * gesture.deltaPosition.x * moveSpeed * Time.deltaTime);
            if (transform.position.x > maxX)
            {
                transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            }
            if (transform.position.x < minX)
            {
                transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            }
        }
        if (transform.position.z - maxZ <= Tolerence && transform.position.z - minZ >= -Tolerence)
        {
            transform.position += (Vector3.forward * gesture.deltaPosition.y * moveSpeed * Time.deltaTime);
            if (transform.position.z > maxZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
            }
            if (transform.position.z < minZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ViewLevel == ViewLevel.Far)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || ToucheUpdate() ==1)
            {
                CloseTweener();
                var finalPos = sum_position / 2;
#if UNITY_EDITOR
                finalPos = Input.mousePosition;
#endif
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(finalPos.x, finalPos.y, Camera.main.WorldToScreenPoint(m_BigMapMeshRender.transform.position).z));
                Vector3 mouseNor = (mousePos - transform.position).normalized;
                //float angle = Vector3.Angle(transform.forward, mouseNor);
                //angle = angle > 90 ? 180 - angle : angle;
                //float dis = m_Dis / Mathf.Cos((angle * Mathf.Deg2Rad));
                //Vector3 target = transform.position + mouseNor * dis;
                Vector3 target = GetIntersectWithLineAndPlane(transform.position, mouseNor, Vector3.up, new Vector3(0, (transform.position + transform.forward * m_Dis).y, 0));
                if (target.x > m_SmallMaX)
                {
                    target = new Vector3(m_SmallMaX, target.y, target.z);
                }
                if (target.x < m_SmallMinX)
                {
                    target = new Vector3(m_SmallMinX, target.y, target.z);
                }

                if (target.z > m_SmallMaxZ)
                {
                    target = new Vector3(target.x, target.y, m_SmallMaxZ);
                }
                if (target.z < m_SmallMinZ)
                {
                    target = new Vector3(target.x, target.y, m_SmallMinZ);
                }
                m_CameraTween = Camera.main.transform.DOMove(target, 0.5f);
                m_ValueTween = DOTween.To(() => m_ChangeValue, x => m_ChangeValue = x, 1, 0.5f);

                DoNearFarTween(false);
                m_ViewLevel = ViewLevel.Near;
            }
        }
        else if (m_ViewLevel == ViewLevel.Near)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 || ToucheUpdate() == -1)
            {
                CloseTweener();
                Vector3 targetPos = transform.position - transform.forward * m_Dis;
                if (targetPos.x > m_BigMaxX)
                {
                    targetPos = new Vector3(m_BigMaxX, targetPos.y, targetPos.z);
                }
                if (targetPos.x < m_BigMinX)
                {
                    targetPos = new Vector3(m_BigMinX, targetPos.y, targetPos.z);
                }

                if (targetPos.z > m_BigMaxZ)
                {
                    targetPos = new Vector3(targetPos.x, targetPos.y, m_BigMaxZ);
                }
                if (targetPos.z < m_BigMinZ)
                {
                    targetPos = new Vector3(targetPos.x, targetPos.y, m_BigMinZ);
                }

                m_CameraTween = Camera.main.transform.DOMove(targetPos, 0.5f);
                m_ValueTween = DOTween.To(() => m_ChangeValue, x => m_ChangeValue = x, 0, 0.5f);
                
                DoNearFarTween(true);
                m_ViewLevel = ViewLevel.Far;
            }
        }

        m_BigMapMeshRender.material.SetFloat("_ThreValue", m_ChangeValue);
    }

    void DoNearFarTween(bool isNearToFar)
    {
        foreach (var spriteList in _big_items)
        {
            foreach (var spriteRenderer in spriteList)
            {
                spriteRenderer.DOFade(isNearToFar?1:0, 0.5f).SetAutoKill(true);
            }
        }
        foreach (var spriteList in _small_items)
        {
            foreach (var spriteRenderer in spriteList)
            {
                spriteRenderer.DOFade(isNearToFar?0:1, 0.5f).SetAutoKill(true);
            }
        }
    }

    void CloseTweener()
    {
        if (m_CameraTween != null)
        {
            m_CameraTween.Complete();
            m_CameraTween.Kill();
            m_CameraTween = null;
        }
        if (m_ValueTween != null)
        {
            m_ValueTween.Complete();
            m_ValueTween.Kill();
            m_ValueTween = null;
        }
    }

    int ToucheUpdate()
    {
        if (Input.touchCount > 1)
        {
            //当第二根手指按下的时候
            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                sum_position = Input.touches[0].position + Input.touches[1].position;

                isTwoTouch = true;
                //获取第一根手指的位置
                firstTouch = Input.touches[0].position;
                //获取第二根手指的位置
                secondTouch = Input.touches[1].position;

                lastDistance = Vector2.Distance(firstTouch, secondTouch);
            }

            //如果有两根手指按下
            if (isTwoTouch)
            {
                //每一帧都得到两个手指的坐标以及距离
                firstTouch = Input.touches[0].position;
                secondTouch = Input.touches[1].position;
                twoTouchDistance = Vector2.Distance(firstTouch, secondTouch);
                if (twoTouchDistance > lastDistance)
                {
                    return 1;
                }
                else if (twoTouchDistance < lastDistance)
                {
                    return -1;
                }
                //这一帧结束后，当前的距离就会变成上一帧的距离了
                lastDistance = twoTouchDistance;
            }

            //当第二根手指结束时（抬起）
            if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                isTwoTouch = false;
                firstTouch = Vector3.zero;
                secondTouch = Vector3.zero;
                
                sum_position = Vector2.zero;
            }
        }
        return 0;
    }

    private Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
        return d * direct.normalized + point;    
    }
}
