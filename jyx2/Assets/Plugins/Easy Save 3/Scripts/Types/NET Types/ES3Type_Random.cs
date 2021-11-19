using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("inext", "inextp", "SeedArray")]
	public class ES3Type_Random : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_Random() : base(typeof(System.Random)){ Instance = this; }

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (System.Random)obj;
			
			writer.WritePrivateField("inext", instance);
			writer.WritePrivateField("inextp", instance);
			writer.WritePrivateField("SeedArray", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (System.Random)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "inext":
					reader.SetPrivateField("inext", reader.Read<System.Int32>(), instance);
					break;
					case "inextp":
					reader.SetPrivateField("inextp", reader.Read<System.Int32>(), instance);
					break;
					case "SeedArray":
					reader.SetPrivateField("SeedArray", reader.Read<System.Int32[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new System.Random();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}

	public class ES3Type_RandomArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_RandomArray() : base(typeof(System.Random[]), ES3Type_Random.Instance)
		{
			Instance = this;
		}
	}
}