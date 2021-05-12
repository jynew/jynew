using UnityEngine;

namespace AmplifyShaderEditor
{
	public static class RectExtension
	{
		private static Rect ValidateBoundaries( this Rect thisRect )
		{
			if ( thisRect.yMin > thisRect.yMax )
			{
				float yMin = thisRect.yMin;
				thisRect.yMin = thisRect.yMax;
				thisRect.yMax = yMin;
			}

			if ( thisRect.xMin > thisRect.xMax )
			{
				float xMin = thisRect.xMin;
				thisRect.xMin = thisRect.xMax;
				thisRect.xMax = xMin;
			}
			return thisRect;
		}

		public static bool Includes( this Rect thisRect , Rect other )
		{
			thisRect = thisRect.ValidateBoundaries();
			other = other.ValidateBoundaries();

			if ( other.xMin >= thisRect.xMin && other.xMax <= thisRect.xMax )
			{
				if ( other.yMin >= thisRect.yMin && other.yMax <= thisRect.yMax )
				{
					return true;
				}
			}
			return false;
		}
	}
}
