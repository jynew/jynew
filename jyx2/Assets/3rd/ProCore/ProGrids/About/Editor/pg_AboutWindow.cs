using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

/**
 * INSTRUCTIONS
 *
 *  - Only modify properties in the USER SETTINGS region.
 *	- All content is loaded from external files (pc_AboutEntry_YourProduct.  Use the templates!
 */

/**
 * Used to pop up the window on import.
 */
public class pg_AboutWindowSetup : AssetPostprocessor
{
#region Initialization

	static void OnPostprocessAllAssets (
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{

		string[] entries = System.Array.FindAll(importedAssets, name => name.Contains("pc_AboutEntry") && !name.EndsWith(".meta"));
		
		foreach(string str in entries)
			if( pg_AboutWindow.Init(str, false) )
				break;
	}

	// [MenuItem("Edit/Preferences/Clear About Version: " + AboutWindow.PRODUCT_IDENTIFIER)]
	// public static void MenuClearVersionPref()
	// {
	// 	EditorPrefs.DeleteKey(AboutWindow.PRODUCT_IDENTIFIER);
	// }
#endregion
}

public class pg_AboutWindow : EditorWindow
{

/**
 * Modify these constants to customize about screen.
 */
#region User Settings

	 /* Path to the root folder */
	const string ABOUT_ROOT = "Assets/ProCore/ProGrids/About";
	
	/**
	 * Changelog.txt file should follow this format:
	 *
	 *	| -- Product Name 2.1.0 -
	 *	|
	 *	| # Features
	 *	| 	- All kinds of awesome stuff
	 *	| 	- New flux capacitor design achieves time travel at lower velocities.
	 *	| 	- Dark matter reactor recalibrated.
	 *	| 
	 *	| # Bug Fixes
	 *	| 	- No longer explodes when spacebar is pressed.
	 *	| 	- Fix rolling issue in Rickmeter.
	 *	| 	
	 *	| # Changes
	 *	| 	- Changed Blue to Red.
	 *	| 	- Enter key now causes explosions.
	 *
	 * This path is relative to the PRODUCT_ROOT path.
	 *
	 * Note that your changelog may contain multiple entries.  Only the top-most
	 * entry will be displayed.
	 */

