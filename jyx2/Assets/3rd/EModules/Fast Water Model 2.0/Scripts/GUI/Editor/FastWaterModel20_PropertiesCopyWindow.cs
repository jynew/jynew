
#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {


class PropCopy : EditorWindow {
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Properties Copy WInow", false, 10000)]
    public static void Enable()
    {   GetWindow<PropCopy>().Show();
    }
    
    static string[] __string0 = new string[20];
    static string GetSlot(int slot)
    {   return __string0[slot] ?? (__string0[slot] = EditorPrefs.GetString("EMX/W20/PC_0" + slot, ""));
    }
    static void SetSlot(string value, int slot)
    {   if (__string0[slot] != value) EditorPrefs.SetString("EMX/W20/PC_0" + slot, value);
        __string0[slot] = value;
    }
    private void OnSelectionChange()
    {   mat = null;
        Repaint();
    }
    int SLOT = 1;
    Material mat;
    
    static Dictionary<Shader, Dictionary<string, ShaderUtil.ShaderPropertyType>> shader_cache = new Dictionary<Shader, Dictionary<string, ShaderUtil.ShaderPropertyType>>();
    Rect DRAW_R;
    bool NEED_DRAW = false;
    string DRAW_NAME = "";
    int DRAW_I = -1;
    Vector4 DRAW_V;
    string[] ks;
    
    
    void CheckSHader(Shader shader)
    {   if (!shader_cache.ContainsKey(shader))
        {   shader_cache.Add(shader, new Dictionary<string, ShaderUtil.ShaderPropertyType>());
            var count = ShaderUtil.GetPropertyCount(shader);
            for (int shc = 0; shc < count; shc++)
                shader_cache[shader].Add(ShaderUtil.GetPropertyName(shader, shc), ShaderUtil.GetPropertyType(shader, shc));
        }
    }
    
