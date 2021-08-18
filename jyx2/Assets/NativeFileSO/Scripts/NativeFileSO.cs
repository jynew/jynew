// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using UnityEngine;

namespace Keiwando.NFSO { 

	/// <summary>
	/// Provides methods for native file import and export functionality
	/// which is shared between both mobile and desktop platforms.
	/// </summary>
	/// <example>
	/// <para>The following example demonstrates how to use the <see cref="NativeFileSO"/>
	/// class in order to copy/export a file from an existing path to a new location
	/// chosen by the user.</para>
	/// <code>
	/// using Keiwando.NFSO;
	/// 
	/// public class SaveTest {
	///
	/// 	public static void Main() {
	///
	/// 		string path = "path/to/existing/fileToSave.txt";
	/// 		string newFilename = "ExportedFile.txt";
	///			
	/// 		FileToSave file = new FileToSave(path, newFilename, SupportedFileType.PlainText);
	/// 		
	/// 		// Allows the user to choose a save location and saves the
	/// 		// file to that location
	/// 		NativeFileSO.shared.SaveFile(file);
	/// 	}
	/// }
	/// </code>
	/// 
	/// <para>The following example demonstrates how to use the <see cref="NativeFileSO"/>
	/// class in order to let the user choose a text file and handle its loaded contents.</para>
	/// <code>
	/// using Keiwando.NFSO;
	/// 
	/// public class OpenTest {
	/// 	
	/// 	public static void Main() {
	/// 		
	/// 		// We want the user to select a plain text file.
	/// 		SupportedFileType[] supportedFileTypes = {
	/// 			SupportedFileType.PlainText
	/// 		};
	/// 		
	/// 		NativeFileSO.shared.OpenFile(supportedFileTypes, 
	/// 									 delegate(bool fileWasOpened, OpenedFile file) {
	/// 			if (fileWasOpened) {
	/// 				// Process the loaded contents of "file" e.g
	/// 				// using <see cref="OpenedFile.ToUTF8String()"/>
	/// 			} else {
	/// 				// The file selection was cancelled.
	/// 			}
	/// 		});
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// <para>The dialogs shown by the Open and Save functions block any other UI
	/// interactions until the user dismisses the dialog.</para>
	/// 
	/// <para>This class is currently compatible with Windows, macOS, iOS and Android.
	/// Attempting to call the class methods on unsupported platforms will result
	/// in a <see cref="NullReferenceException"/>.</para>
	/// 
	/// <para>Thread safety is not guaranteed!</para>
	/// </remarks>
	/// <exception cref="NullReferenceException">
	/// Thrown when the API is attempted to be accessed on unsupported platforms.
	/// </exception>
	public class NativeFileSO : INativeFileSO {
		

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		private static INativeFileSO nativeFileSO = NativeFileSOMacWin.shared;
#elif UNITY_IOS || UNITY_ANDROID
		private static INativeFileSO nativeFileSO = NativeFileSOMobile.shared;
#else
		private static INativeFileSO nativeFileSO = null;
#endif

		/// <summary>
		/// The shared instance through which the API should be accessed.
		/// </summary>
		public static readonly NativeFileSO shared = new NativeFileSO();

