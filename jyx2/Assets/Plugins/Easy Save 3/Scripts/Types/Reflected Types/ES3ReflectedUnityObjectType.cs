using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedUnityObjectType : ES3UnityObjectType
	{
		public ES3ReflectedUnityObjectType(Type type) : base(type)
		{
			isReflectedType = true;
			GetMembers(true);
		}

		protected override void WriteUnityObject(object obj, ES3Writer writer)
		{
			WriteProperties(obj, writer);
		}

		protected override object ReadUnityObject<T>(ES3Reader reader)
		{
			var obj = ES3Reflection.CreateInstance(this.type);
			ReadProperties(reader, obj);
			return obj;
		}

		protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
		{
			ReadProperties(reader, obj);
		}
	}
}
