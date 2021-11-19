using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_short : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_short() : base(typeof(short))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((short)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_short();
		}
	}

	public class ES3Type_shortArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_shortArray() : base(typeof(short[]), ES3Type_short.Instance)
		{
			Instance = this;
		}
	}
}