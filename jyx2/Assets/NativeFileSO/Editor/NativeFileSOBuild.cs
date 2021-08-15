// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using Keiwando.NFSO;

public class NativeFileSOBuild {

	public int callbackOrder { get { return 0; } }

	[PostProcessBuildAttribute(1)]
	public static void OnPostProcessingBuild(BuildTarget target,
											 string pathToProject) {
		if (target == BuildTarget.iOS) {
			PostProcessIOS(pathToProject);
		}
	}

	private static void PostProcessIOS(string path) {

		if (SupportedFilePreferences.supportedFileTypes.Length == 0) {
			return;
		}

		Debug.Log("NativeFileSO: Adding associated file types");

		var pathToProject = PBXProject.GetPBXProjectPath(path);
		PBXProject project = new PBXProject();
		project.ReadFromFile(pathToProject);

		#if UNITY_2019_3_OR_NEWER
		var targetGUID = project.GetUnityFrameworkTargetGuid();
		#else
		var targetName = PBXProject.GetUnityTargetName();
		var targetGUID = project.TargetGuidByName(targetName);
		#endif

		AddFrameworks(project, targetGUID);
		project.WriteToFile(pathToProject);

		// Edit Plist
		var plistPath = Path.Combine(path, "Info.plist");
		var plist = new PlistDocument();
		plist.ReadFromFile(plistPath);
		var rootDict = plist.root;

		rootDict.SetBoolean("UISupportsDocumentBrowser", false);
		rootDict.SetBoolean("LSSupportsOpeningDocumentsInPlace", false);

		var documentTypesArray = rootDict.CreateArray("CFBundleDocumentTypes");

		var exportedTypesArray = rootDict.CreateArray("UTExportedTypeDeclarations");

		foreach (var supportedType in SupportedFilePreferences.supportedFileTypes) {

			var typesDict = documentTypesArray.AddDict();

			typesDict.SetString("CFBundleTypeName", supportedType.Name);
			typesDict.SetString("CFBundleTypeRole", "Viewer");
			typesDict.SetString("LSHandlerRank", supportedType.Owner ? "Owner" : "Alternate");

			var contentTypesArray = typesDict.CreateArray("LSItemContentTypes");
			foreach (var uti in supportedType.AppleUTI.Split('|')) {
				contentTypesArray.AddString(uti);
			}

			if (supportedType.Owner) {

				var exportedTypesDict = exportedTypesArray.AddDict();
				// Export the File Type

				if (!string.IsNullOrEmpty(supportedType.AppleConformsToUTI)) { 
					var conformsToArray = exportedTypesDict.CreateArray("UTTypeConformsTo");
					foreach (var conformanceUTI in supportedType.AppleConformsToUTI.Split('|')) {
						conformsToArray.AddString(conformanceUTI);
					}
				}

				exportedTypesDict.SetString("UTTypeDescription", supportedType.Name);
				exportedTypesDict.SetString("UTTypeIdentifier", supportedType.AppleUTI.Split('|')[0]);

				var tagSpecificationDict = exportedTypesDict.CreateDict("UTTypeTagSpecification");
				var tagSpecificationExtensions = tagSpecificationDict.CreateArray("public.filename-extension");
				foreach (var extension in supportedType.Extension.Split('|')) {
					tagSpecificationExtensions.AddString(extension);
				}
				tagSpecificationDict.SetString("public.mime-type", supportedType.MimeType);
			}
		}

		plist.WriteToFile(plistPath);
	}


	static void AddFrameworks(PBXProject project, string targetGUID) {

		// Based on eppz! (http://eppz.eu/blog/override-app-delegate-unity-ios-osx-1/)
		project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
	}

	[MenuItem("Tools/NativeFileSO/RefreshAndroidPlugin")]
	public static void UpdateAndroidPlugin() {

		var pluginFolder = CombinePaths(Application.dataPath, "Plugins", 
		                                "NativeFileSO", "Android");
		var aarPath = CombinePaths(pluginFolder, "NativeFileSO.aar");

		var manifestName = "AndroidManifest.xml";
		var manifestPath = CombinePaths(pluginFolder, manifestName);

		ZipStorer zip = ZipStorer.Open(aarPath, FileAccess.ReadWrite);

		var centralDir = zip.ReadCentralDir();
		var manifest = centralDir.Find(x => Path.GetFileName(x.FilenameInZip)
		                               == manifestName);

		zip.ExtractFile(manifest, manifestPath);
		UpdateManifestAssociations(manifestPath);

		ZipStorer.RemoveEntries(ref zip, new List<ZipStorer.ZipFileEntry>() { 
			manifest });
		zip.AddFile(ZipStorer.Compression.Deflate, manifestPath, manifest.FilenameInZip, "");

		zip.Close();

		File.Delete(manifestPath);

		Debug.Log("NativeFileSO: Finished updating the Android plugin");
	}

	private static void UpdateManifestAssociations(string manifestPath) { 
	
		var manifestContents = File.ReadAllText(manifestPath);

		var startTag = "<!-- #NativeFileSOIntentsStart# -->";
		var endTag = "<!-- #NativeFileSOIntentsEnd# -->";

		var intentFilters = new System.Text.StringBuilder(startTag);

		var mimeTypes = new HashSet<string>();
		foreach (var fileType in SupportedFilePreferences.supportedFileTypes) {
			if (!fileType.Owner) {
				mimeTypes.Add(fileType.MimeType);
			} else {
				intentFilters.Append(GetIntentForCustomFileType(fileType));
			}
		}

		foreach (var mimeType in mimeTypes) {
			intentFilters.Append(GetIntentForFileBrowser(mimeType));
		}
		intentFilters.Append(endTag);

		var pattern = string.Format("{0}.*{1}", startTag, endTag);
		manifestContents = Regex.Replace(manifestContents,
										 pattern,
										 intentFilters.ToString(),
										 RegexOptions.Singleline);


		File.WriteAllText(manifestPath, manifestContents);
	}

	private static string CombinePaths(params string[] paths) {

		var combined = "";

		foreach (var path in paths) {
			combined = Path.Combine(combined, path);
		}

		return combined;
	}

	private static string GetIntentForFileBrowser(string mimeType) {

		return string.Format(@"
			<intent-filter>
				<action android:name=""android.intent.action.VIEW""/> 
				<action android:name=""android.intent.action.EDIT""/>
				<action android:name=""android.intent.action.SEND""/> 
				<action android:name=""android.intent.action.SEND_MULTIPLE""/> 
				<category android:name=""android.intent.category.DEFAULT""/>
				<data android:mimeType=""{0}""/>
			</intent-filter>" 
         , mimeType);
	}

	private static string GetIntentForCustomFileType(SupportedFileType fileType) {

		return GetIntentForFileBrowser(fileType.MimeType);

		//return string.Format(@"
		//	<intent-filter>
		//		<action android:name=""android.intent.action.VIEW""/> 
		//		<action android:name=""android.intent.action.EDIT""/>
		//		<action android:name=""android.intent.action.SEND""/> 
		//		<action android:name=""android.intent.action.SEND_MULTIPLE""/>
		//		<category android:name=""android.intent.category.DEFAULT""/>
		//		<category android:name=""android.intent.category.BROWSABLE""/>
				
		//		<data android:mimeType=""*/*""/>
				
		//	</intent-filter>"    
		//, fileType.MimeType, fileType.Extension);
	}
}

