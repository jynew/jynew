using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "ratio", "lifetime", "lifetimeMultiplier", "minVertexDistance", "textureMode", "worldSpace", "dieWithParticles", "sizeAffectsWidth", "sizeAffectsLifetime", "inheritParticleColor", "colorOverLifetime", "widthOverTrail", "widthOverTrailMultiplier", "colorOverTrail")]
	public class ES3Type_TrailModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_TrailModule() : base(typeof(UnityEngine.ParticleSystem.TrailModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.TrailModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("ratio", instance.ratio, ES3Type_float.Instance);
			writer.WriteProperty("lifetime", instance.lifetime, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("lifetimeMultiplier", instance.lifetimeMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("minVertexDistance", instance.minVertexDistance, ES3Type_float.Instance);
			writer.WriteProperty("textureMode", instance.textureMode);
			writer.WriteProperty("worldSpace", instance.worldSpace, ES3Type_bool.Instance);
			writer.WriteProperty("dieWithParticles", instance.dieWithParticles, ES3Type_bool.Instance);
			writer.WriteProperty("sizeAffectsWidth", instance.sizeAffectsWidth, ES3Type_bool.Instance);
			writer.WriteProperty("sizeAffectsLifetime", instance.sizeAffectsLifetime, ES3Type_bool.Instance);
			writer.WriteProperty("inheritParticleColor", instance.inheritParticleColor, ES3Type_bool.Instance);
			writer.WriteProperty("colorOverLifetime", instance.colorOverLifetime, ES3Type_MinMaxGradient.Instance);
			writer.WriteProperty("widthOverTrail", instance.widthOverTrail, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("widthOverTrailMultiplier", instance.widthOverTrailMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("colorOverTrail", instance.colorOverTrail, ES3Type_MinMaxGradient.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.TrailModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.TrailModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "ratio":
						instance.ratio = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "lifetime":
						instance.lifetime = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "lifetimeMultiplier":
						instance.lifetimeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "minVertexDistance":
						instance.minVertexDistance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "textureMode":
						instance.textureMode = reader.Read<UnityEngine.ParticleSystemTrailTextureMode>();
						break;
					case "worldSpace":
						instance.worldSpace = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "dieWithParticles":
						instance.dieWithParticles = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "sizeAffectsWidth":
						instance.sizeAffectsWidth = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "sizeAffectsLifetime":
						instance.sizeAffectsLifetime = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "inheritParticleColor":
						instance.inheritParticleColor = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "colorOverLifetime":
						instance.colorOverLifetime = reader.Read<UnityEngine.ParticleSystem.MinMaxGradient>(ES3Type_MinMaxGradient.Instance);
						break;
					case "widthOverTrail":
						instance.widthOverTrail = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "widthOverTrailMultiplier":
						instance.widthOverTrailMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "colorOverTrail":
						instance.colorOverTrail = reader.Read<UnityEngine.ParticleSystem.MinMaxGradient>(ES3Type_MinMaxGradient.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
