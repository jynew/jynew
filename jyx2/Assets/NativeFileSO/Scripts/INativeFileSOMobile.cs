// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Keiwando.NFSO {

	public interface INativeFileSOMobile {
		
		void OpenFiles(SupportedFileType[] supportedTypes, bool canSelectMultiple);
		void SaveFile(FileToSave file);

		OpenedFile[] GetOpenedFiles();
	}
}
