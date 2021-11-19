using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_IntPtr : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_IntPtr() : base(typeof(IntPtr))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive((long)(IntPtr)obj);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)(IntPtr)reader.Read_long();
		}
	}

	public class ES3Type_IntPtrArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_IntPtrArray() : base(typeof(IntPtr[]), ES3Type_IntPtr.Instance)
		{
			Instance = this;
		}
	}
}