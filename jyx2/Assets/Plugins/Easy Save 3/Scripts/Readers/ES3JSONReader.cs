using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using ES3Types;
using System.Globalization;

namespace ES3Internal
{
	/*
	 * 	Specific ES3Reader for reading JSON data.
	 * 
	 * 	Note: 	Leading & trailing whitespace is ignored whenever 
	 * 			reading characters which are part of the JSON syntax,
	 * 			i.e. { } [ ] , " " :
	 */
	public class ES3JSONReader : ES3Reader
	{
		private const char endOfStreamChar = (char)65535;

		public StreamReader baseReader;

		internal ES3JSONReader(Stream stream, ES3Settings settings, bool readHeaderAndFooter = true) : base(settings, readHeaderAndFooter)
		{
			this.baseReader = new StreamReader(stream);

			// Read opening brace from file if we're loading straight from file.
			if(readHeaderAndFooter)
			{
				try
				{
					SkipOpeningBraceOfFile();
				}
				catch
				{
					this.Dispose();
					throw new FormatException("Cannot load from file because the data in it is not JSON data, or the data is encrypted.\nIf the save data is encrypted, please ensure that encryption is enabled when you load, and that you are using the same password used to encrypt the data.");
				}
			}
		}

		#region Property/Key Methods

		/*
		 * 	Reads the name of a property, and must be positioned (with or without whitespace) either:
		 * 		- Before the '"' of a property name.
		 * 		- Before the ',' separating properties.
		 * 		- Before the '}' or ']' terminating this list of properties.
		 * 	Can be used in conjunction with Read(ES3Type) to read a property.
		 */
		public override string ReadPropertyName()
		{
			char c = PeekCharIgnoreWhitespace();

			// Check whether there are any properties left to read.
			if(IsTerminator(c))
				return null;
			else if(c == ',')
				ReadCharIgnoreWhitespace();
			else if(!IsQuotationMark(c))
				throw new FormatException("Expected ',' separating properties or '\"' before property name, found '"+c+"'.");

			var propertyName = Read_string();
			if(propertyName == null)
				throw new FormatException("Stream isn't positioned before a property.");

            ES3Debug.Log("<b>"+propertyName+"</b> (reading property)", null, serializationDepth);

			// Skip the ':' seperating property and value.
			ReadCharIgnoreWhitespace(':');

			return propertyName;
		}

		/*
		 * 	Reads the type data prefixed to this key.
		 * 	If ignore is true, it will return null to save the computation of converting
		 * 	the string to a Type.
		 */
		protected override Type ReadKeyPrefix(bool ignoreType=false)
		{
			StartReadObject();

			Type dataType = null;

			string propertyName = ReadPropertyName();
			if(propertyName == ES3Type.typeFieldName)
			{
				string typeString = Read_string();
                dataType = ignoreType ? null : ES3Reflection.GetType(typeString);
				propertyName  = ReadPropertyName();
			}
				
			if(propertyName != "value")
				throw new FormatException("This data is not Easy Save Key Value data. Expected property name \"value\", found \""+propertyName+"\".");

			return dataType;
		}

		protected override void ReadKeySuffix()
		{
			EndReadObject();
		}


		internal override bool StartReadObject()
		{
            base.StartReadObject();
			return ReadNullOrCharIgnoreWhitespace('{');
		}

		internal override void EndReadObject()
		{
			ReadCharIgnoreWhitespace('}');
            base.EndReadObject();
        }


		internal override bool StartReadDictionary()
		{
			return StartReadObject();
		}

		internal override void EndReadDictionary(){}

		internal override bool StartReadDictionaryKey()
		{
			// If this is an empty Dictionary, return false.
			if(PeekCharIgnoreWhitespace() == '}')
			{
				ReadCharIgnoreWhitespace();
				return false;
			}
			return true;
		}

		internal override void EndReadDictionaryKey()
		{
			ReadCharIgnoreWhitespace(':');
		}

		internal override void StartReadDictionaryValue(){}

		internal override bool EndReadDictionaryValue()
		{
			char c = ReadCharIgnoreWhitespace();
			// If we find a ']', we reached the end of the array.
			if(c == '}')
				return true;
			// Else, we should expect a comma.
			else if(c != ',')
				throw new FormatException("Expected ',' seperating Dictionary items or '}' terminating Dictionary, found '"+c+"'.");
			return false;
		}