	/**
	 * Advertisement thumb constructor is:
	 * new AdvertisementThumb( PathToAdImage : string, URLToPurchase : string, ProductDescription : string )
	 * Provide as many or few (or none) as desired.
	 *
	 * Notes - The http:// part is required.  Partial URLs do not work on Mac.
	 */
	[SerializeField]
	public static AdvertisementThumb[] advertisements = new AdvertisementThumb[] {
		new AdvertisementThumb( ABOUT_ROOT + "/Images/ProBuilder_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/probuilder/", "Build and Texture Geometry In-Editor"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/ProGrids_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/progrids/", "True Grids and Grid-Snapping"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/ProGroups_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/progroups/", "Hide, Freeze, Group, & Organize"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/Prototype_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/prototype/", "Design and Build With Zero Lag"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/QuickBrush_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/quickbrush/", "Quickly Add Detail Geometry"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/QuickDecals_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/quickdecals/", "Add Dirt, Splatters, Posters, etc"),
		new AdvertisementThumb( ABOUT_ROOT + "/Images/QuickEdit_AssetStore_Icon_96px.png", "http://www.protoolsforunity3d.com/quickedit/", "Edit Imported Meshes!"),
	};
#endregion	

/* Recommend you do not modify these. */
#region Private Fields (automatically populated)

	private string AboutEntryPath = "";

	private string ProductName = "";
	// private string ProductIdentifer = "";
	private string ProductVersion = "";
	private string ChangelogPath = "";
	private string BannerPath = ABOUT_ROOT + "/Images/Banner.png";

	
	const int AD_HEIGHT = 96;

	/**
	 * Struct containing data for use in Advertisement shelf.
	 */
	[System.Serializable]
	public struct AdvertisementThumb
	{
		public Texture2D image;
		public string url;
		public string about;
		public GUIContent guiContent;

		public AdvertisementThumb(string imagePath, string url, string about)
		{
			guiContent = new GUIContent("", about);
			this.image = (Texture2D) AssetDatabase.LoadAssetAtPath(imagePath, typeof(Texture2D));
			guiContent.image = this.image;
			this.url = url;
			this.about = about;
		}
	}

	Texture2D banner;

	// populated by first entry in changelog
	string changelog = "";
#endregion

#region Init

	// [MenuItem("Tools/Test Search About Window", false, 0)]
	// public static void MenuInit()
	// {
	// 	// this could be slow in large projects?
	// 	string[] allFiles = System.IO.Directory.GetFiles("Assets/", "*.*", System.IO.SearchOption.AllDirectories);
	// 	string[] entries = System.Array.FindAll(allFiles, name => name.Contains("pc_AboutEntry"));
		
	// 	if(entries.Length > 0)
	// 		AboutWindow.Init(entries[0], true);
	// }

	/**
	 * Return true if Init took place, false if not.
	 */
	public static bool Init (string aboutEntryPath, bool fromMenu)
	{
		string identifier, version;

		if( !GetField(aboutEntryPath, "version: ", out version) || !GetField(aboutEntryPath, "identifier: ", out identifier))
			return false;

		if(fromMenu || EditorPrefs.GetString(identifier) != version)
		{
			string tname;

			pg_AboutWindow win;

			if(!GetField(aboutEntryPath, "name: ", out tname) || !tname.Contains("ProGrids"))
				return false;

			win = (pg_AboutWindow)EditorWindow.GetWindow(typeof(pg_AboutWindow), true, tname, true);
			win.SetAboutEntryPath(aboutEntryPath);
			win.ShowUtility();

			EditorPrefs.SetString(identifier, version);

			return true;
		}
		else
		{
			return false;
		}
	}

	public void OnEnable()
	{
		banner = (Texture2D)AssetDatabase.LoadAssetAtPath(BannerPath, typeof(Texture2D));

		// With Unity 4 (on PC) if you have different values for minSize and maxSize,
		// they do not apply restrictions to window size.
		this.minSize = new Vector2(banner.width + 12, banner.height * 7);
		this.maxSize = new Vector2(banner.width + 12, banner.height * 7);
	}

	public void SetAboutEntryPath(string path)
	{
		AboutEntryPath = path;
		PopulateDataFields(AboutEntryPath);
	}
#endregion

#region GUI

	Color LinkColor = new Color(0f, .682f, .937f, 1f);

	GUIStyle 	boldTextStyle,
				headerTextStyle,
				linkTextStyle;

	GUIStyle advertisementStyle;
	Vector2 scroll = Vector2.zero, adScroll = Vector2.zero;
	// int mm = 32;
	void OnGUI()
	{
		headerTextStyle = headerTextStyle ?? new GUIStyle( EditorStyles.boldLabel );//GUI.skin.label);
		headerTextStyle.fontSize = 16;
		
		linkTextStyle = linkTextStyle ?? new GUIStyle( GUI.skin.label );//GUI.skin.label);
		linkTextStyle.normal.textColor = LinkColor; 
		linkTextStyle.alignment = TextAnchor.MiddleLeft;

		boldTextStyle = boldTextStyle ?? new GUIStyle( GUI.skin.label );//GUI.skin.label);
		boldTextStyle.fontStyle = FontStyle.Bold;
		boldTextStyle.alignment = TextAnchor.MiddleLeft;

		// #if UNITY_4
		// richTextLabel.richText = true;
		// #endif

		advertisementStyle = advertisementStyle ?? new GUIStyle(GUI.skin.button);
		advertisementStyle.normal.background = null;
		
		if(banner != null)
			GUILayout.Label(banner);

		// mm = EditorGUI.IntField(new Rect(Screen.width - 200, 100, 200, 18), "W: ", mm);

		{
			GUILayout.Label("Thank you for purchasing " + ProductName + ". Your support allows us to keep developing this and future tools for everyone.", EditorStyles.wordWrappedLabel);
			GUILayout.Space(2);
			GUILayout.Label("Read these quick \"ProTips\" before starting:", headerTextStyle);
			
			GUILayout.BeginHorizontal();
				GUILayout.Label("1) ", GUILayout.MinWidth(16), GUILayout.MaxWidth(16));	
				GUILayout.Label("Register", boldTextStyle, GUILayout.MinWidth(58), GUILayout.MaxWidth(58));	
				GUILayout.Label("for instant email updates, send your invoice # to", GUILayout.MinWidth(284), GUILayout.MaxWidth(284));	
				if( GUILayout.Button("contact@procore3d.com", linkTextStyle, GUILayout.MinWidth(142), GUILayout.MaxWidth(142)) )
					Application.OpenURL("mailto:contact@procore3d.com?subject=Sign me up for the Beta!");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("2) ", GUILayout.MinWidth(16), GUILayout.MaxWidth(16));	
				GUILayout.Label("Report bugs", boldTextStyle, GUILayout.MinWidth(82), GUILayout.MaxWidth(82));	
				GUILayout.Label("to the ProCore Forum at", GUILayout.MinWidth(144), GUILayout.MaxWidth(144));	
				if( GUILayout.Button("www.procore3d.com/forum", linkTextStyle, GUILayout.MinWidth(162), GUILayout.MaxWidth(162)) )
					Application.OpenURL("http://www.procore3d.com/forum");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("3) ", GUILayout.MinWidth(16), GUILayout.MaxWidth(16));	
				GUILayout.Label("Customize!", boldTextStyle, GUILayout.MinWidth(74), GUILayout.MaxWidth(74));	
				GUILayout.Label("Click on \"Edit > Preferences\" then \"" + ProductName + "\"", GUILayout.MinWidth(276), GUILayout.MaxWidth(276));	
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("4) ", GUILayout.MinWidth(16), GUILayout.MaxWidth(16));	
				GUILayout.Label("Documentation", boldTextStyle, GUILayout.MinWidth(102), GUILayout.MaxWidth(102));	
				GUILayout.Label("Tutorials, & more info:", GUILayout.MinWidth(132), GUILayout.MaxWidth(132));	
				if( GUILayout.Button("www.procore3d.com/" + ProductName.ToLower(), linkTextStyle, GUILayout.MinWidth(190), GUILayout.MaxWidth(190)) )
					Application.OpenURL("http://www.procore3d.com/" + ProductName.ToLower());
			GUILayout.EndHorizontal();

			GUILayout.Space(4);

			GUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
				
				GUILayout.Label("Links:", boldTextStyle);

				linkTextStyle.fontStyle = FontStyle.Italic;
				linkTextStyle.alignment = TextAnchor.MiddleCenter;

				if( GUILayout.Button("procore3d.com", linkTextStyle))
					Application.OpenURL("http://www.procore3d.com");

				if( GUILayout.Button("facebook", linkTextStyle))
					Application.OpenURL("http://www.facebook.com/probuilder3d");

				if( GUILayout.Button("twitter", linkTextStyle))
					Application.OpenURL("http://www.twitter.com/probuilder3d");

				linkTextStyle.fontStyle = FontStyle.Normal;
			GUILayout.EndHorizontal();

			GUILayout.Space(4);
		}

		HorizontalLine();

		// always bold the first line (cause it's the version info stuff)
		scroll = EditorGUILayout.BeginScrollView(scroll);
		GUILayout.Label(ProductName + "  |  version: " + ProductVersion, EditorStyles.boldLabel);
		GUILayout.Label("\n" + changelog);
		EditorGUILayout.EndScrollView();
		
		HorizontalLine();
		
		GUILayout.Label("More ProCore Products", EditorStyles.boldLabel);

		int pad = advertisements.Length * AD_HEIGHT > Screen.width ? 22 : 6;
		adScroll = EditorGUILayout.BeginScrollView(adScroll, false, false, GUILayout.MinHeight(AD_HEIGHT + pad), GUILayout.MaxHeight(AD_HEIGHT + pad));
		GUILayout.BeginHorizontal();

		foreach(AdvertisementThumb ad in advertisements)
		{
			if(ad.url.ToLower().Contains(ProductName.ToLower()))
				continue;
				
			if(GUILayout.Button(ad.guiContent, advertisementStyle,
				GUILayout.MinWidth(AD_HEIGHT), GUILayout.MaxWidth(AD_HEIGHT),
				GUILayout.MinHeight(AD_HEIGHT), GUILayout.MaxHeight(AD_HEIGHT)))
			{
				Application.OpenURL(ad.url);
			}
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndScrollView();
		/* shill other products */
	}

	/**
	 * Draw a horizontal line across the screen and update the guilayout.
	 */
	void HorizontalLine()
	{
		Rect r = GUILayoutUtility.GetLastRect();
		Color og = GUI.backgroundColor;
		GUI.backgroundColor = Color.black;
		GUI.Box(new Rect(0f, r.y + r.height + 2, Screen.width, 2f), "");
		GUI.backgroundColor = og;

		GUILayout.Space(6);
	}
#endregion

#region Data Parsing

	/* rich text ain't wuurkin' in unity 3.5 */
	const string RemoveBraketsRegex = "(\\<.*?\\>)";

	/**
	 * Open VersionInfo and Changelog and pull out text to populate vars for OnGUI to display.
	 */
	void PopulateDataFields(string entryPath)
	{
		/* Get data from VersionInfo.txt */
		TextAsset versionInfo = (TextAsset)AssetDatabase.LoadAssetAtPath( entryPath, typeof(TextAsset));
		
		ProductName = "";
		// ProductIdentifer = "";
		ProductVersion = "";
		ChangelogPath = "";

		if(versionInfo != null)
		{
			string[] txt = versionInfo.text.Split('\n');
			foreach(string cheese in txt)
			{
				if(cheese.StartsWith("name:")) 
					ProductName = cheese.Replace("name: ", "").Trim();
				else 
				if(cheese.StartsWith("version:"))
					ProductVersion = cheese.Replace("version: ", "").Trim();
				else 
				if(cheese.StartsWith("changelog:"))
					ChangelogPath = cheese.Replace("changelog: ", "").Trim();
			}
		}

		// notes = notes.Trim();

		/* Get first entry in changelog.txt */
		TextAsset changelogText = (TextAsset)AssetDatabase.LoadAssetAtPath( ChangelogPath, typeof(TextAsset));

		if(changelogText)
		{
			string[] split = changelogText.text.Split( new string[] {"--"}, System.StringSplitOptions.RemoveEmptyEntries );
			StringBuilder sb = new StringBuilder();
			string[] newLineSplit = split[0].Trim().Split('\n');
			for(int i = 2; i < newLineSplit.Length; i++)
				sb.AppendLine(newLineSplit[i]);
			
			changelog = sb.ToString();
		}
	}

	private static bool GetField(string path, string field, out string value)
	{
		TextAsset entry = (TextAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
		value = "";

		if(!entry) return false;

		foreach(string str in entry.text.Split('\n'))
		{
			if(str.Contains(field))
			{
				value = str.Replace(field, "").Trim();
				return true;
			}
		}

		return false;		
	}
#endregion
}
