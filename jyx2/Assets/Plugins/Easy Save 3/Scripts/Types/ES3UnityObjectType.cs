using System;
using UnityEngine;
using System.Collections;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public abstract class ES3UnityObjectType : ES3ObjectType
	{
		public ES3UnityObjectType(Type type) : base(type)
		{
			this.isValueType = false;
			isES3TypeUnityObject = true;
		}

		protected abstract void WriteUnityObject(object obj, ES3Writer writer);
		protected abstract void ReadUnityObject<T>(ES3Reader reader, object obj);
		protected abstract object ReadUnityObject<T>(ES3Reader reader);

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			WriteObject(obj, writer, ES3.ReferenceMode.ByRefAndValue);
		}

		public virtual void WriteObject(object obj, ES3Writer writer, ES3.ReferenceMode mode)
		{
			if(WriteUsingDerivedType(obj, writer, mode))
				return;

			var instance = obj as UnityEngine.Object;
			if(obj != null && instance == null)
				throw new ArgumentException("Only types of UnityEngine.Object can be written with this method, but argument given is type of "+obj.GetType());

			// If this object is in the instance manager, store it's instance ID with it.
			var refMgr = ES3ReferenceMgrBase.Current;
			if(mode != ES3.ReferenceMode.ByValue)
			{
                if(refMgr == null)
                    throw new InvalidOperationException("An Easy Save 3 Manager is required to load references. To add one to your scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene");
                writer.WriteRef(instance);
				if(mode == ES3.ReferenceMode.ByRef)
					return;
			}
			WriteUnityObject(instance, writer);
		}

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var refMgr = ES3ReferenceMgrBase.Current;
            if (refMgr != null)
            {
                foreach (string propertyName in reader.Properties)
                {
                    if (propertyName == ES3ReferenceMgrBase.referencePropertyName)
                        // If the object we're loading into isn't registered with the reference manager, register it.
                        refMgr.Add((UnityEngine.Object)obj, reader.Read_ref());
                    else
                    {
                        reader.overridePropertiesName = propertyName;
                        break;
                    }
                }
            }
            ReadUnityObject<T>(reader, obj);
        }

        protected override object ReadObject<T>(ES3Reader reader)
		{
			var refMgr = ES3ReferenceMgrBase.Current;
			if(refMgr == null)
				return ReadUnityObject<T>(reader);

			long id = -1;
			UnityEngine.Object instance = null;

			foreach(string propertyName in reader.Properties)
			{
				if(propertyName == ES3ReferenceMgrBase.referencePropertyName)
				{
                    if(refMgr == null)
                        throw new InvalidOperationException("An Easy Save 3 Manager is required to load references. To add one to your scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene");
                    id = reader.Read_ref();
					instance = refMgr.Get(id, type);

					if(instance != null)
						break;
				}
				else
				{
					reader.overridePropertiesName = propertyName;
                    if (instance == null)
                    {
                        instance = (UnityEngine.Object)ReadUnityObject<T>(reader);
                        refMgr.Add(instance, id);
                    }
					break;
				}
			}

			ReadUnityObject<T>(reader, instance);
			return instance;
		}

        protected bool WriteUsingDerivedType(object obj, ES3Writer writer, ES3.ReferenceMode mode)
        {
            var objType = obj.GetType();

            if (objType != this.type)
            {
                writer.WriteType(objType);

                var es3Type = ES3TypeMgr.GetOrCreateES3Type(objType);
                if (es3Type is ES3UnityObjectType)
                    ((ES3UnityObjectType)es3Type).WriteObject(obj, writer, mode);
                else
                    es3Type.Write(obj, writer);

                return true;
            }
            return false;
        }
    }
}