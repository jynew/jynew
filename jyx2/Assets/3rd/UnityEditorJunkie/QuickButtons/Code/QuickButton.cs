// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   03/12/2019
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using UnityEngine;

namespace RoboRyanTron.QuickButtons
{
    /// <summary>
    /// QuickButtons can be placed in any MonoBehaviour, ScriptableObject,
    /// StateMachineBehaviour or Serializable type to draw a button in the
    /// inspector with a custom callback. 
    /// </summary>
    [Serializable]
    public class QuickButton
    {
        #region -- Constants ---------------------------------------------------
        /// <summary>
        /// Broad set of binding flags used to reflect methods.
        /// </summary>
        private const BindingFlags FLAGS = BindingFlags.IgnoreCase |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | 
            BindingFlags.FlattenHierarchy;
        #endregion -- Constants ------------------------------------------------
        
        #region -- Private Variables -------------------------------------------
        /// <summary>
        /// Name of the function to invoke when this button is clicked.
        /// </summary>
        private readonly string functionName;
        
        /// <summary>
        /// Arguments to pass to the function named <see cref="functionName"/>
        /// when it is invoked.
        /// </summary>
        private readonly object[] functionArgs;
        
        /// <summary>
        /// Action to invoke when the button is pressed if this was constructed
        /// with the Action constructor. The object passed will be the object
        /// that this QuickButton is contained in. 
        /// </summary>
        private readonly Action<object> action;
        
        /// <summary>
        /// The call used to respond to a button press. This will be different
        /// based on what constructor was used.
        /// </summary>
        private readonly Action<object> invocation;
        #endregion -- Private Variables ----------------------------------------
        
        #region -- Constructors ------------------------------------------------
        /// <summary>
        /// Makes a button that will draw in the inspector and invoke the named
        /// function when clicked.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function to call when clicked.
        /// </param>
        /// <param name="functionArgs">List of arguments to pass.</param>
        public QuickButton(string functionName, params object[] functionArgs)
        {
            this.functionName = functionName;
            this.functionArgs = functionArgs;
            
            invocation = NameInvoke;
        }

        /// <summary>
        /// Makes a button that will draw in the inspector and invoke the given
        /// action when clicked.
        /// </summary>
        /// <param name="action">
        /// Action to call when the button is clicked. The object that
        /// the button is defined on will be passed in as the argument.
        /// </param>
        public QuickButton(Action<object> action)
        {
            this.action = action;
            
            invocation = ActionInvoke;
        }
        #endregion -- Constructors ---------------------------------------------
        
        #region -- Invocation --------------------------------------------------
        /// <summary> Invoke the button callback. </summary>
        /// <param name="target">Object that the button is defined on.</param>
        public void Invoke(object target)
        {
            invocation.Invoke(target);
        }
        
        private void ActionInvoke(object target)
        {
            action.Invoke(target);
        }
        
        private void NameInvoke(object target)
        {
            Type t = target.GetType();
            MethodInfo method = null;
            while (method == null && t != null)
            {
                method = t.GetMethod(functionName, FLAGS);
                t = t.BaseType;
            }
            if (method != null)
            {
                // TODO: error handling for argument length and types. This could handle a target invocation exception.
                method.Invoke(target, functionArgs);
            }
            else
            {
                Debug.LogError($"Unable to resolve method {functionName} " +
                    $"from type {target.GetType().Name}");
            }
        }
        #endregion -- Invocation -----------------------------------------------
    }
}