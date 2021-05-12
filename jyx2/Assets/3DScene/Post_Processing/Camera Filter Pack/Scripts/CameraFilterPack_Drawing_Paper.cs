////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Drawing/Paper")]
public class CameraFilterPack_Drawing_Paper : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
public Color Pencil_Color = new Color(0.156f,0.3f,0.738f,1.0f);
[Range(0.0001f, 0.0022f)]
public float Pencil_Size = 0.0008f;
[Range(0, 2)]
public float Pencil_Correction = 0.76f;
[Range(0, 1)]
public float Intensity = 1.0f;
[Range(0, 2)]
public float Speed_Animation = 1f;
[Range(0, 1)]
public float Corner_Lose = 0.5f;
[Range(0, 1)]
public float Fade_Paper_to_BackColor = 0f;
[Range(0, 1)]
public float Fade_With_Original = 1f;
public Color Back_Color = new Color(1.0f,1.0f,1.0f,1.0f);
private Material SCMaterial;
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
Texture2 = Resources.Load ("CameraFilterPack_Paper1") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/Drawing_Paper");
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
material.SetColor("_PColor", Pencil_Color);
material.SetFloat("_Value1", Pencil_Size);
material.SetFloat("_Value2", Pencil_Correction);
material.SetFloat("_Value3", Intensity);
material.SetFloat("_Value4", Speed_Animation);
material.SetFloat("_Value5", Corner_Lose);
material.SetFloat("_Value6", Fade_Paper_to_BackColor);
material.SetFloat("_Value7",Fade_With_Original);
material.SetColor("_PColor2", Back_Color);
material.SetTexture("_MainTex2", Texture2);
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
SCShader = Shader.Find("CameraFilterPack/Drawing_Paper");
Texture2 = Resources.Load ("CameraFilterPack_Paper1") as Texture2D;
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