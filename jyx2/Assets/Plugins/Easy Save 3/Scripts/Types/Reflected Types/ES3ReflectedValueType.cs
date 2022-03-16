using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedValueType : ES3Type
	{
		public ES3ReflectedValueType(Type type) : base(type)
		{
			isReflectedType = true;
			GetMembers(true);
		}

		public override void Write(object obj, ES3Writer writer)
		{
			WriteProperties(obj, writer);
		}

		public override object Read<T>(ES3Reader reader)
		{
			var obj = ES3Reflection.CreateInstance(this.type);

			if(obj == null)
				throw new NotSupportedException("Cannot create an instance of "+this.type+". However, you may be able to add support for it using a custom ES3Type file. For more information see: http://docs.moodkie.com/easy-save-3/es3-guides/controlling-serialization-using-es3types/");
			// Make sure we return the result of ReadProperties as properties aren't assigned by reference.
			return ReadProperties(reader, obj);
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			throw new NotSupportedException("Cannot perform self-assigning load on a value type.");
		}
	}
}