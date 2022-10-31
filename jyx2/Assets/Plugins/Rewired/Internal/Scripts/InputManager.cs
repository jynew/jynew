// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_5_4_PLUS
#define SUPPORTS_SCENE_MANAGEMENT
#endif

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired {

    using UnityEngine;
    using System.Collections.Generic;
    using Rewired.Platforms;
    using Rewired.Utils;
    using Rewired.Utils.Interfaces;
    using System;
#if SUPPORTS_SCENE_MANAGEMENT
    using UnityEngine.SceneManagement;
#endif

    [AddComponentMenu("Rewired/Input Manager")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class InputManager : InputManager_Base {

        private bool ignoreRecompile;

        protected override void OnInitialized() {
            SubscribeEvents();
        }

        protected override void OnDeinitialized() {
            UnsubscribeEvents();
        }

        protected override void DetectPlatform() {
            // Set the editor and platform versions

#if UNITY_EDITOR
            // Do not check for recompile if using "Recompile After Finish Playing" mode or Rewired will be disabled and never reinitialize due to a bug in EditorApplication.isCompiling
            ignoreRecompile = (ScriptChangesDuringPlayOptions)UnityEditor.EditorPrefs.GetInt("ScriptCompilationDuringPlay", 0) == ScriptChangesDuringPlayOptions.RecompileAfterFinishedPlaying;
#endif

            scriptingBackend = ScriptingBackend.Mono;
            scriptingAPILevel = ScriptingAPILevel.Net20;
            editorPlatform = EditorPlatform.None;
            platform = Platform.Unknown;
            webplayerPlatform = WebplayerPlatform.None;
            isEditor = false;
            string deviceName = SystemInfo.deviceName ?? string.Empty;
            string deviceModel = SystemInfo.deviceModel ?? string.Empty;

#if UNITY_EDITOR
            isEditor = true;
            editorPlatform = EditorPlatform.Unknown;
#endif

#if UNITY_EDITOR_WIN
            editorPlatform = EditorPlatform.Windows;
#endif

#if UNITY_EDITOR_LINUX
            editorPlatform = EditorPlatform.Linux;
#endif

#if UNITY_EDITOR_OSX
            editorPlatform = EditorPlatform.OSX;
#endif

#if UNITY_EDITOR && REWIRED_DEBUG_MOCK_BUILD_PLAYER
        Debug.LogWarning("Rewired is mocking the build player in the editor");
        isEditor = false;
        editorPlatform = EditorPlatform.None;
#endif

#if UNITY_STANDALONE_OSX
            platform = Platform.OSX;
#endif

#if UNITY_DASHBOARD_WIDGET

#endif

#if UNITY_STANDALONE_WIN
            platform = Platform.Windows;
#endif

#if UNITY_STANDALONE_LINUX
            platform = Platform.Linux;
#endif

#if UNITY_ANDROID
            platform = Platform.Android;
#if !UNITY_EDITOR
            // Handle special Android platforms
            if(CheckDeviceName("OUYA", deviceName, deviceModel)) {
                platform = Platform.Ouya;
            } else if(CheckDeviceName("Amazon AFT.*", deviceName, deviceModel)) {
                platform = Platform.AmazonFireTV;
            } else if(CheckDeviceName("razer Forge", deviceName, deviceModel)) {
#if REWIRED_OUYA && REWIRED_USE_OUYA_SDK_ON_FORGETV
                platform = Platform.Ouya;
#else
                platform = Platform.RazerForgeTV;
#endif
            }
#endif
#endif

#if UNITY_BLACKBERRY
            platform = Platform.Blackberry;
#endif

#if UNITY_IPHONE || UNITY_IOS
            platform = Platform.iOS;
#endif

#if UNITY_TVOS
            platform = Platform.tvOS;
#endif

#if UNITY_PS3
            platform = Platform.PS3;
#endif

#if UNITY_PS4
            platform = Platform.PS4;
#endif

#if UNITY_PS5
            platform = Platform.PS5;
#endif

#if UNITY_PSP2
            platform = Platform.PSVita;
#endif

#if UNITY_PSM
            platform = Platform.PSMobile;
#endif

#if UNITY_XBOX360
            platform = Platform.Xbox360;
#endif

#if UNITY_GAMECORE_XBOXONE
            platform = Platform.GameCoreXboxOne;
#elif UNITY_XBOXONE
            platform = Platform.XboxOne;
#endif

#if UNITY_GAMECORE_SCARLETT
            platform = Platform.GameCoreScarlett;
#endif

#if UNITY_WII
            platform = Platform.Wii;
#endif

#if UNITY_WIIU
            platform = Platform.WiiU;
#endif

#if UNITY_N3DS
            platform = Platform.N3DS;
#endif

#if UNITY_SWITCH
            platform = Platform.Switch;
#endif

#if UNITY_FLASH
            platform = Platform.Flash;
#endif

#if UNITY_METRO || UNITY_WSA || UNITY_WSA_8_0
            platform = Platform.WindowsAppStore;
#endif

#if UNITY_WSA_8_1
            platform = Platform.Windows81Store;
#endif

            // Windows 8.1 Universal
#if UNITY_WINRT_8_1 && !UNITY_WSA_8_1 // this seems to be the only way to detect this
    platform = Platform.Windows81Store;
#endif

            // Windows Phone overrides Windows Store -- this is not set when doing Universal 8.1 builds
#if UNITY_WP8 || UNITY_WP8_1 || UNITY_WP_8 || UNITY_WP_8_1 // documentation error on format of WP8 defines, so include both
            platform = Platform.WindowsPhone8;
#endif

#if UNITY_WSA_10_0
            platform = Platform.WindowsUWP;
#endif

#if UNITY_WEBGL
            platform = Platform.WebGL;
#endif

#if UNITY_STADIA
            platform = Platform.Stadia;
#endif

            // Check if Webplayer
#if UNITY_WEBPLAYER

            webplayerPlatform = UnityTools.DetermineWebplayerPlatformType(platform, editorPlatform); // call this BEFORE you change the platform so we still know what base system this is
            platform = Platform.Webplayer;

#endif

#if ENABLE_MONO
            scriptingBackend = ScriptingBackend.Mono;
#endif

#if ENABLE_DOTNET
            scriptingBackend = ScriptingBackend.DotNet;
#endif

#if ENABLE_IL2CPP
            scriptingBackend = ScriptingBackend.IL2CPP;
#endif

#if NET_2_0
            scriptingAPILevel = ScriptingAPILevel.Net20;
#endif

#if NET_2_0_SUBSET
            scriptingAPILevel = ScriptingAPILevel.Net20Subset;
#endif

#if NET_4_6
            scriptingAPILevel = ScriptingAPILevel.Net46;
#endif

#if NET_STANDARD_2_0
            scriptingAPILevel = ScriptingAPILevel.NetStandard20;
#endif
        }

        protected override void CheckRecompile() {
#if UNITY_EDITOR
            if(ignoreRecompile) return;

            // Destroy system if recompiling
            if(UnityEditor.EditorApplication.isCompiling) { // editor is recompiling
                if(!isCompiling) { // this is the first cycle of recompile
                    isCompiling = true; // flag it
                    RecompileStart();
                }
                return;
            }

            // Check for end of compile
            if(isCompiling) { // compiling is done
                isCompiling = false; // flag off
                RecompileEnd();
            }
#endif
        }

        protected override IExternalTools GetExternalTools() {
            return new ExternalTools();
        }

        private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel) {
            return System.Text.RegularExpressions.Regex.IsMatch(deviceName, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                System.Text.RegularExpressions.Regex.IsMatch(deviceModel, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private void SubscribeEvents() {
            UnsubscribeEvents();
#if SUPPORTS_SCENE_MANAGEMENT
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }

        private void UnsubscribeEvents() {
#if SUPPORTS_SCENE_MANAGEMENT
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }

#if SUPPORTS_SCENE_MANAGEMENT
      
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            OnSceneLoaded();
        }

#else
        private void OnLevelWasLoaded(int index) {
            OnSceneLoaded();
        }

#endif

#if UNITY_EDITOR
        private enum ScriptChangesDuringPlayOptions {
            RecompileAndContinuePlaying,
            RecompileAfterFinishedPlaying,
            StopPlayingAndRecompile
        }
#endif
    }
}