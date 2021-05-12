#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

namespace ProGrids
{
	/**
	 *	Assign this script to a GameObject to tell ProGrids to ignore snapping on this object.
	 *	Child objects are still subject to snapping.
	 *
	 *	Note - On Unity versions less than 5.2 this will not take effect until after a script
	 *	reload.
	 */
	[ProGridsNoSnap]
	public class pg_IgnoreSnap : MonoBehaviour {}
}

#endif
