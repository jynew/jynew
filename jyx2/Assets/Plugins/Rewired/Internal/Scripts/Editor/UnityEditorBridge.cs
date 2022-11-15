// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0414

namespace Rewired.Editor {
    using UnityEditor;
    using Rewired.Platforms;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class UnityEditorBridge {

        private static EditorPlatform editorPlatform;
        private static Platform platform;
        private static WebplayerPlatform webplayerPlatform;
        private static bool isEditor;

        private static void Initialize() {
            DeterminePlatform();
        }

        private static void DeterminePlatform() {
            editorPlatform = EditorPlatform.None;
            platform = Platform.Unknown;
            webplayerPlatform = WebplayerPlatform.None;
            isEditor = false;

            // Find out the platform
#if UNITY_EDITOR
            isEditor = true;
            editorPlatform = EditorPlatform.Unknown;
#endif

#if UNITY_EDITOR_WIN
            editorPlatform = EditorPlatform.Windows;
#endif

#if UNITY_EDITOR_OSX
            editorPlatform = EditorPlatform.OSX;
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

#if UNITY_STANDALONE

#endif

#if UNITY_ANDROID
            platform = Platform.Android;
#endif

#if UNITY_BLACKBERRY
            platform = Platform.Blackberry;
#endif

#if UNITY_WP8
            platform = Platform.WindowsPhone8;
#endif

#if UNITY_IPHONE
            platform = Platform.iOS;
#endif

#if UNITY_IOS
            platform = Platform.iOS;
#endif

#if UNITY_PS3
            platform = Platform.PS3;
#endif

#if UNITY_PS4
            platform = Platform.PS4;
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

#if UNITY_XBOXONE
            platform = Platform.XboxOne;
#endif

#if UNITY_WII
            platform = Platform.Wii;
#endif

#if UNITY_WIIU
            platform = Platform.WiiU;
#endif

#if UNITY_FLASH
            platform = Platform.Flash;
#endif

#if UNITY_METRO
            platform = Platform.WindowsAppStore;
#endif

#if UNITY_WINRT

#endif

#if UNITY_WEBGL
            platform = Platform.WebGL;
#endif

            // Check if Webplayer
#if UNITY_WEBPLAYER

            webplayerPlatform = UnityTools.DetermineWebplayerPlatformType(platform, editorPlatform); // call this BEFORE you change the platform so we still know what base system this is
            platform = Platform.Webplayer;

#endif

        }
    }
}