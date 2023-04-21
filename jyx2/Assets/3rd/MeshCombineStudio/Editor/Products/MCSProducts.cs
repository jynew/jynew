using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace MeshCombineStudio
{
	public class MCSProducts
	{
        public Texture asIconSale, asIcon;
        public Texture mcsIcon;
        public Texture mcsCavesIcon;
        public Texture deIcon;
        public Texture tcIcon;
        public Texture wcIcon;
        public Texture saleIcon;
        
        public bool mcsInProject, mcsCavesInProject, deInProject, tcInProject, wcInProject;
        public bool showAs, hasSale, mcsSale, mcsCavesSale, deSale, tcSale, wcSale;
        public string saleText, mcsSaleText, mcsCavesSaleText, deSaleText, tcSaleText, wcSaleText;
        public string asUrl;
        public int waitSecondsForRecheck;

        float scrollBarX;
        UnityWebRequest www;

        public MCSProducts()
        {
            mcsInProject = (GetType("MeshCombineStudio.MeshCombiner") != null);
            mcsCavesInProject = (GetType("MeshCombineStudio.RemoveOverlappingTris") != null);
            deInProject = (GetType("DebuggingEssentials.RuntimeInspector") != null);
            tcInProject = (GetType("TerrainComposer2.TC_Generate") != null);
            wcInProject = (GetType("WorldComposer.terrain_area_class") != null);
        }

        public static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName + ", Assembly-CSharp");
			if (type != null) return type;

			type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
			return type;
		}

        void UpdateSale()
        {
            if (www == null)
            {
                EditorApplication.update -= UpdateSale;
                SetPlayerPrefs();
                return;
            }

            if (!www.isDone) return;

            EditorApplication.update -= UpdateSale;
#if UNITY_2020_2_OR_NEWER
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.result == UnityWebRequest.Result.DataProcessingError)
            {
                //Debug.Log(www.error);
                SetPlayerPrefs();
                return;
            }
#else
            if (www.isNetworkError || www.isHttpError)
            {
                //Debug.Log(www.error);
                SetPlayerPrefs();
                return;
            }
