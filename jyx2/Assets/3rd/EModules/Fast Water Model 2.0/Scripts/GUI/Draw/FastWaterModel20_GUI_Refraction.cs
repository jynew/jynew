
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {


    POP_CLASS __pop_refraction;
    public POP_CLASS pop_refraction
    {   get
        {   return __pop_refraction ?? (__pop_refraction = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.REFRACTION_NONE, FIELDS.REFRACTION_ONLYZCOLOR, FIELDS.REFRACTION_BAKED_FROM_TEXTURE, FIELDS.REFRACTION_BAKED_VIA_SCRIPT, FIELDS.REFRACTION_BAKED_ONAWAKE, FIELDS.REFRACTION_GRABPASS },
                contents = new string[] { "none", "No Texture Only Depth Color", "Bake Texture in Editor", "Bake Runtime Via Script", "Bake Runtime On Awake", "RealTime GrabPass" },
                defaultIndex = 5
            });
        }
    }
    POP_CLASS __pop_refraction_blur;
    public POP_CLASS pop_refraction_blur
    {   get
        {   return __pop_refraction_blur ?? (__pop_refraction_blur = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.REFRACTION_BLUR_1, FIELDS.REFRACTION_BLUR_2, FIELDS.REFRACTION_BLUR_3 },
                contents = new string[] { "1 Interation", "2 Interations", "3 Interations" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_refrmaskb;
    public POP_CLASS pop_refrmaskb
    {   get
        {   return __pop_refrmaskb ?? (__pop_refrmaskb = new POP_CLASS()
            {   target = this,
                    keys = new[] { "REFR_MASK_R", "REFR_MASK_G", "REFR_MASK_B", "REFR_MASK_A" },
                contents = new string[] { "MainTex (R)", "MainTex (G)", "MainTex (B)", "MainTex (A)" },
                defaultIndex = 3
            });
        }
    }
    
    POP_CLASS __pop_caustintop;
    public POP_CLASS pop_caustintop
    {   get
        {   return __pop_caustintop ?? (__pop_caustintop = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "CAUSTIC_BOTTOM", "CAUSTIC_TOPBOTTOM" },
                contents = new string[] { "At Top", "At Bottom", "Top And Bottom" },
                defaultIndex = 0
            });
        }
    }
    POP_CLASS __pop_causteffect;
    public POP_CLASS pop_causteffect
    {   get
        {   return __pop_causteffect ?? (__pop_causteffect = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "C_BLUR", "C_ANIM" },
                contents = new string[] { "none", "Blur", "Animation" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    
    public class D_Refraction : IDrawable {
        override public void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 400, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var og = GUI.enabled;
            
            var S = 0;
            
            target.DRAW_TOGGLE(r, "images_refraction", null, true, out tV); r.y += 31;
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Refraction", MessageType.None); r.y += r.height;
            var oldV = target.pop_refraction.VALUE;
            target.pop_refraction.DrawPop(null, r); r.y += r.height;
            var wasChanged = target.pop_refraction.VALUE != oldV;
            if (target.pop_refraction.VALUE == 0)
            {   r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "None", MessageType.None); r.y += r.height;
            }
            else
            {
            
                var haveMask = false;
                if (target.pop_refraction.VALUE == 2)
                {   //REFRACTION_BAKED_FROM_TEXTURE
                    target.target.compiler.SetTexture(FIELDS.baked_RefractionTex, target.target.backed_refraction);
                    r = target.DRAW_BAKED_TEXTURE(r, FIELDS.baked_RefractionTex, true, BakeOrUpdateType.Refraction);
                    if (target.target.backed_refraction != target.target.compiler.GetTexture(FIELDS.baked_RefractionTex))
                    {   target.target.Undo();
                        target.target.backed_refraction = target.target.compiler.GetTexture(FIELDS.baked_RefractionTex) as Texture2D;
                        target.target.SetDirty();
                    }
                    
                    r.height = FastWaterModel20ControllerEditor.H * 4;
                    EditorGUI.HelpBox(r, "Different objects with same material may use different textures", MessageType.None); r.y += r.height;
                    r.y += r.height;
                }
                else if (target.pop_refraction.VALUE == 3)
                {   //REFRACTION_BAKED_VIA_SCRIPT
                    r = target.DRAW_RESOLUTION(r, FIELDS.baked_RefractionTex_temp_size); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H * 7;
                    EditorGUI.HelpBox(r, "Use BakeOrUpdateTexture( BakeOrUpdateType.ZDepth );", MessageType.None); r.y += r.height;
                    haveMask = true;
                }
                else if (target.pop_refraction.VALUE == 4)
                {   //REFRACTION_BAKED_ONAWAKE
                    bool rc;
                    r = target.DRAW_RESOLUTION(r, FIELDS.baked_RefractionTex_temp_size, out rc); r.y += r.height;
                    if (wasChanged || rc) target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.Refraction, target.LastResolution);
                    r.height = FastWaterModel20ControllerEditor.H * 3;
                    EditorGUI.HelpBox(r, "Texture will baking OnEnable once", MessageType.None); r.y += r.height;
                    haveMask = true;
                }
                else if (target.pop_refraction.VALUE == 5)
                {   //GRABPASS
                    r.height = FastWaterModel20ControllerEditor.H * 3;
                    EditorGUI.HelpBox(r, "Material will use Realtime GrabPass", MessageType.None); r.y += r.height;
                    
                    if (target.IS_OPAQUE)
                    {   r.height = FastWaterModel20ControllerEditor.H * 6;
                        EditorGUI.HelpBox(r, "GrabPass not working with opaque shader", MessageType.Error); r.y += r.height;
                    }
                }
                else
                {   GUI.enabled = target.pop_refraction.VALUE != 0;
                }
                
                if (haveMask)
                {   r = target.DRAW_KAYERMASK(r, "Render Layers", "_RefractionBakeLayers", () => target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.Refraction, target.LastResolution)); r.y += r.height;
                }
                
                
                var venable = true;
                if (target.pop_refraction.VALUE == 2 || target.pop_refraction.VALUE == 3 || target.pop_refraction.VALUE == 4)
                {   var oldValue = target.target.compiler.BakedRefractionCameraOffset;
                    var newValue = oldValue;
                    Vector2 ov;
                    r = target.DRAW_SLIDER(r, "Baking Offset", null, compilerValue: ref newValue, leftValue: 0, rightValue: 200, enable: GUI.enabled, output: out ov);
                    GUI_TOOLTIP(r, "Use this to fix artifacts near the shore, but be careful, so far as another objects can be baked als, use culling mask layers to exlude them");
                    r.y += r.height;
                    if (oldValue != newValue)
                    {   target.target.Undo();
                        target.target.compiler.BakedRefractionCameraOffset = newValue;
                        target.target.SetDirty();
                        target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.Refraction, target.LastResolution);
                    }
                    
                    r.y += FastWaterModel20ControllerEditor.H;
                    venable = target.DRAW_TOGGLE(r, "Volume Calc Refr", "SKIP_REFRACTION_CALC_DEPTH_FACTOR", GUI.enabled && target.ZEnabled(), out tV) & GUI.enabled && target.ZEnabled();
                    GUI_TOOLTIP(r, "Additional calculations to give the volume to flat refraction texture");
                    r.y += 31;
                    if (venable)
                    {   r = target.DRAW_SLIDER(r, "Volume Factor", "_RefrDeepFactor", 16, 4096, GUI.enabled, div: 0.01f); r.y += r.height;
                    }
                    
                }
                if (!target.MODE_ULTRA_FAST)
                {   if (venable && !target.pop_depth.IS(FIELDS.ALLOW_MANUAL_DEPTH))
                    {   r.y += FastWaterModel20ControllerEditor.H;
                        target.DRAW_TOGGLE(r, "Deep Color Fine", "SKIP_ADDITIONAL_DEEP_COLOR_CALC", GUI.enabled && target.ZEnabled(), out tV);
                        GUI_TOOLTIP(r, "Additional calculations for deep color, it take double z depth texture unpacking");
                        r.y += 31;
                    }
                    
                    
                }
                
                
                
                
                
                
                if (target.MODE_ULTRA_FAST)
                {
                
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.d_D_Foam.LowDistortion(ref r, target);
                    
                    r.x = 100;
                    r.y = 0;
                    
                    r.height = FastWaterModel20ControllerEditor.H;
                    EditorGUI.HelpBox(r, "Deep Settings", MessageType.None); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Z Offset", FIELDS._RefrZOffset, -30, 10, GUI.enabled); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Z FallOff", FIELDS._RefrZFallOff, 1, 100, GUI.enabled); r.y += r.height + S;
                    r.y += 10;
                    r = target.DRAW_SLIDER_WITHTOGGLE(r, "Distor ZDepth", "_RefrDistortionZ", 0, 600, GUI.enabled, "USE_ZD_DISTOR"); r.y += r.height + S;
                    target.DRAW_TOGGLE(r, "Fix Distortion", "FIX_DISTORTION", target.target.compiler.IsKeywordEnabled("USE_ZD_DISTOR")); r.y += 31;
                    r = target.DRAW_SLIDER_WITHTOGGLE(r, "Distor Texture", FIELDS._RefrDistortion, 0, 600, GUI.enabled, "USE_REFR_DISTOR"); r.y += r.height + S;
                    
                    
                    
                    
                    var haveDist = target.target.compiler.IsKeywordEnabled("USE_REFR_DISTOR") && ((target.pop_refraction.VALUE != 1) || target.pop_refraction.VALUE == 1);
                    var enen = GUI.enabled;
                    GUI.enabled  = haveDist;
                    r = target.DRAW_SLIDER_WITHTOGGLE(r, "Low Distortion", "_RefrLowDist", 0, 1, GUI.enabled, "USE_REFR_LOW_DISTOR", div: 10); r.y += r.height + S;
                    var usezf = haveDist & target.DRAW_TOGGLE(r, "Depth to Distor", "HAS_REFR_Z_AFFECT_BUMP", haveDist); r.y += 31;
                    GUI.enabled  = usezf;
                    r = target.DRAW_SLIDER(r, null, "_TexRefrDistortFix", 0, 1, usezf, div: 100); r.y += r.height + S;
                    r.y += FastWaterModel20ControllerEditor.H;
                    GUI.enabled                        = enen;
                    r.height = FastWaterModel20ControllerEditor.H * 5;
                    
                    
                    var sha = target.DRAW_TOGGLE(r, "Shore Alpha Override", "SKIP_FOAM_ALPHA_OVERRIDE", usezf, out tV); r.y += 31;
                    
                    var warningFlat = sha && !target.pop_refraction.IS(FIELDS.REFRACTION_GRABPASS)
                                      && !target.IS_OPAQUE
                                      && target.target.compiler.IsKeywordEnabled("FOAM_ALPHA_FLAT");
                    var oc = GUI.color;
                    target.DRAW_TOGGLE(r, "Flat Shore Alpha", "FOAM_ALPHA_FLAT", target.target.compiler.IsKeywordEnabled("UFAST_SHORE_1") && sha, out tV); r.y += 31;
                    GUI.enabled  = sha;
                    EditorGUI.HelpBox(r, "Use 'Flat Shore Alpha' to fix unwanted distortions near the shore", MessageType.None); r.y += r.height + S;
                    if (warningFlat)
                    {   if (warningFlat && target.target.compiler.IsKeywordEnabled("UFAST_SHORE_1")) GUI.color *= Color.yellow;
                        r.height = FastWaterModel20ControllerEditor.H * 8;
                        EditorGUI.HelpBox(r, "You are using flat refraction and transparent shader both, if you wanna have transparent affect the coastline you have to disable flat refraction", MessageType.None);
                        r.y += r.height;
                    }
                    GUI.enabled                        = enen;
                    GUI.color = oc;
                    
                    r.x = 200;
                    r.y = 0;
                    
                    
                    var usefog = target.DRAW_TOGGLE(r, "Use Deep Fog", "USE_DEPTH_FOG", target.pop_refraction.VALUE != 1) || target.pop_refraction.VALUE == 1; r.y += 31;
                    r = target.DRAW_SLIDER(r, "Top Amount", "_RefrTopAmount", 0, 16, GUI.enabled); r.y += r.height + S;
                    r = target.DRAW_COLOR(r, "Top Fog", "_RefrTopZColor", usefog, out tV, false); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Deep Amount", "_RefrDeepAmount", 0, 16, GUI.enabled); r.y += r.height + S;
                    r = target.DRAW_COLOR(r, "Deep Fog", FIELDS._RefrZColor, usefog, out tV, false); r.y += r.height;
                    
                }
                else
                {   r.y += FastWaterModel20ControllerEditor.H;
                
                
                    r = target.DRAW_SLIDER(r, "Amount", "_RefrAmount", 0, 16, GUI.enabled); r.y += r.height + S;
                    var desat = target.DRAW_TOGGLE(r, "DeSaturate", "DESATURATE_REFR", GUI.enabled, out tV) & GUI.enabled; r.y += 31;
                    if (desat) { r = target.DRAW_SLIDER(r, null, "_RefractionDesaturate", 0f, 1f, desat); r.y += r.height + S; }
                    
                    r = target.DRAW_SLIDER_WITHTOGGLE(r, "Distor ZDepth", "_RefrDistortionZ", 0, 600, GUI.enabled, "USE_ZD_DISTOR"); r.y += r.height + S;
                    
                    
                    r = target.DRAW_SLIDER_WITHTOGGLE(r, "Distor Texture", FIELDS._RefrDistortion, 0, 600, GUI.enabled, "USE_REFR_DISTOR"); r.y += r.height + S;
                    target.DRAW_TOGGLE(r, "Angle Distor", "ADDITIONAL_ANGLE_DISTORTION", GUI.enabled, out tV);
                    GUI_TOOLTIP(r, "Additional distortion if the camera top to water"); r.y += 31;
                    var fde = target.DRAW_TOGGLE(r, "Foam Distor", "ADDITIONAL_FOAM_DISTORTION", GUI.enabled, out tV); r.y += 31;
                    if (fde)
                    {   r = target.DRAW_SLIDER(r, "Amount", "ADDITIONAL_FOAM_DISTORTION_AMOUNT", 0, 50, GUI.enabled); r.y += r.height + S;
                    }
                    
                    r.y += FastWaterModel20ControllerEditor.H;
                    DRAW_R_BLUR(ref r);
                    
                    
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.d_D_Foam.LowDistortion(ref r, target);
                    
                    r.x = 100;
                    r.y = 0;
                    
                    
                    GUI.enabled = target.pop_refraction.VALUE >= 2;
                    r.height = FastWaterModel20ControllerEditor.H;
                    //  GUI.Label( r, "Fog" ); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Top Fog", "_RefrTextureFog", 0, 1, GUI.enabled); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Deep Fog", "_RefrRecover", 0, 1, GUI.enabled, inverce: true); r.y += r.height;
                    GUI.enabled = target.pop_refraction.VALUE >= 1;
                    r.height = FastWaterModel20ControllerEditor.H;
                    GUI.Label(r, "Deep"); r.y += r.height;
                    r = target.DRAW_COLOR(r, "Top Color", "_RefrTopZColor", GUI.enabled, out tV, false); r.y += r.height;
                    r = target.DRAW_COLOR(r, "Deep Color", FIELDS._RefrZColor, GUI.enabled, out tV, false); r.y += r.height;
                    // r.height = FastWaterModel20ControllerEditor.H;
                    //EditorGUI.HelpBox( r, "Depth", MessageType.None ); r.y += r.height;
                    //target.DRAW_TOGGLE( r, "HQ Y-Depth calc", FIELDS.SKIP_Z_WORLD_CALCULATION, true, out tV ); r.y += 31;
                    r = target.DRAW_SLIDER(r, "Offset", FIELDS._RefrZOffset, -30, 10, GUI.enabled); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "FallOff", FIELDS._RefrZFallOff, 1, 100, GUI.enabled); r.y += r.height + S;
                    
                    
                    //GUI_TOOLTIP( r, "Set 1 if you want the depth color not to overlap the texture" ); r.y += r.height + S;
                    
                    
                }
                target.DRAW_SPEC_DISOVE(ref r);
                
                r.y += 10;
                target.DRAW_TOGGLE(r, "Debug Depth", "REFRACTION_DEBUG", GUI.enabled); r.y += 31;
                target.DRAW_TOGGLE(r, "Debug Refraction", "REFRACTION_DEBUG_RGB", GUI.enabled); r.y += 31;
                
                //if (!target.MODE_MINIMUM)
                {   r.y += FastWaterModel20ControllerEditor.H;
                    var rbl = target.DRAW_TOGGLE(r, "Fresnel Blend", "USE_REFRACTION_BLEND_FRESNEL", true, out tV); r.y += 31;
                    r = target.DRAW_SLIDER(r, "Amount", "_RefractionBLendAmount", 0f, 1, rbl); r.y += r.height + S;
                    target.DRAW_TOGGLE(r, "Inverse", "SKIP_REFRACTION_BLEND_FRESNEL_INVERCE", rbl, out tV); r.y += 31;
                    target.DRAW_TOGGLE(r, "Debug Blend Val", "DEBUG_REFR_BLEND", rbl); r.y += 31;
                    
                    
                }
                
                
                if (!target.MODE_ULTRA_FAST)
                {
                
                    r.y += FastWaterModel20ControllerEditor.H;
                    
                    //////////////////////////////
                    string FEATURE = "REFR_MASK";
                    string tile = "_REFR_MASK_Tile";
                    string amount = "_REFR_MASK_Amount";
                    string min = "_REFR_MASK_min";
                    string max = "_REFR_MASK_max";
                    string offset = "_REFR_MASK_offset";
                    bool enable = GUI.enabled;
                    POP_CLASS pop = target.pop_refrmaskb;
                    //////////////////////////////
                    r = target.DRAW_MASK_PANEL(r, "Mask", null, FEATURE, tile, offset, amount, min, max, "REFR_MASK_DEBUG", ref enable, pop);
                    //////////////////////////////
                    
                    r.x = 200;
                    r.y = 0;
                    
                    
                    
                    GUI.enabled = target.DRAW_TOGGLE(r, "Fast Caustic", "USE_CAUSTIC", GUI.enabled, out tV); r.y += 31;
                    //r = target.DRAW_BG_TEXTURE(r, null, "_NoiseHQ", GUI.enabled, out tV, null); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H;
                    
                    var og22 = GUI.enabled;
                    GUI.enabled = !target.target.compiler.IsKeywordEnabled("SKIP_C_NOISE") && GUI.enabled;
                    EditorGUI.HelpBox(r, "Noise", MessageType.None); r.y += r.height;
                    
                    r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._NoiseHQ) as Texture2D, GUI.enabled); r.y += r.height;
                    
                    r = target.DRAW_VECTOR(r, "Tiling / Aspect", "_CAUSTIC_Tiling", 0.1f, 100, GUI.enabled); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Speed", "_CAUSTIC_Speed", 0, 3, GUI.enabled, div: 10); r.y += r.height;
                    r = target.DRAW_VECTOR(r, "Noise -Y / Y", "_CAUSTIC_Offset", -100f, 1000, GUI.enabled); r.y += r.height;
                    // r.height = FastWaterModel20ControllerEditor.H * 2;
                    //  EditorGUI.HelpBox(r, "Green Y offset Red X offset", MessageType.None); r.y += r.height;
                    target.DRAW_TOGGLE(r, "Debug Noise", "DEBUG_CAUSTIC_NOISE", GUI.enabled, out tV); r.y += 31;
                    GUI.enabled = og22;
                    
                    var oldT = target.target.compiler.GetVector("_CAUSTIC_Offset").z;
                    r = target.DRAW_VECTOR(r, "Noise Affect", "_CAUSTIC_Offset", 0, 2, GUI.enabled, true, 1, div: 100); r.y += r.height;
                    var newT = target.target.compiler.GetVector("_CAUSTIC_Offset").z;
                    if (oldT != newT && (oldT == 0 || newT == 0))
                    {   target.target.Undo();
                        if (newT == 0) target.target.compiler.EnableKeyword("SKIP_C_NOISE");
                        else target.target.compiler.DisableKeyword("SKIP_C_NOISE");
                        target.target.SetDirty();
                    }
                    
                    
                    r = target.DRAW_BG_TEXTURE(r, null, "_CAUSTIC_MAP", GUI.enabled, out tV, null); r.y += r.height;
                    r = target.DRAW_VECTOR(r, "Tiling X / Y", "_CAUSTIC_Tiling", 0.1f, 1000, GUI.enabled, true); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H;
                    EditorGUI.HelpBox(r, "Placement", MessageType.None); r.y += r.height;
                    target.pop_caustintop.DrawPop(null, r); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Amount", "_CAUSTIC_FOG_Amount", 0, 50, GUI.enabled, div: 10); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Pow", "_CAUSTIC_FOG_Pow", 1, 50, GUI.enabled); r.y += r.height;
                    target.DRAW_TOGGLE(r, "Colorize", "USE_COLOR_CAUSTIC", GUI.enabled, out tV); r.y += 31;
                    target.DRAW_TOGGLE(r, "Fix Y Lines", "USE_FIXV_CAUSTIC", GUI.enabled, out tV);
                    GUI_TOOLTIP(r, "You may use this little fix, to improve drawing along Y axe");
                    r.y += 31;
                    
                    r.height = FastWaterModel20ControllerEditor.H;
                    EditorGUI.HelpBox(r, "Effect", MessageType.None); r.y += r.height;
                    target.pop_causteffect.DrawPop(null, r); r.y += r.height;
                    if (target.pop_causteffect.VALUE == 1)
                    {   //var cblur = target.DRAW_TOGGLE(r, "Blur", "C_BLUR", GUI.enabled, out tV) & GUI.enabled; r.y += 31;
                        r = target.DRAW_SLIDER(r, "Blur Radius", "_C_BLUR_R", 0, 0.2f, true, div: 1000); r.y += r.height;
                    }
                    else if (target.pop_causteffect.VALUE == 2)
                    {   r = target.DRAW_SLIDER(r, "Anim Speed", "_C_BLUR_S", 0, 3f, true, div: 100); r.y += r.height;
                    
                    }
                    
                    // r = target.DRAW_SLIDER(r, "Tiling", "_CAUSTIC_Tiling", 0.01f, 100, GUI.enabled); r.y += r.height;
                    //   r = target.DRAW_VECTOR(r, "Offset", "_CAUSTIC_Offset", -100f, 100, GUI.enabled,true,1); r.y += r.height;
                    
                    /*
                    r = target.DRAW_VECTOR(r, "Tiling X / Y", "_CAUSTIC_PROC_Tiling", 0.1f, 1000, GUI.enabled, true); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Speed", "_CAUSTIC_PROC_GlareSpeed", 0, 10, GUI.enabled, div: 10); r.y += r.height;
                    r = target.DRAW_DOUBLEFIELDS(r, "Min/Max", new[] { "_CAUSTIC_PROC_Contrast", "_CAUSTIC_PROC_BlackOffset" }, new[] { -1, -1f }, new[] { 1f, 1 }, GUI.enabled); r.y += r.height + S;
                    */
                    
                    target.DRAW_TOGGLE(r, "Debug Caustic", "DEBUG_CAUSTIC", GUI.enabled, out tV); r.y += 31;
                    
                    
                    
                    
                }
                
                
                
                
                
                
                
                
                
                if (target.MODE_ULTRA_FAST)
                {   r.x = 300;
                    r.y = 0;
                }
                else
                {   r.y += FastWaterModel20ControllerEditor.H;
                
                }
                
                DRAW_R_BLUR(ref r);
                
                
                if (!target.MODE_ULTRA_FAST)
                {
                
                    r.x = 300;
                    r.y = 0;
                    // GUI.enabled = foam_enable;
                    
                    target.DRAW_RRBLEND(r);
                }
                
                //r.height = FastWaterModel20ControllerEditor.H;
                // GUI.Label( r, "Settings" ); r.y += r.height;
                
                
                
                if (target.pop_depth.VALUE != 0)
                {   //if (target.pop_depth.IS( FIELDS.ALLOW_MANUAL_DEPTH )) {
                    //r = target.DRAW_IMMRESED_WATER( r );
                    // r.height = FastWaterModel20ControllerEditor.H * 6;
                    // EditorGUI.HelpBox( r, "If you have accuracy problems with the foam near the shoreю Try to change ZDepth settings", MessageType.None );
                }
                
                
            }
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
        
        
        void DRAW_R_BLUR(ref Rect r)
        {
        
            var bb = GUI.enabled;
            GUI.enabled = target.pop_refraction.VALUE >= 2;
            var en_blur = target.DRAW_TOGGLE(r, "Blur", FIELDS.REFRACTION_BLUR, GUI.enabled, out tV) & GUI.enabled; r.y += 31;
            GUI.enabled = en_blur;
            
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_refraction_blur.DrawPop(null, r); r.y += r.height;
            if (en_blur)
            {   if (target.MODE_MINIMUM && target.pop_refraction_blur.VALUE >= 0)
                {   r.height = FastWaterModel20ControllerEditor.H * 6;
                    EditorGUI.HelpBox(r, "Minimum Mode doesn't support blur", MessageType.Error); r.y += r.height;
                }
                else if (target.MODE_ULTRA_FAST && target.pop_refraction_blur.VALUE > 0)
                {   r.height = FastWaterModel20ControllerEditor.H * 6;
                    EditorGUI.HelpBox(r, "Ultra Fast Mode only 1 Interation awaliable", MessageType.Error); r.y += r.height;
                }
                r = target.DRAW_SLIDER(r, "Radius", FIELDS._RefrBlur, 0, 10, en_blur); r.y += r.height;
                var skip_zd = target.DRAW_TOGGLE(r, "Depth Affect", "SKIP_REFRACTION_BLUR_ZDEPENDS", GUI.enabled, out tV) & en_blur;
                GUI_TOOLTIP(r, "Water will be blurred depending on the depth"); r.y += 31;
                if (skip_zd && !target.ZEnabled())
                {   r.height = FastWaterModel20ControllerEditor.H * 2;
                    EditorGUI.HelpBox(r, "Requests ZDepth", MessageType.Error); r.y += r.height;
                }
                
                r = target.DRAW_SLIDER(r, "Depth Offset", "_RefractionBlurZOffset", -10, 3, skip_zd); r.y += r.height;
                
            }
            GUI.enabled = bb;
        }
    }
    
    
}
}
#endif
