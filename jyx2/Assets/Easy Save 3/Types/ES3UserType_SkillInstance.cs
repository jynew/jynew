using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Key", "Level")]
	public class ES3UserType_SkillInstance : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SkillInstance() : base(typeof(Jyx2.SkillInstance)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Jyx2.SkillInstance)obj;
			
			writer.WriteProperty("Key", instance.Key, ES3Type_int.Instance);
			writer.WriteProperty("Level", instance.Level, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Jyx2.SkillInstance)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Key":
						instance.Key = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Level":
						instance.Level = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Jyx2.SkillInstance();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SkillInstanceArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SkillInstanceArray() : base(typeof(Jyx2.SkillInstance[]), ES3UserType_SkillInstance.Instance)
		{
			Instance = this;
		}
	}
}