using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using System.Globalization;

namespace ES3Internal
{
	internal class ES3CacheWriter : ES3Writer
	{
        ES3File es3File;

		internal ES3CacheWriter(ES3Settings settings, bool writeHeaderAndFooter, bool mergeKeys) : base(settings, writeHeaderAndFooter, mergeKeys)
		{
            es3File = new ES3File(settings);
		}

        /* User-facing methods used when writing randomly-accessible Key-Value pairs. */
        #region Write(key, value) Methods

        /// <summary>Writes a value to the writer with the given key.</summary>
        /// <param name="key">The key which uniquely identifies this value.</param>
        /// <param name="value">The value we want to write.</param>
        public override void Write<T>(string key, object value)
        {
            es3File.Save<T>(key, (T)value);
        }

        internal override void Write(string key, Type type, byte[] value)
        {
            ES3Debug.LogError("Not implemented");
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void Write(Type type, string key, object value)
        {
            es3File.Save(key, value);
        }

        #endregion


        #region WritePrimitive(value) methods.

        internal override void WritePrimitive(int value)		{ }
		internal override void WritePrimitive(float value)	{ }
		internal override void WritePrimitive(bool value)		{ }
		internal override void WritePrimitive(decimal value)	{ }
		internal override void WritePrimitive(double value)	{ }
		internal override void WritePrimitive(long value)		{ }
		internal override void WritePrimitive(ulong value)	{ }
		internal override void WritePrimitive(uint value)		{ }
		internal override void WritePrimitive(byte value)		{ }
		internal override void WritePrimitive(sbyte value)	{ }
		internal override void WritePrimitive(short value)	{ }
		internal override void WritePrimitive(ushort value)	{ }
		internal override void WritePrimitive(char value)		{ }
		internal override void WritePrimitive(byte[] value)		{ }


		internal override void WritePrimitive(string value)
		{ 
		}

		internal override void WriteNull()
		{
		}

		#endregion

		#region Format-specific methods

		private static bool CharacterRequiresEscaping(char c)
		{
            return false;
		}

		private void WriteCommaIfRequired()
		{
		}

		internal override void WriteRawProperty(string name, byte[] value)
		{ 
		}

		internal override void StartWriteFile()
		{
		}

		internal override void EndWriteFile()
		{
		}

		internal override void StartWriteProperty(string name)
		{
            base.StartWriteProperty(name);
		}

		internal override void EndWriteProperty(string name)
		{
		}

		internal override void StartWriteObject(string name)
		{
		}

		internal override void EndWriteObject(string name)
		{
        }

		internal override void StartWriteCollection()
		{
		}

		internal override void EndWriteCollection()
		{
		}

		internal override void StartWriteCollectionItem(int index)
		{
		}

		internal override void EndWriteCollectionItem(int index)
		{
		}

		internal override void StartWriteDictionary()
		{
		}

		internal override void EndWriteDictionary()
		{
		}

		internal override void StartWriteDictionaryKey(int index)
		{
		}

		internal override void EndWriteDictionaryKey(int index)
		{
		}

		internal override void StartWriteDictionaryValue(int index)
		{
		}

		internal override void EndWriteDictionaryValue(int index)
		{
		}

		#endregion

		public override void Dispose(){}
	}
}