#endif
            string text = www.downloadHandler.text;
            string[] lines = text.Split('\n');

            // line 0 => seconds wait
            // line 1 => "No Sale" or `Sale Name`
            // line 2 => "AS__" or "AS_X" + AS URL

            int.TryParse(lines[0], out waitSecondsForRecheck);
            if (waitSecondsForRecheck < 10) waitSecondsForRecheck = 10;

            showAs = lines[2].Contains("AS__");
            if (showAs) asUrl = lines[2].Substring(5); 

            if (lines[1].Contains("No Sale"))
            {
                saleText = string.Empty;
                SetPlayerPrefs();
                return;
            }
            hasSale = true;
            saleText = lines[1];
            deSale = lines[3].Contains("DE_1");
            mcsSale = lines[4].Contains("MCS_1");
            mcsCavesSale = lines[5].Contains("MCSCaves_1");
            wcSale = lines[6].Contains("WC_1");
            tcSale = lines[7].Contains("TC_1");

            if (deSale) deSaleText = "50% Discount on Unity's " + saleText + " NOW!\n" + lines[3].Replace("DE_1 ", "") + "\n\n";
            if (mcsSale) mcsSaleText = "50% Discount on Unity's " + saleText + " NOW!\n" + lines[4].Replace("MCS_1 ", "") + "\n\n";
            if (mcsCavesSale) mcsCavesSaleText = "50% Discount on Unity's " + saleText + " NOW!\n" + lines[5].Replace("MCSCaves_1 ", "") + "\n\n";
            if (wcSale) wcSaleText = "50% Discount on Unity's " + saleText + " NOW!\n" + lines[6].Replace("WC_1 ", "") + "\n\n";
            if (tcSale) tcSaleText = "50% Discount on Unity's " + saleText + " NOW!\n" + lines[7].Replace("TC_1 ", "") + "\n\n";

            // Debug.Log(saleText + "de " + deSale + " mcs " + mcsSale + " mcsCaves " + mcsCavesSale + " wc " + wcSale + " tc " + tcSale);

            //for (int i = 0; i < lines.Length; i++)
            //{
            //    Debug.Log(lines[i]);
            //}
            SetPlayerPrefs();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        void SetPlayerPrefs()
        {
            PlayerPrefs.SetInt("PON_waitSecondsForRecheck", waitSecondsForRecheck);
            PlayerPrefs.SetString("PON_sale", saleText);
            PlayerPrefs.SetString("PON_deSale", deSaleText);
            PlayerPrefs.SetString("PON_mcsSale", mcsSaleText);
            PlayerPrefs.SetString("PON_mcsCavesSale", mcsCavesSaleText);
            PlayerPrefs.SetString("PON_tcSale", tcSaleText);
            PlayerPrefs.SetString("PON_wcSale", wcSaleText);
            PlayerPrefs.SetString("PON_asUrl", asUrl);
        }

        void GetPlayerPrefs()
        {
            saleText = PlayerPrefs.GetString("PON_sale"); hasSale = (saleText != string.Empty);
            deSaleText = PlayerPrefs.GetString("PON_deSale"); deSale = (deSaleText != string.Empty);
            mcsSaleText = PlayerPrefs.GetString("PON_mcsSale"); mcsSale = (mcsSaleText != string.Empty);
            mcsCavesSaleText = PlayerPrefs.GetString("PON_mcsCavesSale"); mcsCavesSale = (mcsCavesSaleText != string.Empty);
            tcSaleText = PlayerPrefs.GetString("PON_tcSale"); tcSale = (tcSaleText != string.Empty);
            wcSaleText = PlayerPrefs.GetString("PON_wcSale"); wcSale = (wcSaleText != string.Empty);
            asUrl = PlayerPrefs.GetString("PON_asUrl"); showAs = (asUrl != string.Empty);
        }

        void GetSale()
        {
            bool checkForSales = true;
#if UNITY_2022_1_OR_NEWER
            // TODO: Unity 2022.1 throws InvalidOperationException: Insecure connection not allowed because URL for checking sales is http
            //       need to update sale URL to https
            checkForSales = false;
#endif

            if (!checkForSales)
            {
                www = null;
                return;
            }

            DateTime lastDate;
            string lastDateText = PlayerPrefs.GetString("PON_LastSaleCheck");
            DateTime dateTime = DateTime.Now;

            if (lastDateText == string.Empty) lastDate = DateTime.Now;
            else
            {
                DateTime.TryParse(lastDateText, out lastDate);

                TimeSpan timeSpan = dateTime - lastDate;
                // Debug.Log(timeSpan.TotalSeconds);
                waitSecondsForRecheck = PlayerPrefs.GetInt("PON_waitSecondsForRecheck");

                if (timeSpan.TotalSeconds < waitSecondsForRecheck)
                {
                    GetPlayerPrefs();
                    return;
                }
            }
            PlayerPrefs.SetString("PON_LastSaleCheck", dateTime.ToString("", System.Globalization.CultureInfo.InvariantCulture));
            // Debug.Log("Get Sale");
            hasSale = false;
            www = UnityWebRequest.Get("http://www.terraincomposer.com/sales.html");
            try
            {
#if UNITY_2017_1
                www.Send();
#else
                www.SendWebRequest();
#endif

                EditorApplication.update -= UpdateSale;
                EditorApplication.update += UpdateSale;
            }
            catch
            {
                // Unity 2022.1 beta is throwing InvalidOperationException: Insecure connection not allowed
                // Ignore the exception
                // TODO: need to update sale URL to https

                www = null;
            }
        }

        string GetSaleText()
        {
            return "50% Discount on Unity's " + saleText + " NOW!";
        }

        public void Draw(MonoBehaviour monoBehaviour)
        {
            if (mcsCavesIcon == null || deIcon == null)
            {
                GetSale();
                string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(monoBehaviour)).Replace("Scripts/Mesh/MeshCombiner.cs", "Products/");
                // string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(monoBehaviour)).Replace("WindowManager/WindowManager.cs", "Products/");
                // string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(monoBehaviour)).Replace("Scripts/Terrain/TC_TerrainArea.cs", "Products/");

                asIcon = AssetDatabase.LoadAssetAtPath(path + "asIcon.png", typeof(Texture)) as Texture;
                asIconSale = AssetDatabase.LoadAssetAtPath(path + "asIconSale.png", typeof(Texture)) as Texture;
                mcsIcon = AssetDatabase.LoadAssetAtPath(path + "MCSIcon.jpg", typeof(Texture)) as Texture;
                mcsCavesIcon = AssetDatabase.LoadAssetAtPath(path + "MCSCavesIcon.jpg", typeof(Texture)) as Texture;
                deIcon = AssetDatabase.LoadAssetAtPath(path + "deIcon.jpg", typeof(Texture)) as Texture;
                tcIcon = AssetDatabase.LoadAssetAtPath(path + "tcIcon.jpg", typeof(Texture)) as Texture;
                wcIcon = AssetDatabase.LoadAssetAtPath(path + "wcIcon.jpg", typeof(Texture)) as Texture;
                saleIcon = AssetDatabase.LoadAssetAtPath(path + "saleIcon.png", typeof(Texture)) as Texture;
            }
             
            Rect rect1 = GUILayoutUtility.GetLastRect();

            Rect rect = new Rect(rect1.x, rect1.y, 104, 104);
            rect.x += 4;
            rect.y += 2;
            rect.x -= scrollBarX;
            float width = rect.width + 4;

            // Debug.Log(Screen.width + " " + (width * 4));
            float alpha = 0.45f;
            float active = 6.65f;

            if (showAs)
            {
                if (GUI.Button(rect, new GUIContent(string.Empty, (hasSale ? asIconSale : asIcon), (hasSale ? ("Unity is running the " + saleText + "!\n\n") : "") + "Click to go to the 'Asset Store'.")))
                {
                    Application.OpenURL(asUrl);
                }
                rect.x += width;
            }
            else active--;

            if (!mcsInProject) DrawMCS(ref rect, width, alpha);
            if (!mcsCavesInProject) DrawMCSCaves(ref rect, width, alpha);
            if (!deInProject) DrawDE(ref rect, width, alpha);
            if (!tcInProject) DrawTC(ref rect, width, alpha);
            if (!wcInProject) DrawWC(ref rect, width, alpha);

            if (mcsInProject) DrawMCS(ref rect, width, alpha);
            if (mcsCavesInProject) DrawMCSCaves(ref rect, width, alpha);
            if (deInProject) DrawDE(ref rect, width, alpha);
            if (tcInProject) DrawTC(ref rect, width, alpha);
            if (wcInProject) DrawWC(ref rect, width, alpha);

            float size = (rect.width * active) - Screen.width;

            GUILayout.Space(rect.width + 4);
            if (size > 0)
            {
                scrollBarX = GUILayout.HorizontalScrollbar(scrollBarX, 25, 0, size);
            }
            DrawSpacer(0, 5, 0);
        }

        void DrawMCS(ref Rect rect, float width, float alpha)
        {
            if (mcsInProject) GUI.color = new Color(1, 1, 1, alpha);
            if (GUI.Button(rect, new GUIContent(string.Empty, mcsIcon, mcsSaleText + "Click to go to 'Mesh Combine Studio 2'.\n\nMesh Combine Studio is an automatic grid cell based mesh combiner which can dramatically improve the performance of your game. MCS can give up to 20x better performance compared to Unity’s static batching, while giving a more smooth and stable FPS.\n\nWe use MCS grid cell based combining in our game DRONE for the modular building in our arena editor and pre - made arenas, without MCS our game wouldn't run.\n\nInstead of manually combining meshes, which is very tedious, MCS will do this automatically for you sorted in grid cells, and the performance improvements it gives cannot be achieved with manual combining. Just simply drag and drop a MCS prefab in your Scene and tweak the many available conditions to your specific needs and you are ready to go.")))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/modeling/mesh-combine-studio-2-101956?aid=1011le9dK&pubref=uMCS");
            }
            if (mcsSale)
            {
                GUI.DrawTexture(new Rect(rect.x + 10, rect.y + 20, rect.width - 20, (rect.width - 20) * 0.37068f), saleIcon);
            }
            rect.x += width;
        }

        void DrawMCSCaves(ref Rect rect, float width, float alpha)
        {
            if (mcsCavesInProject) GUI.color = new Color(1, 1, 1, alpha); else GUI.color = Color.white;
            if (GUI.Button(rect, new GUIContent(string.Empty, mcsCavesIcon, mcsCavesSaleText + "Click to go to 'MCS Caves & Overhangs' Extension.\n\nThis 'Mesh Combine Studio 2' extension gives the best performance possible by using the easiest way to get amazing looking Caves and Overhangs on any terrain solution.\n\nYou can use any Rock Asset pack from the Unity Asset Store and stack them together to create caves and overhangs, and this extension will combine all rocks and remove all inside polygons that are never visible. Resulting in a ~60-80% polygon removal + combining which gives unbeatable performance and lightmap texture reduction.\n\nOur arenas in DRONE are optimized with this MCS extension. It can also be used on any other kind of closed meshes like a level consisting out of snapped cubes.")))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/mcs-caves-overhangs-144413?aid=1011le9dK&pubref=uMCSCaves");
            }
            if (mcsCavesSale)
            {
                GUI.DrawTexture(new Rect(rect.x + 10, rect.y + 20, rect.width - 20, (rect.width - 20) * 0.37068f), saleIcon);
            }
            rect.x += width;
        }

        void DrawDE(ref Rect rect, float width, float alpha)
        {
            if (deInProject) GUI.color = new Color(1, 1, 1, alpha); else GUI.color = Color.white;
            if (GUI.Button(rect, new GUIContent(string.Empty, deIcon, deSaleText + "Click to go to 'Debugging Essentials'.\n\nDebugging Essentials contains 5 crucial tools which will save you tons of time needed for coding while avoiding debugging headaches, making developing a lot more enjoyable!\n\n* Runtime Hierarchy.\n* Runtime Deep Inspector.\n* Runtime Camera Navigation.\n* Runtime Console.\n* HTML Debug Logs.")))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/debugging-essentials-170773?aid=1011le9dK&pubref=uDE");
            }
            if (deSale)
            {
                GUI.DrawTexture(new Rect(rect.x + 15, rect.y + 8, rect.width - 30, (rect.width - 30) * 0.37068f), saleIcon);
            }
            rect.x += width;
        }

        void DrawTC(ref Rect rect, float width, float alpha)
        {
            if (tcInProject) GUI.color = new Color(1, 1, 1, alpha); else GUI.color = Color.white;
            if (GUI.Button(rect, new GUIContent(string.Empty, tcIcon, tcSaleText + "Click to go to 'Terrain Composer 2'.\n\nTerrainComposer2 is a powerful node based multi-terrain tile generator. TC2 makes use of the latest GPU technology to give you instant real-time results, which makes creating terrains faster and more easy than ever before.\n\nTC2 its folder like layer system and workflow is similar to that of Photoshop, which makes it possible to have full control and make quick changes any time during the workflow.")))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/terrain-composer-2-65563?aid=1011le9dK&pubref=uTC");
            }
            if (tcSale)
            {
                GUI.DrawTexture(new Rect(rect.x + 10, rect.y + 8, rect.width - 20, (rect.width - 20) * 0.37068f), saleIcon);
            }
            rect.x += width;
        }

        void DrawWC(ref Rect rect, float width, float alpha)
        {
            if (wcInProject) GUI.color = new Color(1, 1, 1, alpha); else GUI.color = Color.white;
            if (GUI.Button(rect, new GUIContent(string.Empty, wcIcon, wcSaleText + "Click to go to 'World Composer'.\n\nWorldComposer is a tool to extract heightmap data and satellite images from the real world. It uses Bing maps, like in the new Microsoft Flight Simulator 2020.\n\nWorldComposer can create terrains by itself, but the exported heightmaps(e.g. as stamps) and satellite images can also be used in other terrain tools like TerrainComposer 2, Gaia, WorldMachine, MapMapic, WorldCreator, etc.")))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/world-composer-13238?aid=1011le9dK&pubref=uWC");
            }
            if (wcSale)
            {
                GUI.DrawTexture(new Rect(rect.x + 10, rect.y + 8, rect.width - 20, (rect.width - 20) * 0.37068f), saleIcon);
            }
            rect.x += width;
            GUI.color = Color.white;
        }

        static public void DrawSpacer(float spaceBegin = 5, float height = 5, float spaceEnd = 5)
        {
            GUILayout.Space(spaceBegin - 1);
            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);
            GUILayout.Button(string.Empty, GUILayout.Height(height));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(spaceEnd - 1);

            GUI.color = Color.white;
        }
    }
}
