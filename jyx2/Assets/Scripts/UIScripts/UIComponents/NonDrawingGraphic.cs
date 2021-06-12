using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NonDrawingGraphic : Graphic
{
	protected NonDrawingGraphic ()
	{
		useLegacyMeshGeneration = false;
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		toFill.Clear();
	}
}
