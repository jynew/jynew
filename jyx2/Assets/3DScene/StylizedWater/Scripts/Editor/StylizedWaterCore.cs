using UnityEngine;
using System.Collections;
using System.Net;
using UnityEditor;

namespace StylizedWater
{
    public class StylizedWaterCore : Editor
    {
        public const string ASSET_NAME = "Stylized Water Shader";
        public const string ASSET_ABRV = "SWS";
        public const string ASSET_ID = "71207";

        public const string INSTALLED_VERSION = "2.0.2";
        public const string MIN_UNITY_VERSION = "5.4";

        public static bool compatibleVersion = true;
        public static bool untestedVersion = false;

        public const string VERSION_FETCH_URL = "http://www.staggart.xyz/backend/versions/stylizedwater.php";
        public const string DOC_URL = "http://staggart.xyz/unity/stylized-water-shader/documentation/";
        public const string FORUM_URL = "http://forum.unity3d.com/threads/430118/";

        //Set to true if external version doesn't match this version
        //Set to false if installed version matches external version
        public static bool IsUpToDate
        {
            get { return EditorPrefs.GetBool(ASSET_ABRV + "_UPTODATE"); }
            set { EditorPrefs.SetBool(ASSET_ABRV + "_UPTODATE", value); }
        }

        public static void CheckUnityVersion()
        {
#if !UNITY_5_4_OR_NEWER
            compatibleVersion = false;
#endif
#if UNITY_2018_3_OR_NEWER
            compatibleVersion = false;
            untestedVersion = true;
#endif
        }

