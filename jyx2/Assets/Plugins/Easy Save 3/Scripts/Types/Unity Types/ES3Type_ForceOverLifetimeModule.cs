using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "x", "y", "z", "xMultiplier", "yMultiplier", "zMultiplier", "space", "randomized")]
	public class ES3Type_ForceOverLifetimeModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_ForceOverLifetimeModule() : base(typeof(UnityEngine.ParticleSystem.ForceOverLifetimeModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.ForceOverLifetimeModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("x", instance.x, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("y", instance.y, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("z", instance.z, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("xMultiplier", instance.xMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("yMultiplier", instance.yMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("zMultiplier", instance.zMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("space", instance.space);
			writer.WriteProperty("randomized", instance.randomized, ES3Type_bool.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.ForceOverLifetimeModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.ForceOverLifetimeModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "x":
						instance.x = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "y":
						instance.y = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "z":
						instance.z = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "xMultiplier":
						instance.xMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "yMultiplier":
						instance.yMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "zMultiplier":
						instance.zMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "space":
						instance.space = reader.Read<UnityEngine.ParticleSystemSimulationSpace>();
						break;
					case "randomized":
						instance.randomized = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
