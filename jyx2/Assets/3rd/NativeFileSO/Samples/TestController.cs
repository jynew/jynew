// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Keiwando.NFSO.Samples {

	public class TestController : MonoBehaviour {

		[SerializeField]
		private Button buttonTemplate;

		[SerializeField]
		private Text textField;

#if UNITY_STANDALONE_WIN
	private const string testDirectory = @"C:\Users";
#elif UNITY_STANDALONE_OSX
	private const string testDirectory = @"~/Desktop";
#else
		private const string testDirectory = "";
#endif

		private const string testTitle = "Custom Title";

		private SupportedFileType[] testTypes = SupportedFilePreferences.supportedFileTypes;

		private System.Threading.Thread mainThread;

		void Start() {

			mainThread = System.Threading.Thread.CurrentThread;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
			SetupDesktop();
#elif UNITY_IOS || UNITY_ANDROID
			SetupMobile();
#endif

			NativeFileSOMobile.shared.FilesWereOpened += delegate (OpenedFile[] files) {

				ShowContents(files);
			};
		}

		private void SetupMobile() {

			buttonTemplate.gameObject.SetActive(true);

			Debug.Log("Setting up Mobile");

			var shareButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(shareButton, "Share");
			shareButton.onClick.AddListener(delegate () {
				SaveFileTest();
			});

			var openButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openButton, "Open One");
			openButton.onClick.AddListener(delegate () {
				OpenSingleFileTest();
			});

			var openMultipleButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openMultipleButton, "Open Multiple");
			openMultipleButton.onClick.AddListener(delegate () {
				OpenMultipleTest();
			});

			buttonTemplate.gameObject.SetActive(false);
		}

		private void SetupDesktop() {

			buttonTemplate.gameObject.SetActive(true);

			Debug.Log("Setting up Desktop");

			var shareButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(shareButton, "Save");
			shareButton.onClick.AddListener(delegate () {
				SaveFileTest();
			});

			var openButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openButton, "Open One");
			openButton.onClick.AddListener(delegate () {
				OpenSingleFileTest();
			});

			var openMultipleButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openMultipleButton, "Open Multiple");
			openMultipleButton.onClick.AddListener(delegate () {
				OpenMultipleTest();
			});

			// Additional DesktopAPI
			var openMultipleDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openMultipleDesktopButton, "Open files with title and dir.");
			openMultipleDesktopButton.onClick.AddListener(delegate () {
				OpenFilesDesktopTest();
			});

			var openMultipleSyncDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openMultipleSyncDesktopButton, "Open files with title and dir. (sync)");
			openMultipleSyncDesktopButton.onClick.AddListener(delegate () {
				OpenFilesDesktopSyncTest();
			});

			var openPathsDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openPathsDesktopButton, "Select open paths");
			openPathsDesktopButton.onClick.AddListener(delegate () {
				OpenPathsDesktopTest();
			});

			var openPathsSyncDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(openPathsSyncDesktopButton, "Select open paths (sync)");
			openPathsSyncDesktopButton.onClick.AddListener(delegate () {
				OpenPathsDesktopSyncTest();
			});

			var saveTitleDirectoryDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(saveTitleDirectoryDesktopButton, "Save File with title and dir.");
			saveTitleDirectoryDesktopButton.onClick.AddListener(delegate () {
				SaveTitleDirectoryDesktopTest();
			});

			var savePathDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(savePathDesktopButton, "Select save path");
			savePathDesktopButton.onClick.AddListener(delegate () {
				SavePathDesktopTest();
			});

			var savePathSyncDesktopButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
			SetButtonTitle(savePathSyncDesktopButton, "Select save path (sync)");
			savePathSyncDesktopButton.onClick.AddListener(delegate () {
				SavePathDesktopSyncTest();
			});

			buttonTemplate.gameObject.SetActive(false);
		}

		private void SetButtonTitle(Button button, string title) {

			button.GetComponentInChildren<Text>().text = title;
		}

		private void SaveFileTest() {
			
			FileWriter.WriteTestFile(Application.persistentDataPath);
			NativeFileSO.shared.SaveFile(GetFileToSave());
			//FileWriter.DeleteTestFile(Application.persistentDataPath);
		}

		private void OpenSingleFileTest() {

			NativeFileSO.shared.OpenFile(testTypes, delegate (bool wasFileOpened, OpenedFile file) {
				if (wasFileOpened) {
					ShowContents(file);
				}
			});
		}

		private void OpenMultipleTest() {

			NativeFileSO.shared.OpenFiles(testTypes, delegate (bool wereFilesOpened, OpenedFile[] files) {
				if (wereFilesOpened) {
					ShowContents(files);
				}
			});
		}

		private void OpenFilesDesktopTest() {
			NativeFileSOMacWin.shared.OpenFiles(testTypes, true,
				testTitle, testDirectory, delegate (bool wereFilesSelected, OpenedFile[] files) {
					if (wereFilesSelected) {
						ShowContents(files);
					} else {
						textField.text = "File selection was cancelled.";
					}
				});
		}

		private void OpenFilesDesktopSyncTest() {

			var files = NativeFileSOMacWin.shared.OpenFilesSync(testTypes, true, testTitle, testDirectory);

			if (files.Length > 0) {
				ShowContents(files);
			} else {
				textField.text = "File selection was cancelled.";
			}
		}

		private void OpenPathsDesktopTest() {
			NativeFileSOMacWin.shared.SelectOpenPaths(testTypes, true,
			  testTitle, testDirectory, delegate (bool werePathsSelected, string[] paths) {
				  if (werePathsSelected) {
					  textField.text = string.Format("Selected paths:\n{0}", string.Join("\n", paths));
				  } else {
					  textField.text = "Path selection was cancelled.";
				  }
			  });
		}

		/// <summary>
		/// Note: No custom directory : Should remember previous directory
		/// </summary>
		private void OpenPathsDesktopSyncTest() {
			var paths = NativeFileSOMacWin.shared.SelectOpenPathsSync(testTypes, true, testTitle, null);
			if (paths.Length > 0) {
				textField.text = string.Format("Selected paths:\n{0}", string.Join("\n", paths));
			} else {
				textField.text = "Path selection was cancelled.";
			}
		}

		private void SaveTitleDirectoryDesktopTest() {

			FileWriter.WriteTestFile(Application.persistentDataPath);
			NativeFileSOMacWin.shared.SaveFile(GetFileToSave(), testTitle, testDirectory);
			// FileWriter.DeleteTestFile(Application.persistentDataPath);
		}

		private void SavePathDesktopTest() {
			
			NativeFileSOMacWin.shared.SelectSavePath(GetFileToSave(), testTitle, testDirectory, delegate (bool didSelectPath, string savePath) {
				if (didSelectPath) {
					textField.text = string.Format("Selected paths:\n{0}", savePath);
				} else {
					textField.text = "Path selection was cancelled.";
				}
			});
		}

		private void SavePathDesktopSyncTest() {
			var path = NativeFileSOMacWin.shared.SelectSavePathSync(GetFileToSave(), "Save Path Sync", testDirectory);
			if (path != null) {
				textField.text = string.Format("Selected paths:\n{0}", path);
			} else {
				textField.text = "Path selection was cancelled.";
			}
		}


		private FileToSave GetFileToSave() {
			var testFilePath = Path.Combine(Application.persistentDataPath, "NativeFileSOTest.txt");
			return new FileToSave(testFilePath, "NativeFileSOTest.txt", SupportedFileType.PlainText);
		}

		private void ShowContents(OpenedFile[] files) {
			if (files.Length == 1) {
				ShowContents(files[0]);
				return;
			}

			var fileContents = files.Select(delegate (OpenedFile file) {
				return file.Data.Length > 100000 ? "File > 0.1MB ... contents not shown" : file.ToUTF8String();
			}).ToArray();
			string output = string.Join("\n", fileContents);

			Debug.Log("D / Plugin DEBUG: " + output);
			textField.text = output;
		}

		private void ShowContents(OpenedFile file) {

			string contents = file.Data.Length > 100000 ? "File > 0.1MB ... contents not shown" : file.ToUTF8String();

			string output = string.Format("File Contents: \n{0}\n --- EOF ---\n{1} bytes\n{2}\n{3}",
									   contents, file.Data.Length,
									   file.Name, file.Extension);

			Debug.Log("D/Plugin DEBUG: " + output);
			textField.text = output;
		}

		private bool IsOnMainThread() {
			return System.Threading.Thread.CurrentThread == mainThread;
		}
	}
}


