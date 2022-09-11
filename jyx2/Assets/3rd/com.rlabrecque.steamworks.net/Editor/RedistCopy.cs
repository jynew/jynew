// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using Steamworks;

public class RedistCopy {
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		// We only want to do this on Steam supported platforms.
		if ((target != BuildTarget.StandaloneWindows) && (target != BuildTarget.StandaloneWindows64) && (target != BuildTarget.StandaloneLinux64)) {
			return;
		}

		CopyDebugInfo(target, pathToBuiltProject);

		DeleteOldSteamApiDlls(target, pathToBuiltProject);
	}

	static void CopyDebugInfo(BuildTarget target, string pathToBuiltProject) {
		string baseDir = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), Path.GetFileNameWithoutExtension(pathToBuiltProject) + "_Data");
		string pluginsDir = Path.Combine(baseDir, "Plugins");

		// Create if it doesn't exist yet
		Directory.CreateDirectory(pluginsDir);

		string[] DebugInfo = {
			"Steamworks.NET created by Riley Labrecque",
			"http://steamworks.github.io",
			"",
			"Steamworks.NET Version: " + Steamworks.Version.SteamworksNETVersion,
			"Steamworks SDK Version: " + Steamworks.Version.SteamworksSDKVersion,
			"Steam API DLL Version:  " + Steamworks.Version.SteamAPIDLLVersion,
			"Steam API DLL Size:     " + Steamworks.Version.SteamAPIDLLSize,
			"Steam API64 DLL Size:   " + Steamworks.Version.SteamAPI64DLLSize,
			""
		};
		File.WriteAllLines(Path.Combine(pluginsDir, "Steamworks.NET.txt"), DebugInfo);
	}

	static void DeleteOldSteamApiDlls(BuildTarget target, string pathToBuiltProject) {
		string strDllPath = Path.Combine(pathToBuiltProject, "steam_api.dll");
		if (File.Exists(strDllPath)) {
			try {
				File.Delete(strDllPath);
			}
			catch (System.Exception e) {
				Debug.LogWarning($"[Steamworks.NET] Attempted to delete an old copy of 'steam_api.dll' in the following location: '{strDllPath}', but could not due to the following exception:");
				Debug.LogException(e);
			}
		}

		string strDll64Path = Path.Combine(pathToBuiltProject, "steam_api64.dll");
		if (File.Exists(strDll64Path)) {
			try {
				File.Delete(strDll64Path);
			}
			catch (System.Exception e) {
				Debug.LogWarning($"[Steamworks.NET] Attempted to delete an old copy of 'steam_api64.dll' in the following location: '{strDll64Path}', but could not due to the following exception:");
				Debug.LogException(e);
			}
		}
	}
}

#endif // !DISABLESTEAMWORKS
