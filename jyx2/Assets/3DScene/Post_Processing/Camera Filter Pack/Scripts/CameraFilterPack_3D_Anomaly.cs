///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2018 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Anomaly")]
public class CameraFilterPack_3D_Anomaly : MonoBehaviour {
#region Variables
public Shader SCShader;
public bool _Visualize=false;
private float TimeX = 1.0f;
 
private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 23f;  
[Range(-0.5f, 0.99f)]
public float Anomaly_Near = 0.045f;
[Range(0f, 1f)]
public float Anomaly_Far = 0.11f;

[Range(0f, 2f)]
public float Intensity = 1f;

[Range(0f, 1f)]
public float AnomalyWithoutObject = 1;


[Range(0.1f, 1f)]
public float Anomaly_Distortion = 0.25f;
[Range(4f, 64f)]
public float Anomaly_Distortion_Size = 12f;
[Range(-4f, 8f)]
public float Anomaly_Intensity = 2f;





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

SCShader = Shader.Find("CameraFilterPack/3D_Anomaly");
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
material.SetFloat("_Value2", Intensity);
material.SetFloat("Anomaly_Distortion", Anomaly_Distortion);
material.SetFloat("Anomaly_Distortion_Size", Anomaly_Distortion_Size);
material.SetFloat("Anomaly_Intensity", Anomaly_Intensity);
material.SetFloat("_Visualize", _Visualize ? 1 : 0);


material.SetFloat("_FixDistance",_FixDistance);

material.SetFloat("Anomaly_Near", Anomaly_Near);
material.SetFloat("Anomaly_Far", Anomaly_Far);
material.SetFloat("Anomaly_With_Obj", AnomalyWithoutObject);


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
			SCShader = Shader.Find("CameraFilterPack/3D_Anomaly");

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
