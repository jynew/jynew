using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "inside", "outside", "enter", "exit", "radiusScale")]
	public class ES3Type_TriggerModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_TriggerModule() : base(typeof(UnityEngine.ParticleSystem.TriggerModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.TriggerModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("inside", instance.inside);
			writer.WriteProperty("outside", instance.outside);
			writer.WriteProperty("enter", instance.enter);
			writer.WriteProperty("exit", instance.exit);
			writer.WriteProperty("radiusScale", instance.radiusScale, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.TriggerModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.TriggerModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "inside":
						instance.inside = reader.Read<UnityEngine.ParticleSystemOverlapAction>();
						break;
					case "outside":
						instance.outside = reader.Read<UnityEngine.ParticleSystemOverlapAction>();
						break;
					case "enter":
						instance.enter = reader.Read<UnityEngine.ParticleSystemOverlapAction>();
						break;
					case "exit":
						instance.exit = reader.Read<UnityEngine.ParticleSystemOverlapAction>();
						break;
					case "radiusScale":
						instance.radiusScale = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
