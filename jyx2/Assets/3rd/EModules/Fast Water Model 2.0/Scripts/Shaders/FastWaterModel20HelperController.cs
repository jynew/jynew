#define OVERRIDE_EDITOR_TEX_REMOVER
//#define USE_TEX2D_CONVERTER
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
}

////////////////////////////////////////////////
////////////////////////////////////////////////
////////////////////////////////////////////////

[System.Serializable]
public enum BakeOrUpdateType
{ ZDepth = 0, Refraction = 1, Reflection = 2, LastFrame = 3}

#if UNITY_EDITOR
    [ExecuteInEditMode, CanEditMultipleObjects]
#endif
public partial class FastWaterModel20Controller : MonoBehaviour {

    public const string VERSION = "1.0";
    public const string NAME = "Fast Water Model 2.0";
    
    #if DEBUG_BAKED_TEXTURE
    static FastWaterModel20Controller lastTarget;
    static void sv(SceneView sv)
    {   if (!lastTarget || cache_GET_RENDER_TEXTURE.Count == 0)
            return;
        Handles.BeginGUI();
        //if (screenShot) GUI.DrawTexture( new Rect( 0, 0, screenShot.width / 4f, screenShot.height / 4f ), screenShot );
        var f = cache_GET_RENDER_TEXTURE.First();
        if (f.Value.ContainsKey(BakeOrUpdateType.Refraction))
        {   var DepthRenderTexture = f.Value[BakeOrUpdateType.Refraction].texture;
            if (DepthRenderTexture)
                GUI.DrawTexture(new Rect(0, 0, 256, 256), DepthRenderTexture);
        }
        
        Handles.EndGUI();
        
        // Debug.Log( cache_GET_RENDER_TEXTURE.Count );
    }
    #endif
    
    #if UNITY_EDITOR
    static UnityEngine.SceneManagement.Scene lastScene1;
    static UnityEngine.SceneManagement.Scene lastScene2;
    public static void DestroyAllEditorDatasDeep(bool destroyMats = false)
    {   if (System.Threading.Thread.CurrentThread != mainThread ) return;
    
        if (EditorApplication.isCompiling || EditorApplication.isPaused || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isUpdating
                || !UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().IsValid() || !UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isLoaded) return;
        if (destroyMats)
        {   if (lastScene1 == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()) return;
            lastScene1 = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        }
        else
        {   if (lastScene2 == UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()) return;
            lastScene2 = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        }
        
        
        #if OVERRIDE_EDITOR_TEX_REMOVER
        Resources.FindObjectsOfTypeAll<Texture>().Where(n => (n.hideFlags & HideFlags.HideAndDontSave) != 0 && n.name.StartsWith("EModules/MobileWater")).ToList().ForEach(t => DestroyImmediate(t));
        #endif
        /* var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
         if (scene.IsValid() && scene.isLoaded)
             foreach (var go in scene.GetRootGameObjects())
                 if ((go.hideFlags & HideFlags.HideAndDontSave) != 0 && go.name.StartsWith("_MobileWater")) DestroyImmediate(go);*/
        
        
        if (destroyMats)
        {   var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.IsValid() && scene.isLoaded)
                foreach (var go in scene.GetRootGameObjects())
                    if ((go.hideFlags & HideFlags.HideAndDontSave) != 0 && go.name.StartsWith("_MobileWater"))
                        DestroyImmediate(go);
                        
            Resources.FindObjectsOfTypeAll<Material>().Where(n => n && (n.hideFlags & HideFlags.HideAndDontSave) != 0 && n.shader && ( n.shader.name.StartsWith("EM-X/Fast Water Model") ||
                    n.shader.name.StartsWith("Hidden/InternalErrorShader"))).ToList().ForEach(t => DestroyImmediate(t));
                    
        }
        //   Debug.Log(Resources.FindObjectsOfTypeAll<Material>().First(m => m.name.StartsWith("Fast Water Material - Mobile Sea A 30")).shader.name);
        
    }
    #endif
    
    
    //---------------------------------------------------------------------
    #region//------PUBLIC VARS
    //---------------------------------------------------------------------
    /*#if UNITY_EDITOR
            [System.NonSerialized, HideInInspector]
            public string[] supported_sahders = {
          "EM-X/" + NAME,
          };
    #endif*/
    public Transform DirectionLight = null;
    public Transform __FakeLight = null;
    public FastWaterModel20FakeLight __FakeLightComponent = null;
    
    
    
    //public void BakeOrUpdateTexture(int resolution) { BakeOrUpdateTexture( BakeOrUpdateType.ZDepth, resolution ); }
    //public void BakeOrUpdateTexture(BakeOrUpdateType type) { BakeOrUpdateTexture( type, 1024 ); }
    
    int waterLayer { get { return __waterLyaer ?? (__waterLyaer = LayerMask.NameToLayer("Water")).Value; } }
    int? __waterLyaer;
    
    
    [SerializeField]
    Mesh __BoundBakedMesh;
    [SerializeField]
    Vector3[] __GetBOunds = null;
    [SerializeField]
    Vector3 n_Forward, n_Up, n_tangenta, n_tangentb, n_binormala, n_binormalb;
    Vector3[] GetBounds(Mesh mesh)
    {   if (__GetBOunds != null && __BoundBakedMesh == mesh)
        {   return __GetBOunds;
        }
        
        __BoundBakedMesh = mesh;
        
        var vertices = mesh.vertices;
        var uv = mesh.uv.Select((u, i) => new UV() { uv = u, v = vertices[i] }).ToArray();
        
        //xnot normalized bounds
        var bound_MAX_X_non = WhereMax(/**/WhereMax(uv, t => t.uv.x)/**/, uv2 => uv2.uv.y)[0];
        var bound_MAX_Y_non = WhereMax(/**/WhereMax(uv, t => t.uv.y)/**/, uv2 => uv2.uv.x)[0];
        var bound_MIN_X_non = WhereMin(/**/WhereMin(uv, t => t.uv.x)/**/, uv2 => uv2.uv.y)[0];
        var bound_MIN_Y_non = WhereMin(/**/WhereMin(uv, t => t.uv.y)/**/, uv2 => uv2.uv.x)[0];
        var av_boundsUvCenter = (bound_MAX_X_non.uv + bound_MAX_Y_non.uv + bound_MIN_X_non.uv + bound_MIN_Y_non.uv) / 4;
        var av_boundsVertexCenter = (bound_MAX_X_non.v + bound_MAX_Y_non.v + bound_MIN_X_non.v + bound_MIN_Y_non.v) / 4;
        
        
        //normalized bounds
        var BOUNDS_UV_non = new[] { new Vector2(bound_MAX_X_non.uv.x, bound_MAX_Y_non.uv.y), new Vector2(bound_MIN_X_non.uv.x, bound_MIN_Y_non.uv.y) } .ToArray();
        var F = SymmetryFactorUV(BOUNDS_UV_non, av_boundsUvCenter, new[] { new Vector2(1, 1), new Vector2(0, 0) });
        var BOUNDS_V_non = new[] { bound_MAX_X_non, bound_MAX_Y_non, bound_MIN_X_non, bound_MIN_Y_non } .ToArray();
        var BOUNDS = NormalizeVetex(BOUNDS_V_non, av_boundsVertexCenter, new[] { F[0], F[0], F[1], F[1] });
        
        
        //width height rotation position
        //  var n_Forward = -transform.TransformDirection(mesh.normals.Aggregate((n, n2) => n + n2) / mesh.normals.Length);
        {   var N_XA = BOUNDS.Select(v => v.x).Max();
            var N_XI = BOUNDS.Select(v => v.x).Min();
            var N_ZA = BOUNDS.Select(v => v.z).Max();
            var N_ZI = BOUNDS.Select(v => v.z).Min();
            n_Forward = -mesh.normals[0].normalized;
            n_Up = (-(new Vector3((N_ZA + N_ZI) / 2, 0, N_XA) - new Vector3((N_ZA + N_ZI) / 2, 0, N_XI)).normalized);
            n_tangenta = new Vector3(N_XA, 0, N_ZI);
            n_tangentb = new Vector3(N_XI, 0, N_ZI);
            n_binormala = new Vector3(N_XI, 0, N_ZA);
            n_binormalb = new Vector3(N_XI, 0, N_ZI);
            // n_tangent = transform.TransformPoint(new Vector3(N_XA, 0, N_ZI)) - transform.TransformPoint(new Vector3(N_XI, 0, N_ZI));
            // n_binormal = transform.TransformPoint(new Vector3(N_XI, 0, N_ZA)) - transform.TransformPoint(new Vector3(N_XI, 0, N_ZI));
        }
        
        return __GetBOunds = BOUNDS;
    }
    System. Action OnPortRenderAction = null;
    internal void m_BakeOrUpdateTexture(BakeOrUpdateType type, int resolution) { m_BakeOrUpdateTexture(type, resolution, null); }
    internal void m_BakeOrUpdateTexture(BakeOrUpdateType type, int resolution, string saveDialog)
    {   if (type == BakeOrUpdateType.Reflection)
            return;
            
        if (resolution == 0)
            resolution = 256;
            
        var mf = GetComponent<MeshFilter>();
        var rndr = RENDERER;
        if (!mf || !rndr)
        {  Debug.LogWarning(NAME + " cannot bake " + name + ". there are no MeshFilter or MeshRenderer."); return; }
        var mesh = mf.sharedMesh;
        if (!mesh)
        {   Debug.LogWarning(NAME + " cannot bake " + name + ". there isnt mesh.");  return;
        }
        if (mesh.vertexCount < 2)
        {   Debug.LogWarning(NAME + " cannot bake " + name + ". mesh.vertexCount < 2."); return;
        }
        
        // Vector3 tangent, binormal;
        var BOUNDS = GetBounds(mesh);
        
        var CAMERA_SIZE = 200;
        
        var OF = transform.rotation * n_Forward;
        if (type == BakeOrUpdateType.ZDepth)
            OF *= CAMERA_SIZE;
        else
            OF *= 2;
        var pivot = BOUNDS.Select(b => transform.TransformPoint(b)).Aggregate((v1, v2) => v1 + v2) / BOUNDS.Length - OF;
        
        //camera
        if (!RenderDepthOrRefractionCamera)
        {   RenderDepthOrRefractionCamera = new GameObject("_MobileWaterZDepthCamera", typeof(Camera), typeof(FlareLayer)).GetComponent<Camera>();
            RenderDepthOrRefractionCamera.orthographic = true;
            RenderDepthOrRefractionCamera.useOcclusionCulling = false;
            RenderDepthOrRefractionCamera.allowHDR = false;
            RenderDepthOrRefractionCamera.allowMSAA = false;
            RenderDepthOrRefractionCamera.nearClipPlane = 0;
            RenderDepthOrRefractionCamera.renderingPath = RenderingPath.Forward;
            RenderDepthOrRefractionCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            RenderDepthOrRefractionCamera.gameObject.SetActive(false);
            RenderDepthOrRefractionCamera.depthTextureMode = DepthTextureMode.None;
            RenderDepthOrRefractionCamera.gameObject.layer = 7;
            
        }
        
        
        var user_mask = (int)VectorToMask(compiler.GetVector(type == BakeOrUpdateType.ZDepth ? _ZDepthBakeLayers : _RefractionBakeLayers));
        RenderDepthOrRefractionCamera.cullingMask = user_mask & CulMask;
        RenderDepthOrRefractionCamera.farClipPlane = type == BakeOrUpdateType.ZDepth ? (CAMERA_SIZE * 2) : (CAMERA_SIZE + compiler.BakedRefractionCameraOffset);
        
        //resolution
        var difX = (transform.TransformPoint(n_tangenta) - transform.TransformPoint(n_tangentb)).magnitude;
        var difZ = (transform.TransformPoint(n_binormala) - transform.TransformPoint(n_binormalb)).magnitude;
        if (difX == 0 || difZ == 0)
        {   Debug.LogWarning(NAME + " cannot bake " + name + ". Please check mesh's vertices positions.");
            return;
        }
        //var resolutionX = resolution;
        //var resolutionZ = resolution;
        //if (Mathf.Abs( difX - difZ ) < 0.001) { }
        //else if (difX < difZ) resolutionX = (int)(resolution * (difX / difZ));
        // else if (difZ < difX) resolutionZ = (int)(resolution * (difZ / difX));
        
        //  var oldState = rndr.enabled;
        //var oldState = gameObject.activeSelf;
        // rndr.enabled = false;
        // gameObject.SetActive( false );
        var oldstate = gameObject.layer;
        gameObject.layer = waterLayer;
        
        
        //shaders
        if (compiler.Support128Shader == null)
        {   var sh = Shader.Find("EM-X/Fast Water Model 2.0/Test Support 128 Block Instructions");
            var m = Resources.FindObjectsOfTypeAll<Material>().First(a => a.shader == sh);
            compiler.Support128Shader = m;
            #if UNITY_EDITOR
            SetDirty(compiler);
            #endif
        }
        if (compiler.DepthRenderShader == null)
        {   compiler.DepthRenderShader = Shader.Find("Hidden/EM-X/DepthRender");
            #if UNITY_EDITOR
            SetDirty(compiler);
            #endif
        }
        // if (DepthRenderShader_SecondPass == null) DepthRenderShader_SecondPass = Shader.Find("Hidden/EM-X/DepthRender SecondPass");
        if (compiler.DepthRenderShader_SP == null)
        {   compiler.DepthRenderShader_SP = Shader.Find("Hidden/EM-X/DepthRender SecondPass");
            #if UNITY_EDITOR
            SetDirty(compiler);
            #endif
        }
        if (DepthRenderShader_SecondPass_Matertial == null)
        {   DepthRenderShader_SecondPass_Matertial = new Material(compiler.DepthRenderShader_SP);
            DepthRenderShader_SecondPass_Matertial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (compiler.RefractionRenderShader == null)
        {   compiler.RefractionRenderShader = Shader.Find("Hidden/EM-X/RefractionRender");
            #if UNITY_EDITOR
            SetDirty(compiler);
            #endif
        }
        if (!compiler.DepthRenderShader)
            Debug.LogWarning(NAME + " cannot find 'Hidden/EM-X/DepthRender' shader");
        if (!compiler.RefractionRenderShader)
            Debug.LogWarning(NAME + " cannot find 'Hidden/EM-X/RefractionRender' shader");
            
            
        //render
        var rt = GET_RENDER_TEXTURE(type, resolution);
        /*new RenderTexture(resolutionX, resolutionZ, 24);
        rt.isPowerOfTwo = true;
        rt.hideFlags = HideFlags.DontSave;
        rt.name = "EModules/MobileWater/" + type + "-RenderTexture" + gameObject.name;*/
        //if (rt.lastCamera && rt.lastCamera.targetTexture == rt.texture) rt.lastCamera.targetTexture = null;
        //var oldT = RenderDepthOrRefractionCamera.targetTexture;
        RenderDepthOrRefractionCamera.targetTexture = rt.texture;
        // rt.lastCamera = RenderDepthOrRefractionCamera;
        RenderDepthOrRefractionCamera.orthographicSize = difZ / 2;
        RenderDepthOrRefractionCamera.aspect = difX / difZ;
        RenderDepthOrRefractionCamera.transform.rotation = Quaternion.LookRotation(transform.rotation * n_Forward, transform.rotation * n_Up);
        if (type == BakeOrUpdateType.ZDepth)
            RenderDepthOrRefractionCamera.transform.position = pivot;
        else
            RenderDepthOrRefractionCamera.transform.position = pivot - compiler.BakedRefractionCameraOffset * (transform.rotation * n_Forward);
        RenderDepthOrRefractionCamera.gameObject.SetActive(true);
        
        var f = RenderSettings.fog;
        RenderSettings.fog = false;
        if (type == BakeOrUpdateType.ZDepth)
            RenderDepthOrRefractionCamera.RenderWithShader(compiler.DepthRenderShader, null);
        //else if (type == BakeOrUpdateType.Refraction) RenderDepthOrRefractionCamera.RenderWithShader( RefractionRenderShader, null );
        else if (type == BakeOrUpdateType.Refraction)
            RenderDepthOrRefractionCamera.Render();
        RenderSettings.fog = f;
        
        
        //Debug.Log(gameObject.name + " " + SECOND_DEPTH_NEED() + " " + compiler.IsKeywordEnabled("REFRACTION_BAKED_ONAWAKE"));
        
        //if (compiler.IsKeywordEnabled("SHORE_WAVES") && (!compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH") || compiler.IsKeywordEnabled("ALLOW_MANUAL_DEPTH")))
        if (SECOND_DEPTH_NEED())
        {   if (rt.texture_swap == null || rt.texture.width != rt.texture_swap.width)
            {   DESTORY(rt.texture_swap);
                rt.texture_swap = new RenderTexture(rt.texture);
            }
            if (rt.lastCamera)
                rt.lastCamera.targetTexture = null;
            //  DepthRenderShader_SecondPass_Matertial.SetTexture("_MainTex", rt.texture);
            float[] mistergauss = new double[]
            {
            
                0.000036,    0.000363,   0.001446,    0.002291,    0.001446,    0.000363,    0.000036,
                0.000363,    0.003676,   0.014662,    0.023226,    0.014662,    0.003676,    0.000363,
                0.001446,    0.014662,   0.058488,    0.092651,    0.058488,    0.014662,    0.001446,
                0.002291,    0.023226,   0.092651,    0.146768,    0.092651,    0.023226,    0.002291,
                0.001446,    0.014662,   0.058488,    0.092651,    0.058488,    0.014662,    0.001446,
                0.000363,    0.003676,   0.014662,    0.023226,    0.014662,    0.003676,    0.000363,
                0.000036,    0.000363,   0.001446,    0.002291,    0.001446,    0.000363,    0.000036,
            } .Select(f1 => (float)f1 * 1000).ToArray();
            
            /*0        ,   0.000003,    0.000039,   0.000093,   0.000039,    0.000003,   0        ,
            0.000003 ,   0.000252,    0.003518,   0.008339,   0.003518,    0.000252,   0.000003 ,
            0.000039 ,   0.003518,    0.049046,   0.116257,   0.049046,    0.003518,   0.000039 ,
            0.000093 ,   0.008339,    0.116257,   0.275573,   0.116257,    0.008339,   0.000093 ,
            0.000039 ,   0.003518,    0.049046,   0.116257,   0.049046,    0.003518,   0.000039 ,
            0.000003 ,   0.000252,    0.003518,   0.008339,   0.003518,    0.000252,   0.000003 ,
            0        ,   0.000003,    0.000039,   0.000093,   0.000039,    0.000003,   0        ,*/
            
            float quality = compiler.GetFloat("_ShoreWavesQual");
            var SHORE_BORDERS = compiler.IsKeywordEnabled("SKIP_SHORE_BORDERS") ? 0 : 1;
            DepthRenderShader_SecondPass_Matertial.SetFloat("SHORE_BORDERS", SHORE_BORDERS);
            
            var SHORE_SIM_RAD = compiler.IsKeywordEnabled("USE_SIM_RADIUS") ? 0 : 1;
            DepthRenderShader_SecondPass_Matertial.SetFloat("SHORE_SIM_RAD", SHORE_SIM_RAD);
            
            DepthRenderShader_SecondPass_Matertial.SetFloat("Q", quality);
            DepthRenderShader_SecondPass_Matertial.SetFloat("STEPS", 7);
            DepthRenderShader_SecondPass_Matertial.SetFloatArray("MISTERGAUSS", mistergauss);
            float radius = compiler.GetFloat("_ShoreWavesRadius");
            
            var ls = transform.localScale;
            var rv = new Vector2(radius, radius) / new Vector2(VertexSize.z, VertexSize.w) / new Vector2(ls.x, ls.z) * 150;
            //  rv = new Vector2(radius, radius);
            DepthRenderShader_SecondPass_Matertial.SetVector("BLUR_R", rv);
            /*var ___ = rt.texture_swap;
            rt.texture_swap = rt.texture;
            rt.texture = ___;*/
            #if UNITY_WEBGL
            var isWebGl = true;
            #else
            var isWebGl = false;
            #endif
            
            if (SystemInfo.deviceType == DeviceType.Handheld || isWebGl)
            {   var texture_gles20 = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true);
                texture_gles20.hideFlags = HideFlags.HideAndDontSave;
                texture_gles20.name = "EModules/MobileWater/" + type + "-" + gameObject.name;
                var oo = RenderTexture.active;
                RenderTexture.active = rt.texture;
                texture_gles20.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
                RenderTexture.active = null;
                texture_gles20.Apply();
                
                var dstr1 = rt.texture_swap;
                var dstr2 = rt.texture;
                rt.texture = null;
                rt.texture_swap = null;
                // rt.texture_swap = rt.texture;
                // rt.texture = null;
                var hash = goID;
                if (cache_GET_RENDER_TEXTURE.ContainsKey(hash) && cache_GET_RENDER_TEXTURE[hash].ContainsKey(type))
                {   cache_GET_RENDER_TEXTURE[hash].Remove(type);
                }
                
                rt = GET_RENDER_TEXTURE(type, resolution);
                Graphics.Blit(texture_gles20, rt.texture, DepthRenderShader_SecondPass_Matertial);
                DESTORY(texture_gles20);
                OnPortRenderAction += () =>
                {   DESTORY(dstr1);
                    DESTORY(dstr2);
                };
                
                RenderTexture.active = oo;
            }
            else
            {   Graphics.Blit(rt.texture, rt.texture_swap, DepthRenderShader_SecondPass_Matertial);
                Graphics.CopyTexture(rt.texture_swap, rt.texture);
                DESTORY(rt.texture_swap);
            }
            
            
            
            //rt.texture_swap.Release();
            /* for (int i = 0; i < 8; i++)
             {
                 DepthRenderShader_SecondPass_Matertial.SetFloat("BLUR_R", radius / (8 - i));
                 Graphics.Blit(rt.texture, rt.texture_swap, DepthRenderShader_SecondPass_Matertial);
                 Graphics.CopyTexture(rt.texture_swap, rt.texture);
             }*/
            //Debug.Log(gameObject.name);
        }
        
        
        /*
        // Tim-CUnity Technologies
        cmdBuffer = new CommandBuffer();
        cmdBuffer.name = "cmdBuffer";
        int texID = Shader.PropertyToID("_OlderTexture");
        cmdBuffer.GetTemporaryRT(texID, -1, -1, 0);
        
        // set the target to be the temp created texture + the current depth / stencil buffer
        // when we draw now we will be using the old depth stencil but a NEW color target
        cmdBuffer.SetRenderTarget(texID, BuiltinRenderTextureType.Depth);
        cmdBuffer.DrawMesh(MyMesh, MeshMatrix, material);
        Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, cmdBuffer);*/
        
        #if UNITY_EDITOR
        if (saveDialog != null)
        {   DESTORY(screenShot);
            screenShot = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true);
            screenShot.hideFlags = HideFlags.HideAndDontSave;
            screenShot.name = "EModules/MobileWater/" + type + "-" + gameObject.name;
            var oo = RenderTexture.active;
            RenderTexture.active = rt.texture;
            screenShot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            RenderTexture.active = oo;
            screenShot.Apply();
        }
        
        #endif
        
        //#if USE_TEX2D_CONVERTER
        // if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 && Application.isPlaying)
        if (UseTextureConverter)
        {   var currentFrame = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true);
            currentFrame.hideFlags = HideFlags.HideAndDontSave;
            currentFrame.name = "EModules/MobileWater/" + type + "-" + gameObject.name;
            var oo = RenderTexture.active;
            RenderTexture.active = rt.texture;
            currentFrame.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            RenderTexture.active = oo;
            currentFrame.Apply();
            DESTORY(rt.texture_gles20);
            rt.texture_gles20 = currentFrame;
        }
        //#endif
        /*
        DESTORY( rt );*/
        RenderDepthOrRefractionCamera.targetTexture = null;
        
        //  if (type == BakeOrUpdateType.Refraction) rt.texture.GenerateMips();
        
        //RenderDepthOrRefractionCamera.targetTexture = oldT;
        //  rt.lastCamera = null;
        gameObject.layer = oldstate;
        
        
        if (!Application.isPlaying)
            DESTROY_CAMERA(RenderDepthOrRefractionCamera);
        else
            RenderDepthOrRefractionCamera.gameObject.SetActive(false);
        // rndr.enabled = oldState;
        //gameObject.SetActive( oldState );
        
        
        
        #if UNITY_EDITOR
        if (saveDialog != null)
        {   System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath.Remove(Application.dataPath.ToLower().LastIndexOf("assets")), saveDialog), screenShot.EncodeToPNG());
            AssetDatabase.ImportAsset(saveDialog, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
        #endif
        
        
        //saving
        var targetTexture = type == BakeOrUpdateType.ZDepth ? _BakedData_temp : type == BakeOrUpdateType.Refraction ? _RefractionTex_temp : _;
        /*var materialTexture =  material.GetTexture( targetTexture );
        #if UNITY_EDITOR
        //var dirty = materialTexture != rt.texture;
        #endif
        if (materialTexture && materialTexture != rt.texture
        #if UNITY_EDITOR
          && string.IsNullOrEmpty( AssetDatabase.GetAssetPath( materialTexture ) )
        #endif
          ) {
          Debug.Log( "Force Destroy" );
          if (Application.isPlaying) Destroy( materialTexture );
          else DestroyImmediate( materialTexture );
        }*/
        if (compiler.HasProperty(targetTexture))
        {
            #if UNITY_EDITOR
            if (saveDialog != null)
            {   if (!Application.isPlaying)
                    Undo();
                if (type == BakeOrUpdateType.ZDepth)
                {   if (saveDialog != null)
                        compiler.SetTexture("_BakedData", screenShot);
                    Undo();
                    backed_zdepth = screenShot;
                    SetDirty();
                }
                else if (type == BakeOrUpdateType.Refraction)
                {   if (saveDialog != null)
                        compiler.SetTexture("_RefractionTex", screenShot);
                    Undo();
                    SetDirty();
                    backed_refraction = screenShot;
                }
                else
                {   // compiler.SetTexture("_", screenShot);
                
                }
                need_update = true;
                if (!Application.isPlaying)
                    SetDirty();
            }
            else
            #endif
            {   //compiler.SetTexture(targetTexture, rt.texture);
            }
        }
        //editor undo fix
        #if UNITY_EDITOR
        //if (dirty && !Application.isPlaying) SetDirty( material );
        #endif
        
        
        
    } /*** BakeOrUpdateTexture() ***/
    
    public System.Action < int, int?, FastWaterModel20Controller > OnDisableAction;
    
    public int VectorToMask(Vector4 v)
    {   int result = 0;
        for (int i = 0; i < 4; i++)
        {   var b = (byte)(int)v[i];
            result |= b << (i * 8);
        }
        return result;
    }
    public Vector4 MaskToVector(int mask)
    {   Vector4 result = Vector4.zero; ;
        for (int i = 0; i < 4; i++)
        {   var b = (mask & (255 << (i * 8))) >> (i * 8);
            result[i] = (byte)b;
        }
        return result;
    }
    //---------------------------------------------------------------------
    #endregion//------PUBLIC VARS
    //---------------------------------------------------------------------
    
    
    
    
    
    //---------------------------------------------------------------------
    #region//------BAKE HELPERS
    //---------------------------------------------------------------------
