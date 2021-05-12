
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_TIZEN || UNITY_TVOS || UNITY_WP_8 || UNITY_WP_8_1
    #define MOBILE_TARGET
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif


namespace EModules.FastWaterModel20 {

#if UNITY_EDITOR
public partial class FastWaterModel20ControllerEditor : Editor {

    public static float H { get { return EditorGUIUtility.singleLineHeight;} }
    // public static float H { get { return 18;} }
    
    static GUIStyle __buttonStyle;
    static GUIStyle buttonStyle
    {   get
        {   if (__buttonStyle == null) __buttonStyle = new GUIStyle() { active = new GUIStyleState() { background = IC("modes_hover") } };
            return __buttonStyle;
        }
    }
    
    public class D_TopMenu : IDrawable {
    
    
        override public void Draw(Rect input, out Vector2[] output) { throw new System.Exception(); }
        public void Draw()
        {   //DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100, ACTIVE_COLOR );
            // DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100 , PASSIVE_COLOR );
            // DRAW_TEXTURE("i_RGB", fullRect);
            
            
            
            {   if (!target.target.compiler || !target.target.compiler.material) return;
            
                // GUI.Label( new Rect( fullRect.x, fullRect.y, fullRect.width, FastWaterModel20ControllerEditor.H ), "Independent common settings:" );
                // fullRect.y += FastWaterModel20ControllerEditor.H;
                GUILayout.Label("Common settings:");
                
                
                var r = EditorGUILayout.GetControlRect(GUILayout.Width(390), GUILayout.Height(70));
                var resetX = r.x;
                
                var S = 10;
                r.width = 0;
                r.height = 31;
                
                target.DRAW_TOGGLE(r, "Fog", FIELDS.SKIP_FOG, true, out tV);
                r.x = tV.x + S;
                target.DRAW_TOGGLE(r, "Force Opaque", "FORCE_OPAQUE", !target.target.compiler.IsKeywordEnabled("USE_SHADOWS"), out tV);
                GUI_TOOLTIP(r, "You may tor try Opaque shader with baked refraction and zdepth");
                
                //  GUI_TOOLTIP(r, "Not sure that disabled option always works");
                r.x = tV.x + S;
                
                target.DRAW_TOGGLE(r, "Receive Shadows", "USE_SHADOWS", !target.MODE_MINIMUM, out tV);
                GUI_TOOLTIP(r, "Works only with non transparent shader");
                r.x = tV.x + S;
                target.DRAW_TOGGLE(r, "Ambient", FIELDS.SKIP_AMBIENT_COLOR, true, out tV);
                
                
                
                r.y += 35;
                r.x = resetX;
                /*  var oldUFM = target.target.compiler.IsKeywordEnabled("ULTRA_FAST_MODE");
                  target.DRAW_TOGGLE(r, "Ultra Fast Mode", "ULTRA_FAST_MODE", true, out tV);
                  var newUFM = target.target.compiler.IsKeywordEnabled("ULTRA_FAST_MODE");
                  if (oldUFM != newUFM)
                  {
                      target.target.Undo(target.target.compiler);
                      target.target.compiler.WaterType = newUFM ? FastWaterModel20Compiler.WaterShaderType.UltraFast : FastWaterModel20Compiler.WaterShaderType.Standard;
                      FastWaterModel20Controller.SetDirty(target.target.compiler);
                      target.target.compiler.UpdateCompiler();
                  }
                  GUI_TOOLTIP(r, "Special Option for Mobile platforms");
                
                
                  r.x = tV.x + S;*/
                target.DRAW_TOGGLE(r, "LightMaps (test)", FIELDS.SKIP_LIGHTMAPS, true, out tV);
                // GUI_TOOLTIP(r, "If there're no genrated lightmaps then water plane will be black");
                r.x = tV.x + S;
                
                
                
                r.x = (tV.x + S - resetX) * 2 + resetX;
                
                r.x = tV.x + S;
                target.DRAW_TOGGLE(r, "GPU Instance", FIELDS.USE_GPU_INSTANCE_FEATURE, true, out tV);
                GUI_TOOLTIP(r, "DirectX 11+/ES3.0+");
                r.x = tV.x + S;
                target.DRAW_TOGGLE(r, "Stereo Mode", FIELDS.USE_VERTEX_OUTPUT_STEREO, true, out tV);
                GUI_TOOLTIP(r, "USE_VERTEX_OUTPUT_STEREO");
                
                r.x = tV.x + S;
                bool? v = target.target.UseTextureConverter;
                target.DRAW_TOGGLE(r, "Textures Converter", null, true, out tV, ref v);
                GUI_TOOLTIP(r, @"This option affects automatically baked textures
- If it's OFF then the original Render Textures will be assigned to the shader, this little slowdown the shader's work on mobile devices
- However, if you Enable this option, the conversion will take much longer time during the level initialization or the manual baking, will be better if you test it itself and try to use 32 or 64 texture's size");
                if (v != target.target.UseTextureConverter)
                {   target.target.Undo();
                    target.target.UseTextureConverter = v.Value;
                    target.target.SetDirty();
                }
            }
            
            
            
            
            GUILayout.BeginHorizontal();
            var oc = GUI.color;
            GUI.color *= target.IS_GRABPASS ? (Color)new Color32(54, 181, 255, 255) : Color.white;
            var HR = EditorGUILayout.GetControlRect(GUILayout.Height(FastWaterModel20ControllerEditor.H), GUILayout.Width(390));
            HR.width /= 2;
            EditorGUI.HelpBox(HR, target.IS_GRABPASS ? "GrabPass Enabled" : "GrabPass Disabled", target.IS_GRABPASS ? MessageType.Warning : MessageType.Info);
            if (target.IS_GRABPASS) GUI_TOOLTIP(GUILayoutUtility.GetLastRect(),
                                                    "This means that the shader will render in real time the objects behind material (Use the refraction settings to disable it)"); //r.x += r.width;
            GUI.color = oc;
            GUI.color *= target.IS_OPAQUE ? (Color)new Color32(54, 181, 255, 255) : Color.white;
            HR.x += HR.width;
            EditorGUI.HelpBox(HR, target.IS_OPAQUE ? "Opaque Enabled" : "Opaque Disabled", target.IS_OPAQUE ? MessageType.Warning : MessageType.Info);
            if (target.IS_OPAQUE) GUI_TOOLTIP(GUILayoutUtility.GetLastRect(), "This means that the shader will not be transparent (Disable shadows to disable it)");
            GUI.color = oc;
            GUILayout.EndHorizontal();
            
            
            
            GUILayout.BeginVertical(GUILayout.Width(390));
            
            if (target.IS_OPAQUE)
            {   /* var cont = new GUIContent("Material is Opaque. Basically it is required that the material receives shadows. Opaque makes Real-time ZDepth mode not available.");
                 // r.height = EditorStyles.helpBox.CalcHeight( cont, LAST_GROUP_R.width );
                 EditorGUILayout.HelpBox(cont.text, MessageType.None); //r.y += r.height;*/
            }
            /* if (target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.ALLOW_MANUAL_DEPTH && !target.IS_OPAQUE) {
            var cont = new GUIContent("You are using a mobile's devices target, the Real-time ZDepth may not work correctly, you just to make sure is it working fine or change the ZDepth \"Camera Render Realtime\" option to Baked");
            // r.height = EditorStyles.helpBox.CalcHeight( cont, LAST_GROUP_R.width );
            // r.height = FastWaterModel20ControllerEditor.H * 3;
            EditorGUILayout.HelpBox( cont.text, MessageType.Warning ); //r.y += r.height;
            }*/
            #if MOBILE_TARGET
            
            
            if (target.IS_GRABPASS)
            {
            
            
                var cont = new
                GUIContent("Note that, GrabPass does not work on the openGLES 2.0, for 2.0 it is better to use an opaque shader with baked refraction, you may make a separate material for different platforms");
                //var cont = new GUIContent("Material has GrabPass. For mobile it is better to use an opaque shader with baked refraction. if you do not need GrabPass");
                // r.height = EditorStyles.helpBox.CalcHeight( cont, LAST_GROUP_R.width );
                EditorGUILayout.HelpBox(cont.text, MessageType.Warning); //r.y += r.height;
            }
            
            if (target.target.compiler.IsKeywordEnabled("REFLECTION_PLANAR"))
            {   // var cont = new GUIContent("Material has Plannar reflection. Please change material reflection settings for mobile devices");
                var cont = new GUIContent("Material has Plannar reflection. PLease try to keep minimum resolution for reflection");
                // r.height = EditorStyles.helpBox.CalcHeight( cont, LAST_GROUP_R.width );
                EditorGUILayout.HelpBox(cont.text, MessageType.Info); //r.y += r.height;
            }
            if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_PROBE_AND_INTENSITY || target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_PROBE)
            {   var cp = target.target.transform.GetComponentInChildren<ReflectionProbe>();
                if (cp && cp.mode == UnityEngine.Rendering.ReflectionProbeMode.Realtime)
                {   var cont = new GUIContent("Note that, Mobile platform does not support RealTime reflection probes, you shoud go to the probes settings and change RealTime options to Bake");
                    // r.height = EditorStyles.helpBox.CalcHeight( cont, LAST_GROUP_R.width );
                    EditorGUILayout.HelpBox(cont.text, MessageType.Warning); //r.y += r.height;
                }
            }
            
            #endif
            
            GUILayout.EndVertical();
            
            
            
            {
            
                // GUI.Label( new Rect( fullRect.x, fullRect.y, fullRect.width, FastWaterModel20ControllerEditor.H ), "Water settings:" );
                // fullRect.y += FastWaterModel20ControllerEditor.H;
                GUILayout.Space(10);
                GUILayout.Label("Water settings:");
                
                var r = EditorGUILayout.GetControlRect(GUILayout.Width(390), GUILayout.Height(60));
                var S = 10;
                r.width = 90;
                // r.x = resetX;
                // r.y += 35;
                r = target.DRAW_BG_TEXTURE(r, "Waves Normal", FIELDS._BumpMap, true, out tV, null);
                V_NORMAL = new Vector2(r.x, r.y + r.height / 2);
                r.x += TOGGLE_WIDTH + S;
                // r.y += r.height;
                r = target.DRAW_BG_TEXTURE(r, "Main Texture", FIELDS._MainTex, true, out tV, null);
                V_MAINTEXTURE = new Vector2(r.x + r.width * .66f, r.y + r.height);
                r.x += TOGGLE_WIDTH + S;
                
                
                if (target.MODE_ADVANCEPC)
                {   r = target.DRAW_BG_TEXTURE(r, "Utils Texture", FIELDS._Utility, true, out tV, null);// r.y += r.height;
                    V_UTILS = new Vector2(r.x + r.width * .33f, r.y + r.height);
                    r.x += TOGGLE_WIDTH + S;
                    // r.height = FastWaterModel20ControllerEditor.H * 6;
                    //  EditorGUI.HelpBox(r, "(R) - Fresnel Gradient\n(G|B) - Foam Gradient", MessageType.None);// r.y += r.height;
                    
                    r = target.DRAW_BG_TEXTURE(r, "Noise", FIELDS._NoiseHQ, target.HAS_NOISEHQ_TEX, out tV, null);// r.y += r.height;
                }
                else
                {   r = target.DRAW_BG_TEXTURE(r, "Second Texture", "_UF_NMASK_Texture", target.HAS_SECOND_TEX, out tV, null);// r.y += r.height;
                    V_UTILS = new Vector2(r.x + r.width * .33f, r.y + r.height);
                    r.x += TOGGLE_WIDTH + S;
                    // r.height = FastWaterModel20ControllerEditor.H * 6;
                    //  EditorGUI.HelpBox(r, "(R) - Fresnel Gradient\n(G|B) - Foam Gradient", MessageType.None);// r.y += r.height;
                    
                    r = target.DRAW_BG_TEXTURE(r, "Utils Texture", FIELDS._Utility, true, out tV, null);// r.y += r.height;
                }
                
                
                tV = new Vector2(r.x + r.width, r.y + r.height + FastWaterModel20ControllerEditor.H);
                
                GUILayout.Space(FastWaterModel20ControllerEditor.H * 1.5f);
                //EditorGUILayout.GetControlRect()
                
            }
            
            
        }
    }
    
    
    bool HAS_SECOND_TEX { get { return !shader_cache.ContainsKey(target.compiler.shader) ? true : shader_cache[target.compiler.shader].declaredKeys.ContainsKey("HAS_SECOND_TEXTURE"); } }
    bool HAS_NOISEHQ_TEX { get { return target.compiler.IsKeywordEnabled("WAVES_GERSTNER") || target.compiler.IsKeywordEnabled("USE_CAUSTIC"); } }
    
#pragma warning disable
    static Vector2 V_NORMAL, V_MAINTEXTURE, V_UTILS;
#pragma warning restore
    
    
    static GUIContent __tooltip = new GUIContent();
    static void GUI_TOOLTIP(Rect r, string message)
    {   __tooltip.tooltip = message;
        if (r.width == 0) r.width = 31;
        if (r.height == 0) r.height = 31;
        GUI.Label(r, __tooltip);
    }
    const int BOTTOM_W = 90;
    
    
    
