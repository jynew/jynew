using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("alpha", "time")]
	public class ES3Type_GradientAlphaKey : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_GradientAlphaKey() : base(typeof(UnityEngine.GradientAlphaKey))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.GradientAlphaKey)obj;
			
			writer.WriteProperty("alpha", instance.alpha, ES3Type_float.Instance);
			writer.WriteProperty("time", instance.time, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new UnityEngine.GradientAlphaKey(reader.ReadProperty<float>(ES3Type_float.Instance),
													reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_GradientAlphaKeyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_GradientAlphaKeyArray() : base(typeof(GradientAlphaKey[]), ES3Type_GradientAlphaKey.Instance)
		{
			Instance = this;
		}
	}
}