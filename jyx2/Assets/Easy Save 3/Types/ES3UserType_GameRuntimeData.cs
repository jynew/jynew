using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("AllRoles", "TeamId", "SubMapData", "WorldData", "KeyValues", "Items", "ItemUser", "ShopItems", "EventCounter", "MapPic", "ItemAdded", "_startDate")]
	public class ES3UserType_GameRuntimeData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GameRuntimeData() : base(typeof(Jyx2.GameRuntimeData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Jyx2.GameRuntimeData)obj;
			
			writer.WriteProperty("AllRoles", instance.AllRoles, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.Int32, Jyx2.RoleInstance>)));
			writer.WritePrivateField("TeamId", instance);
			writer.WriteProperty("SubMapData", instance.SubMapData, ES3UserType_SubMapSaveData.Instance);
			writer.WriteProperty("WorldData", instance.WorldData, ES3UserType_WorldMapSaveData.Instance);
			writer.WriteProperty("KeyValues", instance.KeyValues, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.String>)));
			writer.WriteProperty("Items", instance.Items, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, (System.Int32, System.Int32)>)));
			writer.WriteProperty("ItemUser", instance.ItemUser, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Int32>)));
			writer.WriteProperty("ShopItems", instance.ShopItems, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Int32>)));
			writer.WriteProperty("EventCounter", instance.EventCounter, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Int32>)));
			writer.WriteProperty("MapPic", instance.MapPic, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Int32>)));
			writer.WritePrivateField("ItemAdded", instance);
			writer.WritePrivateField("_startDate", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Jyx2.GameRuntimeData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "AllRoles":
						instance.AllRoles = reader.Read<System.Collections.Generic.Dictionary<System.Int32, Jyx2.RoleInstance>>();
						break;
					case "TeamId":
					reader.SetPrivateField("TeamId", reader.Read<System.Collections.Generic.List<System.Int32>>(), instance);
					break;
					case "SubMapData":
						instance.SubMapData = reader.Read<SubMapSaveData>(ES3UserType_SubMapSaveData.Instance);
						break;
					case "WorldData":
						instance.WorldData = reader.Read<WorldMapSaveData>(ES3UserType_WorldMapSaveData.Instance);
						break;
					case "KeyValues":
						instance.KeyValues = reader.Read<System.Collections.Generic.Dictionary<System.String, System.String>>();
						break;
					case "Items":
						instance.Items = reader.Read<System.Collections.Generic.Dictionary<System.String, (System.Int32, System.Int32)>>();
						break;
					case "ItemUser":
						instance.ItemUser = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Int32>>();
						break;
					case "ShopItems":
						instance.ShopItems = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Int32>>();
						break;
					case "EventCounter":
						instance.EventCounter = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Int32>>();
						break;
					case "MapPic":
						instance.MapPic = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Int32>>();
						break;
					case "ItemAdded":
					reader.SetPrivateField("ItemAdded", reader.Read<System.Collections.Generic.List<System.Int32>>(), instance);
					break;
					case "_startDate":
					reader.SetPrivateField("_startDate", reader.Read<System.DateTime>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Jyx2.GameRuntimeData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_GameRuntimeDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GameRuntimeDataArray() : base(typeof(Jyx2.GameRuntimeData[]), ES3UserType_GameRuntimeData.Instance)
		{
			Instance = this;
		}
	}
}