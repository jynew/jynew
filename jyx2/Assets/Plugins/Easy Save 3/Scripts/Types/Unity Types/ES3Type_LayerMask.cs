using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("colorKeys", "alphaKeys", "mode")]
	public class ES3Type_LayerMask : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_LayerMask() : base(typeof(UnityEngine.LayerMask))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.LayerMask)obj;

			writer.WriteProperty("value", instance.value, ES3Type_int.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			LayerMask instance = new LayerMask();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					case "value":
						instance = reader.Read<int>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}
}
