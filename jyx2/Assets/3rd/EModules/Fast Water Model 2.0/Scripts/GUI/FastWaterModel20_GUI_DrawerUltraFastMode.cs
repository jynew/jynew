
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_TIZEN || UNITY_TVOS || UNITY_WP_8 || UNITY_WP_8_1
    #define MOBILE_TARGET
#endif

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace EModules.FastWaterModel20 {

partial class FastWaterModel20ControllerEditor : Editor {





}
}
#endif
