// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace Keiwando.NFSO {

	public class NativeFileSOMac: INativeFileSODesktop {

		[DllImport("NativeFileSOMac")]
		private static extern void pluginSetCallback(UnityCallbackPathsSelected callback);

		[DllImport("NativeFileSOMac")]
		private static extern void pluginOpenFile(string extensions, 
		                                          bool canSelectMultiple, 
		                                          string title, 
		                                          string directory);
		
		[DllImport("NativeFileSOMac")]
		private static extern IntPtr pluginOpenFileSync(string extensions,
												  	  bool canSelectMultiple,
												  	  string title,
												  	  string directory);

		[DllImport("NativeFileSOMac")]
		private static extern void pluginSaveFile(string name, 
		                                          string extension, 
		                                          string title, 
		                                          string directory);

		[DllImport("NativeFileSOMac")]
		private static extern IntPtr pluginSaveFileSync(string name,
												  	  string extension,
												 	  string title,
												  	  string directory);

		[DllImport("NativeFileSOMac")]
		private static extern void pluginFreeMemory();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void UnityCallbackPathsSelected(
			bool pathsSelected, IntPtr paths, ulong length);

		public static NativeFileSOMac shared = new NativeFileSOMac();

		private static string[] _noPaths = new string[0];
		private static OpenedFile[] _noFiles = new OpenedFile[0];

		private bool isBusy = false;
		private FileToSave _cachedFileToSave;
		private Action<bool, OpenedFile[]> _filesCallback;
		private Action<bool, string[]> _pathsCallback;

		public void OpenFile(SupportedFileType[] supportedTypes, 
		                     Action<bool, OpenedFile> onCompletion) {

			OpenFiles(supportedTypes, false, null, null, 
			          delegate(bool wasOpened, OpenedFile[] files) {

				if (wasOpened) {
					onCompletion(true, files[0]);
			  	} else {
				  onCompletion(false, null);			
				}
			});
		}

		public void OpenFiles(SupportedFileType[] supportedTypes,
							 Action<bool, OpenedFile[]> onCompletion) {

			OpenFiles(supportedTypes, true, null, null, onCompletion);
		}

		public void SaveFile(FileToSave file) {

			SaveFile(file, null, null);
		}

		// MARK: - INativeFileSODesktop

		public void OpenFiles(SupportedFileType[] fileTypes, bool canSelectMultiple,
					   string title, string directory, 
					   Action<bool, OpenedFile[]> onCompletion) {

			if (isBusy) return;
			isBusy = true;

			_filesCallback = onCompletion;
			var extensions = EncodeExtensions(fileTypes);

			pluginSetCallback(DidSelectPathsForOpenFileCB);
			pluginOpenFile(extensions, canSelectMultiple, title, directory);
		}

		public OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, bool canSelectMultiple, 
								   string title, string directory) {

			var paths = SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
			var files = new List<OpenedFile>();

			for (int i = 0; i < paths.Length; i++) {
				files.Add(NativeFileSOMacWin.FileFromPath(paths[i]));
			}

			return files.ToArray();
		}

		public void SelectOpenPaths(SupportedFileType[] fileTypes, bool canSelectMultiple,
							 string title, string directory,
							 Action<bool, string[]> onCompletion) {

			if (isBusy) return;
			isBusy = true;

			_pathsCallback = onCompletion;
			var extensions = EncodeExtensions(fileTypes);

			pluginSetCallback(DidSelectPathsForPathsCB);
			pluginOpenFile(extensions, canSelectMultiple, title, directory);
		}

		public string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, bool canSelectMultiple,
									 		string title, string directory) {

			if (isBusy) { return _noPaths; }
			isBusy = true;

			var pathPtr = pluginOpenFileSync(EncodeExtensions(fileTypes), canSelectMultiple, title, directory);
			pluginFreeMemory();
			isBusy = false;

			var paths = Marshal.PtrToStringAnsi(pathPtr); 
			if (paths == null) { return _noPaths; }
			return paths.Split((char)28);
		}

		public void SaveFile(FileToSave file, string title, string directory) {

			if (isBusy) return;
			isBusy = true;

			pluginSetCallback(DidSelectPathForSaveCB);

			_cachedFileToSave = file;
			pluginSaveFile(file.Name, file.Extension, title, directory);
		}

		public void SelectSavePath(FileToSave file,
								   string title, string directory,
								   Action<bool, string> onCompletion) { 
		
			if (isBusy) { return; }
			isBusy = true;

			pluginSetCallback(DidSelectPathsForPathsCB);

			_pathsCallback = delegate (bool selected, string[] paths) {
				var path = paths.Length > 0 ? paths[0] : null;
				onCompletion(selected, path);
			};
			_cachedFileToSave = file;
			pluginSaveFile(file.Name, file.Extension, title, directory);
		}

		public string SelectSavePathSync(FileToSave file,
										 string title, 
		                                 string directory) { 

			if (isBusy) { return null; }
			isBusy = true;

			var pathPtr = pluginSaveFileSync(file.Name, file.Extension, title, directory);
			pluginFreeMemory();
			isBusy = false;

			return Marshal.PtrToStringAnsi(pathPtr);
		}

		// MARK: - Callbacks

		[AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
		private static void DidSelectPathsForOpenFileCB(bool pathsSelected, IntPtr pathsPtr, ulong length) {

			if (pathsSelected) {
				string paths = Marshal.PtrToStringAuto(pathsPtr, (int)length);
				shared.DidSelectPathsForOpenFile(pathsSelected, paths.Split((char)28));
			} else { 
				shared.DidSelectPathsForOpenFile(pathsSelected, _noPaths);
			}
		}

		[AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
		private static void DidSelectPathsForPathsCB(bool pathsSelected, IntPtr pathsPtr, ulong length) {

			if (pathsSelected) {
				string paths = Marshal.PtrToStringAuto(pathsPtr, (int)length);
				shared.DidSelectPathsForPaths(pathsSelected, paths.Split((char)28));
			} else { 
				shared.DidSelectPathsForPaths(pathsSelected,_noPaths);
			}

		}

		[AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
		private static void DidSelectPathForSaveCB(bool pathsSelected, IntPtr pathsPtr, ulong length) {

			if (pathsSelected) {
				string paths = Marshal.PtrToStringAuto(pathsPtr, (int)length);
				shared.DidSelectPathForSave(pathsSelected, paths.Split((char)28));
			} else { 
				shared.DidSelectPathsForPaths(pathsSelected, _noPaths);
			}
		}

		private void DidSelectPathsForOpenFile(bool pathsSelected, string[] paths) {

			isBusy = false;

			if (_filesCallback == null) { return; }

			if (pathsSelected) {
				var files = NativeFileSOMacWin.FilesFromPaths(paths);

				if (files.Length > 0) {
					_filesCallback(true, files);
				} else {
					_filesCallback(false, _noFiles);
				}
			} else {
				_filesCallback(false, _noFiles);
			}
		}

		private void DidSelectPathsForPaths(bool pathsSelected, string[] paths) {

			isBusy = false;

			if (_pathsCallback == null) { return; }

			if (pathsSelected) {
				_pathsCallback(true, paths);
			} else {
				_pathsCallback(false, _noPaths);
			}
		}

		private void DidSelectPathForSave(bool pathsSelected, string[] paths) {

			isBusy = false;

			var toSave = _cachedFileToSave;
			if (toSave == null || !pathsSelected) return;

			var path = paths[0];

			NativeFileSOMacWin.SaveFileToPath(toSave, path);
		}

		private string EncodeExtensions(string[] extensions) {

			return string.Join("%", extensions);
		}

		private string EncodeExtensions(SupportedFileType[] fileTypes) { 

			var extensions = fileTypes.SelectMany(x => x.Extension.Split('|')).ToArray();
			return EncodeExtensions(extensions);
		} 
	}
}
#endif