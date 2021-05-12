//#define SHOW_NOISE_CATEGORY

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace EModules.FastWaterModel20 {

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(FastWaterModel20Controller))]
partial class FastWaterModel20ControllerEditor : Editor {


    new FastWaterModel20Controller target;
    // static bool? USE_LIST_GUI = null;
    GUIContent content = new GUIContent("Inspector Settings:");
    static Vector2? __scrl;
    static Vector2 scrl
    {   get { return __scrl ?? (__scrl = new Vector2(EditorPrefs.GetFloat("EModules/Water20/MatScrollX", 0), EditorPrefs.GetFloat("EModules/Water20/MatScrollY", 0))).Value; }
        set
        {   if (__scrl != value)
            {   EditorPrefs.SetFloat("EModules/Water20/MatScrollX", value.x);
                EditorPrefs.SetFloat("EModules/Water20/MatScrollY", value.y);
            }
            __scrl = value;
            
        }
    }
    static Dictionary<string, FastWaterModel20Compiler[]> cahced_mats = new Dictionary<string, FastWaterModel20Compiler[]>();
    static Dictionary<string, bool> cahced_path = new Dictionary<string, bool>();
    class PostFake : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {   cahced_mats.Clear();
        }
    }
    
    
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Create Water Target 2.0 Object", false, 10)]
    [MenuItem("GameObject/3D Object/EM-X/Water Target 2.0")]
    public static void CreateNewWater()
    {   CreateNewWater(null);
    }
    public static void CreateNewWater(MeshFilter inmf)
    {   var water = new GameObject("Water Plane");
        water.layer = LayerMask.GetMask("Water");
        water.AddComponent<MeshRenderer>();
        var mesh = new FastWaterModel20Controller.MeshData();
        var mf = water.AddComponent<MeshFilter>();
        if (inmf && inmf.sharedMesh)
        {   mesh = new FastWaterModel20Controller.MeshData(inmf.sharedMesh);
        }
        //else {
        mesh.WriteToMesh(mf, null);
        //}
        
        
        var MWM = water.AddComponent<FastWaterModel20Controller>();
        water.AddComponent<FastWaterModel20ManualBaking>();
        MWM.AssignMaterial();
        // MWM.DepthRenderShader = Shader.Find("Hidden/EM-X/DepthRender");
        // MWM.RefractionRenderShader = Shader.Find("Hidden/EM-X/RefractionRender");
        if (inmf)
        {   water.transform.SetParent(inmf.gameObject.transform.parent);
            water.transform.rotation = inmf.gameObject.transform.rotation;
            water.transform.localScale = inmf.gameObject.transform.localScale;
        }
        water.transform.position = SceneView.lastActiveSceneView.pivot;
        UnityEditor.Undo.RegisterCompleteObjectUndo(water, "Create Water");
        Selection.objects = new[] { (UnityEngine.Object)water };
        UnityEditorInternal.InternalEditorUtility.ExecuteCommandOnKeyWindow("GameObject/Move To View");
        
        
        //SceneView.FrameLastActiveSceneView();
        
    }
    
    
    
    static bool EditAlpha = false;
    static float EditAlphaAmount = 1;
    static void DisableEditAlpha()
    {   Selection.selectionChanged -= DisableEditAlpha;
        SceneView.onSceneGUIDelegate -= EditAlphaGUI;
        EditAlphaNowDraw = false;
        EditAlpha = false;
        EditAlphaGameObject = null;
        EditAlphaMf = null;
        EditAlphaMesh = null;
        if (EditAlphaMesh) EditorUtility.SetDirty(EditAlphaMesh);
    }
    static void EnableEditAlpha(GameObject target)
    {   if (!target)
        {   Debug.Log("Cannot EnableEditAlpha there's no assigned GameObject");
            return;
        }
        var mf = target.GetComponent<MeshFilter>();
        if (!mf)
        {   Debug.Log("Cannot EnableEditAlpha there's no assigned MeshFilter");
            return;
        }
        var mesh = mf.sharedMesh;
        if (!mesh)
        {   Debug.Log("Cannot EnableEditAlpha there's no assigned Mesh");
            return;
        }
        Selection.selectionChanged -= DisableEditAlpha;
        Selection.selectionChanged += DisableEditAlpha;
        SceneView.onSceneGUIDelegate -= EditAlphaGUI;
        SceneView.onSceneGUIDelegate += EditAlphaGUI;
        
        var inst = Resources.FindObjectsOfTypeAll<MeshFilter>().Count(r => r.sharedMesh == mesh);
        if (inst > 1)
        {   var m = FastWaterModel20Controller.CopyMesh(mesh);
            Undo.RecordObject(mf, "Edit vertices");
            mf.sharedMesh = m;
            FastWaterModel20Controller.SetDirty(mf);
            mesh = m;
        }
        
        EditAlphaGameObject = target;
        EditAlphaMf = mf;
        EditAlphaMesh = mesh;
    }
    static GameObject EditAlphaGameObject = null;
    static MeshFilter EditAlphaMf = null;
    static Mesh EditAlphaMesh = null;
    static bool EditAlphaNowDraw = false;
    static void EditAlphaGUI(SceneView sv)
    {   if (Event.current.rawType == EventType.MouseUp) EditAlphaNowDraw = false;
    
        if (!EditAlphaGameObject || !EditAlphaMf || !EditAlphaMesh)
        {   if (EditAlphaNowDraw)
            {   DisableEditAlpha();
            }
            return;
        }
        var vertices = EditAlphaMesh.vertices;
        if (EditAlphaMesh.colors.Length != vertices.Length) EditAlphaMesh.colors = Enumerable.Repeat(new Color(1, 1, 1, 1), vertices.Length).ToArray();
        var c = EditAlphaMesh.colors;
        bool change = false;
        Handles.BeginGUI();
        for (int i = 0; i < vertices.Length; i++)
        {   var p = HandleUtility.WorldToGUIPoint(EditAlphaGameObject.transform.TransformPoint(vertices[i]));
            var SIZE = 30;
            var rect = new Rect(p.x - SIZE / 2, p.y - SIZE / 2, SIZE, SIZE);
            EditorGUI.DrawRect(rect, new Color(c[i].a, c[i].a, c[i].a, 1));
            if (rect.Contains(Event.current.mousePosition) &&
                    (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.functionKey
                     ||
                     EditAlphaNowDraw && c[i].a != EditAlphaAmount
                    ))
            {   c[i] = new Color(c[i].r, c[i].g, c[i].b, EditAlphaAmount);
                if (Event.current.isMouse) Event.current.Use();
                EditorGUIUtility.hotControl = 0;
                EditorGUIUtility.keyboardControl = 0;
                change = true;
                EditAlphaNowDraw = true;
            }
        }
        if (change)
        {   EditAlphaMesh.colors = c;
            EditorUtility.SetDirty(EditAlphaMesh);
        }
        Handles.EndGUI();
    }
    
    
    
    
    void DrawMaterialField(FastWaterModel20Controller[] ts)
    {   FastWaterModel20Compiler oldM = ts[0].RENDERER ? ts[0].compiler : null;
        foreach (var target in ts)
        {   if (!target.RENDERER || !oldM) continue;
            var tm = target.RENDERER ? target.compiler : null;
            if (tm != oldM) oldM = null;
        }
        GUILayout.Label("Material:");
        
        var oldC = GUI.color;
        // GUI.color *= new Color32(66, 116, 76, 225);
        GUI.color *= new Color32(166, 216, 176, 225);
        var newM = EditorGUILayout.ObjectField(oldM, typeof(FastWaterModel20Compiler), false, GUILayout.Height(FastWaterModel20ControllerEditor.H)) as FastWaterModel20Compiler;
        GUI.color = oldC;
        if (newM != oldM)
        {   foreach (var target in ts)
            {   UnityEditor.Undo.RecordObject(target, "Set Material");
                UnityEditor.Undo.RecordObject(target.GetComponent<Renderer>(), "Set Material");
                target.compiler = newM;
                FastWaterModel20Controller.SetDirty(target);
                FastWaterModel20Controller.SetDirty(target.GetComponent<Renderer>());
            }
            if (newM == null || !ts[0].compiler.material.HasProperty("_MainTex") || !ts[0].compiler.material.HasProperty("_MainTexAngle"))
            {   EditorGUILayout.HelpBox("No Material", MessageType.Error);
                return;
            }
        }
    }
    
    Dictionary<FastWaterModel20Compiler, string> _GetMatPathCache = new Dictionary<FastWaterModel20Compiler, string>();
    string GetMatPath(FastWaterModel20Compiler c)
    {   if (!c) return null;
        if (!_GetMatPathCache.ContainsKey(c)) _GetMatPathCache.Add(c, AssetDatabase.GetAssetPath(c).Remove(AssetDatabase.GetAssetPath(c).LastIndexOf('/')));
        return _GetMatPathCache[c];
    }
    
    public static Dictionary<Shader, FastWaterModel20Compiler.  shaderCache> shader_cache = new Dictionary<Shader, FastWaterModel20Compiler.  shaderCache>();
    public override void OnInspectorGUI()
    {   var offset = (EditorGUIUtility.currentViewWidth - 450) / 2 ;
        offset = Mathf.Max(offset, 0);
        GUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        GUILayout.BeginVertical();
        _OnInspectorGUI();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    public  void _OnInspectorGUI()
    {   if (base.targets.Length > 1)
        {   DrawMaterialField(base.targets.Select(t => t as FastWaterModel20Controller).Where(c => c).ToArray());
        
            return;
        }
        
        target = (FastWaterModel20Controller)base.target;
        if (!target.Validate()) return;
        //Debug.Log( ChannelRemover.shader );
        
        /*if (GUILayout.Button( "Bake Z" )) {
          target.BakeOrUpdateTexture();
        }*/
        
        GUILayout.BeginVertical(GUILayout.Width(390));
        
        // EditorGUILayout.ObjectField(MonoScript.FromMonoBehaviour(target), typeof(MonoScript), false);
        GUILayout.Space(10);
        
        var newb = EditorGUILayout.Foldout(EditorPrefs.GetBool("EModules/Water20/Object Settings", true), "Object Settings");
        if (newb != EditorPrefs.GetBool("EModules/Water20/Object Settings")) EditorPrefs.SetBool("EModules/Water20/Object Settings", newb);
        if (newb)
        {
        
        
            GUILayout.Label("Plane:");
            GUILayout.BeginHorizontal();
            var mf = target.GetComponent<MeshFilter>();
            if (!mf)
            {   GUILayout.Label("Requests MeshFilter Component");
                if (GUILayout.Button("Add"))
                {   target.Undo();
                    UnityEditor.Undo.RegisterCreatedObjectUndo(target.gameObject.AddComponent<MeshFilter>(), "Add MeshFilter");
                    target.SetDirty();
                }
                return;
            }
            if (GUILayout.Button(new GUIContent("Create new GameObject")))
            {   CreateNewWater(mf);
                EditorGUIUtility.ExitGUI();
            }
            var mesh = target.GET_MESH(mf.sharedMesh);
            if (GUILayout.Button(new GUIContent("Update Mesh", "Create and assign new mesh with new parametrs")))
            {   target.UpdateMesh(mf, mesh);
                if (EditAlpha) EditAlphaMesh = mf.sharedMesh;
            }
            /*if (GUILayout.Button("Update Current Mesh", "Rulteplce asset's vertices" )) {
              mesh.WriteToMesh( mf, null );
            }*/
            if (GUILayout.Button(new GUIContent("Reset Scale", "If Scale != 1 you may update vertices according your scale to improve little perfomance")))
            {   /*if (EditorUtility.DisplayDialog( "Rewrite Mesh?", "This rewrite object's mesh, are your sure?", "Yes", "Cancel" ))*/
                mesh.ResetScale(mf);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("X_VERT", GUILayout.Width(50)); var n_XC = Mathf.Clamp(EditorGUILayout.IntField(mesh.XCOUNT, GUILayout.Width(32)), 1, int.MaxValue);
            GUILayout.Label("Z_VERT", GUILayout.Width(50)); var n_ZC = Mathf.Clamp(EditorGUILayout.IntField(mesh.ZCOUNT, GUILayout.Width(32)), 1, int.MaxValue);
            GUILayout.Label("XSIZE", GUILayout.Width(50)); var n_XS = Mathf.Clamp(EditorGUILayout.FloatField(mesh.XSIZE, GUILayout.Width(48)), 1, int.MaxValue);
            GUILayout.Label("ZSIZE", GUILayout.Width(50)); var n_ZS = Mathf.Clamp(EditorGUILayout.FloatField(mesh.ZSIZE, GUILayout.Width(48)), 1, int.MaxValue);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Edit Vertices' Alpha")))
            {   EditAlpha = !EditAlpha;
                if (EditAlpha)
                {   EnableEditAlpha(target.gameObject);
                }
                else
                {   DisableEditAlpha();
                }
            }
            if (EditAlpha && Event.current.type == EventType.Repaint) GUI.skin.button.Draw(GUILayoutUtility.GetLastRect(), new GUIContent("Edit Vertices' Alpha"), true, true, true, false);
            var oldE = GUI.enabled;
            GUI.enabled = EditAlpha;
            EditAlphaAmount = EditorGUILayout.Slider(EditAlphaAmount, 0, 1);
            GUI.enabled = oldE;
            GUILayout.EndHorizontal();
            
            
            if (n_XC != mesh.XCOUNT || n_ZC != mesh.ZCOUNT || n_XS != mesh.XSIZE || n_ZS != mesh.ZSIZE)
            {   mesh.XCOUNT = n_XC;
                mesh.ZCOUNT = n_ZC;
                mesh.XSIZE = n_XS;
                mesh.ZSIZE = n_ZS;
            }
            
            GUILayout.Space(10);
            
            
            var repaint_c = EditorGUILayout.GetControlRect(GUILayout.Height(1));
            var thisPath = GetMatPath(target.compiler);
            if (thisPath != target.MaterialsForlder) target.MaterialsForlder = thisPath;
            
            var invalidPath = string.IsNullOrEmpty(target.MaterialsForlder) || cahced_path.ContainsKey(target.MaterialsForlder) && cahced_path[target.MaterialsForlder];
            var repatint_h = FastWaterModel20ControllerEditor.H + (invalidPath ? 5 : FastWaterModel20ControllerEditor.H * 2.5f);
            if (Event.current.type == EventType.Repaint)
            {   GUI.skin.box.Draw(new Rect(repaint_c.x - 6, repaint_c.y - 7, repaint_c.width + 6, repatint_h + 14), "", false, false, false, false);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("MatSwitch:", GUILayout.Width(70));
            var oldA = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label(target.MaterialsForlder, GUILayout.Width(390 - 80));
            GUI.skin.label.alignment = oldA;
            /* var newMF = GUILayout.TextField(target.MaterialsForlder, GUILayout.Width(280 - FastWaterModel20ControllerEditor.H * 2));
             if (GUILayout.Button("...", GUILayout.Width(FastWaterModel20ControllerEditor.H * 2)))
             {
                 var tempFolder = EditorUtility.OpenFilePanel("Materials Folder", target.MaterialsForlder, "asset");
                 if (string.IsNullOrEmpty(tempFolder))
                 {
            
                 }
                 else
                 if (!tempFolder.StartsWith(Application.dataPath))
                 {
                     EditorUtility.DisplayDialog("Invalid Folder", "You should select folder into your project's folder", "Ok");
                 }
                 else
                 {
                     var nnn = tempFolder.Substring(Application.dataPath.Length - "Assets".Length);
                     if (nnn.Contains('.')) nnn = nnn.Remove(nnn.LastIndexOf('/'));
                     newMF = nnn;
                 }
             }
             if (newMF != target.MaterialsForlder)
             {
                 target.Undo();
                 target.MaterialsForlder = newMF.Trim('/');
                 target.SetDirty();
             }*/
            GUILayout.EndHorizontal();
            
            if (!string.IsNullOrEmpty(target.MaterialsForlder))
            {   if (!cahced_path.ContainsKey(target.MaterialsForlder))
                {   var inv = System.IO.Path.GetInvalidPathChars();
                    var result = target.MaterialsForlder.ToCharArray().Any(c => inv.Contains(c));
                    cahced_path.Add(target.MaterialsForlder, result);
                }
            }
            
            if (string.IsNullOrEmpty(target.MaterialsForlder) || cahced_path[target.MaterialsForlder])
            {   //GUILayout.Label( "Invalid path" );
            }
            else
            {   if (!System.IO.Directory.Exists(target.MaterialsForlder))
                {   GUILayout.Label("Invalid path");
                    GUILayout.Space(10);
                }
                else
                {   if (!cahced_mats.ContainsKey(target.MaterialsForlder))
                    {   //Debug.Log( target.MaterialsForlder + " " + AssetDatabase.Load( target.MaterialsForlder ).Length );
                        var s = target.MaterialsForlder.ToCharArray().Count(c => c == '/') + 1;
                        var mats = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith(target.MaterialsForlder)
                                   && p.ToCharArray().Count(c => c == '/') == s).OrderBy(p => p).Select(p => AssetDatabase.LoadAssetAtPath<FastWaterModel20Compiler>(p)).Where(mmm => mmm).ToArray();
                        cahced_mats.Add(target.MaterialsForlder, mats);
                    }
                    
                    GUILayout.BeginVertical(GUILayout.Width(390));
                    scrl = GUILayout.BeginScrollView(scrl, GUILayout.Height(FastWaterModel20ControllerEditor.H * 2.5f));
                    GUILayout.BeginHorizontal();
                    var m = cahced_mats[target.MaterialsForlder];
                    for (int i = 0; i < m.Length; i++)
                    {   var oc = GUI.color;
                        if (m[i] == target.compiler) GUI.color = Color.red;
                        
                        var oldBA = GUI.skin.button.alignment;
                        GUI.skin.button.alignment = TextAnchor.MiddleRight;
                        var oldBF = GUI.skin.button.fontSize;
                        GUI.skin.button.fontSize = Mathf.RoundToInt(FastWaterModel20ControllerEditor.H * 0.5f);
                        var res = GUILayout.Button(new GUIContent(m[i].name, m[i].name), GUILayout.Width(50));
                        GUI.skin.button.alignment = oldBA;
                        GUI.skin.button.fontSize = oldBF;
                        if (res)
                        {   target.Undo();
                            target.compiler = m[i];
                            target.SetDirty();
                            EditorGUIUtility.ExitGUI();
                        }
                        
                        
                        
                        GUI.color = oc;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
            }
            
            GUILayout.Space(10);
            DrawMaterialField(new[] { target });
            
            
            
            
            {   GUILayout.Label("Shader Mode:");
                var r = EditorGUILayout.GetControlRect(GUILayout.Height(53), GUILayout.Width(390));
                var oc = GUI.color;
                GUI.color *= new Color(1, 1, 1, 0.5f);
                GUI.DrawTexture(new Rect(r.x, r.y, 50, r.height), IC("modes_bg"));
                GUI.DrawTexture(new Rect(r.width - 50 + r.x, r.y, 50, r.height), IC("modes_bg"));
                GUI.color = oc;
                var texb = new[] { IC("modes_b0"), IC("modes_b1"), IC("modes_b2") };
                var hov = IC("modes_hover");
                float space = (r.width - texb[0].width * 3) / 10;
                r.width = texb[0].width;
                r.x += space * 4;
                r.height -= 4;
                r.y += 2;
                int currentMode = target.compiler.IsKeywordEnabled("ULTRA_FAST_MODE") ? 1 : target.compiler.IsKeywordEnabled("MINIMUM_MODE") ? 0 : 2;
                for (int i = 0; i < texb.Length; i++)
                {   if (i == 2)
                    {   GUI.enabled = false;
                        GUI.color *= new Color(1, 1, 1, 0.1f);
                    }
                    GUI.DrawTexture(r, texb[i]);
                    EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
                    if (GUI.Button(r, "", buttonStyle))
                    {   if (EditorUtility.DisplayDialog("Change Mode", "Are You Sure?", "Change", "Cancel"))
                        {   target.Undo();
                            //target.compiler.DisableKeywordAndNotUpdate("NORMAL_MODE");
                            if (i == 0)
                            {   target.compiler.DisableKeywordAndNotUpdate("ULTRA_FAST_MODE");
                                target.compiler.EnableKeyword("MINIMUM_MODE");
                            }
                            else if (i == 1)
                            {   target.compiler.DisableKeywordAndNotUpdate("MINIMUM_MODE");
                                target.compiler.EnableKeyword("ULTRA_FAST_MODE");
                            }
                            else
                            {   target.compiler.DisableKeywordAndNotUpdate("ULTRA_FAST_MODE");
                                target.compiler.DisableKeyword("MINIMUM_MODE");
                            }
                            // else if (i == 2) target.compiler.EnableKeyword("ULTRA_HQ_MODE");
                            target.SetDirty();
                        }
                        
                    }
                    if (i == currentMode) GUI.DrawTexture(r, hov);
                    if (i == 2) GUI.Label(r, "Alpha 0.5");
                    r.x += r.width;
                    r.x += space;
                }
                GUI.color = oc;
            }
            
            GUI.enabled = true;
            GUILayout.Space(10);
            
            
            var ubfo = EditorGUILayout.Popup(new GUIContent("Material Quality:", "You may change it runtime"), target.compiler.BufferResolution,
                                             new  string[] { "[1⁄1] Full Quality", "[1⁄2] Half Quality", "[1⁄4] Quarter Quality", "[1⁄8] Eighth Quality" });
            if (ubfo != target.compiler.BufferResolution)
            {   target.Undo();
                target.compiler.BufferResolution = ubfo;
                target.SetDirty();
            }
            
            GUILayout.BeginHorizontal();
            var oldScale = target.compiler.material.GetTextureScale("_MainTex");
            var oldOffset = target.compiler.material.GetTextureOffset("_MainTex");
            GUILayout.Label("Scale", GUILayout.Width(50));
            float W = 41;
            var os_x = EditorGUILayout.FloatField(oldScale.x, GUILayout.Width(W));
            var os_y = EditorGUILayout.FloatField(oldScale.y, GUILayout.Width(W));
            GUILayout.Label("Offset", GUILayout.Width(50));
            var oo_x = EditorGUILayout.FloatField(oldOffset.x, GUILayout.Width(W));
            var oo_y = EditorGUILayout.FloatField(oldOffset.y, GUILayout.Width(W));
            if (oldScale.x != os_x || oldScale.y != os_y || oldOffset.x != oo_x || oldOffset.y != oo_y)
            {   target.Undo();
                target.compiler.material.SetTextureScale("_MainTex", new Vector2(os_x, os_y));
                target.compiler.material.SetTextureOffset("_MainTex", new Vector2(oo_x, oo_y));
                target.SetDirty();
            }
            if (GUILayout.Button("Apply to All", GUILayout.Width(100)))
            {   FastWaterModel20Compiler.ApplyToAll("Apply Scale", (loaded) =>
                {   Undo.RecordObject(loaded, "Apply Scale");
                    loaded.material.SetTextureScale("_MainTex", new Vector2(os_x, os_y));
                    loaded.material.SetTextureOffset("_MainTex", new Vector2(oo_x, oo_y));
                    EditorUtility.SetDirty(loaded.material);
                    FastWaterModel20Controller.undoRedoPerfowrm();
                });
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotate", GUILayout.Width(50));
            var oldR = target.compiler.GetFloat("_MainTexAngle");
            var newR = EditorGUILayout.Slider(oldR / 2 / Mathf.PI * 360, 0, 360) / 360 * Mathf.PI * 2;
            if (oldR != newR)
            {   target.Undo();
                target.compiler.material.SetFloat("_MainTexAngle", newR);
                if ((oldR == 0 || newR == 0))
                {   if (newR != 0) target.compiler.EnableKeyword("HAS_ROTATION");
                    else target.compiler.DisableKeyword("HAS_ROTATION");
                }
                target.SetDirty();
            }
            if (GUILayout.Button("Apply to All", GUILayout.Width(100)))
            {   FastWaterModel20Compiler.ApplyToAll("Apply Rotate", (loaded) =>
                {   Undo.RecordObject(loaded, "Apply Rotate");
                    loaded.SetFloat("_MainTexAngle", newR);
                    if (newR != 0) loaded.EnableKeyword("HAS_ROTATION");
                    else loaded.DisableKeyword("HAS_ROTATION");
                    EditorUtility.SetDirty(loaded.material);
                    FastWaterModel20Controller.undoRedoPerfowrm();
                });
            }
            GUILayout.EndHorizontal();
            
            target.OBJECT_SPEED = EditorGUILayout.FloatField(new GUIContent("Material Speed:", "You may change it runtime (<FastWaterModel20Controller>().SetSpeed(x))"), target.OBJECT_SPEED);
            
            
            
            if (GUILayout.Button("Create Copy Of Material"))
            {   if (target.compiler)
                {   var PROP = "Materials";
                    var P1 = AssetDatabase.GetAssetPath(target.compiler);
                    var P2 = "";
                    if (string.IsNullOrEmpty(P1))
                    {   P1 = EditorPrefs.GetString("EModules/Water20/" + PROP + " Name", null);
                        P2 = EditorPrefs.GetString("EModules/Water20/" + PROP + " Path", null);
                    }
                    else
                    {   P2 = P1.Remove(P1.LastIndexOf('/'));
                        P1 = P1.Substring(P1.LastIndexOf('/') + 1);
                    }
                    var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Material", P1, "asset", "Select the folder to save the baked texture ", P2);
                    if (!string.IsNullOrEmpty(path))
                    {   if (System.IO.File.Exists(path))
                        {   UnityEditor.EditorUtility.DisplayDialog("Error", "Can not overwrite an existing file", "Ok");
                        }
                        else
                        {   var filename = path.Substring(path.LastIndexOf('/') + 1);
                            var normalized_path = path.Remove(path.LastIndexOf('/'));
                            EditorPrefs.SetString("EModules/Water20/" + PROP + " Name", filename);
                            EditorPrefs.SetString("EModules/Water20/" + PROP + " Path", normalized_path);
                            target.Undo();
                            target.compiler = target.CreateAsset(target.compiler, path);
                            target.SetDirty();
                        }
                    }
                }
            }
            USE_FIELDS = EditorGUILayout.Popup(content, USE_FIELDS ? 1 : 0, new[] { new GUIContent("Fields like Sliders"), new GUIContent("Standard Fields") }) == 1;
            EditorGUILayout.HelpBox("Press 'Alt' to change values more smoothly, or press 'Enter' to enter value from keyboard", MessageType.Info);
            
            var shader = target.compiler.shader;
            if (shader)
            {   if (!shader_cache.ContainsKey(shader))
                {   shader_cache.Add(shader, target.compiler.GetShaderUniform());
                }
                
                var texCount = shader_cache[shader].texCount;
                if (target.compiler.BufferResolution == 0)
                {   texCount--;
                }
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Textures Count: " + (texCount == -1 ? "error read cginc" : texCount.ToString() ) );
                GUI.enabled                          = texCount != -1;
                if (GUILayout.Button("Log Names", GUILayout.Width(80)))
                {   foreach (var item in shader_cache[shader].texNames)
                    {   if (target.compiler.BufferResolution == 0 &&  item .Contains ("_FrameBuffer")) continue;
                        if (item.Contains("_UF_NMASK_Texture")) Debug.Log("_SecontTexture");
                        else if (item.Contains("_MainTex")) Debug.Log("_MaintTexture");
                        else if (item.Contains("_BakedData")) Debug.Log("_BakedZChannel");
                        else if (item.Contains("_RefractionTex")) Debug.Log("_BakedRefractionChannel");
                        else if (item.Contains("_Utility")) Debug.Log("_Utils");
                        else if (item.Contains("_ShoreWavesGrad")) Debug.Log("_ShoreTexture");
                        else
                        {   Debug.Log(item.Replace(" ", "").Replace("sampler2D", ""));
                        
                        }
                    }
                }
                GUI.enabled                          = true;
                GUILayout.EndHorizontal();
                var gt = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget);
                if (gt.Any(g => g == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 ) && gt.All(g => g != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3))
                {   if (texCount > 8)
                    {   EditorGUILayout.HelpBox("If you will use OpenGLES2 please please try to keep less than 9 textures\nHowever, you can try it yourself, whether it works or not", MessageType.Warning);
                    }
                }
                GUILayout.Space(4);
            }
            else
            {   GUILayout.Space(20);
            
            }
            
        }
        else
        {   DrawMaterialField(new[] { target });
        }
        
        
        var zdepth_request = IsZDepthRequested();
        var zdepth_enabled = ZEnabled();
        
        if (zdepth_request && !zdepth_enabled)
        {   EditorGUILayout.HelpBox("You include parameters that request to include ZDepth, you must disable them or enable ZDepth", MessageType.Error);
            if (GUILayout.Button("Fix ZDepth"))
            {   var fixmenu = new GenericMenu();
                if (!zdepth_enabled)
                {   for (int i = 1; i < pop_depth.contents.Length; i++)
                    {   var captureI = i;
                        fixmenu.AddItem(new GUIContent("Enable ZDepth/Enable " + pop_depth.contents[captureI]), false, () =>
                        {   target.Undo(true);
                            pop_depth.VALUE = captureI;
                            target.SetDirty();
                        });
                    }
                }
                if (zdepth_request) CreateFixMenu(ref fixmenu);
                fixmenu.ShowAsContext();
            }
            GUILayout.Space(10);
        }
        
        
        
        if (target.compiler)
        {   /* if (target.compiler.IsKeywordEnabled("ALLOW_MANUAL_DEPTH") && (FastWaterModel20Controller.GET_CAMERA.depthTextureMode & DepthTextureMode.Depth) == 0)
             {
                 FastWaterModel20Controller.GET_CAMERA.depthTextureMode |= DepthTextureMode.Depth;
             }*/
            
            if (lastUsedGUID == null) lastUsedGUID = EditorPrefs.GetString("EModules/Water20/LAST_MATERIAL", "");
            if (!materialToGUID.ContainsKey(target.compiler))
            {   var path = AssetDatabase.GetAssetPath(target.compiler);
                if (!string.IsNullOrEmpty(path))
                {   var guid = AssetDatabase.AssetPathToGUID(path);
                    materialToGUID.Add(target.compiler, guid);
                }
                else
                {   materialToGUID.Add(target.compiler, "");
                }
            }
            var currentGuid = materialToGUID[target.compiler];
            if (!string.IsNullOrEmpty(currentGuid) && lastUsedGUID != currentGuid)
            {   EditorPrefs.SetString("EModules/Water20/LAST_MATERIAL", currentGuid);
                lastUsedGUID = currentGuid;
            }
            
            /* if (FastWaterModel20Controller.GET_CAMERA && FastWaterModel20Controller.GET_CAMERA.orthographic && !target.material.IsKeywordEnabled( "ORTO_CAMERA" )) {
               target.material.EnableKeyword( "ORTO_CAMERA" );
               target.SetDirty();
             }
             if (FastWaterModel20Controller.GET_CAMERA && !FastWaterModel20Controller.GET_CAMERA.orthographic && target.material.IsKeywordEnabled( "ORTO_CAMERA" )) {
               target.material.DisableKeyword( "ORTO_CAMERA" );
               target.SetDirty();
             }*/
        }
        
        //&& !target.material.GetTexture( FIELDS._BackedData )
        
        GUILayout.EndVertical();
        
        
        target.__CheckShader();
        
        /* if (USE_LIST_GUI ?? (USE_LIST_GUI = EditorPrefs.GetBool( "EModules/MobileWater20/USE_LIST_GUI", false )).Value) LIST_GUI();
         else {*/
        if (!target.compiler.material) return;
        
        d_D_TopMenu.target = this;
        d_D_TopMenu.Draw();
        
        
        if (Event.current.type != EventType.ScrollWheel)
        {   SCROLL_X = EditorGUILayout.BeginScrollView(new Vector2(SCROLL_X, 0), GUILayout.Height(MENU_CURRENT_HEIGHT()), GUILayout.Width(390)).x;
            //EditorGUILayout.GetControlRect( GUILayout.Width( 380 ), GUILayout.Height( 0 ) );
            var fullRect = EditorGUILayout.GetControlRect(GUILayout.Height(400), GUILayout.Width(390));
            
            // if (Event.current.type != EventType.Layout)
            TEXTURED_GUI(fullRect);
            
            if (Event.current.rawType == EventType.MouseUp || Event.current.rawType == EventType.MouseDown) MouseDrag = false;
            MouseCheck = false;
            if (Event.current.type == EventType.MouseDown) MouseCheck = true;
            
            if (MouseDrag)
            {   //if (Event.current.type != EventType.Layout) TEXTURED_GUI( fullRect );
            }
            else
            {   // if (Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp) TEXTURED_GUI( fullRect );
            }
            
            if (MouseCheck && Event.current.type != EventType.MouseDown) MouseDrag = true;
            GUI.enabled = true;
            GUI.color = Color.white;
            if (Event.current.type == EventType.Repaint)
            {   GL.Clear(true, false, Color.white);
                GL.PushMatrix();
                //GL.Viewport( new Rect( 0, -20, Screen.width, Screen.height ) );
                BlitMaterial.SetPass(0);
                //GL.LoadPixelMatrix();
                GL.Begin(GL.LINES);
                GL.Color(ACTIVE_COLOR);
                foreach (var line in lines.Values)
                {   if (!line.used) continue;
                    //lineIndex--;
                    GL.Vertex3(line.worldp1.x, line.worldp1.y, 0);
                    GL.Vertex3(line.worldp2.x, line.worldp2.y, 0);
                    line.used = false;
                }
                GL.End();
                GL.PopMatrix();
            }
            lineIndex = 0;
            EditorGUILayout.EndScrollView();
        }
        
        
        // }
        target.skipClamp = false;
        
    }
    
    bool MouseCheck = false;
    bool MouseDrag = false;
    
    public bool IS_OPAQUE { get { return target.compiler.IsKeywordEnabled("USE_SHADOWS") || target.compiler.IsKeywordEnabled("FORCE_OPAQUE"); } }
    public bool IS_GRABPASS { get { return target.compiler.IsKeywordEnabled("REFRACTION_GRABPASS"); } }
    
    
    
    
    static Dictionary<FastWaterModel20Compiler, string> materialToGUID = new Dictionary<FastWaterModel20Compiler, string>();
    static string lastUsedGUID;
    
    class fixer_mixer {
        public string[] options;
        public string fix;
#pragma warning disable
        public bool skipFirst;
#pragma warning restore
        public bool IsTrue(FastWaterModel20Controller target)
        {   return options.Any(o =>
            {   if (o.ToLower().Contains("skip")) return !target.compiler.IsKeywordEnabled(o);
                else return target.compiler.IsKeywordEnabled(o);
            });
        }
        public void FIX(FastWaterModel20Controller target)
        {   var __skipFirst = skipFirst;
            target.Undo(true);
            foreach (var o in options)
            {   if (__skipFirst)
                {   __skipFirst = false;
                    continue;
                }
                if (o.ToLower().Contains("skip")) target.compiler.EnableKeyword(o);
                else target.compiler.DisableKeyword(o);
            }
            /*  if (fix.ToLower().Contains( "skip" )) target.material.DisableKeyword( fix );
              else target.material.EnableKeyword( fix );*/
            target.compiler.EnableKeyword(fix);
            target.SetDirty();
        }
    }
    
    
    fixer_mixer f_refr = new fixer_mixer() { options = new[] { "REFRACTION_GRABPASS", "REFRACTION_BAKED_FROM_TEXTURE", "REFRACTION_BAKED_ONAWAKE", "REFRACTION_BAKED_VIA_SCRIPT", "REFRACTION_ONLYZCOLOR" }, fix = "REFRACTION_NONE" };
    fixer_mixer f_foam = new fixer_mixer() { options = new[] { "SKIP_FOAM" }, fix = "SKIP_FOAM" };
    fixer_mixer f_grad = new fixer_mixer() { options = new[] { "USE_OUTPUT_BLEND_1", "USE_OUTPUT_BLEND_3" }, fix = "USE_OUTPUT_BLEND_2" };
    fixer_mixer f_3dv1 = new fixer_mixer() { options = new[] { "VERTEX_ANIMATION_BORDER_FADE" }, fix = "VERTEX_ANIMATION_BORDER_CLAMPXYZ" };
    // fixer f_3dv2 = new fixer(){options = new[]{ "SKIP_3DVERTEX_HEIGHT_COLORIZE"  }, fix = "SKIP_3DVERTEX_HEIGHT_COLORIZE" };
    
    bool IsZDepthRequested()
    {   if (f_refr.IsTrue(target)) return true;
        if (f_foam.IsTrue(target)) return true;
        if (target.compiler.IsKeywordEnabled("USE_OUTPUT_GRADIENT") && f_grad.IsTrue(target)) return true;
        if (!target.compiler.IsKeywordEnabled("SKIP_3DVERTEX_ANIMATION") && f_3dv1.IsTrue(target)) return true;
        //if (!target.material.IsKeywordEnabled( "SKIP_3DVERTEX_ANIMATION" ) && f_3dv2.IsTrue( target )) return true;
        return false;
        
        
        /*if (target.material.IsKeywordEnabled( "REFRACTION_GRABPASS" ) || target.material.IsKeywordEnabled( "REFRACTION_BAKED" ) || target.material.IsKeywordEnabled( "REFRACTION_ONLYZCOLOR" )) return true;
        if (!target.material.IsKeywordEnabled( "SKIP_FOAM" )) return true;
        if (target.material.IsKeywordEnabled( "USE_OUTPUT_GRADIENT" ) &&
          (target.material.IsKeywordEnabled( "USE_OUTPUT_BLEND_1" ) || target.material.IsKeywordEnabled( "USE_OUTPUT_BLEND_3" ))) return true;
        if (!target.material.IsKeywordEnabled( "SKIP_3DVERTEX_ANIMATION" ) &&
          (target.material.IsKeywordEnabled( "VERTEX_ANIMATION_BORDER_FADE" ) || !target.material.IsKeywordEnabled( "SKIP_3DVERTEX_HEIGHT_COLORIZE" ))) return true;
        return false;*/
    }
    
    
    static bool HAS_REFRACTION(FastWaterModel20ControllerEditor target)
    {   return target.f_refr.IsTrue(target.target);
    }
    static bool HAS_MAIN_TEX(FastWaterModel20ControllerEditor target)
    {   return target.target.compiler.IsKeywordEnabled("SKIP_MAINTEXTURE") ||
               target.target.compiler.IsKeywordEnabled("USE_NOISED_GLARE_PROCEDURALHQ") ||
               HAS_REFRACTION(target) && !target.target.compiler.IsKeywordEnabled("SKIP_REFLECTION_MASK");
    }
    
    static bool HAS_NOISED_GLARE(FastWaterModel20ControllerEditor target)
    {   return !target.target.compiler.IsKeywordEnabled(FIELDS.SKIP_NOISED_GLARE_HQ) ||
               target.target.compiler.IsKeywordEnabled(FIELDS.USE_NOISED_GLARE_ADDWAWES1) ||
               target.target.compiler.IsKeywordEnabled(FIELDS.USE_NOISED_GLARE_ADDWAWES2) ||
               target.target.compiler.IsKeywordEnabled(FIELDS.USE_NOISED_GLARE_LQ) ||
               target.target.compiler.IsKeywordEnabled(FIELDS.USE_NOISED_GLARE_PROCEDURALHQ);
    }
    
    static Color GET_COLOR(int channel)
    {   switch (channel)
        {   case 0: return new Color(1, 0, 0, 0);
            case 1: return new Color(0, 1, 0, 0);
            case 2: return new Color(0, 0, 1, 0);
            case 3: return new Color(0, 0, 0, 1);
        }
        return new Color(1, 1, 1, 1);
    }
    
    
    void CreateFixMenu(ref GenericMenu menu)
    {   if (f_refr.IsTrue(target))
            menu.AddItem(new GUIContent("Fix Material/Disabe Refraction"), false, () => f_refr.FIX(target));
        if (f_foam.IsTrue(target))
            menu.AddItem(new GUIContent("Fix Material/Disabe Foam"), false, () => f_foam.FIX(target));
        if (target.compiler.IsKeywordEnabled("USE_OUTPUT_GRADIENT") && f_grad.IsTrue(target))
            menu.AddItem(new GUIContent("Fix Material/Disabe Gradient"), false, () => f_grad.FIX(target));
        if (!target.compiler.IsKeywordEnabled("SKIP_3DVERTEX_ANIMATION") && f_3dv1.IsTrue(target))
            menu.AddItem(new GUIContent("Fix Material/Disabe 3D waves Border Clamp"), false, () => f_3dv1.FIX(target));
    }
    
    bool ZEnabled()
    {   return
            target.compiler.IsKeywordEnabled(FIELDS.BAKED_DEPTH_EXTERNAL_TEXTURE) ||
            target.compiler.IsKeywordEnabled(FIELDS.BAKED_DEPTH_VIASCRIPT) ||
            target.compiler.IsKeywordEnabled(FIELDS.BAKED_DEPTH_ONAWAKE) ||
            target.compiler.IsKeywordEnabled(FIELDS.ALLOW_MANUAL_DEPTH);
    }
    
    
    // //////////
    // //////////
    //LIST_GUI //
    void LIST_GUI()
    {   pop_reflection.DrawPop("Reflection:");
        /* var Reflection = EditorGUILayout.Popup("Reflection:", target.USE_REFLECTION_PROBE,
        
         if (Reflection != target.USE_REFLECTION_PROBE) {
           Undo();
           target.USE_REFLECTION_PROBE = Reflection;
           SetDirty();
         }*/
    }
    
    
    
    // //////////////
    // //////////////
    //TEXTURED_GUI //
#pragma warning disable
    D_MainTexture d_D_MainTexture = new D_MainTexture();
    D_TopMenu d_D_TopMenu = new D_TopMenu();
    D_Texture d_D_Texture = new D_Texture();
    D_Lightin d_D_Lightin = new D_Lightin();
    D_Foam d_D_Foam = new D_Foam();
    D_Vertices d_D_Vertices = new D_Vertices();
    D_Noise d_D_Noise = new D_Noise();
    D_Reflection d_D_Reflection = new D_Reflection();
    D_Utils d_D_Utils = new D_Utils();
    D_Refraction d_D_Refraction = new D_Refraction();
    D_Output d_D_Output = new D_Output();
    Vector2[] draw_output = new Vector2[0];
    
#pragma warning restore
    
    
    bool popChecker = false;
    static System.Reflection.FieldInfo[] pop_fields;
    static System.Reflection.PropertyInfo vlaue_prop;
    
    void TEXTURED_GUI(Rect fullRect)
    {   if (target.compiler.NowCompile)
        {   GUI.Label(fullRect, "Compilation...");
            return;
        }
        
        
        lineIndex = 0;
        fullRect.width = 390;
        fullRect.x -= 4;
        
        
        if (!popChecker)
        {   popChecker = true;
            if (pop_fields == null)
            {   var c = typeof(POP_CLASS);
                pop_fields = this.GetType().GetFields().Where(f => f.FieldType == c).ToArray();
                vlaue_prop = c.GetProperty("VALUE", (System.Reflection.BindingFlags)(-1));
            }
            foreach (var f in pop_fields)
            {   var f2 = f.GetValue(this);
                vlaue_prop.GetValue(f2, null);
            }
        }
        
        if (!target.compiler || !target.compiler.material || !target.RENDERER || !target.RENDERER.sharedMaterial) return;
        
        // HasDebug = false;
        switch (DRAW_MENU(ref fullRect))
        {   case 0: CATEGORY_1_NORMAL(fullRect); break;
            case 2: CATEGORY_2_SUFRFACE(fullRect); break;
            case 1: CATEGORY_3_ZFoam(fullRect); break;
                #if SHOW_NOISE_CATEGORY
            case 3: CATEGORY_3_NOISE(fullRect); break;
            case 4: CATEGORY_4_REFL(fullRect); break;
            case 5: CATEGORY_4_REFR(fullRect); break;
            case 6: CATEGORY_5_OUTPUT(fullRect); break;
                #else
            case 3: CATEGORY_4_REFL(fullRect); break;
            case 4: CATEGORY_4_REFR(fullRect); break;
            case 5: CATEGORY_5_OUTPUT(fullRect); break;
                #endif
        }
        //  if (HasDebug)
        
        
    }
    
    
    void CATEGORY_1_NORMAL(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_MainTexture.target = this;
        d_D_MainTexture.Draw(new Rect(fullRect.x, FL, 100, 0), out draw_output);
        
        
        
    }
    
    void CATEGORY_2_SUFRFACE(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Lightin.target = this;
        d_D_Lightin.Draw(new Rect(fullRect.x, FL, 200, 0), out draw_output);
        
    }
    
    
    void CATEGORY_3_ZFoam(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Utils.target = this;
        d_D_Utils.Draw(new Rect(fullRect.x, fullRect.y, 100, 0), out draw_output);
        d_D_Foam.target = this;
        d_D_Foam.Draw(new Rect(fullRect.x + 100, FL, 100, 0), out draw_output);
    }
    void CATEGORY_3_NOISE(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Noise.target = this;
        d_D_Noise.Draw(new Rect(fullRect.x, FL, 400, 0), out draw_output);
    }
    
    void CATEGORY_4_REFL(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Reflection.target = this;
        d_D_Reflection.Draw(new Rect(fullRect.x, FL, 390, 0), out draw_output);
    }
    void CATEGORY_4_REFR(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Refraction.target = this;
        d_D_Refraction.Draw(new Rect(fullRect.x, FL, 390, 0), out draw_output);
    }
    
    
    
    void CATEGORY_5_OUTPUT(Rect fullRect)
    {   var FL = fullRect.y;
        d_D_Output.target = this;
        d_D_Output.Draw(new Rect(fullRect.x, FL, 390, 0), out draw_output);
    }
    
    
    bool[,] arrows =
    {   {true, false, true, true, true, true },
        {true, true, true, true, true, true },
        {true, true, true, true, true, true },
        {true, true, true, true, true, true },
        {true, true, true, true, true, true },
        {true, true, true, true, true, true },
        {true, true, true, true, true, true },
    };
    
    int DRAW_MENU(ref Rect r)
    {
    
        var result = r;
        r.height = FastWaterModel20ControllerEditor.H * 3;
        var currentItem = EditorPrefs.GetInt("EModules/Water20/CurrentMenuItem", 0);
        var itemsNames = new[] { "Normal", "Z/Foam",    "Surface",
                                 #if SHOW_NOISE_CATEGORY
                                 "Noise",
                                 #endif
                                 "Reflect", "Refract", "Output"
                               };
        var itemsTexture = new[] { "MENU_1_3", "MENU_3_2",  "MENU_2_2",
                                   #if SHOW_NOISE_CATEGORY
                                   "MENU_3_1",
                                   #endif
                                   "MENU_4", "MENU_4_I", "MENU_5"
                                 };
        GUIContent content = new GUIContent();
        
        r.width /= itemsNames.Length;
        var resetX = r.x;
        
        
        for (int i = 0; i < itemsNames.Length; i++)
        {   // var style = i == 0 ? EditorStyles.miniButtonLeft : i == itemsNames.Length - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid;
            // var style = EditorStyles.helpBox;
            var style = currentItem == i ? EditorStyles.helpBox : GUI.skin.button;
            content.text = "";
            var buttonRect = r;
            #if SHOW_NOISE_CATEGORY
            if (i == 3) buttonRect = SHRINK(buttonRect, 10);
            #endif
            buttonRect.x += (buttonRect.width - buttonRect.height) / 2;
            buttonRect.width = buttonRect.height;
            if (GUI.Button(buttonRect, content, style))
            {   EditorPrefs.SetInt("EModules/Water20/CurrentMenuItem", i);
                currentItem = i;
            }
            if (i != itemsNames.Length - 1 && arrows[currentItem, i])
            {   //var v = new Vector2(buttonRect.x + buttonRect.width + 10, buttonRect.y + buttonRect.height / 2);
                //  DrawArrowedLine(v, Offset_X(v, 6), 2, true);
            }
            
            if (currentItem == i && Event.current.type == EventType.Repaint) style.Draw(buttonRect, content, true, true, true, false);
            DRAW_MASKEDTEXTURE(itemsTexture[i], SHRINK(buttonRect, 4), true);
            buttonRect.y += buttonRect.height;
            buttonRect.height = FastWaterModel20ControllerEditor.H;
            GUI.Label(buttonRect, itemsNames[i]);
            
            
            
            r.x += r.width;
        }
        r.y += r.height + FastWaterModel20ControllerEditor.H + 10;
        /*r.height = FastWaterModel20ControllerEditor.H;
        r.x = resetX;
        for (int i = 0 ; i < itemsNames.Length ; i++) {
          GUI.Label( r, itemsNames[i] );
          r.x += r.width;
        }*/
        
        
        result.y = r.y;
        r = result;
        return currentItem;
    }
    
    int MENU_CURRENT_HEIGHT()
    {   return Mathf.RoundToInt( 1000 * EditorGUIUtility.singleLineHeight / 16);
    }
    
    
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    Material __BlitMateriald;
    Material BlitMaterial
    {   get { return __BlitMateriald ? __BlitMateriald : (__BlitMateriald = (Material)typeof(GUI).GetMethod("get_blitMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, null)); }
    }
    static int lineIndex = 0;
    class Line {
        public Color color = Color.gray; public Vector2 p1; public Vector2 p2; public Vector2 worldp1; public Vector2 worldp2;
        internal bool used;
    }
    static Dictionary<int, Line> lines = new Dictionary<int, Line>();
    static Line DrawLine(Vector2 v1, Vector2 v2, Color? color = null) { return DrawLine(v1.x, v1.y, v2.x, v2.y, color); }
    static Line DrawLine(float x1, float y1, float x2, float y2, Color? color = null)
    {   if (!lines.ContainsKey(lineIndex)) lines.Add(lineIndex, new Line());
        x1 = Mathf.RoundToInt(x1);
        y1 = Mathf.RoundToInt(y1);
        x2 = Mathf.RoundToInt(x2);
        y2 = Mathf.RoundToInt(y2);
        lines[lineIndex].p1.Set(x1, y1);
        lines[lineIndex].worldp1 = SUM(lines[lineIndex].p1, LAST_GROUP_R);
        lines[lineIndex].p2.Set(x2, y2);
        lines[lineIndex].worldp2 = SUM(lines[lineIndex].p2, LAST_GROUP_R);
        //lines[lineIndex].color = color ?? ACTIVE_COLOR;
        lines[lineIndex].color = ACTIVE_COLOR;
        lines[lineIndex].used = true;
        return lines[lineIndex++];
    }
    static Vector2 tV_A, tV_B;
    static Vector2 DrawLine(Vector2 v1, Vector2 v2, int bold, bool active) { return DrawLine(v1.x, v1.y, v2.x, v2.y, bold, active ? ACTIVE_COLOR : PASSIVE_COLOR).p2; }
    static Vector2 DrawLine(Vector2 v1, Vector2 v2, int bold, Color? color = null) { return DrawLine(v1.x, v1.y, v2.x, v2.y, bold, color).p2; }
    static Line DrawLine(float x1, float y1, float x2, float y2, int bold, Color? color = null)
    {   var id = lineIndex;
        tV_A.Set(x1, y1);
        tV_B.Set(x2, y2);
        var backD = (tV_A - tV_B).normalized * bold;
        var wide1 = Swap(backD) / 3f;
        var wide2 = -Swap(backD) / 3f;
        for (float i = 0; i <= bold; i += 0.5f)
        {   if (!lines.ContainsKey(lineIndex)) lines.Add(lineIndex, new Line());
            var offset = Vector2.Lerp(wide1, wide2, i / ((float)bold));
            DrawLine(x1 + offset.x, y1 + offset.y, x2 + offset.x, y2 + offset.y, color);
        }
        return lines[id + bold / 2];
    }
    /* void DrawArrowedLine(float x1, float y1, float x2, float y2, Color color)
     {
       var l = DrawLine( x1, y1, x2, y2 , 2, color);
       const int L = 10;
       Vector2 target = l.p2;
       var backD = (l.p1 - target).normalized * L;
       var wide1 = Swap(backD) /2  + backD;
       var wide2 = -Swap(backD) /2 + backD;
       for (int i = 0 ; i <= L ; i++) {
         var targetColor = Color.Lerp(color + new Color(0.2f,0.2f,0.2f,1), color, 1- 2 * Mathf.Abs(i - (L/2) )/(float)L);
         DrawLine( Vector2.Lerp( wide1, wide2, i/(float)L ) + target, target, color: targetColor );
       }
    
     }*/
    static Vector2 DrawArrowedLine(Vector2 v1, Vector2 v2, int bold, bool active)
    {   const int L = 12;
    
        var color = active ? ACTIVE_COLOR : PASSIVE_COLOR;
        var t_v2 = v2 + (-(v2 - v1)).normalized * L / 2;
        DrawLine(v1, t_v2, bold, color);
        Vector2 target = v2;
        var backD = (v1 - target).normalized * L;
        var wide1 = Swap(backD) / 2 + backD;
        var wide2 = -Swap(backD) / 2 + backD;
        for (float i = 0; i <= L; i += 0.5f)
        {   //var targetColor = Color.Lerp(color + new Color(0.2f,0.2f,0.2f,1), color, 1- 2 * Mathf.Abs(i - (L/2) )/(float)L);
            var targetColor = Color.Lerp(color + new Color(0.2f, 0.2f, 0.2f, 1), color, 1 - 2 * Mathf.Pow(Mathf.Abs(i - (L / 2)), 0.4f) / (float)L);
            DrawLine(ROUND(Vector2.Lerp(wide1, wide2, i / (float)L) + target), target, color: targetColor);
        }
        return v2;
    }
    
    static Vector2 ROUND(Vector2 v)
    {   v.x = Mathf.RoundToInt(v.x);
        v.y = Mathf.RoundToInt(v.y);
        return v;
    }
    
    static Vector2 Swap(Vector2 v)
    {   var t = v.x;
        v.x = -v.y;
        v.y = t;
        return v;
    }
    /* private void OnEnable()
     {
       var shader = Shader.Find("Hidden/Internal-Colored");
       InternalColored = new Material( shader );
     }
     private void OnDisable()
     {
       DestroyImmediate( InternalColored );
     }*/
    
    // const float CM_V = 0.9f;
    // static Color ACTIVE_COLOR = new Color32(46,233,230,255) * new Color(CM_V,CM_V,CM_V,1);
    static Color ACTIVE_COLOR = new Color32(71, 173, 238, 255);
    // static Color PASSIVE_COLOR = Color.gray;
    static Color PASSIVE_COLOR = new Color32(225, 225, 225, 255);
    
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    
    
    //-------------------------------------
    #region//-----DRAWING TOOLS
    //-------------------------------------
    
    
    
    
    
    static Rect DRAW_DOT_12(Rect r) { return __drawDot(r, "DOT_12"); }
    static Rect DRAW_DOT_16(Rect r) { return __drawDot(r, "DOT_16"); }
    static Rect DRAW_DOT_BLUE(Rect r, bool enable) { return __drawDot(r, enable ? "DOT_BLUE" : "DOT_BLUE_PASSIVE"); }
    static Rect __drawDot(Rect r, string t_string)
    {   var t = IC(t_string);
        r.y = r.y + r.height / 2 - t.height / 2;
        r.width = r.height = 0;
        return DRAW_TEXTURE(t_string, r);
    }
    GUIStyle __HEADERED_STYLE;
    GUIStyle HEADERED_STYLE
    {   get
        {   if (__HEADERED_STYLE == null)
            {   __HEADERED_STYLE = new GUIStyle();
                __HEADERED_STYLE.normal.background = IC("BG_toogleBox");
                __HEADERED_STYLE.border = new RectOffset(20, 20, 0, 0);
            }
            return __HEADERED_STYLE;
        }
    }
    public Rect DRAW_BG_TEXTURE(Rect r, string message, string fieldName, bool enable, out Vector2 output, string texture, bool IsCube = false)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        var result = new Rect(r.x, r.y, 0, 0);
        
        
        r.height = FastWaterModel20ControllerEditor.H;
        if (r.width == 0) r.width = IC("BG_toogleBox").width;
        if (texture != null || message != null)
        {   EditorGUI.HelpBox(r, message, MessageType.None);
            r.y += r.height;
        }
        
        r.height = texture != null ? IC("BG_toogleBox").height : 60;
        if (Event.current.type == EventType.Repaint)
        {   if (texture != null) HEADERED_STYLE.Draw(r, EmptyContent, 0);
            else SIMPLE_STYLE.Draw(r, EmptyContent, 0);
        }
        //r = DRAW_TEXTURE( "BG_toogleBox", r, enable );
        result.width = r.x + r.width - result.x;
        result.height = r.y + r.height - result.y;
        
        if (texture != null)
        {   var label_R = SPACE_X(r, 4);
            label_R.height = 30;
            //label_R.x += label_R.height - 4;
            label_R.width -= 17;
            DRAW_MASKEDTEXTURE(texture, SHRINK(label_R, 4), enable);
            label_R.x += label_R.width;
            var dot_R = DRAW_DOT_BLUE(label_R, enable);
            output = CENTER(dot_R);
            
            r.y += label_R.height;
            r.height -= label_R.height;
        }
        else
        {   output = CENTER(r);
        }
        
        
        r = SHRINK(r, 7);
        
        if (enable)
        {   if (!IsCube)
            {   var MT = target.compiler.GetTexture(fieldName);
                var NEW_MT = EditorGUI.ObjectField(r, MT, typeof(Texture2D), false) as Texture2D;
                if (MT != NEW_MT)
                {   target.Undo();
                    target.compiler.SetTexture(fieldName, NEW_MT);
                    target.SetDirty();
                }
            }
            else
            {   var MT = target.compiler.GetTexture(fieldName) as Cubemap;
                var NEW_MT = EditorGUI.ObjectField(r, MT, typeof(Cubemap), false) as Cubemap;
                if (MT != NEW_MT)
                {   target.Undo();
                    target.compiler.SetTexture(fieldName, NEW_MT);
                    target.SetDirty();
                }
            }
        }
        
        
        
        
        GUI.enabled = e;
        return result;
    }
    
    const float COLOR_WIDTH = 80;
    const float TOGGLE_WIDTH = 90;
    
    public bool DRAW_TOGGLE(Rect r, string message, string fieldName, bool enable)
    {   Vector2 v = Vector2.zero;
        bool? v2 = null;
        var res = DRAW_TOGGLE(r, message, fieldName, enable, out v, ref v2);
        return res;
    }
    //public bool HasDebug = false;
    public bool DRAW_TOGGLE(Rect r, string message, string fieldName, bool enable, out Vector2 output)
    {   bool? v = null;
        return DRAW_TOGGLE(r, message, fieldName, enable, out output, ref v);
    }
    public bool DRAW_TOGGLE(Rect r, string message, string fieldName, bool enable, out Vector2 output, ref bool? overrideValue)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        var result = new Rect(r.x, r.y, 0, 0);
        
        var MT = fieldName == null ? overrideValue ?? true : target.compiler.IsKeywordEnabled(fieldName);
        if (fieldName != null && fieldName.ToLower().Contains("skip")) MT = !MT;
        var textureName = MT ? "HUGE_CHECK_ON" : "HUGE_CHECK_OFF";
        
        var WW = r.width = r.width == 0 ? TOGGLE_WIDTH : r.width;
        r.height = FastWaterModel20ControllerEditor.H;
        if (message != null)
        {   var lowerField = message.ToLower();
            var oldC = GUI.color;
            if (fieldName != null && lowerField.Contains("debug") && target.compiler.IsKeywordEnabled(fieldName))
            {   //  HasDebug = true;
                GUI.color *= Color.red;
            }
            
            var tW = fieldName == null && !overrideValue.HasValue ? 0 : IC(textureName).width;
            r.x += tW;
            r.width -= tW;
            r.height = IC(textureName).height;
            if (__ICONS.ContainsKey(message))
            {   EditorGUI.HelpBox(r, "", MessageType.None);
                DRAW_MASKEDTEXTURE(message, r, enable && MT);
            }
            else
            {   EditorGUI.HelpBox(r, message, MessageType.None);
                //r.y += r.height;
            }
            r.x -= tW;
            
            GUI.color = oldC;
        }
        else
        {   WW = IC(textureName).width;
        }
        
        if (fieldName != null || overrideValue.HasValue)
        {
        
            r.width = r.height = 0;
            r = DRAW_TEXTURE(textureName, r, enable);
            result.width = r.x + r.width - result.x;
            result.height = r.y + r.height - result.y;
            
            if (GUI.Button(r, "", BS))
            {   MT = !MT;
            
                target.Undo(true);
                if (fieldName != null)
                {   var lowerField = fieldName.ToLower();
                    if (lowerField.Contains("skip")) MT = !MT;
                    if (MT) target.compiler.EnableKeyword(fieldName);
                    else target.compiler.DisableKeyword(fieldName);
                }
                else
                {   overrideValue = MT;
                }
                target.SetDirty();
            }
            
        }
        
        r.x += WW;
        r.width = 0;
        output = CENTER(r);
        
        EditorGUIUtility.AddCursorRect(result, MouseCursor.Link);
        
        GUI.enabled = e;
        return MT;
    }
    internal static GUIStyle __SIMPLE_STYLE;
    internal static GUIStyle SIMPLE_STYLE
    {   get
        {   if (__SIMPLE_STYLE == null || !__SIMPLE_STYLE.normal.background)
            {   __SIMPLE_STYLE = new GUIStyle();
                __SIMPLE_STYLE.normal.background = IC("BG_simpleBox");
                __SIMPLE_STYLE.border = new RectOffset(5, 5, 5, 5);
            }
            return __SIMPLE_STYLE;
        }
    }
    
    
    
    
    
    static GUIContent EmptyContent = new GUIContent();
    public Rect DRAW_COLOR(Rect r, string message, string fieldName, bool enable, out Vector2 output, bool hasOut = true)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        var result = new Rect(r.x, r.y, 0, 0);
        
        if (r.width == 0) r.width = COLOR_WIDTH;
        FIRST(ref r, ref message, ref result, true);
        /* r.height = FastWaterModel20ControllerEditor.H;
         EditorGUI.HelpBox( r, message, MessageType.None );
         r.y += r.height;
         r.height = FastWaterModel20ControllerEditor.H + 8;
        
         if (Event.current.type == EventType.Repaint) SIMPLE_STYLE.Draw( r, EmptyContent, 0 );
         result.width = r.x + r.width - result.x;
         result.height = r.y + r.height - result.y;
         r = SHRINK( r, 4 );
         r.width -= 15;*/
        
        if (target.compiler.HasProperty(fieldName))
        {   var MT = !target.compiler.HasProperty(fieldName) ? Color.white : target.compiler.GetColor(fieldName);
            #if UNITY_2018_1_OR_NEWER
            var NEW_MT = EditorGUI.ColorField(r, EmptyContent, MT, true, false, false);
            #else
            var NEW_MT = EditorGUI.ColorField( r, EmptyContent, MT, true, false, false, null);
            #endif
            if (MT != NEW_MT)
            {   target.Undo();
                target.compiler.SetColor(fieldName, NEW_MT);
                target.SetDirty();
                EditorGUIUtility.ExitGUI();
            }
            if (hasOut)
            {   r.x += r.width + 1;
                r = DRAW_DOT_BLUE(r, enable);
            }
            
        }
        else
        {   EditorGUI.DrawRect(r, Color.red);
        }
        
        output = CENTER(r);
        
        
        
        GUI.enabled = e;
        return result;
    }
    
#pragma warning disable
    Vector2 DRAW_SLIDER_V;
#pragma warning restore
    
    public Rect DRAW_SLIDER_WITHTOGGLE(Rect r, string message, string fieldName, float leftValue, float rightValue, bool enable, string toogleFieldName, float div = 1)
    {   // var oldV = target.compiler.GetFloat(fieldName);
        var result = DRAW_SLIDER(r, message, fieldName, ref fake_float, leftValue, rightValue, enable, out DRAW_SLIDER_V, div: div);
        var nV = target.compiler.GetFloat(fieldName);
        /*if (oldV != nV && (oldV == 0 || nV == 0) && (oldV != 0 || nV != 0))
        {
            if (nV != 0) target.compiler.EnableKeyword(toogleFieldName);
            else target.compiler.DisableKeyword(toogleFieldName);
        }*/
        if (nV != 0 && !target.compiler.IsKeywordEnabled(toogleFieldName)) target.compiler.EnableKeyword(toogleFieldName);
        if (nV == 0 && target.compiler.IsKeywordEnabled(toogleFieldName)) target.compiler.DisableKeyword(toogleFieldName);
        
        return result;
    }
    
    
    public Rect DRAW_SLIDER(Rect r, string message, string fieldName, float leftValue, float rightValue, bool enable, bool useColor = false, bool inverce = false, bool integer = false, float div = 1)
    {   return DRAW_SLIDER(r, message, fieldName, ref fake_float, leftValue, rightValue, enable, out DRAW_SLIDER_V, useColor, useOut: false, inverce: inverce, integer: integer, div: div);
    }
    
    static float? __SCROLL_X;
    static float SCROLL_X
    {   get { return __SCROLL_X ?? (__SCROLL_X = EditorPrefs.GetFloat("EModules/Water20/SCROLL_X")).Value; }
        set
        {   if (__SCROLL_X == value) return;
            EditorPrefs.SetFloat("EModules/Water20/USE_FIELDS", value);
            __SCROLL_X = value;
        }
    }
    static bool? __USE_FIELDS;
    static bool USE_FIELDS
    {   get { return __USE_FIELDS ?? (__USE_FIELDS = EditorPrefs.GetBool("EModules/Water20/USE_FIELDS")).Value; }
        set
        {   if (__USE_FIELDS == value) return;
            EditorPrefs.SetBool("EModules/Water20/USE_FIELDS", value);
            __USE_FIELDS = value;
        }
    }
    
    float FLOAT_FIELD(Rect r, float value, float leftValue, float rightValue, string fieldName, string message, float div = 1)
    {   var oc = GUI.color;
        if (fieldName == null) fieldName = "";
        if (message == null) message = "";
        if (!string.IsNullOrEmpty(fieldName) && (fieldName.ToLower().Contains("amount") || message.ToLower().Contains("amount"))) GUI.color *= new Color32(255, 235, 180, 255);
        float result;
        if (USE_FIELDS)
        {   result = Mathf.Clamp(EditorGUI.FloatField(new Rect(EditorGUIUtility.singleLineHeight * 2, r.y, r.width, r.height), EmptyContent, value * div) / div, leftValue, rightValue);
        }
        else result = EditorGUI.Slider(r, value * div, leftValue * div, rightValue * div) / div;
        
        GUI.color = oc;
        return result;
    }
    float fake_float;
    public Rect DRAW_SLIDER(Rect r, string message, string fieldName, float leftValue, float rightValue, bool enable, out Vector2 output, bool useColor = false, bool useOut = true, bool inverce = false,
                            bool integer = false, float div = 1)
    {   fake_float = 1;
        return DRAW_SLIDER(r, message, fieldName, ref fake_float, leftValue, rightValue, enable, out output, useColor, useOut: false, inverce: inverce, integer: integer, div: div);
    }
    public Rect DRAW_SLIDER(Rect r, string message, string fieldName, ref float compilerValue, float leftValue, float rightValue, bool enable, out Vector2 output, bool useColor = false,
                            bool useOut = false, bool inverce = false, bool integer = false, float div = 1)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        
        var result = new Rect(r.x, r.y, 0, 0);
        
        if (fieldName != null && !target.compiler.HasProperty(fieldName))
        {   EditorGUI.DrawRect(r, Color.red);
            output = Vector2.zero;
            return r;
        }
        
        if (r.width == 0) r.width = COLOR_WIDTH;
        FIRST(ref r, ref message, ref result, useOut);
        
        if (fieldName == null || target.compiler.HasProperty(fieldName))
        {   var MT = fieldName == null ? compilerValue : !target.compiler.HasProperty(fieldName) ? 0 : (useColor ? target.compiler.GetColor(fieldName).a : target.compiler.GetFloat(fieldName));
            if (!target.skipClamp) MT = Mathf.Clamp(MT, leftValue, rightValue);
            GUI.BeginClip(r);
            var NEW_MT = 0f;
            if (inverce)
            {   MT = (rightValue - leftValue) - (MT - leftValue);
                NEW_MT = FLOAT_FIELD(new Rect(0, 0, r.width, r.height), MT, 0, rightValue - leftValue, fieldName, message, div);
            }
            else
            {   NEW_MT = FLOAT_FIELD(new Rect(0, 0, r.width, r.height), MT, leftValue, rightValue, fieldName, message, div);
            }
            
            EditorGUI.DrawRect(r, Color.red);
            
            if (integer)
            {   if (NEW_MT < MT) NEW_MT = Mathf.FloorToInt(NEW_MT);
                if (NEW_MT > MT) NEW_MT = Mathf.CeilToInt(NEW_MT);
            }
            if (inverce) NEW_MT = ((rightValue - leftValue) - NEW_MT) + leftValue;
            GUI.EndClip();
            
            GUI.Label(new Rect(r.x, r.y, r.width / 3, r.height), "└───");
            if (MT != NEW_MT)
            {   target.Undo();
                if (fieldName == null)
                {   compilerValue = NEW_MT;
                }
                else if (useColor)
                {   var c = target.compiler.GetColor(fieldName);
                    c.a = NEW_MT;
                    target.compiler.SetColor(fieldName, c);
                }
                else target.compiler.SetFloat(fieldName, NEW_MT);
                target.SetDirty();
            }
        }
        else
        {   EditorGUI.DrawRect(r, Color.red);
        }
        
        Vector2 __output = Vector2.zero;
        SECOND(ref r, enable, ref __output, useOut);
        output = __output;
        GUI.enabled = e;
        
        return result;
    }
    
    
    public Rect DRAW_VECTOR_WITHTOGGLE(Rect r, string message, string fieldName, float leftValue, float rightValue, bool enable, string toogleFieldName, float div = 1)
    {   //var oldV = target.compiler.GetVector(fieldName);
        var result = DRAW_VECTOR(r, message, fieldName, leftValue, rightValue, enable, div: div);
        var nV = target.compiler.GetVector(fieldName);
        /*if (oldV != nV && (oldV.x == 0 && oldV.y == 0 || nV.x == 0 && nV.y == 0) && (oldV.x != 0 || oldV.y != 0 || nV.x != 0 || nV.y != 0))
        {
            if (nV.x != 0 || nV.y != 0) target.compiler.EnableKeyword(toogleFieldName);
            else target.compiler.DisableKeyword(toogleFieldName);
        }*/
        if ((nV.x != 0 || nV.y != 0) && !target.compiler.IsKeywordEnabled(toogleFieldName)) target.compiler.EnableKeyword(toogleFieldName);
        if ((nV.x == 0 && nV.y == 0) && target.compiler.IsKeywordEnabled(toogleFieldName)) target.compiler.DisableKeyword(toogleFieldName);
        
        return result;
    }
    
    public Rect DRAW_VECTOR(Rect r, string message, string fieldName, float leftValue, float rightValue, bool enable, bool useSecondPair = false, int useOneIndex = 3, float div = 1)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        var result = new Rect(r.x, r.y, 0, 0);
        
        if (r.width == 0) r.width = COLOR_WIDTH;
        FIRST(ref r, ref message, ref result, false);
        
        if (target.compiler.HasProperty(fieldName))
        {   var MT = !target.compiler.HasProperty(fieldName) ? Vector4.zero : target.compiler.GetVector(fieldName);
            var NEW_MT = MT;
            var I = useSecondPair ? 2 : 0;
            var R = r;
            if (useOneIndex == 3) R.width /= 2;
            if ((useOneIndex & 1) == 1)
            {   GUI.BeginClip(R);
                var CR = R;
                CR.x = CR.y = 0;
                NEW_MT[I] = Mathf.Clamp(FLOAT_FIELD(CR, MT[I], leftValue, rightValue, fieldName, message, div), leftValue, rightValue);
                GUI.EndClip();
                R.x += R.width;
            }
            
            if ((useOneIndex & 2) == 2)
            {   GUI.BeginClip(R);
                var CR = R;
                CR.x = CR.y = 0;
                NEW_MT[I + 1] = Mathf.Clamp(FLOAT_FIELD(CR, MT[I + 1], leftValue, rightValue, fieldName, message, div), leftValue, rightValue);
                GUI.EndClip();
            }
            
            if (MT != NEW_MT)
            {   target.Undo();
                target.compiler.SetVector(fieldName, NEW_MT);
                target.SetDirty();
            }
        }
        else
        {   EditorGUI.DrawRect(r, Color.red);
        }
        
        
        Vector2 __output = Vector2.zero;
        SECOND(ref r, enable, ref __output, false);
        GUI.enabled = e;
        return result;
    }
    
    public Rect DRAW_DOUBLEFIELDS(Rect r, string message, string[] fieldName, float[] leftValue, float[] rightValue, bool enable, Color? firstColor = null, float? firstDiv = null, float? seconddiv = null)
    {   var e = GUI.enabled;
        GUI.enabled = enable;
        var result = new Rect(r.x, r.y, 0, 0);
        
        if (r.width == 0) r.width = COLOR_WIDTH;
        FIRST(ref r, ref message, ref result, false);
        
        
        var R = r;
        R.width /= 2;
        
        for (int i = 0; i < 2; i++)
        {   if (!target.compiler.HasProperty(fieldName[i]))
            {   EditorGUI.DrawRect(R, Color.red);
                if (i == 0) R.x += R.width;
                continue;
            }
            var MT = !target.compiler.HasProperty(fieldName[i]) ? 0 : target.compiler.GetFloat(fieldName[i]);
            if (!target.skipClamp) MT = Mathf.Clamp(MT, leftValue[i], rightValue[i]);
            GUI.BeginClip(R);
            var CR = R;
            CR.x = CR.y = 0;
            
            var oc = GUI.color;
            if (firstColor.HasValue && i == 0) GUI.color *= firstColor.Value;
            float div = 1;
            if (seconddiv.HasValue && i == 1) div = seconddiv.Value;
            if (firstDiv.HasValue && i == 0) div = firstDiv.Value;
            var NEW_MT = FLOAT_FIELD(CR, MT, leftValue[i], rightValue[i], fieldName[i], null, div);
            GUI.color = oc;
            
            GUI.EndClip();
            
            if (MT != NEW_MT)
            {   target.Undo();
                target.compiler.SetFloat(fieldName[i], NEW_MT);
                target.SetDirty();
            }
            
            if (i == 0) R.x += R.width;
        }
        
        
        Vector2 __output = Vector2.zero;
        SECOND(ref r, enable, ref __output, false);
        GUI.enabled = e;
        return result;
    }
    
    
    void FIRST(ref Rect r, ref string message, ref Rect result, bool useOut)
    {   r.height = FastWaterModel20ControllerEditor.H;
        if (message != null)
        {   EditorGUI.HelpBox(r, message, MessageType.None);
            r.y += r.height;
        }
        
        r.height = FastWaterModel20ControllerEditor.H + 8;
        
        if (Event.current.type == EventType.Repaint) SIMPLE_STYLE.Draw(r, EmptyContent, 0);
        result.width = r.x + r.width - result.x;
        result.height = r.y + r.height - result.y;
        r = SHRINK(r, 4);
        if (useOut) r.width -= 15;
    }
    void SECOND(ref Rect r, bool enable, ref Vector2 output, bool useOut)
    {   if (useOut)
        {   r.x += r.width + 1;
            r = DRAW_DOT_BLUE(r, enable);
        }
        else
        {   r.x += r.width;
            r.width = 0;
        }
        output = CENTER(r);
    }
    
    
    static Vector2 CENTER(Rect label_R)
    {   return (new Vector2(label_R.x + label_R.width / 2, label_R.y + label_R.height / 2));
    }
    static Rect SPACE_X(Rect r, int space)
    {   r.x += space;
        r.width -= space;
        return r;
    }
    static Rect BOXED(Rect r)
    {   r.width = r.height;
        return r;
    }
    static Rect SHRINK(Rect r, int off)
    {   r.x += off;
        r.y += off;
        r.width -= off * 2;
        r.height -= off * 2;
        return r;
    }
    
    static GUIStyle __bs;
    static GUIStyle BS { get { return __bs ?? (__bs = new GUIStyle() { active = new GUIStyleState() { background = IC("HOVER") } }); } }
    
    static Rect DRAW_MASKEDTEXTURE(string texture, Rect r, bool enable = true)
    {   GUI.BeginClip(r);
        DRAW_TEXTURE(texture, 0, 0, (int)r.width, (int)r.height, enable);
        GUI.EndClip();
        return r;
    }
    static Rect DRAW_TEXTURE_ALIGNX(string texture, Rect r, bool enable = true)
    {   r.x -= IC(texture).width / 2;
        return DRAW_TEXTURE(texture, (int)r.x, (int)r.y, (int)r.width, (int)r.height, enable);
    }
    static Rect DRAW_TEXTURE_ALIGNY(string texture, Rect r, bool enable = true)
    {   r.y -= IC(texture).height / 2;
        return DRAW_TEXTURE(texture, (int)r.x, (int)r.y, (int)r.width, (int)r.height, enable);
    }
    static Rect DRAW_TEXTURE(string texture, Rect r, bool enable = true)
    {   return DRAW_TEXTURE(texture, (int)r.x, (int)r.y, (int)r.width, (int)r.height, enable);
    }
    static Rect DRAW_TEXTURE(string texture, int x, int y, int width, int height, bool enable = true)
    {   Rect drawRect = new Rect();
        var t = IC(texture);
        var dX = Mathf.Max(0, width - t.width) / 2;
        var dY = Mathf.Max(0, height - t.height) / 2;
        drawRect.Set(x + dX, y + dY, t.width, t.height);
        var oc = GUI.color;
        if (!enable) GUI.color *= new Color(1, 1, 1, 0.4f);
        GUI.DrawTexture(drawRect, t);
        GUI.color = oc;
        return drawRect;
    }
    
    static Rect DRAW_GRAPHIC(Rect r, float h, Texture2D t, bool enable = true, Color? mask = null, float YUP = 0, float YDOWN = 0)
    {
    
        r.height = h;
        if (Event.current.type != EventType.Repaint) return r;
        var result = r;
        
        if (Event.current.type == EventType.Repaint)
        {   SIMPLE_STYLE.Draw(r, EmptyContent, 0);
        }
        // DRAW_TEXTURE( "BG_simpleBox", r, enable );
        r = SHRINK(r, 7);
        
        if (!t)
        {   EditorGUI.HelpBox(r, "No texture", MessageType.Error);
            return result;
        }
        //  GUI.BeginClip( r );
        
        Rect drawRect = new Rect();
        var dX = Mathf.Max(r.x, r.width - t.width) / 2;
        var dY = Mathf.Max(r.y, r.height - t.height) / 2;
        drawRect.Set(0 + dX, 0 + dY, t.width, t.height);
        var oc = GUI.color;
        if (!enable) oc *= new Color(1, 1, 1, 0.4f);
        if (mask.HasValue)
        {   if (mask.Value.r == 0 && mask.Value.g == 0 && mask.Value.b == 0)
            {   //GUI.DrawTexture( drawRect, Texture2D.whiteTexture );
            }
            ChannelRemover.SetColor("_Mask", mask.Value);
        }
        else ChannelRemover.SetColor("_Mask", Color.white);
        ChannelRemover.SetTexture("_MainTex", t);
        
        if (enable)
            EditorGUI.DrawRect(r, new Color(0, 0, 0, 1));
            
        if (Event.current.type == EventType.Repaint)
        {   // GL.Clear( true, false, Color.white );
            GL.PushMatrix();
            //GL.Viewport( new Rect( 0, -20, Screen.width, Screen.height ) );
            ChannelRemover.SetPass(0);
            //GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(oc);
            GL.TexCoord2(0, 1 - YUP);
            GL.Vertex3(r.x, r.y, 0);
            GL.TexCoord2(0, 0 + YDOWN);
            GL.Vertex3(r.x, r.y + r.height, 0);
            GL.TexCoord2(1, 0 + YDOWN);
            GL.Vertex3(r.x + r.width, r.y + r.height, 0);
            GL.TexCoord2(1, 1 - YUP);
            GL.Vertex3(r.x + r.width, r.y, 0);
            GL.End();
            GL.PopMatrix();
        }
        // Graphics.DrawTexture( drawRect, t, new Rect( 0, 0, t.width, t.height ), 0, 0, 0, 0, oc, ChannelRemover );
        // GUI.EndClip();
        return result;
    }
    
    
    void SetMask(string field, int newMask)
    {   target.Undo();
        target.compiler.SetVector(field, target.MaskToVector(newMask));
        target.SetDirty();
    }
    
    
    
    public Rect DRAW_KAYERMASK(Rect r, string label, string field, System.Action callback)
    {
    
        if (!target.compiler.HasProperty(field))
        {   EditorGUI.DrawRect(r, Color.red);
            return r;
        }
        r.height = FastWaterModel20ControllerEditor.H;
        EditorGUI.HelpBox(r, label, MessageType.None); r.y += r.height;
        var mask = (int)target.VectorToMask(target.compiler.GetVector(field));
        var layers = UnityEditorInternal.InternalEditorUtility.layers;
        
        string content = "";
        int maxValue = 0;
        for (int i = 0; i < 32; i++) maxValue |= (1 << i);
        
        if (mask == 0) content += "Nothing";
        else if (mask == maxValue) content += "Everything";
        else
        {   for (int i = 0; i < layers.Length; i++)
            {   // if (i != 0) content += ", ";
                var LM = LayerMask.GetMask(layers[i]);
                var m = LM;
                if ((mask & m) != 0)
                {   if (content != "") content += ", ";
                    content += layers[i];
                }
            }
        }
        
        if (GUI.Button(r, content, EditorStyles.popup))
        {   var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Nothing"), mask == 0, () => { SetMask(field, 0); callback(); });
            menu.AddItem(new GUIContent("Everything"), mask == maxValue, () => { SetMask(field, maxValue); callback(); });
            menu.AddSeparator("");
            for (int i = 0; i < layers.Length; i++)
            {   var LM = LayerMask.GetMask(layers[i]);
                var m = LM;
                //var m = (1<<LM);
                if (string.IsNullOrEmpty(layers[i]) && (mask & m) == 0) continue;
                menu.AddItem(new GUIContent(layers[i]), (mask & m) != 0, () =>
                {   if ((mask & m) != 0) mask &= ~m;
                    else mask |= m;
                    SetMask(field, mask);
                    callback();
                });
            }
            menu.ShowAsContext();
        }
        
        return r;
    }
    
    float LAST_DIRECTION_VALUE;
    public Rect DIRECTION(Rect r, string fieldName, bool enalbe, bool VECTOR = true, float? leftClamp = null, float? rightClamp = null, float? value = null)
    {   var en = GUI.enabled;
        GUI.enabled = enalbe;
        
        if (fieldName != null && !target.compiler.HasProperty(fieldName))
        {   EditorGUI.DrawRect(r, Color.red);
            return r;
        }
        double oldV = 0;
        
        if (VECTOR)
        {   var v = (Vector3)target.compiler.GetVector(fieldName);
            v = v.normalized;
            oldV = System.Math.Atan2(v.z, v.x) / System.Math.PI * 180;
        }
        else
        {   if (value.HasValue) oldV = value.Value / System.Math.PI * 180;
            else oldV = target.compiler.GetFloat(fieldName) / System.Math.PI * 180;
        }
        
        
        r.height = FastWaterModel20ControllerEditor.H + 8;
        var result = r;
        
        GUI.BeginClip(r);
        // var OR = r;
        r.x = 0;
        r.y = 0;
        
        if (Event.current.type == EventType.Repaint) SIMPLE_STYLE.Draw(r, EmptyContent, 0);
        
        r = SHRINK(r, 4);
        
        var fixOldV = -(float)oldV / 180f * Mathf.PI;
        var newV = FLOAT_FIELD(r, fixOldV, leftClamp ?? -Mathf.PI, rightClamp ?? Mathf.PI, null, null);
        var cr = new Rect(r.x, r.y, r.width / 3, r.height);
        DrawCircle(new Rect(cr.x, cr.y, r.height, r.height), newV * 180f / Mathf.PI, enalbe);
        GUI.Label(new Rect(cr.x + cr.width / 2, cr.y, cr.width / 2, cr.height), " ─"); //└
        //var newV = EditorGUI.Slider(r, (float)oldV / 180f, -1, 1);
        if (fixOldV != newV)
        {   newV = -newV * 180f / Mathf.PI;
            target.Undo();
            if (VECTOR) target.compiler.SetVector(fieldName, new Vector3((float)System.Math.Cos(newV * System.Math.PI / 180), 0, (float)System.Math.Sin(newV * System.Math.PI / 180)).normalized);
            else if (!value.HasValue) target.compiler.SetFloat(fieldName, (float)(newV * System.Math.PI / 180));
            else LAST_DIRECTION_VALUE = (float)(newV * System.Math.PI / 180);
            target.SetDirty();
        }
        else if (value.HasValue)
            LAST_DIRECTION_VALUE = value.Value;
            
        GUI.EndClip();
        GUI.enabled = en;
        
        return result;
    }
    
    public void DrawCircle(Rect r, float angle, bool enable)
    {
    
        if (Event.current.type == EventType.Repaint)
        {   ChannelRemover.SetColor("_Mask", Color.white);
            ChannelRemover.SetTexture("_MainTex", IC("TUNNING"));
            GL.Clear(false, false, new Color(1, 1, 1, 1));
            GL.PushMatrix();
            /*var translate = new Vector3(LAST_GROUP_R.x + r.x + r.width/2,LAST_GROUP_R.y + r.y + r.height / 2, 0);
             GL.MultMatrix( Matrix4x4.Translate( translate ) );
             GL.MultMatrix( Matrix4x4.Rotate(  ) );
             GL.MultMatrix( Matrix4x4.Translate( -translate ) );*/
            var rot = Quaternion.Euler(0, 0, -angle + 90);
            var v1 = rot * new Vector2(-0.5f, -0.5f) + new Vector3(0.5f, 0.5f);
            var v2 = rot * new Vector2(0.5f, -0.5f) + new Vector3(0.5f, 0.5f);
            var v3 = rot * new Vector2(0.5f, 0.5f) + new Vector3(0.5f, 0.5f);
            var v4 = rot * new Vector2(-0.5f, 0.5f) + new Vector3(0.5f, 0.5f);
            //GL.Viewport( new Rect( 0, -20, Screen.width, Screen.height ) );
            ChannelRemover.SetPass(0);
            //GL.LoadPixelMatrix();
            var oc = GUI.color;
            if (!enable) oc *= new Color(1, 1, 1, 0.4f);
            
            GL.Begin(GL.QUADS);
            GL.Color(oc);
            GL.TexCoord2(v1.x, v1.y);
            GL.Vertex3(r.x, r.y, 0);
            GL.TexCoord2(v2.x, v2.y);
            GL.Vertex3(r.x + r.width, r.y, 0);
            GL.TexCoord2(v3.x, v3.y);
            GL.Vertex3(r.x + r.width, r.y + r.height, 0);
            GL.TexCoord2(v4.x, v4.y);
            GL.Vertex3(r.x, r.y + r.height, 0);
            GL.End();
            GL.PopMatrix();
        }
    }
    
    
    //-------------------------------------
    #endregion//-----DRAWING TOOLS
    //-------------------------------------
    
    
    static Material __channelRemover;
    static Material ChannelRemover
    {   get
        {   if (__channelRemover == null)
            {   __channelRemover = EditorUtility.InstanceIDToObject(EditorPrefs.GetInt("EModules/Water20/CHR", -1)) as Material;
                if (!__channelRemover)
                {   __channelRemover = new Material(Shader.Find("Hidden/EM-X/ChannerRemover"));
                    __channelRemover.hideFlags = HideFlags.HideAndDontSave;
                    EditorPrefs.SetInt("EModules/Water20/CHR", __channelRemover.GetInstanceID());
                }
            }
            return __channelRemover;
        }
    }
    
    
    
    
    
    static Texture2D IC(string key)
    {   if (!__ICONS.ContainsKey(key.ToLower())) Debug.Log(key);
        return __ICONS[key.ToLower()];
    }
    
    
    
    
    
    static Rect R(float x, float y)
    {   return new Rect(x, y, 0, 0);
    }
    
    public class IDrawable {
        public IDrawable() { }
        public IDrawable(FastWaterModel20ControllerEditor target) { this.target = target; }
        public FastWaterModel20ControllerEditor target;
        virtual public void Draw(Rect input, out Vector2[] output) { throw new System.Exception(); }
    }
    /* if (GUI.Button( DRAW_TEXTURE( value ? "CHECK_ON" : "CHECK_OFF", BOXED( R(0,0) ) ), "", BS )) {
          value = !value;
        }*/
    
    
    static Vector2 Offset_X(Vector2 v, float value)
    {   v.x += value;
        return v;
    }
    static Vector2 Offset_Y(Vector2 v, float value)
    {   v.y += value;
        return v;
    }
    
    
    
    public void VECTOR_FIELD(ref Rect r, string title, string fieldName)
    {   r.height = FastWaterModel20ControllerEditor.H;
        EditorGUI.HelpBox(r, title, MessageType.None);
        r.y += r.height;
        var oldV = target.compiler.GetVector(fieldName);
        // var newV = EditorGUI.Vector2Field(r, "", (Vector2)oldV);
        // if (newV != (Vector2)oldV)
        var RECT = r;
        RECT.width = RECT.width / 4;
        var newV = oldV;
        for (int i = 0; i < 4; i++)
        {   GUI.BeginClip(RECT);
            var fs = GUI.skin.textField.fontSize;
            GUI.skin.textField.fontSize = (int)(GUI.skin.textField.fontSize * 0.7f);
            newV[i] = FLOAT_FIELD(new Rect(0, 0, RECT.width, RECT.height), oldV[i], -50, 50, fieldName, null);
            GUI.skin.textField.fontSize = GUI.skin.textField.fontSize;
            GUI.EndClip();
            RECT.x += RECT.width;
        }
        // var newV = EditorGUI.Vector4Field(r, "", oldV);
        if (newV != oldV)
        {   var nv4 = (Vector4)newV;
            //nv4.z = oldV.z;
            // nv4.w = oldV.w;
            target.Undo();
            target.compiler.SetVector(fieldName, nv4);
            target.SetDirty();
        }
        r.y += r.height;
    }
    
    static Vector2 SUM(Vector2 v1, Vector2 v2) { return new Vector2(v1.x + v2.x, v1.y + v2.y); }
    static Vector2 SUM(Vector2 v1, Rect v2) { return new Vector2(v1.x + v2.x, v1.y + v2.y); }
    static Vector2 SUM(Rect v1, Vector2 v2) { return new Vector2(v1.x + v2.x, v1.y + v2.y); }
    static Vector2 SUM(Rect v1, Rect v2) { return new Vector2(v1.x + v2.x, v1.y + v2.y); }
    static Vector2 tV;
    static Rect LAST_GROUP_R;
    static Vector2 R_to_V(Rect r) { return new Vector2(r.x, r.y); }
    static Rect V_to_R(Vector2 r) { return new Rect(r.x, r.y, 0, 0); }
    
    
}



#endif

}

