using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("color", "time")]
	public class ES3Type_GradientColorKey : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_GradientColorKey() : base(typeof(UnityEngine.GradientColorKey))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.GradientColorKey)obj;
			
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("time", instance.time, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new UnityEngine.GradientColorKey(reader.ReadProperty<Color>(ES3Type_Color.Instance),
													reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_GradientColorKeyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_GradientColorKeyArray() : base(typeof(GradientColorKey[]), ES3Type_GradientColorKey.Instance)
		{
			Instance = this;
		}
	}
}