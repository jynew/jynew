using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3Properties("x", "y", "z")]
	public class ES3Type_Vector3 : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Vector3() : base(typeof(Vector3))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Vector3 casted = (Vector3)obj;
			writer.WriteProperty("x", casted.x, ES3Type_float.Instance);
			writer.WriteProperty("y", casted.y, ES3Type_float.Instance);
			writer.WriteProperty("z", casted.z, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Vector3(	reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_Vector3Array : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Vector3Array() : base(typeof(Vector3[]), ES3Type_Vector3.Instance)
		{
			Instance = this;
		}
	}
}