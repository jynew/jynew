using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_string : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_string() : base(typeof(string))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((string)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return reader.Read_string();
		}
	}

	public class ES3Type_StringArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_StringArray() : base(typeof(string[]), ES3Type_string.Instance)
		{
			Instance = this;
		}
	}
}