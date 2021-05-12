
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {
    POP_CLASS __pop_noiseblend;
    public POP_CLASS pop_noiseblend
    {   get
        {   return __pop_noiseblend ?? (__pop_noiseblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "NOISE_BLEND_1", "NOISE_BLEND_2", "NOISE_BLEND_3" },
                contents = new string[] { "Add", "Multiply", "2X Multiply" },
                defaultIndex = 1
            });
        }
    }
    
    POP_CLASS __pop_hqrgb;
    public POP_CLASS pop_hqrgb
    {   get
        {   return __pop_hqrgb ?? (__pop_hqrgb = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.NOISED_GLARE_HQ_R, FIELDS.NOISED_GLARE_HQ_G, FIELDS.NOISED_GLARE_HQ_B, FIELDS.NOISED_GLARE_HQ_A },
                contents = new string[] { "Noise (R)", "Noise (G)", "Noise (B)", "Noise (A)" },
                defaultIndex = 0
            });
        }
    }
    
    
    POP_CLASS __pop_hqaddrgb;
    public POP_CLASS pop_hqaddrgb
    {   get
        {   return __pop_hqaddrgb ?? (__pop_hqaddrgb = new POP_CLASS()
            {   target = this,
                    keys = new[] { "NOISED_GLARE_HQ_ADDMIX_NONE", "NOISED_GLARE_HQ_ADDMIX_R", "NOISED_GLARE_HQ_ADDMIX_G", "NOISED_GLARE_HQ_ADDMIX_B", "NOISED_GLARE_HQ_ADDMIX_A" },
                contents = new string[] { "none", "Noise (R)", "Noise (G)", "Noise (B)", "Noise (A)" },
                defaultIndex = 2
            });
        }
    }
    
    
    
    POP_CLASS __pop_lqrgb;
    public POP_CLASS pop_lqrgb
    {   get
        {   return __pop_lqrgb ?? (__pop_lqrgb = new POP_CLASS()
            {   target = this,
                    keys = new[] { FIELDS.NOISED_GLARE_LQ_R, FIELDS.NOISED_GLARE_LQ_G, FIELDS.NOISED_GLARE_LQ_B, FIELDS.NOISED_GLARE_LQ_A },
                contents = new string[] { "Noise (R)", "Noise (G)", "Noise (B)", "Noise (A)" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    
    
    
    
    
    public class D_Noise : IDrawable {
    
    
    
        override public void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, input.width, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, 90, 0);
            var og = GUI.enabled;
            var S = 0;
            var RES = FastWaterModel20ControllerEditor.H * 4;
            
            var oldC = GUI.color;
            GUI.color *= new Color32(255, 206, 206, 225);
            
            
            {
            
            
            
                EditorGUI.HelpBox(new Rect(0, 0, 390, FastWaterModel20ControllerEditor.H * 2), "This section for tests, and does not work at all", MessageType.Warning);
                
                
                r.height = FastWaterModel20ControllerEditor.H * 2;
                r.y = RES;
                
                GUI.enabled = HAS_NOISED_GLARE(target);
                EditorGUI.HelpBox(r, "Common Noise Texture", MessageType.None); r.y += r.height;
                r = target.DRAW_BG_TEXTURE(r, null, FIELDS._NoiseHQ, GUI.enabled, out tV, null); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Blend Mode", MessageType.None); r.y += r.height;
                target.pop_noiseblend.DrawPop(null, r); r.y += r.height;
                r.y += 10;
                
                GUI.enabled = true;
                var hq_enable = target.DRAW_TOGGLE(r, "NOISE HQ", FIELDS.SKIP_NOISED_GLARE_HQ, true, out tV); r.y += 31;
                var ge = GUI.enabled;
                GUI.enabled = hq_enable;
                
                r.height = FastWaterModel20ControllerEditor.H;
                target.pop_hqrgb.DrawPop(null, r); r.y += r.height;
                r.y += 2;
                EditorGUI.HelpBox(r, "Use Add Mix", MessageType.None); r.y += r.height;
                target.pop_hqaddrgb.DrawPop(null, r); r.y += r.height;
                
                
                // r = target.DRAW_SLIDER( r, "Tile", FIELDS._NHQ_GlareFriq, 0.1f, 100, hq_enable ); r.y += r.height + S;
                //  r = target.DRAW_SLIDER( r, "Amount", FIELDS._NHQ_GlareAmount, 0.1f, 50, hq_enable ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Tile / Amount", new[] { FIELDS._NHQ_GlareFriq, FIELDS._NHQ_GlareAmount }, new[] { 0.1f, 0.1f }, new[] { 100, 50f }, hq_enable); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Speed XY / Z", new[] { FIELDS._NHQ_GlareSpeedXY, FIELDS._NHQ_GlareSpeedZ }, new[] { -3, 0f }, new[] { 3, 5f }, hq_enable); r.y += r.height + S;
                var ne = target.DRAW_TOGGLE(r, "Normal Affect", FIELDS.SKIP_NOISED_GLARE_HQ_NORMALEFFECT, hq_enable, out tV) & hq_enable; r.y += 31;
                r = target.DRAW_SLIDER(r, null, FIELDS._NHQ_GlareNormalsEffect, 0, 2f, ne); r.y += r.height + S;
                var f1 = target.DRAW_TOGGLE(r, "Filtres", FIELDS.SKIP_NOISED_GLARE_HQ_FILTRES, hq_enable, out tV) & hq_enable; r.y += 31;
                //  r = target.DRAW_SLIDER( r, "Contrast", FIELDS._NHQ_GlareContrast, 0, 2.5f, f1 ); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Black Point", FIELDS._NHQ_GlareBlackOffset, -1, 0f, f1 ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Contrast/Clamp", new[] { FIELDS._NHQ_GlareContrast, FIELDS._NHQ_GlareBlackOffset }, new[] { 0f, -1f }, new[] { 1f, 0f }, f1);
                GUI_TOOLTIP(r, "Contrast / Black Point"); r.y += r.height + S;
                GUI.enabled = ge;
            }
            
            r.y = RES;
            r.x += r.width + 10;
            
            
            {   var lq_enable = target.DRAW_TOGGLE(r, "NOISE LOW", FIELDS.USE_NOISED_GLARE_LQ, true, out tV); r.y += 31;
                var ge = GUI.enabled;
                GUI.enabled = lq_enable;
                
                var usetxt = target.DRAW_TOGGLE(r, "Use Own Texture", FIELDS.NOISED_GLARE_LQ_SKIPOWNTEXTURE, lq_enable, out tV) & lq_enable; r.y += 31;
                r = target.DRAW_BG_TEXTURE(r, null, FIELDS._NoiseLQ, usetxt, out tV, null); r.y += r.height;
                r.height = FastWaterModel20ControllerEditor.H;
                GUI.enabled = usetxt;
                target.pop_lqrgb.DrawPop(null, r); r.y += r.height;
                GUI.enabled = lq_enable;
                
                
                // r = target.DRAW_SLIDER( r, "Tile", FIELDS._NE1_GlareFriq, 0.1f, 4, lq_enable ); r.y += r.height + S;
                //  r = target.DRAW_SLIDER( r, "Amount", FIELDS._NE1_GlareAmount, 0.1f, 50, lq_enable ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Tile / Amount", new[] { FIELDS._NE1_GlareFriq, FIELDS._NE1_GlareAmount }, new[] { 0.1f, 0.1f }, new[] { 4, 50f }, lq_enable); r.y += r.height + S;
                //  r = target.DRAW_DOUBLEFIELDS( r, "Speed XY / Z", new[] { FIELDS._NE1_GlareSpeed, FIELDS._NHQ_GlareSpeedZ }, new[] { 0, 0f }, new[] { 3, 5f }, hq_enable ); r.y += r.height + S;
                r = target.DRAW_SLIDER(r, "Speed", FIELDS._NE1_GlareSpeed, -3, 3f, lq_enable); r.y += r.height + S;
                r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Direction", MessageType.None); r.y += r.height;
                var rr = GUI.enabled;
                GUI.enabled = lq_enable;
                /*var oldV = Vector3.Angle(Vector3.forward, v);
                if (Vector3.Dot( Vector3.forward, v ) < 0) oldV = 360 - oldV;
                var newV = EditorGUI.Slider(r,oldV, 0, 360);
                if (oldV != newV) {
                  target.Undo();
                  target.target.material.SetVector( FIELDS._NE1_WavesDirection, (Quaternion.AngleAxis( newV, Vector3.up ) * Vector3.up).normalized );
                  target.SetDirty();
                }*/
                
                // Debug.Log( "___" );
                // Debug.Log( v.x + " " + v.z );
                // Debug.Log( oldV + " " + Vector3.Angle(new Vector3(1,0,0), v ));
                // Debug.Log( v.z + " " + ((float)(System.Math.Sin( oldV * System.Math.PI / 180 )  )));
                //if (oldV < 0) oldV = 360 - oldV;
                r = target.DIRECTION(r, FIELDS._NE1_WavesDirection, lq_enable); r.y += r.height;
                
                GUI.enabled = rr;
                var f2 = target.DRAW_TOGGLE(r, "Filtres", FIELDS.USE_NOISED_GLARE_LQ_FILTRES, lq_enable, out tV) & lq_enable; r.y += 31;
                // r = target.DRAW_SLIDER( r, "Contrast", FIELDS._NE1_GlareContrast, 0, 2.5f, f2 ); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Black Point", FIELDS._NE1_GlareBlackOffset, -1, 0f, f2 ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Contrast/Clamp", new[] { FIELDS._NE1_GlareContrast, FIELDS._NE1_GlareBlackOffset }, new[] { 0f, -1f }, new[] { 2.5f, 0f }, f2);
                GUI_TOOLTIP(r, "Contrast / Black Point"); r.y += r.height + S;
                GUI.enabled = ge;
            }
            
            
            r.y = RES;
            r.x += r.width + 10;
            
            
            {   var wave1_enable = target.DRAW_TOGGLE(r, "NOISE WAVE 1", FIELDS.USE_NOISED_GLARE_ADDWAWES1, true, out tV); r.y += 31;
                var ge = GUI.enabled;
                GUI.enabled = wave1_enable;
                
                r.height = FastWaterModel20ControllerEditor.H;
                // r = target.DRAW_SLIDER( r, "Tile", FIELDS._W1_GlareFriq, 0.1f, 10, wave1_enable ); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Amount", FIELDS._W1_GlareAmount, 0.1f, 50, wave1_enable ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Tile / Amount", new[] { FIELDS._W1_GlareFriq, FIELDS._W1_GlareAmount }, new[] { 0.1f, 0.1f }, new[] { 10, 50f }, wave1_enable); r.y += r.height + S;
                r = target.DRAW_SLIDER(r, "Speed", FIELDS._W1_GlareSpeed, -10, 10f, wave1_enable); r.y += r.height + S;
                // var f4 = target.DRAW_TOGGLE( r, "Filtres", FIELDS.USE_NOISED_GLARE_LQ_FILTRES, wave1_enable, out tV ) & wave1_enable; r.y += 31;
                var f3 = true;
                // r = target.DRAW_SLIDER( r, "Contrast", FIELDS._W1_GlareContrast, 0, 2.5f, f3 ); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Black Point", FIELDS._W1_GlareBlackOffset, -1, 0f, f3 ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Contrast/Clamp", new[] { FIELDS._W1_GlareContrast, FIELDS._W1_GlareBlackOffset }, new[] { 0f, -1f }, new[] { 2.5f, 0f }, f3);
                GUI_TOOLTIP(r, "Contrast / Black Point"); r.y += r.height + S;
                GUI.enabled = ge;
                
                
                
                
                
                GUI.enabled = ge;
                
            }
            
            
            r.y = RES;
            r.x += r.width + 10;
            {   var ge = GUI.enabled;
            
                var prc_hq = target.DRAW_TOGGLE(r, "MEGA HQ TEST", FIELDS.USE_NOISED_GLARE_PROCEDURALHQ, true, out tV); r.y += 31;
                r.height = FastWaterModel20ControllerEditor.H * 2;
                GUI.enabled = prc_hq;
                EditorGUI.HelpBox(r, "Used MainTexture (R)", MessageType.None); r.y += r.height;
                r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._MainTex) as Texture2D, prc_hq, new Color(1, 0, 0, 0)); r.y += r.height;
                
                // r = target.DRAW_SLIDER( r, "Tile", "_PRCHQ_tile", 0.1f, 10, wave1_enable ); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Amount", "_PRCHQ_amount", 0.1f, 50, wave1_enable ); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Tile Tex/Waves", new[] { "_PRCHQ_tileTex", "_PRCHQ_tileWaves" }, new[] { 0.1f, 0.1f }, new[] { 10, 10f }, prc_hq); r.y += r.height + S;
                // r = target.DRAW_DOUBLEFIELDS( r, "Tile / Amount", new[] { "_PRCHQ_tile", "_PRCHQ_amount" }, new[] { 0.1f, 0.1f }, new[] { 10, 50f }, prc_hq ); r.y += r.height + S;
                r = target.DRAW_SLIDER(r, "Amount", "_PRCHQ_amount", 0.1f, 50f, prc_hq); r.y += r.height + S;
                r = target.DRAW_DOUBLEFIELDS(r, "Speed Tex/Waves", new[] { "_PRCHQ_speedTex", "_PRCHQ_speedWaves" }, new[] { -10f, -10f }, new[] { 10, 10f }, prc_hq); r.y += r.height + S;
                // r = target.DRAW_SLIDER( r, "Speed", "_PRCHQ_speed", -10, 10f, prc_hq ); r.y += r.height + S;
                
                target.DRAW_TOGGLE(r, "Normals Affect", FIELDS.SKIP_NOISED_GLARE_SKIP_NOISED_GLARE_PROCEDURALHQ_NORMALEFFECT, prc_hq, out tV); r.y += 31;
                
                
            }
            
            /*{
              var wave2_enable = target.DRAW_TOGGLE( r, "NOISE WAVE 2", FIELDS.USE_NOISED_GLARE_ADDWAWES2, true, out tV); r.y += 31;
              var ge = GUI.enabled;
              GUI.enabled = wave2_enable;
            
              r.height = FastWaterModel20ControllerEditor.H;
              // r = target.DRAW_SLIDER( r, "Tile", FIELDS._W2_GlareFriq, 0.1f, 10, wave2_enable ); r.y += r.height + S;
              //  r = target.DRAW_SLIDER( r, "Amount", FIELDS._W2_GlareAmount, 0.1f, 50, wave2_enable ); r.y += r.height + S;
              r = target.DRAW_DOUBLEFIELDS( r, "Tile / Amount", new[] { FIELDS._W2_GlareFriq, FIELDS._W2_GlareAmount }, new[] { 0.1f, 0.1f }, new[] { 10, 50f }, wave2_enable ); r.y += r.height + S;
              r = target.DRAW_SLIDER( r, "Speed", FIELDS._W2_GlareSpeed, 0, 10f, wave2_enable ); r.y += r.height + S;
              // var f4 = target.DRAW_TOGGLE( r, "Filtres", FIELDS.USE_NOISED_GLARE_LQ_FILTRES, wave2_enable, out tV ) & wave2_enable; r.y += 31;
              var f4 = true;
              // r = target.DRAW_SLIDER( r, "Contrast", FIELDS._W2_GlareContrast, 0, 2.5f, f4 ); r.y += r.height + S;
              // r = target.DRAW_SLIDER( r, "Black Point", FIELDS._W2_GlareBlackOffset, -1, 0f, f4 ); r.y += r.height + S;
              r = target.DRAW_DOUBLEFIELDS( r, "Contrast/Clamp", new[] { FIELDS._W2_GlareContrast, FIELDS._W2_GlareBlackOffset }, new[] { 0f, -1f }, new[] { 2.5f, 0f }, f4 );
              GUI_TOOLTIP( r, "Contrast / Black Point" ); r.y += r.height + S;
              GUI.enabled = ge;
            }*/
            
            
            GUI.color = oldC;
            
            
            GUI.enabled = og;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
    }
    
}
}
#endif