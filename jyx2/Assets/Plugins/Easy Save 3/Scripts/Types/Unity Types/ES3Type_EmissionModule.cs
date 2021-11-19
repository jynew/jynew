using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "rateOverTime", "rateOverTimeMultiplier", "rateOverDistance", "rateOverDistanceMultiplier")]
	public class ES3Type_EmissionModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_EmissionModule() : base(typeof(UnityEngine.ParticleSystem.EmissionModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.EmissionModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("rateOverTime", instance.rateOverTime, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("rateOverTimeMultiplier", instance.rateOverTimeMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("rateOverDistance", instance.rateOverDistance, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("rateOverDistanceMultiplier", instance.rateOverDistanceMultiplier, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.EmissionModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.EmissionModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "rateOverTime":
						instance.rateOverTime = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "rateOverTimeMultiplier":
						instance.rateOverTimeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "rateOverDistance":
						instance.rateOverDistance = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "rateOverDistanceMultiplier":
						instance.rateOverDistanceMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
