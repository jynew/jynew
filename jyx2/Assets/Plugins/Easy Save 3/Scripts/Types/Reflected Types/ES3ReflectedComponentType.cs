using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedComponentType : ES3ComponentType
	{
		public ES3ReflectedComponentType(Type type) : base(type)
		{
			isReflectedType = true;
			GetMembers(true);
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			WriteProperties(obj, writer);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			ReadProperties(reader, obj);
		}
	}
}