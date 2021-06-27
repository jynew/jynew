// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   03/13/2019
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace RoboRyanTron.QuickButtons.Demo
{
    [Serializable]
    public class NestedClass
    {
        [Serializable]
        public class DeeperNestedClass
        {
            public QuickButton DeeperButton = new QuickButton(input =>
                Debug.Log("so deep "));
        }
        
        public DeeperNestedClass DeeperNested;
        public string Message;
      
            
        public QuickButton NestedDelegateButton = new QuickButton(input =>
        {
            NestedClass nested = input as NestedClass;
            Debug.Log("Nested Button Pressed " + nested.Message);
        });
    }
}