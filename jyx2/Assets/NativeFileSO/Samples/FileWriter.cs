// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;

namespace Keiwando.NFSO.Samples { 

	public class FileWriter {

		private static readonly string NAME = "NativeFileSOTest.txt";

		public static void WriteTestFile(string path) {
			
			var fullPath = Path.Combine(path, NAME);
			var contents = "Native File SO Test file";

			File.WriteAllText(fullPath, contents);
		}

		public static void DeleteTestFile(string path) { 
			
			var fullPath = Path.Combine(path, NAME);
			File.Delete(fullPath);
		}
	}
}