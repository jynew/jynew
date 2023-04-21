using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    // 1) Add a MeshCombineStudio GameObject to the root of the scene and configure it
    //    -- make sure that Combine In Runtime is checked
    //    -- make sure that Combine On Start is unchecked
    //    -- make sure that On/Off With Tab is unchecked
    // 2) Attach this script to a GameObject in the root scene (the scene that loads and unloads other scenes)
    // 3) Drag the MeshCombineStudio GameObject into the MCS Game Object field in the RootSceneCombiner component
    public class RootSceneCombiner : MonoBehaviour
    {
        public MeshCombiner MCSGameObject;
    }
}