////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/AAA/Blood_Hit")]
public class CameraFilterPack_AAA_Blood_Hit : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 1)]
public float Hit_Left = 1f;
[Range(0, 1)]
public float Hit_Up = 0f;
[Range(0, 1)]
public float Hit_Right = 0f;
[Range(0, 1)]
public float Hit_Down = 0f;
[Range(0, 1)]
public float Blood_Hit_Left = 0f;
[Range(0, 1)]
public float Blood_Hit_Up = 0f;
[Range(0, 1)]
public float Blood_Hit_Right = 0f;
[Range(0, 1)]
public float Blood_Hit_Down = 0f;
[Range(0, 1)]
public float Hit_Full = 0f;
[Range(0, 1)]
public float Blood_Hit_Full_1 = 0f;
[Range(0, 1)]
public float Blood_Hit_Full_2 = 0f;
[Range(0, 1)]
public float Blood_Hit_Full_3 = 0f;

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
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood_Hit1") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/AAA_Blood_Hit");
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
material.SetFloat("_Value2", Mathf.Clamp(Hit_Left,0,1));
material.SetFloat("_Value3", Mathf.Clamp(Hit_Up,0,1));
material.SetFloat("_Value4", Mathf.Clamp(Hit_Right,0,1));
material.SetFloat("_Value5", Mathf.Clamp(Hit_Down,0,1));
material.SetFloat("_Value6", Mathf.Clamp(Blood_Hit_Left,0,1));
material.SetFloat("_Value7", Mathf.Clamp(Blood_Hit_Up,0,1));
material.SetFloat("_Value8", Mathf.Clamp(Blood_Hit_Right,0,1));
material.SetFloat("_Value9", Mathf.Clamp(Blood_Hit_Down,0,1));
material.SetFloat("_Value10", Mathf.Clamp(Hit_Full,0,1));
material.SetFloat("_Value11", Mathf.Clamp(Blood_Hit_Full_1,0,1));
material.SetFloat("_Value12", Mathf.Clamp(Blood_Hit_Full_2,0,1));
material.SetFloat("_Value13",Mathf.Clamp(Blood_Hit_Full_3,0,1));

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
SCShader = Shader.Find("CameraFilterPack/AAA_Blood_Hit");
Texture2 = Resources.Load ("CameraFilterPack_AAA_Blood_Hit1") as Texture2D;

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