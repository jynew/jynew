using System.IO;
using UnityEngine;

namespace ES3Internal
{
	internal class ES3PlayerPrefsStream : MemoryStream
	{
		private string path;
		private bool append;
		private bool isWriteStream = false;
		private bool isDisposed = false;

		// This constructor should be used for read streams only.
		public ES3PlayerPrefsStream(string path) : base(GetData(path,false))
		{
			this.path = path;
			this.append = false;
		}

		// This constructor should be used for write streams only.
		public ES3PlayerPrefsStream(string path, int bufferSize, bool append=false) : base(bufferSize)
		{
			this.path = path;
			this.append = append;
			this.isWriteStream = true;
		}

		private static byte[] GetData(string path, bool isWriteStream)
		{
			if(!PlayerPrefs.HasKey(path))
				throw new FileNotFoundException("File \""+path+"\" could not be found in PlayerPrefs");
			return System.Convert.FromBase64String(PlayerPrefs.GetString(path));
		}

		protected override void Dispose (bool disposing)
		{
			if(isDisposed)
				return;
			isDisposed = true;
			if(isWriteStream && this.Length > 0)
			{
	            if (append)
	            {
	                // Convert data back to bytes before appending, as appending Base-64 strings directly can corrupt the data.
	                var sourceBytes = System.Convert.FromBase64String(PlayerPrefs.GetString(path));
	                var appendBytes = this.ToArray();
	                var finalBytes = new byte[sourceBytes.Length + appendBytes.Length];
	                System.Buffer.BlockCopy(sourceBytes, 0, finalBytes, 0, sourceBytes.Length);
	                System.Buffer.BlockCopy(appendBytes, 0, finalBytes, sourceBytes.Length, appendBytes.Length);

					PlayerPrefs.SetString(path, System.Convert.ToBase64String(finalBytes));

					PlayerPrefs.Save();
	            }
	            else
					PlayerPrefs.SetString(path + ES3IO.temporaryFileSuffix, System.Convert.ToBase64String(this.ToArray()));
				// Save the timestamp to a separate key.
				PlayerPrefs.SetString("timestamp_" + path, System.DateTime.UtcNow.Ticks.ToString());
			}
			base.Dispose(disposing);
		}
	}
}
