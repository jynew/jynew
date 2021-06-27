// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using UnityEngine;

namespace RoboRyanTron.SearchableEnum
{
    /// <summary>
    /// A demo of the SearchableEnumPopup on a ScriptableObject.
    /// </summary>
    [CreateAssetMenu]
    public class SearchableEnumDemo : ScriptableObject
    {
        [Tooltip("This enum is fucking miserable.")]
        public KeyCode LameKeyCode;
        
        [Tooltip("The finest enum browsing experience one can have.")]
        [SearchableEnum]
        public KeyCode AwesomeKeyCode;
    }
}
