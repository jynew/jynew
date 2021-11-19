using System.IO;

namespace ES3Internal
{
	public enum ES3FileMode {Read, Write, Append}

	public class ES3FileStream : FileStream
	{
		private bool isDisposed = false;

		public ES3FileStream( string path, ES3FileMode fileMode, int bufferSize, bool useAsync)
			: base( GetPath(path, fileMode), GetFileMode(fileMode), GetFileAccess(fileMode), FileShare.None, bufferSize, useAsync)
		{
		}

		// Gets a temporary path if necessary.
		protected static string GetPath(string path, ES3FileMode fileMode)
		{
			string directoryPath = ES3IO.GetDirectoryPath(path);
            // Attempt to create the directory incase it does not exist if we are storing data.
            if (fileMode != ES3FileMode.Read && directoryPath != ES3IO.persistentDataPath)
				ES3IO.CreateDirectory(directoryPath);
			if(fileMode != ES3FileMode.Write || fileMode == ES3FileMode.Append)
				return path;
			return (fileMode == ES3FileMode.Write) ? path + ES3IO.temporaryFileSuffix : path;
		}

		protected static FileMode GetFileMode(ES3FileMode fileMode)
		{
			if (fileMode == ES3FileMode.Read)
				return FileMode.Open;
			else if (fileMode == ES3FileMode.Write)
				return FileMode.Create;
			else
				return FileMode.Append;
		}

		protected static FileAccess GetFileAccess(ES3FileMode fileMode)
		{
			if (fileMode == ES3FileMode.Read)
				return FileAccess.Read;
			else if (fileMode == ES3FileMode.Write)
				return FileAccess.Write;
			else
				return FileAccess.Write;
		}

		protected override void Dispose (bool disposing)
		{
			// Ensure we only perform disposable once.
			if(isDisposed)
				return;
			isDisposed = true;

			base.Dispose(disposing);


			// If this is a file writer, we need to replace the temp file.
			/*if(fileMode == ES3FileMode.Write && fileMode != ES3FileMode.Append)
			{
				// Delete the old file before overwriting it.
				ES3IO.DeleteFile(path);
				// Rename temporary file to new file.
				ES3IO.MoveFile(path + ES3.temporaryFileSuffix, path);
			}*/
		}
	}
}