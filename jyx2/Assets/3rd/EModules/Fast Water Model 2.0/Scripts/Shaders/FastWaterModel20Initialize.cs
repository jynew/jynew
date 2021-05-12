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

    [System.NonSerialized]
    int? __goID;
    int goID
    {   get { return __goID ?? (__goID = Application.isPlaying ? -1 : gameObject.GetInstanceID()).Value; }
        set { __goID = value;}
    }
    
    
    public System.Action < int, int?, FastWaterModel20Controller > OnBecameInVisibleAction;
    bool IsVisible = true;
    private void OnBecameInvisible()
    {   IsVisible = false;
        if (!Application.isPlaying)
            return;
        if (OnBecameInVisibleAction != null)
            OnBecameInVisibleAction(getHash, lastPlanarReflectionSize, this);
        FastWaterModel20FrameBuffer.DeRequestorRuntime(this);
    }
    private void OnBecameVisible()
    {   IsVisible = true;
        if (!Application.isPlaying)
            return;
        WaterYPosCacher.TryRegistrateController(getHash, null, this);
    }
    
    
    
    bool wasAwake = false;
    private void Start()
    {   // CloneMaterial();
    
    
        wasAwake = true;
        RenderOnAwake();
        
        
        
    }
    
    
    
    #if UNITY_EDITOR
    
    
    public static void undoRedoPerfowrm()
    {   if (!mainThread.Equals(System.Threading.Thread.CurrentThread)) return;
        #if FASTWATER20_LOG
        Debug.Log("undoRedoPerfowrm");
        #endif
        
        foreach (var list in editor_cache_matcollector)
        {   foreach (var item in list.Value)
            {   if (item)
                {   if (Application.isPlaying)
                    {/*Destroy(item);*/ }
                    else
                        DestroyImmediate(item);
                }
            }
            list.Value.Clear();
        }
        var resList = editor_cache_materials.Keys.ToArray();
        foreach (var list in editor_cache_gocollector)
        {   foreach (var item in list.Value)
            {   if (item)
                {   editor_cache_materials.Remove(item.goID);
                
                }
            }
            list.Value.Clear();
        }
        WaterYPosCacher.CLEAR_ALL();
        foreach (var item in resList)
        {   if (id_to_controller.ContainsKey(item)  && id_to_controller[item])
                id_to_controller[item].TryCreateTempMaterials();
        }
        
        // Debug.Log( UnityEditor.Undo.() );
    }
    
    
    
    
    static System.Threading.Thread  mainThread = System.Threading.Thread.CurrentThread;
    static Dictionary<int, FastWaterModel20Controller> id_to_controller = new Dictionary<int, FastWaterModel20Controller>();
    #endif
    private void OnEnable()
    {
    
        if (!this || !enabled) return;
        
        // if (_was_enables) return;
        _was_enables = true;
        
        __this = this;
        if (!__goID.HasValue) __goID = gameObject.GetInstanceID();
        #if FASTWATER20_LOG
        Debug.Log("OnEnable " + gameObject.name);
        #endif
        
        #if UNITY_EDITOR
        if (mainThread == null) mainThread = System.Threading.Thread.CurrentThread;
        // if (!mainThread.Equals(System.Threading.Thread.CurrentThread)) return;
        
        if (!id_to_controller.ContainsKey(goID)) id_to_controller.Add(goID, this);
        else id_to_controller[goID] = this;
        
        need_update = true;
        // DidReload();
        #endif
        
        
        
        if (!__renderer)
            __renderer = GetComponent<MeshRenderer>();
            
        if (__compiler)
        {   __compiler.onShaderUpdate -= onShaderUpdate;
            __compiler.onShaderUpdate += onShaderUpdate;
            if (!__compiler.__cID.HasValue)  __compiler.__cID = GetInstanceID();
            
            if (__renderer)
            {   __renderer.receiveShadows = __compiler.IsKeywordEnabled("USE_SHADOWS");
            }
        }
        
        
        
        #if UNITY_EDITOR
        
        
        TryCreateTempMaterials();
        
        
        
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
        EditorApplication.update -= static_Update;
        EditorApplication.update += static_Update;
        
        EditorApplication.playModeStateChanged -= onPlayModeChange;
        EditorApplication.playModeStateChanged += onPlayModeChange;
        
        
        OnEnable2();
        
        
        
        #endif
        Camera.onPostRender -= OnPostRenderMy;
        Camera.onPostRender += OnPostRenderMy;
        Camera.onPreRender -= OnPreRenderMy;
        Camera.onPreRender += OnPreRenderMy;
        
        WaterYPosCacher.TryRegistrateController(getHash, null, this);
        
        
        #if DEBUG_BAKED_TEXTURE
        lastTarget = this;
        SceneView.onSceneGUIDelegate -= sv;
        SceneView.onSceneGUIDelegate += sv;
        #endif
        
        #if UNITY_EDITOR
        UnityEditor.Undo.undoRedoPerformed -= Reset;
        UnityEditor.Undo.undoRedoPerformed += Reset;
        
        UnityEditor.Undo.undoRedoPerformed -= RepantInspector;
        UnityEditor.Undo.undoRedoPerformed += RepantInspector;
        
        UnityEditor.Undo.undoRedoPerformed -= undoRedoPerfowrm;
        UnityEditor.Undo.undoRedoPerformed += undoRedoPerfowrm;
        #endif
        
        CalcVertexSize();
        
        if (!DirectionLight)
        {   var light = GameObject.FindObjectOfType<Light>();
            if (light)
                DirectionLight = light.transform;
            else
                DirectionLight = null;
            #if UNITY_EDITOR
            SetDirty(this);
            #endif
        }
        
        /* if (!compiler)
         {
             if (RENDERER) compiler = RE_NDERER.sharedMaterial;
             else compiler = null;
         }*/
        
        
        Reset();
        
        if (MESH)            CurrentMeshData = GET_MESH(MESH.sharedMesh);
        
        //if (Application.isPlaying && wasAwake) RenderOnAwake();
        
        if (Application.isPlaying && wasAwake)
            Start();
    }
    
    
    
    
