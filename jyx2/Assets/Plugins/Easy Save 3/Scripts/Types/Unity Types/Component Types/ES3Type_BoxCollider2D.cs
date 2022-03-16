using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("size", "density", "isTrigger", "usedByEffector", "offset", "sharedMaterial", "enabled")]
	public class ES3Type_BoxCollider2D : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_BoxCollider2D() : base(typeof(UnityEngine.BoxCollider2D))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.BoxCollider2D)obj;
			
			writer.WriteProperty("size", instance.size);
			if(instance.attachedRigidbody != null && instance.attachedRigidbody.useAutoMass)
				writer.WriteProperty("density", instance.density);
			writer.WriteProperty("isTrigger", instance.isTrigger);
			writer.WriteProperty("usedByEffector", instance.usedByEffector);
			writer.WriteProperty("offset", instance.offset);
			writer.WritePropertyByRef("sharedMaterial", instance.sharedMaterial);
			writer.WriteProperty("enabled", instance.enabled);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.BoxCollider2D)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "size":
						instance.size = reader.Read<UnityEngine.Vector2>();
						break;
					case "density":
						instance.density = reader.Read<System.Single>();
						break;
					case "isTrigger":
						instance.isTrigger = reader.Read<System.Boolean>();
						break;
					case "usedByEffector":
						instance.usedByEffector = reader.Read<System.Boolean>();
						break;
					case "offset":
						instance.offset = reader.Read<UnityEngine.Vector2>();
						break;
					case "sharedMaterial":
						instance.sharedMaterial = reader.Read<UnityEngine.PhysicsMaterial2D>();
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}