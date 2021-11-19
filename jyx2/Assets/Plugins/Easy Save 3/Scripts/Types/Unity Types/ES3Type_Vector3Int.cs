#if UNITY_2017_2_OR_NEWER
using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "z")]
	public class ES3Type_Vector3Int : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Vector3Int() : base(typeof(Vector3Int))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Vector3Int casted = (Vector3Int)obj;
			writer.WriteProperty("x", casted.x, ES3Type_int.Instance);
			writer.WriteProperty("y", casted.y, ES3Type_int.Instance);
			writer.WriteProperty("z", casted.z, ES3Type_int.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Vector3Int(	reader.ReadProperty<int>(ES3Type_int.Instance),
									reader.ReadProperty<int>(ES3Type_int.Instance),
									reader.ReadProperty<int>(ES3Type_int.Instance));
		}
	}

		public class ES3Type_Vector3IntArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Vector3IntArray() : base(typeof(Vector3Int[]), ES3Type_Vector3Int.Instance)
		{
			Instance = this;
		}
	}
}
#endif