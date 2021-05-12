////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2018 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Lut/Lut 2 Lut")]
public class CameraFilterPack_Lut_2_Lut : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;
    private Vector4 ScreenResolution;
    private Material SCMaterial;
    public Texture2D LutTexture = null;
    public Texture2D LutTexture2 = null;
    private Texture3D converted3DLut = null;
    private Texture3D converted3DLut2 = null;
    [Range(0f, 1f)]
    public float Blend = 1f;
    [Range(0f, 1f)]
    public float Fade = 1f;

    private string MemoPath;
    private string MemoPath2;



    #endregion
    #region Properties
    Material material
    {
        get
        {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion
    void Start()
    {
        SCShader = Shader.Find("CameraFilterPack/Lut_2_lut");
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

    }
 

    public void SetIdentityLut()
    {
        int dim = 16;
        var newC = new Color[dim * dim * dim];
        float oneOverDim = 1.0f / (1.0f * dim - 1.0f);

        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    newC[i + (j * dim) + (k * dim * dim)] = new Color((i * 1.0f) * oneOverDim, (j * 1.0f) * oneOverDim, (k * 1.0f) * oneOverDim, 1.0f);
                }
            }
        }

        if (converted3DLut) DestroyImmediate(converted3DLut);
        converted3DLut = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
        converted3DLut.SetPixels(newC);
        converted3DLut.Apply();
        if (converted3DLut2) DestroyImmediate(converted3DLut2);
        converted3DLut2 = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
        converted3DLut2.SetPixels(newC);
        converted3DLut2.Apply();
        
    }

    public bool ValidDimensions(Texture2D tex2d)
    {
        if (!tex2d) return false;
        int h = tex2d.height;
        if (h != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width)))
        {
            return false;
        }
        return true;
    }

    public Texture3D Convert(Texture2D temp2DTex, Texture3D cv3D)
    {
        int dim = 256 * 16;
        if (temp2DTex)
        {

             dim = temp2DTex.width * temp2DTex.height;
            dim = temp2DTex.height;

            if (!ValidDimensions(temp2DTex))
            {
                Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");
                return cv3D;
            }
        }
#if UNITY_EDITOR
        if (Application.isPlaying != true)
            {
                string path = AssetDatabase.GetAssetPath(LutTexture);
                MemoPath = path;
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (!textureImporter.isReadable)
                {

                    textureImporter.isReadable = true;
                    textureImporter.mipmapEnabled = false;

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
              string path2 = AssetDatabase.GetAssetPath(LutTexture2);
                MemoPath2 = path2;
                TextureImporter textureImporter2 = AssetImporter.GetAtPath(path2) as TextureImporter;
                if (!textureImporter2.isReadable)
                {

                    textureImporter2.isReadable = true;
                    textureImporter2.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(path2, ImportAssetOptions.ForceUpdate);
                }
            }
#endif

        var c = temp2DTex.GetPixels();
            var newC = new Color[c.Length];

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        int j_ = dim - j - 1;
                        newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
                    }
                }
            }


            if (cv3D) DestroyImmediate(cv3D);
            cv3D = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
            cv3D.SetPixels(newC);
            cv3D.Apply();

        return cv3D;


    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if ((SCShader != null) || (!SystemInfo.supports3DTextures))
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100) TimeX = 0;

            if (converted3DLut == null)
            {
                if (!LutTexture) SetIdentityLut();
                if (LutTexture) converted3DLut = Convert(LutTexture, converted3DLut);
            }
            if (converted3DLut2 == null)
            {
                if (!LutTexture2) SetIdentityLut();
                if (LutTexture2) converted3DLut2 = Convert(LutTexture2, converted3DLut2);
            }

            if (LutTexture) converted3DLut.wrapMode = TextureWrapMode.Clamp;
            if (LutTexture2) converted3DLut2.wrapMode = TextureWrapMode.Clamp;
            material.SetFloat("_Blend", Blend);
            material.SetFloat("_Fade", Fade);
            material.SetTexture("_LutTex", converted3DLut);
            material.SetTexture("_LutTex2", converted3DLut2);
            Graphics.Blit(sourceTexture, destTexture, material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
    void OnValidate()
    {
#if UNITY_EDITOR

      string path = AssetDatabase.GetAssetPath(LutTexture);
        if (MemoPath != path) converted3DLut = Convert(LutTexture, converted3DLut);
        string path2 = AssetDatabase.GetAssetPath(LutTexture2);
        if (MemoPath2 != path2) converted3DLut2 = Convert(LutTexture2, converted3DLut2);
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("CameraFilterPack/Lut_2_lut");
        }
#endif
    }
    void OnDisable()
    {
        if (SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }
    }
}
