using System;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3ArrayType : ES3CollectionType
	{
		public ES3ArrayType(Type type) : base(type){}
		public ES3ArrayType(Type type, ES3Type elementType) : base(type, elementType){}

		public override void Write(object obj, ES3Writer writer, ES3.ReferenceMode memberReferenceMode)
		{
			var array = (System.Array)obj;

			if(elementType == null)
				throw new ArgumentNullException("ES3Type argument cannot be null.");

			//writer.StartWriteCollection();

			for(int i=0; i<array.Length; i++)
			{
				writer.StartWriteCollectionItem(i);
                writer.Write(array.GetValue(i), elementType, memberReferenceMode);
				writer.EndWriteCollectionItem(i);
			}

			//writer.EndWriteCollection();
		}

        public override object Read(ES3Reader reader)
        {
            var list = new List<object>();
            if (!ReadICollection(reader, list, elementType))
                return null;

            var array = ES3Reflection.ArrayCreateInstance(elementType.type, list.Count);
            int i = 0;
            foreach (var item in list)
            {
                array.SetValue(item, i);
                i++;
            }

            return array;

            /*var instance = new List<object>();

			if(reader.StartReadCollection())
				return null;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;
				instance.Add(reader.Read<object>(elementType));

				if(reader.EndReadCollectionItem())
					break;
			}

			reader.EndReadCollection();

			var array = ES3Reflection.ArrayCreateInstance(elementType.type, instance.Count);
			int i = 0;
			foreach(var item in instance)
			{
				array.SetValue(item, i);
				i++;
			}

			return array;*/
        }

        public override object Read<T>(ES3Reader reader)
		{
            return Read(reader);
            /*var list = new List<object>();
			if(!ReadICollection(reader, list, elementType))
				return null;

            var array = ES3Reflection.ArrayCreateInstance(elementType.type, list.Count);
            int i = 0;
            foreach (var item in list)
            {
                array.SetValue(item, i);
                i++;
            }

            return array;*/
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			ReadICollectionInto(reader, (ICollection)obj, elementType);
		}

		public override void ReadInto(ES3Reader reader, object obj)
		{
            var collection = (IList)obj;

            if (collection.Count == 0)
                ES3Debug.LogWarning("LoadInto/ReadInto expects a collection containing instances to load data in to, but the collection is empty.");

			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			// Iterate through each item in the collection and try to load it.
			foreach(var item in collection)
			{
				itemsLoaded++;

				if(!reader.StartReadCollectionItem())
					break;

				reader.ReadInto<object>(item, elementType);

				// If we find a ']', we reached the end of the array.
				if(reader.EndReadCollectionItem())
					break;

				// If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
				if(itemsLoaded == collection.Count)
					throw new IndexOutOfRangeException("The collection we are loading is longer than the collection provided as a parameter.");
			}

			// If we loaded fewer items than the parameter collection, throw index out of range exception.
			if(itemsLoaded != collection.Count)
				throw new IndexOutOfRangeException("The collection we are loading is shorter than the collection provided as a parameter.");

			reader.EndReadCollection();
		}
	}
}