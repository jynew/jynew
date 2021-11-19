using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("filterMode", "anisoLevel", "wrapMode", "mipMapBias", "rawTextureData")]
	public class ES3Type_Texture : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Texture() : base(typeof(UnityEngine.Texture)){ Instance = this; }

		public override void Write(object obj, ES3Writer writer)
		{
			if (obj.GetType () == typeof(Texture2D)) 
				ES3Type_Texture2D.Instance.Write(obj, writer);
			else
				throw new NotSupportedException ("Textures of type "+obj.GetType()+" are not currently supported.");
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
			if (obj.GetType () == typeof(Texture2D)) 
				ES3Type_Texture2D.Instance.ReadInto<T>(reader, obj);
			else
				throw new NotSupportedException ("Textures of type "+obj.GetType()+" are not currently supported.");
		}

		public override object Read<T>(ES3Reader reader)
		{
			return ES3Type_Texture2D.Instance.Read<T>(reader);
		}
	}

	public class ES3Type_TextureArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_TextureArray() : base(typeof(UnityEngine.Texture[]), ES3Type_Texture.Instance)
		{
			Instance = this;
		}
	}
}