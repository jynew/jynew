using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Collections;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace EModules.FastWaterModel20 {
public partial class FastWaterModel20Controller : MonoBehaviour {



    Dictionary<Material, MaterialSetter> setter_cache = new Dictionary<Material, MaterialSetter>();
    private void OnPreRenderMy(Camera camera)
    /* { }
     void OnWillRenderObject()*/
    //private void OnPreRender()
    {   //m_renderCamera = Camera.current;
        //if (!MainRender(camera)) return;
        // if (Application.isPlaying && !visible) return;
        if (!camera || camera.gameObject.layer == 7)
            return;
        if (  !__this || !enabled)
        {   Camera.onPreRender -= OnPreRenderMy;
            return;
        }
        if (!IsVisible)
            return;
            
            
            
        m_renderCamera = camera;
        if (cameraCounterPreRender)
        {   cameraCounterPreRender = false;
            ++CameraIndexPreRender;
        }
        
        /*int count = 0;
        foreach (var item in cache_GET_RENDER_TEXTURE) {
          foreach (var v2 in item.Value) {
            count += v2.Valu;
          }
        }
        Debug.Log( count );*/
        
        
        
        
        #if UNITY_EDITOR
        if (!compiler)
            return;
        if (!compiler.material)
            return;
        if (!RENDERER)
            return;
        if (!RENDERER.sharedMaterial)
            return;
            
        //RestoreMat();
        
        TryCreateTempMaterials();
        
        
        if (CameraIndexPreRender == -1)
            return;
        //CopyMat( material, editor_cache_materials[this] );
        //EditorJsonUtility.FromJsonOverwrite( EditorJsonUtility.ToJson( material ), editor_cache_materials[this] );
        
        /* if (restoreMat != RE_NDERER.sharedMaterial && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(RE_NDERER.sharedMaterial)))
         {
             restoreMat = RE_NDERER.sharedMaterial;
             SetDirty(this);
         }*/
        //__compiler.OverrideMaterial = RE_NDERER.sharedMaterial = editor_cache_materials[gameObject];
        
        #endif
        
        
        // var M = RENDERER.sharedMaterial;
        
        if (!setter_cache.ContainsKey(RENDERER.sharedMaterial))
            setter_cache.Add(RENDERER.sharedMaterial, new MaterialSetter() { RENDERER = RENDERER, material = RENDERER.sharedMaterial, compiler = compiler });
        var M = setter_cache[RENDERER.sharedMaterial];
        
        do
        {   if (!USE_FAKE_LIGHTING)
            {   if (!DirectionLight)
                    break;
                if (Application.isPlaying && m_OldDirection == DirectionLight.forward)
                    break;
                    
                // if (!compiler.HasProperty(_LightDir)) break;
                
                m_OldDirection = DirectionLight.forward;
                // m_v3 = -DirectionLight.TransformDirection( 0, 0, 1 ).normalized;
                m_v3 = DirectionLight.forward;
                m_v4.Set(m_v3.x, m_v3.y, m_v3.z, 0);
                M.SetVector(_LightDir, m_v4);
            }
            else
            {   if (!FakeLight)
                    break;
                if (!__FakeLightComponent)
                    __FakeLightComponent = FakeLight.GetComponent<FastWaterModel20FakeLight>();
                if (!__FakeLightComponent)
                    break;
                if (Application.isPlaying && m_OldDirection == FakeLight.forward && m_OldColor == __FakeLightComponent.color)
                    break;
                    
                    
                // if (!compiler.HasProperty(_LightDir)) break;
                // if (!compiler.HasProperty(_LightColor0Fake)) break;
                
                m_OldDirection = FakeLight.forward;
                m_v3 = FakeLight.TransformDirection(0, 0, 1).normalized;
                m_v4.Set(m_v3.x, m_v3.y, m_v3.z, 0);
                M.SetVector(_LightDir, m_v4);
                
                m_OldColor = __FakeLightComponent.color;
                M.SetColor(_LightColor0Fake, __FakeLightComponent.color);
            }
        } while (false);
        
        
        
        
        
        // if (!lastOrto.ContainsKey(M)) lastOrto.Add(M, null);
        // if (!needOrto.ContainsKey(camera)) needOrto.Add(camera, false);
        
        bool CHECKORTO = M.lastOrto != m_renderCamera.orthographic;
        #if UNITY_EDITOR
        if (!Application.isPlaying)
            CHECKORTO = true;
        #endif
            
            
        if (CHECKORTO)
        {   M.lastOrto = m_renderCamera.orthographic;
        
        
        }
        
        
        //
        
        
        /*   if (IsKeywordEnabled(ULTRA_FAST_MODE))
             {
                 var res = bakedRotation ?? (bakedRotation = transform.rotation.eulerAngles.y * Mathf.Deg2Rad).Value;
                 if (!Application.isPlaying) res = transform.rotation.eulerAngles.y / 180 * Mathf.PI;
                 M.SetFloat(_ObjecAngle, 0);
             }*/
        // Debug.Log((Application.isPlaying ? (Time.time / 16) : ((float)((EditorApplication.timeSinceStartup / 16) % (512 / 16)))) % (512 / 16));
        
        
        DO_FRACTIME(M);
        
        
        
        
        
        
        if (Application.isPlaying)
            M.SetVector(_ObjectScale, localScale ?? (localScale = transform.localScale).Value);
        else
            M.SetVector(_ObjectScale, transform.localScale);
            
        if (M.lastOrto == true)
        {   //Debug.Log(camera.gameObject.layer + " " + camera);
            M.SetFloat(_MyNearClipPlane, m_renderCamera.nearClipPlane);
            M.SetFloat(_MyFarClipPlane, m_renderCamera.farClipPlane);
        }
        // return;
        if (IsKeywordEnabled(ALLOW_MANUAL_DEPTH) && (m_renderCamera.depthTextureMode & DepthTextureMode.Depth) == 0)
        {   //lastDepth = camera;
            m_renderCamera.depthTextureMode |= DepthTextureMode.Depth;
            //  Debug.Log(gameObject.name);
        }
        
        //if (!IsKeywordEnabled(SKIP_3DVERTEX_ANIMATION))
        {   /* VertexMin.Set(VertexSize.x, 0, VertexSize.y);
             VertexMax.Set(VertexSize.z, 0, VertexSize.w);
             VertexMin = transform.TransformPoint(VertexMin);
             VertexMax = transform.TransformPoint(VertexMax);
             VertexSizeResult.Set( VertexMin.x, VertexMin.z, -VertexMin.x + VertexMax.x, -VertexMin.z + VertexMax.z);*/
            M.SetVector(_VertexSize, VertexSize);
            
        }
        
        ///UpdateMaterials();
        ///UpdateMaterials();
        if (IsKeywordEnabled(REFLECTION_PLANAR))
        {   DoPlanarReflection(Mathf.RoundToInt(compiler.GetFloat(_ReflectionTex_temp_size)));
        }
        
        //refraction
        if (IsKeywordEnabled(REFRACTION_BAKED_FROM_TEXTURE))
        {   //external
            M.SetTexture(_RefractionTex, backed_refraction);
        }
        if (IsKeywordEnabled(REFRACTION_BAKED_VIA_SCRIPT) || IsKeywordEnabled(REFRACTION_BAKED_ONAWAKE))
        {   //internal
            var t = GET_RENDER_TEXTURE(BakeOrUpdateType.Refraction) ?? emptyTexture;
            // var gett = GetComponent<FastWaterModel20ManualBaking>().t1;
            // Shader.SetGlobalTexture( _RefractionTex_temp, gett );
            // material.SetTexture( _RefractionTex_temp, gett );
            /*#if !TEST_GLES20
                            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 && Application.isPlaying)
            #else
                            if (true)
            #endif*/
            //#if USE_TEX2D_CONVERTER
            if (UseTextureConverter)
                M.SetTexture(_RefractionTex_temp, t.texture_gles20);
            //#else
            else
                M.SetTexture(_RefractionTex_temp, t.texture);
            //#endif
        }
        //zdepth
        if (IsKeywordEnabled(BAKED_DEPTH_EXTERNAL_TEXTURE))
        {   //external
        
            M.SetTexture(_BakedData, backed_zdepth);
        }
        
        
        if (IsKeywordEnabled(BAKED_DEPTH_VIASCRIPT) || IsKeywordEnabled(BAKED_DEPTH_ONAWAKE) || SECOND_DEPTH_NEED())
        {   //internalAS
            var t = GET_RENDER_TEXTURE(BakeOrUpdateType.ZDepth) ?? emptyTexture;
            /*#if !TEST_GLES20
                            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 && Application.isPlaying)
            #else
                            if (true)
            #endif*/
            //#if USE_TEX2D_CONVERTER
            if (UseTextureConverter)
                M.SetTexture(_BakedData_temp, t.texture_gles20);
            //#else
            else
                M.SetTexture(_BakedData_temp, t.texture);
            //#endif
        }
        
        // camera.clearFlags = CameraClearFlags.Nothing;
        // GameObject.FindObjectsOfType<Text>().First(t => t.gameObject.name == "LOG").text = M.lastDownSamling.ToString();
        // M. material.EnableKeyword("WATER_DOWNSAMPLING");
        // M.SetFloat(DOWNSAMPLING_SAMPLE_SIZE, 0.5f);
        
        if (M.lastDownSamling/* && CameraIndexPreRender != -1 && camera.targetTexture*/ != 0)
        {   var buffer = FastWaterModel20FrameBuffer.AddRequestorRuntime(this, camera, IsKeywordEnabled(FORCE_OPAQUE) );
            /* if (buffer.LastTexture)
             {   M.SetTexture(_LastFrame, buffer.LastTexture);
             }*/
            if (buffer.LastTexture2)
            {
            
                Shader.SetGlobalTexture(_LastFrame2, buffer.LastTexture2);
                // M.SetTexture(_LastFrame2, buffer.LastTexture2);
            }
            
            switch (M.lastDownSamling)
            {   //case 1:  M.SetFloat(DOWNSAMPLING_SAMPLE_SIZE, 0.5f); break;
                case 1:  M.SetFloat(DOWNSAMPLING_SAMPLE_SIZE, 0.25f); break;
                case 2:  M.SetFloat(DOWNSAMPLING_SAMPLE_SIZE, 0.5f); break;
                case 3:  M.SetFloat(DOWNSAMPLING_SAMPLE_SIZE, 0.75f); break;
            }
        }
        /* var trt = GET_RENDER_TEXTURE(BakeOrUpdateType.LastFrame, camera.targetTexture.width, heightSize: camera.targetTexture.height, camera: CameraIndexPreRender).texture;
             M.SetTexture(_LastFrame, trt);
        
             Debug.Log(camera.targetTexture);*/
        //Debug.Log(camera.targetTexture);
        //
        ///UpdateMaterials();
        ///UpdateMaterials();
        
        // __compiler.OverrideMaterial = null;
        
    }
    
    RenderTexturePair emptyTexture = new RenderTexturePair();
    
    
    
    
    
    
    
    
    
    
    
    private void OnWillRenderObject()
    {   cameraCounterPreRender = true;
        cameraCounterPostRender = true;
    }
    [System.NonSerialized]
    static bool cameraCounterPreRender = true;
    static bool cameraCounterPostRender = true;
    RenderTexture trt;
    private void OnPostRenderMy(Camera camera)
    {
    
    
        if (!camera || camera.gameObject.layer == 7)
            return;
        if ( !__this || !enabled)
        {   Camera.onPostRender -= OnPostRenderMy;
            return;
        }
        if (cameraCounterPostRender)
        {   cameraCounterPostRender = false;
            ++CameraIndexPostRender;
        }
        /* var M = setter_cache[RENDERER.sharedMaterial];
         M.SetTexture(_LastFrame, camera.activeTexture);
         M.SetTexture(Shader.PropertyToID("_LastFrame2"), RenderTexture.active);*/
        
        /* if (compiler.UseBufferOptimization && CameraIndexPostRender != -1 && camera.targetTexture)
         {   var trt = GET_RENDER_TEXTURE(BakeOrUpdateType.LastFrame, camera.targetTexture.width, heightSize: camera.targetTexture.height, camera: CameraIndexPostRender).texture;
             trt.DiscardContents();
             Graphics.Blit(camera.targetTexture, trt);
        
         }*/
        
        
        //Debug.Log( "OnPostRender" );
        // RestoreMat();
        /*if (!setter_cache.ContainsKey(RENDERER.sharedMaterial))
            setter_cache.Add(RENDERER.sharedMaterial, new MaterialSetter() { material = RENDERER.sharedMaterial, compiler = compiler });
        var M = setter_cache[RENDERER.sharedMaterial];*/
        /*  if (!cameraCounter)
          {   cameraCounter = true;
              CameraIndex = 0;
          }
          ++CameraIndex;
        
        
          var trt = GET_RENDER_TEXTURE(BakeOrUpdateType.LastFrame, camera.targetTexture.width, heightSize: camera.targetTexture.height, camera: CameraIndex).texture;
          Graphics.CopyTexture(camera.targetTexture, trt);
          compiler.material.SetTexture(_LastFrame, trt);*/
        
        if (OnPortRenderAction != null)
        {   OnPortRenderAction();
            OnPortRenderAction = null;
        }
    }
    
    
    
}
}
