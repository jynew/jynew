#define PRO

using UnityEngine;
using System.Collections;

namespace ProGrids
{
	public enum Axis {
		None = 0x0,
		X = 0x1,
		Y = 0x2,
		Z = 0x4,
		NegX = 0x8,
		NegY = 0x16,
		NegZ = 0x32
	}

	public enum SnapUnit {
		Meter,
		#if PRO
		Centimeter,
		Millimeter,
		Inch,
		Foot,
		Yard,
		Parsec
		#endif
	}

	public static class pg_Enum
	{
		/**
		 * Multiplies a Vector3 using the inverse value of an axis (eg, Axis.Y becomes Vector3(1, 0, 1) )
		 */
		public static Vector3 InverseAxisMask(Vector3 v, Axis axis)
		{
			switch(axis)
			{
				case Axis.X:
				case Axis.NegX:
					return Vector3.Scale(v, new Vector3(0f, 1f, 1f));
				
				case Axis.Y:
				case Axis.NegY:
					return Vector3.Scale(v, new Vector3(1f, 0f, 1f));

				case Axis.Z:
				case Axis.NegZ:
					return Vector3.Scale(v, new Vector3(1f, 1f, 0f));

				default:
					return v;
			}
		}

		public static Vector3 AxisMask(Vector3 v, Axis axis)
		{
			switch(axis)
			{
				case Axis.X:
				case Axis.NegX:
					return Vector3.Scale(v, new Vector3(1f, 0f, 0f));
				
				case Axis.Y:
				case Axis.NegY:
					return Vector3.Scale(v, new Vector3(0f, 1f, 0f));

				case Axis.Z:
				case Axis.NegZ:
					return Vector3.Scale(v, new Vector3(0f, 0f, 1f));

				default:
					return v;
			}
		}

		public static float SnapUnitValue(SnapUnit su)
		{
			switch(su)
			{
				case SnapUnit.Meter:
					return pg_Constant.METER;
#if PRO
				case SnapUnit.Centimeter:
					return pg_Constant.CENTIMETER;
				case SnapUnit.Millimeter:
					return pg_Constant.MILLIMETER;
				case SnapUnit.Inch:
					return pg_Constant.INCH;
				case SnapUnit.Foot:
					return pg_Constant.FOOT;
				case SnapUnit.Yard:
					return pg_Constant.YARD;
				case SnapUnit.Parsec:
					return pg_Constant.PARSEC;
#endif
				default:
					return pg_Constant.METER;
			}
		}
	}
}