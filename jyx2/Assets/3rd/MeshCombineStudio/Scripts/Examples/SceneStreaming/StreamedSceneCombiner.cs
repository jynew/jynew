using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeshCombineStudio
{
    // This script needs to be attached to a GameObject in the loaded scene, which is the parent of the children to be combined.
    // This can be done using the RequireComponentAttribute - for example, with NatureManufacture's WorldStreamer,
    //   this can be added to the ObjectsInSceneParent scripts automatically as follows:
    //   [RequireComponent(typeof(StreamedSceneCombiner))]
    //   public class ObjectsInSceneParent : MonoBehaviour {
    //       // ... rest of code here
    //
    public class StreamedSceneCombiner : MonoBehaviour
    {
        void Start()
        {
            // The rootScene is the scene that loads and unloads other scenes (usually as the player moves)
            var rootScene = SceneManager.GetActiveScene();
            
            foreach (var gameObject in rootScene.GetRootGameObjects())
            {
                // find the RootSceneCombiner in the rootScene (there should be only one!)
                if (gameObject.TryGetComponent<RootSceneCombiner>(out var rootSceneCombiner) &&
                    rootSceneCombiner.MCSGameObject != null &&
                    rootSceneCombiner.MCSGameObject.TryGetComponent<MeshCombiner>(out var _))
                {
                    // clone the MCS GameObject from the rootScene and make it a child of this GameObject's parent
                    var mcsGameObject = Instantiate(rootSceneCombiner.MCSGameObject, this.transform.parent);
                    var meshCombiner = mcsGameObject.GetComponent<MeshCombiner>();

                    // set the parentGOs to this GameObject - this will cause the MeshCombiner to combine the children of this GameObject
                    meshCombiner.searchOptions.parentGOs = new GameObject[] { this.gameObject };

                    // finally, combine the children of this GameObject
                    meshCombiner.CombineAll();
                    break;
                }
            }
        }
    }
}