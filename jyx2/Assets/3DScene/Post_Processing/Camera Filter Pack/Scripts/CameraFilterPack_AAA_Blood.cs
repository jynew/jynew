////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/AAA/Blood")]
public class CameraFilterPack_AAA_Blood : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 128)]
public float Blood1 = 0f;
[Range(0, 128)]
public float Blood2 = 0f;
[Range(0, 128)]
public float Blood3 = 0f;
[Range(0, 128)]
public float Blood4 = 1f;
[Range(0, 0.004f)]
public float LightReflect = 0.002f;

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
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood1") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/AAA_Blood");
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
material.SetFloat("_Value", LightReflect);
material.SetFloat("_Value2", Blood1);
material.SetFloat("_Value3", Blood2);
material.SetFloat("_Value4", Blood3);
material.SetFloat("_Value5", Blood4);
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
SCShader = Shader.Find("CameraFilterPack/AAA_Blood");
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood1") as Texture2D;

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