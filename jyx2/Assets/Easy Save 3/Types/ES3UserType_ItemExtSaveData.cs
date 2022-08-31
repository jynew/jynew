using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("getTime", "count")]
	public class ES3UserType_ItemExtSaveData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ItemExtSaveData() : base(typeof(Jyx2.ItemExtSaveData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Jyx2.ItemExtSaveData)obj;
			
			writer.WriteProperty("getTime", instance.getTime, ES3Type_int.Instance);
			writer.WriteProperty("count", instance.count, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Jyx2.ItemExtSaveData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "getTime":
						instance.getTime = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "count":
						instance.count = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Jyx2.ItemExtSaveData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ItemExtSaveDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ItemExtSaveDataArray() : base(typeof(Jyx2.ItemExtSaveData[]), ES3UserType_ItemExtSaveData.Instance)
		{
			Instance = this;
		}
	}
}