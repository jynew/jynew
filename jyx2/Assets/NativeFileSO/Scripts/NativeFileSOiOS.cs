// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

//#define UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

#if UNITY_IOS

namespace Keiwando.NFSO {

	public class NativeFileSOiOS : INativeFileSOMobile {

		public delegate void UnityCallbackFunction();

		public static NativeFileSOiOS shared = new NativeFileSOiOS();

		[DllImport("__Internal")]
		private static extern void pluginSetCallback(UnityCallbackFunction callback);

		[DllImport("__Internal")]
		private static extern int pluginGetNumberOfOpenedFiles();

		[DllImport("__Internal")]
		private static extern IntPtr pluginGetFilenameForFileAtIndex(int i);

		[DllImport("__Internal")]
		private static extern IntPtr pluginGetDataForFileAtIndex(int i);

		[DllImport("__Internal")]
		private static extern ulong pluginGetDataLengthForFileAtIndex(int i);

		[DllImport("__Internal")]
		private static extern void pluginResetLoadedFile();

		[DllImport("__Internal")]
		private static extern void pluginOpenFile(string utis, bool canSelectMultiple);

		[DllImport("__Internal")]
		private static extern void pluginSaveFile(string srcPath, string name);

		private static OpenedFile[] _noFiles = new OpenedFile[0];

		private NativeFileSOiOS() {
#if !UNITY_EDITOR
			pluginSetCallback(NativeFileSOMobile.FileWasOpenedCallback);
#endif
		}

		public OpenedFile[] GetOpenedFiles() {

			var numOfLoadedFiles = pluginGetNumberOfOpenedFiles();
			if (numOfLoadedFiles == 0) return _noFiles;

			Debug.Log(string.Format("Files loaded: {0}", numOfLoadedFiles));

			var files = new OpenedFile[numOfLoadedFiles];
			for (int i = 0; i < numOfLoadedFiles; i++) {

				byte[] byteContents = new byte[pluginGetDataLengthForFileAtIndex(i)];
				Marshal.Copy(pluginGetDataForFileAtIndex(i), byteContents, 0, byteContents.Length);
				string filename = Marshal.PtrToStringAnsi(pluginGetFilenameForFileAtIndex(i));

				files[i] = new OpenedFile(filename, byteContents);
			}

			pluginResetLoadedFile();

			return files;
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, bool canSelectMultiple) {

			var encodedUTIs = SupportedFileType.Any.AppleUTI;

			if (supportedTypes != null && supportedTypes.Length > 0) {
				encodedUTIs = EncodeUTIs(supportedTypes.Select(x => x.AppleUTI).ToArray());
			}

			pluginOpenFile(encodedUTIs, canSelectMultiple);
		}

		public void SaveFile(FileToSave file) { 
			pluginSaveFile(file.SrcPath, file.Name);
		}

		public void LoadIfTemporaryFileAvailable() {}

		private string EncodeUTIs(string[] utis) {

			return string.Join("|", utis);
		}
	}
}

#endif