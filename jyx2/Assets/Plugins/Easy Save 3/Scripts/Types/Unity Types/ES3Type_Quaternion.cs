using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "z", "w")]
	public class ES3Type_Quaternion : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Quaternion() : base(typeof(Quaternion))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var casted = (Quaternion)obj;
			writer.WriteProperty("x", casted.x, ES3Type_float.Instance);
			writer.WriteProperty("y", casted.y, ES3Type_float.Instance);
			writer.WriteProperty("z", casted.z, ES3Type_float.Instance);
			writer.WriteProperty("w", casted.w, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Quaternion(	reader.ReadProperty<float>(ES3Type_float.Instance),
									reader.ReadProperty<float>(ES3Type_float.Instance),
									reader.ReadProperty<float>(ES3Type_float.Instance),
									reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_QuaternionArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_QuaternionArray() : base(typeof(Quaternion[]), ES3Type_Quaternion.Instance)
		{
			Instance = this;
		}
	}
}
