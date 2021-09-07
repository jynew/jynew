// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;

namespace Keiwando.NFSO {

	/// <summary>
	/// Stores basic information about a file which has been chosen by the user
	/// and has had its contents loaded into memory.
	/// </summary>
	public class OpenedFile {

		/// <summary>
		/// The filename including the extension.
		/// </summary>
		/// <remarks>
		/// The filename here is not the full path to the file but only its last
		/// segment.
		/// </remarks>
		/// <value>The filename.</value>
		public string Name { get; private set; }

		/// <summary>
		/// The file extension (including the period ".").
		/// </summary>
		/// <value>The file extension.</value>
		public string Extension { get; private set; }

		/// <summary>
		/// The loaded byte contents of the file.
		/// </summary>
		/// <value>The byte data.</value>
		public byte[] Data { 
			get {
				return _data;
			}
		}
		private byte[] _data;

		private string _utf8String;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenedFile"/> class
		/// with the specified filename and byte data.
		/// </summary>
		/// <param name="filename">The filename of the file (including its extension).</param>
		/// <param name="data">The loaded byte data of the file.</param>
		public OpenedFile(string filename, byte[] data) {
			this._data = data;
			this.Name = Path.GetFileName(filename);
			this.Extension = Path.GetExtension(filename);
		} 

		/// <summary>
		/// A helper function which can be used to return the byte contents
		/// of the file interpreted as a UTF-8-encoded string. 
		/// </summary>
		/// <remarks>
		/// The generated string is cached after it has been generated for the first 
		/// time.
		/// </remarks>
		/// <returns>The UTF-8 string representation of the file contents. Returns an
		/// empty string if the conversion fails.</returns>
		public string ToUTF8String() {

			if (_utf8String == null) {
				try {
					_utf8String = System.Text.Encoding.UTF8.GetString(_data);
				} catch {
					_utf8String = string.Empty;
				}
			}

			return _utf8String;
		}
	}
}