		internal override bool StartReadCollection()
		{
			return ReadNullOrCharIgnoreWhitespace('[');
		}

		internal override void EndReadCollection(){}

		internal override bool StartReadCollectionItem()
		{
			// If this is an empty collection, return false.
			if(PeekCharIgnoreWhitespace() == ']')
			{
				ReadCharIgnoreWhitespace();
				return false;
			}
			return true;
		}

		internal override bool EndReadCollectionItem()
		{
			char c = ReadCharIgnoreWhitespace();
			// If we find a ']', we reached the end of the array.
			if(c == ']')
				return true;
			// Else, we should expect a comma.
			else if(c != ',')
				throw new FormatException("Expected ',' seperating collection items or ']' terminating collection, found '"+c+"'.");
			return false;
		}

		#endregion

		#region Seeking Methods

		/* 
		 * 	Reads a string value into a StreamWriter.
		 * 	Reader should be positioned after the opening quotation mark.
		 * 	Will also read the closing quotation mark.
		 * 	If the 'skip' parameter is true, data will not be written into a StreamWriter and will return null.
		 */
		private void ReadString(StreamWriter writer, bool skip=false)
		{
			bool endOfString = false;
			// Read to end of string, or throw error if we reach end of stream.
			while(!endOfString)
			{
				char c = ReadOrSkipChar(writer, skip);
				switch(c)
				{
					case endOfStreamChar:
						throw new FormatException("String without closing quotation mark detected.");
					case '\\':
						ReadOrSkipChar(writer, skip);
						break;
					default:
						if(IsQuotationMark(c))
							endOfString = true;
						break;
				}
			}
		}

		/*
		 * 	Reads the current object in the stream.
		 * 	Stream position should be somewhere before the opening brace for the object.
		 * 	When this method successfully exits, it will be on the closing brace for the object.
		 * 	If the 'skip' parameter is true, data will not be written into a StreamWriter and will return null.
		 */
		internal override byte[] ReadElement(bool skip=false)
		{
			// If 'skip' is enabled, don't create a stream or writer as we'll discard all bytes we read.
			StreamWriter writer = skip ? null : new StreamWriter(new MemoryStream(settings.bufferSize));

			using(writer)
			{
				int nesting = 0;
				char c = (char)baseReader.Peek();

				// Determine if we're skipping a primitive type.
				// First check if it's an opening object or array brace.
				if(!IsOpeningBrace(c))
				{
					// If we're skipping a string, use SkipString().
					if(c == '\"')
					{
						// Skip initial quotation mark as SkipString() requires this.
						ReadOrSkipChar(writer, skip);
						ReadString(writer, skip);
					}
					// Else we just need to read until we reach a closing brace.
					else
						// While we've not peeked a closing brace.
						while(!IsEndOfValue((char)baseReader.Peek()))
							ReadOrSkipChar(writer, skip);

					if(skip)
						return null;
					writer.Flush();
					return ((MemoryStream)writer.BaseStream).ToArray();
				}

				// Else, we're skipping a type surrounded by braces.
				// Iterate through every character, logging nesting.
				while(true)
				{
					c = ReadOrSkipChar(writer, skip);

					if(c == endOfStreamChar) // Detect premature end of stream, which denotes missing closing brace.
						throw new FormatException("Missing closing brace detected, as end of stream was reached before finding it.");

					// Handle quoted strings.
					// According to the RFC, only '\' and '"' must be escaped in strings.
					if(IsQuotationMark(c))
					{
						ReadString(writer, skip);
						continue;
					}

					// Handle braces and other characters.
					switch(c)
					{
						case '{': // Entered another level of nesting.
						case '[': 
							nesting++;
							break;
                        case '}': // Exited a level of nesting.
						case ']':
							nesting--;
							// If nesting < 1, we've come to the end of the object.
							if(nesting<1)
							{
								if(skip)
									return null;
								writer.Flush();
								return ((MemoryStream)writer.BaseStream).ToArray();
							}
							break;
						default:
							break;
					}
				}
			}
		}