    public void DRAW_RRBLEND(Rect r)
    {   GUI.enabled = pop_reflection.VALUE != 0 || pop_refraction.VALUE != 0;
        //   r.y += FastWaterModel20ControllerEditor.H;
        r.height = FastWaterModel20ControllerEditor.H * 3;
        EditorGUI.HelpBox(r, "Reflection & Refraction Common Settings", MessageType.None); r.y += r.height;
        r.y += FastWaterModel20ControllerEditor.H;
        
        
        r.height = FastWaterModel20ControllerEditor.H;
        EditorGUI.HelpBox(r, "Blend", MessageType.None); r.y += r.height;
        pop_rrblend.DrawPop(null, r); r.y += r.height;
        
        
        
        
        
        
        
        r.y += FastWaterModel20ControllerEditor.H;
        
        
        var oldC = GUI.color;
        GUI.color *= new Color32(206, 255, 216, 225);
        
        
        if (pop_rrblend.VALUE == 2 || pop_rrblend.VALUE == 1)
        {   r = DRAW_SLIDER(r, "Offset", "_AverageOffset", 0, 1f, true); r.y += r.height;
        }
        
        if (pop_rrblend.VALUE == 3 || pop_rrblend.VALUE == 5)
        {   if (!ZEnabled())
            {   r.height = FastWaterModel20ControllerEditor.H * 2;
                EditorGUI.HelpBox(r, "Reques ZDepth Enable", MessageType.Error); r.y += r.height;
            }
            else
            {   r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "ZDepth", MessageType.None); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H * 2;
                DRAW_TOGGLE(r, "Inverse", "REFRACTION_Z_BLEND_INVERSE", true, out tV); r.y += 31;
                r = DRAW_SLIDER(r, "Z Offset", "_RefractionBlendOffset", -50f, 2f, true); r.y += r.height;
                r = DRAW_SLIDER(r, "Z Fade", "_RefractionBlendFade", 1f, 30, true); r.y += r.height;
            }
            
        }
        GUI.color = oldC;
        oldC = GUI.color;
        GUI.color *= new Color32(176, 216, 255, 225);
        
