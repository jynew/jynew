using UnityEngine;

namespace EModules.FastWaterModel20 {
[ExecuteInEditMode]
public class FastWaterModel20FakeLight : MonoBehaviour {
    public Color color = Color.white;
    
    #if UNITY_EDITOR
    public bool DrawHandles = true;
    public float HandlesSizes = 20;
    
    public void OnEnable()
    {   UnityEditor.SceneView.onSceneGUIDelegate -= ongui;
        UnityEditor.SceneView.onSceneGUIDelegate += ongui;
    }
    private void OnDisable()
    {   UnityEditor.SceneView.onSceneGUIDelegate -= ongui;
    }
    void ongui(UnityEditor.SceneView sv)
    {   if (!DrawHandles) return;
        var target = this;
        UnityEditor.Handles.color = target.color;
        UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Disabled;
        UnityEditor.Handles.SphereHandleCap( 0, target.transform.position, target.transform.rotation, target.HandlesSizes, EventType.Repaint );
        UnityEditor.Handles.ArrowHandleCap( 0, target.transform.position, target.transform.rotation, target.HandlesSizes * 2.5f, EventType.Repaint );
        
        UnityEditor.Handles.ArrowHandleCap( 0, target.transform.position + target.HandlesSizes * target.transform.right, target.transform.rotation, target.HandlesSizes * 1.5f, EventType.Repaint );
        UnityEditor.Handles.ArrowHandleCap( 0, target.transform.position + target.HandlesSizes * (Quaternion.AngleAxis(120, target.transform.forward) * target.transform.right), target.transform.rotation, target.HandlesSizes * 1.5f, EventType.Repaint );
        UnityEditor.Handles.ArrowHandleCap( 0, target.transform.position + target.HandlesSizes * (Quaternion.AngleAxis(240, target.transform.forward) * target.transform.right), target.transform.rotation, target.HandlesSizes * 1.5f, EventType.Repaint );
        
        
    }
    #endif
}
}

