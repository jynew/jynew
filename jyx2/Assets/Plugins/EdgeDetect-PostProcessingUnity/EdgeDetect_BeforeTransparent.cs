//--------------------------------------------------------------------------------------------------------------------------------
// Port of the Legacy Unity "Edge Detect" image effect to Post Processing Stack v2
// Jean Moreno, 2017-2018
// Legacy Image Effect: https://docs.unity3d.com/550/Documentation/Manual/script-EdgeDetectEffectNormals.html
// Post Processing Stack v2: https://github.com/Unity-Technologies/PostProcessing/tree/v2
//--------------------------------------------------------------------------------------------------------------------------------

using UnityEngine.Rendering.PostProcessing;

//--------------------------------------------------------------------------------------------------------------------------------

[System.Serializable]
[PostProcess(typeof(EdgeDetectPostProcessingRenderer_BeforeTransparent), PostProcessEvent.BeforeTransparent, "Unity Legacy/Edge Detection (Before Transparent)")]
public sealed class EdgeDetect_BeforeTransparent : EdgeDetectPostProcessing { }

//--------------------------------------------------------------------------------------------------------------------------------

public sealed class EdgeDetectPostProcessingRenderer_BeforeTransparent : EdgeDetectPostProcessingRenderer<EdgeDetect_BeforeTransparent> { }
