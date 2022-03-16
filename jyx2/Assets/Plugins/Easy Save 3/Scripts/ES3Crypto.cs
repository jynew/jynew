#if !DISABLE_ENCRYPTION
using System.IO;
using System.Security.Cryptography;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace ES3Internal
{
	public static class ES3Hash
	{
#if NETFX_CORE
		public static string SHA1Hash(string input)
		{
			return System.Text.Encoding.UTF8.GetString(UnityEngine.Windows.Crypto.ComputeSHA1Hash(System.Text.Encoding.UTF8.GetBytes(input)));
		}
#else
		public static string SHA1Hash(string input)
		{
			using (SHA1Managed sha1 = new SHA1Managed())
				return System.Text.Encoding.UTF8.GetString(sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input)));
		}
#endif
	}

    public abstract class EncryptionAlgorithm
    {
        public abstract byte[] Encrypt(byte[] bytes, string password, int bufferSize);
        public abstract byte[] Decrypt(byte[] bytes, string password, int bufferSize);
        public abstract void Encrypt(Stream input, Stream output, string password, int bufferSize);
        public abstract void Decrypt(Stream input, Stream output, string password, int bufferSize);

        protected static void CopyStream(Stream input, Stream output, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = input.Read(buffer, 0, bufferSize)) > 0)
                output.Write(buffer, 0, read);
        }
    }

    public class AESEncryptionAlgorithm : EncryptionAlgorithm
    {
        private const int ivSize = 16;
        private const int keySize = 16;
        private const int pwIterations = 100;

        public override byte[] Encrypt(byte[] bytes, string password, int bufferSize)
        {
            using (var input = new MemoryStream(bytes))
            {
                using (var output = new MemoryStream())
                {
                    Encrypt(input, output, password, bufferSize);
                    return output.ToArray();
                }
            }
        }

        public override byte[] Decrypt(byte[] bytes, string password, int bufferSize)
        {
            using (var input = new MemoryStream(bytes))
            {
                using (var output = new MemoryStream())
                {
                    Decrypt(input, output, password, bufferSize);
                    return output.ToArray();
                }
            }
        }

        public override void Encrypt(Stream input, Stream output, string password, int bufferSize)
        {
            input.Position = 0;

#if NETFX_CORE
            // Generate an IV and write it to the output.
            var iv = CryptographicBuffer.GenerateRandom(ivSize);
            output.Write(iv.ToArray(), 0, ivSize);

            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(iv, pwIterations);
            // Create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, keySize);

            var provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var key = provider.CreateSymmetricKey(keyMaterial);

            // Get the input stream as an IBuffer.
            IBuffer msg;
            using(var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                msg = ms.ToArray().AsBuffer();
            }

            var buffEncrypt = CryptographicEngine.Encrypt(key, msg, iv);


            output.Write(buffEncrypt.ToArray(), 0, (int)buffEncrypt.Length);
            output.Dispose();
#else
            using (var alg = Aes.Create())
			{
                alg.Mode = CipherMode.CBC;
                alg.Padding = PaddingMode.PKCS7;
                alg.GenerateIV();
                var key = new Rfc2898DeriveBytes(password, alg.IV, pwIterations);
				alg.Key = key.GetBytes(keySize);
				// Write the IV to the output stream.
				output.Write(alg.IV, 0, ivSize);
				using(var encryptor = alg.CreateEncryptor())
				using(var cs = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
					CopyStream(input, cs, bufferSize);
			}
#endif
        }

        public override void Decrypt(Stream input, Stream output, string password, int bufferSize)
        {
#if NETFX_CORE
            var thisIV = new byte[ivSize];
            input.Read(thisIV, 0, ivSize);
            var iv = thisIV.AsBuffer();

            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);

            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(iv, pwIterations);
            // Create a key based on original key and derivation parameters.
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, keySize);
            
            var provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var key = provider.CreateSymmetricKey(keyMaterial);

            // Get the input stream as an IBuffer.
            IBuffer msg;
            using(var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                msg = ms.ToArray().AsBuffer();
            }

            var buffDecrypt = CryptographicEngine.Decrypt(key, msg, iv);

            output.Write(buffDecrypt.ToArray(), 0, (int)buffDecrypt.Length);
#else
            using (var alg = Aes.Create())
			{
                var thisIV = new byte[ivSize];
                input.Read(thisIV, 0, ivSize);
                alg.IV = thisIV;

                var key = new Rfc2898DeriveBytes(password, alg.IV, pwIterations);
				alg.Key = key.GetBytes(keySize);

				using(var decryptor = alg.CreateDecryptor())
				using(var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
					CopyStream(cryptoStream, output, bufferSize);

			}
#endif
            output.Position = 0;
        }
    }

    public class UnbufferedCryptoStream : MemoryStream
    {
        private readonly Stream stream;
        private readonly bool isReadStream;
        private string password;
        private int bufferSize;
        private EncryptionAlgorithm alg;
        private bool disposed = false;

        public UnbufferedCryptoStream(Stream stream, bool isReadStream, string password, int bufferSize, EncryptionAlgorithm alg) : base()
        {
            this.stream = stream;
            this.isReadStream = isReadStream;
            this.password = password;
            this.bufferSize = bufferSize;
            this.alg = alg;


            if (isReadStream)
                alg.Decrypt(stream, this, password, bufferSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;

            if (!isReadStream)
                alg.Encrypt(this, stream, password, bufferSize);
            stream.Dispose();
            base.Dispose(disposing);
        }
    }
}
#endif