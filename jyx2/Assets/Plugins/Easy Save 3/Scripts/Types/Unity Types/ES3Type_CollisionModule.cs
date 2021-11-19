using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "type", "mode", "dampen", "dampenMultiplier", "bounce", "bounceMultiplier", "lifetimeLoss", "lifetimeLossMultiplier", "minKillSpeed", "maxKillSpeed", "collidesWith", "enableDynamicColliders", "maxCollisionShapes", "quality", "voxelSize", "radiusScale", "sendCollisionMessages")]
	public class ES3Type_CollisionModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_CollisionModule() : base(typeof(UnityEngine.ParticleSystem.CollisionModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.CollisionModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled);
			writer.WriteProperty("type", instance.type);
			writer.WriteProperty("mode", instance.mode);
			writer.WriteProperty("dampen", instance.dampen);
			writer.WriteProperty("dampenMultiplier", instance.dampenMultiplier);
			writer.WriteProperty("bounce", instance.bounce);
			writer.WriteProperty("bounceMultiplier", instance.bounceMultiplier);
			writer.WriteProperty("lifetimeLoss", instance.lifetimeLoss);
			writer.WriteProperty("lifetimeLossMultiplier", instance.lifetimeLossMultiplier);
			writer.WriteProperty("minKillSpeed", instance.minKillSpeed);
			writer.WriteProperty("maxKillSpeed", instance.maxKillSpeed);
			writer.WriteProperty("collidesWith", instance.collidesWith);
			writer.WriteProperty("enableDynamicColliders", instance.enableDynamicColliders);
			writer.WriteProperty("maxCollisionShapes", instance.maxCollisionShapes);
			writer.WriteProperty("quality", instance.quality);
			writer.WriteProperty("voxelSize", instance.voxelSize);
			writer.WriteProperty("radiusScale", instance.radiusScale);
			writer.WriteProperty("sendCollisionMessages", instance.sendCollisionMessages);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.CollisionModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.CollisionModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>();
						break;
					case "type":
						instance.type = reader.Read<UnityEngine.ParticleSystemCollisionType>();
						break;
					case "mode":
						instance.mode = reader.Read<UnityEngine.ParticleSystemCollisionMode>();
						break;
					case "dampen":
						instance.dampen = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "dampenMultiplier":
						instance.dampenMultiplier = reader.Read<System.Single>();
						break;
					case "bounce":
						instance.bounce = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "bounceMultiplier":
						instance.bounceMultiplier = reader.Read<System.Single>();
						break;
					case "lifetimeLoss":
						instance.lifetimeLoss = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "lifetimeLossMultiplier":
						instance.lifetimeLossMultiplier = reader.Read<System.Single>();
						break;
					case "minKillSpeed":
						instance.minKillSpeed = reader.Read<System.Single>();
						break;
					case "maxKillSpeed":
						instance.maxKillSpeed = reader.Read<System.Single>();
						break;
					case "collidesWith":
						instance.collidesWith = reader.Read<UnityEngine.LayerMask>();
						break;
					case "enableDynamicColliders":
						instance.enableDynamicColliders = reader.Read<System.Boolean>();
						break;
					case "maxCollisionShapes":
						instance.maxCollisionShapes = reader.Read<System.Int32>();
						break;
					case "quality":
						instance.quality = reader.Read<UnityEngine.ParticleSystemCollisionQuality>();
						break;
					case "voxelSize":
						instance.voxelSize = reader.Read<System.Single>();
						break;
					case "radiusScale":
						instance.radiusScale = reader.Read<System.Single>();
						break;
					case "sendCollisionMessages":
						instance.sendCollisionMessages = reader.Read<System.Boolean>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}