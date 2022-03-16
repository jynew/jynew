using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "width", "height")]
	public class ES3Type_Rect : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Rect() : base(typeof(UnityEngine.Rect))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Rect)obj;
			
			writer.WriteProperty("x", instance.x, ES3Type_float.Instance);
			writer.WriteProperty("y", instance.y, ES3Type_float.Instance);
			writer.WriteProperty("width", instance.width, ES3Type_float.Instance);
			writer.WriteProperty("height", instance.height, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new Rect(reader.ReadProperty<float>(ES3Type_float.Instance), 
							reader.ReadProperty<float>(ES3Type_float.Instance), 
							reader.ReadProperty<float>(ES3Type_float.Instance), 
							reader.ReadProperty<float>(ES3Type_float.Instance));
		}
	}
}
