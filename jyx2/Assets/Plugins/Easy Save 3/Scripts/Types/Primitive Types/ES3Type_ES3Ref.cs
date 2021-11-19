using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3Type_ES3Ref : ES3Type
	{
		public static ES3Type Instance = new ES3Type_ES3Ref();

		public ES3Type_ES3Ref() : base(typeof(long))
		{
			isPrimitive = true;
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			writer.WritePrimitive(((long)obj).ToString());
		}

		public override object Read<T>(ES3Reader reader)
		{
			return (T)(object)new ES3Ref(reader.Read_ref());
		}
	}

	public class ES3Type_ES3RefArray : ES3ArrayType
	{
		public static ES3Type Instance = new ES3Type_ES3RefArray();

		public ES3Type_ES3RefArray() : base(typeof(ES3Ref[]), ES3Type_ES3Ref.Instance)
		{
			Instance = this;
		}
	}

    public class ES3Type_ES3RefDictionary : ES3DictionaryType
    {
        public static ES3Type Instance = new ES3Type_ES3RefDictionary();

        public ES3Type_ES3RefDictionary() : base(typeof(Dictionary<ES3Ref, ES3Ref>), ES3Type_ES3Ref.Instance, ES3Type_ES3Ref.Instance)
        {
            Instance = this;
        }
    }
}

public class ES3Ref
{
    public long id;
    public ES3Ref(long id)
    {
        this.id = id;
    }
}