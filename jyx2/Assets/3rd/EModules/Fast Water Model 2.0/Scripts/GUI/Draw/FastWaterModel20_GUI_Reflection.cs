
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {

    POP_CLASS __pop_rrblend;
    public POP_CLASS pop_rrblend
    {   get
        {   return __pop_rrblend ?? (__pop_rrblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "RRMAXBLEND", "RRMULTIBLEND", "RRSIMPLEBLEND", "REFRACTION_Z_BLEND", "RRFRESNEL", "REFRACTION_Z_BLEND_AND_FRESNEL" },
                contents = new string[] { "Use Max", "Multiply", "Avverage", "Z-Depth", "Fresnel", "Z-Depth + Fresnel" },
                defaultIndex = 2
            });
        }
    }
    
    
    POP_CLASS __pop_reflection_blur;
    public POP_CLASS pop_reflection_blur
    {   get
        {   return __pop_reflection_blur ?? (__pop_reflection_blur = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.REFLECTION_BLUR_1, FIELDS.REFLECTION_BLUR_2, FIELDS.REFLECTION_BLUR_3 },
                contents = new string[] { "1 Interation", "2 Interations", "3 Interations" },
                defaultIndex = 0
            });
        }
    }
    POP_CLASS __pop_reflection;
    public POP_CLASS pop_reflection
    {   get
        {   return __pop_reflection ?? (__pop_reflection = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.REFLECTION_NONE, "REFLECTION_JUST_COLOR", FIELDS.REFLECTION_2D, FIELDS.REFLECTION_USER, FIELDS.REFLECTION_PROBE, FIELDS.REFLECTION_PROBE_AND_INTENSITY, FIELDS.REFLECTION_PLANAR },
                // contents = new string[] { "none", "Simple Color", "Use flat baked 2D ", "Use user samplerCUBE", "Use UNITY Realtime/SkyCUBE or ProbesCUBE", "Use UNITY Realtime/ProbesCUBE + intensity", "Use Realtime Planar Rendering" },
                contents = new string[] { "none", "Simple Color", "Use flat baked 2D ", "Use user samplerCUBE", "Unity Probes or Sky", "Unity Probes or Sky + intensity", "Use Realtime Planar Rendering" },
                defaultIndex = 4
            });
        }
    }
    POP_CLASS __pop_reflectmaskb;
    public POP_CLASS pop_reflectmaskb
    {   get
        {   return __pop_reflectmaskb ?? (__pop_reflectmaskb = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.REFLECTION_MASK_R, FIELDS.REFLECTION_MASK_G, FIELDS.REFLECTION_MASK_B, FIELDS.REFLECTION_MASK_A },
                contents = new string[] { "MainTex (R)", "MainTex (G)", "MainTex (B)", "MainTex (A)" },
                defaultIndex = 3
            });
        }
    }
    POP_CLASS __pop_rrfastblend;
    public POP_CLASS pop_rrfastblend
    {   get
        {   return __pop_rrfastblend ?? (__pop_rrfastblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "null", "USE_LERP_BLEND", "USE_FAST_FRESNEL" },
                contents = new string[] { "Standard Blend", "Camera Angle Blend", "Fresnel" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    public class D_Reflection : IDrawable {
        override public void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 390, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            // r.x += BOTTOM_W + 10;
            
            var og = GUI.enabled;
            
            var S = 0;
            bool useBlur = false;
            
            target.DRAW_TOGGLE(r, "images_reflection", null, target.pop_reflection.VALUE != 0, out tV); r.y += 31;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Reflection", MessageType.None);
            target.pop_reflection.DrawPop(null, r); r.y += r.height;
            var use_refl = target.pop_reflection.VALUE != 0;
            GUI.enabled = use_refl;
            
            if (target.MODE_MINIMUM && target.pop_reflection.VALUE != 0)
            {   r.height = FastWaterModel20ControllerEditor.H * 12;
                EditorGUI.HelpBox(r, "Reflection not awaliable in Minimum Mode", MessageType.Info); r.y += r.height;
                
                GUI.enabled = og;
                LAST_GROUP_R = Rect.zero;
                GUI.EndGroup();
                return;
            }
            
            
            
            if (target.pop_reflection.VALUE == 0)
            {   //  EditorGUI.HelpBox(r, "None", MessageType.None);
                r.y += FastWaterModel20ControllerEditor.H;
                r.y += FastWaterModel20ControllerEditor.H;
                
                r.y += FastWaterModel20ControllerEditor.H;
                
                
            }
            else
            {
            
                bool useCreateBrobe = false;
                if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_2D)
                {   r = target.DRAW_BAKED_TEXTURE(r, FIELDS.baked_ReflectionTex, use_refl, BakeOrUpdateType.Reflection);
                    useBlur = target.MODE_ADVANCEPC;
                }
                else if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_USER)
                {   r = target.DRAW_BG_TEXTURE(r, "Cubemap Texture", FIELDS._ReflectionUserCUBE, use_refl, out tV, null, true); r.y += r.height;
                    // r = target.DRAW_SLIDER( r, "Cubemap LOD", FIELDS._ReflectionLOD, 0f, 4, use_refl ); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Y Offset", FIELDS._ReflectionYOffset, -0.4f, 0.4f, use_refl); r.y += r.height + S;
                    useBlur = true;
                }
                else if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_PROBE)
                {   // r = target.DRAW_SLIDER( r, "Cubemap LOD", FIELDS._ReflectionLOD, 0f, 4, use_refl ); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Y Offset", FIELDS._ReflectionYOffset, -0.4f, 0.4f, use_refl); r.y += r.height + S;
                    useCreateBrobe = true;
                    useBlur = true;
                }
                else if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_PROBE_AND_INTENSITY)
                {   // r = target.DRAW_SLIDER( r, "Cubemap LOD", FIELDS._ReflectionLOD, 0f, 4, use_refl ); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Y Offset", FIELDS._ReflectionYOffset, -0.4f, 0.4f, use_refl); r.y += r.height + S;
                    useCreateBrobe = true;
                    useBlur = true;
                }
                else if (target.pop_reflection.keys[target.pop_reflection.VALUE] == FIELDS.REFLECTION_PLANAR)
                {   r = target.DRAW_RESOLUTION(r, FIELDS.baked_ReflectionTex_temp + "_size"); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H * 2;
                    EditorGUI.HelpBox(r, "Obj with same Y using one pass", MessageType.None); r.y += r.height;
                    r = target.DRAW_KAYERMASK(r, "Render Layers", "_ReflectionBakeLayers", () => { }); r.y += r.height;
                    useBlur = target.MODE_ADVANCEPC;
                    useBlur = true;
                    
                    var oldValue = target.target.compiler.PlanarReflectionClipPlaneOffset;
                    var newValue = oldValue;
                    Vector2 ov;
                    r = target.DRAW_SLIDER(r, "Y Offset", null, compilerValue: ref newValue, leftValue: -10, rightValue: 10, enable: GUI.enabled, output: out ov);
                    GUI_TOOLTIP(r, "Use this to offset reflectioin camera");
                    r.y += r.height;
                    if (oldValue != newValue)
                    {   target.target.Undo();
                        target.target.compiler.PlanarReflectionClipPlaneOffset = newValue;
                        target.target.SetDirty();
                    }
                    
                    var oldValue2 = target.target.compiler.PlanarReflectionSkipEveryFrame;
                    var newValue2 = (float)oldValue2;
                    r = target.DRAW_SLIDER(r, "SkipEveryFrame", null, compilerValue: ref newValue2, leftValue: 0, rightValue: 30, enable: GUI.enabled, output: out ov, integer: true);
                    GUI_TOOLTIP(r, "Use this to improve performance by skip reflection's render in every n frame");
                    r.y += r.height;
                    if (oldValue2 != newValue2)
                    {   target.target.Undo();
                        target.target.compiler.PlanarReflectionSkipEveryFrame = (int)newValue2;
                        target.target.SetDirty();
                    }
                    
                    
                }
                else if (target.pop_reflection.keys[target.pop_reflection.VALUE] == "REFLECTION_JUST_COLOR")
                {   r = target.DRAW_COLOR(r, "Color", "_ReflectionJustColor", true, out tV, false); r.y += r.height;
                
                }
                if (useCreateBrobe)
                {   r.height = FastWaterModel20ControllerEditor.H * 2;
                    if (GUI.Button(r, "Create Probe"))
                    {   if (target.target.gameObject.GetComponentInChildren<ReflectionProbe>())
                        {   Selection.objects = new[] { (UnityEngine.Object)target.target.gameObject.GetComponentInChildren<ReflectionProbe>().gameObject };
                        }
                        else
                        {   var o = new GameObject("Reflection Probe");
                            var rp = o.AddComponent<ReflectionProbe>();
                            rp.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                            rp.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                            rp.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                            rp.resolution = 256;
                            rp.size = new Vector3(5, 5, 5);
                            rp.hdr = false;
                            rp.shadowDistance = 100;
                            rp.farClipPlane = 1000;
                            rp.cullingMask = FastWaterModel20Controller.CulMask;
                            rp.transform.position = target.target.transform.position;
                            rp.transform.SetParent(target.target.transform);
                            Undo.RegisterCreatedObjectUndo(o, "Create Reflection Probe");
                        }
                    }
                    
                    r.y += r.height;
                    
                    
                }
                if (target.MODE_ULTRA_FAST)
                {   r.y += FastWaterModel20ControllerEditor.H;
                    target.d_D_Foam.LowDistortion(ref r, target);
                    r.x = 100;
                    r.y = 0;
                }
                else
                {   r.y += FastWaterModel20ControllerEditor.H;
                }
                
                var ur = target.DRAW_TOGGLE(r, "Use Color", "COLORIZE_REFLECTION", use_refl, out tV) & use_refl; r.y += 31;
                r = target.DRAW_COLOR(r, null, "_ReflectColor", ur, out tV, false); r.y += r.height;
                
                
                r = target.DRAW_SLIDER(r, "Amount", FIELDS._ReflectionAmount, 0f, 10, use_refl); r.y += r.height + S;
                if (!target.MODE_ULTRA_FAST)
                {   var desat = target.DRAW_TOGGLE(r, "DeSaturate", "DESATURATE_REFL", use_refl, out tV) & use_refl; r.y += 31;
                    if (desat) { r = target.DRAW_SLIDER(r, null, "_ReflectionDesaturate", 0f, 1f, desat); r.y += r.height + S; }
                    
                }
                r = target.DRAW_SLIDER(r, "Distortion", FIELDS.baked_ReflectionTex_distortion, 0f, 200, use_refl); r.y += r.height + S;
                r = target.DRAW_SLIDER(r, "Low Distortion", "LOW_ReflectionTex_distortion", 0f, 100, use_refl, div: 10); r.y += r.height + S;
                
                
                if (target.MODE_ULTRA_FAST)
                {
                
                
                    r.y += FastWaterModel20ControllerEditor.H;
                    r.height = FastWaterModel20ControllerEditor.H;
                    EditorGUI.HelpBox(r, "Blend Type", MessageType.None); r.y += r.height;
                    target.pop_rrfastblend.DrawPop(null, r); r.y += r.height;
                    { r = target.DRAW_SLIDER(r, "Blend Amount", "_ReflectionBlendAmount", 0f, 1f, use_refl); r.y += r.height + S; }
                    
                    // var ff = target.DRAW_TOGGLE(r, "Use Fast Fresnel", "USE_FAST_FRESNEL", use_refl); r.y += 31;
                    {   r = target.DRAW_SLIDER(r, "Fresnel Amount", "_FastFresnelAmount", 0f, 20, target.pop_rrfastblend.VALUE == 2 && use_refl); r.y += r.height + S;
                        r = target.DRAW_SLIDER(r, "Fresnel Pow", "_FastFresnelPow", 0f, 20, target.pop_rrfastblend.VALUE == 2 && use_refl); r.y += r.height + S;
                    }
                    
                    
                    
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.DRAW_TOGGLE(r, "Debug Reflect", "REFLECTION_DEBUG_RGB", use_refl); r.y += 31;
                    
                    r.x = 300;
                    r.y = 0;
                }
                else
                {   r.y += FastWaterModel20ControllerEditor.H;
                }
                
                var bb = GUI.enabled;
                bool en_blur = true;
                if (useBlur)
                {   en_blur = target.DRAW_TOGGLE(r, "Blur", FIELDS.REFLECTION_BLUR, useBlur, out tV) & GUI.enabled & useBlur; r.y += 31;
                    GUI.enabled = en_blur;
                    
                    if (target.MODE_ADVANCEPC)
                    {   r.height = FastWaterModel20ControllerEditor.H;
                        target.pop_reflection_blur.DrawPop(null, r); r.y += r.height;
                        
                        if (target.MODE_ULTRA_FAST && target.pop_reflection_blur.VALUE > 0)
                        {   r.height = FastWaterModel20ControllerEditor.H * 6;
                            EditorGUI.HelpBox(r, "Ultra Fast Mode only 1 Interation awaliable", MessageType.Error); r.y += r.height;
                        }
                    }
                    
                }
                else
                {   // r.height = FastWaterModel20ControllerEditor.H;
                    //GUI.Label(r, "Blur");
                    // r.y += r.height;
                }
                
                if (useBlur)
                {   r = target.DRAW_SLIDER(r, "Radius", FIELDS._ReflectionBlurRadius, 0, 10, en_blur); r.y += r.height + S;
                    if (target.MODE_ADVANCEPC)
                    {
                    
                        var skip_zd = target.DRAW_TOGGLE(r, "Depth Affect", FIELDS.SKIP_REFLECTION_BLUR_ZDEPENDS, en_blur, out tV) & en_blur;
                        GUI_TOOLTIP(r, "Water Reflection will be blurred depending on the depth"); r.y += 31;
                        if (skip_zd && !target.ZEnabled())
                        {   r.height = FastWaterModel20ControllerEditor.H * 2;
                            EditorGUI.HelpBox(r, "Requests ZDepth", MessageType.Error); r.y += r.height;
                        }
                        r = target.DRAW_SLIDER(r, "Depth Offset", "_ReflectionBlurZOffset", -10, 3, skip_zd); r.y += r.height + S;
                    }
                }
                GUI.enabled = bb;
                
                
                if (!target.MODE_ULTRA_FAST)
                {   r.y += FastWaterModel20ControllerEditor.H;
                    target.d_D_Foam.LowDistortion(ref r, target);
                    r.x = 100;
                    r.y = 0;
                    
                    //////////////////////////////
                    string FEATURE = FIELDS.SKIP_REFLECTION_MASK;
                    string tile = FIELDS._ReflectionMask_Tile;
                    string amount = FIELDS._ReflectionMask_Amount;
                    string min = FIELDS._ReflectionMask_Offset;
                    string max = FIELDS._ReflectionMask_UpClamp;
                    string offset = "_ReflectionMask_TexOffsetF";
                    bool enable = use_refl;
                    POP_CLASS pop = target.pop_reflectmaskb;
                    //////////////////////////////
                    r = target.DRAW_MASK_PANEL(r, "Mask", null, FEATURE, tile, offset, amount, min, max, "REFL_MASK_DEBUG", ref enable, pop);
                    //////////////////////////////
                    ///
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.DRAW_TOGGLE(r, "Debug Reflect", "REFLECTION_DEBUG_RGB", GUI.enabled); r.y += 31;
                }
                
                
                
                if (!target.MODE_ULTRA_FAST)
                {
                
                
                    r.x = 200;
                    r.y = 0;
                    
                    
                    GUI.enabled = use_refl || target.pop_reflection.VALUE == 4 || target.pop_refraction.VALUE == 5 ||
                                  target.target.compiler.IsKeywordEnabled("RIM") ||
                                  target.target.compiler.IsKeywordEnabled("RRFRESNEL") ||
                                  target.target.compiler.IsKeywordEnabled("HAS_REFRACTION_Z_BLEND_AND_RRFRESNEL")
                                  ;
                    var usefr = target.DRAW_TOGGLE(r, "Use Fresnel", FIELDS.SKIP_FRESNEL_CALCULATION, GUI.enabled, out tV) & GUI.enabled; r.y += 31;
                    var
                    oldC = GUI.color;
                    GUI.color *= new Color32(176, 216, 255, 225);
                    /*if (pow)*/
                    { r = target.DRAW_SLIDER(r, "F Amount", "_FresnelAmount", 0.6f, 5.8f, usefr && !target.target.compiler.IsKeywordEnabled("REFLECTION_DEBUG_RGB")); r.y += r.height + S; }
                    //var grad = DRAW_TOGGLE(r, "Gradient (R)", FIELDS.SKIP_FRESNEL_GRADIENT, usefr, out tV) & usefr; r.y += 31;
                    //if (grad) { r = DRAW_GRAPHIC(r, 40, compiler.GetTexture(FIELDS._Utility) as Texture2D, use_refl, new Color(1, 0, 0, 0)); r.y += r.height; }
                    // var pow = DRAW_TOGGLE(r, "Use Pow", FIELDS.USE_FRESNEL_POW, usefr, out tV) & usefr; r.y += 31;
                    /*if (pow)*/
                    {   r = target.DRAW_SLIDER(r, "F Pow", FIELDS._FresnelPow, 0f, 100, usefr); r.y += r.height + S;
                        { r = target.DRAW_SLIDER(r, "F Fade", "_FresnelFade", 0.0f, 1, usefr); r.y += r.height + S; }
                    }
                    
                    
                    var ben = target.DRAW_TOGGLE(r, "Fresnel to Blur", "SKIP_FRES_BLUR",
                                                 usefr & target.target.compiler.IsKeywordEnabled(FIELDS.REFLECTION_BLUR)) & usefr & target.target.compiler.IsKeywordEnabled(FIELDS.REFLECTION_BLUR); r.y += 31;
                    if (ben)
                    {   //  { r = DRAW_SLIDER(r, "Amount", "", 1f, 10, usefr); r.y += r.height + S; }
                        r = target.DRAW_DOUBLEFIELDS(r, "Amount/Offset", new[] { "FRES_BLUR_AMOUNT", "FRES_BLUR_OFF" }, new[] { 0, 0.05f }, new[] { 10f, 0.95f }, usefr); r.y += r.height + S;
                    }
                    target.DRAW_TOGGLE(r, "Inverse", "FRESNEL_INVERCE", usefr, out tV); r.y += 31;
                    
                    
                    
                    
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.DRAW_TOGGLE(r, "Debug Fresnel", "DEBUG_FRESNEL", usefr); r.y += 31;
                    GUI.color = oldC;
                    
                    
                    /* var usemask = target.DRAW_TOGGLE( r, "Mask", FIELDS.SKIP_REFLECTION_MASK, use_refl, out tV ) & use_refl; r.y += 31;
                     GUI.enabled = usemask;
                     string maskTexture;
                     r.height = FastWaterModel20ControllerEditor.H * 2;
                    
                     // if (!target.target.material.IsKeywordEnabled( FIELDS.SKIP_MAINTEXTURE )) {
                     EditorGUI.HelpBox( r, "Used Main Texture", MessageType.Info ); r.y += r.height;
                     maskTexture = FIELDS._MainTex;
                    
                     r.height = FastWaterModel20ControllerEditor.H;
                     target.pop_reflectmaskb.DrawPop( null, r ); r.y += r.height;
                     r = DRAW_GRAPHIC( r, 40, target.target.material.GetTexture( maskTexture ) as Texture2D, usemask, GET_COLOR( target.pop_reflectmaskb.VALUE ) ); r.y += r.height;
                     r = target.DRAW_SLIDER( r, "Tile XY", FIELDS._ReflectionMask_Tile, 0, 2, usemask ); r.y += r.height + S;
                     r = target.DRAW_SLIDER( r, "Mask Amount", FIELDS._ReflectionMask_Amount, 1, 5, usemask ); r.y += r.height + S;
                     r = target.DRAW_DOUBLEFIELDS( r, "Min/Max", new[] { FIELDS._ReflectionMask_Offset, FIELDS._ReflectionMask_UpClamp }, new[] { 0, 0.5f }, new[] { 2f, 10 }, usemask ); r.y += r.height + S;
                     //r = target.DRAW_SLIDER( r, "Mask Clamp", FIELDS._ReflectionMask_UpClamp, 0.5f, 10, usemask ); r.y += r.height + S;*/
                    
                    
                    r.x = 300;
                    r.y = 0;
                    
                    target.DRAW_RRBLEND(r);
                }
                
                
                
                
            }
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
        
        
    }
    
    
}
}
#endif
