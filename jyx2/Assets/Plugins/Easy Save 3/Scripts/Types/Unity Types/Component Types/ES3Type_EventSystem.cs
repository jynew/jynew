using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_EventSystem : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_EventSystem() : base(typeof(EventSystem))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			foreach(string propertyName in reader.Properties)
				reader.Skip();
		}
	}
}