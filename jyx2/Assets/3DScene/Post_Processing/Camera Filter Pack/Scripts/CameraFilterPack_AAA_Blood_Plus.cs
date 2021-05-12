////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/AAA/Blood_Plus")]
public class CameraFilterPack_AAA_Blood_Plus : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 1)]
public float Blood_1 = 1f;
[Range(0, 1)]
public float Blood_2 = 0f;
[Range(0, 1)]
public float Blood_3 = 0f;
[Range(0, 1)]
public float Blood_4 = 0f;
[Range(0, 1)]
public float Blood_5 = 0f;
[Range(0, 1)]
public float Blood_6 = 0f;
[Range(0, 1)]
public float Blood_7 = 0f;
[Range(0, 1)]
public float Blood_8 = 0f;
[Range(0, 1)]
public float Blood_9 = 0f;
[Range(0, 1)]
public float Blood_10 = 0f;
[Range(0, 1)]
public float Blood_11 = 0f;
[Range(0, 1)]
public float Blood_12 = 0f;

[Range(0, 1f)]
public float LightReflect = 0.5f;

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
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood2") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/AAA_Blood_Plus");
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
material.SetFloat("_Value2", Mathf.Clamp(Blood_1,0,1));
material.SetFloat("_Value3", Mathf.Clamp(Blood_2,0,1));
material.SetFloat("_Value4", Mathf.Clamp(Blood_3,0,1));
material.SetFloat("_Value5", Mathf.Clamp(Blood_4,0,1));
material.SetFloat("_Value6", Mathf.Clamp(Blood_5,0,1));
material.SetFloat("_Value7", Mathf.Clamp(Blood_6,0,1));
material.SetFloat("_Value8", Mathf.Clamp(Blood_7,0,1));
material.SetFloat("_Value9", Mathf.Clamp(Blood_8,0,1));
material.SetFloat("_Value10", Mathf.Clamp(Blood_9,0,1));
material.SetFloat("_Value11", Mathf.Clamp(Blood_10,0,1));
material.SetFloat("_Value12", Mathf.Clamp(Blood_11,0,1));
material.SetFloat("_Value13",Mathf.Clamp( Blood_12,0,1));

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
SCShader = Shader.Find("CameraFilterPack/AAA_Blood_Plus");
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood2") as Texture2D;

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