		/*
		 * 	Reads the next char into a stream, or ignores it if 'skip' is true.
		 */
		private char ReadOrSkipChar(StreamWriter writer, bool skip)
		{
			char c = (char)baseReader.Read();
			if(!skip) writer.Write(c);
			return c;
		}

		#endregion

		#region JSON-specific methods.

		/*
		 * 	Reads a char from the stream and ignores leading and trailing whitespace.
		 */
		private char ReadCharIgnoreWhitespace(bool ignoreTrailingWhitespace=true)
		{
			char c;
			// Skip leading whitespace and read char.
			while(IsWhiteSpace(c = (char)baseReader.Read()))
			{}

			// Skip trailing whitespace.
            if(ignoreTrailingWhitespace)
			    while(IsWhiteSpace((char)baseReader.Peek()))
				    baseReader.Read();

			return c;
		}

		/*
		 * 	Reads a char, or the NULL value, from the stream and ignores leading and trailing whitespace.
		 * 	Returns true if NULL was read.
		 */
		private bool ReadNullOrCharIgnoreWhitespace(char expectedChar)
		{
			char c = ReadCharIgnoreWhitespace();

			// Check for null
			if(c == 'n')
			{
				var chars = new char[3];
				baseReader.ReadBlock(chars, 0, 3);
				if((char)chars[0] == 'u' && (char)chars[1] == 'l' && (char)chars[2] == 'l')
					return true;
			}

			if(c != expectedChar)
			{
				if(c == endOfStreamChar)
					throw new FormatException("End of stream reached when expecting '"+expectedChar+"'.");
				else
					throw new FormatException("Expected \'"+expectedChar+"\' or \"null\", found \'"+c+"\'.");
			}
			return false;
		}

		/*
		 * 	Reads a char from the stream and ignores leading and trailing whitespace.
		 * 	Throws an error if the char isn't equal to the one specificed as a parameter, or if it's the end of stream.
		 */
		private char ReadCharIgnoreWhitespace(char expectedChar)
		{
			char c = ReadCharIgnoreWhitespace();
			if(c != expectedChar)
			{
				if(c == endOfStreamChar)
					throw new FormatException("End of stream reached when expecting '"+expectedChar+"'.");
				else
					throw new FormatException("Expected \'"+expectedChar+"\', found \'"+c+"\'.");
			}
			return c;
		}

		private bool ReadQuotationMarkOrNullIgnoreWhitespace()
		{
			char c = ReadCharIgnoreWhitespace(false); // Don't read trailing whitespace as this is the value.

			if(c == 'n')
			{
				var chars = new char[3];
				baseReader.ReadBlock(chars, 0, 3);
				if((char)chars[0] == 'u' && (char)chars[1] == 'l' && (char)chars[2] == 'l')
					return true;
			}
			else if(!IsQuotationMark(c))
			{
				if(c == endOfStreamChar)
					throw new FormatException("End of stream reached when expecting quotation mark.");
				else
					throw new FormatException("Expected quotation mark, found \'"+c+"\'.");
			}
			return false;
		}

		/*
		 * 	Peeks the next char in the stream, ignoring leading whitespace, but not trailing whitespace.
		 */
		private char PeekCharIgnoreWhitespace(char expectedChar)
		{
			char c = PeekCharIgnoreWhitespace();
			if(c != expectedChar)
			{
				if(c == endOfStreamChar)
					throw new FormatException("End of stream reached while peeking, when expecting '"+expectedChar+"'.");
				else
					throw new FormatException("Expected \'"+expectedChar+"\', found \'"+c+"\'.");
			}
			return c;
		}

		/*
		 * 	Peeks the next char in the stream, ignoring leading whitespace, but not trailing whitespace.
		 *	Throws an error if the char isn't equal to the one specificed as a parameter.
		 */
		private char PeekCharIgnoreWhitespace()
		{
			char c;
			// Skip leading whitespace and read char.
			while(IsWhiteSpace(c = (char)baseReader.Peek()))
				baseReader.Read();
			return c;
		}

		// Skips all whitespace immediately after the current position.
		private void SkipWhiteSpace()
		{
			while(IsWhiteSpace((char)baseReader.Peek()))
				baseReader.Read();
		}

