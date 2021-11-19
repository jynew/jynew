using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("properties", "systems", "types")]
	public class ES3Type_SubEmittersModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_SubEmittersModule() : base(typeof(UnityEngine.ParticleSystem.SubEmittersModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.SubEmittersModule)obj;

			var seProperties = new ParticleSystemSubEmitterProperties[instance.subEmittersCount];
			var seSystems = new ParticleSystem[instance.subEmittersCount]; 
			var seTypes = new ParticleSystemSubEmitterType[instance.subEmittersCount];

			for(int i=0; i<instance.subEmittersCount; i++)
			{
				seProperties[i] = instance.GetSubEmitterProperties(i);
				seSystems[i] = instance.GetSubEmitterSystem(i);
				seTypes[i] = instance.GetSubEmitterType(i);
			}
			
			writer.WriteProperty("properties", seProperties);
			writer.WriteProperty("systems", seSystems);
			writer.WriteProperty("types", seTypes);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.SubEmittersModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.SubEmittersModule)obj;

			ParticleSystemSubEmitterProperties[] seProperties = null;
			ParticleSystem[] seSystems = null; 
			ParticleSystemSubEmitterType[] seTypes = null;

			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "properties":
						seProperties = reader.Read<ParticleSystemSubEmitterProperties[]>(new ES3ArrayType(typeof(ParticleSystemSubEmitterProperties[])));
						break;
					case "systems":
						seSystems = reader.Read<ParticleSystem[]>();
						break;
					case "types":
						seTypes = reader.Read<ParticleSystemSubEmitterType[]>();
						break;
					default:
						reader.Skip();
						break;
				}
			}

			if(seProperties != null)
			{
				for(int i=0; i<seProperties.Length; i++)
				{
					instance.RemoveSubEmitter(i);
					instance.AddSubEmitter(seSystems[i],seTypes[i],seProperties[i]);
				}
			}
		}
	}
}
