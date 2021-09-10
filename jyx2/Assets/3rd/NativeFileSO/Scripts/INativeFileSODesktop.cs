// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
namespace Keiwando.NFSO {

	public interface INativeFileSODesktop: INativeFileSO {

		void OpenFiles(SupportedFileType[] fileTypes, 
		               bool canSelectMultiple,
					   string title, 
		               string directory, 
					   Action<bool, OpenedFile[]> onCompletion);

		OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, 
		                           bool canSelectMultiple, 
								   string title, 
		                           string directory);

		void SelectOpenPaths(SupportedFileType[] fileTypes, 
		                     bool canSelectMultiple,
							 string title, 
		                     string directory,
							 Action<bool, string[]> onCompletion);

		string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, 
		                             bool canSelectMultiple,
									 string title, 
		                             string directory);

		void SaveFile(FileToSave file, string title, string directory);

		void SelectSavePath(FileToSave file,
							string title, 
		                    string directory,
							Action<bool, string> onCompletion);

		string SelectSavePathSync(FileToSave file,
								  string title, 
		                          string directory);
	}
}
