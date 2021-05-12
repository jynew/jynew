////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Snow")]
public class CameraFilterPack_3D_Snow : MonoBehaviour {
#region Variables
public Shader SCShader;
public bool _Visualize=false;
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 5f;  
[Range(-0.5f, 0.99f)]
public float Snow_Near = 0.08f;
[Range(0f, 1f)]
public float Snow_Far = 0.55f;
[Range(0f, 1f)]
public float Fade = 1f;
[Range(0f, 2f)]
public float Intensity = 1f;

[Range(0.4f, 2f)]
public float Size = 1f;
[Range(0f, 0.5f)]
public float Speed = 0.275f;



[Range(0f, 1f)]
public float SnowWithoutObject = 1;

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

Texture2 = Resources.Load ("CameraFilterPack_Blizzard1") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/3D_Snow");
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

material.SetFloat("_Value4", Speed*6);
material.SetFloat("_Value5", Size);
material.SetFloat("_Visualize", _Visualize ? 1 : 0);

material.SetFloat("_FixDistance",_FixDistance);

material.SetFloat("Drop_Near", Snow_Near);
material.SetFloat("Drop_Far", Snow_Far);
material.SetFloat("Drop_With_Obj", SnowWithoutObject);


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
			SCShader = Shader.Find("CameraFilterPack/3D_Snow");
			Texture2 = Resources.Load ("CameraFilterPack_Blizzard1") as Texture2D;

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