#pragma warning disable
    [System.NonSerialized, HideInInspector]
    static int
    _VertexSize = Shader.PropertyToID("_VertexSize"),
    _BakedData = Shader.PropertyToID("_BakedData"),
    _BakedData_temp = Shader.PropertyToID("_BakedData_temp"),
    _LastFrame = Shader.PropertyToID("_LastFrame"),
    _LastFrame2 = Shader.PropertyToID("_FrameBuffer"),
    DOWNSAMPLING_SAMPLE_SIZE = Shader.PropertyToID("DOWNSAMPLING_SAMPLE_SIZE"),
    _RefractionTex = Shader.PropertyToID("_RefractionTex"),
    _RefractionTex_temp = Shader.PropertyToID("_RefractionTex_temp"),
    _ = Shader.PropertyToID("_"),
    _ReflectionTex_temp = Shader.PropertyToID("_ReflectionTex_temp"),
    _BakedData_temp_size = Shader.PropertyToID("_BakedData_temp_size"),
    _RefractionTex_temp_size = Shader.PropertyToID("_RefractionTex_temp_size"),
    _ReflectionTex_temp_size = Shader.PropertyToID("_ReflectionTex_temp_size"),
    _ReflectionBakeLayersHash = Shader.PropertyToID("_ReflectionBakeLayers"),
    _RefractionBakeLayers = Shader.PropertyToID("_RefractionBakeLayers"),
    _ZDepthBakeLayers = Shader.PropertyToID("_ZDepthBakeLayers"),
    _ObjecAngle = Shader.PropertyToID("_ObjecAngle");
#pragma warning restore
    
    #if UNITY_EDITOR
    [System.NonSerialized, HideInInspector]
    Texture2D screenShot = null;
    #endif
    static Material DepthRenderShader_SecondPass_Matertial = null;
    
    
    [SerializeField]
    public bool UseTextureConverter = true;
    [System.NonSerialized, HideInInspector]
    static Camera RenderDepthOrRefractionCamera = null;
    [System.NonSerialized, HideInInspector]
    static Camera RenderReflectionCamera = null;
    [System.NonSerialized, HideInInspector]
    static Skybox RenderReflectionCameraSkyBox = null;
    
    List<RenderTexturePair> cache_get = null;
#pragma warning disable
    class RenderTexturePair { public RenderTexture texture; public RenderTexture texture_swap; public int resolution; public Camera lastCamera; public Texture2D texture_gles20; }
