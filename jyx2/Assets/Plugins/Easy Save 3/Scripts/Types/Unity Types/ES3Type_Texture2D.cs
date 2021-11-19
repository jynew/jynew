using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("filterMode", "anisoLevel", "wrapMode", "mipMapBias", "rawTextureData")]
	public class ES3Type_Texture2D : ES3UnityObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_Texture2D() : base(typeof(Texture2D)){ Instance = this; }

        protected override void WriteUnityObject(object obj, ES3Writer writer)
        {
            var instance = (Texture2D)obj;

            if (!IsReadable(instance))
            {
                ES3Internal.ES3Debug.LogWarning("Easy Save cannot save the pixels or properties of this Texture because it is not read/write enabled, so Easy Save will store it by reference instead. To save the pixel data, check the 'Read/Write Enabled' checkbox in the Texture's import settings. Clicking this warning will take you to the Texture, assuming it is not generated at runtime.", instance);
                return;
            }

            writer.WriteProperty("width", instance.width, ES3Type_int.Instance);
			writer.WriteProperty("height", instance.height, ES3Type_int.Instance);
			writer.WriteProperty("format", instance.format);
			writer.WriteProperty("mipmapCount", instance.mipmapCount, ES3Type_int.Instance);
			writer.WriteProperty("filterMode", instance.filterMode);
			writer.WriteProperty("anisoLevel", instance.anisoLevel, ES3Type_int.Instance);
			writer.WriteProperty("wrapMode", instance.wrapMode);
			writer.WriteProperty("mipMapBias", instance.mipMapBias, ES3Type_float.Instance);
            writer.WriteProperty("rawTextureData", instance.GetRawTextureData(), ES3Type_byteArray.Instance);
		}

		protected override void ReadUnityObject<T>(ES3Reader reader, object obj)
		{
            if (obj == null)
                return;

            if (obj.GetType() == typeof(RenderTexture))
            {
                ES3Type_RenderTexture.Instance.ReadInto<T>(reader, obj);
                return;
            }

			var instance = (Texture2D)obj;

            if (!IsReadable(instance))
                ES3Internal.ES3Debug.LogWarning("Easy Save cannot load the properties or pixels for this Texture because it is not read/write enabled, so it will be loaded by reference. To load the properties and pixels for this Texture, check the 'Read/Write Enabled' checkbox in its Import Settings.", instance);

            foreach (string propertyName in reader.Properties)
			{
                // If this Texture isn't readable, we should skip past all of its properties.
                if (!IsReadable(instance))
                {
                    reader.Skip();
                    continue;
                }

                switch (propertyName)
				{
					case "filterMode":
						instance.filterMode = reader.Read<UnityEngine.FilterMode>();
						break;
					case "anisoLevel":
						instance.anisoLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "wrapMode":
						instance.wrapMode = reader.Read<UnityEngine.TextureWrapMode>();
						break;
					case "mipMapBias":
						instance.mipMapBias = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "rawTextureData":
                        // LoadRawTextureData requires that the correct width, height, TextureFormat and mipMaps are set before being called.
                        // If an error occurs here, it's likely that we're using LoadInto to load into a Texture which differs in these values.
                        // In this case, LoadInto should be avoided and Load should be used instead.
                        if (!IsReadable(instance))
                        {
                            ES3Internal.ES3Debug.LogWarning("Easy Save cannot load the pixels of this Texture because it is not read/write enabled, so Easy Save will ignore the pixel data. To load the pixel data, check the 'Read/Write Enabled' checkbox in the Texture's import settings. Clicking this warning will take you to the Texture, assuming it is not generated at runtime.", instance);
                            reader.Skip();
                        }
                        else
                        {
                            try
                            {
                                instance.LoadRawTextureData(reader.Read<byte[]>(ES3Type_byteArray.Instance));
                                instance.Apply();
                            }
                            catch(Exception e)
                            {
                                ES3Internal.ES3Debug.LogError("Easy Save encountered an error when trying to load this Texture, please see the end of this messasge for the error. This is most likely because the Texture format of the instance we are loading into is different to the Texture we saved.\n"+e.ToString(), instance);
                            }
                        }
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

        protected override object ReadUnityObject<T>(ES3Reader reader)
		{
			var instance = new Texture2D(	reader.Read<int>(ES3Type_int.Instance), // Property name has already been read in ES3UnityObjectType, so we only need to read the value.
											reader.ReadProperty<int>(ES3Type_int.Instance),
											reader.ReadProperty<TextureFormat>(), 
											(reader.ReadProperty<int>(ES3Type_int.Instance) > 1));
			ReadObject<T>(reader, instance);
			return instance;
		}

        protected bool IsReadable(Texture2D instance)
        {
            #if UNITY_2018_3_OR_NEWER
            return instance != null && instance.isReadable;
            #else
            return true;
            #endif
        }
    }

	public class ES3Type_Texture2DArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_Texture2DArray() : base(typeof(Texture2D[]), ES3Type_Texture2D.Instance)
		{
			Instance = this;
		}
	}
}