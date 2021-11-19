using System;
using UnityEngine;
using System.Collections.Generic;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("center", "size")]
	public class ES3Type_Bounds : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Bounds() : base(typeof(Bounds))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Bounds casted = (Bounds)obj;

			writer.WriteProperty("center", casted.center, ES3Type_Vector3.Instance);
			writer.WriteProperty("size", casted.size, ES3Type_Vector3.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Bounds(	reader.ReadProperty<Vector3>(ES3Type_Vector3.Instance), 
								reader.ReadProperty<Vector3>(ES3Type_Vector3.Instance));
		}
	}

		public class ES3Type_BoundsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_BoundsArray() : base(typeof(Bounds[]), ES3Type_Bounds.Instance)
		{
			Instance = this;
		}
	}
}