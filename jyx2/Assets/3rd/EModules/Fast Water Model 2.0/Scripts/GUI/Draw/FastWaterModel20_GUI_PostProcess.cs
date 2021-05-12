
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {


    POP_CLASS __pop_gradientoutput;
    public POP_CLASS pop_gradientoutput
    {   get
        {   return __pop_gradientoutput ?? (__pop_gradientoutput = new POP_CLASS()
            {   target = this,
                    keys = new[] { "GRAD_5", "GRAD_4", "GRAD_3", "GRAD_2", "GRAD_1" },
                contents = new string[] { "Gradient 1", "Gradient 2", "Gradient 3", "Gradient 4", "Gradient 5" },
                defaultIndex = 4
            });
        }
    }
    POP_CLASS __pop_outputblend;
    public POP_CLASS pop_outputblend
    {   get
        {   return __pop_outputblend ?? (__pop_outputblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "USE_OUTPUT_BLEND_1", "USE_OUTPUT_BLEND_2", "USE_OUTPUT_BLEND_3" },
                contents = new string[] { "MaxMix+ZDepth", "ZDepth", "Simple" },
                defaultIndex = 1
            });
        }
    }
    
    POP_CLASS __pop_zcolorize1;
    public POP_CLASS pop_zcolorize1
    {   get
        {   return __pop_zcolorize1 ?? (__pop_zcolorize1 = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "lerped_post_SUB_1", "lerped_post_MUL_1", "lerped_post_LERP_1" },
                contents = new string[] { "Add", "Substract", "Multiply", "Replace" },
                defaultIndex = 1
            });
        }
    }
    POP_CLASS __pop_zcolorize2;
    public POP_CLASS pop_zcolorize2
    {   get
        {   return __pop_zcolorize2 ?? (__pop_zcolorize2 = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "lerped_post_SUB_2", "lerped_post_MUL_2", "lerped_post_LERP_2" },
                contents = new string[] { "Add", "Substract", "Multiply", "Replace" },
                defaultIndex = 1
            });
        }
    }
    
    
    
    POP_CLASS __pop_mmcolorize;
    public POP_CLASS pop_mmcolorize
    {   get
        {   return __pop_mmcolorize ?? (__pop_mmcolorize = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "MM_SUB", "MM_MUL", "MM_LERP" },
                contents = new string[] { "Add", "Substract", "Multiply", "Replace" },
                defaultIndex = 1
            });
        }
    }
    
    /*  POP_CLASS __pop_mmchannel;
      public POP_CLASS pop_mmchannel
      {   get
          {   return __pop_mmchannel ?? (__pop_mmchannel = new POP_CLASS()
              {   target = this,
                      keys = new[] { null, "MM_G", "MM_B" },
                  contents = new string[] { "R", "G", "B" },
                  defaultIndex = 1
              });
          }
      }*/
    
    
    POP_CLASS __pop_postTExtureSwitcher;
    public POP_CLASS pop_postTExtureSwitcher
    {   get
        {   return __pop_postTExtureSwitcher ?? (__pop_postTExtureSwitcher = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "POST_SECOND_TEXTURE", "POST_FOAM_TEXTURE", "POST_OWN_TEXTURE"},
                contents = new[] {"Main Texture", "Second Texture", "Foam Texture if awaliable", "Own Texture"},
                defaultIndex = 3
            });
        }
    }
    
    POP_CLASS __pop_postTextureChannel;
    public POP_CLASS pop_postTextureChannel
    {   get
        {   return __pop_postTextureChannel ?? (__pop_postTextureChannel = new POP_CLASS()
            {   target = this,
                    keys = new string[] { null, "MM_G", "MM_B", "MM_A" },
                contents = new[] { "R", "G", "B", "A" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    
    
    
    
    
    public class D_Output : IDrawable {
        override public void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, input.width, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W * 1.333333f, 0);
            var og = GUI.enabled;
            
            
            
            // r.y += 10;
            
            
            GUI.enabled = target.DRAW_TOGGLE(r, "ZDepth Colorize", "USE_lerped_post", true, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "Use Deep Z Depth", "lerped_post_USE_depthZ", GUI.enabled, out tV); r.y += 31;
            
            r = target.DRAW_SLIDER(r, "Offset", "lerped_post_offset", -80, 80, GUI.enabled); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Fallof", "lerped_post_offset_falloff", 0, 80, GUI.enabled); r.y += r.height;
            target.DRAW_TOGGLE(r, "Debug Offset", "lerped_post_Debug", GUI.enabled, out tV); r.y += 31;
            var oe = GUI.enabled;
            var c1 = target.DRAW_TOGGLE(r, "Color A", "USE_lerped_post_Color_1", oe, out tV) && oe; r.y += 31;
            GUI.enabled = c1;
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_zcolorize1.DrawPop(null, r); r.y += r.height;
            r = target.DRAW_COLOR(r, null, "lerped_post_color1", GUI.enabled, out tV, false); r.y += r.height;
            
            var c2 = target.DRAW_TOGGLE(r, "Color B", "USE_lerped_post_Color_2", oe, out tV) && oe; r.y += 31;
            GUI.enabled = c2;
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_zcolorize2.DrawPop(null, r); r.y += r.height;
            r = target.DRAW_COLOR(r, null, "lerped_post_color2", GUI.enabled, out tV, false); r.y += r.height;
            
            GUI.enabled = true;
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            
            var ent = target.DRAW_TOGGLE(r, "Texture Colorize", "POST_TEXTURE_TINT", true, out tV); r.y += 31;
            GUI.enabled = ent;
            
            
            /*  var foam = false;
              if (target.MODE_ULTRA_FAST)foam =  target.DRAW_TOGGLE(r, "Use Foam Texture", "POST_FOAM_TEXTURE", GUI.enabled, out tV) && GUI.enabled; r.y += 31;
              var own =  target.DRAW_TOGGLE(r, "Use Own Texture", "POST_OWN_TEXTURE", !foam && GUI.enabled, out tV) && !foam && GUI.enabled; r.y += 31;
              GUI.enabled = ent;
              if (foam) { r = target.DRAW_BG_TEXTURE(r, null, "_ShoreWavesGrad", foam, out tV, null); r.y += r.height; }
              else if (own) { r = target.DRAW_BG_TEXTURE(r, null, "_MM_Texture", own, out tV, null); r.y += r.height; }
              else { r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._MainTex) as Texture2D, GUI.enabled, null); r.y += r.height; }*/
            
            
            string[] texture = new[] { "_MainTex", "_UF_NMASK_Texture", "_ShoreWavesGrad", "_MM_Texture"} ;
            POP_CLASS texture_SWITCHER = target.pop_postTExtureSwitcher;
            POP_CLASS pop_channel = target.pop_postTextureChannel;
            bool enable = GUI.enabled;
            //////////////////////////////
            r =  target. DrawTextureWIthChannelAndSwitcher(r, GUI.enabled, texture, texture_SWITCHER, pop_channel);
            
            
            
            
            
            /*r.height = FastWaterModel20ControllerEditor.H;
            target.pop_mmchannel.DrawPop(null, r); r.y += r.height;*/
            r = target.DRAW_VECTOR(r, "Tile X/Y", "_MM_Tile", 0, 10000, GUI.enabled, div: 100); r.y += r.height;
            r = target.DRAW_VECTOR(r, "Offset X/Y", "_MM_offset", 0, 10000, GUI.enabled, div: 100); r.y += r.height;
            
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_mmcolorize.DrawPop(null, r); r.y += r.height;
            if (target.pop_mmcolorize.VALUE == 2)
            {   r = target.DRAW_SLIDER(r, "Multy Offset", "_MM_MultyOffset", 0, 1, GUI.enabled); r.y += r.height;
            
            }
            r = target.DRAW_COLOR(r, "Color", "_MM_Color", GUI.enabled, out tV, false); r.y += r.height;
            
            
            GUI.enabled = true;
            
            /*  GUI.enabled = true;
              var use_sg = GUI.enabled = target.DRAW_TOGGLE(r, "Surface Gradient", "USE_SURFACE_GRADS", GUI.enabled, out tV); r.y += 31;
              r = target.DRAW_SLIDER(r, "Offset Top", "_WaveGradTopOffset", 0, 100, GUI.enabled); r.y += r.height;
              target.DRAW_TOGGLE(r, "Debug Gradient", "DEBUG_TOP_GRAD", use_sg, out tV); r.y += 31;
              r = target.DRAW_COLOR(r, null, "_WaveGrad0", use_sg, out tV, false); r.y += r.height;
              r = target.DRAW_COLOR(r, null, "_WaveGrad1", use_sg, out tV, false); r.y += r.height;
              r = target.DRAW_COLOR(r, null, "_WaveGrad2", use_sg, out tV, false); r.y += r.height;
              r = target.DRAW_SLIDER(r, "Offset Mid", "_WaveGradMidOffset", 0, 1, GUI.enabled); r.y += r.height;
            
              */
            
            GUI.enabled = true;
            
            
            
            r.y += 10;
            r.x += r.width + 14;
            r.y = 0;
            
            GUI.enabled = og;
            GUI.enabled = target.DRAW_TOGGLE(r, "images_postrize", "POSTRIZE", true, out tV); r.y += 31;
            r = target.DRAW_SLIDER(r, "Posterize", "POSTRIZE_Colors", 1, 24, GUI.enabled); r.y += r.height;
            GUI.enabled = og;
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            
            
            
            
            GUI.enabled = target.DRAW_TOGGLE(r, "Convert Colors", "USE_OUTPUT_GRADIENT", true, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "images_cc", null, GUI.enabled, out tV); r.y += 31;
            r.height = FastWaterModel20ControllerEditor.H;
            //EditorGUI.HelpBox( r, "Utils (RGB:y>0.5)", MessageType.None ); r.y += r.height;
            r = target.DRAW_BG_TEXTURE(r, "Gradient Texture", "_GradTexture", GUI.enabled, out tV, null); r.y += r.height;
            // r = DRAW_GRAPHIC( r, 40, target.target.material.GetTexture( FIELDS._Utility ) as Texture2D, GUI.enabled, new Color( 1, 1, 1, 1 ), YUP: 0.5f ); r.y += r.height;
            
            r.height = FastWaterModel20ControllerEditor.H;
            target.pop_gradientoutput.DrawPop(null, r); r.y += r.height;
            EditorGUI.HelpBox(r, "Blend Mode", MessageType.None); r.y += r.height;
            target.pop_outputblend.DrawPop(null, r); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Blend", "_OutGradBlend", 0, 1, GUI.enabled); r.y += r.height;
            if (target.pop_outputblend.VALUE != 2)
            {   if (target.__pop_depth != null && target.__pop_depth.VALUE == 0)
                {   r.height = FastWaterModel20ControllerEditor.H * 3;
                    EditorGUI.HelpBox(r, "Request ZDepth", MessageType.Error); r.y += r.height;
                }
                r = target.DRAW_SLIDER(r, "ZDepth Affect", "_OutGradZ", 1, 100, GUI.enabled); r.y += r.height;
            }
            
            if (target.pop_outputblend.VALUE != 0)
            {   var fhl = target.DRAW_TOGGLE(r, "Fix Highlights", "FIX_HL", GUI.enabled, out tV) & GUI.enabled; r.y += 31;
                if (fhl)
                {   r = target.DRAW_SLIDER(r, "Clamp Value", "_FixHLClamp", 0.1f, 1, fhl); r.y += r.height;
                
                }
            }
            
            
            /*GUI.enabled = target.DRAW_TOGGLE(r, "Additional Shadows", "USE_OUTPUT_SHADOWS", GUI.enabled, out tV); r.y += 31;
            if (GUI.enabled) r = target.DRAW_SLIDER(r, "Amount", "_OutShadowsAmount", 0, 2, GUI.enabled); r.y += r.height;*/
            
            GUI.enabled = og;
            
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            GUI.enabled = target.DRAW_TOGGLE(r, "images_rim", "RIM", true, out tV); r.y += 31;
            r = target.DRAW_BG_TEXTURE(r, "Gradient", "_RimGradient", GUI.enabled, out tV, null); r.y += r.height;
            r = target.DRAW_SLIDER_WITHTOGGLE(r, "Blend", "_RIM_BLEND", 0, 1, GUI.enabled, "USE_RIM_BLEND"); r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H * 2;
            if (target.target.compiler.IsKeywordEnabled("SKIP_FRESNEL_CALCULATION")) EditorGUI.HelpBox(r, "Fresnel Disabled", MessageType.None);
            else EditorGUI.HelpBox(r, "Using Fresnel Channel", MessageType.None);
            r.y += r.height;
            r = target.DRAW_SLIDER(r, "Rim Blend", "RIM_Plus", 0, 1, GUI.enabled, div: 100); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Rim Pow", "RIM_Minus", 0, 256, GUI.enabled); r.y += r.height;
            //  target.DRAW_TOGGLE(r, "Inverse", "RIM_INVERSE", GUI.enabled, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "Shore Affect", "RIM_SCHORE_SKIP", GUI.enabled, out tV); r.y += 31;
            
            
            target.DRAW_TOGGLE(r, "Rim Debug", "RIM_DEBUG", GUI.enabled, out tV); r.y += 31;
            
            GUI.enabled = true;
            
            r.y += 10;
            r.x += r.width + 14;
            r.y = 0;
            
            GUI.enabled = target.target.compiler.IsKeywordEnabled("USE_SHADOWS") && target.IS_OPAQUE;
            target.DRAW_TOGGLE(r, "Shadows", null, GUI.enabled, out tV); r.y += 31;
            r = target.DRAW_SLIDER(r, "Shadows Amount", "_ShadorAmount", 0, 1, GUI.enabled); r.y += r.height;
            GUI.enabled = true;
            
            r.y += FastWaterModel20ControllerEditor.H;
            r.height = FastWaterModel20ControllerEditor.H * 3;
            EditorGUI.HelpBox(r, "Is case, if you don't use fullscreen effects, you may apply LUT gradient here", MessageType.None); r.y += r.height;
            GUI.enabled = target.DRAW_TOGGLE(r, "Use LUT's", "USE_LUT", GUI.enabled, out tV); r.y += 31;
            r = target.DRAW_BG_TEXTURE(r, null, "_Lut2D", GUI.enabled, out tV, null); r.y += r.height;
            if (GUI.enabled)
            {   var t = target.target.compiler.GetTexture("_Lut2D");
                if (!t)
                {   t = ASSIGN_LUT();
                    if (t)
                    {   target.target.Undo();
                        target.target.compiler.SetTexture("_Lut2D", t);
                        target.target.SetDirty();
                    }
                }
                if (t)
                {   var tp = new Vector4(1f / t.width, 1f / t.height, t.height - 1f, 0);
                    if (tp != target.target.compiler.GetVector("_Lut2D_params"))
                    {   target.target.Undo();
                        target.target.compiler.SetVector("_Lut2D_params", tp);
                        target.target.SetDirty();
                    }
                }
                
            }
            
            
            r = target.DRAW_SLIDER(r, "Amount", "_LutAmount", 0, 1, GUI.enabled); r.y += r.height;
            target.DRAW_TOGGLE(r, "HQ Mode", "LUT_HQ_MODE", GUI.enabled, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "Fix Overexposure", "FIX_OVEREXPO", GUI.enabled, out tV); r.y += 31;
            r.height = FastWaterModel20ControllerEditor.H * 3;
            EditorGUI.HelpBox(r, "Enable Fix if there're wrong colors near the overexposured pixels", MessageType.None);
            
            
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
    }
    
    
}
}
#endif
