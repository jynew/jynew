////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Weather/Rain_Pro")]
public class CameraFilterPack_Atmosphere_Rain_Pro : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0f, 1f)]
public float Fade = 1f;
[Range(0f, 2f)]
public float Intensity = 0.5f;
[Range(-0.25f, 0.25f)]
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
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Rain_Pro");
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
material.SetFloat("_Value3", DirectionX);
material.SetFloat("_Value4", Speed);
material.SetFloat("_Value5", Size);
material.SetFloat("_Value6", Distortion);
material.SetFloat("_Value7", StormFlashOnOff);
material.SetFloat("_Value8", DropOnOff);

material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
material.SetTexture("Texture2", Texture2);

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
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Rain_Pro");
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
