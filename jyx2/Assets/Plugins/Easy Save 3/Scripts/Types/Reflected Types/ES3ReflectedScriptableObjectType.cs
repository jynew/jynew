using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedScriptableObjectType : ES3ScriptableObjectType
	{
		public ES3ReflectedScriptableObjectType(Type type) : base(type)
		{
			isReflectedType = true;
			GetMembers(true);
		}

		protected override void WriteScriptableObject(object obj, ES3Writer writer)
		{
			WriteProperties(obj, writer);
		}

		protected override void ReadScriptableObject<T>(ES3Reader reader, object obj)
		{
			ReadProperties(reader, obj);
		}
	}
}