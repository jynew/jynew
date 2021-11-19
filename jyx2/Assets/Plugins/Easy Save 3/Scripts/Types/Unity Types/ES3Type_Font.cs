using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("material", "name")]
	public class ES3Type_Font : ES3UnityObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_Font() : base(typeof(UnityEngine.Font)){ Instance = this; }

		protected override void WriteUnityObject(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Font)obj;

			writer.WriteProperty("name", instance.name, ES3Type_string.Instance);
            writer.WriteProperty("material", instance.material);
		}

		protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Font)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
				case "material":
					instance.material = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
					break;
				default:
					reader.Skip();
					break;
				}
			}
		}

		protected override object ReadUnityObject<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.Font(reader.ReadProperty<string>(ES3Type_string.Instance));
			ReadObject<T>(reader, instance);
			return instance;
		}
	}

	public class ES3Type_FontArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_FontArray() : base(typeof(UnityEngine.Font[]), ES3Type_Font.Instance)
		{
			Instance = this;
		}
	}
}