        public class RunOnImport : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (string str in importedAssets)
                {
                    if (str.Contains("StylizedWaterCore.cs"))
                    {
                        StylizedWaterCore.CheckUnityVersion();
                        VersionChecking.CheckForUpdate();

                        if (!compatibleVersion || untestedVersion)
                            SWS_Window.ShowWindow();
                    }
                }

            }
        }

        public static void OpenStorePage()
        {
            Application.OpenURL("com.unity3d.kharma:content/" + ASSET_ID);
        }

        public static class VersionChecking
        {
            public static string newVersion;

            //[MenuItem("Help/Stylized Water/Check for update")]
            public static void GetLatestVersionPopup()
            {
                CheckForUpdate();

                if (!IsUpToDate)
                {
                    if (EditorUtility.DisplayDialog(ASSET_NAME + ", version " + INSTALLED_VERSION, "A new version is available: " + newVersion, "Open store page", "Close"))
                    {
                        OpenStorePage();
                    }
                }
                else
                {
                    if (EditorUtility.DisplayDialog(ASSET_NAME + ", version " + INSTALLED_VERSION, "Your current version is up-to-date!", "Close")) { }
                }
            }


            public static void CheckForUpdate()
            {
                //Debug.Log("Checking for update...");

                WebClient webClient = new WebClient();
                try
                {
                    //Fetching latest version
                    newVersion = webClient.DownloadString(VERSION_FETCH_URL);
                }
                catch (WebException ex)
                {
                    Debug.LogWarning("[" + ASSET_NAME + "] Contacting update server failed: " + ex.Status);

                    //When failed, assume installation is up-to-date
                    newVersion = INSTALLED_VERSION;
                }

                //Success
                IsUpToDate = (INSTALLED_VERSION == newVersion) ? true : false;
            }

        }

    }//SWS Core class

    public class SWS_Window : EditorWindow
    {
        //Window properties
        private static int width = 440;
        private static int height = 500;

        //Tabs
        private bool isTabInstallation = true;
        private bool isTabDocumentation = true;
        private bool isTabSupport = false;

        [MenuItem("Help/Stylized Water", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow<SWS_Window>(true, "Help", true);
            editorWindow.titleContent = new GUIContent("Help");
            editorWindow.autoRepaintOnSceneChange = true;

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.width) / 2f, (Screen.height) / 2f, (width * 2), height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, 200);

            //Init();
            //StylizedWaterCore.VersionChecking.CheckForUpdate();

            StylizedWaterCore.CheckUnityVersion();

            editorWindow.Show();
        }

        private void SetWindowHeight(float height)
        {
            this.maxSize = new Vector2(width, height);
            this.minSize = new Vector2(width, height);
        }

        private void OnGUI()
        {
            //Can't use SessionState in 5.4, so needs to check every frame
            StylizedWaterCore.CheckUnityVersion();

            DrawHeader();

            GUILayout.Space(5);
            DrawTabs();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (isTabInstallation) DrawInstallation();

            if (isTabDocumentation) DrawDocumentation();

            if (isTabSupport) DrawSupport();

            //DrawActionButtons();

            EditorGUILayout.EndVertical();

            DrawFooter();
        }

        void DrawHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=24>" + StylizedWaterCore.ASSET_NAME + "</size>", Header);

            GUILayout.Label("Version: " + StylizedWaterCore.INSTALLED_VERSION, Footer);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

            Texture2D dot = null;
            if (StylizedWaterCore.compatibleVersion && !StylizedWaterCore.untestedVersion) dot = SmallGreenDot;
            else if (StylizedWaterCore.untestedVersion || !StylizedWaterCore.IsUpToDate) dot = SmallOrangeDot;
            else if (!StylizedWaterCore.compatibleVersion) dot = SmallRedDot;

            if (GUILayout.Toggle(isTabInstallation, new GUIContent("Installation", dot), Tab))
            {
                isTabInstallation = true;
                isTabDocumentation = false;
                isTabSupport = false;
            }

            if (GUILayout.Toggle(isTabDocumentation, "Documentation", Tab))
            {
                isTabInstallation = false;
                isTabDocumentation = true;
                isTabSupport = false;
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Toggle(isTabSupport, "Support", Tab))
            {
                isTabInstallation = false;
                isTabDocumentation = false;
                isTabSupport = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawInstallation()
        {
            SetWindowHeight(300f);

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent(" Compiling scripts...", EditorGUIUtility.FindTexture("cs Script Icon")), Header);

                EditorGUILayout.Space();
                return;
            }

            if (StylizedWaterCore.compatibleVersion == false && StylizedWaterCore.untestedVersion == false)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("This version of Unity is not supported.", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Please upgrade to at least Unity " + StylizedWaterCore.MIN_UNITY_VERSION);
                return;
            }

            //Folder
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Unity version");

            Color defaultColor = GUI.contentColor;
            if (StylizedWaterCore.compatibleVersion)
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Compatible");
                GUI.contentColor = defaultColor;
            }
            else if (StylizedWaterCore.untestedVersion)
            {
                GUI.contentColor = new Color(1f, 0.65f, 0f);
                EditorGUILayout.LabelField("Untested", EditorStyles.boldLabel);
                GUI.contentColor = defaultColor;
            }

            EditorGUILayout.EndHorizontal();
            if (StylizedWaterCore.untestedVersion)
            {
                EditorGUILayout.LabelField("The current Unity version has not been tested yet, or compatibility is being worked on. You may run into errors. \n\n" +
                    "Please view the forum thread for more information", EditorStyles.helpBox);
                EditorGUILayout.Space();
            }

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                //PPSv2
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
#if !UNITY_2018_1_OR_NEWER
                EditorGUILayout.LabelField("Target graphics API");
#else
            EditorGUILayout.LabelField("Post Processing");
