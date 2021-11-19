using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using ES3Types;
using ES3Internal;

public abstract class ES3Reader : System.IDisposable
{
	/// <summary>The settings used to create this reader.</summary>
	public ES3Settings settings;

    protected int serializationDepth = 0;

	#region ES3Reader Abstract Methods

	internal abstract int 		Read_int();
	internal abstract float 	Read_float();
	internal abstract bool 		Read_bool();
	internal abstract char 		Read_char();
	internal abstract decimal 	Read_decimal();
	internal abstract double 	Read_double();
	internal abstract long 		Read_long();
	internal abstract ulong 	Read_ulong();
	internal abstract byte 		Read_byte();
	internal abstract sbyte 	Read_sbyte();
	internal abstract short 	Read_short();
	internal abstract ushort 	Read_ushort();
	internal abstract uint 		Read_uint();
	internal abstract string 	Read_string();
	internal abstract byte[]	Read_byteArray();
    internal abstract long      Read_ref();

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract string ReadPropertyName();

	protected abstract Type ReadKeyPrefix(bool ignore = false);
	protected abstract void ReadKeySuffix();
	internal abstract byte[] ReadElement(bool skip=false);

	/// <summary>Disposes of the reader and it's underlying stream.</summary>
	public abstract void Dispose();

	// Seeks to the given key. Note that the stream position will not be reset.
    internal virtual bool Goto(string key)
    {
        if (key == null)
            throw new ArgumentNullException("Key cannot be NULL when loading data.");

        string currentKey;
        while ((currentKey = ReadPropertyName()) != key)
        {
            if (currentKey == null)
                return false;
            Skip();
        }
        return true;
    }

    internal virtual bool StartReadObject()
    {
        serializationDepth++;
        return false;
    }

	internal virtual void EndReadObject()
    {
        serializationDepth--;
    }

	internal abstract bool StartReadDictionary();
	internal abstract void EndReadDictionary();
	internal abstract bool StartReadDictionaryKey();
	internal abstract void EndReadDictionaryKey();
	internal abstract void StartReadDictionaryValue();
	internal abstract bool EndReadDictionaryValue();

	internal abstract bool StartReadCollection();
	internal abstract void EndReadCollection();
	internal abstract bool StartReadCollectionItem();
	internal abstract bool EndReadCollectionItem();

	#endregion

	internal ES3Reader(ES3Settings settings, bool readHeaderAndFooter = true)
	{
		this.settings = settings;
	}

	// If this is not null, the next call to the Properties will return this name.
	internal string overridePropertiesName = null;
	/// <summary>Allows you to enumerate over each field name. This should only be used within an ES3Type file.</summary>
	public virtual ES3ReaderPropertyEnumerator Properties
	{
		get
		{
			return new ES3ReaderPropertyEnumerator (this);
		}
	}

	internal virtual ES3ReaderRawEnumerator RawEnumerator
	{
		get
		{
			return new ES3ReaderRawEnumerator (this);
		}
	}

	/*
	 * 	Skips the current object in the stream.
	 * 	Stream position should be somewhere before the opening brace for the object.
	 * 	When this method successfully exits, it will be on the closing brace for the object.
	 */
	/// <summary>Skips the current object in the stream.</summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public virtual void Skip()
	{
		ReadElement(true);
	}

	/// <summary>Reads a value of type T from the reader.</summary>
	public virtual T Read<T>()
	{ 
		return Read<T>(ES3TypeMgr.GetOrCreateES3Type(typeof(T))); 
	}

	/// <summary>Reads a value of type T from the reader into an existing object.</summary>
	/// <param name="obj">The object we want to read our value into.</param>
	public virtual void ReadInto<T>(object obj)
	{ 
		ReadInto<T>(obj, ES3TypeMgr.GetOrCreateES3Type(typeof(T))); 
	}

	/// <summary>Reads a property (i.e. a property name and value) from the reader, ignoring the property name and only returning the value.</summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public T ReadProperty<T>()
	{
		return ReadProperty<T>(ES3TypeMgr.GetOrCreateES3Type(typeof(T)));
	}

	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public T ReadProperty<T>(ES3Type type)
	{
		ReadPropertyName();
		return Read<T>(type);
	}

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public long ReadRefProperty()
    {
        ReadPropertyName();
        return Read_ref();
    }

    internal Type ReadType()
	{
		return ES3Reflection.GetType(Read<string>(ES3Type_string.Instance));
	}