#pragma warning disable
    bool _was_enables = false;
#pragma warning restore
    void OnDisable()
    {   if (!this || !enabled) return;
        if (OnDisableAction != null)
            OnDisableAction(getHash, lastPlanarReflectionSize, this);
        FastWaterModel20FrameBuffer.DestroyAllScript();
        
        
        
        
        #if UNITY_EDITOR
        // if (!mainThread.Equals(System.Threading.Thread.CurrentThread)) return;
        /* if (!Applicati   on.isPlaying)
         {   if (!initFlag)
             {   initFlag = true;*/
        // if (!Application.isPlaying) DestroyAllEditorDatasDeep();
        /*  }
        }*/
        #endif
        // if (gameObject.activeSelf) return;
        _was_enables = false;
        
        if (__compiler)
        {   __compiler.onShaderUpdate -= onShaderUpdate;
        
        }
        
        #if FASTWATER20_LOG
        Debug.Log("OnDisable " + gameObject.name);
        #endif
        #if UNITY_EDITOR
        OnDisable2();
        
        
        EditorApplication.update -= EditorUpdate;
        // RestoreMat();
        #endif
        Camera.onPostRender -= OnPostRenderMy;
        Camera.onPreRender -= OnPreRenderMy;
        
        
        
        if (RenderDepthOrRefractionCamera)
            DESTROY_CAMERA(RenderDepthOrRefractionCamera);
        if (RenderReflectionCamera)
            DESTROY_CAMERA(RenderReflectionCamera);
            
            
        destroyHelper.Push(goID);
        
        if (__compiler)
        {   /* if (cache_GET_RENDER_TEXTURE.ContainsKey(goID))
             {
            
                 foreach (var itemList in cache_GET_RENDER_TEXTURE[goID])
                 {   foreach (var item in itemList.Value)
                     {   if (item == null)
                             continue;
                         if (item.lastCamera)
                             item.lastCamera.targetTexture = null;
                         if (item.texture_swap)
                             DESTORY(item.texture_swap);
                         if (item.texture_gles20)
                             DESTORY(item.texture_gles20);
                     }
                     itemList.Value.Clear();
                     // item.Value.lastCamera
                 }
             }*/
            
            
            /* #if UNITY_EDITOR
             if (editor_cache_materials.ContainsKey(goID))
             {   //Debug.Log("ASD");
                 if (editor_cache_materials[goID])
                 {   Destroy(editor_cache_materials[goID].GetTexture(_ReflectionTex_temp), 5);
                     Destroy(editor_cache_materials[goID].GetTexture(_RefractionTex_temp), 5);
                     Destroy(editor_cache_materials[goID].GetTexture(_BakedData_temp), 5);
                     // DESTORY(editor_cache_materials[goID].GetTexture(_ReflectionTex_temp));
                     // DESTORY(editor_cache_materials[goID].GetTexture(_RefractionTex_temp));
                     // DESTORY(editor_cache_materials[goID].GetTexture(_BakedData_temp));
                     / * var t1 = editor_cache_materials[goID].GetTexture(_ReflectionTex_temp) as RenderTexture;
                      if (t1) t1.DiscardContents(true, true);
                      if (t1) t1.Release();
                      var t2 = editor_cache_materials[goID].GetTexture(_RefractionTex_temp) as RenderTexture;
                      if (t2) t2.DiscardContents(true, true);
                      if (t2) t1.Release();
                      var t3 = editor_cache_materials[goID].GetTexture(_BakedData_temp) as RenderTexture;
                      if (t3)  t3.DiscardContents(true, true);
                      if (t3) t1.Release();* /
            
                     //Unity 2018.2f14 crashed when scene was changing and had paticles emmiter
                     // if (!Application.isPlaying) DESTORY(editor_cache_materials[goID]);
                     //Unity 2018.2f14 crashed when scene was changing and had paticles emmiter
                 }
                 editor_cache_materials.Remove(goID);
             }
             #else
             DESTORY(__compiler.material.GetTexture(_ReflectionTex_temp));
             DESTORY(__compiler.material.GetTexture(_RefractionTex_temp));
             DESTORY(__compiler.material.GetTexture(_BakedData_temp));
             #endif*/
            
            
        }
        
        //Unity 2018.2f14 crashed when scene was changing and had paticles emmiter
        /* DESTORY(DepthRenderShader_SecondPass_Matertial);
         if (__renderer && __renderer.sharedMaterial && (__renderer.sharedMaterial.hideFlags & HideFlags.HideAndDontSave) != 0)
             DESTORY(__renderer.sharedMaterial);*/
        //Unity 2018.2f14 crashed when scene was changing and had paticles emmiter
        
    }
    
    static DestroyHelper __destroyHelper ;
    static DestroyHelper destroyHelper { get { return __destroyHelper ?? (__destroyHelper = new DestroyHelper()); } }
    class DestroyHelper {
        List<int> target = null;
        public DestroyHelper()
        {   /*   string read = null;
               if (Application.isPlaying) read = PlayerPrefs.GetString("EMX/WaterModel20/DestroyHelper", null);
               else read = EditorPrefs.GetString("EMX/WaterModel20/DestroyHelper", null);*/
            
            string read = PlayerPrefs.GetString("EMX/WaterModel20/DestroyHelper", null);
            if (!string.IsNullOrEmpty(read)) target = read.Split(' ').Select(s => int.Parse(s)).ToList();
        }
        public void Push(int hash)
        {   if (target == null) target = new List<int>();
            if (target.Contains(hash)) return;
            target.Add(hash);
            PlayerPrefs.SetString("EMX/WaterModel20/DestroyHelper", target.Select(a => a.ToString()).Aggregate((a, b) => a + " " + b));
            PlayerPrefs.Save();
        }
        public void Destroy()
        {   if (target == null) return;
            foreach (var t in target)
            {   DESTORY(  GET_RENDER_TEXTURE_BYHASH(t, BakeOrUpdateType.Reflection));
                DESTORY(  GET_RENDER_TEXTURE_BYHASH(t, BakeOrUpdateType.Refraction));
                DESTORY(  GET_RENDER_TEXTURE_BYHASH(t, BakeOrUpdateType.ZDepth));
                #if UNITY_EDITOR
                editor_cache_materials.Remove(t);
                // if (!Application.isPlaying) DestroyAllEditorDatasDeep(true);
                #endif
            }
            target = null;
            PlayerPrefs.SetString("EMX/WaterModel20/DestroyHelper", null);
            PlayerPrefs.Save();
        }
    }
    
    
    #if UNITY_EDITOR
    
    void onPlayModeChange(PlayModeStateChange state)
    {   if (state == PlayModeStateChange.EnteredEditMode)
            need_update = true;
    }
    [UnityEditor.Callbacks.DidReloadScripts]
    static void DidReload()
    {   // if (!Application.isPlaying) DestroyAllEditorDatasDeep(true);
        need_update = true;
        //EditorApplication.playModeStateChanged -= playModeStateChanged;
        // EditorApplication.playModeStateChanged += playModeStateChanged;
        
    }
    /*static void playModeStateChanged(PlayModeStateChange state)
    {   if (state != PlayModeStateChange.ExitingEditMode) return;
        DestroyAllEditorDatasDeep(true);
    }*/
    
    #endif
    
    
    
    
    private void Update()
    {   if (!Application.isPlaying)
            return;
        local_Update();
        if (!wasStaticUpdate)
            static_Update();
    }
    
    
    private void LateUpdate()
    {
    
    
        wasStaticUpdate = false;
        if (need_update)
        {
            #if FASTWATER20_LOG
            Debug.Log("local_update");
            #endif
            need_update = false;
        }
        if (local_update)
        {
        
            #if UNITY_EDITOR
            TryCreateTempMaterials();
            #endif
            setter_cache.Clear();
            RenderOnAwake();
            local_update = false;
            #if UNITY_EDITOR
            //  if (!Application.isPlaying)
            {   WaterYPosCacher.CLEAR_ALL();
                CalcVertexSize();
            }
            #endif
            
        }
        
    }
    
    
    
    #if UNITY_EDITOR
    private void OnEnable2()
    {   SceneView.onSceneGUIDelegate -= SV;
        SceneView.onSceneGUIDelegate += SV;
    }
    private void OnDisable2()
    {   SceneView.onSceneGUIDelegate -= SV;
    }
    void SV(SceneView sv)
    {   if (Event.current.type != EventType.Repaint)
            return;
        TryCreateTempMaterials();
    }
    #endif
}
}