#endif

                if (PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0] != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 || PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS)[0] != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2)
                {
                    GUI.contentColor = Color.green;
                    EditorGUILayout.LabelField("OpenGL ES 3.0 or better");
                    GUI.contentColor = defaultColor;
                }
                else
                {
                    GUI.contentColor = Color.red;
                    EditorGUILayout.LabelField("OpenGL ES 2.0", EditorStyles.boldLabel);
                    GUI.contentColor = defaultColor;
                }
                EditorGUILayout.EndHorizontal();
            }

            //Version
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Package version");

            defaultColor = GUI.contentColor;
            if (StylizedWaterCore.IsUpToDate)
            {

                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Up-to-date");
                GUI.contentColor = defaultColor;

            }
            else
            {
                GUILayout.FlexibleSpace();
                GUI.contentColor = new Color(1f, 0.65f, 0f);
                EditorGUILayout.LabelField("Outdated", EditorStyles.boldLabel);

                GUI.contentColor = defaultColor;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            if (!StylizedWaterCore.IsUpToDate)
            {
                if (GUILayout.Button(new GUIContent("Update package"), UpdateText))
                {
                    StylizedWaterCore.OpenStorePage();
                }
            }
            else
            {
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Check for update"))
                {
                    StylizedWaterCore.VersionChecking.GetLatestVersionPopup();
                }
            }
            EditorGUILayout.EndHorizontal();


        }

        void DrawDocumentation()
        {
            SetWindowHeight(335);

            EditorGUILayout.HelpBox("Please view the documentation for further details about this package and its workings.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b><size=12>Documentation</size></b>\n<i>Usage instructions</i>", Button))
            {
                Application.OpenURL(StylizedWaterCore.DOC_URL);
            }
            if (GUILayout.Button("<b><size=12>Troubleshooting</size></b>\n<i>Common issues and solutions</i>", Button))
            {
                Application.OpenURL(StylizedWaterCore.DOC_URL + "#troubleshooting");
            }
            EditorGUILayout.EndHorizontal();

        }

        void DrawSupport()
        {
            SetWindowHeight(350f);

            EditorGUILayout.BeginVertical(); //Support box

            EditorGUILayout.HelpBox("If you have any questions, or ran into issues, please get in touch.", MessageType.Info);

            EditorGUILayout.Space();

            //Buttons box
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<b><size=12>Email</size></b>\n<i>Contact</i>", Button))
            {
                Application.OpenURL("mailto:contact@staggart.xyz");
            }
            if (GUILayout.Button("<b><size=12>Twitter</size></b>\n<i>Follow developments</i>", Button))
            {
                Application.OpenURL("https://twitter.com/search?q=staggart%20creations");
            }
            if (GUILayout.Button("<b><size=12>Forum</size></b>\n<i>Join the discussion</i>", Button))
            {
                Application.OpenURL(StylizedWaterCore.FORUM_URL);
            }
            EditorGUILayout.EndHorizontal();//Buttons box

            EditorGUILayout.EndVertical(); //Support box
        }

        private void DrawFooter()
        {
            //EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.Label("- Staggart Creations -", Footer);
        }

        #region Styles
        private static Texture2D _smallGreenDot;
        public static Texture2D SmallGreenDot
        {
            get
            {
                if (_smallGreenDot == null)
                {
                    _smallGreenDot = EditorGUIUtility.FindTexture("d_winbtn_mac_max");
                }

                return _smallGreenDot;
            }
        }

        private static Texture2D _smallOrangeDot;
        public static Texture2D SmallOrangeDot
        {
            get
            {
                if (_smallOrangeDot == null)
                {
                    _smallOrangeDot = EditorGUIUtility.FindTexture("d_winbtn_mac_min");
                }

                return _smallOrangeDot;
            }
        }

        private static Texture2D _smallRedDot;
        public static Texture2D SmallRedDot
        {
            get
            {
                if (_smallRedDot == null)
                {
                    _smallRedDot = EditorGUIUtility.FindTexture("d_winbtn_mac_close_h");
                }

                return _smallRedDot;
            }
        }

        private static GUIStyle _UpdateText;
        public static GUIStyle UpdateText
        {
            get
            {
                if (_UpdateText == null)
                {
                    _UpdateText = new GUIStyle("Button")
                    {
                        //fontSize = 10,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = false,
                    };
                }

                return _UpdateText;
            }
        }

        private static GUIStyle _Footer;
        public static GUIStyle Footer
        {
            get
            {
                if (_Footer == null)
                {
                    _Footer = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _Footer;
            }
        }

        private static GUIStyle _Button;
        public static GUIStyle Button
        {
            get
            {
                if (_Button == null)
                {
                    _Button = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Button;
            }
        }

        private static GUIStyle _Header;
        public static GUIStyle Header
        {
            get
            {
                if (_Header == null)
                {
                    _Header = new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 18,
                        fontStyle = FontStyle.Normal
                    };
                }

                return _Header;
            }
        }

        private static GUIStyle _Tab;
        public static GUIStyle Tab
        {
            get
            {
                if (_Tab == null)
                {
                    _Tab = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Tab;
            }
        }
        #endregion

    }
}//Namespace
