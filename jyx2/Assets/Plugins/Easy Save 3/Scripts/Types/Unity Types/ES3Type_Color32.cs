using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("r", "g", "b", "a")]
	public class ES3Type_Color32 : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Color32() : base(typeof(Color32))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Color32 casted = (Color32)obj;
			writer.WriteProperty("r", casted.r, ES3Type_byte.Instance);
			writer.WriteProperty("g", casted.g, ES3Type_byte.Instance);
			writer.WriteProperty("b", casted.b, ES3Type_byte.Instance);
			writer.WriteProperty("a", casted.a, ES3Type_byte.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Color32(	reader.ReadProperty<byte>(ES3Type_byte.Instance),
								reader.ReadProperty<byte>(ES3Type_byte.Instance),
								reader.ReadProperty<byte>(ES3Type_byte.Instance),
								reader.ReadProperty<byte>(ES3Type_byte.Instance));
		}

		public static bool Equals(Color32 a, Color32 b)
		{
			if(a.r != b.r || a.g != b.g || a.b != b.b || a.a != b.a)
				return false;
			return true;
		}
	}

	public class ES3Type_Color32Array : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Color32Array() : base(typeof(Color32[]), ES3Type_Color32.Instance)
		{
			Instance = this;
		}
	}
}