using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("center", "size", "enabled", "isTrigger", "contactOffset", "sharedMaterial")]
	public class ES3Type_BoxCollider : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_BoxCollider() : base(typeof(UnityEngine.BoxCollider))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.BoxCollider)obj;
			
			writer.WriteProperty("center", instance.center);
			writer.WriteProperty("size", instance.size);
			writer.WriteProperty("enabled", instance.enabled);
			writer.WriteProperty("isTrigger", instance.isTrigger);
			writer.WriteProperty("contactOffset", instance.contactOffset);
			writer.WritePropertyByRef("material", instance.sharedMaterial);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.BoxCollider)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "center":
						instance.center = reader.Read<UnityEngine.Vector3>();
						break;
					case "size":
						instance.size = reader.Read<UnityEngine.Vector3>();
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>();
						break;
					case "isTrigger":
						instance.isTrigger = reader.Read<System.Boolean>();
						break;
					case "contactOffset":
						instance.contactOffset = reader.Read<System.Single>();
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