		private void SkipOpeningBraceOfFile()
		{
			// Skip the whitespace and '{' at the beginning of the JSON file.
			char firstChar = ReadCharIgnoreWhitespace();
			if(firstChar != '{') // If first char isn't '{', it's not valid JSON.
				throw new FormatException("File is not valid JSON. Expected '{' at beginning of file, but found '"+firstChar+"'.");
		}

		private static bool IsWhiteSpace(char c)
		{
			return (c == ' ' || c == '\t' || c == '\n' || c == '\r');
		}

		private static bool IsOpeningBrace(char c)
		{
			return (c == '{' || c == '[');
		}

		private static bool IsEndOfValue(char c)
		{
			return (c == '}' || c == ' ' || c == '\t' || c == ']' || c == ',' || c== ':' || c == endOfStreamChar || c == '\n' || c == '\r');
		}

		private static bool IsTerminator(char c)
		{
			return (c == '}' || c == ']');
		}

		private static bool IsQuotationMark(char c)
		{
			return c == '\"' || c == '“' || c == '”';
		}

		private static bool IsEndOfStream(char c)
		{
			return c == endOfStreamChar;
		}

		/*
		 * 	Reads a value (i.e. non-string, non-object) from the stream as a string.
		 * 	Used mostly in Read_[type]() methods.
		 */
		private string GetValueString()
		{
			StringBuilder builder = new StringBuilder();

			while(!IsEndOfValue(PeekCharIgnoreWhitespace()))
				builder.Append((char)baseReader.Read());

			// If it's an empty value, return null.
			if(builder.Length == 0)
				return null;
			return builder.ToString();
		}

		#endregion

		#region Primitive Read() Methods.

		internal override string Read_string()
		{
			if(ReadQuotationMarkOrNullIgnoreWhitespace())
				return null;
			char c;

			StringBuilder sb = new StringBuilder();

			while(!IsQuotationMark((c = (char)baseReader.Read())))
			{
				// If escape mark is found, generate correct escaped character.
				if(c == '\\')
				{
					c = (char)baseReader.Read();
					if(IsEndOfStream(c))
						throw new FormatException("Reached end of stream while trying to read string literal.");

					switch(c)
					{
						case 'b':
							c = '\b';
							break;
						case 'f':
							c = '\f';
							break;
						case 'n':
							c = '\n';
							break;
						case 'r':
							c = '\r';
							break;
						case 't':
							c = '\t';
							break;
						default:
							break;
					}
				}
				sb.Append(c);
			}
			return sb.ToString();
		}

        internal override long Read_ref()
        {
            if (ES3ReferenceMgr.Current == null)
                throw new InvalidOperationException("An Easy Save 3 Manager is required to load references. To add one to your scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene");
            if (IsQuotationMark(PeekCharIgnoreWhitespace()))
                return long.Parse(Read_string());
            return Read_long();
        }

        internal override char		Read_char()		{ return char.Parse(		Read_string()); 	}
		internal override float		Read_float()	{ return float.Parse(		GetValueString(), CultureInfo.InvariantCulture); 	}
		internal override int 		Read_int()		{ return int.Parse(			GetValueString()); 	}
		internal override bool 		Read_bool()		{ return bool.Parse(		GetValueString()); 	}
		internal override decimal 	Read_decimal()	{ return decimal.Parse(		GetValueString(), CultureInfo.InvariantCulture); 	}
		internal override double 	Read_double()	{ return double.Parse(		GetValueString(), CultureInfo.InvariantCulture); 	}
		internal override long 		Read_long()		{ return long.Parse(		GetValueString()); 	}
		internal override ulong 	Read_ulong()	{ return ulong.Parse(		GetValueString()); 	}
		internal override uint 		Read_uint()		{ return uint.Parse(		GetValueString()); 	}
		internal override byte 		Read_byte()		{ return (byte)int.Parse(	GetValueString()); 	}
		internal override sbyte 	Read_sbyte()	{ return (sbyte)int.Parse(	GetValueString()); 	}
		internal override short 	Read_short()	{ return (short)int.Parse(	GetValueString()); 	}
		internal override ushort 	Read_ushort()	{ return (ushort)int.Parse(	GetValueString()); 	}

		internal override byte[] 	Read_byteArray(){ return System.Convert.FromBase64String(Read_string()); }

		#endregion


		public override void Dispose()
		{
			baseReader.Dispose();
		}
	}
}