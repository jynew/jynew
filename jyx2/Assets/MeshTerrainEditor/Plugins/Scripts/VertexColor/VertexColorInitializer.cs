using UnityEngine;

//Please keep this attribute, it is used by MTE editor.
[ExecuteInEditMode]
public class VertexColorInitializer : MonoBehaviour
{
    public VertexColor vertexColor = null;

    void Start()
    {
        ApplyVertexColor();
    }

    /// <summary>
    /// Apply the vertex colors in the VertexColor asset file to the mesh
    /// </summary>
    /// <remarks>
    /// Please keep this method public, because MTE will call this method to apply vertex color in editor.
    /// </remarks>
    public void ApplyVertexColor()
    {
        var meshCollider = GetComponent<MeshCollider>();
        if(meshCollider.sharedMesh != null && vertexColor != null && vertexColor.colors != null &&
           vertexColor.colors.Length != 0)
        {
            if(meshCollider.sharedMesh.colors != vertexColor.colors)
            {
                meshCollider.sharedMesh.colors = vertexColor.colors;
            }
        }
    }
}
