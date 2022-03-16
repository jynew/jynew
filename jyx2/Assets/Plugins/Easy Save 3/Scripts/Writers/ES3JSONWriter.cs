using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using System.Globalization;

namespace ES3Internal
{
	internal class ES3JSONWriter : ES3Writer
	{
		internal StreamWriter baseWriter;

		private bool isFirstProperty = true;

		public ES3JSONWriter(Stream stream, ES3Settings settings) : this(stream, settings, true, true){}

		internal ES3JSONWriter(Stream stream, ES3Settings settings, bool writeHeaderAndFooter, bool mergeKeys) : base(settings, writeHeaderAndFooter, mergeKeys)
		{
			baseWriter = new StreamWriter(stream);
			StartWriteFile();
		}

		#region WritePrimitive(value) methods.

		internal override void WritePrimitive(int value)		{ baseWriter.Write(value); }
		internal override void WritePrimitive(float value)	{ baseWriter.Write(value.ToString("R", CultureInfo.InvariantCulture)); }
		internal override void WritePrimitive(bool value)		{ baseWriter.Write(value ? "true" : "false"); }
		internal override void WritePrimitive(decimal value)	{ baseWriter.Write(value.ToString(CultureInfo.InvariantCulture)); }
		internal override void WritePrimitive(double value)	{ baseWriter.Write(value.ToString("R", CultureInfo.InvariantCulture)); }
		internal override void WritePrimitive(long value)		{ baseWriter.Write(value); }
		internal override void WritePrimitive(ulong value)	{ baseWriter.Write(value); }
		internal override void WritePrimitive(uint value)		{ baseWriter.Write(value); }
		internal override void WritePrimitive(byte value)		{ baseWriter.Write(System.Convert.ToInt32(value)); }
		internal override void WritePrimitive(sbyte value)	{ baseWriter.Write(System.Convert.ToInt32(value)); }
		internal override void WritePrimitive(short value)	{ baseWriter.Write(System.Convert.ToInt32(value)); }
		internal override void WritePrimitive(ushort value)	{ baseWriter.Write(System.Convert.ToInt32(value)); }
		internal override void WritePrimitive(char value)		{ WritePrimitive( value.ToString() ); }
		internal override void WritePrimitive(byte[] value)		{ WritePrimitive( System.Convert.ToBase64String(value) ); }


		internal override void WritePrimitive(string value)
		{ 
			baseWriter.Write("\"");

			// Escape any quotation marks within the string.
			for(int i = 0; i<value.Length; i++)
			{
				char c = value[i];
				switch(c)
				{
					case '\"':
					case '“':
					case '”':
					case '\\':
					case '/':
						baseWriter.Write('\\');
						baseWriter.Write(c);
						break;
					case '\b':
						baseWriter.Write("\\b");
						break;
					case '\f':
						baseWriter.Write("\\f");
						break;
					case '\n':
						baseWriter.Write("\\n");
						break;
					case '\r':
						baseWriter.Write("\\r");
						break;
					case '\t':
						baseWriter.Write("\\t");
						break;
					default:
						baseWriter.Write(c);
						break;
				}
			}
			baseWriter.Write("\"");
		}

		internal override void WriteNull()
		{
			baseWriter.Write("null");
		}

		#endregion

		#region Format-specific methods

		private static bool CharacterRequiresEscaping(char c)
		{
			return c == '\"' || c == '\\' || c == '“' || c == '”';
		}

		private void WriteCommaIfRequired()
		{
            if (!isFirstProperty)
                baseWriter.Write(',');
            else
                isFirstProperty = false;
            WriteNewlineAndTabs();
        }

		internal override void WriteRawProperty(string name, byte[] value)
		{ 
			StartWriteProperty(name); baseWriter.Write(settings.encoding.GetString(value, 0, value.Length)); EndWriteProperty(name);
		}

		internal override void StartWriteFile()
		{
            if (writeHeaderAndFooter)
                baseWriter.Write('{');
            base.StartWriteFile();
		}

		internal override void EndWriteFile()
		{
            base.EndWriteFile();
            WriteNewlineAndTabs();
			if(writeHeaderAndFooter)
				baseWriter.Write('}');
		}

		internal override void StartWriteProperty(string name)
		{
            base.StartWriteProperty(name);
			WriteCommaIfRequired();
			Write(name);

            if(settings.prettyPrint)
                baseWriter.Write(' ');
            baseWriter.Write(':');
            if (settings.prettyPrint)
                baseWriter.Write(' ');
        }

		internal override void EndWriteProperty(string name)
		{
            // It's not necessary to perform any operations after writing the property in JSON.
            base.EndWriteProperty(name);
        }

		internal override void StartWriteObject(string name)
		{
            base.StartWriteObject(name);
            isFirstProperty = true;
			baseWriter.Write('{');
        }

		internal override void EndWriteObject(string name)
		{
            base.EndWriteObject(name);
            // Set isFirstProperty to false incase we didn't write any properties, in which case
            // WriteCommaIfRequired() is never called.
            isFirstProperty = false;
            WriteNewlineAndTabs();
            baseWriter.Write('}');
        }

		internal override void StartWriteCollection()
		{
            base.StartWriteCollection();
            baseWriter.Write('[');
            WriteNewlineAndTabs();
        }

		internal override void EndWriteCollection()
		{
            base.EndWriteCollection();
            WriteNewlineAndTabs();
            baseWriter.Write(']');
		}

		internal override void StartWriteCollectionItem(int index)
		{
			if(index != 0)
				baseWriter.Write(',');
		}

		internal override void EndWriteCollectionItem(int index)
		{
		}

		internal override void StartWriteDictionary()
		{
			StartWriteObject(null);
		}

		internal override void EndWriteDictionary()
		{
			EndWriteObject(null);
		}

		internal override void StartWriteDictionaryKey(int index)
		{
			if(index != 0)
				baseWriter.Write(',');
		}

		internal override void EndWriteDictionaryKey(int index)
		{
			baseWriter.Write(':');
		}

		internal override void StartWriteDictionaryValue(int index)
		{
		}

		internal override void EndWriteDictionaryValue(int index)
		{
		}

		#endregion

		public override void Dispose()
		{
			baseWriter.Dispose();
		}

        public void WriteNewlineAndTabs()
        {
            if (settings.prettyPrint)
            {
                baseWriter.Write(Environment.NewLine);
                for (int i = 0; i < serializationDepth; i++)
                    baseWriter.Write('\t');
            }
        }
	}
}
