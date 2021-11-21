using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("MapId", "CurrentPos", "CurrentOri")]
	public class ES3UserType_SubMapSaveData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SubMapSaveData() : base(typeof(SubMapSaveData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SubMapSaveData)obj;
			
			writer.WriteProperty("MapId", instance.MapId, ES3Type_int.Instance);
			writer.WriteProperty("CurrentPos", instance.CurrentPos, ES3Type_Vector3.Instance);
			writer.WriteProperty("CurrentOri", instance.CurrentOri, ES3Type_Quaternion.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SubMapSaveData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "MapId":
						instance.MapId = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "CurrentPos":
						instance.CurrentPos = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "CurrentOri":
						instance.CurrentOri = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SubMapSaveData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SubMapSaveDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SubMapSaveDataArray() : base(typeof(SubMapSaveData[]), ES3UserType_SubMapSaveData.Instance)
		{
			Instance = this;
		}
	}
}