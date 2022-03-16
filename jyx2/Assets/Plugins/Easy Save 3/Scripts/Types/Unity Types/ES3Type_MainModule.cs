using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("duration", "loop", "prewarm", "startDelay", "startDelayMultiplier", "startLifetime", "startLifetimeMultiplier", "startSpeed", "startSpeedMultiplier", "startSize3D", "startSize", "startSizeMultiplier", "startSizeX", "startSizeXMultiplier", "startSizeY", "startSizeYMultiplier", "startSizeZ", "startSizeZMultiplier", "startRotation3D", "startRotation", "startRotationMultiplier", "startRotationX", "startRotationXMultiplier", "startRotationY", "startRotationYMultiplier", "startRotationZ", "startRotationZMultiplier", "randomizeRotationDirection", "startColor", "gravityModifier", "gravityModifierMultiplier", "simulationSpace", "customSimulationSpace", "simulationSpeed", "scalingMode", "playOnAwake", "maxParticles")]
	public class ES3Type_MainModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_MainModule() : base(typeof(UnityEngine.ParticleSystem.MainModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.MainModule)obj;

			writer.WriteProperty("duration", instance.duration, ES3Type_float.Instance);
			writer.WriteProperty("loop", instance.loop, ES3Type_bool.Instance);
			writer.WriteProperty("prewarm", instance.prewarm, ES3Type_bool.Instance);
			writer.WriteProperty("startDelay", instance.startDelay, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startDelayMultiplier", instance.startDelayMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startLifetime", instance.startLifetime, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startLifetimeMultiplier", instance.startLifetimeMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startSpeed", instance.startSpeed, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startSpeedMultiplier", instance.startSpeedMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startSize3D", instance.startSize3D, ES3Type_bool.Instance);
			writer.WriteProperty("startSize", instance.startSize, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startSizeMultiplier", instance.startSizeMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startSizeX", instance.startSizeX, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startSizeXMultiplier", instance.startSizeXMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startSizeY", instance.startSizeY, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startSizeYMultiplier", instance.startSizeYMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startSizeZ", instance.startSizeZ, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startSizeZMultiplier", instance.startSizeZMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startRotation3D", instance.startRotation3D, ES3Type_bool.Instance);
			writer.WriteProperty("startRotation", instance.startRotation, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startRotationMultiplier", instance.startRotationMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startRotationX", instance.startRotationX, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startRotationXMultiplier", instance.startRotationXMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startRotationY", instance.startRotationY, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startRotationYMultiplier", instance.startRotationYMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("startRotationZ", instance.startRotationZ, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("startRotationZMultiplier", instance.startRotationZMultiplier, ES3Type_float.Instance);
			#if UNITY_2018_1_OR_NEWER
			writer.WriteProperty("flipRotation", instance.flipRotation, ES3Type_float.Instance);
			#else
			writer.WriteProperty("randomizeRotationDirection", instance.randomizeRotationDirection, ES3Type_float.Instance);
			#endif
			writer.WriteProperty("startColor", instance.startColor, ES3Type_MinMaxGradient.Instance);
			writer.WriteProperty("gravityModifier", instance.gravityModifier, ES3Type_MinMaxCurve.Instance);
			writer.WriteProperty("gravityModifierMultiplier", instance.gravityModifierMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("simulationSpace", instance.simulationSpace);
			writer.WritePropertyByRef("customSimulationSpace", instance.customSimulationSpace);
			writer.WriteProperty("simulationSpeed", instance.simulationSpeed, ES3Type_float.Instance);
			writer.WriteProperty("scalingMode", instance.scalingMode);
			writer.WriteProperty("playOnAwake", instance.playOnAwake, ES3Type_bool.Instance);
			writer.WriteProperty("maxParticles", instance.maxParticles, ES3Type_int.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.MainModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.MainModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					case "duration":
						instance.duration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "loop":
						instance.loop = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "prewarm":
						instance.prewarm = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "startDelay":
						instance.startDelay = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startDelayMultiplier":
						instance.startDelayMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startLifetime":
						instance.startLifetime = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startLifetimeMultiplier":
						instance.startLifetimeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startSpeed":
						instance.startSpeed = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startSpeedMultiplier":
						instance.startSpeedMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startSize3D":
						instance.startSize3D = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "startSize":
						instance.startSize = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startSizeMultiplier":
						instance.startSizeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startSizeX":
						instance.startSizeX = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startSizeXMultiplier":
						instance.startSizeXMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startSizeY":
						instance.startSizeY = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startSizeYMultiplier":
						instance.startSizeYMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startSizeZ":
						instance.startSizeZ = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startSizeZMultiplier":
						instance.startSizeZMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startRotation3D":
						instance.startRotation3D = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "startRotation":
						instance.startRotation = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startRotationMultiplier":
						instance.startRotationMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startRotationX":
						instance.startRotationX = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startRotationXMultiplier":
						instance.startRotationXMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startRotationY":
						instance.startRotationY = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startRotationYMultiplier":
						instance.startRotationYMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "startRotationZ":
						instance.startRotationZ = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "startRotationZMultiplier":
						instance.startRotationZMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						#if UNITY_2018_1_OR_NEWER
						case "flipRotation":
						instance.flipRotation = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						case "randomizeRotationDirection":
						instance.flipRotation = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						#else
					case "randomizeRotationDirection":
						instance.randomizeRotationDirection = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						#endif
					case "startColor":
						instance.startColor = reader.Read<UnityEngine.ParticleSystem.MinMaxGradient>(ES3Type_MinMaxGradient.Instance);
						break;
					case "gravityModifier":
						instance.gravityModifier = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
						break;
					case "gravityModifierMultiplier":
						instance.gravityModifierMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "simulationSpace":
						instance.simulationSpace = reader.Read<UnityEngine.ParticleSystemSimulationSpace>();
						break;
					case "customSimulationSpace":
						instance.customSimulationSpace = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "simulationSpeed":
						instance.simulationSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "scalingMode":
						instance.scalingMode = reader.Read<UnityEngine.ParticleSystemScalingMode>();
						break;
					case "playOnAwake":
						instance.playOnAwake = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "maxParticles":
						instance.maxParticles = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
