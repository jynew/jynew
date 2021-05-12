
#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EModules.FastWaterModel20 {
partial class FastWaterModel20ControllerEditor : Editor {



    POP_CLASS __pop_lightblend;
    public POP_CLASS pop_lightblend
    {   get
        {   return __pop_lightblend ?? (__pop_lightblend = new POP_CLASS()
            {   target = this,
                    keys = new[] { "LIGHTING_BLEND_SIMPLE", "LIGHTING_BLEND_COLOR", "LIGHTING_BLEND_OVERLAY" },
                contents = new string[] { "Simple Add", "Use Color", "BG Overlay" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    
    POP_CLASS __pop_flatspec;
    public POP_CLASS pop_flatspec
    {   get
        {   return __pop_flatspec ?? (__pop_flatspec = new POP_CLASS()
            {   target = this,
                    keys = new[] { null, "USE_FLAT_OWN" },
                contents = new string[] { "Material Normals", "Own Normals" },
                defaultIndex = 0
            });
        }
    }
    
    
    
    
    public int DRAW_MENU(ref Rect _r, string currentItemkey, string[] itemsNames, string[] itemsTexture)
    {   var r = _r;
        r.height = FastWaterModel20ControllerEditor.H * 2;
        int currentItem = EditorPrefs.GetInt(currentItemkey);
        GUIContent content = new GUIContent();
        
        r.width /= itemsNames.Length;
        var resetX = r.x;
        
        
        for (int i = 0; i < itemsNames.Length; i++)
        {   // var style = i == 0 ? EditorStyles.miniButtonLeft : i == itemsNames.Length - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid;
            // var style = EditorStyles.helpBox;
            var style = currentItem == i ? EditorStyles.helpBox : GUI.skin.button;
            content.text = "";
            var buttonRect = r;
            #if SHOW_NOISE_CATEGORY
            if (i == 3) buttonRect = SHRINK(buttonRect, 10);
            #endif
            // buttonRect.width = buttonRect.height;
            buttonRect.width = 120;
            if (GUI.Button(buttonRect, content, style))
            {   EditorPrefs.SetInt(currentItemkey, i);
                currentItem = i;
            }
            if (i != itemsNames.Length - 1 && arrows[currentItem, i])
            {   //var v = new Vector2(buttonRect.x + buttonRect.width + 10, buttonRect.y + buttonRect.height / 2);
                //  DrawArrowedLine(v, Offset_X(v, 6), 2, true);
            }
            
            if (currentItem == i && Event.current.type == EventType.Repaint) style.Draw(buttonRect, content, true, true, true, false);
            DRAW_MASKEDTEXTURE(itemsTexture[i], SHRINK(buttonRect, 4), true);
            buttonRect.y += buttonRect.height;
            buttonRect.height = FastWaterModel20ControllerEditor.H;
            GUI.Label(buttonRect, itemsNames[i]);
            
            
            
            r.x += r.width + 5;
        }
        _r.y += r.height + FastWaterModel20ControllerEditor.H + 10;
        return currentItem;
    }
    
    
    public class D_Lightin : IDrawable {
    
        override public void Draw(Rect input, out Vector2[] output)
        {   output = new Vector2[1];
            if (!target.target.compiler || !target.target.compiler.material) return;
            for (int i = 0; i < output.Length; i++) output[i] = Vector2.zero;
            LAST_GROUP_R = new Rect(input.x, input.y, 390, target.MENU_CURRENT_HEIGHT());
            GUI.BeginGroup(LAST_GROUP_R);
            var r = new Rect(0, 0, BOTTOM_W, 0);
            var OGE = GUI.enabled;
            var oc = GUI.color;
            
            
            
            if (!target.MODE_ULTRA_FAST)
            {   DRAW_LIGHT(ref r);
                r.x = 100;
                r.y = 0;
                DRAW_ADVANCE_MODE(ref r);
            }
            else
            {   var currentItemkey = "EModules/Water20/SurfaceMenuItem";
                var itemsNames = new[] { "Vertices", "Surface",    "Lighting",
                                       };
                var itemsTexture = new[] { "images_vertices", "images_surfacefoam",  "images_lamp",
                                         };
                r.width = 390;
                switch (target.DRAW_MENU(ref r,  currentItemkey, itemsNames, itemsTexture))
                {   case 0:
                        target.d_D_Vertices.target = target;
                        Vector2[] draw_output;
                        target.d_D_Vertices.Draw(new Rect(r.x, r.y, 390, 0), out draw_output);
                        break;
                    case 1:
                        r.height  = 2000;
                        GUI.BeginGroup(r);
                        r.width = BOTTOM_W;
                        r.y = 0;
                        target.d_D_Texture.target = target;
                        target.d_D_Texture.DRAW_ULTRAFAST_MODE(ref r);
                        GUI.EndGroup();
                        
                        break;
                    case 2:
                        r.height  = 2000;
                        GUI.BeginGroup(r);
                        r.y = 0;
                        r.width = BOTTOM_W;
                        DRAW_LIGHT(ref r);
                        GUI.EndGroup();
                        break;
                }
                
                
            }
            
            
            
            
            GUI.color = oc;
            GUI.enabled = OGE;
            LAST_GROUP_R = Rect.zero;
            GUI.EndGroup();
        }
        
        
        
        void DRAW_LIGHT(ref Rect r)
        {   //DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100, ACTIVE_COLOR );
            // DrawArrowedLine( fullRect.x, fullRect.y, fullRect.x + 100, fullRect.y + 100 , PASSIVE_COLOR );
            // DRAW_TEXTURE("i_RGB", fullRect);
            #region LIGTING
            GUI.enabled = !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_SPECULAR) ||
                          !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_LIGHTING) ||
                          !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_FLAT_SPECULAR);
                          
            Component test = !target.target.USE_FAKE_LIGHTING ? (target.target.DirectionLight == null ? null : target.target.DirectionLight.GetComponent<Light>() as Component) :
                             (target.target.FakeLight == null ? null : target.target.FakeLight.GetComponent<FastWaterModel20FakeLight>());
            var og = GUI.enabled;
            //GUI.enabled = spec_enable || flatspec_enable || mt_enable;
            
            r.x = 300;
            r.y = 0;
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 0;
                r.y = 0;
            }
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Light Type", test ? MessageType.None : MessageType.Error);
            r.y += r.height;
            var oldI = target.target.USE_FAKE_LIGHTING ? 1 : 0;
            var newI = EditorGUI.Popup(r, oldI, new[] { "Natural Light", "Fake Light" });
            if (oldI != newI)
            {   target.target.Undo();
                target.target.USE_FAKE_LIGHTING = newI == 1;
                target.target.SetDirty();
            }
            r.y += r.height;
            var oc = GUI.color;
            GUI.color *= new Color32(255, 245, 181, 255);
            
            
            if (!target.target.USE_FAKE_LIGHTING)
            {   var old_l = target.target.DirectionLight == null ? null : target.target.DirectionLight.GetComponent<Light>();
                var oc2 = GUI.color;
                if (GUI.enabled && !old_l) GUI.color = Color.red;
                var l = EditorGUI.ObjectField(r, old_l, typeof(Light), true) as Light;
                GUI.color = oc2;
                if (l != old_l)
                {   target.target.Undo();
                    target.target.DirectionLight = l == null ? null : l.transform;
                    target.target.SetDirty();
                }
            }
            else
            {   var old_l = target.target.FakeLight == null ? null : target.target.FakeLight.GetComponent<FastWaterModel20FakeLight>();
                var oc2 = GUI.color;
                if (GUI.enabled && !old_l) GUI.color = Color.red;
                var l = EditorGUI.ObjectField(r, old_l, typeof(FastWaterModel20FakeLight), true) as FastWaterModel20FakeLight;
                GUI.color = oc2;
                if (l != old_l)
                {   target.target.Undo();
                    target.target.FakeLight = l == null ? null : l.transform;
                    target.target.SetDirty();
                }
                r.y += r.height;
                if (GUI.Button(r, "Create Fake"))
                {   var go = new GameObject("FastWaterModel20FakeLight", typeof(FastWaterModel20FakeLight));
                    go.transform.SetParent(target.target.transform);
                    go.transform.position = target.target.transform.position;
                    UnityEditor.Undo.RegisterCreatedObjectUndo(go, "FastWaterModel20FakeLight");
                    target.target.Undo();
                    target.target.FakeLight = go.transform;
                    target.target.SetDirty();
                    Selection.objects = new[] {(UnityEngine.Object)go};
                }
            }
            // r.y = LINE_R.y;
            // var v2 = DrawLine( tV, Offset_X( tV, 15 ), 3, mt_enable );
            //tV.x = r.x + r.width;
            r.y += r.height;
            
            if (!target.MODE_ULTRA_FAST)
            {   r.height = FastWaterModel20ControllerEditor.H;
                EditorGUI.HelpBox(r, "Blend Mode", MessageType.None);
                r.y += r.height;
                target.pop_lightblend.DrawPop(null, r); r.y += r.height;
                if (target.pop_lightblend.VALUE == 1)
                {   r = target.DRAW_COLOR(r, null, FIELDS._BlendColor, GUI.enabled, out tV, false); r.y += r.height;
                }
            }
            
            
            var useLN = target.DRAW_TOGGLE(r, "Normals Factor", "USE_LIGHTIN_NORMALS", GUI.enabled, out tV) & GUI.enabled; r.y += 31;
            r = target.DRAW_SLIDER(r, null, "_LightNormalsFactor", 0, 20, useLN); r.y += r.height;
            
            /*  r.x = 300;
              r.y = 0;*/
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            
            GUI.enabled = true;
            
            if (!target.MODE_MINIMUM)
            {
            
                var mt_enable = target.DRAW_TOGGLE(r, "images_lamp", FIELDS.SKIP_LIGHTING, true, out tV); r.y += 31;
                
                GUI_TOOLTIP(r, "Enable Light");
                
                GUI.enabled = mt_enable;
                
                //  r.x = tV.x;
                // r.x = DrawArrowedLine(  tV , Offset_X( tV, 10 ), 2, mt_enable ).p2.x;
                var S = 0;
                r = target.DRAW_SLIDER(r, "Light Amount", FIELDS._LightAmount, 0, 20, mt_enable); r.y += r.height + S;
                target.DRAW_TOGGLE(r, "Light Pow", "SKIP_LIGHTING_POW", mt_enable, out tV); r.y += 31;
                
            }
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            
            if (target.MODE_ULTRA_FAST)
            {   r.x = 100;
                r.y = 0;
            }
            
            var spec_enable = target.DRAW_TOGGLE(r, "images_specualr", FIELDS.SKIP_SPECULAR, true, out tV);
            GUI_TOOLTIP(r, "Enable Light Specular"); r.y += 31;
            target.DRAW_TOGGLE(r, "Anisotrop Specular", "SKIP_SPECULAR_ANIZO", spec_enable, out tV); r.y += 31;
            //  r = target.DRAW_DOUBLEFIELDS(r, "Light Specular", new[] { FIELDS._SpecularAmount, FIELDS._SpecularShininess }, new[] { 0f, 0f }, new[] { 30f, 512f }, spec_enable);
            // GUI_TOOLTIP(r, "Amount/Shininess"); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Light Specular", MessageType.None); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", FIELDS._SpecularAmount, 0, 30, spec_enable); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Shininess", FIELDS._SpecularShininess, 0, 512, spec_enable); r.y += r.height;
            
            
            var glow = target.DRAW_TOGGLE(r, "Glow", "SKIP_SPECULAR_GLOW", spec_enable, out tV); r.y += 31;
            if (glow) { r = target.DRAW_SLIDER(r, null, "_SpecularGlowAmount", 0, 1, spec_enable); r.y += r.height; }
            GUI.enabled = spec_enable && target.pop_refraction.VALUE != 0;
            
            r.y += FastWaterModel20ControllerEditor.H;
            if (target.MODE_ULTRA_FAST)
            {   r.x = 200;
                r.y = 0;
            }
            
            
            var flatspec_enable = target.DRAW_TOGGLE(r, "images_specualrwide", FIELDS.SKIP_FLAT_SPECULAR, true, out tV);
            GUI.enabled  = flatspec_enable;
            GUI_TOOLTIP(r, "Enable Wide Specular"); r.y += 31;
            
            if (!target.MODE_ULTRA_FAST)
            {   r.height = FastWaterModel20ControllerEditor.H;
                target.pop_flatspec.DrawPop(null, r); r.y += r.height;
            }
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Flat Specular", MessageType.None); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", FIELDS._FlatSpecularAmount, 0, 10, flatspec_enable, div: 10); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Shininess", FIELDS._FlatSpecularShininess, 0, 512, flatspec_enable); r.y += r.height;
            // r = target.DRAW_DOUBLEFIELDS(r, "Flat Specular", new[] { FIELDS._FlatSpecularAmount, FIELDS._FlatSpecularShininess }, new[] { 0f, 0f }, new[] { 10f, 512 }, flatspec_enable, firstDiv: 10);
            //  GUI_TOOLTIP(r, "Amount/Shininess"); r.y += r.height;
            
            if (target.pop_flatspec.VALUE > 0)
            {   r = target.DRAW_DOUBLEFIELDS(r, "Tile X / Y", new[] { "_FlatFriqX", "_FlatFriqY" }, new[] { 0.1f, 0.1f }, new[] { 10, 10f }, flatspec_enable); r.y += r.height;
            
                var hq = target.DRAW_TOGGLE(r, "High Qulaity", "USE_FLAT_HQ", flatspec_enable, out tV) & flatspec_enable; r.y += 31;
                if (hq)
                {   r = target.DRAW_SLIDER(r, "Blend Multy", "FLAT_HQ_OFFSET", 1, 1000, hq); r.y += r.height;
                }
                
                target.DRAW_TOGGLE(r, "Stars' Style", "FLAT_AS_STARS", flatspec_enable, out tV); r.y += 31;
            }
            
            r = target.DRAW_SLIDER(r, "Angle", "_Light_FlatSpecTopDir", 0, 1, flatspec_enable, div: 100); r.y += r.height;
            
            
            var clampflatspec_enable = target.DRAW_TOGGLE(r, "Shininess Clamp", FIELDS.SKIP_FLAT_SPECULAR_CLAMP, flatspec_enable, out tV) && flatspec_enable;
            GUI_TOOLTIP(r, "Limitation on the direction of light"); r.y += 31;
            r = target.DRAW_SLIDER(r, "FallOff flat value", FIELDS._FlatSpecularClamp, 0, 256, clampflatspec_enable);
            GUI_TOOLTIP(r, "Limitation on the direction of light"); r.y += r.height;
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            if (target.MODE_ULTRA_FAST)
            {   r.x = 300;
                r.y = 0;
            }
            GUI.enabled =  spec_enable || flatspec_enable;
            target.DRAW_SPEC_DISOVE(ref r);
            
            #endregion
            GUI.color = oc;
            
            
            
            GUI.enabled = true;
        }
        
        
        void DRAW_ADVANCE_MODE(ref Rect r)
        {   GUI.enabled = true;
            GUI.enabled = target.DRAW_TOGGLE(r, "Surface Foam", "SKIP_SURFACE_FOAMS", GUI.enabled, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "images_surfacefoam", null, true, out tV); r.y += 31;
            r = target.DRAW_SLIDER(r, "Amount", "_SurfaceFoamAmount", 0, 30, GUI.enabled); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Contrast", "_SurfaceFoamContrast", 1, 600, GUI.enabled); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Offset", MessageType.None); r.y += r.height;
            var refV = new Vector3(0.12065329648999294186660041025867f, 0.98533525466827569191057001711249f, 0.12065329648999294186660041025867f);
            var rightV = new Vector3(0, 1, 0);
            var leftV = (refV - rightV) * 2 + rightV;
            var leng = (rightV - leftV).magnitude;
            
            var current = (Vector3)target.target.compiler.GetVector("_SUrfaceFoamVector");
            var oldV = (rightV - current).magnitude / leng - 0.5f;
            oldV *= 4;
            r = target.DIRECTION(r, null, GUI.enabled, VECTOR: false, leftClamp: -0.5f, rightClamp: 0.5f, value: oldV); r.y += r.height;
            if (target.LAST_DIRECTION_VALUE != oldV)
            {   target.target.Undo();
                target.LAST_DIRECTION_VALUE /= 4;
                var result = Vector3.Lerp(rightV, leftV, target.LAST_DIRECTION_VALUE + 0.5f).normalized;
                target.target.compiler.SetVector("_SUrfaceFoamVector", result.normalized);
                target.target.SetDirty();
                
            }
            
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            GUI.enabled = true;
            GUI.enabled = target.DRAW_TOGGLE(r, "Surface Waves", "SURFACE_WAVES", true, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "images_surfacewaves", null, GUI.enabled, out tV); r.y += 31;
            r = target.DRAW_VECTOR(r, "Tile X / Y", "_SFW_Tile", 0.001f, 100, GUI.enabled); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", "_SFW_Amount", 0, 50, GUI.enabled); r.y += r.height;
            r = target.DRAW_VECTOR(r, "Speed X / Y", "_SFW_Speed", -10, 10, GUI.enabled, div: 1000); r.y += r.height;
            //  r = target.DRAW_SLIDER(r, "Speed", "_SFW_Speed", -10, 10, GUI.enabled, div:10); r.y += r.height;
            r = target.DIRECTION(r, "_SFW_Dir", GUI.enabled); r.y += r.height;
            r = target.DIRECTION(r, "_SFW_Dir1", GUI.enabled); r.y += r.height;
            r.height = FastWaterModel20ControllerEditor.H;
            EditorGUI.HelpBox(r, "Main Texture", MessageType.None); r.y += r.height;
            r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._MainTex) as Texture2D, GUI.enabled); r.y += r.height;
            r = target.DRAW_VECTOR(r, "Texture Tile", "_SFW_Tile", 0.001f, 100, GUI.enabled, true); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Distort", "_SFW_Distort", 0, 50, GUI.enabled); r.y += r.height;
            
            // r = target.DIRECTION(r, "_FoamDirection", foam_enable, VECTOR: false, leftClamp: -1f, rightClamp: 1f); r.y += r.height;
            GUI.enabled = target.DRAW_TOGGLE(r, "Normal Affect", "SKIP_SURFACE_WAVES_NORMALEFFECT", GUI.enabled, out tV); r.y += 31;
            if (GUI.enabled) r = target.DRAW_SLIDER(r, null, "_SFW_NrmAmount", 0, 10, GUI.enabled); r.y += r.height;
            GUI.enabled = target.DRAW_TOGGLE(r, "Debug Surf Waves", "DEBUG_SURFACE_WAVES", GUI.enabled, out tV); r.y += 31;
            
            
            r.y += FastWaterModel20ControllerEditor.H;
            GUI.enabled = true;
            GUI.enabled = target.DRAW_TOGGLE(r, "Surface Fog", "SURFACE_FOG", true, out tV); r.y += 31;
            r = target.DRAW_SLIDER(r, "Tiling", "_SURFACE_FOG_Tiling", 0.01f, 100, GUI.enabled); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", "_SURFACE_FOG_Amount", 0, 50, GUI.enabled); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Speed", "_SURFACE_FOG_Speed", 0, 3, GUI.enabled); r.y += r.height;
            
            
            
            
            r.x = 0;
            r.y = 0;
            
            GUI.enabled = true;
            
            r.height = FastWaterModel20ControllerEditor.H;
            //EditorGUI.HelpBox(r, "Apply Texture", MessageType.None); r.y += r.height;
            var mt_enable = target.DRAW_TOGGLE(r, "Apply Texture", FIELDS.SKIP_MAINTEXTURE, true, out tV); r.y += 31;
            target.DRAW_TOGGLE(r, "images_waves", null, true, out tV);
            //r.x = tV.x;
            r.y += 31;
            // r.x = DrawArrowedLine(  tV , Offset_X( tV, 10 ), 2, mt_enable ).p2.x;
            
            r = DRAW_GRAPHIC(r, 40, target.target.compiler.GetTexture(FIELDS._MainTex) as Texture2D, mt_enable, new Color(1, 1, 1, 1)); r.y += r.height;
            r = target.DRAW_VECTOR(r, "Tile X / Y", FIELDS._MainTexTile, 0, 100, mt_enable); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Amount", FIELDS._MainTexColor, 0, 20, mt_enable, useColor: true); r.y += r.height;
            r = target.DRAW_SLIDER(r, "Distortion", "_MTDistortion", 0, 6, mt_enable); r.y += r.height;
            
            
            r.height = FastWaterModel20ControllerEditor.H;
            GUI.enabled = mt_enable;
            EditorGUI.HelpBox(r, "Blend", MessageType.None); r.y += r.height;
            target.pop_mainblend.DrawPop(null, r); r.y += r.height;
            if (target.pop_mainblend.VALUE == 1)
            {   r = target.DRAW_SLIDER(r, "Blend Amount", "_MAINTEXMASK_Blend", 0, 2, mt_enable); r.y += r.height;
            }
            r = target.DRAW_VECTOR(r, "Move X / Y", FIELDS._MainTexTile, -2, 2, mt_enable, true, div: 100); r.y += r.height;
            // r = target.DRAW_SLIDER( r, "Speed", "MainTexSpeed", 0, 20, mt_enable ); r.y += r.height;
            r.y += FastWaterModel20ControllerEditor.H;
            
            GUI.enabled = true;
            string FEATURE = "MAINTEXMASK";
            string tile = "_MAINTEXMASK_Tile";
            string amount = "_MAINTEXMASK_Amount";
            string min = "_MAINTEXMASK_min";
            string max = "_MAINTEXMASK_max";
            string offset = "_MAINTEXMASK_offset";
            bool enable = mt_enable;
            POP_CLASS pop = null;
            //////////////////////////////
            r = target.DRAW_MASK_PANEL(r, "Mask", null, FEATURE, tile, offset, amount, min, max, "MAINTEX_MASK_DEBUG", ref enable, pop);
            
            //////////////////////////////
            // var v2 = DrawLine( tV, Offset_X( tV, 15 ), 3, mt_enable );
            // r = DRAW_TEXTURE_ALIGNY( "i_RGB", V_to_R( v2 ), mt_enable );
            // tV.x = r.x + r.width;
            // r.x = DrawArrowedLine( tV, Offset_X( tV, VL.x - tV.x ), 3, mt_enable ).x;
            // r.y = LINE_R.y;
            
            
            r.x = 300;
            r.y = 0;
        }
        
        
        
    }
    
    
}
}
#endif
