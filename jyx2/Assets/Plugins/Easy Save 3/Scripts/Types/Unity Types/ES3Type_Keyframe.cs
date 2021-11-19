using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("time", "value", "inTangent", "outTangent")]
	public class ES3Type_Keyframe : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Keyframe() : base(typeof(UnityEngine.Keyframe))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Keyframe)obj;
			
			writer.WriteProperty("time", instance.time, ES3Type_float.Instance);
			writer.WriteProperty("value", instance.value, ES3Type_float.Instance);
			writer.WriteProperty("inTangent", instance.inTangent, ES3Type_float.Instance);
			writer.WriteProperty("outTangent", instance.outTangent, ES3Type_float.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return new UnityEngine.Keyframe(reader.ReadProperty<System.Single>(ES3Type_float.Instance),
											reader.ReadProperty<System.Single>(ES3Type_float.Instance),
											reader.ReadProperty<System.Single>(ES3Type_float.Instance),
											reader.ReadProperty<System.Single>(ES3Type_float.Instance));
		}
	}

		public class ES3Type_KeyframeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_KeyframeArray() : base(typeof(Keyframe[]), ES3Type_Keyframe.Instance)
		{
			Instance = this;
		}
	}
}
