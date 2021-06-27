// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   03/12/2019
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RoboRyanTron.QuickButtons.Demo
{
    public class QuickButtonsDemo : MonoBehaviour
    {
        /// <summary>
        /// Having a reference to a QuickButtonsDemo makes it easier to use the
        /// nameof operator.
        /// </summary>
        private const QuickButtonsDemo TEMPLATE = null;
        
        // Wrap QuickButtons with a Unity Editor check since they have no use at runtime.
#if UNITY_EDITOR  
        /// <summary>
        /// You can pass the name of the method to call when the button is clicked.
        /// </summary>
        public QuickButton NameButton = new QuickButton("OnDebugButtonClicked");
        
        /// <summary>
        /// A safer way to pass strings around is with the nameof operator.
        /// This way the compiler can make sure the name is right.
        /// </summary>
        public QuickButton NameofButton = new QuickButton(nameof(TEMPLATE.OnDebugButtonClicked));
        
        /// <summary>
        /// You can also pass arguments to these methods.
        /// </summary>
        public QuickButton ArgumentButton = new QuickButton(
            nameof(TEMPLATE.OnParameterizedDebugButtonClicked), "Parameterized Button Clicked");
        
        /// <summary>
        /// You can also make a button that invokes a delegate to easily
        /// combine the button definition and the response. 
        /// </summary>
        public QuickButton DelegateButton = new QuickButton(input =>
        {
            QuickButtonsDemo demo = input as QuickButtonsDemo;
            Debug.Log("Delegate Button Clicked on " + demo.gameObject.name);
        });
#endif
        
        /// <summary>
        /// Buttons defined on nested Serializable types will also draw in the
        /// inspector. These buttons target the type they are defined on.
        /// </summary>
        public NestedClass nested;
        
        /// <summary>
        /// Arrays and lists are also supported.
        /// </summary>
        public List<NestedClass> nested2;
        
        private void OnDebugButtonClicked()
        {
            Debug.Log("Debug Button Clicked");
        }
        
        private void OnParameterizedDebugButtonClicked(string message)
        {
            Debug.Log(message);
        }
    }
}