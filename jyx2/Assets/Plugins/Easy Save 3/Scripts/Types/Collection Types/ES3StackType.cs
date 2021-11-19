using System;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;
using System.Reflection;
using System.Linq;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3StackType : ES3CollectionType
	{
		public ES3StackType(Type type) : base(type){}

		public override void Write(object obj, ES3Writer writer, ES3.ReferenceMode memberReferenceMode)
		{
			var list = (ICollection)obj;

			if(elementType == null)
				throw new ArgumentNullException("ES3Type argument cannot be null.");

			//writer.StartWriteCollection();

			int i = 0;
			foreach(object item in list)
			{
				writer.StartWriteCollectionItem(i);
				writer.Write(item, elementType, memberReferenceMode);
				writer.EndWriteCollectionItem(i);
				i++;
			}

			//writer.EndWriteCollection();
		}

		public override object Read<T>(ES3Reader reader)
		{
            return Read(reader);
			/*if(reader.StartReadCollection())
				return null;

			var stack = new Stack<T>();

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;
				stack.Push(reader.Read<T>(elementType));
				if(reader.EndReadCollectionItem())
					break;
			}

			reader.EndReadCollection();
			return stack;*/
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			var stack = (Stack<T>)obj;

			// Iterate through each item in the collection and try to load it.
			foreach(var item in stack)
			{
				itemsLoaded++;

				if(!reader.StartReadCollectionItem())
					break;

				reader.ReadInto<T>(item, elementType);

				// If we find a ']', we reached the end of the array.
				if(reader.EndReadCollectionItem())
					break;
				// If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
				if(itemsLoaded == stack.Count)
					throw new IndexOutOfRangeException("The collection we are loading is longer than the collection provided as a parameter.");
			}

			// If we loaded fewer items than the parameter collection, throw index out of range exception.
			if(itemsLoaded != stack.Count)
				throw new IndexOutOfRangeException("The collection we are loading is shorter than the collection provided as a parameter.");

			reader.EndReadCollection();
		}

		public override object Read(ES3Reader reader)
		{
			var instance = (IList)ES3Reflection.CreateInstance(ES3Reflection.MakeGenericType(typeof(List<>), elementType.type));

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

            ES3Reflection.GetMethods(instance.GetType(), "Reverse").FirstOrDefault(t => !t.IsStatic).Invoke(instance, new object[]{});
            return ES3Reflection.CreateInstance(type, instance);
            
		}

		public override void ReadInto(ES3Reader reader, object obj)
		{
			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			var collection = (ICollection)obj;

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