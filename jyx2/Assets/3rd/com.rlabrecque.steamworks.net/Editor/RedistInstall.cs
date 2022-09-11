// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// This copys various files into their required locations when Unity is launched to make installation a breeze.
[InitializeOnLoad]
public class RedistInstall {
	static RedistInstall() {
		WriteSteamAppIdTxtFile();
		AddDefineSymbols();
		CheckForOldDlls();
	}

	static void WriteSteamAppIdTxtFile() {
		string strCWDPath = Directory.GetCurrentDirectory();
		string strSteamAppIdPath = Path.Combine(strCWDPath, "steam_appid.txt");

		// If the steam_appid.txt file already exists, then there's nothing to do.
		if (File.Exists(strSteamAppIdPath)) {
			return;
		}

		Debug.Log("[Steamworks.NET] 'steam_appid.txt' is not present in the project root. Writing...");

		try {
			StreamWriter appIdFile = File.CreateText(strSteamAppIdPath);
			appIdFile.Write("480");
			appIdFile.Close();

			Debug.Log("[Steamworks.NET] Successfully copied 'steam_appid.txt' into the project root.");
		}
		catch (System.Exception e) {
			Debug.LogWarning("[Steamworks.NET] Could not copy 'steam_appid.txt' into the project root. Please place 'steam_appid.txt' into the project root manually.");
			Debug.LogException(e);
		}
	}

	static void CheckForOldDlls() {
		string strCwdPath = Directory.GetCurrentDirectory();

		// Unfortunately we can't just delete these outright because Unity loads the dlls in the project root instantly and Windows won't let us delete them because they are in use.

		string strDllPath = Path.Combine(strCwdPath, "steam_api.dll");
		if (File.Exists(strDllPath)) {
			Debug.LogError("[Steamworks.NET] Please delete the old version of 'steam_api.dll' in your project root before continuing.");
		}

		string strDll64Path = Path.Combine(strCwdPath, "steam_api64.dll");
		if (File.Exists(strDll64Path)) {
			Debug.LogError("[Steamworks.NET] Please delete the old version of 'steam_api64.dll' in your project root before continuing.");
		}
	}

	static void AddDefineSymbols() {
		string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
		HashSet<string> defines = new HashSet<string>(currentDefines.Split(';')) {
			"STEAMWORKS_NET"
		};

		string newDefines = string.Join(";", defines);
		if (newDefines != currentDefines) {
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
		}
	}
}
