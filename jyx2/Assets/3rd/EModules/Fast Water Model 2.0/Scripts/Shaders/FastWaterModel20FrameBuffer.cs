
using UnityEngine;
using System.Collections.Generic;
using  UnityEngine.Rendering;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace EModules.FastWaterModel20 {
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
public class FastWaterModel20FrameBuffer : MonoBehaviour {
    static CameraEvent EventType = CameraEvent.AfterForwardAlpha;
    // static CameraEvent EventType = CameraEvent.BeforeImageEffects;
    public RenderTexture LastTexture = null;
    public UnityEngine.Rendering.CommandBuffer LastBuffer = null;
    public UnityEngine.Rendering.CommandBuffer LastBuffer2 = null;
    public static Dictionary<Camera, RenderTexture> TexturesDic = new Dictionary<Camera, RenderTexture>();
    static Dictionary<Camera, FastWaterModel20FrameBuffer> CamDic = new Dictionary<Camera, FastWaterModel20FrameBuffer>();
    public Camera this_camera = null;
#pragma warning disable
    static  bool TransparentFlag = false;
#pragma warning restore
    
    static Dictionary<FastWaterModel20Controller, bool> _requesorDic = new Dictionary<FastWaterModel20Controller, bool>();
    /*  public static void AddRequestorRuntime(FastWaterModel20Controller controller, Camera c, bool opaque)
      {
      }*/
    public static FastWaterModel20FrameBuffer AddRequestorRuntime(FastWaterModel20Controller controller, Camera c, bool opaque)
    {   if (!opaque) TransparentFlag = true;
    
        if (!_requesorDic.ContainsKey(controller))  _requesorDic.Add(controller, true);
        if (!CamDic.ContainsKey(c) )
        {   foreach (var comp in   c.gameObject.GetComponents<FastWaterModel20FrameBuffer> ())
            {   if (!Application.isPlaying) DestroyImmediate(comp, true);
                else Destroy(comp);
            }
            var bf = c.GetCommandBuffers(EventType);
            foreach (var b in bf.Where(b => b.name == "EMX Water Buffer")) c.RemoveCommandBuffer(EventType, b);
            //var newScript = c.gameObject.GetComponent<FastWaterModel20FrameBuffer>() ?? c.gameObject.AddComponent<FastWaterModel20FrameBuffer>();
            var newScript =  c.gameObject.AddComponent<FastWaterModel20FrameBuffer>();
            newScript.hideFlags = HideFlags.HideAndDontSave;
            newScript.this_camera = c;
            newScript.EnableLast2();
            //newScript._OnEnable();
            /* newScript.LastBuffer = new UnityEngine.Rendering.CommandBuffer() {name = "EMX Water Buffer" };
             newScript.ConfigureCommandBuffer(newScript.LastBuffer);
             //   newScript.LastBuffer.
             c.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterEverything, newScript.LastBuffer); //, UnityEngine.Rendering.ComputeQueueType.Background*/
            // c.AddCommandBufferAsync(UnityEngine.Rendering.CameraEvent.AfterEverything, newScript.LastBuffer); //, UnityEngine.Rendering.ComputeQueueType.Background
            CamDic.Add(c, newScript);
            
        }
        var result = CamDic[c];
        if (result.LastBuffer2 == null)  result.EnableLast2();
        if (result.LastTexture2 == null || result.lastSizex != result.this_camera.pixelWidth || result.lastSizey != result.this_camera.pixelHeight)
        {   result.this_camera.RemoveCommandBuffer( EventType, result.LastBuffer2);
            result.EnableLast2();
        }
        
        // result. CheckLastTex();
        result.   LastTexture2.DiscardContents();
        return result;
    }
    
    
    // private CommandBuffer LastBuffer;
    public RenderTexture LastTexture2 = null;
    //
    //  int texID;
    //  void OnPostRender()
    // {
    
    /*  m_CmdAfterEverything.Clear();
    
    if (postProcessLayer == null || !postProcessLayer.enabled || !postProcessLayer.debugLayer.debugOverlayActive)
      return;
    Deb
    m_CmdAfterEverything.Blit(postProcessLayer.debugLayer.debugOverlayTarget, BuiltinRenderTextureType.CameraTarget);*/
    /* if (LastTexture2 == null)
     {   LastTexture2 =   new RenderTexture(this_camera.pixelWidth, this_camera.pixelHeight, 32)
         {   // isPowerOfTwo = true,
             hideFlags = HideFlags.HideAndDontSave,
             name = "EModules/MobileWater/LastFrame-RenderTexture" + gameObject.name,
             // useMipMap = type == BakeOrUpdateType.Refraction || type == BakeOrUpdateType.ZDepth,
         };
     }*/
    
    
    // texID = Shader.PropertyToID("_LastCopyFrame2");
    //LastBuffer.GetTemporaryRT(texID, this_camera.pixelWidth, this_camera.pixelHeight, 16, FilterMode.Point);
    //LastBuffer.Blit(BuiltinRenderTextureType.CameraTarget, texID);
    /* var targ  = new RenderTargetIdentifier(LastTexture2);
    
     Shader shader = Shader.Find ("Hidden/BlitCopy");
     Material material = new Material (shader);
     LastBuffer.SetRenderTarget(targ, BuiltinRenderTextureType.RenderTexture);
     LastBuffer.SetGlobalTexture ("_MainTex", BuiltinRenderTextureType.CurrentActive);
     LastBuffer.Blit (BuiltinRenderTextureType.CurrentActive, targ);
     // LastBuffer.ReleaseTemporaryRT( texID);
     LastBuffer.SetGlobalTexture("_LastFrame2", texID);*/
    /* if (LastBuffer2 == null) LastBuffer2 = new CommandBuffer();
    
     texID = Shader.PropertyToID("_LastFrame2");
     LastBuffer2.GetTemporaryRT (texID, -1, -1, 0, FilterMode.Bilinear);
     LastBuffer2.Blit (BuiltinRenderTextureType.CurrentActive, texID);*/
    //  }
    Mesh _Quad;
    Vector2 marginOffsetUV = new Vector2(0, 0);
    Mesh Quad
    {   get
        {   if (_Quad == null)
            {   _Quad = new Mesh();
                _Quad.vertices = new Vector3[]
                {   new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0)
                };
                
                _Quad.uv = new Vector2[]
                {   new Vector2(marginOffsetUV.x, marginOffsetUV.y),
                    new Vector2(1 - marginOffsetUV.x, marginOffsetUV.y),
                    new Vector2(1 - marginOffsetUV.x, 1 - marginOffsetUV.y),
                    new Vector2(marginOffsetUV.x, 1 - marginOffsetUV.y)
                };
                
                _Quad.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                _Quad.UploadMeshData(false);
                _Quad.name = "full screen quad";
            }
            return _Quad;
        }
    }
    
    
    public static void DeRequestorRuntime(FastWaterModel20Controller controller)
    {   if (_requesorDic.Count == 0 ) return;
        _requesorDic.Remove(controller);
        if (_requesorDic.Count == 0)
        {   DestroyAllScript();
        }
    }
    
    
    void _OnEnable()
    {   if (LastBuffer == null)
        {   /*LastBuffer.GetTemporaryRT(texID, -1, -1, 0);
            LastBuffer.Blit(BuiltinRenderTextureType.CameraTarget, texID);
            // LastBuffer.ReleaseTemporaryRT( texID);
            LastBuffer.SetGlobalTexture("_LastFrame", texID);*/
            /*if (LastTexture == null)
            {   LastTexture =   new RenderTexture(this_camera.pixelWidth, this_camera.pixelHeight, 32)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    name = "EModules/MobileWater/LastFrame-RenderTexture" + gameObject.name,
                };
            }*/
            
            /* LastBuffer = new UnityEngine.Rendering.CommandBuffer() {name = "EMX Water Buffer" };
             texID = Shader.PropertyToID("_LastFrame");
             LastBuffer.GetTemporaryRT (texID, -1, -1, 0, FilterMode.Bilinear);
             LastBuffer.Blit (BuiltinRenderTextureType.CurrentActive, texID);*/
            
            
            int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            LastBuffer.GetTemporaryRT (screenCopyID, -1, -1, 0, FilterMode.Bilinear);
            LastBuffer.Blit (BuiltinRenderTextureType.GBuffer2, screenCopyID);
            LastBuffer.SetGlobalTexture("_LastFrame", screenCopyID);
            Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, LastBuffer);
        }
    }
    
    /* void OnDisable()
     {   if (!__this_camera) return;
         if (LastBuffer != null)
             this_camera.RemoveCommandBuffer(EventType, LastBuffer);
         LastBuffer = null;
         if (LastBuffer2 != null)
             this_camera.RemoveCommandBuffer(EventType, LastBuffer2);
         LastBuffer2 = null;
     }*/
    
    
    public static void DestroyAllScript()
    {   foreach (var c in CamDic)
        {   if (c.Key && c.Value)
            {   if (c.Value.LastBuffer != null )c.Key.RemoveCommandBuffer( EventType, c.Value.LastBuffer);
                if (c.Value.LastBuffer2 != null )c.Key.RemoveCommandBuffer( EventType, c.Value.LastBuffer2);
            }
            if ( c.Value  )
            {   c.Value.DestroyTex1();
                c.Value.DestroyTex2();
                if (!Application.isPlaying) DestroyImmediate(c.Value, true);
                else Destroy(c.Value);
            }
            
        }
        CamDic.Clear();
        foreach (var c in TexturesDic)
        {   if (c.Value  )
            {   if (!Application.isPlaying) DestroyImmediate(c.Value);
                else Destroy(c.Value);
            }
        }
        TexturesDic.Clear();
        _requesorDic.Clear();
    }
    void DestroyTex1()
    {   if (!LastTexture) return;
        if (!Application.isPlaying) DestroyImmediate(LastTexture, true);
        else Destroy(LastTexture);
    }
    void DestroyTex2()
    {   if (!LastTexture2) return;
        if (!Application.isPlaying) DestroyImmediate(LastTexture2, true);
        else Destroy(LastTexture2);
    }
    float lastSizex, lastSizey;
    /*void OnRenderImage(RenderTexture s, RenderTexture d)
    {   // LastTexture =  s;
        // s.ConvertToEquirect
        StackBlit(s);
        Graphics.Blit(s, d);
        //LastTexture2 =  d;
    }*/
    //static CameraEvent EventType {get {return TransparentFlag ? CameraEvent.BeforeImageEffects : CameraEvent.AfterForwardOpaque; } }
    void EnableLast2()
    {   LastBuffer2 = new UnityEngine.Rendering.CommandBuffer() {name = "EMX Water Buffer" };
        CheckLastTex();
        LastBuffer2.Blit(BuiltinRenderTextureType.CurrentActive, LastTexture2);
        this_camera.AddCommandBuffer(EventType, LastBuffer2); //, UnityEngine.Rendering.ComputeQueueType.Background*/
    }
    
    void CheckLastTex()
    {   if (LastTexture2 == null || lastSizex != this_camera.pixelWidth || lastSizey != this_camera.pixelHeight)
        {   if (LastTexture2) DestroyTex2();
            lastSizex = this_camera.pixelWidth ; lastSizey = this_camera.pixelHeight;
            // LastTexture2 =   new RenderTexture(this_camera.pixelWidth, this_camera.pixelHeight, SystemInfo.supports32bitsIndexBuffer ? 32 : 16)
            LastTexture2 =   new RenderTexture(this_camera.pixelWidth, this_camera.pixelHeight,  32)
            {   hideFlags = HideFlags.HideAndDontSave,
                    name = "EModules/MobileWater/LastFrame-RenderTexture" + gameObject.name,
            };
        }
    }
    void StackBlit(RenderTexture s)
    {   if (!this_camera) this_camera = GetComponent<Camera>();
        {   CheckLastTex();
            LastTexture2.DiscardContents();
            Graphics.Blit(s, LastTexture2);
        }
    }
    
    /*void OnPostRender()
     {   if (!__this_camera) this_camera = GetComponent<Camera>();
         StackBlit(this_camera.activeTexture);
     }*/
    /*void OnRenderImage(RenderTexture s, RenderTexture d)
    {   if (!TexturesDic.ContainsKey(this_camera))
        {   TexturesDic.Add(this_camera,
    
                            new RenderTexture(s.width, s.height, 32)
            {   isPowerOfTwo = true,
                    hideFlags = HideFlags.HideAndDontSave,
                    name = "EModules/MobileWater/LastFrame-RenderTexture" + gameObject.name,
                    // useMipMap = type == BakeOrUpdateType.Refraction || type == BakeOrUpdateType.ZDepth,
                    useMipMap = false,
                    autoGenerateMips = false,
                    wrapMode = TextureWrapMode.Clamp,
                    format = RenderTextureFormat.ARGB32
            }
    
                           );
        }
        Graphics.Blit(s, d);
        Graphics.CopyTexture(s, TexturesDic[this_camera]);
        LastTexture = TexturesDic[this_camera];
    }*/
    // RenderTexture releaseTex;
    private   void ConfigureCommandBuffer(CommandBuffer commandBuffer)
    {   if (commandBuffer == null) return;
    
        if (LastTexture == null)
        {   LastTexture =   new RenderTexture(this_camera.pixelWidth, this_camera.pixelHeight, 16)
            {   // isPowerOfTwo = true,
                hideFlags = HideFlags.HideAndDontSave,
                name = "EModules/MobileWater/LastFrame-RenderTexture" + gameObject.name,
                // useMipMap = type == BakeOrUpdateType.Refraction || type == BakeOrUpdateType.ZDepth,
                /* useMipMap = false,
                 autoGenerateMips = false,
                 wrapMode = TextureWrapMode.Clamp,
                 format = RenderTextureFormat.ARGB32*/
            };
        }
        int _LastFrame = Shader.PropertyToID("_LastFrame");
        commandBuffer.GetTemporaryRT(_LastFrame, this_camera.pixelWidth, this_camera.pixelHeight, 0, FilterMode.Trilinear, RenderTextureFormat.ARGB32);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, _LastFrame);
        commandBuffer.SetGlobalTexture("_LastFrame", _LastFrame);
        /*/var tid = Shader.PropertyToID("_LastFrame");
        /* commandBuffer.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Point);
         commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tid);
         commandBuffer.ReleaseTemporaryRT(tid);*/
        
        
        /*  commandBuffer.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Point);
          commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tid);
          commandBuffer.SetRenderTarget(m_frame_buffer);
          //commandBuffer.DrawMesh(m_quad, Matrix4x4.identity, m_mat_copy, 0, 0);
          commandBuffer.ReleaseTemporaryRT(tid);*/
        
        
        // var screenTexPropId = Shader.PropertyToID("_MainTex2");
        //var screenRtId = new RenderTargetIdentifier(Shader.PropertyToID("_MainTex2"));
        /*commandBuffer.Clear();
        
        //commandBuffer.GetTemporaryRT(screenTexPropId, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        commandBuffer.SetRenderTarget(LastTexture);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, LastTexture);*/
        // commandBuffer.SetRenderTarget(releaseTex);
        // commandBuffer.ReleaseTemporaryRT(screenTexPropId);
    }
    
    /*private void ConfigureCommandBuffer(CommandBuffer commandBuffer)
    {
        if (commandBuffer == null) return;
        commandBuffer.Clear();
    
        var screenTexPropId = Shader.PropertyToID("_MainTex2");
        var screenRtId = new RenderTargetIdentifier(Shader.PropertyToID("_MainTex2"));
    
        commandBuffer.GetTemporaryRT(screenTexPropId, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
    
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenRtId);
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive);
    
        commandBuffer.ReleaseTemporaryRT(screenTexPropId);
    }*/
}
}
