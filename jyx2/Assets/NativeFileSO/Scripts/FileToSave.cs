// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;

namespace Keiwando.NFSO {

	/// <summary>
	/// Manages data about a locally existing file to be saved/exported.
	/// </summary>
	public class FileToSave {

		/// <summary>
		/// Gets the current source path of the existing file.
		/// </summary>
		/// <value>The path to the file to be saved.</value>
		public string SrcPath { get; private set; }

		/// <summary>
		/// Gets the name under which the file should be saved (including the extension).
		/// </summary>
		/// <remarks>
		/// On desktop platforms the user still has the option to change the filename
		/// inside of the save dialog.
		/// </remarks>
		/// <value>The new filename.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the file extension under which the file is to be saved.
		/// </summary>
		/// <value>The file extension.</value>
		public string Extension { get; private set; }

		/// <summary>
		/// Gets the MIME type of the file.
		/// </summary>
		/// <value>The MIME type of the file.</value>
		public string MimeType { get; private set; }

		/// <summary>
		/// The <see cref="T:Keiwando.NFSO.SupportedFileType"/> associated with this file.
		/// </summary>
		/// <value>The type of the file.</value>
		public SupportedFileType FileType { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileToSave"/> class
		/// with the specified file path.
		/// </summary>
		/// <param name="srcPath">The current path to the existing file to be saved.</param>
		/// <param name="fileType">The file type associated with this file.</param>
		public FileToSave(string srcPath, SupportedFileType fileType = null) {
			this.SrcPath = srcPath;
			this.Name = Path.GetFileName(srcPath);
			this.Extension = GetExtension(srcPath);
			this.MimeType = "*/*";
			this.FileType = fileType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileToSave"/> class
		/// with the specified file path and the new file name under which to save the file.
		/// </summary>
		/// <param name="srcPath">The current path to the existing file to be saved.</param>
		/// <param name="newName">The new file name including the extension.</param>
		/// <param name="fileType">The file type associated with this file.</param>
		public FileToSave(string srcPath, string newName, SupportedFileType fileType = null) 
			: this(srcPath, fileType) {

			this.Name = newName;
			this.Extension = GetExtension(srcPath);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileToSave"/> class
		/// with the specified file path, new filename and extension.
		/// </summary>
		/// <param name="srcPath">The current path to the existing file to be saved.</param>
		/// <param name="newName">The new file name including the extension.</param>
		/// <param name="extension">The new file extension.</param>
		/// <param name="mimetype">The MIME type of the file.</param>
		/// <param name="fileType">The file type associated with this file.</param>
		public FileToSave(string srcPath, string newName, string extension, 
		                  string mimetype = "*/*", SupportedFileType fileType = null) 
			: this(srcPath, newName, fileType) {

			this.Extension = extension;
			this.MimeType = mimetype;
			this.FileType = fileType;
		}


		private static string GetExtension(string path) {
			var fullExtension = Path.GetExtension(path);
			if (fullExtension.StartsWith(".")) {
				return fullExtension.Substring(1);
			} else {
				return fullExtension;
			}
		}
	}
}
