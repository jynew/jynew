using System;
using UnityEngine;
using System.Collections.Generic;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "z", "w")]
	public class ES3Type_Vector4 : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Vector4() : base(typeof(Vector4))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Vector4 casted = (Vector4)obj;
			writer.WriteProperty("x", casted.x, ES3Type_float.Instance);
			writer.WriteProperty("y", casted.y, ES3Type_float.Instance);
			writer.WriteProperty("z", casted.z, ES3Type_float.Instance);
			writer.WriteProperty("w", casted.w, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Vector4(	reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance));
		}

		public static bool Equals(Vector4 a, Vector4 b)
		{
			return (Mathf.Approximately(a.x,b.x) && Mathf.Approximately(a.y,b.y) && Mathf.Approximately(a.z,b.z) && Mathf.Approximately(a.w,b.w));
		}
	}

		public class ES3Type_Vector4Array : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Vector4Array() : base(typeof(Vector4[]), ES3Type_Vector4.Instance)
		{
			Instance = this;
		}
	}
}