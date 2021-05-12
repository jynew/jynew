
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {



    POP_CLASS __pop_depth;
    public POP_CLASS pop_depth
    {   get
        {   return __pop_depth ?? (__pop_depth = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.DEPTH_NONE, FIELDS.BAKED_DEPTH_EXTERNAL_TEXTURE, FIELDS.BAKED_DEPTH_VIASCRIPT, FIELDS.BAKED_DEPTH_ONAWAKE/*, FIELDS.NOT_ALLOW_MANUAL_DEPTH*/, FIELDS.ALLOW_MANUAL_DEPTH },
                contents = new string[] { "none", "Bake Texture in Editor", "Bake Runtime Via Script", "Bake Runtime On Awake"/*, "Realtime (Skip if platform not supported auto ZDepth)"*/, "Camera Render Realtime" },
                defaultIndex = 4
            });
        }
    }
    
    POP_FLOAT_CLASS __pop_float_horequality;
    public POP_FLOAT_CLASS pop_float_horequality
    {   get
        {   return __pop_float_horequality ?? (__pop_float_horequality = new POP_FLOAT_CLASS()
            {   target = this,
                    keys = "_ShoreWavesQual",
                    contents = new string[] { "Low", "Medium", "Hi", "Ultra" },
                defaultIndex = 1
            });
        }
    }
    
    
    POP_CLASS __pop_costalpha;
    public POP_CLASS pop_costalpha
    {   get
        {   return __pop_costalpha ?? (__pop_costalpha = new POP_CLASS()
            {   target = this,
                    keys = new[] { "SKIP_FOAM_COAST_ALPHA", "SKIP_FOAM_COAST_ALPHA_FAKE", "FOAM_COAST_ALPHA_V2" },
                contents = new string[] { "none", "OneSide", "DoubleSide" },
                defaultIndex = 1
            });
        }
    }
    
    POP_CLASS __pop_blendfoam;
    public POP_CLASS pop_blendfoam
    {   get
        {   return __pop_blendfoam ?? (__pop_blendfoam = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "FOAM_BLEND_LUM" },
                contents = new string[] { "Additive", "Alpha" },
                defaultIndex = 1
            });
        }
    }
    
    
    POP_CLASS __pop_grad;
    public POP_CLASS pop_grad
    {   get
        {   return __pop_grad ?? (__pop_grad = new POP_CLASS()
            {   target = this,
                    keys = new[] { "GRAD_G", "GRAD_B" },
                contents = new string[] { "Grad (G)", "Grad (B)" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    POP_CLASS __pop_shoreunwrap;
    public POP_CLASS pop_shoreunwrap
    {   get
        {   return __pop_shoreunwrap ?? (__pop_shoreunwrap = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "SHORE_UNWRAP_STRETCH_1" },
                contents = new string[] { "Fill", "Stretch" },
                defaultIndex = 0
            });
        }
    }
    
    
    POP_CLASS __pop_shorexanim;
    public POP_CLASS pop_shorexanim
    {   get
        {   return __pop_shorexanim ?? (__pop_shorexanim = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "SHORE_ANIM_XMOVEBLEND_1" },
                contents = new string[] { "none", "Blend 2X Textures" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_unrapmove;
    public POP_CLASS pop_unrapmove
    {   get
        {   return __pop_unrapmove ?? (__pop_unrapmove = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "SHORE_UNWRAP_STRETCH_DOT", "SHORE_UNWRAP_STRETCH_OFFSETDOT" },
                contents = new string[] { "Simple Unwrap", "Dot Unwrap", "Offset Dot Unwrap" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    // add channel
    POP_CLASS __pop_add_texturetype;
    public POP_CLASS pop_add_texturetype
    {   get
        {   return __pop_add_texturetype ?? (__pop_add_texturetype = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "SHORE_USE_ADDITIONAL_GRAD_TEXTURE" },
                contents = new string[] { "Main Texture", "Shore Texture" },
                defaultIndex = 0
            });
        }
    }
    POP_CLASS __pop_add_texturechannel;
    public POP_CLASS pop_add_texturechannel
    {   get
        {   return __pop_add_texturechannel ?? (__pop_add_texturechannel = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "SHORE_USE_ADDITIONAL_GRAD_TEXTURE_G", "SHORE_USE_ADDITIONAL_GRAD_TEXTURE_B" },
                contents = new string[] { "R", "G", "B" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    POP_CLASS __pop_shoreTExtureSwitcher;
    public POP_CLASS pop_shoreTExtureSwitcher
    {   get
        {   return __pop_shoreTExtureSwitcher ?? (__pop_shoreTExtureSwitcher = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "_ShoreWaves_SECOND_TEXTURE", "_ShoreWaves_USE_MAINTEX" },
                contents = new[] { "Own Texture", "Second Texture", "Main Texture" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_shoreTextureChannel;
    public POP_CLASS pop_shoreTextureChannel
    {   get
        {   return __pop_shoreTextureChannel ?? (__pop_shoreTextureChannel = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "_ShoreWaves_B", "_ShoreWaves_R", "_ShoreWaves_A" },
                contents = new[] {  "G", "B", "R", "A" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    
    
    
    
    public static bool LastHaveMask_ZDepth;
    
    public class D_Utils : IDrawable {
        public override void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, input.width, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var og = GUI.enabled;
            
            
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "ZDepth", MessageType.None); r.y += r.height;
            target.DRAW_TOGGLE(r, "images_zdepth", null, true, out tV); r.y += 31;
            
            var oldV2 = target.pop_depth.VALUE;
            target.pop_depth.DrawPop(null, r); r.y += r.height;
            
            if (target.target.compiler.material != null)
            {
            
            
                var wasChanged = target.pop_depth.VALUE != oldV2;
                if (target.pop_depth.VALUE == 0)
                {
                }
                /* else if (target.IS_OPAQUE && target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.ALLOW_MANUAL_DEPTH)
                 {
                     r.height = FastWaterModel20ControllerEditor.H * 7;
                     EditorGUI.HelpBox(r, "Realtime z depth not working with opaque shader", MessageType.Error); r.y += r.height;
                
                
                     // target.DRAW_TOGGLE( r, "Fine Calc Y Height", "SKIP_CALCULATE_HEIGHT_DEPTH", true );
                 }*/
                else
                {   /* r = target.DRAW_BG_TEXTURE( r, null, FIELDS._ReflectionTex, true, out tV, null ); r.y += r.height;
                     r.height = FastWaterModel20ControllerEditor.H;
                     if (GUI.Button( r, "Bake ZDepth" )) {
                       //todo ZDEPTH BAKE
                     }*/
                    
                    bool haveMask = true;
                    // bool use
                    //**
                    if (target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.BAKED_DEPTH_EXTERNAL_TEXTURE)
                    {   target.target.compiler.SetTexture(FIELDS.baked_BakedData, target.target.backed_zdepth);
                        r = target.DRAW_BAKED_TEXTURE(r, FIELDS.baked_BakedData, true, BakeOrUpdateType.ZDepth);
                        if (target.target.backed_zdepth != target.target.compiler.GetTexture(FIELDS.baked_BakedData))
                        {   target.target.Undo();
                            target.target.backed_zdepth = target.target.compiler.GetTexture(FIELDS.baked_BakedData) as Texture2D;
                            target.target.SetDirty();
                        }
                        
                        r.height = FastWaterModel20ControllerEditor.H * 4;
                        EditorGUI.HelpBox(r, "Different objects use different textures", MessageType.None); r.y += r.height;
                        r.height = FastWaterModel20ControllerEditor.H * 3;
                        //  EditorGUI.HelpBox(r, "(RGB) - ZDepth(A) - Flow Directions 0-up 0.25-right", MessageType.None); r.y += r.height;
                        // haveMask = false;
                    }
                    else if (target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.BAKED_DEPTH_VIASCRIPT)
                    {   r = target.DRAW_RESOLUTION(r, FIELDS.baked_BakedData_temp_size); r.y += r.height;
                        r.height = FastWaterModel20ControllerEditor.H * 7;
                        EditorGUI.HelpBox(r, "Use BakeOrUpdateTexture( BakeOrUpdateType.ZDepth );", MessageType.None); r.y += r.height;
                    }
                    else if (target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.BAKED_DEPTH_ONAWAKE)
                    {   bool rc;
                        r = target.DRAW_RESOLUTION(r, FIELDS.baked_BakedData_temp_size, out rc); r.y += r.height;
                        if (wasChanged || rc) target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.ZDepth, target.LastResolution);
                        r.height = FastWaterModel20ControllerEditor.H * 3;
                        EditorGUI.HelpBox(r, "Texture will baking OnEnable once", MessageType.None); r.y += r.height;
                    }
                    else /*if (target.pop_depth.keys[target.pop_depth.VALUE] == FIELDS.ALLOW_MANUAL_DEPTH) */
                    {   r.height = FastWaterModel20ControllerEditor.H * 3;
                        EditorGUI.HelpBox(r, "Material will use Realtime ZDepth", MessageType.None); r.y += r.height;
                        haveMask = false;
                        
                        var zcalcenable = target.DRAW_TOGGLE(r, "Distance Z Calc", "ZDEPTH_CALCANGLE", true);
                        GUI_TOOLTIP(r,
                                    "Use this, If you are face with z offset when the camera is near water. This function uses the distance calculation (But Ultra Fast Mode use another hack, howerver it's 3 additioinal lines also)");
                        r.y += 31;
                        if (zcalcenable)
                        {   r = target.DRAW_SLIDER(r, null, "_ZDistanceCalcOffset", 0.2f, 0.7f, GUI.enabled); r.y += r.height;
                        }
                    }
                    //**
                    
                    r = target.DRAW_SLIDER(r, "Shore Offset", "_Z_BLACK_OFFSET_V", -0.5f, 0.5f, GUI.enabled); r.y += r.height;
                    
                    LastHaveMask_ZDepth = haveMask;
                    
                    if (haveMask)
                    {   r = target.DRAW_KAYERMASK(r, "Render Layers", "_ZDepthBakeLayers", () => target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.ZDepth, target.LastResolution)); r.y += r.height;
                    
                    
                        var zvolcalc = target.DRAW_TOGGLE(r, "Volume Calc Z", "SKIP_Z_CALC_DEPTH_FACTOR", GUI.enabled && target.ZEnabled(), out tV) & GUI.enabled && target.ZEnabled();
                        GUI_TOOLTIP(r, "Additional calculations to give the volume to zdepth texture");
                        r.y += 31;
                        if (zvolcalc)
                            r = target.DRAW_SLIDER(r, "Volume Factor", "_RefrDeepFactor", 16, 4096, GUI.enabled, div: 0.01f); r.y += r.height;
                            
                    }
                    //_RefractionBakeLayers
                    
                    if (!target.MODE_ULTRA_FAST)
                    {   r = target.DRAW_SLIDER(r, "Distor ZDepth", "_RefrDistortionZ", 0, 600, GUI.enabled); r.y += r.height;
                    }
                    else
                    {   r = target.DRAW_SLIDER_WITHTOGGLE(r, "Distor ZDepth", "_RefrDistortionZ", 0, 600, GUI.enabled, "USE_ZD_DISTOR"); r.y += r.height;
                    
                        target.  DRAW_TOGGLE(r, "images_zfix_a", null, target.target.compiler.IsKeywordEnabled("USE_ZD_DISTOR"), out tV); r.y += 31;
                        //target.DRAW_TOGGLE(r, "images_zfix_b", null, true, out tV); r.y += 31;
                        target.DRAW_TOGGLE(r, "Fix Distortion", "FIX_DISTORTION", target.target.compiler.IsKeywordEnabled("USE_ZD_DISTOR")); r.y += 31;
                        
                        
                    }
                    
                    target.DRAW_TOGGLE(r, "Debug ZDepth", "DEGUB_Z", true); r.y += 31;
                    r.y += 10;
                    
                    if (!target.MODE_ULTRA_FAST)
                    {   if (target.pop_depth.VALUE != 0)
                        {   r = target.DRAW_IMMRESED_WATER(r);
                        }
                    }
                    
                }
                
                
                
                
                GUI.enabled = true;
                r.y += FastWaterModel20ControllerEditor.H;
                
                if (target.MODE_ULTRA_FAST)
                {   var oc = GUI.color;
                    GUI.color *= new Color32(234, 255, 235, 255);
                    bool hasAddPass = false;
                    target.d_D_Foam.target = target;
                    hasAddPass |= target.d_D_Foam.DoShoreHasAddPass(1);
                    hasAddPass |= target.d_D_Foam.DoShoreHasAddPass(2);
                    target.d_D_Foam.ShoreWavesCommon(ref r, out hasAddPass, hasAddPass);
                    GUI.color = oc;
                    
                    
                    
                }
                r.y += FastWaterModel20ControllerEditor.H;
                target.d_D_Foam.target = target;
                target.d_D_Foam.LowDistortion(ref r, target);
                
                
                // target.DRAW_TOGGLE( r, "images_reflection", FIELDS._RefractionTex, true, out tV ); r.y += 31;
                
                /*r = target.DRAW_BG_TEXTURE( r, "Baked Texture", FIELDS._BackedData, true, out tV, null ); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H * 6;
                EditorGUI.HelpBox( r, "(R) - ZDepth\n(G) - Flow Directions", MessageType.None ); r.y += r.height;*/
                
                
                
                
            }
            
            
            
            
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
    }
    
    
    
    
    
    
    
    
    
    
    public class D_Foam : IDrawable {
        public override void Draw(Rect input, out Vector2[] output)
        {   if (target.MODE_ULTRA_FAST) Draw_UltraFast(input, out output);
            else Draw_Normal(input, out output);
        }
        
        
        
        void Draw_UltraFast(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 300, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var og = GUI.enabled;
            
            
            
            
            
            
            
            r.y = 0;
            r.x = 0;
            
            
            
            
            /* r.y = 0;
             r.x = 100;*/
            
            GUI.enabled = true;
            DrawShore(ref r, 1);
            
            
            r.y = 0;
            r.x = 200;
            
            
            /*GUI.enabled = true;
            hasAddPass |= DrawShore(ref r, 2);*/
            
            
            
            
            
            
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
            
        }
        
        
        public bool DoShoreHasAddPass(int index)
        {   var waves_enable = target.target.compiler.IsKeywordEnabled("UFAST_SHORE_" + index);
            var hasAddPass = !target.target.compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH_" + index);
            return hasAddPass && waves_enable;
        }
        
        bool DrawShore(ref Rect r, int index)
        {
        
        
#pragma warning disable
            string _ShoreWavesGrad = "_ShoreWavesGrad";
            // string _UFSHORE_Texture_1 = "_UFSHORE_Texture_" + index;
            
            string _UFSHORE_Amount_1 = "_UFSHORE_Amount_" + index;
            string _UFSHORE_Color_1 = "_UFSHORE_Color_" + index;
            string _UFSHORE_Length_1 = "_UFSHORE_Length_" + index;
            string _UFSHORE_Tile_1 = "_UFSHORE_Tile_" + index;
            string _UFSHORE_Distortion_1 = "_UFSHORE_Distortion_" + index;
            
            string _UFSHORE_Speed1 = "_UFSHORE_Speed_" + index;
            string _UFSHORE_ShadowV1_1 = "_UFSHORE_ShadowV1_" + index;
            string _UFSHORE_ShadowV2_1 = "_UFSHORE_ShadowV2_" + index;
            //string NEED_SHORE_WAVES_UNPACK = "NEED_SHORE_WAVES_UNPACK";
            //string _FoamLShoreWavesTileY_SW = "_FoamLShoreWavesTileY_SW";
            //string _FoamDistortionFade_SW = "_FoamDistortionFade_SW";
            string _UFSHORE_AlphaAmount_1 = "_UFSHORE_AlphaAmount_" + index;
            string _UFSHORE_AlphaMax_1 = "_UFSHORE_AlphaMax_" + index;
            //string _FoamTextureTiling_SW = "_FoamTextureTiling_SW";
            //string _FoamWavesSpeed_SW = "_FoamWavesSpeed_SW";
            //string _FoamDirection_SW = "_FoamDirection_SW";
            //string _FoamTexture_SW = "_FoamTexture_SW";
#pragma warning restore
            
            target.DRAW_TOGGLE(r, "images_shorefoam", null, true, out tV); r.y += 31;
            
            var waves_enable = target.DRAW_TOGGLE(r, "Use Shore " + index, "UFAST_SHORE_" + index, true, out tV); r.y += 31;
            var og = GUI.enabled;
            GUI.enabled = waves_enable;
            var oc = GUI.color;
            GUI.color *= new Color32(234, 255, 235, 255);
            var hasAddPass = target.DRAW_TOGGLE(r, "Second Z Pass", "SKIP_SECOND_DEPTH_" + index, waves_enable, out tV) && waves_enable; r.y += 31;
            GUI.color = oc;
            // target.DRAW_TOGGLE(r, "Debug Z Pass " + index, "DEGUB_Z_SHORE_" + index, waves_enable); r.y += 31;
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            //   r = target.DRAW_SLIDER(r, "Length", _UFSHORE_Length_1, 0.001, hasAddPass ? 1 : 50, waves_enable); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Length", _UFSHORE_Length_1, 0.000001f, hasAddPass ? 1 : 50, waves_enable); r.y += r.height;
            r = target.DRAW_COLOR(r, "Color", _UFSHORE_Color_1, waves_enable, out tV, false); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", _UFSHORE_Amount_1, 0, 1000f, waves_enable); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Normal Distor", _UFSHORE_Distortion_1, 0, 1000f, waves_enable);
            GUI_TOOLTIP(r, @"- depends on the normals'"); r.y += r.height;
            // r = target.DRAW_SLIDER(r, "Vertex Y Distortion", , 0, 100f, waves_enable); r.y += r.height;
            r = target.DRAW_SLIDER_WITHTOGGLE(r, "Vertex Distor", "VERTEX_H_DISTORT", 0, 10, waves_enable, "USE_VERTEX_H_DISTORT", div: 10);
            GUI_TOOLTIP(r, @"- depends on the y vertices pos'"); r.y += r.height;
            
            
            //  r.y += FastWaterModel20ControllerEditor.H;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Shore Alpha", MessageType.None); r.y += r.height;
            r = target.DRAW_DOUBLEFIELDS(r, "Grow/Shrink", new[] { _UFSHORE_AlphaAmount_1, _UFSHORE_AlphaMax_1 }, new[] { 0.01f, 0f }, new[] { 1, 1f }, waves_enable); r.y += r.height;
            //r = target.DRAW_SLIDER(r, "Fade Offset", "_FoamMaskOffset_SW", 0, 1f, waves_enable); r.y += r.height + S;
            
            var vol = !target.target.compiler.IsKeywordEnabled("SKIP_REFRACTION_CALC_DEPTH_FACTOR");
            
            
            target.DRAW_TOGGLE(r, "Flat Shore Alpha", "FOAM_ALPHA_FLAT", waves_enable && vol && LastHaveMask_ZDepth, out tV);
            GUI_TOOLTIP(r, @"- You can turn this on if you use a 'Baked Refraction Texture'.
- If the option is turned on, next to the shore in the places where the waves began to splash, the shore 'Baked Texture' will be flat, if you use 'Volume Calc For Refraction', this is necessary to avoid a visual gap between the waves and the virtual depth");
            r.y += 31;
            target.DRAW_TOGGLE(r, "Alpha as Transparent", "APPLY_FOAM_OUTALPHA_" + index, waves_enable && !target.target.compiler.IS_OPAQUE, out tV); r.y += 31;
            
            r.y += FastWaterModel20ControllerEditor.H;
            var ushh = target.DRAW_TOGGLE(r, "Drop Shadows", "SHORE_SHADDOW_" + index, waves_enable) & waves_enable; r.y += 31;
            r = target.DRAW_SLIDER(r, "Color Offset", _UFSHORE_ShadowV1_1, 0, 100f, ushh); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Alpha Offset", _UFSHORE_ShadowV2_1, -100, 100f, ushh); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            //  EditorGUI.HelpBox(r, "Shore Waves", MessageType.None);
            r.y += r.height;
            
            r.y = 0;
            r.x = 100;
            
            r.y += 31;
            r.y += 31;
            r.y += 31;
            r.y += FastWaterModel20ControllerEditor.H;
            
            var use_tex = target.DRAW_TOGGLE(r, "Use Texture", "SKIP_UNWRAP_TEXTURE", waves_enable) & waves_enable; r.y += 31;
            // r = target.DRAW_BG_TEXTURE(r, null, _ShoreWavesGrad, waves_enable, out tV, null); r.y += r.height;
            GUI.enabled = use_tex;
            
            string[] texture = new[] { "_ShoreWavesGrad", "_UF_NMASK_Texture", FIELDS._MainTex} ;
            POP_CLASS texture_SWITCHER = target.pop_shoreTExtureSwitcher;
            POP_CLASS pop_channel = target.pop_shoreTextureChannel;
            bool enable = GUI.enabled;
            //////////////////////////////
            r =  target. DrawTextureWIthChannelAndSwitcher(r, use_tex, texture, texture_SWITCHER, pop_channel);
            
            
            GUI.enabled = use_tex;
            r = target.DRAW_SLIDER(r, "Transparency", "_UFSHORE_UNWRAP_Transparency", -1, 1f, use_tex); r.y += r.height;
            r = target.DRAW_SLIDER_WITHTOGGLE(r, "Low Distortion", "_UFSHORE_UNWRAP_LowDistortion_1", 0, 10, use_tex, "_UFSHORE_UNWRAP_Low", div: 10); r.y += r.height;
            
            if (!target.MODE_MINIMUM)
            {   r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Unwrap", MessageType.None); r.y += r.height;
                target.pop_shoreunwrap.DrawPop(null, r); r.y += r.height;
                
                if (target.__pop_shoreunwrap.VALUE == 1)
                {   EditorGUI.HelpBox(r, "Mode", MessageType.None); r.y += r.height;
                    target.pop_unrapmove.DrawPop(null, r); r.y += r.height;
                    
                    //  r = target.DRAW_SLIDER(r, "Waves Tile", _UFSHORE_Tile_1, 0.00001f, 2000, waves_enable); r.y += r.height;
                    r = target.DRAW_VECTOR(r, "Waves Tile", _UFSHORE_Tile_1, 0.00001f, 2000, use_tex, false, 1); r.y += r.height;
                }
                else
                {   r = target.DRAW_VECTOR(r, "Waves Tile", _UFSHORE_Tile_1, 0.00001f, 2000, use_tex); r.y += r.height;
                }
                
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "X Animation", MessageType.None); r.y += r.height;
                target.pop_shorexanim.DrawPop(null, r); r.y += r.height;
                if (target.pop_shorexanim.VALUE == 1)
                {   r = target.DRAW_SLIDER(r, "Speed", _UFSHORE_Speed1, 0, 100f, use_tex); r.y += r.height;
                }
            }
            else
            {   r = target.DRAW_VECTOR(r, "Waves Tile", _UFSHORE_Tile_1, 0.00001f, 2000, use_tex); r.y += r.height;
            
            }
            
            // target.DRAW_TOGGLE(r, "Y Sin Wiggle", "SHORE_ANIM_YSIN_" + index, use_tex); r.y += 31;
            GUI.enabled = waves_enable;
            
            r.y += FastWaterModel20ControllerEditor.H;
            var usewaves_Gradient = target.DRAW_TOGGLE(r, "Waves Gradient", "SHORE_USE_WAVES_GRADIENT_" + index, waves_enable) & waves_enable; r.y += 31;
            GUI.enabled = usewaves_Gradient;
            r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._Utility) as Texture2D, usewaves_Gradient, null); r.y += r.height;
            r = target.DRAW_DOUBLEFIELDS(r, "Waves Tile", new[] { "MAIN_TEX_FoamGradTile_1", "MAIN_TEX_FoamGradTileYYY_1" }
                                         , new[] { 0.0001f, 0f }, new[] { 100, 100f }, usewaves_Gradient, firstDiv: 10, seconddiv: 10); r.y += r.height;
            // r = target.DRAW_SLIDER(r, "Waves Tile", "MAIN_TEX_FoamGradTile_1", 0, 100f, usewaves_Gradient, div: 10); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Speed", "MAIN_TEX_FoamGradWavesSpeed_1", 0, 100f, usewaves_Gradient, div: 100); r.y += r.height;
            r = target.DRAW_SLIDER_WITHTOGGLE(r, "Low Distortion", "_UFSHORE_LowDistortion_" + index, 0, 10, usewaves_Gradient, "SHORE_USE_LOW_DISTORTION_" + index); r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Direction", MessageType.None); r.y += r.height;
            r = target.DIRECTION(r, "MAIN_TEX_FoamGradDirection_1", usewaves_Gradient, VECTOR: false, leftClamp: -1f, rightClamp: 1f); r.y += r.height;
            // target.DRAW_TOGGLE(r, "Global Direction", "SHORE_USE_GLOB_DIR_" + index, usewaves_Gradient, out tV) ; r.y += 31;
            GUI.enabled = waves_enable;
            
            
            r.y = 0;
            r.x = 200;
            
            r.y += 31;
            r.y += 31;
            r.y += 31;
            r.y += FastWaterModel20ControllerEditor.H;
            
            
            if (!target.MODE_MINIMUM)
            {
            
                var useac = target.DRAW_TOGGLE(r, "Additional Contour", "SHORE_USE_ADDITIONALCONTUR_" + index, waves_enable, out tV) & waves_enable; r.y += 31;
                GUI.enabled = useac;
                r = target.DRAW_SLIDER(r, "Length", "_UFSHORE_ADDITIONAL_Length_1", 0.00001f, 100f, useac); r.y += r.height;
                target.DRAW_TOGGLE(r, "Inverse Length", "SHORE_ADDITIONALCONTUR_INVERSE", useac, out tV); r.y += 31;
                //target.DRAW_TOGGLE(r, "Separate Pass", "SHORE_USE_ADDITIONALCONTUR_SEPARATE", useac, out tV) ; r.y += 31;
                var lp = target.DRAW_TOGGLE(r, "Length Pow", "SHORE_USE_ADDITIONALCONTUR_POW_1", useac, out tV) & useac; r.y += 31;
                if (lp) { r = target.DRAW_SLIDER(r, null, "SHORE_USE_ADDITIONALCONTUR_POW_Amount_1", 0, 100f, useac); r.y += r.height; }
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Texture", MessageType.None); r.y += r.height;
                target.pop_add_texturetype.DrawPop(null, r); r.y += r.height;
                r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(target.pop_add_texturetype.VALUE == 0 ? FIELDS._MainTex : _ShoreWavesGrad) as Texture2D, useac, null); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Channel", MessageType.None); r.y += r.height;
                target.pop_add_texturechannel.DrawPop(null, r); r.y += r.height;
                
                //r = target.DRAW_VECTOR(r, "Tex Tile", "_UFSHORE_ADD_Tile_1", 0.00001f, 100, useac, div: 10); r.y += r.height;
                r = target.DRAW_SLIDER(r, "Tex Tile", "_UFSHORE_ADD_Tile_1", 0, 100f, useac, div: 10); r.y += r.height;
                // r = target.DRAW_COLOR(r, "Color", "_UFSHORE_ADD_Color_1", useac, out tV, false); r.y += r.height;
                r = target.DRAW_SLIDER(r, "Amount", "_UFSHORE_ADD_Amount_1", 0, 100f, useac); r.y += r.height;
                target.DRAW_TOGGLE(r, "No Coast Line", "SKIP_SHORE_ADDITIONALCONTUR_USE_Z", useac, out tV) ; r.y += 31;
                
                r = target.DRAW_SLIDER(r, "Distortion", "_UFSHORE_ADD_Distortion_1", 0, 100f, useac); r.y += r.height;
                r = target.DRAW_SLIDER(r, "Low Distortion", "_UFSHORE_ADD_LowDistortion_1", 0, 10f, useac); r.y += r.height;
                
            }
            
            
            //  r = target.DRAW_DOUBLEFIELDS(r, "Waves Tile", new[] { _FoamLShoreWavesTileX_SW, _FoamLShoreWavesTileY_SW }, new[] { 0.00001f, 0.00001f }, new[] { 25, 25f }, waves_enable, firstDiv: 10, seconddiv: 10); r.y += r.height;
            // r = target.DRAW_DOUBLEFIELDS(r, "Amount/Speed", new[] { _FoamAmount_SW, _FoamWavesSpeed_SW }, new[] { 0f, 0f }, new[] { 100, 10f }, waves_enable); r.y += r.height + S;
            
            // if (target.target.compiler.GetFloat(_FoamWavesSpeed_SW) == 0) { r = target.DRAW_SLIDER(r, "Offset", "_FoamOffset", 0, 1f, waves_enable); r.y += r.height + S; }
            
            GUI.enabled = og;
            
            
            
            return hasAddPass;
        }
        
        
        
        
        
        
        
        
        
        
        
        void Draw_Normal(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 300, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var og = GUI.enabled;
            
            
            
            
            var S = 0;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Shore Foam", MessageType.None); r.y += r.height;
            var foam_enable = target.DRAW_TOGGLE(r, "images_shorefoam", FIELDS.SKIP_FOAM, true, out tV); r.y += 31;
            var oc = GUI.color;
            GUI.color *= new Color32(214, 241, 255, 255);
            //  target.DRAW_TOGGLE(r, "images_surfacefoam", null, true, out tV); r.y += 31;
            GUI.enabled = foam_enable;
            
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_grad.DrawPop(null, r); r.y += r.height;
            r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._Utility) as Texture2D, GUI.enabled, target.pop_grad.VALUE == 0 ? new Color(0, 1, 0, 0) : new Color(0, 0, 1, 0)); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H * 2;
            EditorGUI.HelpBox(r, "Transparent Works", MessageType.None);
            GUI_TOOLTIP(r, "Adds transparent areas to the wave"); r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_costalpha.DrawPop(null, r);
            GUI_TOOLTIP(r, "Adds transparent areas to the wave"); r.y += r.height;
            if (target.pop_costalpha.VALUE == 2)
            {   r = target.DRAW_SLIDER(r, "Alpha Offset", "_FoamAlpha2Amount", -0.001f, 1f, true); r.y += r.height + S;
            }
            
            
            
            
            
            
            //target.DRAW_TOGGLE( r, "Transparent Works", FIELDS.SKIP_FOAM_COAST_ALPHA, foam_enable, out tV ); r.y += 31;
            
            r = target.DRAW_COLOR(r, "Color", FIELDS._FoamColor, foam_enable, out tV, false); r.y += r.height + S;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Blend", MessageType.None); r.y += r.height;
            target.pop_blendfoam.DrawPop(null, r); r.y += r.height;
            if (target.pop_blendfoam.VALUE == 1) { r = target.DRAW_SLIDER(r, null, "_FoamBlendOffset", 0f, 1, true); r.y += r.height + S; }
            
            r = target.DRAW_DOUBLEFIELDS(r, "Amount/Speed", new[] { FIELDS._FoamAmount, FIELDS._FoamWavesSpeed }, new[] { 0f, 0f }, new[] { 30, 2f }, foam_enable, seconddiv: 100); r.y += r.height + S;
            
            if (target.target.compiler.GetFloat(FIELDS._FoamWavesSpeed) == 0) { r = target.DRAW_SLIDER(r, "Offset", "_FoamOffset", 0, 1f, foam_enable); r.y += r.height + S; }
            r = target.DRAW_DOUBLEFIELDS(r, "Length/Fade", new[] { FIELDS._FoamLength, FIELDS._WaterfrontFade }, new[] { 0.01f, 0f }, new[] { 50, 30f }, foam_enable); r.y += r.height + S;
            /* r = target.DRAW_SLIDER( r, "Amount", FIELDS._FoamAmount, 0, 10f, foam_enable ); r.y += r.height + S;
             r = target.DRAW_SLIDER( r, "Length", FIELDS._FoamLength, 0, 50f, foam_enable ); r.y += r.height + S;*/
            //  r = target.DRAW_SLIDER( r, "Waves Speed", FIELDS._FoamWavesSpeed, 0, 2f, foam_enable ); r.y += r.height + S;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Direction", MessageType.None); r.y += r.height;
            r = target.DIRECTION(r, "_FoamDirection", foam_enable, VECTOR: false, leftClamp: -1f, rightClamp: 1f); r.y += r.height;
            r = target.DRAW_DOUBLEFIELDS(r, "Distortion/Fade", new[] { FIELDS._FoamDistortion, "_FoamDistortionFade" }, new[] { 0f, 0f }, new[] { 10, 1f }, foam_enable);
            GUI_TOOLTIP(r, "Distortion is distortion, Fade means fading of the distortion near the shore"); r.y += r.height + S;
            
            
            var usetexture = target.DRAW_TOGGLE(r, "Color Texture", FIELDS.NEED_FOAM_UNPACK, foam_enable, out tV) & foam_enable; r.y += 31;
            r = target.DRAW_BG_TEXTURE(r, null, FIELDS._FoamTexture, usetexture, out tV, null); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Tile X / Y", FIELDS._FoamTextureTile, 0.0001f, 10f, usetexture); r.y += r.height + S;
            r = target.DRAW_SLIDER(r, "Distortion", "_FoamDistortionTexture", 0, 10f, usetexture); r.y += r.height + S;
            r = target.DRAW_SLIDER(r, "Sin +- Moving", FIELDS._FoamOffsetSpeed, 0, 10f, usetexture); r.y += r.height + S;
            //r = target.DRAW_DOUBLEFIELDS( r, "Tile/Distortion", new[] { FIELDS._FoamTextureTile, FIELDS._FoamDistortion }, new[] { 0.001f, 0f }, new[] { 10, 10f }, usetexture ); r.y += r.height + S;
            
            
            
            
            GUI.color = oc;
            
            
            
            
            r.y = 0;
            r.x = 100;
            
            
            
            
            
            
            string NEED_SHORE_WAVES_UNPACK = "NEED_SHORE_WAVES_UNPACK";
            string _FoamLength_SW = "_FoamLength_SW";
            string _FoamLShoreWavesTileX_SW = "_FoamLShoreWavesTileX_SW";
            string _FoamLShoreWavesTileY_SW = "_FoamLShoreWavesTileY_SW";
            string _FoamDistortionFade_SW = "_FoamDistortionFade_SW";
            string _WaterfrontFade_SW = "_WaterfrontFade_SW";
            string _FoamDistortion_SW = "_FoamDistortion_SW";
            string _FoamTextureTiling_SW = "_FoamTextureTiling_SW";
            string _FoamColor_SW = "_FoamColor_SW";
            string _FoamWavesSpeed_SW = "_FoamWavesSpeed_SW";
            string _FoamDirection_SW = "_FoamDirection_SW";
            string _FoamAmount_SW = "_FoamAmount_SW";
            string _ShoreWavesGrad = "_ShoreWavesGrad";
            string _FoamTexture_SW = "_FoamTexture_SW";
            
            
            oc = GUI.color;
            GUI.color *= new Color32(234, 255, 235, 255);
            bool hasAddPass;
            var waves_enable = ShoreWavesCommon(ref r, out hasAddPass);
            GUI.enabled = waves_enable;
            
            r.y = 0;
            r.x = 200;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Shore Waves", MessageType.None); r.y += r.height;
            
            r = target.DRAW_BG_TEXTURE(r, null, _ShoreWavesGrad, waves_enable, out tV, null); r.y += r.height;
            target.DRAW_TOGGLE(r, "Use Pow Appear", "USE_SHORE_POW_APPEAR", waves_enable, out tV); r.y += 31;
            r = target.DRAW_COLOR(r, "Color", _FoamColor_SW, waves_enable, out tV, false); r.y += r.height + S;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Blend", MessageType.None); r.y += r.height;
            target.pop_blendfoam.DrawPop(null, r); r.y += r.height;
            if (target.pop_blendfoam.VALUE == 1) { r = target.DRAW_SLIDER(r, null, "_FoamBlendOffset", 0f, 1, true); r.y += r.height + S; }
            
            r = target.DRAW_DOUBLEFIELDS(r, "Amount/Speed", new[] { _FoamAmount_SW, _FoamWavesSpeed_SW }, new[] { 0f, 0f }, new[] { 100, 10f }, waves_enable); r.y += r.height + S;
            
            // if (target.target.compiler.GetFloat(_FoamWavesSpeed_SW) == 0) { r = target.DRAW_SLIDER(r, "Offset", "_FoamOffset", 0, 1f, waves_enable); r.y += r.height + S; }
            r = target.DRAW_DOUBLEFIELDS(r, "Waves Tile", new[] { _FoamLShoreWavesTileX_SW, _FoamLShoreWavesTileY_SW }, new[] { 0.00001f, 0.00001f }, new[] { 25, 25f }, waves_enable, firstDiv: 10, seconddiv: 10);
            r.y += r.height + S;
            r = target.DRAW_DOUBLEFIELDS(r, "Length/Fade", new[] { _FoamLength_SW, _WaterfrontFade_SW }, new[] { 0.01f, 0f }, new[] { hasAddPass ? 1 : 50, 0.99f }, waves_enable); r.y += r.height + S;
            r = target.DRAW_SLIDER(r, "Fade Offset", "_FoamMaskOffset_SW", 0, 1f, waves_enable); r.y += r.height + S;
            
            
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Direction", MessageType.None); r.y += r.height;
            r = target.DIRECTION(r, _FoamDirection_SW, waves_enable, VECTOR: false, leftClamp: -1f, rightClamp: 1f); r.y += r.height;
            r = target.DRAW_DOUBLEFIELDS(r, "Distortion/Fade", new[] { _FoamDistortion_SW, _FoamDistortionFade_SW }, new[] { 0f, 0f }, new[] { 10, 1f }, waves_enable);
            GUI_TOOLTIP(r, "Distortion is distortion, Fade means fading of the distortion near the shore"); r.y += r.height + S;
            target.DRAW_TOGGLE(r, "Inverse Fade", "INVERCE_FADE_DISTORTION", waves_enable, out tV); r.y += 31;
            
            r.y += 10;
            var usetextureSW = target.DRAW_TOGGLE(r, "Color Texture", NEED_SHORE_WAVES_UNPACK, waves_enable, out tV) & waves_enable; r.y += 31;
            r = target.DRAW_BG_TEXTURE(r, null, _FoamTexture_SW, usetextureSW, out tV, null); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Texture Tile", _FoamTextureTiling_SW, 0.001f, 10f, usetextureSW); r.y += r.height + S;
            r = target.DRAW_SLIDER(r, "Distortion", "_ShoreDistortionTexture", 0, 10f, usetextureSW); r.y += r.height + S;
            
            
            GUI.color = oc;
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void LowDistortion(ref Rect r, FastWaterModel20ControllerEditor target)
        {   GUI.enabled = true;
            var oc = GUI.color;
            GUI.color = new Color32(208, 172, 63, 255);
            r.height = FastWaterModel20ControllerEditor.H * 2;
            EditorGUI.HelpBox(r, "Low Distortion Params", MessageType.None); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Tile X/Y", "_LOW_DISTOR_Tile", 0.001f, 1000f, GUI.enabled); r.y += r.height;
            // r = target.DRAW_SLIDER(r, "Amount", "_LOW_DISTOR_Amount", 0, 10f, GUI.enabled, div: 10); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Speed", "_LOW_DISTOR_Speed", 0, 100f, GUI.enabled); r.y += r.height;
            GUI.color = oc;
            
        }
        
        
        
        
        public bool ShoreWavesCommon(ref Rect r, out bool hasAddPass, bool? enable = null)
        {   string SHORE_WAVES = "SHORE_WAVES";
            string _ShoreWavesRadius = "_ShoreWavesRadius";
            
            
            GUI.enabled = enable ?? true;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Second Z Pass", MessageType.None); r.y += r.height;
            var OV = target.target.compiler.IsKeywordEnabled(SHORE_WAVES);
            var waves_enable = true;
            if (!enable.HasValue)
            {   waves_enable = enable ?? target.DRAW_TOGGLE(r, "images_shorewaves", SHORE_WAVES, true, out tV);
                r.y += 31;
            }
            
            //  target.DRAW_TOGGLE(r, "images_surfacefoam", null, true, out tV); r.y += 31;
            
            
            
            GUI.enabled = waves_enable;
            bool chacngeResolution = false;
            bool chacngeSecondPass = false;
            bool q_changed = false;
            
            
            if (!enable.HasValue)
            {   var old = target.target.compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH");
                hasAddPass = target.DRAW_TOGGLE(r, "Second Z Pass", "SKIP_SECOND_DEPTH", waves_enable, out tV) && waves_enable;
                GUI_TOOLTIP(r, "Second z depth pass, allows you to apply additional settings for the depth map, you can use the main z buffer, if you do not need them"); r.y += r.height;
                
                r.y += 31;
                if (old != target.target.compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH")) chacngeSecondPass = true;
                
                if (!target.target.compiler.IsKeywordEnabled("ALLOW_MANUAL_DEPTH"))
                {
                
                }
            }
            else
            {   hasAddPass = enable.Value;
            }
            
            
            
            GUI.enabled = hasAddPass && waves_enable;
            // if (hasAddPass)
            {   var old2 = target.target.compiler.IsKeywordEnabled("SKIP_SHORE_BORDERS");
                target.DRAW_TOGGLE(r, "Bake Borders", "SKIP_SHORE_BORDERS", GUI.enabled, out tV); r.y += 31;
                if (old2 != target.target.compiler.IsKeywordEnabled("SKIP_SHORE_BORDERS")) chacngeSecondPass = true;
                
                r = target.DRAW_RESOLUTION(r, FIELDS.baked_BakedData_temp_size, out chacngeResolution); r.y += r.height;
                r = target.DRAW_KAYERMASK(r, "Render Layers", "_ZDepthBakeLayers", () => target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.ZDepth, target.LastResolution)); r.y += r.height;
                
                /*  var oldValue = target.target.compiler.BakedRefractionCameraOffset;
                  var newValue = oldValue;
                  Vector2 ov;
                  r = target.DRAW_SLIDER(r, "Baking Offset", null, compilerValue: ref newValue, leftValue: 0, rightValue: 200, enable: GUI.enabled, output: out ov);
                  GUI_TOOLTIP(r, "Use this to fix artifacts near the shore, but be careful, so far as another objects can be baked als, use culling mask layers to exlude them");
                  r.y += r.height;
                  if (oldValue != newValue)
                  {
                      target.target.Undo();
                      target.target.compiler.BakedRefractionCameraOffset = newValue;
                      target.target.SetDirty();
                      target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.Refraction, target.LastResolution);
                  }*/
                
                var oldR = target.target.compiler.GetFloat(_ShoreWavesRadius);
                r = target.DRAW_SLIDER(r, "Radius", _ShoreWavesRadius, 0.001f, 20f, GUI.enabled); r.y += r.height;
                if (oldR != target.target.compiler.GetFloat(_ShoreWavesRadius)) chacngeSecondPass = true;
                
                old2 = target.target.compiler.IsKeywordEnabled("USE_SIM_RADIUS");
                target.DRAW_TOGGLE(r, "Symmetry Radius", "USE_SIM_RADIUS", GUI.enabled, out tV); r.y += 31;
                if (old2 != target.target.compiler.IsKeywordEnabled("USE_SIM_RADIUS")) chacngeSecondPass = true;
                
                q_changed = target.pop_float_horequality.DrawPop("Quality", ref r);
                r.height = FastWaterModel20ControllerEditor.H * 4;
                EditorGUI.HelpBox(r, "You can reduce the texture resolution to achieve more blur", MessageType.None); r.y += r.height;
                
                
                //  if (!enable.HasValue)
                {   target.DRAW_TOGGLE(r, "Debug Second Z", "DEGUB_Z_SHORE", GUI.enabled); r.y += 31;
                    r.y += FastWaterModel20ControllerEditor.H;
                }
                
                
            }
            if (chacngeResolution || q_changed || chacngeSecondPass || OV != target.target.compiler.IsKeywordEnabled(SHORE_WAVES))
                target.target.m_BakeOrUpdateTexture(BakeOrUpdateType.ZDepth, target.LastResolution);
                
            return waves_enable;
        }
    }
    
    
    
    
}
}
#endif
