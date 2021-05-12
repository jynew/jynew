
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {

    POP_CLASS __pop_animatedmaintex;
    public POP_CLASS pop_animatedmaintex
    {   get
        {   return __pop_animatedmaintex ?? (__pop_animatedmaintex = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "USE_CROSSANIMATED_MAINTEX", "USE_CROSSANIMATED4X_MAINTEX", "USE_BLENDANIMATED_MAINTEX" },
                contents = new string[] { "Static Texture", "2x Moving Texture", "4x Moving Texture", "Animated Texture" },
                defaultIndex = 0
            });
        }
    }
    
    
    POP_CLASS __pop_tex_channel;
    public POP_CLASS pop_tex_channel
    {   get
        {   return __pop_tex_channel ?? (__pop_tex_channel = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "TEXTURE_CHANNEL_G", "TEXTURE_CHANNEL_B", "TEXTURE_CHANNEL_RGB" },
                contents = new string[] { "R", "G", "B", "Color RGB" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    public class D_Texture : IDrawable {
    
        internal void DRAW_ULTRAFAST_MODE(ref Rect r)
        {   GUI.enabled = true;
        
            var useTxture = target.DRAW_TOGGLE(r, "Surface Texture", "SKIP_MAIN_TEXTURE", GUI.enabled, out tV) & GUI.enabled; r.y += 31;
            if (!useTxture)
            {   r = target.DRAW_COLOR(r, "Replace Color", "_ReplaceColor", true, out tV, false); r.y += r.height;
            
            }
            else
            {   GUI.enabled = useTxture;
                target.DRAW_TOGGLE(r, "images_surfacefoam", null, useTxture, out tV); r.y += 31;
                
                r = target.DRAW_COLOR(r, "Texture Color", "_ReplaceColor", useTxture, out tV, false); r.y += r.height;
                
                r = target.DRAW_SLIDER(r, "Bright", "MAIN_TEX_Bright", -30, 30f, useTxture); r.y += r.height;
                target.DRAW_TOGGLE(r, "Clamp Bright", "CLAMP_BRIGHT", useTxture, out tV); r.y += 31;
                
                
                r = target.DRAW_SLIDER(r, "Gamma", "MAIN_TEX_Amount", 0, 30f, useTxture); r.y += r.height;
                
                if (!target.MODE_MINIMUM)
                {   r = target.DRAW_SLIDER(r, "Contrast", "MAIN_TEX_Contrast", 0, 200f, useTxture); r.y += r.height;
                
                    var uvh = target.DRAW_TOGGLE(r, "Vertex Height", "SKIP_MAINTEX_VHEIGHT", useTxture && !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_3DVERTEX_ANIMATION), out tV); r.y += 31;
                    if (uvh)
                    {   r = target.DRAW_DOUBLEFIELDS(r, "Amount/Offset", new[] { "MAINTEX_VHEIGHT_Amount", "MAINTEX_VHEIGHT_Offset" },
                                                     new[] { 0f, 0 }, new[] { 10f, 10 }, useTxture && !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_3DVERTEX_ANIMATION)); r.y += r.height;
                    }
                }
                
                GUI.enabled =  useTxture;
                target.DRAW_SPEC_DISOVE(ref r, "_APPLY_REFR_TO_TEX_DISSOLVE_FAST");
                
                r.y += FastWaterModel20ControllerEditor.H;
                if (target.MODE_ULTRA_FAST)
                {   r.x = 100;
                    r.y = 0;
                }
                
                r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._MainTex) as Texture2D, useTxture, null); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Use Channel", MessageType.None); r.y += r.height;
                target.pop_tex_channel.DrawPop(null, r); r.y += r.height;
                
                r = target.DRAW_VECTOR(r, "Tile X/Y", "MAIN_TEX_Tile", 0.0001f, 10000, true); r.y += r.height;
                r = target.DRAW_VECTOR_WITHTOGGLE(r, "Moving Speed", "MAIN_TEX_Move", -100, 100f, useTxture, "MAINTEX_HAS_MOVE", div: 10); r.y += r.height;
                if (!target.MODE_MINIMUM)
                {   r = target.DRAW_SLIDER(r, "Distortion", "MAIN_TEX_Distortion", 0, 20f, useTxture); r.y += r.height;
                }
                r = target.DRAW_SLIDER(r, "LQ Distortion", "MAIN_TEX_LQDistortion", 0, 1, useTxture, div: 10); r.y += r.height;
                if (!target.MODE_MINIMUM)
                {   var difm = target.DRAW_TOGGLE(r, "Distort if Moving", "MAIN_TEX_ADDDISTORTION_THAN_MOVE", useTxture, out tV); r.y += 31;
                    if (difm)
                    {   r = target.DRAW_SLIDER(r, null, "MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount", 0, 1000f, useTxture, div: 0.01f); r.y += r.height;
                    }
                }
                
                
                
                
                
                
                
                
                // r.y += FastWaterModel20ControllerEditor.H;
                if (target.pop_animatedmaintex.VALUE == 0)
                {
                }
                else
                {
                
                }
                
                
                
                
                
                if (!target.MODE_MINIMUM)
                
                
                {   r.x = 200;
                    r.y = 0;
                    
                    if (target.MODE_ULTRA_FAST)
                    {   r.x = 200;
                        r.y = 0;
                    }
                    
                    r.height = FastWaterModel20ControllerEditor.H;
                    r.height = 31;
                    EditorGUI.HelpBox(r, "Surface Animation", MessageType.None); r.y += r.height;
                    r.height = FastWaterModel20ControllerEditor.H;
                    target.pop_animatedmaintex.DrawPop(null, r); r.y += r.height;
                    if (target.pop_animatedmaintex.VALUE == 1 || target.pop_animatedmaintex.VALUE == 2)
                    {   r = target.DRAW_VECTOR(r, "Anim Speed", "MAIN_TEX_CA_Speed", 0, 30f, useTxture, div: 10); r.y += r.height;
                    }
                    if (target.pop_animatedmaintex.VALUE == 3)
                    {   r = target.DRAW_SLIDER(r, "Anim Speed", "MAIN_TEX_BA_Speed", 0, 30f, useTxture); r.y += r.height;
                    
                        r = target.DRAW_SLIDER(r, "Mix Tile", "_LOW_DISTOR_MAINTEX_Tile", 0.001f, 100f, GUI.enabled); r.y += r.height;
                        r = target.DRAW_SLIDER(r, "Mix Speed", "_LOW_DISTOR_MAINTEX_Speed", 0, 100f, GUI.enabled); r.y += r.height;
                        r = target.DRAW_SLIDER(r, "Mix Offset", "MAIN_TEX_MixOffset", -1, 1, GUI.enabled, div: 10); r.y += r.height;
                        r = target.DRAW_SLIDER(r, "Mix Multy", "MAIN_TEX_Multy", -10, 10, GUI.enabled); r.y += r.height;
                    }
                    
                    
                    /*  var oc = GUI.color;
                      GUI.color = new Color32(208, 172, 63, 255);
                      r.height = FastWaterModel20ControllerEditor.H * 2;
                    EditorGUI.HelpBox(r, "Low Distortion Params", MessageType.None); r.y += r.height;*/
                    
                    //  GUI.color = oc;
                }
                
                r.y += FastWaterModel20ControllerEditor.H;
                if (target.MODE_ULTRA_FAST)
                {   r.x = 300;
                    r.y = 0;
                }
                target.DRAW_TOGGLE(r, "Debug Texture", "DEBUG_TEXTURE", useTxture, out tV); r.y += 31;
                
            }
            
            
            
            
            
            GUI.enabled = true;
        }
    }
}
}
#endif