	/// <summary>Sets the value of a private property on an object.</summary>
	/// <param name="name">The name of the property we want to set.</param>
	/// <param name="value">The value we want to set the property to.</param>
	/// <param name="objectContainingProperty">The object containing the property we want to set.</param>
	public void SetPrivateProperty(string name, object value, object objectContainingProperty)
	{
		var property = ES3Reflection.GetES3ReflectedProperty(objectContainingProperty.GetType(), name);
		if(property.IsNull)
			throw new MissingMemberException("A private property named "+ name + " does not exist in the type "+objectContainingProperty.GetType());
		property.SetValue(objectContainingProperty, value);
	}

	/// <summary>Sets the value of a private field on an object.</summary>
	/// <param name="name">The name of the field we want to set.</param>
	/// <param name="value">The value we want to set the field to.</param>
	/// <param name="objectContainingProperty">The object containing the field we want to set.</param>
	public void SetPrivateField(string name, object value, object objectContainingField)
	{
		var field = ES3Reflection.GetES3ReflectedMember(objectContainingField.GetType(), name);
		if(field.IsNull)
			throw new MissingMemberException("A private field named "+ name + " does not exist in the type "+objectContainingField.GetType());
		field.SetValue(objectContainingField, value);
	}

	#region Read(key) & Read(key, obj) methods

	/// <summary>Reads a value from the reader with the given key.</summary>
	/// <param name="key">The key which uniquely identifies our value.</param>
	public virtual T Read<T>(string key)
	{
		if(!Goto(key))
			throw new KeyNotFoundException("Key \"" + key + "\" was not found in file \""+settings.FullPath+"\". Use Load<T>(key, defaultValue) if you want to return a default value if the key does not exist.");

		Type type = ReadTypeFromHeader<T>();

		T obj = Read<T>(ES3TypeMgr.GetOrCreateES3Type(type));

		//ReadKeySuffix(); //No need to read key suffix as we're returning. Doing so would throw an error at this point for BinaryReaders.
		return obj;
	}

	/// <summary>Reads a value from the reader with the given key, returning the default value if the key does not exist.</summary>
	/// <param name="key">The key which uniquely identifies our value.</param>
	/// <param name="defaultValue">The value we want to return if this key does not exist in the reader.</param>
	public virtual T Read<T>(string key, T defaultValue)
	{
		if(!Goto(key))
			return defaultValue;

		Type type = ReadTypeFromHeader<T>();
		T obj = Read<T>(ES3TypeMgr.GetOrCreateES3Type(type));

        //ReadKeySuffix(); //No need to read key suffix as we're returning. Doing so would throw an error at this point for BinaryReaders.
        return obj;
	}

	/// <summary>Reads a value from the reader with the given key into the provided object.</summary>
	/// <param name="key">The key which uniquely identifies our value.</param>
	/// <param name="obj">The object we want to load the value into.</param>
	public virtual void ReadInto<T>(string key, T obj) where T : class
	{
		if(!Goto(key))
			throw new KeyNotFoundException("Key \"" + key + "\" was not found in file \""+settings.FullPath+"\"");

		Type type = ReadTypeFromHeader<T>();

		ReadInto<T>(obj, ES3TypeMgr.GetOrCreateES3Type(type));

        //ReadKeySuffix(); //No need to read key suffix as we're returning. Doing so would throw an error at this point for BinaryReaders.
    }

    protected virtual void ReadObject<T>(object obj, ES3Type type)
	{
		// Check for null.
		if(StartReadObject())
			return;

		type.ReadInto<T>(this, obj);

		EndReadObject();
	}

	protected virtual T ReadObject<T>(ES3Type type)
	{
		if(StartReadObject())
			return default(T);

		object obj = type.Read<T>(this);

		EndReadObject();
		return (T)obj;
	}
		

	#endregion

	#region Read(ES3Type) & Read(obj,ES3Type) methods

	/*
	 * 	Parses the next JSON Object in the stream (i.e. must be between '{' and '}' chars).
	 * 	If the first character in the Stream is not a '{', it will throw an error.
	 * 	Will also read the terminating '}'.
	 * 	If we have reached the end of stream, it will return null.
	 */
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public virtual T Read<T>(ES3Type type)
	{
        if (type == null || type.isUnsupported)
            throw new NotSupportedException("Type of " + type + " is not currently supported, and could not be loaded using reflection.");
        else if (type.isPrimitive)
            return (T)type.Read<T>(this);
        else if (type.isCollection)
            return (T)((ES3CollectionType)type).Read(this);
        else if (type.isDictionary)
            return (T)((ES3DictionaryType)type).Read(this);
        else
            return ReadObject<T>(type);
	}

	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public virtual void ReadInto<T>(object obj, ES3Type type)
	{
		if(type == null || type.isUnsupported)
			throw new NotSupportedException("Type of "+obj.GetType()+" is not currently supported, and could not be loaded using reflection.");

		else if(type.isCollection)
			((ES3CollectionType)type).ReadInto(this, obj);
		else if(type.isDictionary)
			((ES3DictionaryType)type).ReadInto(this, obj);
		else
			ReadObject<T>(obj, type);
	}