		private NativeFileSO() {}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a
		/// single file to be opened. The selected file contents are then loaded 
		/// into memory managed by an instance of the <see cref="OpenedFile"/> class. 
		/// </summary>
		/// <example>
		/// The following example demonstrates how to prompt the user to select
		/// a file to be opened and then handle the results of that operation 
		/// accordingly.
		/// <code>
		/// // Determine which file types the user can choose from.
		/// SupportedFileType[] fileTypes = {
		/// 	SupportedFileType.PlainText
		/// };
		/// 
		/// OpenFile(fileTypes, delegate(bool fileWasOpened, OpenedFile file) {
		/// 	if (fileWasOpened) {
		/// 		// Process the loaded file contents, e.g. using
		/// 		// <see cref="OpenedFile.ToUTF8String()"/>.
		/// 	} else {
		/// 		// The file selection was cancelled.
		/// 	}
		/// });
		/// </code>
		/// </example>
		/// <remarks>
		/// <para>The <paramref name="supportedTypes"/> array is just an indicator to the native file chooser
		/// dialog to only allow the user to choose from files of predefined types.
		/// However, this is not a guarantee that the chosen file is going to contain
		/// data in the correct file format.</para>
		/// 
		/// <para>For one, the file chooser on Android allows the user to circumvent the 
		/// file type filter and ultimately select an arbitrary file. At the same 
		/// time, the file choosers that rely on filtering the files based on their
		/// extension can be "cheated" by simply changing the file extensions of an 
		/// arbitrary file.</para>
		/// 
		/// <para>You should therefore always check whether the opened file inside of the 
		/// <paramref name="onCompletion"/> callback contains valid data, instead of assuming that the 
		/// chosen file type is always correct.</para>
		/// </remarks>
		/// <seealso cref="NativeFileSO.OpenFiles(SupportedFileType[], Action{bool, OpenedFile[]})"/>
		/// <param name="supportedTypes">The file types used to filter the 
		/// available files shown to the user.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has 
		/// been dismissed. The first parameter of the callback specifies whether 
		/// a file was opened or the selection was cancelled.</param>
		public void OpenFile (SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion) {
			nativeFileSO.OpenFile(supportedTypes, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple 
		/// files to be opened at once. The selected file contents are then loaded 
		/// into memory managed by instances of the <see cref="OpenedFile"/> class. 
		/// </summary>
		/// <example>
		/// The following example demonstrates how to prompt the user to select
		/// files to be opened and then handle the results of that operation 
		/// accordingly.
		/// <code>
		/// // Determine which file types the user can choose from.
		/// SupportedFileType[] fileTypes = {
		/// 	SupportedFileType.PlainText
		/// };
		/// 
		/// OpenFiles(fileTypes, delegate(bool filesWereOpened, OpenedFile[] files) {
		/// 	if (filesWereOpened) {
		/// 		foreach (var file in files) {
		/// 			// Process the loaded file contents, e.g. using
		/// 			// <see cref="OpenedFile.ToUTF8String()"/>.
		/// 		}
		/// 	} else {
		/// 		// The file selection was cancelled.
		/// 	}
		/// });
		/// </code>
		/// </example>
		/// <remarks>
		/// <para>The <paramref name="supportedTypes"/> array is just an indicator to the native file chooser
		/// dialog to only allow the user to choose from files of predefined types.
		/// However, this is not a guarantee that the chosen files are going to contain
		/// data in the correct file format.</para>
		/// 
		/// <para>For one, the file chooser on Android allows the user to circumvent the 
		/// file type filter and ultimately select arbitrary files. At the same 
		/// time, the file choosers that rely on filtering the files based on their
		/// extension can be "cheated" by simply changing the file extensions of an 
		/// arbitrary file.</para>
		/// 
		/// <para>You should therefore always check whether the opened files inside of the 
		/// <paramref name="onCompletion"/> callback contain valid data, instead of assuming that the 
		/// file types are always correct.</para>
		/// </remarks>
		/// <param name="supportedTypes">The file types used to filter the available files shown to the user.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has 
		/// been dismissed. The first parameter of the callback specifies whether 
		/// files were opened or the selection was cancelled.</param>
		public void OpenFiles (SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			nativeFileSO.OpenFiles(supportedTypes, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select an export
		/// location for the specified file and exports/copies the file to that location. 
		/// </summary>
		/// <example>
		/// The following example demonstrates how to export a file using a native interface.
		/// <code>
		/// // We have a text file that we want to export
		/// string path = "path/to/existing/fileToSave.txt";
		/// string newFilename = "ExportedFile.txt";
		///
		/// FileToSave file = new FileToSave(path, newFilename, SupportedFileType.PlainText);
		///
		/// // Allows the user to choose a save location and saves the 
		/// // file to that location
		/// NativeFileSO.shared.SaveFile(file);
		/// </code>
		/// </example>
		/// <param name="file">An instance of the <see cref="FileToSave"/> class
		/// which holds information about the file to be exported.</param>
		public void SaveFile (FileToSave file) {
			nativeFileSO.SaveFile(file);
		}
	}
}


