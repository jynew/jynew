// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum
{
    /// <summary>
    /// Put this attribute on a public (or SerialzeField) enum in a
    /// MonoBehaviour or ScriptableObject to get an improved enum selector
    /// popup. The enum list is scrollable and can be filtered by typing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SearchableEnumAttribute : PropertyAttribute {}
}
