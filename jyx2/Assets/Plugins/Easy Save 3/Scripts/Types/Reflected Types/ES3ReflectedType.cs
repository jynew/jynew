using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	internal class ES3ReflectedType : ES3Type
	{
		public ES3ReflectedType(Type type) : base(type)
		{
			isReflectedType = true;
		}

		// Constructs a reflected ES3Type, only serializing members which are in the provided members array.
		public ES3ReflectedType(Type type, string[] members) : this(type)
		{
			GetMembers(false, members);
		}

		public override void Write(object obj, ES3Writer writer)
		{
			// Manage NULL values.
			if(obj == null){writer.WriteNull(); return;};

			UnityEngine.Object unityObj = obj as UnityEngine.Object;
			bool isUnityEngineObject = (unityObj != null);

			// If this is a derived type, write the type as a property and use it's specific ES3Type.
			var objType = obj.GetType();
			if(objType != this.type)
			{
				writer.WriteType(objType);
				ES3TypeMgr.GetOrCreateES3Type(objType).Write(obj, writer);
				return;
			}

			if(isUnityEngineObject)
				writer.WriteRef(unityObj);

			if(members == null)
				GetMembers(writer.settings.safeReflection);
			for(int i=0; i<members.Length; i++)
			{
				var property = members[i];

				if(ES3Reflection.IsAssignableFrom(typeof(UnityEngine.Object), property.type))
				{
					object valueObj = property.reflectedMember.GetValue(obj);
					UnityEngine.Object value = (valueObj == null) ? null : (UnityEngine.Object)valueObj;

					writer.WritePropertyByRef(property.name, value);
				}
				else
					writer.WriteProperty(property.name, property.reflectedMember.GetValue(obj), ES3TypeMgr.GetOrCreateES3Type(property.type));
			}
        }

		public override object Read<T>(ES3Reader reader)
		{
			if(members == null)
				GetMembers(reader.settings.safeReflection);

			object obj;
			string propertyName = reader.ReadPropertyName();

			// If we're loading a derived type, use it's specific ES3Type.
			if(propertyName == ES3Type.typeFieldName)
				return ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).Read<T>(reader);

			// If we're loading a reference, load it. Else, create an instance.
			if(propertyName == ES3ReferenceMgrBase.referencePropertyName)
			{
				long id = reader.Read_ref();
				obj = ES3ReferenceMgrBase.Current.Get(id, type);
				if(obj == null)
				{
					// If an instance isn't already registered for this object, create an instance and register the reference.
					obj = ES3Reflection.CreateInstance(this.type);
					ES3ReferenceMgrBase.Current.Add((UnityEngine.Object)obj, id);
				}
			}
			else
			{
				reader.overridePropertiesName = propertyName;
				obj = ES3Reflection.CreateInstance(this.type);
			}

			// Iterate through each property in the file and try to load it using the appropriate
			// ES3Property in the members array.
			ReadProperties(reader, obj);

			return obj;
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			if(members == null)
				GetMembers(reader.settings.safeReflection);

			string propertyName = reader.ReadPropertyName();

			// If we're loading a derived type, use it's specific ES3Type.
			if(propertyName == ES3Type.typeFieldName)
			{
				ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).ReadInto<T>(reader, obj);
				return;
			}
			else 
				reader.overridePropertiesName = propertyName;

			// Iterate through each property in the file and try to load it using the appropriate
			// ES3Property in the members array.
			ReadProperties(reader, obj);
		}
	}
}