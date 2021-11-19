using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public abstract class ES3ObjectType : ES3Type
	{
		public ES3ObjectType(Type type) : base(type) {}

		protected abstract void WriteObject(object obj, ES3Writer writer);
		protected abstract object ReadObject<T>(ES3Reader reader);

		protected virtual void ReadObject<T>(ES3Reader reader, object obj)
		{
			throw new NotSupportedException("ReadInto is not supported for type "+type);
		}

		public override void Write(object obj, ES3Writer writer)
		{
            if (!WriteUsingDerivedType(obj, writer))
            {
                var baseType = ES3Reflection.BaseType(obj.GetType());
                if (baseType != typeof(object))
                {
                    var es3Type = ES3TypeMgr.GetOrCreateES3Type(baseType);
                    // If it's a Dictionary, we need to write it as a field with a property name.
                    if (es3Type.isDictionary || es3Type.isCollection)
                        writer.WriteProperty("_Values", obj, es3Type);
                }

                WriteObject(obj, writer);
            }
        }

		public override object Read<T>(ES3Reader reader)
		{
			string propertyName;
			while(true)
			{
				propertyName = ReadPropertyName(reader);

				if(propertyName == ES3Type.typeFieldName)
					return ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).Read<T>(reader);
				else if(propertyName == null)
					return null;
				else
				{
					reader.overridePropertiesName = propertyName;

					return ReadObject<T>(reader);
				}
			}
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			string propertyName;
			while(true)
			{
				propertyName = ReadPropertyName(reader);

				if(propertyName == ES3Type.typeFieldName)
				{
					ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).ReadInto<T>(reader, obj);
					return;
				}
                // This is important we return if the enumerator returns null, otherwise we will encounter an endless cycle.
                else if (propertyName == null)
					return;
				else
				{
					reader.overridePropertiesName = propertyName;
					ReadObject<T>(reader, obj);
				}
			}
		}
	}
}