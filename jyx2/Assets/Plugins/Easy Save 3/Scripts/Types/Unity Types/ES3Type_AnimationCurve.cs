using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("keys", "preWrapMode", "postWrapMode")]
	public class ES3Type_AnimationCurve : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_AnimationCurve() : base(typeof(UnityEngine.AnimationCurve))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.AnimationCurve)obj;
			
			writer.WriteProperty("keys", instance.keys, ES3Type_KeyframeArray.Instance);
			writer.WriteProperty("preWrapMode", instance.preWrapMode);
			writer.WriteProperty("postWrapMode", instance.postWrapMode);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new UnityEngine.AnimationCurve();
			ReadInto<T>(reader, instance);
			return instance;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.AnimationCurve)obj;
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "keys":
						instance.keys = reader.Read<UnityEngine.Keyframe[]>();
						break;
					case "preWrapMode":
						instance.preWrapMode = reader.Read<UnityEngine.WrapMode>();
						break;
					case "postWrapMode":
						instance.postWrapMode = reader.Read<UnityEngine.WrapMode>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}
