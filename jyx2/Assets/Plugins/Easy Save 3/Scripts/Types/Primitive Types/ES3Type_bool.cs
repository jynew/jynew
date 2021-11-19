using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_bool : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_bool() : base(typeof(bool))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((bool)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_bool();
		}
	}

	public class ES3Type_boolArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_boolArray() : base(typeof(bool[]), ES3Type_bool.Instance)
		{
			Instance = this;
		}
	}
}