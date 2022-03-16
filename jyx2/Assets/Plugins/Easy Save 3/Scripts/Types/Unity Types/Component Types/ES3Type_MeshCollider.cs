using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sharedMesh", "convex", "inflateMesh", "skinWidth", "enabled", "isTrigger", "contactOffset", "sharedMaterial")]
	public class ES3Type_MeshCollider : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_MeshCollider() : base(typeof(UnityEngine.MeshCollider))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.MeshCollider)obj;

            writer.WritePropertyByRef("sharedMesh", instance.sharedMesh);
            writer.WriteProperty("convex", instance.convex, ES3Type_bool.Instance);
			/*writer.WriteProperty("inflateMesh", instance.inflateMesh, ES3Type_bool.Instance);
			writer.WriteProperty("skinWidth", instance.skinWidth, ES3Type_float.Instance);*/
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("isTrigger", instance.isTrigger, ES3Type_bool.Instance);
			writer.WriteProperty("contactOffset", instance.contactOffset, ES3Type_float.Instance);
			writer.WriteProperty("material", instance.sharedMaterial);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.MeshCollider)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "sharedMesh":
						instance.sharedMesh = reader.Read<UnityEngine.Mesh>(ES3Type_Mesh.Instance);
						break;
					case "convex":
						instance.convex = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					/*case "inflateMesh":
						instance.inflateMesh = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "skinWidth":
						instance.skinWidth = reader.Read<System.Single>(ES3Type_float.Instance);
						break;*/
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isTrigger":
						instance.isTrigger = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "contactOffset":
						instance.contactOffset = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "material":
                        instance.sharedMaterial = reader.Read<UnityEngine.PhysicMaterial>(ES3Type_PhysicMaterial.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

		public class ES3Type_MeshColliderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_MeshColliderArray() : base(typeof(UnityEngine.MeshCollider[]), ES3Type_MeshCollider.Instance)
		{
			Instance = this;
		}
	}
}