        if (pop_rrblend.VALUE == 4 || pop_rrblend.VALUE == 5)
        {   if (pop_rrblend.VALUE == 5) r.y += 10;
            if (target.compiler.IsKeywordEnabled("SKIP_FRESNEL_CALCULATION"))
            {   r.height = FastWaterModel20ControllerEditor.H * 2;
                EditorGUI.HelpBox(r, "Reques Fresnel Enable", MessageType.Error); r.y += r.height;
            }
            else
            {   r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Fresnel", MessageType.None); r.y += r.height;
                r = DRAW_SLIDER(r, "F Amount", "_RefrBled_Fres_Amount", 0, 10f, true); r.y += r.height;
                r = DRAW_SLIDER(r, "F Pow", "_RefrBled_Fres_Pow", 0.2f, 30, true); r.y += r.height;
            }
        }
        GUI.color = oldC;
    }
    
    public Rect DRAW_BAKED_TEXTURE(Rect r, string PROP, bool enable, BakeOrUpdateType type)
    {   var og = GUI.enabled;
        GUI.enabled = enable;
        r = DRAW_BG_TEXTURE(r, "Baked Texture", PROP, enable, out tV, null); r.y += r.height;
        if (type != BakeOrUpdateType.Reflection)
        {   r = DRAW_RESOLUTION(r, PROP + "_size"); r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H * 3;
            if (GUI.Button(r, "Bake\nand\nSave"))
            {   var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Texture", EditorPrefs.GetString("EModules/Water20/" + PROP + " Name", null), "png",
                           "Select the folder to save the baked texture ", EditorPrefs.GetString("EModules/Water20/" + PROP + " Path", null));
                if (!string.IsNullOrEmpty(path))
                {   var filename = path.Substring(path.LastIndexOf('/') + 1);
                    var normalized_path = path.Remove(path.LastIndexOf('/'));
                    EditorPrefs.SetString("EModules/Water20/" + PROP + " Name", filename);
                    EditorPrefs.SetString("EModules/Water20/" + PROP + " Path", normalized_path);
                    target.m_BakeOrUpdateTexture(type, LastResolution, path);
                }
            }
            r.y += r.height;
        }
        
