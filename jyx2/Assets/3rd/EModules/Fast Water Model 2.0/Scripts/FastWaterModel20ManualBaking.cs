using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.Linq;
    using System.Collections.Generic;
#endif

namespace EModules.FastWaterModel20 {
class FastWaterModel20ManualBaking : MonoBehaviour {
    public void BakeZAndRefraction()
    {   GetComponent<FastWaterModel20Controller>().BakeOrUpdateTexture();
    }
}


#if UNITY_EDITOR
[CustomEditor( typeof( FastWaterModel20ManualBaking ) )]
class FastWaterModel20ManualBakingEditor : Editor {
    public override void OnInspectorGUI()
    {   EditorGUILayout.HelpBox( "If you enable the baking via script option then you can bake textures at a convenient time for you.\nNote that, if you're using 'Texture convertions' options baking will take much longer time",
                                 MessageType.None );
        if (GUILayout.Button( "Bake\nZDepth or Refraction or Both", GUILayout.Height( FastWaterModel20ControllerEditor.H * 3 ) )) ((FastWaterModel20ManualBaking)target).BakeZAndRefraction();
        DrawDefaultInspector();
    }
}
#endif

}