#pragma warning restore
    static Dictionary<int, Dictionary<BakeOrUpdateType, List<RenderTexturePair>>> cache_GET_RENDER_TEXTURE = new Dictionary<int, Dictionary<BakeOrUpdateType, List<RenderTexturePair>>>();
    
    void  DESTROY_RENDER_TEXTURE()
    {   var hash = goID;
        if (!cache_GET_RENDER_TEXTURE.ContainsKey(hash))
            return ;
        foreach (var item in cache_GET_RENDER_TEXTURE[hash])
        {   foreach (var texpar in item.Value)
            {   DESTORY(texpar.texture);
                DESTORY(texpar.texture_gles20);
                DESTORY(texpar.texture_swap);
                
            }
            
        }
        cache_GET_RENDER_TEXTURE.Remove(hash);
    }
    
    RenderTexturePair GET_RENDER_TEXTURE(BakeOrUpdateType type, int? camera)
    {   var hash = goID;
        if (hash == -1)
            return null;
        // var hash = type == BakeOrUpdateType.Reflection ? Camera.current ? Camera.current.gameObject : Camera.main.gameObject : gameObject;
        if (!cache_GET_RENDER_TEXTURE.ContainsKey(hash))
            return null;
        if (!cache_GET_RENDER_TEXTURE[hash].ContainsKey(type))
            return null;
        if (camera.HasValue)
            return cache_GET_RENDER_TEXTURE[hash][type][camera.Value];
        return cache_GET_RENDER_TEXTURE[hash][type][0];
    }
    RenderTexturePair GET_RENDER_TEXTURE(BakeOrUpdateType type)
    {   var hash = goID;
        if (hash == -1)
            return null;
        if (!cache_GET_RENDER_TEXTURE.ContainsKey(hash))
            return null;
        if (!cache_GET_RENDER_TEXTURE[hash].ContainsKey(type))
            return null;
        var res = cache_GET_RENDER_TEXTURE[hash][type];
        if (res.Count == 0)
            return null;
        return cache_GET_RENDER_TEXTURE[hash][type][0];
    }
    static RenderTexturePair GET_RENDER_TEXTURE_BYHASH(int hash, BakeOrUpdateType type)
    {   if (hash == -1)
            return null;
        if (!cache_GET_RENDER_TEXTURE.ContainsKey(hash))
            return null;
        if (!cache_GET_RENDER_TEXTURE[hash].ContainsKey(type))
            return null;
        var res = cache_GET_RENDER_TEXTURE[hash][type];
        if (res.Count == 0)
            return null;
        return cache_GET_RENDER_TEXTURE[hash][type][0];
    }
    
    /// [System.NonSerialized]
    /// int renderDebug = 0;
    RenderTexturePair GET_RENDER_TEXTURE(BakeOrUpdateType type, int size, int? camera = null, int? heightSize = null)
    {   if (camera.HasValue && camera.Value == -1)
        {   throw new System.Exception("Cannot GET_RENDER_TEXTURE camera -1");
        }
        /* if (renderDebug > 10)
         {
             Debug.Log("Memory OutOfRange");
             return null;
         }*/
        if (size == -1)
        {   throw new System.Exception("Cannot GET_RENDER_TEXTURE sized -1");
            //if (cache_GET_RENDER_TEXTURE.Count == 0) return null;
            // return cache_GET_RENDER_TEXTURE.First().Value.First().Value;
        }
        
        //var hash = Camera.current ? Camera.current : Camera.main;
        // var hash = type == BakeOrUpdateType.Reflection ? Camera.current ? Camera.current.GetInstanceID() : Camera.main ? Camera.main.GetInstanceID() : -1 : GetInstanceID();
        var hash = goID;
        if (hash == -1)
            return null;
        //if (camera) hash ^= camera.gameObject.GetInstanceID();.
        if (!cache_GET_RENDER_TEXTURE.ContainsKey(hash))
            cache_GET_RENDER_TEXTURE.Add(hash, new Dictionary<BakeOrUpdateType, List<RenderTexturePair>>());
            
        if (!camera.HasValue)
            camera = 0;
        /*0*/
        cache_GET_RENDER_TEXTURE[hash].TryGetValue(type, out cache_get);
        
        
        /*1*/
        if (cache_get == null)
            cache_GET_RENDER_TEXTURE[hash].Add(type, cache_get = new List<RenderTexturePair>());
        /*2*/
        if (cache_get.Count <= camera.Value)
            while (cache_get.Count <= camera.Value)
                cache_get.Add(null);
        /*3*/
        if (cache_get[camera.Value] == null)
            cache_get[camera.Value] = new RenderTexturePair();
        /*4*/
        
        if (!cache_get[camera.Value].texture || size != -1 && cache_get[camera.Value].resolution != size)
        {
            #if FASTWATER20_LOG
            if (type == BakeOrUpdateType.Reflection)
            {   Debug.Log("GET_TEX: " + Camera.current.name + " index:" + camera.Value + " go:" +  gameObject.name + " " + (cache_get == null ? "null" : cache_get[camera.Value].resolution.ToString()) + " " +
                          size);
                //  Debug.Log(cache_get.Count <= camera.Value ? ((cache_get.Count) + " " + (camera.Value)) : (" texture " + (cache_get[camera.Value].texture == null)));
                //renderDebug++;
            }
            #endif
            /* if (type == BakeOrUpdateType.ZDepth)
             {
                 Debug.Log("GET_ZDepth: " +  camera.HasValue + " go:" + gameObject.name + " " + (cache_get == null ? "null" : cache_get[camera.Value].resolution.ToString()) + " " + size);
                 //  Debug.Log(cache_get.Count <= camera.Value ? ((cache_get.Count) + " " + (camera.Value)) : (" texture " + (cache_get[camera.Value].texture == null)));
                 //renderDebug++;
             }*/
            
            // cache_GET_RENDER_TEXTURE[hash].Remove(type);
            // cache_GET_RENDER_TEXTURE[hash].Add(type, cache_get);
            
            
            if (cache_get[camera.Value].texture)
            {   if (cache_get[camera.Value].lastCamera)
                    cache_get[camera.Value].lastCamera.targetTexture = null;
                DESTORY(cache_get[camera.Value].texture);
                DESTORY(cache_get[camera.Value].texture_swap);
                DESTORY(cache_get[camera.Value].texture_gles20);
            }
            
            
            /* if (type == BakeOrUpdateType.Reflection)
             {
                 // Debug.Log(camera.gameObject.GetInstanceID() + " " + Camera.current.gameObject.GetInstanceID());
             }*/
            
            cache_get[camera.Value].resolution = size;
            // Debug.Log("ASD");
            if (heightSize.HasValue)
            {   cache_get[camera.Value].texture = new RenderTexture(size, heightSize ?? size, 16)
                {   hideFlags = HideFlags.HideAndDontSave,
                        name = "EModules/MobileWater/" + type + "-RenderTexture" + gameObject.name,
                };
            }
            else
            {   cache_get[camera.Value].texture = new RenderTexture(size, heightSize ?? size, 32)
                {   isPowerOfTwo = true,
                        hideFlags = HideFlags.HideAndDontSave,
                        name = "EModules/MobileWater/" + type + "-RenderTexture" + gameObject.name,
                        // useMipMap = type == BakeOrUpdateType.Refraction || type == BakeOrUpdateType.ZDepth,
                        useMipMap = false,
                        autoGenerateMips = false,
                        wrapMode = TextureWrapMode.Clamp,
                        format = RenderTextureFormat.ARGB32
                };
            }
            
            cache_GET_RENDER_TEXTURE[hash][type] = cache_get;
            // if (type == BakeOrUpdateType.Reflection) cache_get.texture.memorylessMode = RenderTextureMemoryless.Color | RenderTextureMemoryless.MSAA | RenderTextureMemoryless.Depth;
            // var results = new RenderTexturePair() { lastCamera = cache_get.lastCamera, resolution = cache_get.resolution, texture = cache_get.texture };
            // cache_GET_RENDER_TEXTURE[hash].Add( type, results );
            // return results;
        }
        return cache_get[camera.Value];
    }
    
    struct UV { public Vector2 uv; public Vector3 v; }
    
    void DESTROY_CAMERA(Camera c)
    {   if (!c)
            return;
        DESTORY(c.GetComponent<FlareLayer>());
        //var tt = c.targetTexture;
        c.targetTexture = null;
        /*if (tt) {
          // tt.Release();
          DESTORY( tt );
        }*/
        DESTORY(c.gameObject);
    }
    
    /* void DESTORY(Texture t)
     {
       if (!t) return;
    
       // _disable_texture( (RenderTexture)t );
       DESTORY( (UnityEngine.Object)t );
     }*/
    /* void _disable_texture(RenderTexture t)
     {
       foreach (var c in cache_GET_RENDER_TEXTURE) {
         foreach (var item in c.Value) {
           if (item.Value.texture == t) {
             if (item.Value.lastCamera) {
               item.Value.lastCamera.targetTexture = null;
               Debug.Log( "ASD" );
               //t.Release();
             }
           }
         }
       }
     }*/
    
    static void DESTORY(RenderTexturePair p)
    {   if (p == null) return;
        DESTORY(p.texture);
        DESTORY(p.texture_gles20);
        DESTORY(p.texture_swap);
    }
    public static void DESTORY(UnityEngine.Object o)
    {   if (!o)
            return;
        // Debug.Log("destroy " + o.name);
        if (Application.isPlaying)
        {   if ((o.hideFlags & HideFlags.HideAndDontSave) != 0)
        
            {   //  if (o is Material) return;
                Destroy(o);
            }
        }
        else
        {
            #if UNITY_EDITOR
            if (o is Texture && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o)))
            {   if (RenderTexture.active == o)
                    RenderTexture.active = null;
                DestroyImmediate(o);
            }
            else if ((o.hideFlags & HideFlags.HideAndDontSave) != 0)
            {   DestroyImmediate(o);
            }
            #endif
            
        }
    }
    Vector3 MUL(Vector3 v, Vector3 m)
    {   v.x *= m.x;
        v.y *= m.y;
        v.z *= m.z;
        return v;
    }
    Vector2[] SymmetryFactorUV(Vector2[] IN, Vector2 simmetryPos, Vector2[] targetPos)
    {   var result = new Vector2[IN.Length];
        for (int i = 0; i < IN.Length; i++)
        {   result[i] = (targetPos[i] - IN[i]) / (simmetryPos - IN[i]);
        }
        return result;
    }
    Vector3[] NormalizeVetex(UV[] IN, Vector3 simmetryPos, Vector2[] symmetryFactors)
    {   var result = new Vector3[IN.Length];
        for (int i = 0; i < IN.Length; i++)
        {   result[i] = simmetryPos - IN[i].v;
            result[i].x *= symmetryFactors[i].x;
            result[i].z *= symmetryFactors[i].y;
            result[i] = result[i] + IN[i].v;
        }
        return result;
    }
    T[] WhereMax<T>(T[] arr, System.Func<T, float> maxselector)
    {   List<T> result = new List<T>();
        var max = arr.Select(maxselector).Max();
        foreach (var item in arr)
            if (maxselector(item) == max)
                result.Add(item);
        return result.ToArray();
    }
    T[] WhereMin<T>(T[] arr, System.Func<T, float> maxselector)
    {   List<T> result = new List<T>();
        var max = arr.Select(maxselector).Min();
        foreach (var item in arr)
            if (maxselector(item) == max)
                result.Add(item);
        return result.ToArray();
    }
    //---------------------------------------------------------------------
    #endregion//------BAKE HELPERS
    //---------------------------------------------------------------------
    
    
    
    
    
    
    //---------------------------------------------------------------------
    #region//------PRIVATE VARS
    //---------------------------------------------------------------------
    internal bool? __USE_FAKE_LIGHTING;
    internal bool USE_FAKE_LIGHTING
    {   get { return __USE_FAKE_LIGHTING ?? (__USE_FAKE_LIGHTING = (!compiler || !compiler.material) ? false : compiler.IsKeywordEnabled("USE_FAKE_LIGHTING")).Value; }
        #if UNITY_EDITOR
        set
        {   if (__USE_FAKE_LIGHTING == value)
                return;
            if (value)
                compiler.EnableKeyword("USE_FAKE_LIGHTING");
            else
                compiler.DisableKeyword("USE_FAKE_LIGHTING");
            __USE_FAKE_LIGHTING = value;
        }
        #endif
    }
    static int _LightDir = Shader.PropertyToID("_LightDir");
    static int _LightColor0Fake = Shader.PropertyToID("_LightColor0Fake");
    static int _MyNearClipPlane = Shader.PropertyToID("_MyNearClipPlane");
    static int _ObjectScale = Shader.PropertyToID("_ObjectScale");
    
    static int _MyFarClipPlane = Shader.PropertyToID("_MyFarClipPlane");
    //static int _CameraFarClipPlane = Shader.PropertyToID("_CameraFarClipPlane");
    //static int _LightPos =  Shader.PropertyToID("_LightPos");
    //static int _ReflectionUserCUBE = Shader.PropertyToID("_ReflectionUserCUBE");
    /* uniform float _FracTimeFull;
    #endif
    uniform half _Frac2PITime;
    uniform half _Frac01Time;
    uniform half _Frac01Time_d8_mBlendAnimSpeed;
    uniform half _FracWTime_m4_m3DWavesSpeed_dPI2;
    uniform half _Frac_UFSHORE_Tile_1Time_d10;
    uniform half _Frac_UFSHORE_Tile_2Time_d10;
    //uv scroll
    uniform half2 _Frac_WaterTextureTilingxTime_m_AnimMovex;
    uniform half4 _Frac_UVS_DIR;
    //main tex anim
    uniform half2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move;
    uniform half2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed;
    uniform half2 _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed;*/
    /*half*/
    static int _FracTimeFull = Shader.PropertyToID("_FracTimeFull");
    /*half*/
    static int _Frac2PITime = Shader.PropertyToID("_Frac2PITime");
    /*half*/
    static int _Frac01Time = Shader.PropertyToID("_Frac01Time");
    /*half*/
    static int _Frac01Time_d8_mBlendAnimSpeed = Shader.PropertyToID("_Frac01Time_d8_mBlendAnimSpeed");
    static int _BlendAnimSpeed = Shader.PropertyToID("_BlendAnimSpeed");
    static int _Frac01Time_MAIN_TEX_BA_Speed = Shader.PropertyToID("_Frac01Time_MAIN_TEX_BA_Speed");
    static int _MAIN_TEX_BA_Speed = Shader.PropertyToID("MAIN_TEX_BA_Speed");
    
    /*half*/
    static int _FracW01Time_m4_m3DWavesSpeed_dPI2 = Shader.PropertyToID("_FracWTime_m4_m3DWavesSpeed_dPI2");
    static int _3DWavesSpeed = Shader.PropertyToID("_3DWavesSpeed");
    static int _3DWavesSpeedY = Shader.PropertyToID("_3DWavesSpeedY");
    /*half*/
    
    //static int _Frac_UFSHORE_Tile_1Time_d10 = Shader.PropertyToID("_Frac_UFSHORE_Tile_1Time_d10");
    static int _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1 = Shader.PropertyToID("_Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1");
    static int _UFSHORE_Tile_1 = Shader.PropertyToID("_UFSHORE_Tile_1");
    static int _UFSHORE_Speed1 = Shader.PropertyToID("_UFSHORE_Speed_1");
    /*half*/
    // static int _Frac_UFSHORE_Tile_2Time_d10 = Shader.PropertyToID("_Frac_UFSHORE_Tile_2Time_d10");
    //static int _Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed2 = Shader.PropertyToID("_Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed2");
    // static int _UFSHORE_Tile_2 = Shader.PropertyToID("_UFSHORE_Tile_2");
    // static int _UFSHORE_Speed2 = Shader.PropertyToID("_UFSHORE_Speed_2");
    //uv scroll
    /*half2*/
    static int _Frac_WaterTextureTilingTime_m_AnimMove = Shader.PropertyToID("_Frac_WaterTextureTilingTime_m_AnimMove");
    static int _WaterTextureTiling = Shader.PropertyToID("_WaterTextureTiling");
    static int _AnimMove = Shader.PropertyToID("_AnimMove");
    /*half4*/
    static int _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1 = Shader.PropertyToID("_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1");
    static int MAIN_TEX_FoamGradWavesSpeed_1 = Shader.PropertyToID("MAIN_TEX_FoamGradWavesSpeed_1");
    
    
    static int _Frac_UVS_DIR = Shader.PropertyToID("_Frac_UVS_DIR");
    //static int _WAVES_MAP_CROSS =
    //main tex anim
    /*half2*/
    static int _FracMAIN_TEX_TileTime_mMAIN_TEX_Move = Shader.PropertyToID("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move");
    static int MAIN_TEX_Tile = Shader.PropertyToID("MAIN_TEX_Tile");
    static int MAIN_TEX_Move = Shader.PropertyToID("MAIN_TEX_Move");
    static int MAIN_TEX_CA_Speed = Shader.PropertyToID("MAIN_TEX_CA_Speed");
    /*half2*/
    static int _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed = Shader.PropertyToID("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed");
    /*half2*/
    static int _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed = Shader.PropertyToID("_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed");
    static int _FrameRate = Shader.PropertyToID("_FrameRate");
    
    static Vector2 __tv2;
    static Vector4 TV4;
    static Vector4 __tv4;
    Vector2 V2(float v)
    {   __tv2.Set(v, v);
        return __tv2;
    }
    Vector4 V4(Vector2 v)
    {   __tv4.Set(v.x, v.y, v.x, v.y);
        return __tv4;
    }
    Vector2 V2(Vector4 v)
    {   __tv2.Set(v.x, v.y);
        return __tv2;
    }
    Vector2 FRAC2(Vector2 v1, Vector2 v2)
    {   v1 /= v2;
        v1.x %= 1 / v2.x;
        v1.y %= 1 / v2.y;
        v1 *= v2;
        return v1;
    }
    Vector4 FRAC4(Vector4 v1, Vector4 v2)
    {   for (int i = 0; i < 4; i++)
            v1[i] /= v2[i];
        v1.x %= 1 / v2.x;
        v1.y %= 1 / v2.y;
        v1.z %= 1 / v2.z;
        v1.w %= 1 / v2.w;
        for (int i = 0; i < 4; i++)
            v1[i] *= v2[i];
        return v1;
    }
    public float OBJECT_SPEED = 1;
    float FV;
    Vector2 FV2;
    float PI2 = Mathf.PI * 2;
    // int lastFrame = -1;
    void DO_FRACTIME(MaterialSetter M)
    {
    
        /*    _LastFrame(""_LastFrame"",  2D) = ""white"" {}
        _FracTimeFull(""_FracTimeFull"", Float) = 0
        _Frac2PITime(""_Frac2PITime"", Float) = 0
        _Frac01Time(""_Frac01Time"", Float) = 0
        _Frac01Time_d8_mBlendAnimSpeed(""_Frac01Time_d8_mBlendAnimSpeed"", Float) = 0
        _Frac01Time_MAIN_TEX_BA_Speed(""_Frac01Time_MAIN_TEX_BA_Speed"", Float) = 0
        _FracWTime_m4_m3DWavesSpeed_dPI2(""_FracWTime_m4_m3DWavesSpeed_dPI2"", Vector) = (0,0,0,0)
        _Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1(""_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1"", Float) = 0
        _Frac_UFSHORE_Tile_1Time_d10(""_Frac_UFSHORE_Tile_1Time_d10"", Vector) = (0,0,0,0)
        _Frac_UFSHORE_Tile_2Time_d10(""_Frac_UFSHORE_Tile_2Time_d10"", Vector) = (0,0,0,0)
        _Frac_UFSHORE_Tile_2Time_d10(""_Frac_UFSHORE_Tile_2Time_d10"", Vector) = (0,0,0,0)
        _Frac_WaterTextureTilingTime_m_AnimMove(""_Frac_WaterTextureTilingTime_m_AnimMove"", Vector) = (0,0,0,0)
        _Frac_UVS_DIR(""_Frac_UVS_DIR"", Vector) = (0,0,0,0)
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move"", Vector) = (0,0,0,0)
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed"", Vector) = (0,0,0,0)
        _FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed"", Vector) = (0,0,0,0)*/
        
        
        #if UNITY_EDITOR
        var timeX = Application.isPlaying ? (Time.time / 16) : (float)((EditorApplication.timeSinceStartup / 16) % (int.MaxValue / 2));
        var timeW = Application.isPlaying ? (Time.time) : (float)((EditorApplication.timeSinceStartup) % (int.MaxValue / 2));
        #else
        var timeX = (Time.time / 16);
        var timeW = (Time.time) ;
        #endif
        timeX *= OBJECT_SPEED;
        timeW *= OBJECT_SPEED;
        
        
        int frame = 0;
        if (Application.isPlaying) frame = Time.frameCount;
        else frame = Mathf.RoundToInt( timeW * 60);
        
        // if (lastFrame == frame) return;
        // M.material.SetFloat(_FrameRate, frame % 4);
        if (M.compiler.BufferResolution == 1)   M.material.SetFloat(_FrameRate, (frame % 4) * 4);
        else    M.material.SetFloat(_FrameRate, (frame % 4) * 4 );
        
        
        
        float KF = 0;
        if (IsKeywordEnabled(ULTRA_FAST_MODE) || IsKeywordEnabled(MINIMUM_MODE))
            KF = 1024;
        // KF = 512;
        else
            KF = 8192;
        float KF2 = KF * 2;
        M.SetFloat(_FracTimeFull, (timeX % KF2) - KF);
        
        float _CosTime = Mathf.Cos(timeX);
        float _SinTime = Mathf.Sin(timeX);
        
        FV = timeX % (PI2);
        M.SetFloat(_Frac2PITime, FV);
        FV = timeX % 1;
        M.SetFloat(_Frac01Time, FV);
        
        FV = (timeX / 8 * M.GetFloat(_BlendAnimSpeed)) % 1;
        M.SetFloat(_Frac01Time_d8_mBlendAnimSpeed, FV);
        
        FV2.Set(M.GetFloat(_3DWavesSpeed), M.GetFloat(_3DWavesSpeedY));
        FV2 = FRAC2(timeW * 4 * FV2 / PI2, V2(1));
        M.SetVector(_FracW01Time_m4_m3DWavesSpeed_dPI2, FV2);
        FV2 = FRAC2(V2(timeX / 10 * M.GetFloat(_UFSHORE_Speed1)), V2(M.GetVector(_UFSHORE_Tile_1)));
        M.SetVector(_Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed1, FV2);
        /*FV2 = FRAC2(V2(timeX / 10 * M.GetFloat(_UFSHORE_Speed2)), V2(M.GetVector(_UFSHORE_Tile_2)));
        M.SetVector(_Frac_UFSHORE_Tile_1Time_d10_m_UFSHORE_Speed2, FV2);*/
        /*FV2 = FRAC(V2(timeX / 10), V2(M.GetVector(_UFSHORE_Tile_2)));
        M.SetVector(_Frac_UFSHORE_Tile_2Time_d10, FV2);*/
        FV = (timeX * 16 * M.GetFloat(MAIN_TEX_FoamGradWavesSpeed_1)) % 1;
        M.SetFloat(_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1, FV);
        
        
        
        //uv scroll
        var NORMAL_TILE = V2(M.GetVector(_WaterTextureTiling));
        FV2 = FRAC2(V2(timeX) * V2(M.GetVector(_AnimMove)), NORMAL_TILE);
        M.SetVector(_Frac_WaterTextureTilingTime_m_AnimMove, FV2);
        /* #if defined(WAVES_MAP_CROSS)
            fixed4 UVS_DIR = (fixed4(
                (_FracTimeX / 2 + _CosTime.x * 0.02)	* _WaterTextureTiling.z,
                (_FracTimeX / 2 + _SinTime.x * 0.03)	* _WaterTextureTiling.w ,
                (_FracTimeX / 2 + _SinTime.x * 0.05 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
                (_FracTimeX / 2 + _CosTime.x * 0.04 + 0.5)* _WaterTextureTiling.w  / SCSPDF_DIV
            )1);
        #else
            fixed4 UVS_DIR = (fixed4(\
                (_FracTimeX / 2 + _CosTime.x * 0.04)	* _WaterTextureTiling.z ,
                (_FracTimeX / 2 + _SinTime.x * 0.05)	* _WaterTextureTiling.w ,
                (_FracTimeX / 2 + _CosTime.x * 0.02 + 0.5)	* _WaterTextureTiling.z  / SCSPDF_DIV,
                (_FracTimeX / 2 + _SinTime.x * 0.03 + 0.5) * _WaterTextureTiling.w  / SCSPDF_DIV
            ));
        #endif*/
        var SCSPDF_DIV = 1.4f;
        var WTT = M.GetVector(_WaterTextureTiling);
        if (IsKeywordEnabled(WAVES_MAP_CROSS))
            TV4.Set(
                (timeX / 2 + _CosTime * 0.4f) * WTT.z,
                (timeX / 2 + _SinTime * 0.03f) * WTT.w,
                (timeX / 2 + _SinTime * 0.05f + 0.5f) * WTT.z / SCSPDF_DIV,
                (timeX / 2 + _CosTime * 0.04f + 0.5f) * WTT.w / SCSPDF_DIV);
        else
            TV4.Set(
                (timeX / 2 + _CosTime * 0.4f) * WTT.z,
                (timeX / 2 + _SinTime * 0.05f) * WTT.w,
                (timeX / 2 + _CosTime * 0.02f + 0.5f) * WTT.z / SCSPDF_DIV,
                (timeX / 2 + _SinTime * 0.03f + 0.5f) * WTT.w / SCSPDF_DIV);
        WTT = FRAC4(TV4, V4(V2(WTT)));
        M.SetVector(_Frac_UVS_DIR, WTT);
        
        //main tex anim
        FV = (timeX * M.GetFloat(_MAIN_TEX_BA_Speed)) % 1;
        M.SetFloat(_Frac01Time_MAIN_TEX_BA_Speed, FV);
        
        var MTM = V2(M.GetVector(MAIN_TEX_Move));
        if (!IsKeywordEnabled(MAINTEX_HAS_MOVE))
            MTM.Set(0, 0);
        var MTM_TILE = V2(M.GetVector(MAIN_TEX_Tile));
        FV2 = FRAC2(V2(timeX) * MTM * MTM_TILE, MTM_TILE);
        M.SetVector(_FracMAIN_TEX_TileTime_mMAIN_TEX_Move, FV2);
        FV2 = FRAC2(V2(timeX) * (MTM + V2(M.GetVector(MAIN_TEX_CA_Speed))) * MTM_TILE, MTM_TILE);
        M.SetVector(_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed, FV2);
        FV2 = FRAC2(V2(timeX) * (MTM - V2(M.GetVector(MAIN_TEX_CA_Speed))) * MTM_TILE, MTM_TILE);
        M.SetVector(_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed, FV2);
        
        
        
        /*#if UNITY_EDITOR
                    M.material.SetFloat(_FracTimeX, ((Application.isPlaying ? (Time.time / 16) : ((float)((EditorApplication.timeSinceStartup / 16) % (KF2 / 16)))) % (KF2 / 16)) - KF);
                    M.material.SetFloat(_FracTimeW, ((Application.isPlaying ? Time.time : ((float)((EditorApplication.timeSinceStartup) % KF2))) % KF2) - KF);
        #else
                    M.material.SetFloat(_FracTimeX, ((Time.time / 16) % (KF2 / 16)) - KF);
                    M.material.SetFloat(_FracTimeW, ((Time.time) % KF2) - KF);
        #endif*/
    }
    
    
    
    #if UNITY_EDITOR
    [HideInInspector, SerializeField]
    internal string MaterialsForlder;
    #endif
    
    internal Transform FakeLight
    {   get { return __FakeLight; }
        set
        {   __FakeLight = value;
            __FakeLightComponent = value == null ? null : value.GetComponent<FastWaterModel20FakeLight>();
        }
    }
    
    
    internal MeshRenderer __renderer;
    internal MeshRenderer RENDERER
    {   get
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying && (!this))
                return null;
            #endif
            return __renderer ? __renderer : (__renderer = GetComponent<MeshRenderer>());
        }
    }
    
    public FastWaterModel20Compiler __compiler;
    #if UNITY_EDITOR
    public FastWaterModel20Compiler __overridercompiler;
    #endif
    internal FastWaterModel20Compiler compiler
    {   get
        {
            #if UNITY_EDITOR
            if (__overridercompiler)
                return __overridercompiler;
            #endif
                
                
            #if UNITY_EDITOR
            if (Application.isPlaying)
                return __compiler;
            if (!this)
                return __compiler;
                
            if (!__compiler)
                return null;
            //if (!RENDERER) Debug.LogError("Cannot Get Renderer Component " + gameObject.name);
            if (!RENDERER)
                return null;
                
            __overridercompiler = __compiler;
            TryCreateTempMaterials();
            __overridercompiler = null;
            
            /*if (!RENDERER.sharedMaterial && __compiler.material)
            {
                RENDERER.sharedMaterial = __compiler.material;
                need_update = true;
            }*/
            #endif
            /*var restult = RENDERER ? RENDERER.sharedMaterial : null;
             if (restult != __compiler)
             {
                 __compiler = restult;
            #if UNITY_EDITOR
                 need_update = true;
            #endif
             }
            #if UNITY_EDITOR
             if (restult && supported_sahders.All(s => !restult.shader.name.StartsWith(s))) return null;
            #endif*/
            
            
            
            
            return __compiler;
        }
        set
        {
            #if UNITY_EDITOR
            if (!RENDERER)
                Debug.LogError("Cannot Get MeshRenderer Component " + gameObject.name);
                
            if (value != __compiler || RENDERER.sharedMaterial != value.material)
                need_update = true;
            WaterYPosCacher.RemoveController(getHash, lastPlanarReflectionSize, this, __compiler);
            #endif
            if (__compiler)
                __compiler.onShaderUpdate -= onShaderUpdate;
            if (__compiler != value)
                DESTROY_RENDER_TEXTURE();
            __compiler = value;
            __BAKED_KEYWORDS = null;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                undoRedoPerfowrm();
            #endif
            if (__compiler)
                __compiler.onShaderUpdate += onShaderUpdate;
            // RENDERER.sharedMaterial = !__compiler ? null : __compiler.material;
            PlayModeMaterialCopy();
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {   WaterYPosCacher.CLEAR_ALL();
            
                TryCreateTempMaterials();
                
                if (value)
                    MaterialsForlder = AssetDatabase.GetAssetPath(value);
            }
            #endif
            if (value)
                WaterYPosCacher.TryRegistrateController(getHash, lastPlanarReflectionSize, this);
                
            #if UNITY_EDITOR
            if (!Application.isPlaying)RenderOnAwake();
            #endif
        }
    }
    
    void onShaderUpdate(FastWaterModel20Compiler c)
    {   if (c != compiler)
            return;
            
        __BAKED_KEYWORDS = null;
        setter_cache.Clear();
        
        #if UNITY_EDITOR
        FastWaterModel20ControllerEditor.  shader_cache.Clear();
        TryCreateTempMaterials();
        #endif
        // RENDERER.sharedMaterial = !__compiler ? null : __compiler.material;
    }
    
    /*
    internal Material __material;
    internal Material material {
    get {
    if (Application.isPlaying) return __material ?? (__material = RENDERER ? RENDERER.sharedMaterial : null);
    #if UNITY_EDITOR
    if (!__material && restoreMat) {
      __material = restoreMat;
      need_update = true;
    }
    #endif
    if (!__this) return __material;
    var restult = RENDERER ? RENDERER.sharedMaterial : null;
    if (restult != __material) {
      __material = restult;
    #if UNITY_EDITOR
      need_update = true;
    #endif
    }
    #if UNITY_EDITOR
    if (restult && supported_sahders.All( s => !restult.shader.name.StartsWith( s ) )) return null;
    #endif
    return restult;
    }
    set {
    #if UNITY_EDITOR
    if (value != __material || RENDERER.sharedMaterial != value) need_update = true;
    #endif
    RENDERER.sharedMaterial = __material = value;
    }
    }*/
    Vector3 m_v3;
    Vector4 m_v4;
    
    
    
    
    //---------------------------------------------------------------------
    #endregion//------PRIVATE VARS
    //---------------------------------------------------------------------
    
    
    
    
    
    
    //---------------------------------------------------------------------
    #region//------INITIALIZATION AND RENDER
    //---------------------------------------------------------------------
    
    #if UNITY_EDITOR
    class FileModificationWarningA : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {   need_update = true;
        
            /*   if (importedAssets.Any(i => i.EndsWith(".cs")))
               {   DestroyAllEditorDatasDeep(true);
               }*/
        }
    }
    public class FileModificationWarningB : UnityEditor.AssetModificationProcessor {
        static string[] OnWillSaveAssets(string[] paths)
        {   need_update = true;
        
            return paths;
        }
    }
    private void EditorUpdate()
    {   if (Application.isPlaying)
            return;
        local_Update();
        //Debug.Log("update");
    }
    
    #endif
    
    private void Awake()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) DestroyAllEditorDatasDeep(true);
        #endif
    }
    
    public static bool need_update = false;
    bool local_update = false;
    
    void local_Update()
    {
        #if UNITY_EDITOR
        if (RENDERER && RENDERER.sharedMaterial != lastResetMaterial)
            Reset();
        #endif
        if (need_update)
            local_update = true;
    }
    static bool wasStaticUpdate = false;
    static void static_Update()
    {   wasStaticUpdate = true;
        /* for (int i = 0; i < camerapos_cache_float.Count; i++)
         {
             camerapos_cache_float[i] = -123123123;
         }*/
        destroyHelper.Destroy();
        
        WaterYPosCacher.CLEAR_PLAY_FRAME();
        /*foreach (var cf in camerapos_cache_PTR)
            foreach (var item in cf.Value)
                item.Value.rp = null;*/
        
        CameraIndexPreRender = -1;
        CameraIndexPostRender = -1;
        
        
    }
    
    
    bool HAS_SHORE()
    {   return (IsKeywordEnabled(ULTRA_FAST_MODE) || IsKeywordEnabled(MINIMUM_MODE)) ? compiler.IsKeywordEnabled("UFAST_SHORE_1") ||
               compiler.IsKeywordEnabled("UFAST_SHORE_2")  : compiler.IsKeywordEnabled("SHORE_WAVES");
    }
    bool SKIP_SECOND_DEPTH()
    {   return (IsKeywordEnabled(ULTRA_FAST_MODE) || IsKeywordEnabled(MINIMUM_MODE)) ? compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH_1") ||
               compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH_2")  : compiler.IsKeywordEnabled("SKIP_SECOND_DEPTH");
    }
    bool SECOND_DEPTH_NEED()
    {
    
        var condOne =  HAS_SHORE() && (!SKIP_SECOND_DEPTH()/* || compiler.IsKeywordEnabled("ALLOW_MANUAL_DEPTH")*/);
        // var condOne = IsKeywordEnabled(SHORE_WAVES) && !IsKeywordEnabled(DEPTH_NONE) && !IsKeywordEnabled(SKIP_SECOND_DEPTH);
        //  var condTwo =
        return condOne;
    }
    
    void RenderOnAwake(bool usedScript = false)
    {   //Debug.Log( GET_RENDER_TEXTURE( BakeOrUpdateType.ZDepth ).texture.name );
        if (!this || !enabled) return;
        CloneMaterial();
        Reset();
        
        if (compiler)
        {   if (IsKeywordEnabled(BAKED_DEPTH_ONAWAKE) ||
                    SECOND_DEPTH_NEED() ||
                    (!Application.isPlaying || usedScript) && IsKeywordEnabled(BAKED_DEPTH_VIASCRIPT))
                    
                m_BakeOrUpdateTexture(BakeOrUpdateType.ZDepth, Mathf.RoundToInt(compiler.GetFloat(_BakedData_temp_size)));
                
            if (compiler.IsKeywordEnabled("REFRACTION_BAKED_ONAWAKE") ||
                    (!Application.isPlaying || usedScript) && compiler.IsKeywordEnabled("REFRACTION_BAKED_VIA_SCRIPT"))
                    
                m_BakeOrUpdateTexture(BakeOrUpdateType.Refraction, Mathf.RoundToInt(compiler.GetFloat(_RefractionTex_temp_size)));
        }
    }
    
    
    //#if !UNITY_EDITOR
    bool MatWasAssigned = false;
    //#endif
    void CloneMaterial()
    {   //#if !UNITY_EDITOR
        if (MatWasAssigned)
            return;
        MatWasAssigned = true;
        PlayModeMaterialCopy();
        //#endif
    }
    
    void PlayModeMaterialCopy()
    {   //#if !UNITY_EDITOR
        if (Application.isPlaying && compiler)
        {   if (RENDERER.sharedMaterial && (RENDERER.sharedMaterial.hideFlags & HideFlags.HideAndDontSave) != 0 && compiler.material != RENDERER.sharedMaterial)
                DESTORY(RENDERER.sharedMaterial);
            RENDERER.sharedMaterial = new Material(compiler.material);
            RENDERER.sharedMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        //#endif
    }
    /* void Start()
     {
       local_update = true;
       RenderOnAwake();
     }*/
    /*void OnRenderImage()
    {
      if (local_update) {
        if (material) {
          if (material.IsKeywordEnabled( "BAKED_DEPTH_ONAWAKE" )) BakeOrUpdateTexture( BakeOrUpdateType.ZDepth, Mathf.RoundToInt( material.GetFloat( _BakedData_temp_size ) ) );
          if (material.IsKeywordEnabled( "REFRACTION_BAKED_ONAWAKE" )) BakeOrUpdateTexture( BakeOrUpdateType.Refraction, Mathf.RoundToInt( material.GetFloat( _RefractionTex_temp_size ) ) );
        }
        local_update = false;
      }
    }*/
    //  static bool initFlag = false;
    MonoBehaviour __this;
    static int CameraIndexPreRender = -1;
    static int CameraIndexPostRender = -1;
    
    
    
    
    /*  private void OnDestroy()
      {
          Debug.Log("ASD");
      }*/
    
    
    
    #if UNITY_EDITOR
    static void RepantInspector()
    {   //SceneView.RepaintAll();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
    
    
    
    
    
    Material lastResetMaterial;
    #endif
    private void Reset()
    {
        #if UNITY_EDITOR
        // lastResetMaterial = !compiler ? null : compiler.material;
        lastResetMaterial = !RENDERER ? null : RENDERER.sharedMaterial;
        #endif
        __BAKED_KEYWORDS = null;
        m_OldDirection = Vector3.zero;
        //oldFarClipPlane = null;
        // m_InitScreenReflections = false;
        //if (__compiler) SetLightDir();
        // lastOrto.Clear();
    }
    
    /*private void OnPreCull()
    {
    
    }
    private void OnRenderObject()
    {
    
    }*/
#pragma warning disable
    //[HideInInspector, System.NonSerialized]
    //Camera lastDepth;
    // bool? lastOrto;
    //[HideInInspector, System.NonSerialized]
    // Dictionary<Camera,bool?> needOrto =  new Dictionary<Camera,bool?>();
    //bool needOrto = false;
    
    /*[HideInInspector, SerializeField]
    public int backed_layers;*/
    [SerializeField]
    public Texture2D backed_zdepth;
    [SerializeField]
    public Texture2D backed_refraction;
    
    bool?[] __BAKED_KEYWORDS = null;
    bool?[] BAKED_KEYWORDS { get { return __BAKED_KEYWORDS ?? (__BAKED_KEYWORDS = new bool?[KEYWORDS.Count]); } }
    int REFLECTION_PLANAR = 0;
    int REFRACTION_BAKED_FROM_TEXTURE = 1;
    int BAKED_DEPTH_EXTERNAL_TEXTURE = 2;
    int REFRACTION_BAKED_VIA_SCRIPT = 3;
    int REFRACTION_BAKED_ONAWAKE = 4;
    int BAKED_DEPTH_VIASCRIPT = 5;
    int BAKED_DEPTH_ONAWAKE = 6;
    int DEPTH_NONE = 7;
    int SHORE_WAVES = 8;
    // int SKIP_SECOND_DEPTH = 9;
    int SKIP_3DVERTEX_ANIMATION = 10;
    int ALLOW_MANUAL_DEPTH = 11;
    int ULTRA_FAST_MODE = 12;
    int WAVES_MAP_CROSS = 13;
    int MAINTEX_HAS_MOVE = 14;
    int MINIMUM_MODE = 15;
    int FORCE_OPAQUE = 16;
    
    
    Dictionary<int, string> KEYWORDS = new Dictionary<int, string>()
    { { 0, "REFLECTION_PLANAR" }, { 1, "REFRACTION_BAKED_FROM_TEXTURE" }, { 2, "BAKED_DEPTH_EXTERNAL_TEXTURE" },
        { 3, "REFRACTION_BAKED_VIA_SCRIPT" }, { 4, "REFRACTION_BAKED_ONAWAKE" },
        { 5, "BAKED_DEPTH_VIASCRIPT" }, { 6, "BAKED_DEPTH_ONAWAKE" },
        { 7, "DEPTH_NONE" }, { 8, "SHORE_WAVES" }, { 9, "SKIP_SECOND_DEPTH" },
        {10, "SKIP_3DVERTEX_ANIMATION" }, {11, "ALLOW_MANUAL_DEPTH" }, {12, "ULTRA_FAST_MODE" }, {13, "WAVES_MAP_CROSS" }, {14, "MAINTEX_HAS_MOVE" }, {15, "MINIMUM_MODE" }
        , {16, "FORCE_OPAQUE" }
    };
    bool IsKeywordEnabled(int keyword)
    {   return !Application.isPlaying && compiler.IsKeywordEnabled(KEYWORDS[keyword]) || Application.isPlaying
               && (BAKED_KEYWORDS[keyword] ?? (BAKED_KEYWORDS[keyword] = compiler.IsKeywordEnabled(KEYWORDS[keyword])).Value);
    }
    
    #if UNITY_EDITOR
    static Dictionary<int, Material> editor_cache_materials = new Dictionary<int, Material>();
    static Dictionary<int, List<Material>> editor_cache_matcollector = new Dictionary<int, List<Material>>();
    static Dictionary<int, List<FastWaterModel20Controller>> editor_cache_gocollector = new Dictionary<int, List<FastWaterModel20Controller>>();
    //[SerializeField]
    // Material restoreMat;
    
    /*void CopyMat(Material from, Material to)
    {
        var sh = from.shader;
        var count = ShaderUtil.GetPropertyCount( sh );
        ShaderUtil.ke
    }*/
    
    
    #endif
#pragma warning restore
    
    /*   bool visible = true;
       private void OnBecameInvisible()
       {
           visible = false;
       }
       private void OnBecameVisible()
       {
           visible = true;
       }*/
    
    
    [System.Serializable]
    public class MeshData {
        [SerializeField]
        public int XCOUNT, ZCOUNT;
        [SerializeField]
        public float XSIZE, ZSIZE;
        public MeshData(MeshData data)
        {   ZCOUNT = data.ZCOUNT;
            XCOUNT = data.XCOUNT;
            XSIZE = data.XSIZE;
            ZSIZE = data.ZSIZE;
        }
        public MeshData()
        {   ZCOUNT = XCOUNT = 10;
            XSIZE = ZSIZE = 100;
        }
        public MeshData(Mesh mesh)
        {   var vertices = mesh.vertices;
            var ZGROUP = vertices.GroupBy(v => v.x).First().ToArray();
            var XGROUP = vertices.GroupBy(v => v.z).First().ToArray();
            XCOUNT = XGROUP.Length - 1;
            ZCOUNT = ZGROUP.Length - 1;
            XSIZE = XGROUP.Select(v => v.x).Max() - XGROUP.Select(v => v.x).Min();
            ZSIZE = ZGROUP.Select(v => v.z).Max() - ZGROUP.Select(v => v.z).Min();
        }
        public enum MeshType
        { Normal, Quad, Triangle }
        public void WriteToMesh(MeshFilter meshFilter, Mesh mesh, MeshType type = MeshType.Normal)
        {   if (!mesh)
                mesh = meshFilter.sharedMesh;
            if (!mesh)
                mesh = new Mesh() { name = "Water Dynamic Mesh" };
            #if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(meshFilter, "Change Mesh");
            #endif
            var VX = XCOUNT + 1;
            var VZ = ZCOUNT + 1;
            int[] tri = new int[(VX * VZ * 2 - VX - VZ) * 3];
            Vector3[] v = null;
            Vector2[] uv = null;
            switch (type)
            {   case MeshType.Normal:
                    v = new Vector3[VX * VZ];
                    uv = new Vector2[VX * VZ];
                    break;
                case MeshType.Quad:
                    v = new Vector3[tri.Length / 2];
                    uv = new Vector2[tri.Length / 2];
                    break;
                case MeshType.Triangle:
                    v = new Vector3[tri.Length];
                    uv = new Vector2[tri.Length];
                    break;
            }
            
            var triIndex = 0;
            var X_CELL = XSIZE / XCOUNT;
            var Z_CELL = ZSIZE / ZCOUNT;
            for (int z = 0; z < VZ; z++)
            {   for (int x = 0; x < VX; x++)
                {   v[x + z * VX] = new Vector3(x * X_CELL - XSIZE / 2, 0, z * Z_CELL - ZSIZE / 2);
                    uv[x + z * VX] = new Vector2(1 - x / (float)XCOUNT, 1 - z / (float)ZCOUNT);
                    if (z < VZ - 1 && x < VX - 1)
                    {   tri[triIndex * 3] = x + z * VX;
                        tri[triIndex * 3 + 1] = (x + 0) + (z + 1) * VX;
                        tri[triIndex * 3 + 2] = (x + 1) + z * VX;
                        ++triIndex;
                        tri[triIndex * 3] = (x + 1) + z * VX;
                        tri[triIndex * 3 + 1] = (x + 0) + (z + 1) * VX;
                        tri[triIndex * 3 + 2] = (x + 1) + (z + 1) * VX;
                        ++triIndex;
                    }
                }
            }
            mesh.vertices = v;
            mesh.triangles = tri;
            mesh.uv2 = mesh.uv = uv;
            //mesh.normals = Enumerable.Repeat( new Vector3( 0, 1, 0 ), uv.Length ).ToArray();
            //Unwrapping.GenerateSecondaryUVSet( mesh, new UnwrapParam() { hardAngle = 180 } );
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            meshFilter.sharedMesh = mesh;
            
            if (meshFilter.GetComponent<BoxCollider>())
            {   var c = meshFilter.GetComponent<BoxCollider>();
                c.size = new Vector3(XSIZE, 1, ZSIZE);
            }
            
            #if UNITY_EDITOR
            FastWaterModel20Controller.SetDirty(meshFilter);
            #endif
        }
        
        public void ResetScale(MeshFilter meshFilter)
        {   if (!meshFilter.sharedMesh)
                return;
            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(meshFilter.gameObject, "Change Mesh");
            UnityEditor.Undo.RecordObject(meshFilter, "Change Mesh");
            #endif
            var s = meshFilter.transform.localScale;
            var v = meshFilter.sharedMesh.vertices;
            for (int i = 0; i < v.Length; i++)
            {   v[i].x *= s.x;
                v[i].y *= s.y;
                v[i].z *= s.z;
            }
            XSIZE *= s.x;
            ZSIZE *= s.z;
            cache_mesh.Remove(meshFilter.sharedMesh);
            var m = CopyMesh(meshFilter.sharedMesh);
            m.vertices = v;
            meshFilter.sharedMesh = m;
            meshFilter.transform.localScale = Vector3.one;
            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();
            
            
            if (meshFilter.GetComponent<BoxCollider>())
            {   var c = meshFilter.GetComponent<BoxCollider>();
                c.size = new Vector3(c.size.x * s.x, c.size.y * s.y, c.size.z * s.z);
            }
            
            
            #if UNITY_EDITOR
            need_update = true;
            FastWaterModel20Controller.SetDirty(meshFilter);
            #endif
            
            meshFilter.GetComponent<FastWaterModel20Controller>().CalcVertexSize();
            
            // FastWaterModel20Controller.SetDirty( meshFilter.gameObject );
            
        }
    }
    
    public static Mesh CopyMesh(Mesh source)
    {   var m = new Mesh();
        m.name = source.name;
        m.vertices = source.vertices;
        m.triangles = source.triangles;
        m.colors = source.colors;
        m.normals = source.normals;
        m.uv2 = m.uv = source.uv;
        // Unwrapping.GenerateSecondaryUVSet( m, new UnwrapParam() { hardAngle = 180 } );
        // Unwrapping.GenerateSecondaryUVSet( m );
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
        return m;
    }
    
    
    [SerializeField]
    public MeshData CurrentMeshData = new MeshData();
    static Dictionary<Mesh, MeshData> cache_mesh = new Dictionary<Mesh, MeshData>();
    // MeshData empty_mesh = new MeshData();
    public void UpdateMesh(MeshFilter mf, MeshData mesh)
    {   var newMesh = new Mesh() { name = "Water Dynamic Mesh" };
        mesh.WriteToMesh(mf, newMesh);
        #if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(this, "Change Mesh");
        #endif
        CurrentMeshData = mesh;
        #if UNITY_EDITOR
        FastWaterModel20Controller.SetDirty(this);
        cache_mesh.Clear();
        #endif
    }
    
    public MeshData GET_MESH(Mesh mesh)
    {   if (!mesh)
            new MeshData(CurrentMeshData);
        if (!cache_mesh.ContainsKey(mesh))
            cache_mesh.Add(mesh, new MeshData(mesh));
        return cache_mesh[mesh];
    }
    
    void CalcVertexSize()
    {   var newSt = !MESH ? -1 : !MESH.sharedMesh ? -1 : MESH.sharedMesh.GetInstanceID();
        if (VertexSizeLastStamp != newSt || MESH && !MESH.sharedMesh)
        {
        
            if (MESH)
            {   if (!MESH.sharedMesh)
                    UpdateMesh(MESH, CurrentMeshData);
                    
                var v = MESH.sharedMesh.vertices;
                var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                for (int i = 0; i < v.Length; i++)
                {   if (v[i].x < min.x)
                        min.x = v[i].x;
                    if (v[i].z < min.z)
                        min.z = v[i].z;
                    if (v[i].x > max.x)
                        max.x = v[i].x;
                    if (v[i].z > max.z)
                        max.z = v[i].z;
                }
                // VertexSize.Set(min.x, min.z, max.x, max.z);
                VertexSize.Set(min.x, min.z, max.x - min.x, max.z - min.z);
                #if UNITY_EDITOR
                SetDirty();
                #endif
                
                VertexSizeLastStamp = newSt;
            }
            else
            {   throw new System.Exception("Water Model 2.0 object has't MeshFilter Component");
            }
            
        }
    }
    MeshFilter __mf;
    MeshFilter MESH
    {   get { return __mf ? __mf : (__mf = GetComponent<MeshFilter>()); }
    }
    
    [SerializeField]
    public int VertexSizeLastStamp;
    [SerializeField]
    public Vector4 VertexSize;
    
    
    float? bakedRotation;
    Vector3? localScale;
    /*[System.NonSerialized]
     Vector3 VertexMin, VertexMax;
     [System.NonSerialized]
     Vector4 VertexSizeResult;*/
    
    
    class MaterialSetter {
        public FastWaterModel20Compiler compiler;
        public Material material;
        public MeshRenderer RENDERER;
        bool? __lastOrto;
        public bool? lastOrto
        {   get { return __lastOrto; }
            set
            {   if (__lastOrto == value)
                    return;
                __lastOrto = value;
                if (value.Value)
                {   if (!material.IsKeywordEnabled("ORTO_CAMERA_ON"))
                    {   material.EnableKeyword("ORTO_CAMERA_ON");
                    }
                    //lastOrto[camera] = true;
                }
                else if (material.IsKeywordEnabled("ORTO_CAMERA_ON"))
                    material.DisableKeyword("ORTO_CAMERA_ON");
            }
        }
        
        
        int? __lastDownSamling;
        //   bool? flag1, flag2;
        public int lastDownSamling
        {   get
            {   lastDownSamling = compiler.BufferResolution;
                return   compiler.BufferResolution;
            }
            private set
            {   if (__lastDownSamling == value) return;
            
                __lastDownSamling = value;
                
                
                
                if (value != 0)
                {   material.DisableKeyword("WATER_DOWNSAMPLING");
                    material.DisableKeyword("WATER_DOWNSAMPLING_HARD");
                    
                    
                    material.EnableKeyword("WATER_DOWNSAMPLING");
                    var oldMat = RENDERER.sharedMaterial;
                    if (compiler.Support128Shader)
                    {   RENDERER.sharedMaterial = compiler.Support128Shader;
                        if (!RENDERER.sharedMaterial.shader.isSupported)
                        {   RENDERER.sharedMaterial.DisableKeyword("WATER_DOWNSAMPLING");
                            RENDERER.sharedMaterial.EnableKeyword("WATER_DOWNSAMPLING_HARD");
                            // #if UNITY_EDITOR
                            /*if ( compiler.name == "Fast Water Material - Ocean B")
                                if (Application.isPlaying) GameObject.FindObjectsOfType<Text>().First(t => t.gameObject.name == "LOG").text = "WATER_DOWNSAMPLING_HARD";*/
                            // #endif
                        }
                        else
                        {   //  #if UNITY_EDITOR
                            /* if ( compiler.name == "Fast Water Material - Ocean B")
                                 if (Application.isPlaying) GameObject.FindObjectsOfType<Text>().First(t => t.gameObject.name == "LOG").text = "WATER_DOWNSAMPLING";*/
                            // #endif
                        }
                        
                    }
                    
                    RENDERER.sharedMaterial = oldMat;
                    /* if (Application.isPlaying) Destroy(checkMat);
                     else DestroyImmediate(checkMat, true);*/
                    
                    //  var targetKeyWord = compiler.Support128Shader.isSupported ? "WATER_DOWNSAMPLING" : "WATER_DOWNSAMPLING_HARD";
                    
                    /*  if (!material.IsKeywordEnabled(targetKeyWord))
                      {
                          if ( compiler.name == "Fast Water Material - Ocean B")
                          {   if (Application.isPlaying) GameObject.FindObjectsOfType<Text>().First(t => t.gameObject.name == "LOG").text = targetKeyWord;
                              Debug.Log(targetKeyWord);
                          }
                    
                    
                          // material.shader.err
                          //if (!material.shader.isSupported) GameObject.Find("OCEANS").gameObject.SetActive(false);
                    
                      }*/
                    //lastOrto[camera] = true;
                }
                else
                {   if (material.IsKeywordEnabled("WATER_DOWNSAMPLING"))
                        material.DisableKeyword("WATER_DOWNSAMPLING");
                    if (material.IsKeywordEnabled("WATER_DOWNSAMPLING_HARD"))
                        material.DisableKeyword("WATER_DOWNSAMPLING_HARD");
                }
            }
        }
        
        
        Dictionary<int, Vector4> SetVectorDic = new Dictionary<int, Vector4>();
        public void SetVector(int hash, Vector4 v)
        {   if (!SetVectorDic.ContainsKey(hash))
                SetVectorDic.Add(hash, new Vector4(-1, -1, -1, -1));
            if (SetVectorDic[hash] == v)
                return;
            SetVectorDic[hash] = v;
            material.SetVector(hash, v);
        }
        public Vector4 GetVector(int hash)
        {   if (!SetVectorDic.ContainsKey(hash))
                SetVectorDic.Add(hash, material.GetVector(hash));
            #if UNITY_EDITOR
            SetVectorDic[hash] = material.GetVector(hash);
            #endif
            // if (SetVectorDic[hash] == null) SetVectorDic[hash] = material.GetVector(hash);
            return SetVectorDic[hash];
        }
        
        
        Dictionary<int, Texture> SetTextureDic = new Dictionary<int, Texture>();
        public void SetTexture(int hash, Texture v)
        {   if (!SetTextureDic.ContainsKey(hash))
                SetTextureDic.Add(hash, null);
            if (SetTextureDic[hash] == v)
                return;
            SetTextureDic[hash] = v;
            material.SetTexture(hash, v);
        }
        public Texture GetTexture(int hash)
        {   if (!SetTextureDic.ContainsKey(hash))
                SetTextureDic.Add(hash, material.GetTexture(hash));
            #if UNITY_EDITOR
            SetTextureDic[hash] = material.GetTexture(hash);
            #endif
            // if (SetFloatDic[hash] == null) SetFloatDic[hash] = material.GetFloat(hash);
            return SetTextureDic[hash];
        }
        
        Dictionary < int, float? > SetFloatDic = new Dictionary < int, float? >();
        public void SetFloat(int hash, float v)
        {   if (!SetFloatDic.ContainsKey(hash))
                SetFloatDic.Add(hash, null);
            if (SetFloatDic[hash] == v)
                return;
            SetFloatDic[hash] = v;
            material.SetFloat(hash, v);
        }
        public float GetFloat(int hash)
        {   if (!SetFloatDic.ContainsKey(hash))
                SetFloatDic.Add(hash, material.GetFloat(hash));
            #if UNITY_EDITOR
            SetFloatDic[hash] = material.GetFloat(hash);
            #endif
            if (SetFloatDic[hash] == null)
                SetFloatDic[hash] = material.GetFloat(hash);
            return SetFloatDic[hash].Value;
        }
        
        Dictionary < int, Color? > SetColorDic = new Dictionary < int, Color? >();
        public void SetColor(int hash, Color v)
        {   if (!SetColorDic.ContainsKey(hash))
                SetColorDic.Add(hash, null);
            if (SetColorDic[hash] == v)
                return;
            SetColorDic[hash] = v;
            material.SetColor(hash, v);
        }
        public Color GetColor(int hash)
        {   if (!SetColorDic.ContainsKey(hash))
                SetColorDic.Add(hash, material.GetColor(hash));
            #if UNITY_EDITOR
            SetColorDic[hash] = material.GetColor(hash);
            #endif
            if (SetColorDic[hash] == null)
                SetColorDic[hash] = material.GetColor(hash);
            return SetColorDic[hash].Value;
        }
    }
    
    
    
    /*void RestoreMat()
    {
        __compiler.OverrideMaterial = null;
        if (__compiler && RE_NDERER.sharedMaterial != __compiler.material)
        {
            RE_NDERER.sharedMaterial = __compiler.material;
        }
    }*/
    
    /* private void OnPreRender()
     {
       UpdateMaterials();
     }
     private void OnRenderObject()
     {
       UpdateMaterials();
     }*/
    /*private void Update()
    {
    
    }*/
    
    Color m_OldColor;
    Vector3 m_OldDirection;
    
    
    
    
    bool MainRender(Camera camera)
    {
    
        if (camera == Camera.main)
        {   m_renderCamera = camera;
            #if UNITY_EDITOR
        }
        else if (UnityEditor.SceneView.currentDrawingSceneView && UnityEditor.SceneView.currentDrawingSceneView.camera == camera)
        {   m_renderCamera = camera;
            #endif
        }
        else
        {   return false;
        }
        return true;
    }
    
    
    
    // private static bool m_NowReflectionsRendering = false;
    // int eyeIndex = 0;
    Camera m_renderCamera;
    static Dictionary<Camera, Skybox> skybox_cache = new Dictionary<Camera, Skybox>();
    
    //static List<float> camerapos_cache_float = new List<float>();
    // Dictionary<Camera, RenderTexturePair> camerapos_cache_texture = new Dictionary<Camera, RenderTexturePair>();
    Matrix4x4 reflection = Matrix4x4.zero;
    Vector4 reflectionPlane;
    Vector3 newpos, euler;
    public static int CulMask { get { return __CulMask ?? (__CulMask = ~(LayerMask.GetMask("TransparentFX") | LayerMask.GetMask("UI") | LayerMask.GetMask("Water") | 1 << 7)).Value; } }
    static int? __CulMask;
    
    
    class ccptr { public RenderTexturePair rp; }
    static class WaterYPosCacher {
        public static void TryRegistrateController(int y_pos, int? t_size, FastWaterModel20Controller c)
        {   if (!c.compiler) return;
            if (!compilerCollection.ContainsKey(c.compiler.cID))
                compilerCollection.Add(c.compiler.cID, new Dictionary<int, Dictionary<int, bool>>());
            if (!compilerCollection[c.compiler.cID].ContainsKey(y_pos))
                compilerCollection[c.compiler.cID].Add(y_pos, new Dictionary<int, bool>());
            if (!compilerCollection[c.compiler.cID][y_pos].ContainsKey(c.goID))
            {   compilerCollection[c.compiler.cID][y_pos].Add(c.goID, false);
                c.OnDisableAction += (iny_pos, int_size, inController) =>
                {   RemoveController(iny_pos, int_size, inController, inController.compiler);
                };
                c.OnBecameInVisibleAction += (iny_pos, int_size, inController) =>
                {   RemoveController(iny_pos, int_size, inController, inController.compiler);
                };
                TryMoveTextureBetweenCaches(y_pos, t_size, c);//check
            }
        }
        public static void RemoveController(int y_pos, int? t_size, FastWaterModel20Controller c, FastWaterModel20Compiler compiler)
        {   if (!compiler)
                return;
            if (compilerCollection.ContainsKey(compiler.cID))
            {   if (compilerCollection[compiler.cID].ContainsKey(y_pos))
                {   compilerCollection[compiler.cID][y_pos].Remove(c.goID);
                    if (compilerCollection[compiler.cID][y_pos].Count == 0)
                        compilerCollection[compiler.cID].Remove(y_pos);
                    TryMoveTextureBetweenCaches(y_pos, t_size, c);//check
                }
                if (compilerCollection[compiler.cID].Count == 0)
                    compilerCollection.Remove(compiler.cID);
            }
        }
        
        static Dictionary<int, Dictionary<int, Dictionary<int, bool>>> compilerCollection
            = new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
            
        // objects with different materials and different Y axis positions will be grouped according to the RenderTexture size to "keep video memory"
        static Dictionary<int, Dictionary<int, ccptr>> size_cache_PTR = new Dictionary<int, Dictionary<int, ccptr>>();
        // objects with same materials and Y axis positions will be grouped according y_pos to render planar reflection in "one pass"
        static Dictionary<int, Dictionary<int, Dictionary<int, ccptr>>> camerapos_cache_PTR = new Dictionary<int, Dictionary<int, Dictionary<int, ccptr>>>();
        
        public static void CLEAR_PLAY_FRAME()
        {   foreach (var cam in size_cache_PTR)
                foreach (var size in cam.Value)
                    size.Value.rp = null;
            foreach (var cam in camerapos_cache_PTR)
                foreach (var ypos in cam.Value)
                    foreach (var size in ypos.Value)
                        size.Value.rp = null;
                        
                        
        }
        #if UNITY_EDITOR
        public static void CLEAR_ALL()
        {   foreach (var cam in size_cache_PTR)
                foreach (var size in cam.Value)
                    size.Value.rp = null;
            foreach (var cam in camerapos_cache_PTR)
                foreach (var ypos in cam.Value)
                    foreach (var size in ypos.Value)
                        size.Value.rp = null;
                        
            foreach (var cam in foreverfix_size_cache_PTR)
                foreach (var size in cam.Value)
                    size.Value.rp = null;
            foreach (var cam in foreverfix_camerapos_cache_PTR)
                foreach (var ypos in cam.Value)
                    foreach (var size in ypos.Value)
                        size.Value.rp = null;
        }
        #endif
        
        public static RenderTexturePair GET_PLAY_FRAME(int CameraInder, int y_pos, int t_size, FastWaterModel20Controller c)
        {   TryRegistrateController(y_pos, t_size, c);
            if (compilerCollection[c.compiler.cID][y_pos].Count == 1)
            {   if (size_cache_PTR.ContainsKey(CameraIndexPreRender) &&
                        size_cache_PTR[CameraIndexPreRender].ContainsKey(t_size) &&
                        size_cache_PTR[CameraIndexPreRender][t_size].rp != null)
                    return size_cache_PTR[CameraIndexPreRender][t_size].rp;
                return null;
            }
            else
            {   if (camerapos_cache_PTR.ContainsKey(CameraIndexPreRender) &&
                        camerapos_cache_PTR[CameraIndexPreRender].ContainsKey(y_pos) &&
                        camerapos_cache_PTR[CameraIndexPreRender][y_pos].ContainsKey(t_size) &&
                        camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp != null)
                    return camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp;
                return null;
            }
        }
        public static void SET_PLAY_FRAME(int CameraInder, int y_pos, int t_size, FastWaterModel20Controller c, RenderTexturePair value)
        {   TryRegistrateController(y_pos, t_size, c);
            if (compilerCollection[c.compiler.cID][y_pos].Count == 1)
            {   if (!size_cache_PTR.ContainsKey(CameraIndexPreRender))
                    size_cache_PTR.Add(CameraIndexPreRender, new Dictionary<int, ccptr>());
                if (!size_cache_PTR[CameraIndexPreRender].ContainsKey(t_size))
                    size_cache_PTR[CameraIndexPreRender].Add(t_size, new ccptr() { rp = value });
                else
                    size_cache_PTR[CameraIndexPreRender][t_size].rp = value;
                //else throw new System.Exception("Assign play texture size render texture already exist - " + c.gameObject.name);
            }
            else
            {   if (!camerapos_cache_PTR.ContainsKey(CameraIndexPreRender))
                    camerapos_cache_PTR.Add(CameraIndexPreRender, new Dictionary<int, Dictionary<int, ccptr>>());
                if (!camerapos_cache_PTR[CameraIndexPreRender].ContainsKey(y_pos))
                    camerapos_cache_PTR[CameraIndexPreRender].Add(y_pos, new Dictionary<int, ccptr>());
                if (!camerapos_cache_PTR[CameraIndexPreRender][y_pos].ContainsKey(t_size))
                    camerapos_cache_PTR[CameraIndexPreRender][y_pos].Add(t_size, new ccptr() { rp = value });
                else
                    camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp = value;
                // else throw new System.Exception("Assign play texture y_po render texture already exist - " + c.gameObject.name);
            }
        }
        
        
        static void TryMoveTextureBetweenCaches(int y_pos, int? t_size, FastWaterModel20Controller c)
        {   if (!t_size.HasValue)
                return;
            __TryMoveTextureBetweenCaches(y_pos, t_size.Value, c, ref size_cache_PTR, ref camerapos_cache_PTR);
            #if UNITY_EDITOR
            __TryMoveTextureBetweenCaches(y_pos, t_size.Value, c, ref foreverfix_size_cache_PTR, ref foreverfix_camerapos_cache_PTR);
            #endif
        }
        static void __TryMoveTextureBetweenCaches(int y_pos, int t_size, FastWaterModel20Controller c, ref Dictionary<int, Dictionary<int, ccptr>> sized_dic,
                ref Dictionary<int, Dictionary<int, Dictionary<int, ccptr>>> yposed_dic)
        {   if (!compilerCollection.ContainsKey(c.compiler.cID))
                return;
            if (!compilerCollection[c.compiler.cID].ContainsKey(y_pos))
                return;
            if (compilerCollection[c.compiler.cID][y_pos].Count <= 1)
            {   foreach (var cameraDic in yposed_dic)
                {   if (cameraDic.Value.ContainsKey(y_pos) && cameraDic.Value[y_pos].ContainsKey(t_size) && cameraDic.Value[y_pos][t_size].rp != null)
                    {   DESTORY(cameraDic.Value[y_pos][t_size].rp.texture);
                        DESTORY(cameraDic.Value[y_pos][t_size].rp.texture_swap);
                        DESTORY(cameraDic.Value[y_pos][t_size].rp.texture_gles20);
                        cameraDic.Value[y_pos].Remove(t_size);
                        if (cameraDic.Value[y_pos].Count == 0)
                            cameraDic.Value.Remove(y_pos);
                    }
                }
            }
            else
            {   foreach (var cameraDic in sized_dic)
                {   if (cameraDic.Value.ContainsKey(t_size) &&
                            cameraDic.Value[t_size].rp != null)
                    {   DESTORY(cameraDic.Value[t_size].rp.texture);
                        DESTORY(cameraDic.Value[t_size].rp.texture_swap);
                        DESTORY(cameraDic.Value[t_size].rp.texture_gles20);
                        cameraDic.Value.Remove(t_size);
                    }
                }
            }
        }
        
        
        #if UNITY_EDITOR
        // this is part in order to skip to render planar reflection when the application is hidden
        // because in this case the unity does not clear the camera's render buffer so memory may overflow
        static Dictionary<int, Dictionary<int, ccptr>> foreverfix_size_cache_PTR = new Dictionary<int, Dictionary<int, ccptr>>();
        static Dictionary<int, Dictionary<int, Dictionary<int, ccptr>>> foreverfix_camerapos_cache_PTR = new Dictionary<int, Dictionary<int, Dictionary<int, ccptr>>>();
        
        public static RenderTexturePair GET_EDITOR_FOREVER(int CameraInder, int y_pos, int t_size, FastWaterModel20Controller c)
        {   TryRegistrateController(y_pos, t_size, c);
            if (compilerCollection[c.compiler.cID][y_pos].Count == 1)
            {   if (foreverfix_size_cache_PTR.ContainsKey(CameraIndexPreRender) &&
                        foreverfix_size_cache_PTR[CameraIndexPreRender].ContainsKey(t_size) &&
                        foreverfix_size_cache_PTR[CameraIndexPreRender][t_size].rp != null)
                    return foreverfix_size_cache_PTR[CameraIndexPreRender][t_size].rp;
                return null;
            }
            else
            {   if (foreverfix_camerapos_cache_PTR.ContainsKey(CameraIndexPreRender) &&
                        foreverfix_camerapos_cache_PTR[CameraIndexPreRender].ContainsKey(y_pos) &&
                        foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos].ContainsKey(t_size) &&
                        foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp != null)
                    return foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp;
                return null;
            }
        }
        
        public static void SET_EDITOR_FOREVER(int CameraInder, int y_pos, int t_size, FastWaterModel20Controller c, RenderTexturePair value)
        {   TryRegistrateController(y_pos, t_size, c);
            if (compilerCollection[c.compiler.cID][y_pos].Count == 1)
            {   if (!foreverfix_size_cache_PTR.ContainsKey(CameraIndexPreRender))
                    foreverfix_size_cache_PTR.Add(CameraIndexPreRender, new Dictionary<int, ccptr>());
                if (!foreverfix_size_cache_PTR[CameraIndexPreRender].ContainsKey(t_size))
                    foreverfix_size_cache_PTR[CameraIndexPreRender].Add(t_size, new ccptr() { rp = value });
                else
                    foreverfix_size_cache_PTR[CameraIndexPreRender][t_size].rp = value;
                //else throw new System.Exception("Assign editor size forever render texture already exist - " + c.gameObject.name);
            }
            else
            {   if (!foreverfix_camerapos_cache_PTR.ContainsKey(CameraIndexPreRender))
                    foreverfix_camerapos_cache_PTR.Add(CameraIndexPreRender, new Dictionary<int, Dictionary<int, ccptr>>());
                if (!foreverfix_camerapos_cache_PTR[CameraIndexPreRender].ContainsKey(y_pos))
                    foreverfix_camerapos_cache_PTR[CameraIndexPreRender].Add(y_pos, new Dictionary<int, ccptr>());
                if (!foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos].ContainsKey(t_size))
                    foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos].Add(t_size, new ccptr() { rp = value });
                else
                    foreverfix_camerapos_cache_PTR[CameraIndexPreRender][y_pos][t_size].rp = value;
                // else throw new System.Exception("Assign editor y_pos forever render texture already exist - " + c.gameObject.name);
            }
        }
        #endif
        
    }
    
    int? lastPlanarReflectionSize;
    int getHash { get { return __getHash ?? (__getHash = Mathf.RoundToInt(transform.position.y * 10)).Value; } }
    int? __getHash;
    public void DoPlanarReflection(int m_TextureSize)
    {   //return;
    
        if (m_renderCamera.gameObject.layer == 7)
            return;
            
        int frame = 0;
        #if UNITY_EDITOR
        var timeW = Application.isPlaying ? (Time.time) : (float)((EditorApplication.timeSinceStartup) % (int.MaxValue / 2));
        if (Application.isPlaying) frame = Time.frameCount;
        else frame = Mathf.RoundToInt( timeW * 60);
        #else
        frame = Time.frameCount;
        #endif
        
        
        
        
        if (m_TextureSize == 0)
            m_TextureSize = 64;
            
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;
        var hash = getHash;
        var M = RENDERER.sharedMaterial;
        
        
        
        #if UNITY_EDITOR
        if (m_renderCamera.name == "SceneCamera" && EditorWindow.mouseOverWindow == null)
        {   //var m_ReflectionTexture = GET_RENDER_TEXTURE(BakeOrUpdateType.Reflection, m_TextureSize, m_renderCamera);
            //if (m_ReflectionTexture != null && compiler.HasProperty(_ReflectionTex_temp)) compiler.SetTexture(_ReflectionTex_temp, m_ReflectionTexture.texture);
            //  if (camerapos_cache_texture.ContainsKey(m_renderCamera) && compiler.HasProperty(_ReflectionTex_temp) && camerapos_cache_texture[m_renderCamera] != null && camerapos_cache_texture[m_renderCamera].texture)
            //      RENDERER.sharedMaterial.SetTexture(_ReflectionTex_temp, camerapos_cache_texture[m_renderCamera].texture);
            //WaterYPosCacher.TryRegistrateController(getHash, lastPlanarReflectionSize, this);
            
            var rp = WaterYPosCacher.GET_EDITOR_FOREVER(CameraIndexPreRender, hash, m_TextureSize, this);
            if (rp != null)
                M.SetTexture(_ReflectionTex_temp, rp.texture);
            // if (forecerfix_camerapos_cache_PTR.ContainsKey(CameraIndex) && forecerfix_camerapos_cache_PTR[CameraIndex].ContainsKey(hash) && forecerfix_camerapos_cache_PTR[CameraIndex][hash].rp != null)
            //     M.SetTexture(_ReflectionTex_temp, forecerfix_camerapos_cache_PTR[CameraIndex][hash].rp.texture);
            
            return;
        }
        #endif
        
        if (m_renderCamera.orthographic)
        {   var r = m_renderCamera.transform.rotation;
            if (r.x == 0 || r.y == 0 || r.z == 0)
                return;
        }
        
        /* if (!camerapos_cache_PTR.ContainsKey(m_renderCamera)) camerapos_cache_PTR.Add(m_renderCamera, camerapos_cache_PTR.Count);
         var camPTR = camerapos_cache_PTR[m_renderCamera];
         if (camerapos_cache_PTR[m_renderCamera] >= camerapos_cache_float.Count) camerapos_cache_float.Add(-123123123);
         if (!camerapos_cache_texture.ContainsKey(m_renderCamera)) camerapos_cache_texture.Add(m_renderCamera, null);*/
        
        //  if (!camerapos_cache_PTR.ContainsKey(CameraIndex)) camerapos_cache_PTR.Add(CameraIndex, new Dictionary<int, ccptr>());
        //  if (!camerapos_cache_PTR[CameraIndex].ContainsKey(hash)) camerapos_cache_PTR[CameraIndex].Add(hash, new ccptr());
        var m_ReflectionTexture = WaterYPosCacher.GET_PLAY_FRAME(CameraIndexPreRender, hash, m_TextureSize, this);
        
        #if UNITY_EDITOR
        //if (!forecerfix_camerapos_cache_PTR.ContainsKey(CameraIndex)) forecerfix_camerapos_cache_PTR.Add(CameraIndex, new Dictionary<int, ccptr>());
        // if (!forecerfix_camerapos_cache_PTR[CameraIndex].ContainsKey(hash)) forecerfix_camerapos_cache_PTR[CameraIndex].Add(hash, new ccptr());
        #endif
        bool needRender = false;
        // if (camerapos_cache_PTR[CameraIndex][hash].rp == null || camerapos_cache_PTR[CameraIndex][hash].rp.resolution != m_TextureSize)
        if (m_ReflectionTexture == null || !m_ReflectionTexture.texture || m_ReflectionTexture.resolution != m_TextureSize)
        {   // Debug.Log(CameraIndex + " " + Time.frameCount + " " + hash + " " + (camerapos_cache_PTR[CameraIndex][hash].rp != null ? camerapos_cache_PTR[CameraIndex][hash].rp.resolution.ToString() : "null") + " " + gameObject.name + " " + Camera.current.name);
            m_ReflectionTexture = GET_RENDER_TEXTURE(BakeOrUpdateType.Reflection, m_TextureSize, CameraIndexPreRender);
            WaterYPosCacher.SET_PLAY_FRAME(CameraIndexPreRender, hash, m_TextureSize, this, m_ReflectionTexture);
            #if UNITY_EDITOR
            WaterYPosCacher.SET_EDITOR_FOREVER(CameraIndexPreRender, hash, m_TextureSize, this, m_ReflectionTexture);
            #endif
            needRender = true;
        }
        /* {
             if (camerapos_cache_PTR[CameraIndex][hash].rp.resolution != m_TextureSize)
         }
         bool needRender = camerapos_cache_PTR[CameraIndex][hash].rp == null;*/
        bool needAssign = false;
        
        
        if (compiler.PlanarReflectionSkipEveryFrame != 0 && frame % (compiler.PlanarReflectionSkipEveryFrame + 1 ) != 0
            #if UNITY_EDITOR
                //   && WaterYPosCacher.GET_EDITOR_FOREVER(CameraIndexPreRender, hash, m_TextureSize, this) != null
            #endif
           ) needRender = false;
           
           
        //if (Mathf.Abs(camerapos_cache_float[camPTR] - pos.y) > 0.1)
        if (needRender)
        {   // Debug.Log(CameraIndex + " " + Time.frameCount + " " + hash);
            //Debug.L   og(Time.frameCount);
            // if (m_NowReflectionsRendering) return;
            //if ( !Camera.current ) return;
            // camerapos_cache_float[camPTR] = pos.y;
            
            
            if (!RenderReflectionCamera)
            {   GameObject go = new GameObject("_MobileWaterReflectionCamera" + gameObject.name, typeof(Camera), typeof(FlareLayer));
                RenderReflectionCamera = go.GetComponent<Camera>();
                RenderReflectionCameraSkyBox = RenderReflectionCamera.gameObject.AddComponent<Skybox>();
                go.hideFlags = HideFlags.HideAndDontSave;
                go.layer = 7;
            }
            
            lastPlanarReflectionSize = m_ReflectionTexture.resolution;
            
            if (!skybox_cache.ContainsKey(m_renderCamera))
                skybox_cache.Add(m_renderCamera, m_renderCamera.GetComponent<Skybox>());
            if (skybox_cache[m_renderCamera])
            {   RenderReflectionCameraSkyBox.material = skybox_cache[m_renderCamera].material;
                RenderReflectionCameraSkyBox.enabled = skybox_cache[m_renderCamera].enabled;
            }
            else
            {   RenderReflectionCameraSkyBox.material = null;
                RenderReflectionCameraSkyBox.enabled = false;
            }
            
            
            
            /*var old = RENDERER.enabled;
            RENDERER.enabled = false;*/
            
            RenderReflectionCamera.CopyFrom(m_renderCamera);
            RenderReflectionCamera.useOcclusionCulling = false;
            RenderReflectionCamera.gameObject.layer = 7;
            RenderReflectionCamera.allowMSAA = false;
            // RenderReflectionCamera.allowDynamicResolution = false;
            RenderReflectionCamera.farClipPlane = Mathf.Min(10000, m_renderCamera.farClipPlane);
            RenderReflectionCamera.renderingPath = RenderingPath.Forward;
            //RenderReflectionCamera.transform.position = pos;
            // RenderReflectionCamera.transform.rotation = transform.rotation;
            // RenderReflectionCamera.depthTextureMode = DepthTextureMode.None; //RenderReflectionCamera
            RenderReflectionCamera.depthTextureMode = m_renderCamera.depthTextureMode; //RenderReflectionCamera
            
            // RenderReflectionCamera.cullingMask = m_renderCamera.cullingMask;
            RenderReflectionCamera.aspect = m_renderCamera.aspect;
            // RenderReflectionCamera.gameObject.SetActive( true );
            
            var user_mask = (int)VectorToMask(compiler.GetVector(_ReflectionBakeLayersHash));
            RenderReflectionCamera.cullingMask &= m_renderCamera.cullingMask & user_mask & CulMask;
            RenderReflectionCamera.orthographic = m_renderCamera.orthographic;
            RenderReflectionCamera.orthographicSize = m_renderCamera.orthographicSize;
            
            
            // Reflect camera around reflection plane
            
            
            /* if (!m_renderCamera.orthographic || true)*/
            {
            
            
                float d = -Vector3.Dot(normal, pos) - compiler.PlanarReflectionClipPlaneOffset;
                reflectionPlane.Set(normal.x, normal.y, normal.z, d);
                
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                // Vector3 oldpos = RenderReflectionCamera.transform.position;
                newpos = reflection.MultiplyPoint(pos);
                RenderReflectionCamera.worldToCameraMatrix = m_renderCamera.worldToCameraMatrix * reflection;
                
                Vector4 clipPlane = CameraSpacePlane(RenderReflectionCamera, pos, normal, 1.0f);
                RenderReflectionCamera.projectionMatrix = RenderReflectionCamera.CalculateObliqueMatrix(clipPlane);
                
                RenderReflectionCamera.transform.position = newpos;
                euler = m_renderCamera.transform.eulerAngles;
                euler.x = 0;
                RenderReflectionCamera.transform.eulerAngles = euler;
            }
            /* else
             {
                 RenderReflectionCamera.projectionMatrix = m_renderCamera.projectionMatrix;
                 var Y = (m_renderCamera.transform.position.y - transform.position.y) * 2;
                 RenderReflectionCamera.transform.position = m_renderCamera.transform.position - new Vector3(0, Y, 0);
                 RenderReflectionCamera.transform.rotation = m_renderCamera.transform.rotation;
                 RenderReflectionCamera.transform.Rotate(RenderReflectionCamera.transform.right, -Vector3.Angle(Vector3.forward, RenderReflectionCamera.transform.forward) * 2);
             }*/
            
            /*  if (m_renderCamera.orthographic)
              {
                  Matrix4x4 scaleOffset = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
                  Vector3 scale = transform.lossyScale;
                  Matrix4x4 mtx = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z));
                  mtx = scaleOffset * RenderReflectionCamera.projectionMatrix * RenderReflectionCamera.worldToCameraMatrix * mtx;
                  M.SetMatrix("_ProjMatrix", mtx);
              }*/
            
            if (Mathf.Abs(newpos.y - pos.y) > 0.001)
            {   needAssign = true;
            
                RenderReflectionCamera.enabled = true;
                if (RenderReflectionCamera.targetTexture != m_ReflectionTexture.texture)
                    RenderReflectionCamera.targetTexture = m_ReflectionTexture.texture;
                GL.invertCulling = true;
                //RenderReflectionCamera.RenderDontRestore();
                var ol = gameObject.layer;
                gameObject.layer = 7;
                RenderReflectionCamera.Render();
                gameObject.layer = ol;
                GL.invertCulling = false;
                
                RenderReflectionCamera.targetTexture = null;
                RenderReflectionCamera.enabled = false;
                
                //camerapos_cache_PTR[CameraIndex][hash].rp = m_ReflectionTexture;
                WaterYPosCacher.SET_PLAY_FRAME(CameraIndexPreRender, hash, m_TextureSize, this, m_ReflectionTexture);
                #if UNITY_EDITOR
                //forecerfix_camerapos_cache_PTR[CameraIndex][hash].rp = m_ReflectionTexture;
                WaterYPosCacher.SET_EDITOR_FOREVER(CameraIndexPreRender, hash, m_TextureSize, this, m_ReflectionTexture);
                #endif
            }
            
            //GetComponent<Camera>().getste
            
            //RenderReflectionCamera.cullingMask |= (int.MaxValue & ~(LayerMask.GetMask( "UI" ) | LayerMask.GetMask( "Water" )));
            // RenderReflectionCamera.cullingMask = (RenderReflectionCamera.cullingMask & ~(LayerMask.GetMask( "UI" ) | LayerMask.GetMask( "Water" )));
            //m_ReflectionTexture.lastCamera = RenderReflectionCamera;
            
            
            
            //  RenderReflectionCamera.transform.position = oldpos;
            
            
            // RENDERER.enabled = old;
            
        }
        
        
        
        
        /* var M = RENDERER.sharedMaterial;
         if (camerapos_cache_texture[m_renderCamera] != null) M.SetTexture(_ReflectionTex_temp, camerapos_cache_texture[m_renderCamera].texture);
         if (camerapos_cache_texture[m_renderCamera] != null) compiler.SetTexture(_ReflectionTex_temp, camerapos_cache_texture[m_renderCamera].texture);
         return;*/
        
        if (compiler.HasProperty(_ReflectionTex_temp))
        {
        
            // V3 //////////////////
            var rp = WaterYPosCacher.GET_PLAY_FRAME(CameraIndexPreRender, hash, m_TextureSize, this);
            if (needAssign)
                M.SetTexture(_ReflectionTex_temp, rp.texture);
            else if (rp != null)
                M.SetTexture(_ReflectionTex_temp, rp.texture);
                
            // V2 //////////////////
            /*if (needAssign)
                M.SetTexture(_ReflectionTex_temp, camerapos_cache_PTR[CameraIndex][hash].rp.texture);
            else if (camerapos_cache_PTR.ContainsKey(CameraIndex) && camerapos_cache_PTR[CameraIndex].ContainsKey(hash) && camerapos_cache_PTR[CameraIndex][hash].rp != null)
                M.SetTexture(_ReflectionTex_temp, camerapos_cache_PTR[CameraIndex][hash].rp.texture);*/
            
            // V1 //////////////////
            /*var M = RENDERER.sharedMaterial;
            if (needAssign)
            {
                M.SetTexture(_ReflectionTex_temp, camerapos_cache_texture[m_renderCamera].texture);
            }
            else if (camerapos_cache_texture.ContainsKey(m_renderCamera) && camerapos_cache_texture[m_renderCamera] != null) M.SetTexture(_ReflectionTex_temp, camerapos_cache_texture[m_renderCamera].texture);
            */
        }
        
        
        //RenderReflectionCamera.clearStencilAfterLightingPass = true;
        // RenderReflectionCamera.RemoveAllCommandBuffers();
        
        /*if (!Application.isPlaying)
        {
            DESTORY(m_ReflectionTexture.texture);
        }*/
        //RenderTexture.ReleaseTemporary(RenderReflectionCamera.targetTexture);
        
        //RenderReflectionCamera.targetTexture = null;
        // RenderReflectionCamera.gameObject.SetActive( false );
        //m_NowReflectionsRendering = false;
    }
    //---------------------------------------------------------------------
    #endregion//------INITIALIZATION
    //---------------------------------------------------------------------
    
    
    
    
    
    
    
    //---------------------------------------------------------------------
    #region//------MIRROR REFLECTION 4
    //---------------------------------------------------------------------
    private static float sgn(float a)
    {   if (a > 0.0f)
            return 1.0f;
        if (a < 0.0f)
            return -1.0f;
        return 0.0f;
    }
    // Given position/normal of the plane, calculates plane in camera space.
    
    // float PlanarReflectionClipPlaneOffset = -15.55f;
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {   Vector3 offsetPos = pos + normal * compiler.PlanarReflectionClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
    // Adjusts the given projection matrix so that near plane is the given clipPlane
    // clipPlane is given in camera space. See article in Game Programming Gems 5 and
    // http://aras-p.info/texts/obliqueortho.html
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {   Vector4 q = projection.inverse * new Vector4(
                        sgn(clipPlane.x),
                        sgn(clipPlane.y),
                        1.0f,
                        1.0f
                    );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }
    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {   reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);
        
        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);
        
        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);
        
        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
        
        
    }
    //---------------------------------------------------------------------
    #endregion//------MIRROR REFLECTION 4
    //---------------------------------------------------------------------
    
    
    
    
    
    //---------------------------------------------------------------------
    //------all the rest below - are strange unnecessary editor functions
    //---------------------------------------------------------------------
    #if UNITY_EDITOR
    //[HideInInspector]
    //[System.NonSerialized]
    //bool _validate = false;
    public bool Validate()
    {   var target = (FastWaterModel20Controller)this;
        if (!target.compiler)
        {   var renderer = target.gameObject.GetComponent<Renderer>();
            if (!renderer)
            {   EditorGUILayout.HelpBox("MeshRenderer not assigned", MessageType.Error);
                if (GUILayout.Button("Assign MeshRenderer"))
                    SetDirty(Undo().gameObject.AddComponent<MeshRenderer>().gameObject);
                return false;
            }
            
            GUILayout.Space(10);
            var oldM = target.GetComponent<Renderer>() ? target.compiler : null;
            var newM = EditorGUILayout.ObjectField(oldM, typeof(FastWaterModel20Compiler), false, GUILayout.Height(FastWaterModel20ControllerEditor.H)) as FastWaterModel20Compiler;
            if (newM != oldM)
            {   UnityEditor.Undo.RecordObject(target, "Set Material");
                UnityEditor.Undo.RecordObject(target.GetComponent<Renderer>(), "Set Material");
                target.compiler = newM;
                FastWaterModel20Controller.SetDirty(target);
                FastWaterModel20Controller.SetDirty(target.GetComponent<Renderer>());
            }
            
            if (!compiler)
            {   EditorGUILayout.HelpBox("Material Not Assigned", MessageType.Error);
            
                if (GUILayout.Button("Assign Material"))
                {   return AssignMaterial();
                }
            }
            else
            {
            
            }
            
            /* EditorGUILayout.HelpBox("Material Not Assigned / Or Shader Not Support\nIf you modified water shader, add new shader's name to the 'supported_sahders' variable", MessageType.Error);
             GUILayout.Label("Supported Shaders");
             EditorGUILayout.HelpBox(target.supported_sahders.Select(s => s + "*").Aggregate((a, b) => a + "\n" + b), MessageType.None);
             if (GUILayout.Button("Assign Material"))
             {
                 return AssignMaterial();
             }*/
            return false;
        }
        if (!target.compiler.material || !target.compiler.shader)
        {   EditorGUILayout.HelpBox("Material Not Supported", MessageType.Error);
            if (GUILayout.Button("Assign new Material"))
            {   return AssignMaterial();
            }
            if (GUILayout.Button("Compile current Material"))
            {   target.Undo();
                target.compiler = target.CreateAsset(RENDERER.sharedMaterial);
                target.SetDirty();
            }
            return false;
        }
        if (FastWaterModel20Compiler.EnableAutoREcompileprefs_EnableAutoREcompileEMFastWaterModel20CompilerValidate() && FastWaterModel20Compiler.EnableAutoREcompileValidate())
        {   if (target.compiler && target.compiler.shader && GUILayout.Button("Recompile material"))
            {   // target.Undo();
                //FastWaterModel20Controller.UpdateShaderAsset(target.compiler.shader, target.compiler.CreateShaderString());
                target.compiler.UpdateShaderAsset();
                //target.compiler = target.CreateAsset(REN_DERER.sharedMaterial);
                // target.SetDirty();
            }
        }
        return true;
    }
    
    public Component Undo(Component compe)
    {   UnityEditor.Undo.RecordObject(compe, "Change EM Water");
        return compe;
    }
    
    
    public FastWaterModel20Controller Undo(bool undoShader = false)
    {   //UnityEditor.Undo.SetCurrentGroupName( "Change Mobile Water" );
    
        //  UnityEditor.Undo.RecordObjects( new[] { (UnityEngine.Object)this.gameObject, this, this.material }, "Change Mobile Water" );
        //UnityEditor.Undo.RecordObject( this.material, "Change Mobile Water" );
        // UnityEditor.Undo.CollapseUndoOperations( UnityEditor.Undo.GetCurrentGroup() );
        // UnityEditor.Undo.FlushUndoRecordObjects();
        
        UnityEditor.Undo.RecordObject(this.gameObject, "Change EM Water");
        UnityEditor.Undo.RecordObject(this, "Change EM Water");
        if (this.compiler)
        {   UnityEditor.Undo.RecordObject(this.compiler, "Change EM Water");
            if (undoShader)
                UnityEditor.Undo.RecordObject(this.compiler.shader, "Change EM Water");
            UnityEditor.Undo.RecordObject(this.compiler.material, "Change EM Water");
        }
        return this as FastWaterModel20Controller;
    }
    
    [HideInInspector]
    [System.NonSerialized]
    public bool skipClamp = false;
    public void SetDirty()
    {   if (!this)
            return;
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(((FastWaterModel20Controller)this).gameObject);
        //Debug.Log("ASD");
        if (((FastWaterModel20Controller)this).compiler)
        {   EditorUtility.SetDirty(((FastWaterModel20Controller)this).compiler);
            EditorUtility.SetDirty(((FastWaterModel20Controller)this).compiler.shader);
            EditorUtility.SetDirty(((FastWaterModel20Controller)this).compiler.material);
            if (editor_cache_matcollector.ContainsKey(compiler.cID))
            {   foreach (var item in editor_cache_matcollector[compiler.cID])
                {   if (item)
                    {   if (Application.isPlaying)
                            Destroy(item);
                        else
                            DestroyImmediate(item);
                    }
                }
                editor_cache_matcollector[compiler.cID].Clear();
                var source = editor_cache_gocollector[compiler.cID].ToList();
                foreach (var item in editor_cache_gocollector[compiler.cID])
                {   if (item)
                    {   editor_cache_materials.Remove(item.goID);
                    }
                }
                editor_cache_gocollector.Remove(compiler.cID);
                foreach (var s in source)
                    s.TryCreateTempMaterials();
            }
        }
        skipClamp = true;
        
    }
    public static void SetDirty(GameObject go)
    {   EditorUtility.SetDirty(go);
    }
    public static void SetDirty(UnityEngine.Object go)
    {   EditorUtility.SetDirty(go);
    }
    
    /*internal const string __SHADER = "EM-X/" + NAME + " v1 Shader";
    
    internal const string __SHADER_O = "Opaque";
    internal const string __SHADER_T = "Transparent";
    internal const string __SHADER_GP = "GrabPass";
    internal const string __SHADER_noGP = "NoGrabPass";
    
    const string SHADER_T_GP = __SHADER + " ( " + __SHADER_T + ", " + __SHADER_GP + " )";*/
    /* const string SHADER_O_GP = __SHADER +" ( " + __SHADER_O + ", "+__SHADER_GP+" )";
     const string SHADER_T = __SHADER + " ( "+__SHADER_T+" )";
     const string SHADER_O = __SHADER +" ( " + __SHADER_O + " )";*/
    
    public bool AssignMaterial()
    {   //const string PL_NAME = "Fast Water Model 2.0";
        // const string PL_NAME = NAME;
        const string DEFAULT_MATERIAL = "Fast Water Material - Default";
        // const string PL_NAME = "Water Target 2.0";
        // var renderer = gameObject.GetComponent<Renderer>();
        
        FastWaterModel20Compiler resultMaterial = null;
        var lastUsedGUID = EditorPrefs.GetString("EModules/Water20/LAST_MATERIAL", "");
        if (!string.IsNullOrEmpty(lastUsedGUID))
        {   var path = AssetDatabase.GUIDToAssetPath(lastUsedGUID);
            if (!string.IsNullOrEmpty(path))
            {   resultMaterial = AssetDatabase.LoadAssetAtPath<FastWaterModel20Compiler>(path);
            }
        }
        
        if (!resultMaterial)
        {   // var findedMat = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.Contains(PL_NAME + " Material") && p.ToLower().EndsWith(".asset"));
            var findedMat = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.Contains(DEFAULT_MATERIAL) && p.ToLower().EndsWith(".asset"));
            
            if (!string.IsNullOrEmpty(findedMat) && AssetDatabase.LoadAssetAtPath<FastWaterModel20Compiler>(findedMat))
            {   resultMaterial = AssetDatabase.LoadAssetAtPath<FastWaterModel20Compiler>(findedMat);
            }
            else
            {   /* resultMaterial = new Material(shader);
                 resultMaterial.name = "New " + PL_NAME;
                 var newPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + resultMaterial.name + ".mat");
                 AssetDatabase.CreateAsset(resultMaterial, newPath);
                 AssetDatabase.SaveAssets();
                 AssetDatabase.Refresh();
                 AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceUpdate);*/
                CreateAsset();
            }
        }
        if (resultMaterial != null)
        {   compiler = resultMaterial;
        }
        return compiler;
    }
    
    /*public void CheckMaterial()
    {
        if (!compiler) return;
        if (!compiler.material) return;
        if (compiler.material != RE_NDERER.sharedMaterial)
        {
            Undo(RENDERER);
            RE_NDERER.sharedMaterial = compiler.material;
            SetDirty();
            need_update = true;
        }
    }*/
    
    public void __CheckShader()
    {   if (Application.isPlaying)
            return;
        var c = compiler;
        if (!c)
            return;
        if (!c.material)
            return;
        if (c.material.shader != c.shader)
        {   c.material.shader = c.shader;
            // Debug.LogWarning(c.name + " - The shader for the material was recovered");
        }
        if (!RENDERER)
            return;
        if (!RENDERER.sharedMaterial)
            return;
        if (RENDERER.sharedMaterial.shader != c.shader)
        {   RENDERER.sharedMaterial.shader = c.shader;
            // Debug.LogWarning(c.name + " - The shader for the material was recovered");
        }
    }
    
    void TryCreateTempMaterials()
    {   if (!this || !compiler)
            return;
            
        __CheckShader();
        Material newMat = null;
        if (!editor_cache_materials.ContainsKey(goID))
            editor_cache_materials.Add(goID, newMat = CreateMaterial());
        else if (!editor_cache_materials[goID])
            editor_cache_materials[goID] = newMat = CreateMaterial();
        else if (compiler.shader != editor_cache_materials[goID].shader)
        {   DESTORY(editor_cache_materials[goID]);
            editor_cache_materials[goID] = newMat = CreateMaterial();
        }
        if (newMat)
        {   need_update = false;
        }
        if (RENDERER && RENDERER.sharedMaterial != editor_cache_materials[goID])
        {   RENDERER.sharedMaterial = editor_cache_materials[goID];
        }
    }
    
    Material CreateMaterial()
    {   //var ov = compiler.OverrideMaterial;
        // compiler.OverrideMaterial = null;
        var mat = new Material(compiler.material);
        mat.hideFlags = HideFlags.HideAndDontSave;
        
        mat.name = compiler.material.name + " Instance";
        // compiler.OverrideMaterial = ov;
        if (!editor_cache_matcollector.ContainsKey(compiler.cID))
            editor_cache_matcollector.Add(compiler.cID, new List<Material>());
        editor_cache_matcollector[compiler.cID].Add(mat);
        if (!editor_cache_gocollector.ContainsKey(compiler.cID))
            editor_cache_gocollector.Add(compiler.cID, new List<FastWaterModel20Controller>());
            
        editor_cache_gocollector[compiler.cID].Add(this);
        return mat;
    }
    
    
    
    
    
    /*
        var T_O = IS_OPAQUE ? FastWaterModel20Controller.__SHADER_O : FastWaterModel20Controller.__SHADER_T;
        var GP_noGP = IS_GRABPASS ? FastWaterModel20Controller.__SHADER_GP : FastWaterModel20Controller.__SHADER_noGP;
        var oldMat = target.compiler.shader.name;
        var newMat = FastWaterModel20Controller.__SHADER + " ( " + T_O + ", " + GP_noGP + " )";
        if (oldMat != newMat)
        {
            var newShader = Shader.Find(newMat);
            if (newShader == null)
            {
                Debug.LogWarning("Cannot find shader: " + newMat);
            }
            else
            {
                target.Undo();
                target.compiler.shader = newShader;
                target.SetDirty();
            }
        }*/
    #endif
}



}


