using System.IO;
using UnityEngine;

namespace ES3Internal
{
	internal class ES3ResourcesStream : MemoryStream
	{
		// Check that data exists by checking stream is not empty.
		public bool Exists{ get{ return this.Length > 0; } }

		// Used when creating 
		public ES3ResourcesStream(string path) : base(GetData(path))
		{
		}

		private static byte[] GetData(string path)
		{
			var textAsset = Resources.Load(path) as TextAsset;

			// If data doesn't exist in Resources, return an empty byte array.
			if(textAsset == null)
				return new byte[0];
			
			return textAsset.bytes;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