        GUI.enabled = og;
        return r;
    }
    public int LastResolution;
    public Rect DRAW_RESOLUTION(Rect r, string property)
    {   bool c;
        return DRAW_RESOLUTION(r, property, out c);
    }
    public Rect DRAW_RESOLUTION(Rect r, string property, out bool wasChange)
    {   var resolution = Mathf.RoundToInt(target.compiler.GetFloat(property));
        if (resolution == 0) resolution = 256;
        r.height = FastWaterModel20ControllerEditor.H;
        List<int> VALUES = new[] { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 } .ToList();
        var indexof = VALUES.IndexOf(resolution);
        if (indexof == -1) indexof = VALUES.IndexOf(256);
        var newR = EditorGUI.Popup(r, indexof, VALUES.Select(v => v + " x " + v).ToArray());
        wasChange = false;
        if (newR != indexof)
        {   target.Undo();
            target.compiler.SetFloat(property, VALUES[newR]);
            target.SetDirty();
            wasChange = true;
        }
        LastResolution = VALUES[newR];
        return r;
    }
    
    public Rect DRAW_IMMRESED_WATER(Rect r)
    {   // if (HAS_REFRACTION(this))
        {   DRAW_TOGGLE(r, "images_zfix_a", null, true, out tV); r.y += 31;
            DRAW_TOGGLE(r, "Fix not Immersed", "SKIP_ZDEPTH_REFRACTION_DISTORTIONCORRECTION", GUI.enabled, out tV);
            GUI_TOOLTIP(r, "For high distortion values, objects that are not immersed in water can cause artifacts on water surface\n(This also affects the accuracy of the foam on the shore)"); r.y += 31;
        }
        
        // GUI.enabled = HAS_REFRACTION( target );
        // if (target.material.IsKeywordEnabled( "HAS_CAMERA_DEPTH" ))
        {   DRAW_TOGGLE(r, "images_zfix_b", null, true, out tV); r.y += 31;
            var fix = DRAW_TOGGLE(r, "Fix Immersed", "SKIP_FOAM_FINE_REFRACTIOND_DOSTORT", GUI.enabled, out tV);
            GUI_TOOLTIP(r, "Turn it on If you see artifacts on objects partially immersed in water\n(This also affects the accuracy of the foam on the shore)"); r.y += 31;
            if (fix)
            {   r = DRAW_SLIDER(r, "Fix Multyply", "_FixMulty", 0.1f, 10f, fix); r.y += r.height;
            }
        }
        
        return r;
    }
    
    void DRAW_SPEC_DISOVE(ref Rect r, string otherTarget = null)
    {   // if (MODE_ULTRA_FAST) return;
        var target = MODE_ULTRA_FAST ? "_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST" : "_APPLY_REFR_TO_SPECULAR_DISSOLVE";
        var aptspc = DRAW_TOGGLE(r, "Deep to Specular", otherTarget ?? "APPLY_REFR_TO_SPECULAR", GUI.enabled, out tV);
        GUI_TOOLTIP(r, "Fade specular or texture according water transparent. Features make small water areas more realistic"); r.y += 31;
        if (aptspc) { r = DRAW_SLIDER(r, "Dissolve", target, MODE_ULTRA_FAST ? 0 : -10.00f, MODE_ULTRA_FAST ? 1 :  10, GUI.enabled, div : 10); r.y += r.height; }
    }
    
    
    public Rect DRAW_MASK_PANEL(Rect r, string name, string help, string FEATURE,
                                string tile,
                                string offset,
                                string amount,
                                string min,
                                string max,
                                string maskDebug,
                                ref bool enable,
                                POP_CLASS pop, Vector2? amountClamp = null)
    {
    
        if (!target.compiler.material) return r;
        
        amountClamp = amountClamp ?? new Vector2(0, 5);
        
        var S = 0;
        var usemask = DRAW_TOGGLE(r, name, FEATURE, enable, out tV) & enable;
        
        var oc = GUI.color;
        GUI.color *= new Color32(255, 255, 211, 255);
        
        r.y += 31;
        if (help != null)
        {   var h = EditorStyles.helpBox.CalcHeight(new GUIContent(help), r.width);
            r.height = h;
            EditorGUI.HelpBox(r, help, MessageType.None); r.y += r.height;
        }
        
        var og = GUI.enabled;
        GUI.enabled = usemask;
        enable = usemask;
        string maskTexture;
        
        
        maskTexture = FIELDS._MainTex;
        
        Color c = new Color(1, 0, 0, 0);
        if (pop != null)
        {   r.height = FastWaterModel20ControllerEditor.H * 2;
            EditorGUI.HelpBox(r, "Used MainTexture", MessageType.None);
            r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H;
            pop.DrawPop(null, r); r.y += r.height;
            c = GET_COLOR(pop.VALUE);
        }
        else
        {   r.height = FastWaterModel20ControllerEditor.H * 2;
            EditorGUI.HelpBox(r, "Used MainTexture (R)", MessageType.None); r.y += r.height;
        }
        r = DRAW_GRAPHIC(r, 40, target.compiler.GetTexture(maskTexture) as Texture2D, usemask, c); r.y += r.height;
        // r = DRAW_SLIDER( r, "Tile XY", tile, 0, 2, usemask ); r.y += r.height + S;
        //  r = DRAW_SLIDER( r, "Mask Amount", amount, amountClamp.Value.x, amountClamp.Value.y, usemask ); r.y += r.height + S;
        r = DRAW_DOUBLEFIELDS(r, "Amount/Tile", new[] { amount, tile }, new[] { amountClamp.Value.x, 0f }, new[] { amountClamp.Value.y, 5 }, usemask, new Color32(255, 230, 180, 255), 10, seconddiv: 10);
        r.y += r.height + S;
        r = DRAW_VECTOR(r, "Offset X/Y", offset, -20, 20, enable, div: 100); r.y += r.height + S;
        r = DRAW_DOUBLEFIELDS(r, "Min/Max", new[] { min, max }, new[] { -1, 0.0f }, new[] { 2f, 10 }, usemask); r.y += r.height + S;
        
        DRAW_TOGGLE(r, "Debug Mask", maskDebug, enable, out tV);
        r.y += 31;
        
        
        
        GUI.enabled = og;
        
        GUI.color = oc;
        return r;
        //r = target.DRAW_SLIDER( r, "Mask Clamp", FIELDS._ReflectionMask_UpClamp, 0.5f, 10, usemask ); r.y += r.height + S;
    }
    
    public Rect DRAW_MASK_PANEL_CB(Rect r, string name, string help, string FEATURE,
                                   string[] _texture,
                                   POP_CLASS  texture_SWITCHER,
                                   string tile,
                                   string offset,
                                   string contrast,
                                   string brightnes,
                                   string maskDebug,
                                   ref bool enable, POP_CLASS pop)
    {
    
        if (!target.compiler.material) return r;
        
        //   amountClamp = amountClamp ?? new Vector2(0, 5);
        
        var S = 0;
        var usemask = DRAW_TOGGLE(r, name, FEATURE, enable, out tV) & enable;  r.y += 31;
        var og = GUI.enabled;
        GUI.enabled = usemask;
        enable = usemask;
        
        var oc = GUI.color;
        GUI.color *= new Color32(255, 255, 211, 255);
        
        r = DrawTextureWIthChannelAndSwitcher(r, usemask, _texture, texture_SWITCHER, pop);
        
        if (help != null)
        {   var h = EditorStyles.helpBox.CalcHeight(new GUIContent(help), r.width);
            r.height = h;
            EditorGUI.HelpBox(r, help, MessageType.None); r.y += r.height;
        }
        
        
        //string maskTexture;
        
        
        
        r = DRAW_SLIDER( r, "Tile X/Y", tile, 0, 100, usemask, div: 10); r.y += r.height + S;
        r = DRAW_VECTOR(r, "Offset X/Y", offset, -100, 100, enable, div: 100); r.y += r.height + S;
        r = DRAW_SLIDER( r, "Contrast", contrast, 0, 100, usemask ); r.y += r.height + S;
        r = DRAW_SLIDER( r, "Brightnes", brightnes, -100, 100, usemask ); r.y += r.height + S;
        //  r = DRAW_DOUBLEFIELDS(r, "Brightnes", new[] { min, max }, new[] { -1, 0.0f }, new[] { 2f, 10 }, usemask); r.y += r.height + S;
        
        DRAW_TOGGLE(r, "Debug Mask", maskDebug, enable, out tV);
        r.y += 31;
        
        
        
        GUI.enabled = og;
        
        GUI.color = oc;
        return r;
        //r = target.DRAW_SLIDER( r, "Mask Clamp", FIELDS._ReflectionMask_UpClamp, 0.5f, 10, usemask ); r.y += r.height + S;
    }
    
    public Rect DrawTextureWIthChannelAndSwitcher(Rect r,  bool enable,
            string[] _texture,
            POP_CLASS  texture_SWITCHER, POP_CLASS pop)
    {   int index = 0;
        if (texture_SWITCHER != null)
        {   r.height = FastWaterModel20ControllerEditor.H;
            texture_SWITCHER.DrawPop(null, r); r.y += r.height;
            index = texture_SWITCHER.VALUE;
        }
        string texture = _texture[index];
        
        // maskTexture = FIELDS._MainTex;
        
        //Color c = new Color(1, 0, 0, 0);
        
        //r.height = FastWaterModel20ControllerEditor.H * 2;
        // EditorGUI.HelpBox(r, "Used MainTexture (R)", MessageType.None); r.y += r.height;
        // r = DRAW_GRAPHIC(r, 40, target.compiler.GetTexture(maskTexture) as Texture2D, usemask, c); r.y += r.height;
        
        
        
        if (index == texture_SWITCHER.defaultIndex)        { r = DRAW_BG_TEXTURE(r, "Texture", texture, enable, out tV, null); r.y += r.height; }
        else
        {   r = DRAW_GRAPHIC(r, 50, target.compiler.GetTexture(texture) as Texture2D, enable); r.y += r.height;
            //  r = DRAW_BG_TEXTURE(r, "Texture", texture, usemask, out tV, null); r.y += r.height;
        }
        Color c = new Color(1, 0, 0, 0);
        if (pop != null)
        {   //r.height = FastWaterModel20ControllerEditor.H * 2;
            // EditorGUI.HelpBox(r, "Used MainTexture", MessageType.None);
            //r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H;
            pop.DrawPop(null, r); r.y += r.height;
            c = GET_COLOR(pop.VALUE);
        }
        else
        {   r.height = FastWaterModel20ControllerEditor.H * 2;
            EditorGUI.HelpBox(r, "Used MainTexture (R)", MessageType.None); r.y += r.height;
        }
        r = DRAW_GRAPHIC(r, 30, target.compiler.GetTexture(texture) as Texture2D, enable, c); r.y += r.height;
        return r;
    }
    
    
    
    static bool __ASSIGN_LUT_B;
    static Texture __ASSIGN_LUT_T;
    static Texture ASSIGN_LUT()
    {
    
        if (__ASSIGN_LUT_B) return __ASSIGN_LUT_T;
        __ASSIGN_LUT_B = true;
        var pat = "Photographic Lens 1 ++.png".ToLower();
        var path = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.ToLower().EndsWith(pat));
        if (string.IsNullOrEmpty(path)) return null;
        __ASSIGN_LUT_T = AssetDatabase.LoadAssetAtPath<Texture>(path);
        
        return __ASSIGN_LUT_T;
    }
    
}



#endif

}

