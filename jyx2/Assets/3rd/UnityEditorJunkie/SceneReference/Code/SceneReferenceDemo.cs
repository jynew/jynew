// ----------------------------------------------------------------------------
// Author: Ryan Hipple
// Date:   05/07/2018
// ----------------------------------------------------------------------------

using UnityEngine;

namespace RoboRyanTron.SceneReference
{
    public class SceneReferenceDemo : MonoBehaviour
    {
        public SceneReference SceneReferenceA;
        public SceneReference SceneReferenceB;

        /// <summary>
        /// Called by UnityEvent from a Button.
        /// </summary>
        public void LoadSceneA()
        {
            SceneReferenceA.LoadScene();
        }
        
        /// <summary>
        /// Called by UnityEvent from a Button.
        /// </summary>
        public void LoadSceneB()
        {
            SceneReferenceB.LoadScene();
        }
    }
}