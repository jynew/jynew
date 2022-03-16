using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("name","maximumLOD")]
	public class ES3Type_Shader : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Shader() : base(typeof(UnityEngine.Shader)){ Instance = this; }

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Shader)obj;

			writer.WriteProperty("name", instance.name, ES3Type_string.Instance);
			writer.WriteProperty("maximumLOD", instance.maximumLOD, ES3Type_int.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			Shader obj = Shader.Find(reader.ReadProperty<string>(ES3Type_string.Instance));
			if(obj == null)
				obj = Shader.Find("Diffuse");
			ReadInto<T>(reader, obj);
			return obj;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Shader)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					case "name":
						instance.name = reader.Read<string>(ES3Type_string.Instance);
						break;
					case "maximumLOD":
						instance.maximumLOD = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

		public class ES3Type_ShaderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ShaderArray() : base(typeof(UnityEngine.Shader[]), ES3Type_Shader.Instance)
		{
			Instance = this;
		}
	}
}