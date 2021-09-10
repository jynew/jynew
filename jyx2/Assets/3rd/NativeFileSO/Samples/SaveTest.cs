// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Keiwando.NFSO.Samples {

	public class SaveTest {

		public static void Main() {

			string path = "path/to/existing/fileToSave.txt";
			string newFilename = "ExportedFile.txt";

			FileToSave file = new FileToSave(path, newFilename, SupportedFileType.PlainText);

			// Allows the user to choose a save location and saves the 
			// file to that location
			NativeFileSO.shared.SaveFile(file);
		}
	}
}
