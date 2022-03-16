using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("colorKeys", "alphaKeys", "mode")]
	public class ES3Type_Gradient : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Gradient() : base(typeof(UnityEngine.Gradient))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Gradient)obj;
			writer.WriteProperty("colorKeys", instance.colorKeys, ES3Type_GradientColorKeyArray.Instance);
			writer.WriteProperty("alphaKeys", instance.alphaKeys, ES3Type_GradientAlphaKeyArray.Instance);
			writer.WriteProperty("mode", instance.mode);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.Gradient();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Gradient)obj;
			instance.SetKeys(
				reader.ReadProperty<UnityEngine.GradientColorKey[]>(ES3Type_GradientColorKeyArray.Instance),
				reader.ReadProperty<UnityEngine.GradientAlphaKey[]>(ES3Type_GradientAlphaKeyArray.Instance)
			);

			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					case "mode":
						instance.mode = reader.Read<UnityEngine.GradientMode>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
