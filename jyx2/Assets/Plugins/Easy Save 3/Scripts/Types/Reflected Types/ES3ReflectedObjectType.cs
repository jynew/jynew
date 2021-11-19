using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedObjectType : ES3ObjectType
	{
		public ES3ReflectedObjectType(Type type) : base(type)
		{
			isReflectedType = true;
			GetMembers(true);
		}

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			WriteProperties(obj, writer);

        }

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var obj = ES3Reflection.CreateInstance(this.type);
			ReadProperties(reader, obj);
			return obj;
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			ReadProperties(reader, obj);
		}
	}
}