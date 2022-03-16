using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y")]
	public class ES3Type_Vector2 : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Vector2() : base(typeof(Vector2))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Vector2 casted = (Vector2)obj;
			writer.WriteProperty("x", casted.x, ES3Type_float.Instance);
			writer.WriteProperty("y", casted.y, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Vector2(	reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_Vector2Array : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Vector2Array() : base(typeof(Vector2[]), ES3Type_Vector2.Instance)
		{
			Instance = this;
		}
	}
}