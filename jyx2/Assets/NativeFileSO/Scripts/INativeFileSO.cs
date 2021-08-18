// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace Keiwando.NFSO { 

	public interface INativeFileSO {

		void OpenFile(SupportedFileType[] supportedTypes, Action<bool, OpenedFile> onCompletion);
		void OpenFiles(SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion);

		void SaveFile(FileToSave file);
	}
}


