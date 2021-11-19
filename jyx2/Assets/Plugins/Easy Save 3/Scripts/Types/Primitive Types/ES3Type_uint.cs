using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_uint : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_uint() : base(typeof(uint))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((uint)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_uint();
		}
	}

	public class ES3Type_uintArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_uintArray() : base(typeof(uint[]), ES3Type_uint.Instance)
		{
			Instance = this;
		}
	}
}