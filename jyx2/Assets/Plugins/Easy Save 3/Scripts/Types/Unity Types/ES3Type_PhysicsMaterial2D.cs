using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("bounciness", "friction")]
	public class ES3Type_PhysicsMaterial2D : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_PhysicsMaterial2D() : base(typeof(UnityEngine.PhysicsMaterial2D)){ Instance = this; }

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.PhysicsMaterial2D)obj;
			
			writer.WriteProperty("bounciness", instance.bounciness, ES3Type_float.Instance);
			writer.WriteProperty("friction", instance.friction, ES3Type_float.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.PhysicsMaterial2D)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "bounciness":
						instance.bounciness = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "friction":
						instance.friction = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.PhysicsMaterial2D();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}

		public class ES3Type_PhysicsMaterial2DArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_PhysicsMaterial2DArray() : base(typeof(UnityEngine.PhysicsMaterial2D[]), ES3Type_PhysicsMaterial2D.Instance)
		{
			Instance = this;
		}
	}
}