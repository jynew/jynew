////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Weather/Rain_Pro_3D")]
public class CameraFilterPack_Atmosphere_Rain_Pro_3D : MonoBehaviour {
#region Variables
public Shader SCShader;
public bool _Visualize=false;
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 3f;  
[Range(0f, 1f)]
public float Fade = 1f;
[Range(0f, 2f)]
public float Intensity = 0.5f;
public bool DirectionFollowCameraZ = false;
[Range(-0.45f, 0.45f)]
public float DirectionX = 0.120f;
[Range(0.4f, 2f)]
public float Size = 1.5f;
[Range(0f, 0.5f)]
public float Speed = 0.275f;
[Range(0f, 0.5f)]
public float Distortion = 0.025f;
[Range(0f, 1f)]
public float StormFlashOnOff = 1f;
[Range(0f, 1f)]
public float DropOnOff = 1;
[Range(-0.5f, 0.99f)]
public float Drop_Near = 0;
[Range(0f, 1f)]
public float Drop_Far = 0.5f;
[Range(0f, 1f)]
public float Drop_With_Obj = 0.2f;
[Range(0f, 1f)]
public float Myst = 0.1f;
[Range(0f, 1f)]
public float Drop_Floor_Fluid = 0.0f;


public Color Myst_Color = new Color(0.5f,0.5f,0.5f,1);
private Texture2D Texture2;


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

Texture2 = Resources.Load ("CameraFilterPack_Atmosphere_Rain_FX") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Rain_Pro_3D");
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
material.SetFloat("_Value", Fade);
material.SetFloat("_Value2", Intensity);
if (DirectionFollowCameraZ)
{
float z = GetComponent<Camera>().transform.rotation.z;


if ((z>0) && (z<360))
{ 
material.SetFloat("_Value3", z);
}

if (z<0)
{ 
material.SetFloat("_Value3", z);
}
}
else
{
material.SetFloat("_Value3", DirectionX);
}
material.SetFloat("_Value4", Speed);
material.SetFloat("_Value5", Size);
material.SetFloat("_Value6", Distortion);
material.SetFloat("_Value7", StormFlashOnOff);
material.SetFloat("_Value8", DropOnOff);
material.SetFloat("_FixDistance",_FixDistance);
material.SetFloat("_Visualize", _Visualize ? 1 : 0);
material.SetFloat("Drop_Near", Drop_Near);
material.SetFloat("Drop_Far", Drop_Far);
material.SetFloat("Drop_With_Obj", 1-Drop_With_Obj);
material.SetFloat("Myst", Myst);
material.SetColor("Myst_Color",Myst_Color);
material.SetFloat("Drop_Floor_Fluid", Drop_Floor_Fluid);


material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
material.SetTexture("Texture2", Texture2);
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
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Rain_Pro_3D");
Texture2 = Resources.Load ("CameraFilterPack_Atmosphere_Rain_FX") as Texture2D;

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
