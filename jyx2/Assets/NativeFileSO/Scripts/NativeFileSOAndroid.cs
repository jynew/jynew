// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

//#define UNITY_ANDROID
using System;
using System.Linq;

using UnityEngine;

#if UNITY_ANDROID
namespace Keiwando.NFSO {
	
	public class NativeFileSOAndroid: INativeFileSOMobile {

		public static NativeFileSOAndroid shared = new NativeFileSOAndroid();

		private AndroidJavaObject Activity { 
			get {
				if (_activity == null) {
					using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) { 
						_activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					}
				}
				return _activity;
			}
		} 
		private AndroidJavaObject _activity;

		private AndroidJavaClass JavaNativeSO { 
			get {
				if (_javaNativeSO == null) {
					_javaNativeSO = new AndroidJavaClass("com.keiwando.lib_nativefileso.NativeFileSO");
				}
				return _javaNativeSO;
			}
		}
		private AndroidJavaClass _javaNativeSO;

		private static OpenedFile[] _noFiles = new OpenedFile[0];

		private NativeFileSOAndroid() { }

		public OpenedFile[] GetOpenedFiles() { 

			JavaNativeSO.CallStatic("LoadTemporaryFiles", Activity);
			int numberOfLoadedFiles = JavaNativeSO.CallStatic<int>("GetNumberOfLoadedFiles");

			if (numberOfLoadedFiles <= 0) { return _noFiles; }

			OpenedFile[] openedFiles = new OpenedFile[numberOfLoadedFiles];
			for (int i = 0; i < openedFiles.Length; i++) {
				AndroidJavaObject loadedFile = JavaNativeSO.CallStatic<AndroidJavaObject>("GetLoadedFileAtIndex", i);
				string filename = loadedFile.Call<string>("getFilename");
				//byte[] data = loadedFile.Call<byte[]>("getData");
				string path = loadedFile.Call<string>("getPath");
				byte[] data = System.IO.File.ReadAllBytes(path);
				openedFiles[i] = new OpenedFile(filename, data);
			}

			// Reset the loaded data
			JavaNativeSO.CallStatic("FreeMemory", Activity);

			return openedFiles;
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, bool canSelectMultiple) {

			string encodedMimeTypes = SupportedFileType.Any.MimeType;
			if (supportedTypes != null || supportedTypes.Length != 0) { 
				encodedMimeTypes = EncodeMimeTypes(supportedTypes.Select(x => x.MimeType).ToArray());
			}

			string methodName = canSelectMultiple ? "OpenFiles" : "OpenFile";
			JavaNativeSO.CallStatic(methodName, Activity, encodedMimeTypes);
		}

		public void SaveFile(FileToSave file) { 
		
			JavaNativeSO.CallStatic("SaveFile", Activity, file.SrcPath, file.MimeType);
		}

		private string EncodeMimeTypes(string[] mimetypes) {
			return string.Join(" ", mimetypes);
		}
	}
}
#endif
