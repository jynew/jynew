using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_UIntPtr : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_UIntPtr() : base(typeof(UIntPtr))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((ulong)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (object)reader.Read_ulong();
		}
	}

	public class ES3Type_UIntPtrArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_UIntPtrArray() : base(typeof(UIntPtr[]), ES3Type_UIntPtr.Instance)
		{
			Instance = this;
		}
	}
}