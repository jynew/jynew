// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Keiwando.NFSO {

	/// <summary>
	/// Provides methods for native file open and save functionality which is
	/// shared between Windows and macOS.
	/// </summary>
	/// <example>
	/// The following example demonstrates how to use the <see cref="NativeFileSOMacWin"/>
	/// class in order to allow the user to select multiple paths of files to be opened.
	/// <code>
	/// using Keiwando.NFSO;
	/// 
	/// public class OpenPathsTest {
	///
	/// 	public static void Main() {
	///
	/// 		// We want the user to select paths to plain text files.
	/// 		SupportedFileType[] supportedFileTypes = {
	/// 			SupportedFileType.PlainText
	/// 		};
	/// 
	/// 		bool multiSelect = true; // Can select multiple paths at once
	/// 		string title = "Custom Title"; // The title of the panel
	/// 		string directory = ""; // Remember and reset to the previously selected directory
	/// 		NativeFileSOMacWin.shared.SelectOpenPaths(supportedFileTypes, 
	/// 		  mutltiSelect, title, directory,
	/// 		  delegate(bool pathsWereSelected, string[] paths) {
	/// 			if (pathsWereSelected) {
	/// 				// Process the information within the paths array.
	/// 			} else {
	/// 				// The file selection was cancelled.
	/// 			}
	/// 		});
	///		}
	/// }
	/// </code>
	/// 
	/// See also <see cref="NativeFileSO"/> for examples of how to use the more
	/// general API that also applies to mobile platforms.
	/// </example>
	/// <remarks>
	/// <para>Compared to the <see cref="NativeFileSO"/> class, the <see cref="NativeFileSOMacWin"/>
	/// class provides additional methods which cannot be implemented in the same 
	/// way on mobile platforms due to the different available native APIs.</para>
	/// 
	/// <para>For example, on iOS and Android, the path to a selected file to be opened 
	/// is only temporarily valid due to native security features and access
	/// restrictions. Therefore, the entire file has to be copied into memory before
	/// its data can be handed over to the caller of the method.</para>
	/// 
	/// <para>On desktop platforms, however, it is possible to provide methods that
	/// simply return the chosen file path for a save or open operation. This 
	/// then allows for more custom processing of the selected files. For example,
	/// the file contents can be loaded and processed in smaller chunks which is more 
	/// memory efficient and a preferred solution compared to loading the entire
	/// file contents into memory, which should only be done if necessary.</para>
	/// 
	/// <para>This class is currently compatible with Windows and macOS. Attempting
	/// Attempting to call the class methods on unsupported platforms will result
	/// in a <see cref="NullReferenceException"/>.</para>
	/// 
	/// <para>Thread safety is not guaranteed!</para>
	/// 
	/// <para>Note for macOS: The Open and Save API methods that take a completion 
	/// callback display the NSOpenPanel modally as a sheet, which makes it 
	/// anchored to the top of the window and non-draggable. The ..Sync variants
	/// of those calls, however, use a floating panel which is detached from the
	/// main application window and can be dragged around by the user.</para>
	/// 
	/// <para>If the visual style of the panel is of importance to you, simply
	/// call the respective method.</para>
	/// 
	/// </remarks>
	/// <exception cref="NullReferenceException">
	/// Thrown when the API is attempted to be accessed on unsupported platforms.
	/// </exception>
	public class NativeFileSOMacWin : INativeFileSO {

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
		private INativeFileSODesktop nativeFileSO = NativeFileSOMac.shared;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		private INativeFileSODesktop nativeFileSO = NativeFileSOWindows.shared;
#else
		private INativeFileSODesktop nativeFileSO = null;
#endif

		/// <summary>
		///  The shared instance through which the API should be accessed.
		/// </summary>
		public static NativeFileSOMacWin shared = new NativeFileSOMacWin();

		private delegate void UnityCallbackPathSelected(bool pathSelected, IntPtr path);

		private NativeFileSOMacWin() {}

		// MARK: - INativeFileSO Implementation

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a
		/// single file to be opened. The selected file contents are then loaded 
		/// into memory managed by an instance of the <see cref="OpenedFile"/> class.
		/// </summary>
		/// <remarks>
		/// See <see cref="NativeFileSO.OpenFile(SupportedFileType[], Action{bool, OpenedFile})"/>
		/// for more information on the usage of this method.
		/// </remarks>
		/// <param name="supportedTypes"> The file types used to filter the available files shown to the user.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has been dismissed. 
		/// The first parameter of the callback specifies whether 
		/// a file was opened or the selection was cancelled.</param>
		public void OpenFile (SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion) {
			nativeFileSO.OpenFile(supportedTypes, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple 
		/// files to be opened at once. The selected file contents are then loaded 
		/// into memory managed by instances of the <see cref="OpenedFile"/> class.
		/// </summary>
		/// <remarks>
		/// See <see cref="NativeFileSO.OpenFiles(SupportedFileType[], Action{bool, OpenedFile[]})"/>
		/// for more information on the usage of this method.
		/// </remarks>
		/// <param name="supportedTypes">The file types used to filter the available files shown to the user.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has been dismissed. 
		/// The first parameter of the callback specifies whether 
		/// a file was opened or the selection was cancelled.</param>
		public void OpenFiles (SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			nativeFileSO.OpenFiles(supportedTypes, true, "", "", onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a save
		/// location for the specified file and copies the file to that location. 
		/// </summary>
		/// <remarks>
		/// See <see cref="NativeFileSO.SaveFile(FileToSave)"/> for more information
		/// on the usage of this method.
		/// </remarks>
		/// <param name="file">An instance of the <see cref="FileToSave"/> class
		/// which holds information about the file to be exported.</param>
		public void SaveFile (FileToSave file) {
			nativeFileSO.SaveFile(file);
		}

		// MARK: - Desktop Only functionality

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple
		/// files to be opened and loads the selected files into instances of the 
		/// <see cref="OpenedFile"/> class.
		/// </summary>
		/// <remarks>
		/// On macOS, this will present a non-floating panel.
		/// </remarks>
		/// <param name="fileTypes">The file types used to filter the available files shown 
		/// to the user.</param>
		/// <param name="canSelectMultiple">Determines whether multiple files can be selected 
		/// and loaded at once.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has been dismissed. 
		/// The first parameter of the callback specifies whether 
		/// files were opened or the selection was cancelled.</param>
		public void OpenFiles (SupportedFileType[] fileTypes, 
		                      bool canSelectMultiple,
					   		  string title, 
		                      string directory, 
					   		  Action<bool, OpenedFile[]> onCompletion) {
			
			nativeFileSO.OpenFiles(fileTypes, canSelectMultiple, title, directory, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple
		/// files to be opened and loads the selected files into instances of the 
		/// <see cref="OpenedFile"/> class.
		/// </summary>
		/// <remarks>
		/// On macOS, this will present a floating panel.
		/// </remarks>
		/// <returns>The array of loaded files managed by <see cref="OpenedFile"/>
		/// instances.</returns>
		/// <param name="fileTypes">The file types used to filter the available files shown 
		/// to the user.</param>
		/// <param name="canSelectMultiple">Determines whether multiple files can be selected 
		/// and loaded at once.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		public OpenedFile[] OpenFilesSync (SupportedFileType[] fileTypes, 
		                                  bool canSelectMultiple, 
								   		  string title, 
		                                  string directory) {
			
			return nativeFileSO.OpenFilesSync(fileTypes, canSelectMultiple, title, directory);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple
		/// file paths for opening files.
		/// </summary>
		/// <remarks>
		/// On macOS, this will present a non-floating panel.
		/// </remarks>
		/// <param name="fileTypes">The file types used to filter the available files shown 
		/// to the user.</param>
		/// <param name="canSelectMultiple">Determines whether multiple files can be selected 
		/// and loaded at once.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		/// <param name="onCompletion">A callback for when the presented dialog has been dismissed. 
		/// The first parameter of the callback specifies whether 
		/// a path was selected or the selection was cancelled.</param>
		public void SelectOpenPaths (SupportedFileType[] fileTypes, 
		                            bool canSelectMultiple,
							 		string title, 
		                            string directory,
							 		Action<bool, string[]> onCompletion) {
			
			nativeFileSO.SelectOpenPaths(fileTypes, canSelectMultiple, title, directory, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select multiple
		/// file paths for opening files.
		/// </summary>
		/// <remarks>
		/// On macOS, this will present a floating panel.
		/// </remarks>
		/// <returns>The array of selected file paths.</returns>
		/// <param name="fileTypes">The file types used to filter the available files shown 
		/// to the user.</param>
		/// <param name="canSelectMultiple">Determines whether multiple files can be selected 
		/// and loaded at once.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		public string[] SelectOpenPathsSync (SupportedFileType[] fileTypes, 
		                                   	bool canSelectMultiple,
									 		string title, 
		                                    string directory) {
			
			return nativeFileSO.SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a save
		/// location for the specified file and copies the file to that location. 
		/// </summary>
		/// <param name="file">An instance of the <see cref="FileToSave"/> class
		/// which holds information about the file to be exported.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		public void SaveFile (FileToSave file, string title, string directory) {
			nativeFileSO.SaveFile(file, title, directory);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a save
		/// location for the specified file.
		/// </summary>
		/// <param name="file">An instance of the <see cref="FileToSave"/> class
		/// which holds information about the file to be exported.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		/// <param name="onCompletion">A callback for when the presented dialog
		/// has been dismissed. The first parameter of the callback specifies whether 
		/// a path was selected or the selection was cancelled.</param>
		public void SelectSavePath (FileToSave file,
								   string title,
								   string directory,
								   Action<bool, string> onCompletion) {
			nativeFileSO.SelectSavePath(file, title, directory, onCompletion);
		}

		/// <summary>
		/// Presents a native dialog to the user which allows them to select a save
		/// location for the specified file.
		/// </summary>
		/// <returns>The selected save location. Returns <c>null</c> if no path was selected.</returns>
		/// <param name="file">An instance of the <see cref="FileToSave"/> class
		/// which holds information about the file to be exported.</param>
		/// <param name="title">The title of the dialog. Note: The title is not
		/// shown on macOS 10.11 and above.</param>
		/// <param name="directory">The default directory in which file navigation
		/// should start. If this value is empty, the panel will remember the 
		/// last visited directory.</param>
		public string SelectSavePathSync (FileToSave file,
								  		 string title,
										 string directory) {
			return nativeFileSO.SelectSavePathSync(file, title, directory);
		}


		/// <summary>
		/// Loads the contents of a file at the specified path into an instance
		/// of the <see cref="OpenedFile"/> class.
		/// </summary>
		/// <returns>The <see cref="OpenedFile"/> instance or null, if the file
		/// could not be loaded.</returns>
		/// <param name="path">The full path to the file that is to be loaded.</param>
		public static OpenedFile FileFromPath (string path) {

			try {
				byte[] data = File.ReadAllBytes(path);
				var name = Path.GetFileName(path);
				return new OpenedFile(name, data);
			} catch (Exception e) {
				Debug.Log(e.StackTrace);
				return null;
			}
		}

		/// <summary>
		/// Loads the contents of multiple files at the specified paths into instances
		/// of the <see cref="OpenedFile"/> class.
		/// </summary>
		/// <returns>An array of <see cref="OpenedFile"/> instances containing the 
		/// loaded file data.</returns>
		/// <param name="paths">An array of full paths to the files to be loaded.</param>
		public static OpenedFile[] FilesFromPaths (string[] paths) {
			var files = new List<OpenedFile>();

			foreach (var path in paths) {
				var file = FileFromPath(path);
				if (file != null) {
					files.Add(file);
				}
			}
			return files.ToArray();
		}

		/// <summary>
		/// Copies the specified file to the given path.
		/// </summary>
		/// <param name="file">The file to be saved/copied.</param>
		/// <param name="path">The full save path denoting the new file location.</param>
		public static void SaveFileToPath (FileToSave file, string path) {
			File.Copy(file.SrcPath, path, true);
		}
	}
}


