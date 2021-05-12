////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Drawing/EnhancedComics")]
public class CameraFilterPack_Drawing_EnhancedComics : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Material SCMaterial;
[Range(0, 1)] public float DotSize = 0.15f;
[Range(0, 1)] public float _ColorR = 0.9f;
[Range(0, 1)] public float _ColorG = 0.4f;
[Range(0, 1)] public float _ColorB = 0.4f;
[Range(0, 1)] public float _Blood = 0.5f;
[Range(0, 1)] public float _SmoothStart = 0.02f;
[Range(0, 1)] public float _SmoothEnd = 0.1f;
public Color ColorRGB = new Color (1.0f,0.0f,0.0f);



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


SCShader = Shader.Find("CameraFilterPack/Drawing_EnhancedComics");

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
material.SetFloat("_DotSize", DotSize);
material.SetFloat("_ColorR", _ColorR);
material.SetFloat("_ColorG", _ColorG);
material.SetFloat("_ColorB", _ColorB);
material.SetFloat("_Blood", _Blood);

material.SetColor("_ColorRGB",ColorRGB);
material.SetFloat("_SmoothStart",_SmoothStart);
material.SetFloat("_SmoothEnd",_SmoothEnd);


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
SCShader = Shader.Find("CameraFilterPack/Drawing_EnhancedComics");

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