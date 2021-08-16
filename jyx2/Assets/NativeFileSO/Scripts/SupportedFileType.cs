// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Keiwando.NFSO {

	/// <summary>
	/// Represents a file type that can be opened by the application.
	/// </summary>
	/// <remarks>
	/// Use one of the static presets or create your own instance of this class.
	/// Include instances of this type in the <see cref="T:Keiwando.NFSO.SupportedFilePreferences.supportedFileTypes"/>
	/// array in order to associate them with this application on iOS and Android.
	/// </remarks>
	public class SupportedFileType {

		/// <summary>
		/// The title of this file type.
		/// </summary>
		public string Name;

		/// <summary>
		/// The extension(s) of this file type. Separate multiple
		/// extensions with a pipe (|).
		/// </summary>
		/// <example>
		/// Here's how the JPEG preset is defined with two possible extensions:
		/// <code>
		/// public static readonly SupportedFileType JPEG = new SupportedFileType {
		/// 	Name = "JPEG Image",
		/// 	Extension = "jpg|jpeg",
		///    Owner = false,
		///    AppleUTI = "public.jpeg",
		///    MimeType = "image/jpeg"
		/// };
		/// </code>
		/// </example>
		public string Extension;

		/// <summary>
		/// Specifies whether this application is the primary creator
		/// of this file type.
		/// </summary>
		/// <remarks>
		/// This property is only added to the Info.plist file for iOS
		/// projects when associating this SupportedFileType with the application. 
		/// It can be ignored if iOS is not a targeted platform.
		/// </remarks>
		public bool Owner = false;

		/// <summary>
		/// The Uniform Type Identifier used for file association on iOS. 
		/// Separate multiple UTIs with a pipe (|).
		/// </summary>
		/// <remarks>
		/// See <a href="https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/understanding_utis/understand_utis_declare/understand_utis_declare.html#//apple_ref/doc/uid/TP40001319-CH204-SW1"
		/// >Declaring New Uniform Type Identifiers</a>
		/// for more info. This property is only relevant on iOS.
		/// </remarks>
		public string AppleUTI = "public.data|public.content";

		/// <summary>
		/// Set this value if you are declaring a custom file type that conforms
		/// to an existing UTI. 
		/// 
		/// Separate multiple UTIs with a pipe (|).
		/// </summary>
		/// <example>
		/// For example, if you are declaring a custom file type based on plain-text, 
		/// you should set this property to "public.plain-text".
		/// </example>
		public string AppleConformsToUTI = "";

		/// <summary>
		/// The MIME type of the file.
		/// </summary>
		/// <remarks>
		/// On Android, only the MimeType is relevant for filtering supported 
		/// files (And even these filters are not guaranteed to be respected
		/// by the Android file chooser).
		/// </remarks>
		public string MimeType = "*/*";

		// MARK: - Presets

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches files of all types.
		/// </summary>
		public static readonly SupportedFileType Any = new SupportedFileType {

			Name = "Any",
			Extension = "*",
			Owner = false,
			AppleUTI = "public.data|public.content",
			MimeType = "*/*"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches plain text files
		/// with a .txt extension.
		/// </summary>
		public static readonly SupportedFileType PlainText = new SupportedFileType {

			Name = "Text File",
			Extension = "txt",
			Owner = false,
			AppleUTI = "public.plain-text",
			MimeType = "text/plain"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches XML files with
		/// a .xml extension.
		/// </summary>
		public static readonly SupportedFileType XML = new SupportedFileType {

			Name = "XML File",
			Extension = "xml",
			Owner = false,
			AppleUTI = "public.text",
			MimeType = "text/xml"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches JPEG image
		/// files with either a .jpg or .jpeg file extension.
		/// </summary>
		public static readonly SupportedFileType JPEG = new SupportedFileType {

			Name = "JPEG Image",
			Extension = "jpg|jpeg",
			Owner = false,
			AppleUTI = "public.jpeg",
			MimeType = "image/jpeg"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches PNG image
		/// files with a .png file extension.
		/// </summary>
		public static readonly SupportedFileType PNG = new SupportedFileType {

			Name = "PNG Image",
			Extension = "png",
			Owner = false,
			AppleUTI = "public.png",
			MimeType = "image/png"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches MP4 video files
		/// with a .mp4 extension.
		/// </summary>
		public static readonly SupportedFileType MP4 = new SupportedFileType {

			Name = "MPEG-4 content",
			Extension = "mp4",
			Owner = false,
			AppleUTI = "public.mpeg-4",
			MimeType = "video/mp4"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches mp3 audio files
		/// with a .mp3 extension.
		/// </summary>
		public static readonly SupportedFileType MP3 = new SupportedFileType {

			Name = "MPEG-3 audio",
			Extension = "mp3",
			Owner = false,
			AppleUTI = "public.mp3",
			MimeType = "audio/mpeg3"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches PDF files
		/// with a .pdf file extension.
		/// </summary>
		public static readonly SupportedFileType PDF = new SupportedFileType {

			Name = "PDF",
			Extension = "pdf",
			Owner = false,
			AppleUTI = "com.adobe.pdf",
			MimeType = "application/pdf"
		};

		/// <summary>
		/// The <see cref="SupportedFileType"/> preset that matches GIF files
		/// with a .gif file extension.
		/// </summary>
		public static readonly SupportedFileType GIF = new SupportedFileType {

			Name = "Gif",
			Extension = "gif",
			Owner = false,
			AppleUTI = "com.compuserve.gif",
			MimeType = "image/gif"
		};
	}
}