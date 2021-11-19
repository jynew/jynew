using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "color")]
	public class ES3Type_ColorOverLifetimeModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_ColorOverLifetimeModule() : base(typeof(UnityEngine.ParticleSystem.ColorOverLifetimeModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.ColorOverLifetimeModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("color", instance.color, ES3Type_MinMaxGradient.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.ColorOverLifetimeModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.ColorOverLifetimeModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.ParticleSystem.MinMaxGradient>(ES3Type_MinMaxGradient.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
