
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_TIZEN || UNITY_TVOS || UNITY_WP_8 || UNITY_WP_8_1
    #define MOBILE_TARGET
#endif


#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {
    public bool MODE_ADVANCEPC { get { return !target.compiler.IsKeywordEnabled("MINIMUM_MODE") && !target.compiler.IsKeywordEnabled("ULTRA_FAST_MODE"); } }
    public bool MODE_ULTRA_FAST { get { return target.compiler.IsKeywordEnabled("ULTRA_FAST_MODE") || target.compiler.IsKeywordEnabled("MINIMUM_MODE"); } }
    public bool MODE_MINIMUM { get { return target.compiler.IsKeywordEnabled("MINIMUM_MODE"); } }
    
    
    POP_CLASS __pop_waves;
    public POP_CLASS pop_waves
    {   get
        {   return __pop_waves ?? (__pop_waves = new POP_CLASS()
            {   target = this,
                    keys = new[] { "WAVES_MAP_NORMAL", "WAVES_MAP_CROSS", "USE_4X_BUMP"  },
                contents = new string[] { "Parallel (Texture)", "Cross (Texture)", "HQ Mix (4x Texture)" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_waves_blend;
    public POP_CLASS pop_waves_blend
    {   get
        {   return __pop_waves_blend ?? (__pop_waves_blend = new POP_CLASS()
            {   target = this,
                    /// keys = new[] { "WAVES_MAP_BLENDMIX", "WAVES_MAP_BLENDMAX" },
                    keys = new[] { null, "WAVES_MAP_BLENDLERP" },
                /// contents = new string[] { "Blend Both", "Use Max" },
                contents = new string[] { "Blend Sum", "Blend Lerp" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_mainblend;
    public POP_CLASS pop_mainblend
    {   get
        {   return __pop_mainblend ?? (__pop_mainblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "MAINTEXMAXBLEND", "MAINTEXLERPBLEND" },
                contents = new string[] { "Use Max", "Blend Both" },
                defaultIndex = 1
            });
        }
    }
    
    
    POP_CLASS __pop_tilemasktype;
    public POP_CLASS pop_tilemasktype
    {   get
        {   return __pop_tilemasktype ?? (__pop_tilemasktype = new POP_CLASS()
            {   target = this,
                    keys = new[] { "TILEMASKTYPE_OFFSET", "TILEMASKTYPE_TILE", "TILEMASKTYPE_OFFSETTILE" },
                contents = new string[] { "Offset Only", "Tile Only", "Offset + Tile" },
                defaultIndex = 1
            });
        }
    }
    
    POP_CLASS __pop_detiler;
    public POP_CLASS pop_detiler
    {   get
        {   return __pop_detiler ?? (__pop_detiler = new POP_CLASS()
            {   target = this,
                    keys = new string[] { "DETILE_NONE", "DETILE_LQ", "DETILE_HQ" },
                contents = new[] { "none", "Vertex Detiler", "HQ Detiler" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_3dwavesborders;
    public POP_CLASS pop_3dwavesborders
    {   get
        {   return __pop_3dwavesborders ?? (__pop_3dwavesborders = new POP_CLASS()
            {   target = this,
                    keys = new[] { "VERTEX_ANIMATION_BORDER_NONE", "VERTEX_ANIMATION_BORDER_CLAMPXZ", "VERTEX_ANIMATION_BORDER_CLAMPXYZ", "VERTEX_ANIMATION_BORDER_FADE" },
                contents = new string[] { "Animate All", "Skip Edges Vertices (XZ)", "Skip Edges Vertices (XZ)+(Y)", "Depend on Baked ZDepth " },
                defaultIndex = 0
            });
        }
    }
    
    
    POP_CLASS __pop_normalMastTextureSwitcher;
    public POP_CLASS pop_normalMastTextureSwitcher
    {   get
        {   return __pop_normalMastTextureSwitcher ?? (__pop_normalMastTextureSwitcher = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "_UF_NMASK_USE_MAINTEX" },
                contents = new[] { "Second Texture", "Main Texture" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_normalMastChannel;
    public POP_CLASS pop_normalMastChannel
    {   get
        {   return __pop_normalMastChannel ?? (__pop_normalMastChannel = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "_UF_NMASK_G", "_UF_NMASK_B", "_UF_NMASK_A" },
                contents = new[] { "R", "G", "B", "A" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    public class D_MainTexture : IDrawable {
    
        public override void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material)
                return;
            for (int i = 0; i < output.Length; i++)
                output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 390, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            //DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100, ACTIVE_COLOR );
            // DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100 , PASSIVE_COLOR );
            // DRAW_TEXTURE("i_RGB", fullRect);
            
            
            
            r = target.DRAW_COLOR(r, "Main Tint", FIELDS._MainTexColor, true, out tV, false); r.y += r.height;
            // r = target.DRAW_SLIDER( r, "Alpha", FIELDS._Transparency, 0, 1, true ); r.y += r.height;
            var enableAlpha = !target.IS_OPAQUE;
            
            var a = target.DRAW_TOGGLE(r, "Alpha Pow", "TRANSPARENT_POW", enableAlpha); r.y += 31;
            if (a)
            {   target.DRAW_TOGGLE(r, "Inverse Pow", "TRANSPARENT_POW_INV", enableAlpha); r.y += 31;
                r = target.DRAW_SLIDER(r, "Transparency", "_TransparencyPow", -1, 1, true); r.y += r.height;
            }
            a = target.DRAW_TOGGLE(r, "Alpha Luminosity", "TRANSPARENT_LUMINOSITY", enableAlpha); r.y += 31;
            if (a)
            {   r = target.DRAW_SLIDER(r, "Transparency", "_TransparencyLuminosity", 0, 5, true); r.y += r.height;
            }
            a = target.DRAW_TOGGLE(r, "Alpha Simple", "TRANSPARENT_SIMPLE", enableAlpha); r.y += 31;
            if (a)
            {   r = target.DRAW_SLIDER(r, "Transparency", "_TransparencySimple", 0, 1, true);
                r.y += r.height;
            }
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            //var VL = DrawLine( tV , Offset_X( tV, 160 ), 3 );
            // DrawLine( VL, Offset_Y( VL, 300 ), 3 );
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 100;
                r.y = 0;
            }
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Style", MessageType.None); r.y += r.height;
            target.pop_waves.DrawPop(null, r); r.y += r.height;
            if (target.pop_waves.VALUE == 2)
            {   if (target.MODE_ULTRA_FAST && !target.MODE_MINIMUM)
                {
                
                }
                else
                {   r.height = FastWaterModel20ControllerEditor.H * 6;
                    EditorGUI.HelpBox(r, "Only Ultra Fast Mode Support", MessageType.Error); r.y += r.height;
                }
            }
            //  if (target.pop_waves.VALUE < 2)
            {   r.height = FastWaterModel20ControllerEditor.H;
                r = target.DRAW_SLIDER(r, "Waves Amount", FIELDS._BumpAmount, 0, 100, true); r.y += r.height;
                if (!target.MODE_MINIMUM)
                {   var am = target.DRAW_TOGGLE(r, "Alternative Mix", "ALTERNATIVE_NORMALS_MIX", true); r.y += 31;
                    if (am)
                    {   r = target.DRAW_SLIDER(r, null, "_BumpMixAmount", 0, 1, true, div: 100); r.y += r.height;
                    }
                }
                
                r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._BumpMap) as Texture2D, true, null); r.y += r.height;
                r = target.DRAW_VECTOR(r, "Tile X/Y", FIELDS._WaterTextureTile, 0, 10000, true);
                GUI_TOOLTIP(r, "This Tile affects all child Tile");
                r.y += r.height;
                r = target.DRAW_VECTOR(r, "Speed Cross", FIELDS._WaterTextureTile, -10, 10, true, true, div: 10);
                GUI_TOOLTIP(r, "Two textures will scroll towards each other, and will make a simple animation ");
                r.y += r.height;
                r = target.DRAW_VECTOR(r, "Move Direction", "_AnimMove", -10, 10, true, false, div: 100);
                GUI_TOOLTIP(r, "This option in order to create common flow direction ");
                r.y += r.height;
                if (!target.MODE_MINIMUM)
                {
                
                    var ba = target.DRAW_TOGGLE(r, "Use Blend Animation", "SKIP_BLEND_ANIMATION", true); r.y += 31;
                    if (ba)
                    {   r = target.DRAW_SLIDER(r, "Speed Blend", "_BlendAnimSpeed", 0.001f, 4, true); r.y += r.height;
                        target.DRAW_TOGGLE(r, "Smooth Animation", "SMOOTH_BLEND_ANIMATION", true); r.y += 31;
                        
                        #if MOBILE_TARGET
                        /*  r.height = FastWaterModel20ControllerEditor.H * 6;
                          EditorGUI.HelpBox(r, "Note that, Blend Animation does not work on the openGLES 2.0, you may make a separate material for different platforms", MessageType.None); r.y += r.height;*/
                        #endif
                    }
                }
                
                r.y += FastWaterModel20ControllerEditor.H;
                a = target.DRAW_TOGGLE(r, "Debug Normals", "DEBUG_NORMALS", true); r.y += 31;
            }
            
            r.y += 10;
            
            
            if (target.MODE_ADVANCEPC)
            {   var classicnoise = target.DRAW_TOGGLE(r, "Procedure Pass Test", "WAVES_GERSTNER", true); r.y += 31;
                // else
                if (classicnoise)
                {   r = target.DRAW_SLIDER(r, "LOD Distance", "_CN_DISTANCE", 1, 1000, true);
                    GUI_TOOLTIP(r, "At what distance will the noise");
                    r.y += r.height;
                    target.DRAW_TOGGLE(r, "Debug Noise", "_CN_DEBUG", true); r.y += 31;
                    r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._NoiseHQ) as Texture2D, GUI.enabled); r.y += r.height;
                    r = target.DRAW_VECTOR(r, "P Tile X/Y", "_CN_TILING", 0.001f, 100, true, false); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "P Add Amount", "_CN_AMOUNT", 0, 100, true); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "P Pow Blend", "_CLASNOISE_PW", 0, 1, true, div: 10); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "P Texel Size", "_CN_TEXEL", 0.0001f, 0.1f, true, div: 100); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "P Speed", "_CN_SPEED", 0, 10, true); r.y += r.height;
                    
                    
                    /* r = target.DRAW_SLIDER(r, "Waves Amount", FIELDS._BumpAmount, 0, 2, true); r.y += r.height;
                     target.VECTOR_FIELD(ref r, "_GAmplitude", "_GAmplitude");
                     target.VECTOR_FIELD(ref r, "_GFrequency", "_GFrequency");
                     // target.VECTOR_FIELD(ref r, "_GSteepness", "_GSteepness");
                     target.VECTOR_FIELD(ref r, "_GSpeed", "_GSpeed");
                     target.VECTOR_FIELD(ref r, "_GDirectionAB", "_GDirectionAB");
                     target.VECTOR_FIELD(ref r, "_GDirectionCD", "_GDirectionCD");*/
                }
                
                r.y += 10;
                
                
                
                
                
                
                // r.y += r.height + FastWaterModel20ControllerEditor.H;
                
                //////////////////////////////
                
            }
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 200;
                r.y = 0;
            }
            else
            {   r.y = 0;
                r.x = 100;
            }
            
            if (!target.MODE_MINIMUM)
            {
            
            
                target.DRAW_TOGGLE(r, "Normalize wNormal", "SKIP_WNN", !target.target.compiler.IsKeywordEnabled("HAS_Z_AFFECT_BUMP"), out tV);
                GUI_TOOLTIP(r, "If enabled, normal will normalize for each pixel. You may disable this, if you're using mega old device"); r.y += 31;
                r.y += FastWaterModel20ControllerEditor.H;
                
                var zfade = target.DRAW_TOGGLE(r, "Normals Z Fade", "Z_AFFECT_BUMP", true, out tV); r.y += 31;
                if (zfade)
                {   r = target.DRAW_SLIDER(r, "Offset", "_BumpZFade", -30f, 10, true); r.y += r.height;
                    r = target.DRAW_SLIDER(r, "Fade", "_BumpZOffset", 1f, 30, true); r.y += r.height;
                    target.DRAW_TOGGLE(r, "Inverse", "INVERT_Z_AFFECT_BUMP", true, out tV); r.y += 31;
                    
                }
            }
            
            
            if (target.MODE_ADVANCEPC)
            {   r.y += FastWaterModel20ControllerEditor.H;
                var oct = target.DRAW_TOGGLE(r, "Multi Octaves", "MULTI_OCTAVES", true, out tV); r.y += 31;
                if (oct)
                {   r = target.DRAW_SLIDER(r, "Count", "_MultyOctaveNormals", 1f, 8, true, integer: true); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H;
                    r = target.DRAW_SLIDER(r, "Blend Amount", "_FadingFactor", 0f, 1, true); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H;
                    target.pop_waves_blend.DrawPop(null, r); r.y += r.height;
                    
                    target.DRAW_TOGGLE(r, "Toward Moving", "SKIP_MO_TOWARD", true, out tV);
                    GUI_TOOLTIP(r, "If enabled, then two same normals will mix together moving towards each other (I do not think that this is useful, but and did not remove)");
                    r.y += 31;
                    r = target.DRAW_VECTOR(r, "Tile Offset", "_MultyOctavesTileOffset", 0.1f, 10, true); r.y += r.height;
                    GUI_TOOLTIP(r, "Offset means that each new octave will be offset by a given coefficient relative to the base value");
                    r = target.DRAW_SLIDER(r, "Speed Offset", "_MultyOctavesSpeedOffset", 0.1f, 3f, true); r.y += r.height;
                    GUI_TOOLTIP(r, "Offset means that each new octave will be offset by a given coefficient relative to the base value");
                    
                    var rotenable = target.DRAW_TOGGLE(r, "Rotate by Time", "MULTI_OCTAVES_ROTATE", true, out tV);
                    GUI_TOOLTIP(r, "During the time the normals will rotate, this will give the little dynamus, at higher values it will become like boiling water");
                    r.y += 31;
                    if (rotenable)
                    {   r = target.DRAW_SLIDER(r, "Base Rotate", "_MOR_Base", 0f, 10, true); r.y += r.height;
                        r = target.DRAW_SLIDER(r, "Rotate Offset ", "_MOR_Offset", 0.1f, 3f, true);
                        GUI_TOOLTIP(r, "Offset means that each new octave will be offset by a given coefficient relative to the base value"); r.y += r.height;
                        
                        var uvtoroate = target.DRAW_TOGGLE(r, "Add UV to Rotate", "MULTI_OCTAVES_ROTATE_TILE", true, out tV);
                        GUI_TOOLTIP(r, "Some normals have a texture with parallel waves, use this feature to avoid of artifacts when using such textures");
                        r.y += 31;
                        if (uvtoroate)
                        {   r = target.DRAW_SLIDER(r, "UV Scale ", "_MOR_Tile", 0.1f, 300f, true);
                            r.y += r.height;
                        }
                        
                    }
                    
                    
                    //  r = target.DRAW_SLIDER(r, "Tile Offset", "_MultyOctavesTileOffset", 0.5f, 2f, true); r.y += r.height;
                    
                    
                }
            }
            
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            var sd = target.DRAW_TOGGLE(r, "Sin Disatortion", "SIN_OFFSET", true); r.y += 31;
            if (sd)
            {   r = target.DRAW_SLIDER(r, "Amount", "_sinAmount", -0.5f, 0.5f, true); r.y += r.height;
                r = target.DRAW_SLIDER(r, "Tile", "_sinFriq", 0.1f, 300, true); r.y += r.height;
            }
            
            if (!target.MODE_MINIMUM)
            {
            
                GUI.enabled = true;
                r.y += FastWaterModel20ControllerEditor.H;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "DeTiler", MessageType.None); r.y += r.height;
                target.DRAW_TOGGLE(r, "images_detiling", null, true, out tV); r.y += 31;
                r.height = FastWaterModel20ControllerEditor.H;
                target.pop_detiler.DrawPop(null, r);
                r.y += r.height;
                if (target.MODE_ULTRA_FAST && target.pop_detiler.VALUE == 2)
                {   r.height = FastWaterModel20ControllerEditor.H * 8;
                    EditorGUI.HelpBox(r, "HQ detiling not allow with Ultra Fast Mode", MessageType.Error);
                }
                else
                {   if (target.pop_detiler.VALUE != 0)
                    {   r = target.DRAW_SLIDER(r, "Amount", "_DetileAmount", 0.01f, 1, true, div: 10); r.y += r.height;
                        r = target.DRAW_SLIDER(r, "Frequency", "_DetileFriq", 0.01f, 50, true); r.y += r.height;
                        target.DRAW_TOGGLE(r, "Same dir as anim", "DETILE_SAME_DIR", true, out tV); r.y += 31;
                    }
                }
                
            }
            
            
            if (target.MODE_ULTRA_FAST)
            {   r.y = 0;
                r.x = 300;
                
                string FEATURE = "UF_AMOUNTMASK";
                string[] texture = new[] { "_UF_NMASK_Texture", FIELDS._MainTex} ;
                POP_CLASS texture_SWITCHER = target.pop_normalMastTextureSwitcher;
                POP_CLASS pop_channel = target.pop_normalMastChannel;
                string tile = "_UF_NMASK_Tile";
                string offset = "_UF_NMASK_offset";
                string contrast = "_UF_NMASK_Contrast";
                string brightness = "_UF_NMASK_Brightnes";
                bool enable = GUI.enabled;
                //////////////////////////////
                r = target.DRAW_MASK_PANEL_CB(r, "Normals Mask", null, FEATURE, texture, texture_SWITCHER, tile, offset, contrast, brightness,
                                              "AMOUNT_MASK_DEBUG", ref enable, pop_channel); r.y += r.height;
                //////////////////////////////
                
                
            }
            
            if (target.MODE_ADVANCEPC)
            {   r.y = 0;
                r.x = 200;
                
                
                string FEATURE = "AMOUNTMASK";
                string tile = "_AMOUNTMASK_Tile";
                string amount = "_AMOUNTMASK_Amount";
                string min = "_AMOUNTMASK_min";
                string max = "_AMOUNTMASK_max";
                string offset = "_AMOUNTMASK_offset";
                bool enable = GUI.enabled;
                POP_CLASS pop = null;
                //////////////////////////////
                r = target.DRAW_MASK_PANEL(r, "Amount Mask", "white means that the waves in this place will be larger", FEATURE, tile, offset, amount, min, max, "AMOUNT_MASK_DEBUG", ref enable, pop);
                //////////////////////////////
                
                r.y += FastWaterModel20ControllerEditor.H;
                
                FEATURE = "TILINGMASK";
                tile = "_TILINGMASK_Tile";
                amount = "_TILINGMASK_Amount";
                min = "_TILINGMASK_min";
                max = "_TILINGMASK_max";
                offset = "_TILINGMASK_offset";
                enable = GUI.enabled;
                pop = null;
                //////////////////////////////
                r = target.DRAW_MASK_PANEL(r, "Tile Mask", "white means that the waves in this place will be more tiled", FEATURE, tile, offset, amount, min, max, "TILE_MASK_DEBUG", ref enable, pop,
                                           amountClamp: new Vector2(0, 20));
                                           
                                           
                GUI.enabled = enable;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Tile Type", MessageType.None); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H;
                target.pop_tilemasktype.DrawPop(null, r); r.y += r.height;
                if (target.pop_tilemasktype.VALUE > 0)
                {   r = target.DRAW_SLIDER(r, "Tile Factor", "_TILINGMASK_factor", 0.01f, 50, GUI.enabled);
                    r.y += r.height;
                }
                //////////////////////////////
                
                
                
                r.x += r.width + 14;
                r.x = 200;
                //r.y = VL.y + 20;
                r.y = 60;
                // r.y =0 ;
                
            }
            
            
            if (!target.MODE_ULTRA_FAST)
            {   target.d_D_Vertices.target = target;
                Vector2[] draw_output;
                target.d_D_Vertices.Draw(new Rect(300, 0, 100, 0), out draw_output);
            }
            
            
            
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
    }
    
    
    
    
    
    
    
    POP_CLASS __pop_vertTExtureSwitcher;
    public POP_CLASS vertTExtureSwitcher
    {   get
        {   return __pop_vertTExtureSwitcher ?? (__pop_vertTExtureSwitcher = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "VERT_SECOND_TEXTURE", "VERT_FOAM_TEXTURE" },
                contents = new[] { "Main Texture", "Second Texture", "Shore Texture" },
                defaultIndex = 0
            });
        }
    }
    
    POP_CLASS __pop_vertTextureChannel;
    public POP_CLASS pop_vertTextureChannel
    {   get
        {   return __pop_vertTextureChannel ?? (__pop_vertTextureChannel = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "VERT_G", "VERT_B", "VERT_A" },
                contents = new[] {  "R", "G", "B", "A" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    public class D_Vertices : IDrawable {
    
    
    
    
    
    
        public override void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material)
                return;
                
            for (int i = 0; i < output.Length; i++)
                output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, input.width, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var og = GUI.enabled;
            
            var S = 0;
            var foam_enable = target.DRAW_TOGGLE(r, "images_vertices", FIELDS.SKIP_3DVERTEX_ANIMATION, true, out tV); r.y += 31;
            GUI.enabled = foam_enable;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "3D Vertices", MessageType.None); r.y += r.height;
            
            // r = target.DRAW_DOUBLEFIELDS(r, "Tile Z", new[] { "_3DWavesTileZ", "_3DWavesTileZAm" }, new[] { 0.01f, 0f }, new[] { 2, 2f }, foam_enable); r.y += r.height;
            r = target.DRAW_DOUBLEFIELDS(r, "Height/Wind", new[] { FIELDS._3DWavesHeight, FIELDS._3DWavesWind }, new[] { 0, -64f }, new[] { 256, 64f }, foam_enable, firstDiv: 100, seconddiv: 100);
            r.y += r.height;
            
            r = target.DRAW_DOUBLEFIELDS(r, "Speed X/Y", new[] { "_3DWavesSpeed", "_3DWavesSpeedY" },
                                         new[] { -4f, -4 }, new[] { 4f, 4 }, foam_enable, firstDiv: 50, seconddiv: 50); r.y += r.height;
                                         
            r.y += FastWaterModel20ControllerEditor.H;
            
            r = target.DRAW_VECTOR(r, "Detile Y Clamp", "_VERTEX_ANIM_DETILE_YOFFSET", -10, 10, foam_enable); r.y += r.height;
            r = target.DRAW_VECTOR(r, "Detile Y Offset", "_VERTEX_ANIM_DETILE_YOFFSET", -10, 10, foam_enable, useOneIndex: 1, useSecondPair: true); r.y += r.height;
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            r = target.DRAW_SLIDER(r, "UV Amount", "_VertexToUv", 0, 1, foam_enable); r.y += r.height;
            
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 100;
                r.y = 0;
            }
            
            r = target.DRAW_VECTOR(r, "Tile X / Y", FIELDS._3DWavesTile, 0, 2000, foam_enable); r.y += r.height + S;
            
            // var detileEnalbe = foam_enable && ( target.target.compiler.IsKeywordEnabled("DETILE_LQ") || target.target.compiler.IsKeywordEnabled("DETILE_HQ" ));
            var detileEnalbe = foam_enable && target.DRAW_TOGGLE(r, "Detailed Tile", "HAS_WAVES_DETILE", foam_enable, out tV); r.y += 31;
            r = target.DRAW_DOUBLEFIELDS(r, "Detile/Tile", new[] { "_VERTEX_ANIM_DETILEAMOUNT", "_VERTEX_ANIM_DETILEFRIQ" }, new[] { 0, 0.001f }, new[] { 256, 64f },
                                         detileEnalbe, seconddiv: 10); r.y += r.height;
            //  r = target.DRAW_SLIDER(r, "Detile Offset", "_VERTEX_ANIM_DETILE_YOFFSET", -10, 10, detileEnalbe); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Detile Speed", "_VERTEX_ANIM_DETILESPEED", 0, 64, detileEnalbe, div: 100); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Direction", MessageType.None); r.y += r.height;
            var oldT = target.target.compiler.GetFloat("_WavesDirAngle");
            r = target.DIRECTION(r, "_WavesDirAngle", foam_enable, VECTOR: false, leftClamp: 0, rightClamp: 6.2834f); r.y += r.height;
            var newT = target.target.compiler.GetFloat("_WavesDirAngle");
            if (oldT != newT && (oldT == 0 || newT == 0))
            {   target.target.Undo();
                if (newT != 0)
                    target.target.compiler.EnableKeyword("HAS_WAVES_ROTATION");
                else
                    target.target.compiler.DisableKeyword("HAS_WAVES_ROTATION");
                target.target.SetDirty();
            }
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 200;
                r.y = 0;
            }
            if (!target.MODE_MINIMUM)
            {
            
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Animating Area", MessageType.None); r.y += r.height;
                target.pop_3dwavesborders.DrawPop(null, r); r.y += r.height;
                // var bf = target.DRAW_TOGGLE( r, "Border Fading", "SKIP_3DVERTEX_ANIMATION_BORDER_FADE", foam_enable, out tV ) & foam_enable; r.y += 31;
                if (target.pop_3dwavesborders != null && target.pop_3dwavesborders.VALUE == 3 && target.pop_depth != null)
                {   r.height = FastWaterModel20ControllerEditor.H * 3;
                    if (target.__pop_depth.VALUE == 0 )
                    {   EditorGUI.HelpBox(r, "Request ZDepth", MessageType.Error);
                        r.y += r.height + S;
                    }
                    if (target.__pop_depth.IS(FIELDS.ALLOW_MANUAL_DEPTH) && target.target.compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH"))
                    {   EditorGUI.HelpBox(r, "Request Backed ZDepth", MessageType.Error);
                        r.y += r.height + S;
                    }
                    r = target.DRAW_SLIDER(r, "Clamp Amount", "_3Dwaves_BORDER_FACE", -4, 4f, foam_enable, div: 50); r.y += r.height + S;
                    
                }
                //  _BAKED_DEPTH_EXTERNAL_TEXTURE_Amount
            }
            //r = target.DRAW_VECTOR(r, "Speed X/Y", FIELDS._3DWavesSpeed, -4, 4, foam_enable, div: 50); r.y += r.height + S;
            // r = target.DRAW_SLIDER(r, "Speed X/Y", "_3DWavesSpeed", -4, 4f, foam_enable, div: 50); r.y += r.height + S;
            
            r.y += FastWaterModel20ControllerEditor.H;
            if (!target.MODE_MINIMUM)
            {   //r = target.DRAW_SLIDER( r, "Wind Amount", , 0.1f, 32, foam_enable ); r.y += r.height + S;
                //r = target.DRAW_COLOR( r, "Color", FIELDS._3d, true, out tV ); r.y += r.height + S;
                var nrmmrm = target.DRAW_TOGGLE(r, "Calc Normals", FIELDS.WAW3D_NORMAL_CALCULATION, foam_enable) & foam_enable; r.y += 31;
                r = target.DRAW_SLIDER(r, "Amount", "_3dwanamnt", -1000f, 1000, nrmmrm); r.y += r.height;
                
            }
            
            r.y += FastWaterModel20ControllerEditor.H;
            if (target.MODE_ULTRA_FAST)
            {   r.x = 300;
                r.y = 0;
            }
            var SimpleColorize = target.DRAW_TOGGLE(r, "Simple Colorize", "USE_SIMPLE_VHEIGHT_FADING", foam_enable, out tV) & foam_enable; r.y += 31;
            r = target.DRAW_SLIDER(r, "Color Amount", "SIMPLE_VHEIGHT_FADING_AFFECT", 0, 5f, SimpleColorize, div: 10); r.y += r.height + S;
            
            
            if (!target.MODE_MINIMUM)
            {   /*uniform MYFIXED _3DWavesYFoamAmount;
                uniform MYFIXED _WaveGradTopOffset;
                uniform MYFIXED _VERT_Tile;
                uniform MYFIXED3 _VERT_Color;*/
                r.y += FastWaterModel20ControllerEditor.H;
                var Colorize = target.DRAW_TOGGLE(r, "Add Texture", FIELDS.SKIP_3DVERTEX_HEIGHT_COLORIZE, foam_enable, out tV) & foam_enable; r.y += 31;
                if (Colorize)
                {   r = target.DRAW_SLIDER(r, "Y Multiply", FIELDS._3DWavesYFoamAmount, 0, 10f, Colorize, div: 100); r.y += r.height + S;
                    r = target.DRAW_SLIDER(r, "Y Offset", "_WaveGradTopOffset", 0, 100f, Colorize); r.y += r.height + S;
                    r.y += FastWaterModel20ControllerEditor.H;
                    r = target.DRAW_SLIDER(r, "Amount", "_VERT_Amount", 0, 10f, Colorize); r.y += r.height + S;
                    r = target.DRAW_COLOR(r, null, "_VERT_Color", Colorize, out tV, false); r.y += r.height;
                    target.DRAW_TOGGLE(r, "Multiply MainTex", "VERT_USE_MULTIPLUOUT", Colorize, out tV); r.y += 31;
                    target.DRAW_TOGGLE(r, "Camera Angle", "USE_VERT_FRESNEL", Colorize, out tV); r.y += 31;
                    
                    string[] texture = new[] { FIELDS._MainTex,  "_ShoreWavesGrad", "_UF_NMASK_Texture" } ;
                    POP_CLASS texture_SWITCHER = target.vertTExtureSwitcher;
                    POP_CLASS pop_channel = target.pop_vertTextureChannel;
                    bool enable = GUI.enabled;
                    //////////////////////////////
                    r =  target. DrawTextureWIthChannelAndSwitcher(r, Colorize, texture, texture_SWITCHER, pop_channel);
                    
                    r = target.DRAW_VECTOR(r, "Tile", "_VERT_Tile", 0.1f, 10, Colorize,  div: 10); r.y += r.height;
                    //  r = target.DRAW_SLIDER(r, "Tile", "_VERT_Tile", 0.0001f, 10f, Colorize, div: 10); r.y += r.height + S;
                    r.y += FastWaterModel20ControllerEditor.H;
                    target.DRAW_TOGGLE(r, "Debug Gradient", "DEBUG_TOP_GRAD", Colorize, out tV); r.y += 31;
                }
                
                
                /*
                 if (Colorize)
                 {   // var use_sg = GUI.enabled = target.DRAW_TOGGLE(r, "Surface Gradient", "USE_SURFACE_GRADS", GUI.enabled, out tV); r.y += 31;
                     r = target.DRAW_SLIDER(r, "Offset Top", "_WaveGradTopOffset", 0, 100, Colorize); r.y += r.height;
                     target.DRAW_TOGGLE(r, "Debug Gradient", "DEBUG_TOP_GRAD", Colorize, out tV); r.y += 31;
                     r = target.DRAW_COLOR(r, null, "_WaveGrad0", Colorize, out tV, false); r.y += r.height;
                     r = target.DRAW_COLOR(r, null, "_WaveGrad1", Colorize, out tV, false); r.y += r.height;
                     r = target.DRAW_COLOR(r, null, "_WaveGrad2", Colorize, out tV, false); r.y += r.height;
                     // r = target.DRAW_SLIDER(r, "Offset Mid", "_WaveGradMidOffset", 0, 1, Colorize); r.y += r.height;
                 }*/
            }
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
    }
    
    
}
}
#endif