    private void OnGUI()
    {   EditorGUILayout.HelpBox(@"This window to quickly copy material's parameters to another materials, or persist a set of parameters in memory.
1) Select the material in the project window
2) Open the 'PropertiesCopyWindow' window
3) Select slot 1
4) Add a few parameters to the button '+' for example '_REFR_MASK_Amount'
('FastWaterModel20Compiler' file contains the full list)
5) Press 'Copy'
(material's parameters that equaled to the buffer's parametrs will be highlighted in green
6) Select one or more materials in the project window
7) Press 'Paste' to apply saved settings to the selected material", MessageType.Info);
        mat = Selection.objects.FirstOrDefault() as Material;
        if (!mat || !mat.shader) return;
        
        GUILayout.BeginHorizontal();
        for (int i = 1; i < 8; i++)
        {   if (GUILayout.Button(i.ToString()))
            {   SLOT = i;
            }
            if (SLOT == i) EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), new Color(0, 1, 0, 0.1f));
        }
        GUILayout.EndHorizontal();
        
        var lines = GetSlot(SLOT).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        bool error = false;
        NEED_DRAW = false;
        for (int i = 0; i < lines.Length; i++)
        {   var line = lines[i];
            var param = line.Split(new[] { '#' }, StringSplitOptions.None);
            var _name = param[0];
            /* float r;*/
            /*Debug.Log(float.Parse(param[1], System.Globalization.CultureInfo.CurrentUICulture));
            Debug.Log(float.Parse(param[2]));
            Debug.Log(float.Parse(param[3]));
            Debug.Log(float.Parse(param[4]));*/
            var _v1 = new Vector4(float.Parse(param[1], System.Globalization.CultureInfo.CurrentUICulture), float.Parse(param[2], System.Globalization.CultureInfo.CurrentUICulture),
                                  float.Parse(param[3], System.Globalization.CultureInfo.CurrentUICulture), float.Parse(param[4], System.Globalization.CultureInfo.CurrentUICulture));
            // var _type = param[5];
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("TextField" + i);
            _name = EditorGUILayout.TextField(_name, GUILayout.Width(150));
            if (GUI.changed)
            {   lines[i] = _name + '#' + _v1.x + '#' + _v1.y + '#' + _v1.z + '#' + _v1.w + '#' + (int) - 1;
                SetSlot(lines.Aggregate((a, b) => a + "\n" + b), SLOT);
            }
            
            
            CheckSHader(mat.shader);
            
            
            if (!mat.HasProperty(_name))
            {   GUILayout.Label("Wrong Name");
                error = true;
                
                if (GUI.GetNameOfFocusedControl() == "TextField" + i && !string.IsNullOrEmpty(_name) && _name.Length >= 2)
                {   DRAW_R = GUILayoutUtility.GetLastRect();
                    DRAW_R.y += DRAW_R.height;
                    DRAW_R.width = 200;
                    DRAW_R.x += 50;
                    NEED_DRAW = true;
                    DRAW_NAME = _name;
                    DRAW_I = i;
                    DRAW_V = _v1;
                    
                    var la = lines.Select(l => l.Remove(l.IndexOf('#'))).ToArray();
                    
                    ks = shader_cache[mat.shader].Keys.Where(k => (k.ToLower().StartsWith(DRAW_NAME.ToLower()) || k.ToLower().Trim('_').StartsWith(DRAW_NAME.ToLower())) && !la.Contains(k)).ToArray();
                    if (ks.Length == 0) DRAW_R.height = EModules.FastWaterModel20.FastWaterModel20ControllerEditor.H;
                    else DRAW_R.height = FastWaterModel20ControllerEditor.H * ks.Length;
                    
                    var OY = DRAW_R.y;
                    DRAW_DROP(ref lines);
                    DRAW_R.y = OY;
                }
                
            }//error
            
            
            if (!error)
            {
            
            
                var _type = shader_cache[mat.shader][_name];
                GUI.enabled = false;
                EditorGUILayout.TextArea(_type.ToString(), GUILayout.Width(150));
                GUI.enabled = true;
                
                //_type = EditorGUILayout.TextField(_type, GUILayout.Width(150));
                
                
                
                //else
                {
                
                    switch (_type)
                    {   case ShaderUtil.ShaderPropertyType.Float: DrawFloat(_name, _v1); break;
                        case ShaderUtil.ShaderPropertyType.Color: DrawColor(_name, _v1); break;
                        case ShaderUtil.ShaderPropertyType.Vector: DrawVector(_name, _v1); break;
                        case ShaderUtil.ShaderPropertyType.TexEnv: DrawcTexture(_name, _v1); break;
                        default:
                            GUILayout.Label("Wrong Type");
                            error = true;
                            break;
                    }
                    
                }
                
            }
            
            
            GUILayout.FlexibleSpace();
            GUILayout.Label("/" + _v1.ToString());
            if (GUILayout.Button("-", GUILayout.Width(FastWaterModel20ControllerEditor.H)))
            {   ArrayUtility.RemoveAt(ref lines, i);
                SetSlot(lines.Aggregate((a, b) => a + "\n" + b), SLOT);
            }
            GUILayout.EndHorizontal();
            
        }
        
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+"))
        {   ArrayUtility.Add(ref lines, "#0#0#0#0#");
            SetSlot(lines.Aggregate((a, b) => a + "\n" + b), SLOT);
        }
        GUILayout.Space(FastWaterModel20ControllerEditor.H);
        GUI.enabled = !error;
        if (GUILayout.Button("Copy"))
        {   for (int i = 0; i < lines.Length; i++)
            {   var line = lines[i];
                var param = line.Split(new[] { '#' }, StringSplitOptions.None);
                lines[i] = ReadToString(param);
            }
            SetSlot(lines.Aggregate((a, b) => a + "\n" + b), SLOT);
        }
        if (GUILayout.Button("Paste"))
        {   foreach (var m in Selection.objects.Where(o => o is Material))
            {   mat = m as Material;
            
                var lines2 = GetSlot(SLOT).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                for (int i = 0; i < lines2.Length; i++)
                {   var line = lines2[i];
                    var param = line.Split(new[] { '#' }, StringSplitOptions.None);
                    Undo.RecordObject(mat, "past");
                    StringToMat(param, mat);
                    EditorUtility.SetDirty(mat);
                }
            }
        }
        
        GUI.enabled = true;
        if (NEED_DRAW)
        {   DRAW_DROP(ref lines);
        }
    }
    
    void DRAW_DROP(ref string[] lines)
    {   GUI.Box(DRAW_R, "");
    
        if (ks.Length == 0)
        {   GUI.Label(DRAW_R, "not found any fields");
        }
        else
        {   DRAW_R.height = FastWaterModel20ControllerEditor.H;
            for (int ss = 0; ss < ks.Length; ss++)
            {   var oa = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                var r = GUI.Button(DRAW_R, ks[ss]);
                GUI.skin.button.alignment = oa;
                if (r)
                {   NEED_DRAW = false;
                    DRAW_NAME = ks[ss];
                    Event.current.Use();
                    lines[DRAW_I] = DRAW_NAME + '#' + DRAW_V.x + '#' + DRAW_V.y + '#' + DRAW_V.z + '#' + DRAW_V.w + '#' + (int) - 1;
                    SetSlot(lines.Aggregate((a, b) => a + "\n" + b), SLOT);
                    EditorGUIUtility.hotControl = 0;
                    EditorGUIUtility.keyboardControl = 0;
                }
                DRAW_R.y += DRAW_R.height;
            }
        }
    }
    
    void StringToMat(string[] param, Material mat)
    {   var _name = param[0];
        //  var _type = ShaderUtil.GetPropertyType(mat.shader, Shader.PropertyToID(_name));
        CheckSHader(mat.shader);
        
        var _type = shader_cache[mat.shader][_name];
        // var _type = param[5];
        
        var _v1 = new Vector4(float.Parse(param[1], System.Globalization.CultureInfo.CurrentUICulture), float.Parse(param[2], System.Globalization.CultureInfo.CurrentUICulture),
                              float.Parse(param[3], System.Globalization.CultureInfo.CurrentUICulture), float.Parse(param[4], System.Globalization.CultureInfo.CurrentUICulture));
                              
        switch (_type)
        {   case ShaderUtil.ShaderPropertyType.Float: mat.SetFloat(_name, _v1.x); break;
            case ShaderUtil.ShaderPropertyType.Color: mat.SetColor(_name, new Color(_v1.x, _v1.y, _v1.z, _v1.w)); break;
            case ShaderUtil.ShaderPropertyType.Vector: mat.SetVector(_name, _v1); break;
            case ShaderUtil.ShaderPropertyType.TexEnv: mat.SetTexture(_name, EditorUtility.InstanceIDToObject((int)_v1.x) as Texture); break;
        }
    }
    
    string ReadToString(string[] param)
    {   var _name = param[0];
        // var _type = ShaderUtil.GetPropertyType(mat.shader, Shader.PropertyToID(_name));
        var _type = shader_cache[mat.shader][_name];
        // var _type = param[5];
        
        Vector4 result;
        switch (_type)
        {   case ShaderUtil.ShaderPropertyType.Float: result = new Vector4(mat.GetFloat(_name), 0, 0, 0); break;
            case ShaderUtil.ShaderPropertyType.Color: result = new Vector4(mat.GetColor(_name).r, mat.GetColor(_name).g, mat.GetColor(_name).b, mat.GetColor(_name).a); break;
            case ShaderUtil.ShaderPropertyType.Vector: result = new Vector4(mat.GetVector(_name).x, mat.GetVector(_name).y, mat.GetVector(_name).z, mat.GetVector(_name).w); break;
            case ShaderUtil.ShaderPropertyType.TexEnv: result = new Vector4(mat.GetTexture(_name) ? mat.GetTexture(_name).GetInstanceID() : -1, 0, 0, 0); break;
            default:
                throw new Exception();
        }
        
        
        return _name + '#' + result.x + '#' + result.y + '#' + result.z + '#' + result.w + '#' + (int)_type;
    }
    
    void DrawcTexture(string _name, Vector4 value)
    {   var mv = mat.GetTexture(_name);
        var r = EditorGUILayout.GetControlRect();
        if ((mv ? mv.GetInstanceID() : -1) == (int)value.x) EditorGUI.DrawRect(r, Color.green);
        // var oc = GUI.color;
        // if (mv.GetInstanceID() == (int)value.x) GUI.color = Color.green;
        EditorGUI.ObjectField(r, mv, typeof(Texture2D), false);
    }
    
    bool CMP(float v1, float v2)
    {   return Mathf.Abs(v1 - v2) < 0.00001;
    }
    void DrawFloat(string _name, Vector4 value)
    {   var mv = mat.GetFloat(_name);
        var r = EditorGUILayout.GetControlRect();
        if (CMP(mv, value.x)) EditorGUI.DrawRect(r, Color.green);
        GUI.Label(r, mv.ToString());
    }
    void DrawColor(string _name, Vector4 value)
    {   var mv = mat.GetColor(_name);
        var r = EditorGUILayout.GetControlRect();
        if (CMP(mv.r, value.x) && CMP(mv.g, value.y) && CMP(mv.b, value.z) && CMP(mv.a, value.w)) EditorGUI.DrawRect(r, Color.green);
        EditorGUI.ColorField(r, mv);
    }
    void DrawVector(string _name, Vector4 value)
    {   var mv = mat.GetVector(_name);
        var r = EditorGUILayout.GetControlRect();
        if (mv == value) EditorGUI.DrawRect(r, Color.green);
        GUI.Label(r, mv.ToString());
    }
}
}
#endif
