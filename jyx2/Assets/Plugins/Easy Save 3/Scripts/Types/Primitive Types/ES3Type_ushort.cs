using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_ushort : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_ushort() : base(typeof(ushort))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((ushort)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)reader.Read_ushort();
		}
	}

	public class ES3Type_ushortArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ushortArray() : base(typeof(ushort[]), ES3Type_ushort.Instance)
		{
			Instance = this;
		}
	}
}