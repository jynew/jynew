using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Id", "Count")]
	public class ES3UserType_CsRoleItem : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CsRoleItem() : base(typeof(Jyx2.CsRoleItem)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Jyx2.CsRoleItem)obj;
			
			writer.WriteProperty("Id", instance.Id, ES3Type_int.Instance);
			writer.WriteProperty("Count", instance.Count, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Jyx2.CsRoleItem)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Id":
						instance.Id = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Count":
						instance.Count = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Jyx2.CsRoleItem();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CsRoleItemArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CsRoleItemArray() : base(typeof(Jyx2.CsRoleItem[]), ES3UserType_CsRoleItem.Instance)
		{
			Instance = this;
		}
	}
}