// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using AOT;
using UnityEngine;

namespace Keiwando.NFSO {

	/// <summary>
	/// Provides methods for native file open and share functionality on 
	/// Android and iOS.
	/// </summary>
	/// <remarks>
	/// See <see cref="NativeFileSO"/> for a more in-depth overview of how to
	/// use the individual methods.
	/// </remarks>
	public class NativeFileSOMobile : INativeFileSO {

#if UNITY_IOS
		private INativeFileSOMobile nativeFileSO = NativeFileSOiOS.shared;
#elif UNITY_ANDROID
		private INativeFileSOMobile nativeFileSO = NativeFileSOAndroid.shared;
#else
		private INativeFileSOMobile nativeFileSO = null;
#endif

		public static NativeFileSOMobile shared = new NativeFileSOMobile();

		/// <summary>
		/// This event fires when files were opened without the user being manually 
		/// prompted to open files from within the app. This occurs if certain
		/// file types have been associated with the application.
		/// </summary>
		/// <remarks>See <see cref="SupportedFilePreferences"/> for more information on
		/// file associations.</remarks>
		public event Action<OpenedFile[]> FilesWereOpened;

		private Action<bool, OpenedFile[]> _callback;
		private bool isBusy = false;

		private NativeFileSOMobile() {

#if !UNITY_EDITOR

			NativeFileSOUnityEvent.UnityReceivedControl += delegate {
				TryRetrieveOpenedFile();
				isBusy = false;
			};
#endif
		}

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

			if (isBusy) return;
			isBusy = true;

			_callback = delegate (bool wasOpened, OpenedFile[] openedFiles) {
				if (onCompletion != null) {
					onCompletion(wasOpened, wasOpened ? openedFiles[0] : null);
				}
			};
			nativeFileSO.OpenFiles(supportedTypes, false);
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
		/// files were opened or the selection was cancelled.</param>
		public void OpenFiles (SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {

			if (isBusy) return;
			isBusy = true;
			_callback = onCompletion;

			nativeFileSO.OpenFiles(supportedTypes, true);
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

			if (isBusy) return;

			isBusy = true;
			nativeFileSO.SaveFile(file);
		}

		private void TryRetrieveOpenedFile() {

			if (nativeFileSO == null) return;

			var files = nativeFileSO.GetOpenedFiles();
			SendFileOpenedEvent(files.Length > 0, files);
		}

		private void SendFileOpenedEvent(bool fileWasOpened, OpenedFile[] file) {
			
			if (_callback != null) {
				_callback(fileWasOpened, file);
				_callback = null;
				return;
			}

			if (fileWasOpened && FilesWereOpened != null) {
				FilesWereOpened(file);
			}
		}

#if UNITY_IOS
		[MonoPInvokeCallback(typeof(NativeFileSOiOS.UnityCallbackFunction))]
		internal static void FileWasOpenedCallback() {
			shared.TryRetrieveOpenedFile();
			shared.isBusy = false;
		}
#endif
	}
}


