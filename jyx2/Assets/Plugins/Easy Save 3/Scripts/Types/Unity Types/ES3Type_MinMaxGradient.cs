using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("mode", "gradientMax", "gradientMin", "colorMax", "colorMin", "color", "gradient")]
	public class ES3Type_MinMaxGradient : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_MinMaxGradient() : base(typeof(UnityEngine.ParticleSystem.MinMaxGradient))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.MinMaxGradient)obj;
			
			writer.WriteProperty("mode", instance.mode);
			writer.WriteProperty("gradientMax", instance.gradientMax, ES3Type_Gradient.Instance);
			writer.WriteProperty("gradientMin", instance.gradientMin, ES3Type_Gradient.Instance);
			writer.WriteProperty("colorMax", instance.colorMax, ES3Type_Color.Instance);
			writer.WriteProperty("colorMin", instance.colorMin, ES3Type_Color.Instance);
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("gradient", instance.gradient, ES3Type_Gradient.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.MinMaxGradient();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{

					case "mode":
						instance.mode = reader.Read<UnityEngine.ParticleSystemGradientMode>();
						break;
					case "gradientMax":
						instance.gradientMax = reader.Read<UnityEngine.Gradient>(ES3Type_Gradient.Instance);
						break;
					case "gradientMin":
						instance.gradientMin = reader.Read<UnityEngine.Gradient>(ES3Type_Gradient.Instance);
						break;
					case "colorMax":
						instance.colorMax = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "colorMin":
						instance.colorMin = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "gradient":
						instance.gradient = reader.Read<UnityEngine.Gradient>(ES3Type_Gradient.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}
}
