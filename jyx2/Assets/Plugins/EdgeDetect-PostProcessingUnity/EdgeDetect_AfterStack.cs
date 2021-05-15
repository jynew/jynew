//--------------------------------------------------------------------------------------------------------------------------------
// Port of the Legacy Unity "Edge Detect" image effect to Post Processing Stack v2
// Jean Moreno, 2017-2018
// Legacy Image Effect: https://docs.unity3d.com/550/Documentation/Manual/script-EdgeDetectEffectNormals.html
// Post Processing Stack v2: https://github.com/Unity-Technologies/PostProcessing/tree/v2
//--------------------------------------------------------------------------------------------------------------------------------

using UnityEngine.Rendering.PostProcessing;

//--------------------------------------------------------------------------------------------------------------------------------

[System.Serializable]
[PostProcess(typeof(EdgeDetectPostProcessingRenderer_AfterStack), PostProcessEvent.AfterStack, "Unity Legacy/Edge Detection (After Stack)")]
public sealed class EdgeDetect_AfterStack : EdgeDetectPostProcessing { }

//--------------------------------------------------------------------------------------------------------------------------------

public sealed class EdgeDetectPostProcessingRenderer_AfterStack : EdgeDetectPostProcessingRenderer<EdgeDetect_AfterStack> { }
