using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("center", "radius", "height", "direction", "enabled", "isTrigger", "contactOffset", "sharedMaterial")]
	public class ES3Type_CapsuleCollider : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_CapsuleCollider() : base(typeof(UnityEngine.CapsuleCollider))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.CapsuleCollider)obj;
			
			writer.WriteProperty("center", instance.center, ES3Type_Vector3.Instance);
			writer.WriteProperty("radius", instance.radius, ES3Type_float.Instance);
			writer.WriteProperty("height", instance.height, ES3Type_float.Instance);
			writer.WriteProperty("direction", instance.direction, ES3Type_int.Instance);

			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("isTrigger", instance.isTrigger, ES3Type_bool.Instance);
			writer.WriteProperty("contactOffset", instance.contactOffset, ES3Type_float.Instance);
			writer.WritePropertyByRef("material", instance.sharedMaterial);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.CapsuleCollider)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					case "center":
						instance.center = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "radius":
						instance.radius = reader.Read<float>(ES3Type_float.Instance);
						break;
					case "height":
						instance.height = reader.Read<float>(ES3Type_float.Instance);
						break;
					case "direction":
						instance.direction = reader.Read<int>(ES3Type_int.Instance);
						break;
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
                        instance.sharedMaterial = reader.Read<UnityEngine.PhysicMaterial>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}