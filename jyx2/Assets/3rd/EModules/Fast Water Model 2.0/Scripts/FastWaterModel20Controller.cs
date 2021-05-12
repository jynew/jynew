using UnityEngine;
using System.Linq;


namespace EModules.FastWaterModel20 {
// use these functions to runtimme interaction
public partial class FastWaterModel20Controller : MonoBehaviour {


    // baking textures via script
    public void BakeOrUpdateTexture()
    {   RenderOnAwake(true);
    }
    
    
    // current object animations speed
    public void SetSpeed(float newSpeed)
    {   OBJECT_SPEED = newSpeed;
    }
    
    
    // experimental features that try use partitional optimization (you may change it in inspector if water object selected, or here runtime)
    public void SetMaterialQuality(MaterialQuality QualityAmount)
    {   if (compiler.BufferResolution == (int)QualityAmount) return;
        compiler.BufferResolution = (int)QualityAmount;
    }
    public enum MaterialQuality { FullQuality = 0, HalfQUlaity = 1, QuarterQuality = 2, EighthQuality = 3}
    
    
    // replacing current material and rebaking if needs
    public void ReplaceMaterial(FastWaterModel20Compiler mat)
    {   if (mat == null || mat.material == null) throw new System.Exception("EModules.FastWaterModel20.FastWaterModel20Controller null material exeption");
    
        compiler = mat;
        Reset();
        MatWasAssigned = false;
        CloneMaterial();
        
        OnEnable(); // Start();
    }
    
    
    // removes all temporary textures, even baked and using, note that
    //--
    //FastWaterModel20 has automatic removing of unused textures,
    //but if you notice that some textures are not destroying when different scenes loading, you can manually invoke this function if you notice any problems with cleaning the memory
    public void DestroyAllTempTexturesDeep()
    {   Resources.FindObjectsOfTypeAll<Texture>().Where(n => ( n.hideFlags & HideFlags.HideAndDontSave) != 0 && n.name.StartsWith("EModules/MobileWater")).ToList().ForEach(t => DestroyImmediate(t));
    }
}
}