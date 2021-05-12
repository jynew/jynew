using UnityEngine;
using System;

namespace ProGrids
{
	/**
	 * ProGridsNoSnapAttribute tells ProGrids to skip snapping on this object.
	 *	Note - On Unity versions less than 5.2 this will not take effect until after a script
	 *	reload.
	 */
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ProGridsNoSnapAttribute : Attribute
	{
	}

	/**
	 * ProGridsConditionalSnapAttribute tells ProGrids to check `bool IsSnapEnabled()` function on this object.
	 */
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ProGridsConditionalSnapAttribute : Attribute
	{
	}
}
