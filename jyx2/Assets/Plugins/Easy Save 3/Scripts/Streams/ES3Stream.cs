using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace ES3Internal
{
	public static class ES3Stream
	{
		public static Stream CreateStream(ES3Settings settings, ES3FileMode fileMode)
		{
            bool isWriteStream = (fileMode != ES3FileMode.Read);
            Stream stream = null;

            try
            {
                if (settings.location == ES3.Location.InternalMS)
                {
                    // There's no point in creating an empty MemoryStream if we're only reading from it.
                    if (!isWriteStream)
                        return null;
                    stream = new MemoryStream(settings.bufferSize);
                }
                else if (settings.location == ES3.Location.File)
                {
                    if (!isWriteStream && !ES3IO.FileExists(settings.FullPath))
                        return null;
                    stream = new ES3FileStream(settings.FullPath, fileMode, settings.bufferSize, false);
                }
                else if (settings.location == ES3.Location.PlayerPrefs)
                {
                    if (isWriteStream)
                        stream = new ES3PlayerPrefsStream(settings.FullPath, settings.bufferSize, (fileMode == ES3FileMode.Append));
                    else
                    {
                        if (!PlayerPrefs.HasKey(settings.FullPath))
                            return null;
                        stream = new ES3PlayerPrefsStream(settings.FullPath);
                    }
                }
                else if (settings.location == ES3.Location.Resources)
                {
                    if (!isWriteStream)
                    {
                        var resourcesStream = new ES3ResourcesStream(settings.FullPath);
                        if (resourcesStream.Exists)
                            stream = resourcesStream;
                        else
                        {
                            resourcesStream.Dispose();
                            return null;
                        }
                    }
                    else if (UnityEngine.Application.isEditor)
                        throw new System.NotSupportedException("Cannot write directly to Resources folder. Try writing to a directory outside of Resources, and then manually move the file there.");
                    else
                        throw new System.NotSupportedException("Cannot write to Resources folder at runtime. Use a different save location at runtime instead.");
                }

                return CreateStream(stream, settings, fileMode);
            }
            catch(System.Exception e)
            {
                if (stream != null)
                    stream.Dispose();
                throw e;
            }
		}

		public static Stream CreateStream(Stream stream, ES3Settings settings, ES3FileMode fileMode)
		{
            try
            {
                bool isWriteStream = (fileMode != ES3FileMode.Read);

			    #if !DISABLE_ENCRYPTION
                // Encryption
			    if(settings.encryptionType != ES3.EncryptionType.None && stream.GetType() != typeof(UnbufferedCryptoStream))
			    {
				    EncryptionAlgorithm alg = null;
				    if(settings.encryptionType == ES3.EncryptionType.AES)
					    alg = new AESEncryptionAlgorithm();
				    stream = new UnbufferedCryptoStream(stream, !isWriteStream, settings.encryptionPassword, settings.bufferSize, alg);
			    }
                #endif

                // Compression
                if (settings.compressionType != ES3.CompressionType.None && stream.GetType() != typeof(GZipStream))
                {
                    if (settings.compressionType == ES3.CompressionType.Gzip)
                        stream = isWriteStream ? new GZipStream(stream, CompressionMode.Compress) : new GZipStream(stream, CompressionMode.Decompress);
                }

                return stream;
            }
            catch (System.Exception e)
            {
                if (stream != null)
                    stream.Dispose();
                if (e.GetType() == typeof(System.Security.Cryptography.CryptographicException))
                    throw new System.Security.Cryptography.CryptographicException("Could not decrypt file. Please ensure that you are using the same password used to encrypt the file.");
                else
                    throw e;
            }
        }

        public static void CopyTo(Stream source, Stream destination)
        {
        #if UNITY_2019_1_OR_NEWER
            source.CopyTo(destination);
        #else
            byte[] buffer = new byte[2048];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, bytesRead);
        #endif
        }
	}
}
