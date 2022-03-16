using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "mode", "curve", "curveMultiplier")]
	public class ES3Type_InheritVelocityModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_InheritVelocityModule() : base(typeof(UnityEngine.ParticleSystem.InheritVelocityModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.InheritVelocityModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("mode", instance.mode);
			writer.WriteProperty("curve", instance.curve, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("curveMultiplier", instance.curveMultiplier, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.InheritVelocityModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.InheritVelocityModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "mode":
						instance.mode = reader.Read<UnityEngine.ParticleSystemInheritVelocityMode>();
						break;
					case "curve":
						instance.curve = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "curveMultiplier":
						instance.curveMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
