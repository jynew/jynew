using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled", "shapeType", "randomDirectionAmount", "sphericalDirectionAmount", "alignToDirection", "radius", "angle", "length", "box", "meshShapeType", "mesh", "meshRenderer", "skinnedMeshRenderer", "useMeshMaterialIndex", "meshMaterialIndex", "useMeshColors", "normalOffset", "meshScale", "arc")]
	public class ES3Type_ShapeModule : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_ShapeModule() : base(typeof(UnityEngine.ParticleSystem.ShapeModule))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.ParticleSystem.ShapeModule)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("shapeType", instance.shapeType);
			writer.WriteProperty("randomDirectionAmount", instance.randomDirectionAmount, ES3Type_float.Instance);
			writer.WriteProperty("sphericalDirectionAmount", instance.sphericalDirectionAmount, ES3Type_float.Instance);
			writer.WriteProperty("alignToDirection", instance.alignToDirection, ES3Type_bool.Instance);
			writer.WriteProperty("radius", instance.radius, ES3Type_float.Instance);
			writer.WriteProperty("angle", instance.angle, ES3Type_float.Instance);
			writer.WriteProperty("length", instance.length, ES3Type_float.Instance);
			#if UNITY_5
			writer.WriteProperty("box", instance.box, ES3Type_Vector3.Instance);
			writer.WriteProperty("meshScale", instance.meshScale, ES3Type_float.Instance);
			#else
			writer.WriteProperty("scale", instance.scale, ES3Type_Vector3.Instance);
			#endif
			writer.WriteProperty("meshShapeType", instance.meshShapeType);
			writer.WritePropertyByRef("mesh", instance.mesh);
			writer.WritePropertyByRef("meshRenderer", instance.meshRenderer);
			writer.WritePropertyByRef("skinnedMeshRenderer", instance.skinnedMeshRenderer);
			writer.WriteProperty("useMeshMaterialIndex", instance.useMeshMaterialIndex, ES3Type_bool.Instance);
			writer.WriteProperty("meshMaterialIndex", instance.meshMaterialIndex, ES3Type_int.Instance);
			writer.WriteProperty("useMeshColors", instance.useMeshColors, ES3Type_bool.Instance);
			writer.WriteProperty("normalOffset", instance.normalOffset, ES3Type_float.Instance);
			writer.WriteProperty("arc", instance.arc, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.ParticleSystem.ShapeModule();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.ParticleSystem.ShapeModule)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "shapeType":
						instance.shapeType = reader.Read<UnityEngine.ParticleSystemShapeType>();
						break;
					case "randomDirectionAmount":
						instance.randomDirectionAmount = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "sphericalDirectionAmount":
						instance.sphericalDirectionAmount = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "alignToDirection":
						instance.alignToDirection = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "radius":
						instance.radius = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "angle":
						instance.angle = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "length":
						instance.length = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						#if UNITY_5
					case "box":
						instance.box = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "meshScale":
						instance.meshScale = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
						#else
					case "scale":
						instance.scale = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					#endif
					case "meshShapeType":
						instance.meshShapeType = reader.Read<UnityEngine.ParticleSystemMeshShapeType>();
						break;
					case "mesh":
						instance.mesh = reader.Read<UnityEngine.Mesh>();
						break;
					case "meshRenderer":
						instance.meshRenderer = reader.Read<UnityEngine.MeshRenderer>();
						break;
					case "skinnedMeshRenderer":
						instance.skinnedMeshRenderer = reader.Read<UnityEngine.SkinnedMeshRenderer>();
						break;
					case "useMeshMaterialIndex":
						instance.useMeshMaterialIndex = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "meshMaterialIndex":
						instance.meshMaterialIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "useMeshColors":
						instance.useMeshColors = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "normalOffset":
						instance.normalOffset = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "arc":
						instance.arc = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
