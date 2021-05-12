using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(bl_HUDText))]
public class bl_HUDTextEditor : Editor
{

    // target component
    public bl_HUDText m_Component = null;


    private GUIStyle m_NoteStyle = null;

    public void OnEnable()
    {
        m_Component = (bl_HUDText)target;
    }

    public override void OnInspectorGUI()
    {
        MainComponents();
    }

    void MainComponents()
    {
        GUI.enabled = false;
        GUILayout.BeginVertical("box");
        GUILayout.BeginVertical("box");
        GUILayout.Label("Reference transform where texts are instantiated", NoteStyle);
        GUI.enabled = true;
        m_Component.CanvasParent = EditorGUILayout.ObjectField("Canvas Parent", m_Component.CanvasParent, typeof(Transform), true) as Transform;
        m_Component.TextPrefab = EditorGUILayout.ObjectField("Text Prefab", m_Component.TextPrefab, typeof(GameObject), true) as GameObject;
        m_Component.m_Type = (bl_HUDText.GameType)EditorGUILayout.EnumPopup("Game Type: ", m_Component.m_Type);
        GUI.enabled = false;
        GUILayout.EndVertical();
        GUILayout.Space(7);
        EditorGUILayout.Separator();
        GUILayout.BeginVertical("box");
        GUILayout.Label("Global Settings", NoteStyle);
        GUI.enabled = true;
        m_Component.m_TextAnimationType = (bl_HUDText.TextAnimationType)EditorGUILayout.EnumPopup("Animation Type: ", m_Component.m_TextAnimationType);
        m_Component.FadeSpeed = EditorGUILayout.Slider("Fade Speed", m_Component.FadeSpeed, 0, 100);
        m_Component.FadeCurve = EditorGUILayout.CurveField("Fade Curve", m_Component.FadeCurve);

        m_Component.FloatingSpeed = EditorGUILayout.Slider("Floating Speed", m_Component.FloatingSpeed, 0, 100);
        m_Component.HideDistance = EditorGUILayout.Slider("Hide Distance", m_Component.HideDistance, 0, 1000);
        m_Component.MaxViewAngle = EditorGUILayout.Slider("Max View Angle", m_Component.MaxViewAngle, 0, 180);
        m_Component.FactorMultiplier = EditorGUILayout.Slider("Factor Multiplier", m_Component.FactorMultiplier, 0.1f, 1.0f);
        m_Component.DelayStay = EditorGUILayout.Slider("Delay Movement", m_Component.DelayStay, 0.0f, 5.0f);
        EditorGUILayout.Separator();
        m_Component.CanReuse = EditorGUILayout.Toggle("Can Re-use", m_Component.CanReuse);
        if (m_Component.CanReuse)
        {
            m_Component.MaxUses = EditorGUILayout.IntSlider("Max Re-uses", m_Component.MaxUses, 0, 10);
        }
        EditorGUILayout.Separator();
        m_Component.DestroyTextOnDeath = EditorGUILayout.ToggleLeft("Destroy Text On Death", m_Component.DestroyTextOnDeath);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUI.enabled = false;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(m_Component);
        }

    }

    public GUIStyle NoteStyle
    {
        get
        {
            if (m_NoteStyle == null)
            {
                m_NoteStyle = new GUIStyle("Label");
                m_NoteStyle.fontSize = 9;
                m_NoteStyle.alignment = TextAnchor.LowerCenter;
            }
            return m_NoteStyle;
        }
    }

    public bool SmallToggle(string label, bool state)
    {

        EditorGUILayout.BeginHorizontal();
        state = GUILayout.Toggle(state, label, GUILayout.MaxWidth(12));
        GUILayout.Label(label, LeftAlignedPathStyle);
        EditorGUILayout.EndHorizontal();

        return state;

    }

    private GUIStyle m_LeftAlignedPathStyle = null;
    public GUIStyle LeftAlignedPathStyle
    {
        get
        {
            if (m_LeftAlignedPathStyle == null)
            {
                m_LeftAlignedPathStyle = new GUIStyle("Label");
                m_LeftAlignedPathStyle.fontSize = 9;
                m_LeftAlignedPathStyle.alignment = TextAnchor.LowerLeft;
                m_LeftAlignedPathStyle.padding = new RectOffset(0, 0, 2, 0);
            }
            return m_LeftAlignedPathStyle;
        }
    }
}