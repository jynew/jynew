using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("r", "g", "b", "a")]
	public class ES3Type_Color : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Color() : base(typeof(Color))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Color casted = (Color)obj;
			writer.WriteProperty("r", casted.r, ES3Type_float.Instance);
			writer.WriteProperty("g", casted.g, ES3Type_float.Instance);
			writer.WriteProperty("b", casted.b, ES3Type_float.Instance);
			writer.WriteProperty("a", casted.a, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Color(	reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance),
								reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

	public class ES3Type_ColorArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ColorArray() : base(typeof(Color[]), ES3Type_Color.Instance)
		{
			Instance = this;
		}
	}
}