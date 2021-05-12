///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2018 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Distortion")]
public class CameraFilterPack_3D_Distortion : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
public bool _Visualize=false;

private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 1f;  
[Range(-0.99f, 0.99f)]
public float _Distance = 0.5f;  
[Range(0f, 0.5f)]
public float _Size = 0.1f;  
[Range(0f, 10f)]
public float DistortionLevel = 1.2f;  
[Range(0.1f, 10f)]
public float DistortionSize = 1.40f;  
[Range(-2f, 4f)]
public float LightIntensity = 0.08f;  
public bool AutoAnimatedNear=false;
[Range(-5f, 5f)]
public float AutoAnimatedNearSpeed=0.5f;


public static Color ChangeColorRGB;
#endregion

#region Properties
Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
}
return SCMaterial;
}
}
#endregion
void Start () 
{
SCShader = Shader.Find("CameraFilterPack/3D_Distortion");

if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}

void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_TimeX", TimeX);
if (AutoAnimatedNear)
{
_Distance+=Time.deltaTime*AutoAnimatedNearSpeed;
if (_Distance>1) _Distance=-1f;
if (_Distance<-1f) _Distance=1;
material.SetFloat("_Near",_Distance);
}
else
{
material.SetFloat("_Near",_Distance);
}
material.SetFloat("_Far",_Size);
material.SetFloat("_FixDistance",_FixDistance);
material.SetFloat("_DistortionLevel",DistortionLevel*28);
material.SetFloat("_DistortionSize",DistortionSize*16);
material.SetFloat("_LightIntensity",LightIntensity*64);
material.SetFloat("_Visualize", _Visualize ? 1 : 0);

float _FarCamera = GetComponent<Camera>().farClipPlane; 
material.SetFloat("_FarCamera",1000/_FarCamera);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

Graphics.Blit(sourceTexture, destTexture, material);

}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}


}

void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/3D_Distortion");

}
#endif

}

void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);	
}

}


}