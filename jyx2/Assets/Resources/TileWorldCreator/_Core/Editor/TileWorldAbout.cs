/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Create awesome tile worlds in seconds.
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using TileWorld;

//Disable Log warnings for assigned variables but not used
#pragma warning disable 0219

namespace TileWorld
{
	public class TileWorldAbout : EditorWindow {
		
		string version = "2.0.3";
		string doc = "http://tileworldcreator.doorfortyfour.com/documentation";
        string api = "http://tileworldcreator.doorfortyfour.com/api.html";
		string web = "http://tileworldcreator.doorfortyfour.com";
		string mail = "mailto:mail@doorfortyfour.com";

        string ppfxUrl = "http://u3d.as/8FM";
        string savemeUrl = "http://u3d.as/5mc";

		Texture2D logo;
        Texture2D iconPPFX;
        Texture2D iconSaveme;

        string changelog = "";
        Vector2 scrollPos = new Vector2(0, 0);

		public static void InitAbout () {
            // Get existing open window or if none, make a new one:
            TileWorldAbout window = (TileWorldAbout)EditorWindow.GetWindow (typeof (TileWorldAbout));
		}
		
		void OnEnable()
		{
			minSize = new Vector2(300f, 555f);
			maxSize = new Vector2(300f, 555.1f);

            var _path = ReturnInstallPath.GetInstallPath("Editor", this);
			logo = AssetDatabase.LoadAssetAtPath(_path + "Res/topLogo.png",typeof(Texture2D)) as Texture2D;
            iconPPFX = AssetDatabase.LoadAssetAtPath(_path + "Res/ppfx_icon.png", typeof(Texture2D)) as Texture2D;
            iconSaveme = AssetDatabase.LoadAssetAtPath(_path + "Res/saveme_icon.png", typeof(Texture2D)) as Texture2D;


            //load changelog
            var _changelogPath = Path.Combine(_path.Substring(0, _path.Length - 6), "changelog.txt");
            var changelogAsset = AssetDatabase.LoadAssetAtPath(_changelogPath, typeof(TextAsset)) as TextAsset;
            changelog = changelogAsset.text;
        }
		
		void OnGUI () {
			GUILayout.Label(logo);
			
			GUILayout.BeginHorizontal("Box");
			GUILayout.Label ("Thank you for purchasing TileWorldCreator" + "\n" +
				"(c) 2015 doorfortyfour OG" + "\n" +
				"version: " + version);
			GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            GUILayout.TextArea(changelog);
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Documentation"))
			{
				Application.OpenURL(doc);
			}
            //if (GUILayout.Button("API"))
            //{
            //    Application.OpenURL(api);
            //}
			if (GUILayout.Button("Support"))
			{
				Application.OpenURL(mail);
			}
			if (GUILayout.Button("Website"))
			{
				Application.OpenURL(web);
			}
			GUILayout.Space(5);

           
            GUILayout.BeginVertical("Box");
            GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(100);
                    if (GUILayout.Button(iconPPFX, GUILayout.Width(80), GUILayout.Height(80)))
                    {
                        Application.OpenURL(ppfxUrl);
                    }
                    //GUILayout.Space(20);
                    //if (GUILayout.Button(iconSaveme, GUILayout.Width(80), GUILayout.Height(80)))
                    //{
                    //    Application.OpenURL(savemeUrl);
                    //}
                    
                GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();

			if (GUILayout.Button("Close", GUILayout.Height(30)))
			{
				this.Close();
			}
		}
	}
}
#endif