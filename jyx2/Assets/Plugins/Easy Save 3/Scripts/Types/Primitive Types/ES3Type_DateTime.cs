using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_DateTime : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_DateTime() : base(typeof(DateTime))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WriteProperty("ticks", ((DateTime)obj).Ticks, ES3Type_long.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			reader.ReadPropertyName();
			return new DateTime(reader.Read<long>(ES3Type_long.Instance));
		}
	}

	public class ES3Type_DateTimeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_DateTimeArray() : base(typeof(DateTime[]), ES3Type_DateTime.Instance)
		{
			Instance = this;
		}
	}
}