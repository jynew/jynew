using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("WorldPosition", "WorldRotation", "BoatWorldPos", "BoatRotate", "OnBoat")]
	public class ES3UserType_WorldMapSaveData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_WorldMapSaveData() : base(typeof(WorldMapSaveData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (WorldMapSaveData)obj;
			
			writer.WriteProperty("WorldPosition", instance.WorldPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("WorldRotation", instance.WorldRotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("BoatWorldPos", instance.BoatWorldPos, ES3Type_Vector3.Instance);
			writer.WriteProperty("BoatRotate", instance.BoatRotate, ES3Type_Quaternion.Instance);
			writer.WriteProperty("OnBoat", instance.OnBoat, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (WorldMapSaveData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "WorldPosition":
						instance.WorldPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "WorldRotation":
						instance.WorldRotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "BoatWorldPos":
						instance.BoatWorldPos = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "BoatRotate":
						instance.BoatRotate = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "OnBoat":
						instance.OnBoat = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new WorldMapSaveData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_WorldMapSaveDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_WorldMapSaveDataArray() : base(typeof(WorldMapSaveData[]), ES3UserType_WorldMapSaveData.Instance)
		{
			Instance = this;
		}
	}
}