    #endregion
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal Type ReadTypeFromHeader<T>()
	{
		// Check whether we need to determine the type by reading the header.
		if(typeof(T) == typeof(object))
			return ReadKeyPrefix();
		else if(settings.typeChecking)
		{
			Type type = ReadKeyPrefix();
			if(type != typeof(T))
				throw new InvalidOperationException("Trying to load data of type "+typeof(T)+", but data contained in file is type of "+type+".");
			return type;
		}
		else
		{
			ReadKeyPrefix(true);
			return typeof(T);
		}
	}

	/// <summary>Creates a new ES3Reader and loads the default file into it.</summary>
	public static ES3Reader Create()
	{
		return Create(new ES3Settings());
	}

	/// <summary>Creates a new ES3Reader and loads a file in storage into it.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load into the reader.</param>
	public static ES3Reader Create(string filePath)
	{
		return Create(new ES3Settings(filePath));
	}

	/// <summary>Creates a new ES3Reader and loads a file in storage into it.</summary>
	/// <param name="filePath">The relative or absolute path of the file we want to load into the reader.</param>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static ES3Reader Create(string filePath, ES3Settings settings)
	{
		return Create(new ES3Settings(filePath, settings));
	}

	/// <summary>Creates a new ES3Reader and loads a file in storage into it.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static ES3Reader Create(ES3Settings settings)
	{
		Stream stream = ES3Stream.CreateStream(settings, ES3FileMode.Read);
		if(stream == null)
			return null;

        // Get the baseWriter using the given Stream.
        if (settings.format == ES3.Format.JSON)
            return new ES3JSONReader(stream, settings);
		return null;
	}

	/// <summary>Creates a new ES3Reader and loads the bytes provided into it.</summary>
	public static ES3Reader Create(byte[] bytes)
	{
		return Create(bytes, new ES3Settings());
	}

	/// <summary>Creates a new ES3Reader and loads the bytes provided into it.</summary>
	/// <param name="settings">The settings we want to use to override the default settings.</param>
	public static ES3Reader Create(byte[] bytes, ES3Settings settings)
	{
		Stream stream = ES3Stream.CreateStream(new MemoryStream(bytes), settings, ES3FileMode.Read);
		if(stream == null)
			return null;

		// Get the baseWriter using the given Stream.
		if(settings.format == ES3.Format.JSON)
			return new ES3JSONReader(stream, settings);
        return null;
	}

	internal static ES3Reader Create(Stream stream, ES3Settings settings)
	{
		stream = ES3Stream.CreateStream(stream, settings, ES3FileMode.Read);

		// Get the baseWriter using the given Stream.
		if(settings.format == ES3.Format.JSON)
			return new ES3JSONReader(stream, settings);
        return null;
	}

	internal static ES3Reader Create(Stream stream, ES3Settings settings, bool readHeaderAndFooter)
	{
		// Get the baseWriter using the given Stream.
		if(settings.format == ES3.Format.JSON)
			return new ES3JSONReader(stream, settings, readHeaderAndFooter);
        return null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ES3ReaderPropertyEnumerator
	{
		public ES3Reader reader;

		public ES3ReaderPropertyEnumerator(ES3Reader reader)
		{
			this.reader = reader;
		}

		public IEnumerator GetEnumerator()
		{
			string propertyName;
			while(true)
			{
				// Allows us to repeat a property name or insert one of our own.
				if(reader.overridePropertiesName != null)
				{
					string tempName = reader.overridePropertiesName;
					reader.overridePropertiesName = null;
					yield return tempName;
				}
				else
				{
					if((propertyName = reader.ReadPropertyName()) == null)
						yield break;
					yield return propertyName;
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ES3ReaderRawEnumerator
	{
		public ES3Reader reader;

		public ES3ReaderRawEnumerator(ES3Reader reader)
		{
			this.reader = reader;
		}

		public IEnumerator GetEnumerator()
		{
			while(true)
			{
				string key = reader.ReadPropertyName();
				if(key == null)
					yield break;

                Type type = reader.ReadTypeFromHeader<object>();

				byte[] bytes = reader.ReadElement();

                reader.ReadKeySuffix();

                if(type != null)
				    yield return new KeyValuePair<string,ES3Data>(key, new ES3Data(type, bytes));
			}
		}
	}
}