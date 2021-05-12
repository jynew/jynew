#define PRO

using UnityEngine;
using System.Collections;

namespace ProGrids
{
	public static class pg_Constant
	{
		public const string ProGridsIsEnabled = "pgProGridsIsEnabled";
		public const string ProGridsIsExtended = "pgProGridsIsExtended";
		public const string ProGridsUpgradeURL = "http://u3d.as/content/six-by-seven-studio/pro-grids/3ov";
		public const string SnapValue = "pgSnapValue";
		public const string SnapMultiplier = "pgSnapMultiplier";
		public const string SnapEnabled = "pgSnapEnabled";
		public const string UseAxisConstraints = "pgUseAxisConstraints";
		public const string LastOrthoToggledRotation = "pgLastOrthoToggledRotation";
		public const string BracketIncreaseValue = "pgBracketIncreaseValue";
		public const string GridUnit = "pg_GridUnit";
		public const string LockGrid = "pg_LockGrid";
		public const string LockedGridPivot = "pg_LockedGridPivot";
		public const string PGVersion = "pg_Version";
		public const string GridAxis = "pg_GridAxis";
		public const string PerspGrid = "pg_PerspGrid";
		public const string SnapScale = "pg_SnapOnScale";
		public const string PredictiveGrid = "pg_PredictiveGrid";
		public const string SnapAsGroup = "pg_SnapAsGroup";
		public const string MajorLineIncrement = "pg_MajorLineIncrement";
		public const string SyncUnitySnap = "pg_SyncUnitySnap";

		public const float METER = 1f;
		#if PRO
		public const float CENTIMETER = .01f;
		public const float MILLIMETER = .001f;
		public const float INCH = 0.0253999862840074f;
		public const float FOOT = 0.3048f;
		public const float YARD = 1.09361f;
		public const float PARSEC = 5f;
		#endif
	}
}
