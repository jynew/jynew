using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_long : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_long() : base(typeof(long))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((long)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_long();
		}
	}

	public class ES3Type_longArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_longArray() : base(typeof(long[]), ES3Type_long.Instance)
		{
			Instance = this;
		}
	}
}