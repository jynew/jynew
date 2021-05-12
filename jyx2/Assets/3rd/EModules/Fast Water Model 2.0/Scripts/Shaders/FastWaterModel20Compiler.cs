#define USE_STRIP_UNIFORMS
//#define COMPILE_TO_BUFFER
//#define COMPILE_TO_BUFFER_AND_DONTWRITE_TOSHADER
//#define OVERRIDE_OPTIMIZER

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif



#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
public partial class FastWaterModel20Compiler : ScriptableObject {

    [SerializeField]
    public Material Support128Shader = null;
    [SerializeField]
    public Shader DepthRenderShader_SP = null;
    [SerializeField]
    public Shader DepthRenderShader = null;
    [SerializeField]
    public Shader RefractionRenderShader = null;
    
    //[SerializeField]
    //bool __UseBufferOptimization = false;
    /* internal bool UseBufferOptimization
     {   get { return __BufferResolution == 0;}
         set
         {   if (value && __BufferResolution == 0) __BufferResolution = 1;
             if (!value) __BufferResolution = 0;
    
             if (__BufferResolution == 0)EModules.FastWaterModel20.FastWaterModel20FrameBuffer.DestroyAllScript();
         }
     }*/
    
    [System.NonSerialized]
    public int? __cID;
    public int cID
    {   get { return __cID ?? (__cID = Application.isPlaying ? -1 : GetInstanceID()).Value; }
        set { __cID = value;}
    }
    
    [SerializeField]
    int _BufferResolution = 0;
    [System.NonSerialized]
    internal int? BufferResolutionOverride;
    internal int BufferResolution
    {   get
        {   if (Application.isPlaying) return BufferResolutionOverride ?? _BufferResolution;
            return _BufferResolution;
        }
        set
        {   if (Application.isPlaying)
            {   BufferResolutionOverride = value;
            }
            else
            {   if (_BufferResolution == value)return;
                _BufferResolution = value;
            }
            if (BufferResolution == 0)EModules.FastWaterModel20.FastWaterModel20FrameBuffer.DestroyAllScript();
            if (onShaderUpdate != null)
                onShaderUpdate(this);
            // EModules.FastWaterModel20.FastWaterModel20Controller.need_update = true;
        }
    }
    public System.Action<FastWaterModel20Compiler> onShaderUpdate;
    
    #if UNITY_EDITOR
    static FastWaterModel20Compiler()
    {   Undo.undoRedoPerformed -= UndoPerf;
        Undo.undoRedoPerformed += UndoPerf;
        
    }
    
    
    private void OnEnable()
    {
    
        if (!__cID.HasValue)  __cID = GetInstanceID();
        Undo.undoRedoPerformed -= UndoPerf;
        Undo.undoRedoPerformed += UndoPerf;
    }
    static void UndoPerf()
    {   ___ketwords_cache = null;
    }
    
    const string prefs_EnableAutoREcompile = "EModules/Water20/EnableAutoREcompile";
    const string prefs_EnableAutoREcompileEMFastWaterModel20Compiler = "EModules/Water20/EnableAutoREcompileEMFastWaterModel20Compiler";
    
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Enable Auto Compile when 'EModules Water Model ... .cginc' changed", true, 100)]
    public static bool EnableAutoREcompileValidate() { return !EditorPrefs.GetBool(prefs_EnableAutoREcompile, false); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Enable Auto Compile when 'EModules Water Model ... .cginc' changed", false, 100)]
    static void EnableAutoREcompile() { EditorPrefs.SetBool(prefs_EnableAutoREcompile, true); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Disable Auto Compile when 'EModules Water Model ... .cginc' changed", true, 100)]
    static bool DisableAutoREcompileValidate() { return EditorPrefs.GetBool(prefs_EnableAutoREcompile, false); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Disable Auto Compile when 'EModules Water Model ... .cginc' changed", false, 100)]
    static void DisableAutoREcompile() { EditorPrefs.SetBool(prefs_EnableAutoREcompile, false); }
    
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Enable Auto Compile when 'EMFastWaterModel20Compiler.cs' changed", true, 1000)]
    public static bool EnableAutoREcompileprefs_EnableAutoREcompileEMFastWaterModel20CompilerValidate() { return !EditorPrefs.GetBool(prefs_EnableAutoREcompileEMFastWaterModel20Compiler, false); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Enable Auto Compile when 'EMFastWaterModel20Compiler.cs' changed", false, 1000)]
    static void EnableAutoREcompileprefs_EnableAutoREcompileEMFastWaterModel20Compiler() { EditorPrefs.SetBool(prefs_EnableAutoREcompileEMFastWaterModel20Compiler, true); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Disable Auto Compile when 'EMFastWaterModel20Compiler.cs' changed", true, 1000)]
    static bool DisableAutoREcompileprefs_EnableAutoREcompileEMFastWaterModel20CompilerValidate() { return EditorPrefs.GetBool(prefs_EnableAutoREcompileEMFastWaterModel20Compiler, false); }
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Disable Auto Compile when 'EMFastWaterModel20Compiler.cs' changed", false, 1000)]
    static void DisableAutoREcompilFastWaterModel20Compiler() { EditorPrefs.SetBool(prefs_EnableAutoREcompileEMFastWaterModel20Compiler, false); }
    
    [MenuItem("Tools/Fast Water Model 2.0 ( EMX )/Recompile All Water's Shaders", false, 100000)]
    static void RecompileAll() { RecompileAll(-1); }
    static void RecompileAll(int yep)
    {   System.Action<FastWaterModel20Compiler> action = (loaded) => { loaded.UpdateShaderAsset(); };
        ApplyToAll("Compile Water Model 2.0", action, yep);
    }
    public static void ApplyToAll(string actionName, System.Action<FastWaterModel20Compiler> action, int param = 0)
    {   EditorUtility.DisplayProgressBar(actionName, "Completed:", 0);
        List<FastWaterModel20Compiler> result = new List<FastWaterModel20Compiler>();
        List<FastWaterModel20Compiler> resultfix = new List<FastWaterModel20Compiler>();
        
        WaterShaderType? wt = null;
        EModules.FastWaterModel20.FastWaterModel20Controller __go = null;
        if (param < 1)
        {   var go = Selection.gameObjects.FirstOrDefault();
            if (go)
            {   var c = go.GetComponent<EModules.FastWaterModel20.FastWaterModel20Controller>();
                if (c && c.compiler)
                {   wt = c.compiler.WaterType;
                    __go = c;
                }
            }
        }
        
        if (param != 0)
        {   if (param == 1)
                wt = WaterShaderType.Minimum;
            if (param == 2)
                wt = WaterShaderType.UltraFast;
            if (param == 3)
                wt = WaterShaderType.AdvancePC;
        }
        
        
        foreach (var item in AssetDatabase.GetAllAssetPaths())
        {   // string wt_marker = null;
            /* */
            
            if (item.EndsWith(".asset"))
            
            {   var loaded = AssetDatabase.LoadAssetAtPath<FastWaterModel20Compiler>(item);
                if (loaded)
                {   resultfix.Add(loaded);
                
                    if (wt == null)
                    {   if (loaded.WaterType == wt)
                            result.Add(loaded);
                    }
                    else
                    {   if (loaded.WaterType == wt)
                            result.Add(loaded);
                    }
                    
                }
            }
        }
        
        var tt = result.Count != 0 ? result : resultfix;
        
        if (tt.Count != 0)
        {   for (int i = 0; i < tt.Count; i++)
            {   var loaded = tt[i];
                if (__go != null && param != -1 && loaded != __go.compiler)
                    continue;
                EditorUtility.DisplayProgressBar("Compile", "Completed: " + (i + 1) + "/" + (tt.Count) + " - " + loaded.name, (i + 1f) / (tt.Count));
                // EModules.FastWaterModel20.FastWaterModel20Controller.UpdateShaderAsset(loaded.shader, loaded.CreateShaderString());
                action(loaded);
            }
            
        }
        
        EditorUtility.ClearProgressBar();
    }
    
    class FileModificationWarningA : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {   var __prefs_EnableAutoREcompile = EditorPrefs.GetBool(prefs_EnableAutoREcompile, false);
            var __FastWaterModel20Compiler = EditorPrefs.GetBool(prefs_EnableAutoREcompileEMFastWaterModel20Compiler, false);
            if (!EditorPrefs.GetBool(prefs_EnableAutoREcompile, false))
                return;
            int yep = 0;
            for (int i = 0; i < importedAssets.Length; i++)
            {
            
                if (__prefs_EnableAutoREcompile && importedAssets[i].EndsWith(".cginc"))
                {   _uniformFileRead = null;
                    propsplit = null;
                    if (importedAssets[i].Contains("EModules Water Model"))
                        // if (importedAssets[i].EndsWith("EModules Water Model 2.0 v1.cginc") || importedAssets[i].EndsWith("EModules Water Model 2.0 uniform.cginc"))
                    {   var sub = importedAssets[i].Substring(importedAssets[i].LastIndexOf('/'));
                        sub = sub.Remove(sub.Length - 9);
                        if (sub.EndsWith("Ultra Fast Mode"))
                            yep |= 2;
                        else if (sub.EndsWith("Water Model 2.0"))
                            yep |= 3;
                        else if (sub.EndsWith("Minimum Mode"))
                            yep |= 1;
                        else
                            yep = -2;
                        break;
                        
                    }
                }
                if (__FastWaterModel20Compiler && importedAssets[i].EndsWith(".cs"))
                    if (importedAssets[i].EndsWith("FastWaterModel20Compiler.cs"))
                    {   yep = -2;
                        break;
                    }
            }
            
            
            if (yep != 0)
                RecompileAll(yep);
        }
    }
    public  class shaderCache {
        public string uniform;
        public int texCount;
        public List<string> texNames;
        public Dictionary<string, string> declaredKeys = new Dictionary<string, string>();
    }
    static string _uniformFileRead = null;
    public string GetShaderUniformText()
    {   return _uniformFileRead ?? (_uniformFileRead = System.IO.File.ReadAllText( FastWaterModel20Compiler.CgincPath + "EModules Water Model 2.0 uniform.cginc"));
    }
    public shaderCache GetShaderUniform()
    {   var newsk = new shaderCache();
        // var cnt = 0;
        List<string > samplerCount = new List<string>();
        
        var __file = _uniformFileRead ?? (_uniformFileRead = System.IO.File.ReadAllText( FastWaterModel20Compiler.CgincPath + "EModules Water Model 2.0 uniform.cginc"));
        int scan  = 0;
        while (scan < __file.Length - 1 && __file.IndexOf("/*", scan) != -1)
            //  while (__file.IndexOf("/*", scan) != -1)
        {   scan = __file.IndexOf("/*", scan) + 2;
            var end = __file.IndexOf("*/", scan);
            __file = __file.Remove(scan - 2) + __file.Substring(end + 2);
        }
        scan  = 0;
        while (__file.IndexOf("#ifdef", scan) != -1)
        {   scan = __file.IndexOf("#ifdef", scan) + 6;
            var end = __file.IndexOf("#endif", scan);
            __file = __file.Remove(scan - 6) + "#DEFIF" + __file.Substring(scan );
            __file = __file.Remove(end) + "#DEFEN" + __file.Substring(end + 6);
        }
        
        
        var file = __file.Split('\n').Select(s => s.Trim()).ToArray();
        for (int i = 0; i < file.Length; i++)
        {   if (file[i].IndexOf("//") != -1) file[i] = file[i].Remove(file[i].IndexOf("//"));
            //  if (file[i].StartsWith("#ifdef") ) file[i] = "";
        }
        
        
        List<int> deep = new List<int>();
        deep.Add(1);
        int waitNext = 0;
        int lineind = 0;
        List<string> compilerShader = new List<string>();
        List<string> declaredKeys = new List<string>();
        
        var keylist = _keywords.Distinct().ToDictionary(k => k);
        System.Func<string, bool> enabled = (str) =>
        {   bool inverce = false;
            if (str[0] == '!')
            {   inverce = true;
                str = str.Substring(1);
            }
            var result = keylist.ContainsKey(str);
            if (inverce) result = !result;
            return result;
        };
        
        foreach (var _line in file)
        {   lineind ++;
            var line = _line.Trim();
            
            if (waitNext != 0)
            {   if (line.StartsWith("#endif")) waitNext--;
                if (line.StartsWith("#if")) waitNext++;
                if (waitNext == 1)
                {   if (line.StartsWith("#elif")) waitNext--;
                    if (line.StartsWith("#else")) waitNext--;
                }
                else
                {
                }
            }
            if (waitNext != 0 )
                continue;
                
            var noIf = !line.StartsWith("#if") && !line.StartsWith("#elif") && !line.StartsWith("#else") && !line.StartsWith("#endif");
            
            if (deep.Last() == 1)
            {   var temp = line.Trim();
                if (temp.StartsWith("uniform")) temp = temp.Substring("uniform".Length);
                temp = temp.Trim();
                if (temp.StartsWith("sampler2D")  )samplerCount.Add(temp);
                if (temp.StartsWith("#define ")  )
                {   var fp = temp.IndexOf(' ');
                    var key = temp.Substring(fp, temp.IndexOf(' ', fp + 1) - fp).Trim();
                    if (!keylist.ContainsKey(key))
                    {   keylist.Add(key, null);
                        //  Debug.Log(key);
                    }
                    declaredKeys.Add(key);
                    
                }
                compilerShader.Add(line);
                /* if (noIf)
                     compilerShader.Add(line);
                 else
                     compilerShader.Add("//" + line);*/
            }
            else
            {   if (line.StartsWith("#"))
                {   if (NeedOptimize)                    compilerShader.Add("//" + line);
                    else                     compilerShader.Add(line);
                }
            }
            if (!line.StartsWith("#")) continue;
            
            if (noIf) continue;
            if (line.StartsWith("#if"))
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                deep.Add(0);
            }
            if (line.StartsWith("#else") )
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                if (deep.Last() == 0) deep[deep.Count - 1] = 1;
                else
                {   deep[deep.Count - 1] = 2;
                    waitNext = 1;
                }
                continue;
            }
            if (line.StartsWith("#endif"))
            {   // Debug.Log("REMOVE " + deep.Last() + " " +  line);
                deep.RemoveAt(deep.Count - 1);
                continue;
            }
            if (deep.Last() == 2)
            {   waitNext = 1;
                continue;
            }
            if (line.StartsWith("#elif") && deep.Last() > 0)
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                deep[deep.Count - 1] = 2;
                continue;
            }
            
            var inndd = line.IndexOf(' ');
            var innff = line.IndexOf('(');
            if (innff != -1 && innff < inndd)inndd = innff;
            if (inndd == -1 ) continue;
            
            line = line.Substring(inndd);
            line = line.Replace(" ", "").Replace("&&", "-&&-").Replace("||", "-||-");
            while (line.IndexOf("defined(") != -1)
            {   var d = line.IndexOf("defined(");
                var ca = line.ToCharArray();
                ca[line.IndexOf('(', d)] = '[';
                ca[line.IndexOf(')', d)] = ']';
                line = new string(ca);
            }
            
            
            
            var lb = -1;
            do
            {   lb = line.LastIndexOf('(') ;
                var rb = lb == -1 ? -1 : line.IndexOf(')', lb);
                
                var _expression = lb == -1 ? line : line.Substring(lb + 1, (rb - lb) - 1);
                var expression =         _expression.Split(new[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Replace("defined[", "").Replace("]", ""))
                                         .ToList();
                                         
                                         
                if (expression.Count == 1)
                {   expression[0] = expression[0] == "TRUE" ? "TRUE" : enabled(expression[0]) ? "TRUE" : "FALSE" ;
                }
                else
                {   while (expression.IndexOf("&&") != -1)
                    {   var ind = expression.IndexOf("&&");
                        var left = expression[ind - 1] == "TRUE" || enabled(expression[ind - 1]) ;
                        var right = expression[ind + 1] == "TRUE" || enabled(expression[ind + 1]);
                        expression.Insert(ind - 1, left && right ? "TRUE" : "FALSE" );
                        expression.RemoveAt(ind);
                        expression.RemoveAt(ind);
                        expression.RemoveAt(ind);
                    }
                    while (expression.IndexOf("||") != -1)
                    {   var ind = expression.IndexOf("||");
                    
                        var left = expression[ind - 1] == "TRUE" || enabled(expression[ind - 1]) ;
                        var right = expression[ind + 1] == "TRUE" || enabled(expression[ind + 1]);
                        expression.Insert(ind - 1, left || right ? "TRUE" : "FALSE" );
                        expression.RemoveAt(ind);
                        expression.RemoveAt(ind);
                        expression.RemoveAt(ind);
                    }
                }
                
                if (expression[0] != "TRUE" && expression[0] != "FALSE")
                {   samplerCount = null;
                    // Debug.Log(lineind + " error expression " + expression[0] + " " + expression.Count);
                    break;
                }
                var result = lb == -1 ? "" : line.Remove(lb);
                result += expression[0];
                result += lb != -1 && rb + 1 < line.Length ? line.Substring(rb + 1) : "";
                
                line = result;
            } while (lb != -1);
            
            if (samplerCount == null || line != "TRUE" && line != "FALSE")
            {   samplerCount = null;
                //  Debug.Log(lineind + " error line " + line);
                break;
            }
            if (line == "FALSE") waitNext = 1;
            else deep[deep.Count - 1] = 1;
            //var expression = line.Substring(line.IndexOf(' ')).Split(new[]{' ' } ,System.StringSplitOptions.RemoveEmptyEntries  ) .Select(s=>s.Trim());
        }
        
        //  EditorGUIUtility.systemCopyBuffer = compilerShader.Where(s => !s.Trim().StartsWith("#")).Aggregate((a, b) => a + "\n" + b);
        newsk.uniform = compilerShader.Aggregate((a, b) => a + "\n" + b).Replace("#DEFIF", "#ifdef").Replace("#DEFEN", "#endif");
        
        /*var l  = ShaderUtil.GetPropertyCount(shader);
        for (int i = 0; i < l; i++)
        {   var type = ShaderUtil.GetPropertyType(shader, i);
            if (type == ShaderUtil.ShaderPropertyType.TexEnv )
            {   var name = ShaderUtil.GetPropertyName(shader, i);
        
                if (target.compiler.material.HasProperty(name)  && !ShaderUtil.IsShaderPropertyHidden(shader, i))
                {   cnt ++;
                }
            }
        }*/
        /* foreach (Object obj in EditorUtility.CollectDependencies(new[] {target.RENDERER }))
         {   if (obj is Texture)
             {   cnt ++;
                 Debug.Log(obj.name);
             }
         }*/
        
        newsk.declaredKeys =  declaredKeys.ToDictionary(k => k, _ => "");
        
        newsk.texCount = samplerCount == null ? -1 : samplerCount.Count;
        newsk.texNames =  samplerCount;
        lastCache = newsk;
        return newsk;
        //return newsk;
    }
#pragma warning disable
    shaderCache lastCache;
#pragma warning restore
    public void UpdateShaderAsset(string newShaderName = null /*Shader s, string shader*/)
    {   if (!material)
            return;
        ___ketwords_cache = new Dictionary<FastWaterModel20Compiler, Dictionary<string, bool>>();
        
        var shaderString = CreateShaderString(newShaderName);
        #if COMPILE_TO_BUFFER || COMPILE_TO_BUFFER_AND_DONTWRITE_TOSHADER
        EditorGUIUtility.systemCopyBuffer = shaderString;
        #endif
        #if COMPILE_TO_BUFFER_AND_DONTWRITE_TOSHADER
        return;
        #else
        //ShaderUtil.UpdateShaderAsset(s, shader);
        var path = AssetDatabase.GetAssetPath(shader);
        // path = path.Substring(path.LastIndexOf('/')) + "/" + base.name + ".asset";
        Undo.DestroyObjectImmediate(shader);
        //DestroyImmediate(shader, true);
        //  Debug.Log( path);
        var newSHader = ShaderUtil.CreateShaderAsset(shaderString);
        newSHader.name = base.name;
        AssetDatabase.AddObjectToAsset(newSHader, path);
        //  AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        //  AssetDatabase.SaveAssets();
        Undo.RecordObject(this, "Change EM Water");
        Undo.RecordObject(material, "Change EM Water");
        material.shader = newSHader;
        Undo.RegisterCreatedObjectUndo(newSHader, "Change EM Water");
        
        EModules.FastWaterModel20.FastWaterModel20ControllerEditor.shader_cache.Remove(material.shader);
        EModules.FastWaterModel20.FastWaterModel20ControllerEditor.shader_cache.Add(material.shader, lastCache);
        
        
        if (onShaderUpdate != null)
            onShaderUpdate(this);
        // EditorUtility.SetDirty(this);
        //  UnityEditor.Undo.FlushUndoRecordObjects();
        //
        /* var path = AssetDatabase.GetAssetPath(s);
         System.IO.File.WriteAllText(path, shader);
         AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);*/
        
        #endif
    }
    
    
    
    [System.NonSerialized]
    Shader childShader;
    [System.NonSerialized]
    Material childMaterial;
    bool DC1, DC2;
    void CheckMaterials()
    {   if (childShader == null || childMaterial == null)
        {   var path = AssetDatabase.GetAssetPath(this);
            var all = AssetDatabase.LoadAllAssetsAtPath(path);
            childShader = all.FirstOrDefault(s => s is Shader) as Shader;
            childMaterial = all.FirstOrDefault(m => m is Material) as Material;
            if (childShader == null || childMaterial == null)
                Debug.LogWarning("Compiler cannot load Material or Shader " + name);
        }
        if (childShader != _shader || childMaterial != _material)
        {   _shader = childShader;
            _material = childMaterial;
            EditorUtility.SetDirty(this);
            if (childShader != _shader || childMaterial != _material)
                Debug.LogWarning("Compiler cannot assign Material or Shader " + name);
        }
    }
    #endif
    /* [System.NonSerialized]
     public Material OverrideMaterial;*/
    [SerializeField]
    Material _material = null;
    public Material material
    {   get
        {
            #if UNITY_EDITOR
            CheckMaterials();
            #endif
            return /*OverrideMaterial ??*/ _material;
        }
        set
        {   _material = value;
        }
    }
    [SerializeField]
    Shader _shader = null;
    public Shader shader
    {   // get { CheckMaterials(); return material.shader; }
        get
        {
            #if UNITY_EDITOR
            CheckMaterials();
            #endif
            return _shader;
        }
        set
        {   _shader = value;
        }
    }
    [System.Serializable]
    public enum WaterShaderType { Minimum = 0, UltraFast = 1, AdvancePC = 2/*, UltraHQ = 2*/ }
    
    [SerializeField]
    public float BakedRefractionCameraOffset = 0;
    [SerializeField]
    public float PlanarReflectionClipPlaneOffset = +1;
    [SerializeField]
    public int PlanarReflectionSkipEveryFrame = +0;
    
    //[SerializeField]
    public WaterShaderType WaterType
    {   get
        {   int currentMode = IsKeywordEnabled("ULTRA_FAST_MODE") ? 1 : IsKeywordEnabled("MINIMUM_MODE") ? 0 : 2;
            return (WaterShaderType)currentMode;
        }
    }
    [SerializeField]
    public string[] _keywords = new string[0];
    [System.NonSerialized]
    static Dictionary<FastWaterModel20Compiler, Dictionary<string, bool>> ___ketwords_cache = new Dictionary<FastWaterModel20Compiler, Dictionary<string, bool>>();
    static Dictionary<FastWaterModel20Compiler, Dictionary<string, bool>> _ketwords_cache { get { return ___ketwords_cache ?? (___ketwords_cache = new Dictionary<FastWaterModel20Compiler, Dictionary<string, bool>>()); } }
    Dictionary<string, bool> KeyWrods
    {   get
        {   if (!_ketwords_cache.ContainsKey(this))
                _ketwords_cache.Add(this, _keywords.Distinct().ToDictionary(k => k, k => true));
            return _ketwords_cache[this];
        }
    }
    
    public bool IsKeywordEnabled(string keyword) { return KeyWrods.ContainsKey(keyword); }
    
    #if UNITY_EDITOR
    public void EnableKeyword(string keyword) { if (UnityEditor.ArrayUtility.Contains(_keywords, keyword)) return; UnityEditor.ArrayUtility.Add(ref _keywords, keyword); UpdateCompiler(); }
    public void EnableKeywordAndNotUpdate(string keyword) { if (UnityEditor.ArrayUtility.Contains(_keywords, keyword)) return; UnityEditor.ArrayUtility.Add(ref _keywords, keyword); }
    public void DisableKeyword(string keyword) { _keywords = _keywords.Where(l => l != keyword).ToArray(); UpdateCompiler(); }
    public void DisableKeywordAndNotUpdate(string keyword) { _keywords = _keywords.Where(l => l != keyword).ToArray(); }
    [HideInInspector]
    public bool NowCompile;
    public void UpdateCompiler()
    {   // NowCompile = true;
        //  UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        // _ketwords_cache.Remove(this);
        // EModules.FastWaterModel20.FastWaterModel20Controller.UpdateShaderAsset(shader, CreateShaderString());
        UpdateShaderAsset();
        //  NowCompile = false;
        //  UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
    
    static bool _error = false;
    static string _cgincPath;
    public static string CgincPath
    {   get
        {   if (_cgincPath == null)
            {   var depthShader = Shader.Find("Hidden/EM-X/DepthRender");
                var sp = !depthShader ? null : AssetDatabase.GetAssetPath(depthShader);
                if (string.IsNullOrEmpty(sp))
                {   if (_error)
                        return "";
                    _error = true;
                    EditorUtility.DisplayDialog("EM Water Error", "Can't find '" + "EModules Water Model 2.0 v1.cginc" + "' shader'", "Ok");
                    return "";
                }
                sp = sp.Remove(sp.LastIndexOf('/'));
                _cgincPath = sp + "/";
            }
            return _cgincPath;
        }
    }
    public string GetTypeMarker
    {   get
        {   switch (WaterType)
            {   case WaterShaderType.Minimum: return "Minimum Mode";
                case WaterShaderType.UltraFast: return "Ultra Fast Mode";
                case WaterShaderType.AdvancePC: return "AdvancePC Mode";
                default: throw new System.Exception();
            }
        }
    }
#pragma warning disable
    static string[] propsplit  = null;
#pragma warning restore
    
    string TrimmAll(string line, List<string> trim)
    {   int index = -1;
        while ((index = trim.FindIndex(t => line.EndsWith(t)) ) != -1)
            line = line.Remove(index);
        return line;
    }
    
    public string CreateShaderString(string newName = null)
    {
    
        /* var name = shader.name;
         if (newName != null)
             name = newName;
         // while (name.EndsWith(GetTypeMarker))
         name = TrimmAll(name, new[] {" ", "Minimum Mode", "Ultra Fast Mode", "AdvancePC Mode" } .ToList());*/
        
        name = base.name;
        // name = name + " " + GetTypeMarker;
        while (name.IndexOf("  ") != -1)
            name = name.Replace("  ", " ");
        // name = EModules.FastWaterModel20.FastWaterModel20CompilerHandler.UniqueShaderName(name);
        
        var result = new System.Text.StringBuilder();
        result.Append("//version: " + EModules.FastWaterModel20.FastWaterModel20Controller.VERSION);
        result.AppendLine();
        if (name.Contains('/'))
            name = name.Substring(name.LastIndexOf('/') + 1);
            
        result.Append("Shader \"EM-X/Fast Water Model 2.0 Compiled/" + (name) + "\"");
        result.AppendLine();
        
        #if USE_STRIP_UNIFORMS
        var raw_uniform = GetShaderUniform();
        var uniform =  (raw_uniform.uniform);
        var sampleKeys = raw_uniform.texNames.Select(dk => dk.Substring(dk.IndexOf(' ') + 1).Replace(";", "").Trim()).Distinct().ToDictionary(sk => sk);
        
        var prop_builder =  new System.Text.StringBuilder();
        foreach (var item in propsplit ?? (propsplit = EModules.FastWaterModel20.FastWaterModel20CompilerHandler.properties.Split('\n')))
        {   if (!item.Contains("2D") && !item.Replace(" ", "").Contains(",2D)") && !item.Contains("Cube") && !item.Replace(" ", "").Contains(",Cube)")) { prop_builder.AppendLine(item); continue; }
            var captureItem = item;
            if (sampleKeys.Any(sk => captureItem.Contains(sk.Key))) prop_builder.AppendLine(item);
            
        }
        
        result.Append(prop_builder.ToString());
        result.AppendLine();
        #else
        
        result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.properties);
        result.AppendLine();
        // var uniform =  "#include \"" + CgincPath + "Modules Water Model 2.0 uniform.cginc\"";
        var uniform = ("#include \"" + CgincPath + "EModules Water Model 2.0 uniform.cginc" + "\"");
        // var uniform = GetShaderUniformText();
        result.AppendLine();
        #endif
        // Debug.Log(sampleKeys.First());
        
        
        if (IS_GRABPASS) result.AppendLine("GrabPass{}");
        result.AppendLine("Tags{");
        if (!IS_OPAQUE )result.AppendLine(@"""Queue"" = ""Transparent-5""");
        if (!IS_OPAQUE )result.AppendLine(@"""RenderType"" = ""Transparent""");
        if (!IS_OPAQUE )result.AppendLine(@"""ForceNoShadowCasting"" = ""True""");
        // if (IS_OPAQUE )result.AppendLine(@"""Queue"" = ""Geometry""");
        if (IS_OPAQUE )result.AppendLine(@"""RenderType"" = ""Opaque""");
        //  if (IS_OPAQUE )result.AppendLine(@" ""LightMode"" = ""ForwardBase""");
        
        result.AppendLine(@"""IgnoreProjector"" = ""False""");
        result.AppendLine("}");
        if (!IS_OPAQUE )result.AppendLine("Blend SrcAlpha OneMinusSrcAlpha");
        if (!IS_OPAQUE )result.AppendLine("AlphaTest Off");
        result.AppendLine("Lighting Off");
        //if (IsKeywordEnabled("ALLOW_MANUAL_DEPTH") && IS_OPAQUE)result.AppendLine("ZWrite On");
        //else result.AppendLine("ZWrite Off");
        
        
        
        
        /*  if (IS_OPAQUE && IS_GRABPASS)
              result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.shadertag_GrapPass_Opaque);
          if (!IS_OPAQUE && IS_GRABPASS)
              result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.shadertag_GrapPass_Tranparent);
          if (IS_OPAQUE && !IS_GRABPASS)
              result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.shadertag_NoGrapPass_Opaque);
          if (!IS_OPAQUE && !IS_GRABPASS)
              result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.shadertag_NoGrapPass_Tranparent);*/
        
        
        /*    public static string shadertag_GrapPass_Tranparent = @"
        GrabPass{}
        Tags{ ""Queue"" = ""Transparent-5"" ""RenderType"" = ""Transparent"" ""ForceNoShadowCasting"" = ""True"" ""IgnoreProjector"" = ""False""}
         Blend SrcAlpha OneMinusSrcAlpha
         AlphaTest Off
        Lighting Off
        ";
        public static string shadertag_NoGrapPass_Tranparent = @"
        Tags{ ""Queue"" = ""Transparent-5"" ""RenderType"" = ""Transparent""  ""ForceNoShadowCasting"" = ""True"" ""IgnoreProjector"" = ""False""}
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Off
        Lighting Off
        //ColorMask RGB
        ";
        public static string shadertag_GrapPass_Opaque = @"
        GrabPass{}
        	Tags{ ""Queue"" = ""Geometry"" ""RenderType"" = ""Opaque""    ""IgnoreProjector"" = ""False"" }
        Lighting Off
        ";
           public static string shadertag_NoGrapPass_Opaque = @"
        	 Tags{ ""RenderType"" = ""Opaque""    ""IgnoreProjector"" = ""False"" }
        Lighting Off
        ";*/
        
        
        
        result.AppendLine();
        result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.prekeywords);
        result.AppendLine();
        
        if ( IsKeywordEnabled("ULTRA_FAST_MODE") || IsKeywordEnabled("MINIMUM_MODE"))
        {   result.Append("#pragma target 2.0");
        }
        else
        {   result.Append("#pragma target 3.0");
        }
        result.AppendLine();
        
        if (!NeedOptimize)
        {   Dictionary<string, bool> checkDic = new Dictionary<string, bool>();
            foreach (var item in _keywords)
            {   if (string.IsNullOrEmpty(item.Trim()) || checkDic.ContainsKey(item)) continue;
                result.Append("#define " + item + " 1;\n");
                checkDic.Add(item, false);
            }
        }
        result.AppendLine();
        
        /* string comlipeoption = "#pragma multi_compile_fwdbase ";
         if (!IsKeywordEnabled("USE_LIGHTMAPS"))
             comlipeoption += "nolightmap nodirlightmap nodynlightmap novertexlight";
         if (!IsKeywordEnabled("USE_SHADOWS"))
             comlipeoption += "noshadow";
         result.AppendLine(comlipeoption);*/
        
        result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.postkeywords);
        result.AppendLine();
        if (IsKeywordEnabled("USE_SHADOWS")) result.AppendLine("#pragma multi_compile_fwdbase");
        
        
        
        result.Append(uniform);
        
        result.AppendLine();
        ApplyBody(ref result, raw_uniform);
        result.AppendLine();
        result.Append(EModules.FastWaterModel20.FastWaterModel20CompilerHandler.closeshader);
        
        EModules.FastWaterModel20.FastWaterModel20Controller.need_update = true;
        
        return result.ToString();
    }
    
    [SerializeField]
    public int Optimized = 0;
    
    #if USE_STRIP_UNIFORMS
    [System.NonSerialized]
    public bool __NeedOptimize = false;
    public bool NeedOptimize
    {
        #if OVERRIDE_OPTIMIZER
        get { return true;}
        set { }
        #else
        get { return __NeedOptimize;}
        set { __NeedOptimize = value;}
        #endif
    }
    #else
    public bool NeedOptimize
    {   get { return false;}
        set { }
    }
    #endif
    
    void ApplyBody(ref System.Text.StringBuilder result, shaderCache raw_uniform)
    {   string FILENAME = null;
        switch (WaterType)
        {   case WaterShaderType.Minimum:
                FILENAME = "EModules Water Model Minimum Mode v1.cginc";
                break;
            case WaterShaderType.AdvancePC:
                FILENAME = "EModules Water Model 2.0 v1.cginc";
                break;
            case WaterShaderType.UltraFast:
                FILENAME = "EModules Water Model Ultra Fast Mode v1.cginc";
                break;
            default: throw new System.Exception();
        }
        
        if (!NeedOptimize)
        {   result.Append("#include \"" + CgincPath + FILENAME + "\"");
        }
        else
        {   SH_Body_declaredKeys = raw_uniform.declaredKeys.Keys.Concat(_keywords).Distinct().ToDictionary(k => k);
            SH_BODY_declaredBody = SH_Body_declaredKeys.Where(d => !string.IsNullOrEmpty(d.Value) && d.Value != "1" && d.Value != "= 1" && d.Value != "= 1;").ToDictionary(k => k.Key, v => v.Value);
            result.Append(GetShaderBody(GetShaderText(FILENAME), ref raw_uniform));
        }
    }
    
    
    
    bool EnabledGetShaderBody  (string str)
    {   bool inverce = false;
        if (str[0] == '!')
        {   inverce = true;
            str = str.Substring(1);
        }
        var result = SH_Body_declaredKeys.ContainsKey(str);
        if (inverce) result = !result;
        return result;
    }
    Dictionary<string, string> SH_BODY_declaredBody = new Dictionary<string, string>();
    Dictionary<string, string> SH_Body_declaredKeys = new Dictionary<string, string>();
    static  Dictionary<string, string> _GetShaderTextCache = new Dictionary<string, string>();
    public string GetShaderText( string fileName)
    {   if (!_GetShaderTextCache.ContainsKey(fileName)) _GetShaderTextCache.Add(fileName, System.IO.File.ReadAllText( FastWaterModel20Compiler.CgincPath + fileName) );
        return _GetShaderTextCache[fileName];
    }
    public string GetShaderBody(string __file, ref shaderCache raw_uniform)
    {
    
        int scan  = 0;
        while (scan < __file.Length - 1 && __file.IndexOf("/*", scan) != -1)
        {   scan = __file.IndexOf("/*", scan) + 2;
            var end = __file.IndexOf("*/", scan);
            __file = __file.Remove(scan - 2) + __file.Substring(end + 2);
        }
        
        
        
        var file = __file.Split('\n').Select(s => s.Trim()).ToArray();
        for (int i = 0; i < file.Length; i++)
        {   if (file[i].IndexOf("//") != -1) file[i] = file[i].Remove(file[i].IndexOf("//"));
            //  if (file[i].StartsWith("#ifdef") ) file[i] = "";
        }
        
        
        List<int> deep = new List<int>();
        deep.Add(1);
        int waitNext = 0;
        int lineind = 0;
        List<string> compilerShader = new List<string>();
        
        // var keylist = _keywords.Distinct().ToDictionary(k => k);
        
        
        // foreach (var _line in file)
        for (int f = 0; f < file.Length; f++)
        {
        
            lineind ++;
            var line = file[f].Trim();
            
            if (waitNext != 0)
            {   if (line.StartsWith("#endif")) waitNext--;
                if (line.StartsWith("#if")) waitNext++;
                if (waitNext == 1)
                {   if (line.StartsWith("#elif")) waitNext--;
                    if (line.StartsWith("#else")) waitNext--;
                }
                else
                {
                }
            }
            if (waitNext != 0 )
                continue;
                
            if (line.StartsWith("#include"))
            {   compilerShader.Add(GetShaderBody(GetShaderText(line.Substring(line.IndexOf(' ')).Trim(' ', '\"')), ref raw_uniform));
                continue;
            }
            
            var noIf =  !line.StartsWith("#if") && !line.StartsWith("#elif") && !line.StartsWith("#else") && !line.StartsWith("#endif");
            
            if (deep.Last() == 1)
            {   var temp = line.Trim();
                // if (temp.StartsWith("uniform")) temp = temp.Substring("uniform".Length);
                // temp = temp.Trim();
                // if (temp.StartsWith("sampler2D")  )samplerCount.Add(temp);
                if (temp.StartsWith("#define ")  )
                {   var fp = temp.IndexOf(' ');
                    var key = temp.Substring(fp, temp.IndexOf(' ', fp + 1) - fp).Trim();
                    var sliptI = key.IndexOf(' ');
                    var content = sliptI == -1 ? "" : key.Substring(sliptI).Trim();
                    if (sliptI != -1)
                    {   key = key.Remove(sliptI);
                        while (key.EndsWith("\\"))
                        {   compilerShader.Add(file[++f]);
                            key = key.Trim(' ', '\\') + file[++f].Trim();
                        }
                    }
                    
                    if (!SH_Body_declaredKeys.ContainsKey(key))
                        SH_Body_declaredKeys.Add(key, content);
                    if (!SH_BODY_declaredBody.ContainsKey(key) && content != "1" && content != "= 1" && content != "= 1;")
                        SH_BODY_declaredBody.Add(key, content);
                }
                else
                {   foreach (var item in SH_BODY_declaredBody)
                    {   if (line.Contains(item.Key))
                        {   var split_line = line.Split(new[] {' ' }, System.StringSplitOptions.RemoveEmptyEntries );
                            var has_cahnges = false;
                            for (int i = 0; i < split_line.Length; i++)
                            {   if (split_line[i] == item.Key)
                                {split_line[i] = item.Value; has_cahnges = true;}
                            }
                            if (has_cahnges) line = split_line.Aggregate((a, b) => a + ' ' + b);
                        }
                    }
                    compilerShader.Add(line);
                }
                
                
                /* if (noIf)
                     compilerShader.Add(line);
                 else
                     compilerShader.Add("//" + line);*/
            }
            else
            {   if (line.StartsWith("#") )
                {   if (NeedOptimize)                    compilerShader.Add("//" + line);
                    else                     compilerShader.Add(line);
                }
            }
            if (!line.StartsWith("#")) continue;
            
            if (noIf) continue;
            if (line.StartsWith("#if"))
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                if (line.StartsWith("#ifdef") || line.StartsWith("#ifndef"))
                {   deep.Add(0);
                }
                else
                {   deep.Add(0);
                }
            }
            if (line.StartsWith("#else") )
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                if (deep.Last() == 0) deep[deep.Count - 1] = 1;
                else
                {   deep[deep.Count - 1] = 2;
                    waitNext = 1;
                }
                continue;
            }
            if (line.StartsWith("#endif"))
            {   // Debug.Log("REMOVE " + deep.Last() + " " +  line);
                deep.RemoveAt(deep.Count - 1);
                continue;
            }
            if (deep.Last() == 2)
            {   waitNext = 1;
                continue;
            }
            if (line.StartsWith("#elif") && deep.Last() > 0)
            {   //  Debug.Log("ADD " + deep.Last() + " " +  line);
                deep[deep.Count - 1] = 2;
                continue;
            }
            
            if (line.StartsWith("#ifdef"))
            {   line = "TRUE";
            }
            else
            {   var inndd = line.IndexOf(' ');
                var innff = line.IndexOf('(');
                if (innff != -1 && innff < inndd)inndd = innff;
                if (inndd == -1 ) continue;
                
                line = line.Substring(inndd);
                line = line.Replace(" ", "").Replace("&&", "-&&-").Replace("||", "-||-");
                while (line.IndexOf("defined(") != -1)
                {   var d = line.IndexOf("defined(");
                    var ca = line.ToCharArray();
                    ca[line.IndexOf('(', d)] = '[';
                    ca[line.IndexOf(')', d)] = ']';
                    line = new string(ca);
                }
                
                
                
                var lb = -1;
                do
                {   lb = line.LastIndexOf('(') ;
                    var rb = lb == -1 ? -1 : line.IndexOf(')', lb);
                    //Debug.Log(line);
                    
                    var _expression = lb == -1 ? line : line.Substring(lb + 1, (rb - lb) - 1);
                    var expression =         _expression.Split(new[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries)
                                             .Select(s => s.Replace("defined[", "").Replace("]", ""))
                                             .ToList();
                                             
                    if (expression.Count == 1)
                    {   expression[0] = expression[0] == "TRUE" ? "TRUE" : EnabledGetShaderBody(expression[0]) ? "TRUE" : "FALSE" ;
                    }
                    else
                    {   while (expression.IndexOf("&&") != -1)
                        {   var ind = expression.IndexOf("&&");
                            var left = expression[ind - 1] == "TRUE" || EnabledGetShaderBody(expression[ind - 1]) ;
                            var right = expression[ind + 1] == "TRUE" || EnabledGetShaderBody(expression[ind + 1]);
                            expression.Insert(ind - 1, left && right ? "TRUE" : "FALSE" );
                            expression.RemoveAt(ind);
                            expression.RemoveAt(ind);
                            expression.RemoveAt(ind);
                        }
                        while (expression.IndexOf("||") != -1)
                        {   var ind = expression.IndexOf("||");
                        
                            var left = expression[ind - 1] == "TRUE" || EnabledGetShaderBody(expression[ind - 1]) ;
                            var right = expression[ind + 1] == "TRUE" || EnabledGetShaderBody(expression[ind + 1]);
                            expression.Insert(ind - 1, left || right ? "TRUE" : "FALSE" );
                            expression.RemoveAt(ind);
                            expression.RemoveAt(ind);
                            expression.RemoveAt(ind);
                        }
                    }
                    
                    if (expression[0] != "TRUE" && expression[0] != "FALSE")
                    {   break;
                    }
                    var result = lb == -1 ? "" : line.Remove(lb);
                    result += expression[0];
                    result += lb != -1 && rb + 1 < line.Length ? line.Substring(rb + 1) : "";
                    
                    line = result;
                } while (lb != -1);
            }
            
            
            
            if ( line != "TRUE" && line != "FALSE")
            {   break;
            }
            if (line == "FALSE") waitNext = 1;
            else deep[deep.Count - 1] = 1;
            //var expression = line.Substring(line.IndexOf(' ')).Split(new[]{' ' } ,System.StringSplitOptions.RemoveEmptyEntries  ) .Select(s=>s.Trim());
        }
        
        return compilerShader.Aggregate((a, b) => a + "\n" + b);
        
        
    }
    
    
    
    
    
    #endif
    
    public bool IS_OPAQUE { get { return IsKeywordEnabled("USE_SHADOWS") || IsKeywordEnabled("FORCE_OPAQUE"); } }
    public bool IS_GRABPASS { get { return IsKeywordEnabled("REFRACTION_GRABPASS"); } }
    
    
    public Color GetColor(int key) { return material.GetColor(key); }
    public Color GetColor(string key) { return material.GetColor(key); }
    public void SetColor(string key, Color c) { material.SetColor(key, c); }
    public void SetColor(int key, Color c) { material.SetColor(key, c); }
    public Vector4 GetVector(int key) { return material.GetVector(key); }
    public void SetVector(int key, Vector4 v) { material.SetVector(key, v); }
    public void SetVector(string key, Vector4 v) { material.SetVector(key, v); }
    public Vector4 GetVector(string key) { return material.GetVector(key); }
    public bool HasProperty(int key)
    {
        #if UNITY_EDITOR
        if (!material)
            return false;
        #endif
        return material.HasProperty(key);
    }
    public bool HasProperty(string key)
    {
        #if UNITY_EDITOR
        if (!material)
            return false;
        #endif
        return material.HasProperty(key);
    }
    public void SetTexture(string key, Texture t) { material.SetTexture(key, t); }
    public Texture GetTexture(string key) { return material.GetTexture(key); }
    public void SetTexture(int key, Texture t) { material.SetTexture(key, t); }
    public Texture GetTexture(int key) { return material.GetTexture(key); }
    public void SetFloat(string key, float t) { material.SetFloat(key, t); }
    public float GetFloat(string key) { return material.GetFloat(key); }
    public void SetFloat(int key, float t) { material.SetFloat(key, t); }
    public float GetFloat(int key) { return material.GetFloat(key); }
}




namespace EModules.FastWaterModel20 {
#if UNITY_EDITOR
public partial class FastWaterModel20Controller : MonoBehaviour {
    void Compile()
    {   var m = compiler;
        if (!m)
            return;
        if (!m.shader)
            return;
        var path = AssetDatabase.GetAssetPath(m);
        if (string.IsNullOrEmpty(path))
            return;
        var mainasset = AssetDatabase.LoadMainAssetAtPath(path) as FastWaterModel20Compiler;
        if (!mainasset)
            mainasset = CreateAsset(RENDERER.sharedMaterial);
            
        //UpdateShaderAsset(mainasset.shader, mainasset.CreateShaderString());
        mainasset.UpdateShaderAsset();
        EditorUtility.SetDirty(mainasset);
    }
    
    public FastWaterModel20Compiler CreateAsset(Material m)
    {   string newName;
        string path;
        newName = EModules.FastWaterModel20.FastWaterModel20CompilerHandler.UniqueShaderName(m.name);
        path = AssetDatabase.GetAssetPath(m);
        path = path.Remove(path.LastIndexOf('/')) + "/" + newName + ".asset";
        
        var result = CreateAsset(path, m);
        
        var oldShader = m.shader;
        if (oldShader)
        {   //var crop = oldShader.ToString();
            var crop = System.IO.File.ReadAllText(AssetDatabase.GetAssetPath(oldShader));
            var featuresIndex = crop.IndexOf("#pragma shader_feature");
            
            if (featuresIndex != -1)
            {   crop = crop.Substring(featuresIndex);
                crop = crop.Remove(crop.IndexOf('}'));
                foreach (var item in crop.Split(new[] { "\n", System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries))
                {   var trim = item.Trim();
                    trim = trim.Trim('\t');
                    if (trim.StartsWith("#pragma shader_feature"))
                    {   trim = trim.Substring("#pragma shader_feature".Length);
                        if (trim.IndexOf('/') != -1)
                            trim = trim.Remove(trim.IndexOf('/'));
                            
                        foreach (var keyword in trim.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries))
                        {   var keywordtrim = keyword.Trim(' ');
                            keywordtrim = keywordtrim.Trim('\t');
                            // Debug.Log(keywordtrim);
                            // Debug.Log(m.IsKeywordEnabled(keywordtrim));
                            if (result.material.IsKeywordEnabled(keywordtrim))
                            {   result.EnableKeyword(keywordtrim);
                                result.material.DisableKeyword(keyword);
                            }
                        }
                    }
                }
                // UpdateShaderAsset(result.shader, result.CreateShaderString());
                result.UpdateShaderAsset();
                EditorUtility.SetDirty(result);
                EditorUtility.SetDirty(result.material);
            }
            else
            {   Debug.LogWarning(NAME + " compiler: shader hasn't shader_feature's line");
            }
        }
        
        return result;
    }
    
    public FastWaterModel20Compiler CreateAsset()
    {   string newName;
        string path;
        newName = "Fast Water Model 2.0 User Material";
        path = "Assets/" + newName + ".asset";
        return CreateAsset(path);
    }
    
    public FastWaterModel20Compiler CreateAsset(FastWaterModel20Compiler oldCompiler, string path)
    {   //string path = AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(oldCompiler));
        // string newName = path.Remove(path.LastIndexOf('.'));
        // newName = newName.Substring(newName.LastIndexOf('/') + 1);
        
        var result = CreateAsset(path, oldCompiler.material);
        
        result._keywords = oldCompiler._keywords.ToArray();
        //UpdateShaderAsset(result.shader, result.CreateShaderString());
        result.UpdateShaderAsset();
        EditorUtility.SetDirty(result);
        
        return result;
        
    }
    
    public static Shader CreateShaderAsset(string shader, string path)
    {   var result = ShaderUtil.CreateShaderAsset(shader);
        AssetDatabase.AddObjectToAsset(result, path);
        return result;
        /*   path = path.Remove(path.LastIndexOf('.')) + ".shader";
         System.IO.File.WriteAllText(path, shader);
         AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
         var result = AssetDatabase.LoadAssetAtPath<Shader>(path);
         return result;*/
        // var s = ShaderUtil.CreateShaderAsset(newSvhader);
        // s.;
        // new Shader();
        // AssetDatabase.Load
        // ShaderUtil.RegisterShader()
        /*#pragma warning disable
                    var m = new Material(shader);
                    var result = m.shader;
                    DestroyImmediate(m);
                    return result;
        #pragma warning restore*/
    }
    
    
    public FastWaterModel20Compiler CreateAsset(string path, Material oldM = null)
    {   path = AssetDatabase.GenerateUniqueAssetPath(path);
    
        var mainasset = ScriptableObject.CreateInstance<FastWaterModel20Compiler>();
        // var mainasset = new FastWaterModel20Compiler();
        var newName = System.IO.Path.GetFileNameWithoutExtension(path);
        mainasset.name = newName;
        
        AssetDatabase.CreateAsset(mainasset, path);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        
        var newShader = mainasset.CreateShaderString(newName);
        #if COMPILE_TO_BUFFER
        // EditorGUIUtility.systemCopyBuffer = newShader;
        #endif
        
        var s = CreateShaderAsset(newShader, path);
        s.name = newName;
        //
        // Debug.Log(path);
        // AssetDatabase.CreateAsset(s, path.Remove(path.LastIndexOf('.')) + ".shader");
        //
        
        var m = oldM ? new Material(oldM) : new Material(s);
        m.name = newName;
        AssetDatabase.AddObjectToAsset(m, path);
        
        
        mainasset.material = m;
        mainasset.shader = s;
        m.shader = s;
        compiler = mainasset;
        
        AssetDatabase.SaveAssets();
        
        EditorUtility.SetDirty(s);
        EditorUtility.SetDirty(m);
        EditorUtility.SetDirty(mainasset);
        
        return mainasset;
    }
    
}
#endif



#if UNITY_EDITOR

[CustomEditor(typeof(FastWaterModel20Compiler))]
public class FastWaterModel20CompilerEditor : Editor {
    public override bool HasPreviewGUI()
    {   return false;
    }
    // Material matEditorold;
    Shader matEditorold;
    Editor matEditor;
    public override void DrawPreview(Rect previewArea)
    {   var complier = target as FastWaterModel20Compiler;
        if (!complier.material)
            return;
            
        if (matEditor == null || matEditorold != complier.material)
        {   matEditor = Editor.CreateEditor(complier.material);
            matEditorold = complier.shader;
            // matEditorold = complier.material;
        }
        matEditor.DrawPreview(previewArea);
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {   DrawPreview(r);
    }
    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {   DrawPreview(r);
    }
    string editorName;
    
    public override void OnInspectorGUI()
    {   var complier = target as FastWaterModel20Compiler;
    
        if (string.IsNullOrEmpty(editorName))
            editorName = target.name;
        editorName = EditorGUILayout.TextField("Name:", editorName);
        
        if (GUILayout.Button("Rename Material & Shader"))
        {   var path = AssetDatabase.GetAssetPath(complier);
            AssetDatabase.RenameAsset(path, editorName);
            complier.name = editorName;
            
            
            complier.material.name = editorName;
            complier.shader.name = EModules.FastWaterModel20.FastWaterModel20CompilerHandler.UniqueShaderName(editorName);
            // Debug.Log(complier.shader.name);
            //f var newShaderString = complier.CreateShaderString(editorName);
            // FastWaterModel20Controller.UpdateShaderAsset(complier.shader, newShaderString);
            complier.UpdateShaderAsset(editorName);
            
            EditorUtility.SetDirty(complier);
        }
        base.OnInspectorGUI();
    }
}


public partial class FastWaterModel20CompilerHandler {
    public static string UniqueShaderName(string name)
    {   var shaders = ShaderUtil.GetAllShaderInfo().ToDictionary(s => s.name);
        int interator = -1;
        while (shaders.ContainsKey(name + (interator == -1 ? "" : (" " + interator.ToString()))))
            interator++;
        return name + (interator == -1 ? "" : (" " + interator.ToString()));
    }
    
    public static string properties = @"
{
	Properties{

		_FrameRate(""_FrameRate"", Float) = 0

	//_LastFrame2(""_LastFrame2"",  2D) = ""white"" {}
	//_LastFrame(""_LastFrame"",  2D) = ""white"" {}
		_FracTimeFull(""_FracTimeFull"", Float) = 0
		_Frac2PITime(""_Frac2PITime"", Float) = 0
		_Frac01Time(""_Frac01Time"", Float) = 0
		_Frac01Time_d8_mBlendAnimSpeed(""_Frac01Time_d8_mBlendAnimSpeed"", Float) = 0
		_Frac01Time_MAIN_TEX_BA_Speed(""_Frac01Time_MAIN_TEX_BA_Speed"", Float) = 0
		_FracWTime_m4_m3DWavesSpeed_dPI2(""_FracWTime_m4_m3DWavesSpeed_dPI2"", Vector) = (0,0,0,0)
		_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1(""_Frac01Time_m16_mMAIN_TEX_FoamGradWavesSpeed_1"", Float) = 0
		_Frac_UFSHORE_Tile_1Time_d10(""_Frac_UFSHORE_Tile_1Time_d10"", Vector) = (0,0,0,0)
		_Frac_UFSHORE_Tile_2Time_d10(""_Frac_UFSHORE_Tile_2Time_d10"", Vector) = (0,0,0,0)
		_Frac_UFSHORE_Tile_2Time_d10(""_Frac_UFSHORE_Tile_2Time_d10"", Vector) = (0,0,0,0)
	_Frac_WaterTextureTilingTime_m_AnimMove(""_Frac_WaterTextureTilingTime_m_AnimMove"", Vector) = (0,0,0,0)
	_Frac_UVS_DIR(""_Frac_UVS_DIR"", Vector) = (0,0,0,0)
	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move"", Vector) = (0,0,0,0)
	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_pMAIN_TEX_CA_Speed"", Vector) = (0,0,0,0)
	_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed(""_FracMAIN_TEX_TileTime_mMAIN_TEX_Move_sMAIN_TEX_CA_Speed"", Vector) = (0,0,0,0)

	_MM_Tile(""_MM_Tile"", Vector) = (1,1,1,1)
	_MM_offset(""_MM_offset"", Vector) = (0,0,0,0)
	_MM_Color(""_MM_Color"", COLOR) = (1,1,1,1)
	_ReflectionJustColor(""_ReflectionJustColor"", COLOR) = (1,1,1,1)

		MAIN_TEX_Bright(""MAIN_TEX_Bright"", Float) = 0

		lerped_post_offset(""lerped_post_offset"", Float) = 0
		lerped_post_offset_falloff(""lerped_post_offset_falloff"", Float) = 1
	lerped_post_color1(""lerped_post_color1"", COLOR) = (1,1,1,1)
	lerped_post_color2(""lerped_post_color2"", COLOR) = (1,1,1,1)

		[Header(Texture)]//////////////////////////////////////////////////
	[NoScaleOffset] _MainTex(""Additional Color Texture(RGB) Mask for reflection(A)"", 2D) = ""black"" {}
		_MainTexAngle(""_MainTexAngle"", Float) = 0 //Range(0.1,10)
	_MainTexTile(""_MainTexTile"", Vector) = (1,1,0,0)
	_MainTexColor(""Tint Color (RGB) Amount Texture (A)"", COLOR) = (1,1,1,1)
		_OutGradBlend(""_OutGradBlend"", Float) = 1 //Range(0.1,10)
		_OutShadowsAmount(""_OutShadowsAmount"", Float) = 20 //Range(0.1,10)
		_OutGradZ(""_OutGradZ"", Float) = 1 //Range(0.1,10)
		_UFSHORE_UNWRAP_Transparency(""_UFSHORE_UNWRAP_Transparency"", Float) = 0 //Range(0.1,10)
		
		_AnimMove(""_AnimMove"", Vector) = (0,0,0,0)


		_VERTEX_ANIM_DETILESPEED(""_VERTEX_ANIM_DETILESPEED"", Float) = 0
		_VERTEX_ANIM_DETILEAMOUNT(""_VERTEX_ANIM_DETILEAMOUNT"", Float) = 1


		_Z_BLACK_OFFSET_V(""_Z_BLACK_OFFSET_V"", Float) = 0

		MAINTEX_VHEIGHT_Amount(""MAINTEX_VHEIGHT_Amount"", Float) = 1
		MAINTEX_VHEIGHT_Offset(""MAINTEX_VHEIGHT_Offset"", Float) = 0.8

		_FoamBlendOffset(""_FoamBlendOffset"", Float) = 0

	[Header(Commons)]//////////////////////////////////////////////////
        _TransparencyPow(""_TransparencyPow"", Float) = 1 //Range(0,2)
		_TransparencyLuminosity(""_TransparencyLuminosity"", Float) = 1 //Range(0,2)
		_TransparencySimple(""_TransparencySimple"", Float) = 1 //Range(0,2)


	_WaterTextureTiling(""_Tiling [x,y]; Speed[z];"", Vector) = (1,1,1,-1)
	_BumpAmount(""_BumpAmount"", Float) = 1 //Range(0,1)
	[NoScaleOffset] _BumpMap(""_BumpMap "", 2D) = ""bump"" {}
	[NoScaleOffset] _MM_Texture(""_MM_Texture "", 2D) = ""bump"" {}
	_MM_MultyOffset(""_MM_MultyOffset"", Float) = 0.3

	[Header(Specular)]//////////////////////////////////////////////////
        _LightAmount(""_LightAmount"", Float ) = 1//Range(0,2)
	_SpecularAmount(""_SpecularAmount"", Float) = 2 //Range(0,10)
	_SpecularShininess(""_SpecularShininess"", Float) = 48 //Range(0,512)
	_FlatSpecularAmount(""_FlatSpecularAmount"", Float ) = 1.0//Range(0,10)
	_FlatSpecularShininess(""_FlatSpecularShininess"", Float ) = 48//Range(0,512)
	_FlatSpecularClamp(""_FlatSpecularClamp"", Float  ) = 10//Range(0,100)
			_FlatFriqX(""_FlatFriqX"", Float) = 1//Range(0,100)
			_FlatFriqY(""_FlatFriqY"", Float) = 1//Range(0,100)

		_LightColor0Fake(""_LightColor0Fake"", COLOR) = (1,1,1,1)
			_BlendColor(""_BlendColor"", COLOR) = (1,1,1,1)

	[Header(Foam)]//////////////////////////////////////////////////
        _FoamAmount(""_FoamAmount"", Float ) = 5 //Range(0,10)
	_FoamLength(""_FoamLength"", Float ) = 10 //Range(0,20)
	_FoamColor(""_FoamColor"", COLOR) = (1,1,1,1)
		_FoamDistortion(""_FoamDistortion"", Float ) = 1 //Range(0,10)
		_FoamDistortionFade(""_FoamDistortionFade"", Float) = 0.8 //Range(0,1)
		_FoamDirection(""_FoamDirection"", Float) = 0.05 //Range(0,1)
		_FixMulty(""_FixMulty"", Float) = 1 //Range(0,1)
		_FoamAlpha2Amount(""_FoamAlpha2Amount"", Float ) = 0.5 //Range(0,1)

		_BlendAnimSpeed(""_BlendAnimSpeed"", Float) = 1 //Range(0,1)
		
	_WaterfrontFade(""_WaterfrontFade"", Float ) = 10//Range(0,30) 
	_ShadorAmount(""_ShadorAmount"", Float ) = 0.5
	VERTEX_H_DISTORT(""VERTEX_H_DISTORT"", Float ) = 0



	_VERT_Amount(""_VERT_Amount"", Float ) = 1
	_VERT_Tile(""_VERT_Tile"", Vector ) = (1,1,1,1)
	_VERT_Color(""_VERT_Color"", COLOR) = (1,1,1,1)

	_RIM_BLEND(""_RIM_BLEND"", Float ) = 0




	_FoamTextureTiling(""_FoamTextureTiling"", Float ) = 1 //Range(0.01,2)
	_FoamWavesSpeed(""_FoamWavesSpeed"", Float ) = 0.15//Range(0,2 )
	_FoamOffsetSpeed(""_FoamOffsetSpeed"", Float) = 0.2//Range(0,4 ) 
		_FoamOffset(""_FoamOffset"", Float) = 0//Range(0,4 ) 
		[NoScaleOffset] _FoamTexture(""_FoamTexture"",  2D) = ""white"" {}


		_MultyOctavesSpeedOffset(""_MultyOctavesSpeedOffset"", Float) = 0.89
		_MultyOctavesTileOffset(""_MultyOctavesTileOffset"", Vector) = (1.63,1.63,1,1)
		_FadingFactor(""_FadingFactor"", Float) = 0.5
		FLAT_HQ_OFFSET(""FLAT_HQ_OFFSET"", Float) = 50


	[Header(3D Waves)]//////////////////////////////////////////////////
        _3DWavesTile(""_3DWavesTile"", Vector) = (1,1,-1,-1)
	//_3DWavesSpeed(""_3DWavesSpeed"",  Vector) = (0.1 ,0.1, 0.1, 0.1)
	_3DWavesSpeed(""_3DWavesSpeed"",  Float) = 0.1
	_3DWavesSpeedY(""_3DWavesSpeedY"",  Float) = 0.1
	_3DWavesHeight(""_3DWavesHeight"", Float  ) = 1//Range(0.1,32)
	_3DWavesWind(""_3DWavesWind"", Float  ) = 0.1//Range(0.1,32)
	_3DWavesYFoamAmount(""_3DWavesYFoamAmount"", Float  ) = 0.02//Range(0,10)
	_3DWavesTileZ(""_3DWavesTileZ"", Float ) = 1 //Range(0.1,32)
	_3DWavesTileZAm(""_3DWavesTileZAm"", Float ) = 1 //Range(0.1,32)
	ADDITIONAL_FOAM_DISTORTION_AMOUNT(""ADDITIONAL_FOAM_DISTORTION_AMOUNT"", Float ) = 1 //Range(0.1,32)

	_SURFACE_FOG_Amount(""_SURFACE_FOG_Amount"", Float ) = 1 //Range(0.1,32)
	_SURFACE_FOG_Speed(""_SURFACE_FOG_Speed"", Float ) = 1 //Range(0.1,32)
	_SURFACE_FOG_Tiling(""_SURFACE_FOG_Tiling"", Float ) = 1 //Range(0.1,32)

	_Light_FlatSpecTopDir(""_Light_FlatSpecTopDir"",  Float ) = 0.5
	MAIN_TEX_Multy(""MAIN_TEX_Multy"",  Float ) = 1


	//_LastFrame(""_LastFrame"",  2D) = ""white"" {}


	_RefrLowDist(""_RefrLowDist"",  Float ) = 0.1
	_VertexToUv(""_VertexToUv"",  Float ) = 1


_WavesDirAngle(""_WavesDirAngle"", Float ) = 0
	_VertexSize(""_VertexSize"", Vector ) = (0,0,1,1)

	_ZDistanceCalcOffset(""_ZDistanceCalcOffset"", Float ) = 0.25

	[Header(NOISES)]//////////////////////////////////////////////////
        [Header(HQ)]//////////////////////////////////////////////////
        _NHQ_GlareAmount(""_NHQ_GlareAmount"", Float  ) = 1//Range(0.1,50)
	_NHQ_GlareFriq(""_NHQ_GlareFriq"", Float ) = 30 //Range(0.1,100)
	_NHQ_GlareSpeedXY(""_NHQ_GlareSpeedXY"", Float  ) = 1//Range(0,3)
	_NHQ_GlareSpeedZ(""_NHQ_GlareSpeedZ"", Float ) = 1 //Range(0,5)
	_NHQ_GlareContrast(""_NHQ_GlareContrast"", Float  ) = 0//Range(0,2.5)
	_NHQ_GlareBlackOffset(""_NHQ_GlareBlackOffset"", Float ) = 0 //Range(-1,0)
	_NHQ_GlareNormalsEffect(""_NHQ_GlareNormalsEffect"", Float ) = 1//Range(0,2) 
	[Header(LQ)]//////////////////////////////////////////////////
        _NE1_GlareAmount(""_NE1_GlareAmount"", Float ) = 1//Range(0.1,50 )
	_NE1_GlareFriq(""_NE1_GlareFriq"", Float  ) = 1//Range(0.1,4)
	_NE1_GlareSpeed(""_NE1_GlareSpeed"", Float  ) = 1//Range(0.1,3)
	_NE1_GlareContrast(""_NE1_GlareContrast"", Float  ) = 0//Range(0,2.5)
	_NE1_GlareBlackOffset(""_NE1_GlareBlackOffset"", Float) = 0 //Range(-1,0) 
	_NE1_WavesDirection(""_NE1_WavesDirection"", Vector) = (0.5,0,0.25)
	[Header(WAWES1)]//////////////////////////////////////////////////
        _W1_GlareAmount(""_W1_GlareAmount"", Float  ) = 1//Range(0.1,50)
	_W1_GlareFriq(""_W1_GlareFriq"", Float  ) = 1//Range(0.1,10)
	_W1_GlareSpeed(""_W1_GlareSpeed"", Float  ) = 1//Range(0.1,10)
	_W1_GlareContrast(""_W1_GlareContrast"", Float  ) = 0//Range(0,2.5)
	_W1_GlareBlackOffset(""_W1_GlareBlackOffset"", Float  ) = 0//Range(-1,0)

	[Header(WAWES2)]//////////////////////////////////////////////////
        _W2_GlareAmount(""_W2_GlareAmount"", Float  ) = 1//Range(0.1,50)
	_W2_GlareFriq(""_W2_GlareFriq"", Float  ) = 1//Range(0.1,10)
	_W2_GlareSpeed(""_W2_GlareSpeed"", Float ) = 1//Range(0.1,10) 
	_W2_GlareContrast(""_W2_GlareContrast"", Float  ) = 0//Range(0,2.5)
	_W2_GlareBlackOffset(""_W2_GlareBlackOffset"", Float ) = 0//Range(-1,0) 
	MAIN_TEX_LQDistortion(""MAIN_TEX_LQDistortion"", Float ) = 0.1
	MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount(""MAIN_TEX_ADDDISTORTION_THAN_MOVE_Amount"", Float ) = 100


	[Header(PRC HQ)]//////////////////////////////////////////////////
        _PRCHQ_amount(""_PRCHQ_amount"", Float  ) = 1//Range(0.1,50)
	_PRCHQ_tileTex(""_PRCHQ_tileTex"", Float ) = 1//Range(0.1,10 )
	_PRCHQ_tileWaves(""_PRCHQ_tileWaves"", Float  ) = 1//Range(0.1,10)
	_PRCHQ_speedTex(""_PRCHQ_speedTex"", Float  ) = 1//Range(0.1,10)
	_PRCHQ_speedWaves(""_PRCHQ_speedWaves"", Float ) = 1 //Range(0.1,10)
	MAIN_TEX_Amount(""MAIN_TEX_Amount"", Float ) = 3 //Range(0.1,10)
	MAIN_TEX_Contrast(""MAIN_TEX_Contrast"", Float ) = 2 //Range(0.1,10)
	MAIN_TEX_BA_Speed(""MAIN_TEX_BA_Speed"", Float ) = 8 //Range(0.1,10)
	MAIN_TEX_CA_Speed(""MAIN_TEX_CA_Speed"", Vector ) = (0.1,0.1,0.1,0.1)
	MAIN_TEX_Tile(""MAIN_TEX_Tile"", Vector ) = (1,1,1,1)
	MAIN_TEX_Move(""MAIN_TEX_Move"", Vector ) = (0,0,0,0)

	SIMPLE_VHEIGHT_FADING_AFFECT(""SIMPLE_VHEIGHT_FADING_AFFECT"", Float ) = 0.3
	MAIN_TEX_Distortion(""MAIN_TEX_Distortion"", Float ) = 2
	_ReflectionBlendAmount(""_ReflectionBlendAmount"", Float ) = 2
		

	[Header(fresnelFac)]//////////////////////////////////////////////////
        [NoScaleOffset] _Utility(""_Utility"", 2D) = ""white"" {}
	[NoScaleOffset] _NoiseHQ(""_NoiseHQ"", 2D) = ""white"" {}
	[NoScaleOffset] _NoiseLQ(""_NoiseLQ"", 2D) = ""white"" {}
	_FresnelPow(""_FresnelPow"", Float  ) = 1.58//Range(0.5,32)
		_FresnelFade(""_FresnelFade"", Float) = 0.7//Range(0.5,32)
		_FresnelAmount(""_FresnelAmount"", Float) = 4   //Range(0.5,32)
		_RefrDistortionZ(""_RefrDistortionZ"", Float) = 0   //Range(0.5,32)
		FRES_BLUR_OFF(""FRES_BLUR_OFF"", Float) = 0.3   //Range(0.5,32)
		FRES_BLUR_AMOUNT(""FRES_BLUR_AMOUNT"", Float) = 3   //Range(0.5,32)
		_BumpMixAmount(""_BumpMixAmount"", Float) = 0.5   //Range(0.5,32)
		_FastFresnelPow(""_FastFresnelPow"", Float) = 10   //Range(0.5,32)
		_VERTEX_ANIM_DETILE_YOFFSET(""_VERTEX_ANIM_DETILE_YOFFSET"", Vector) = (-10,10,0,0 )  //Range(0.5,32)


	[Header(Reflection)]//////////////////////////////////////////////////
        _ReflectionAmount(""_ReflectionAmount"", Float  ) = 1//Range(0,2)
		_ReflectColor(""_ReflectColor"", COLOR) = (1,1,1,1)
		baked_ReflectionTex_distortion(""baked_ReflectionTex_distortion"", Float  ) = 15//Range(0,50)
		LOW_ReflectionTex_distortion(""LOW_ReflectionTex_distortion"", Float  ) = 0.1//Range(0,50)


		_ReflectionYOffset(""_ReflectionYOffset"", Float  ) = -0.15//Range(-0.4,0)
		_ReflectionLOD(""_ReflectionLOD"", Float  ) = 1//Range(0,4)
		_ReflectionUserCUBE(""_ReflectionUserCUBE"", Cube) = """" {}

		[Header(ReflRefrMask)]//////////////////////////////////////////////////
        _ReflectionMask_Amount(""_ReflectionMask_Amount"", Float  ) = 3//Range(0, 5)
			_ReflectionMask_Offset(""_ReflectionMask_Offset"", Float) = 0.66//Range(0, 0.5)
			_ReflectionMask_UpClamp(""_ReflectionMask_UpClamp"", Float) = 1 //Range(0.5, 10)
		_ReflectionMask_Tiling(""_ReflectionMask_Tiling"", Float  ) = 0.1//Range(0, 2)
			_ReflectionBlurRadius(""_ReflectionBlurRadius"", Float) = 0.1//Range(0, 2)
			_ReflectionBlurZOffset(""_ReflectionBlurZOffset"", Float) = 0//Range(0, 2)
			_RefractionBlurZOffset(""_RefractionBlurZOffset"", Float) = 0//Range(0, 2)
			_ReflectionMask_TexOffsetF(""_ReflectionMask_TexOffsetF"", Vector) =(0,0,0,0)

			_ObjectScale(""_ObjectScale"", Vector) =(1,1,1,1)

			_AverageOffset(""_AverageOffset"", Float) = 0.5//Range(0, 2)
			_REFR_MASK_Tile(""_REFR_MASK_Tile"", Float) = 0.1//Range(0, 2)
			_REFR_MASK_Amount(""_REFR_MASK_Amount"", Float) = 3//Range(0, 5)
			_REFR_MASK_min(""_REFR_MASK_min"", Float) = 0.66//Range(0, 0.5)
			_REFR_MASK_max(""_REFR_MASK_max"", Float) = 1 //Range(0.5, 10)




        _UF_NMASK_Texture(""_UF_NMASK_Texture"", 2D) = ""white"" {}
        _UF_NMASK_Tile(""_UF_NMASK_Tile"", Float) = 0.1
			_UF_NMASK_offset(""_UF_NMASK_offset"", Vector) = (0,0,0,0)
			_UF_NMASK_Contrast(""_UF_NMASK_Contrast"", Float) = 1
			_UF_NMASK_Brightnes(""_UF_NMASK_Brightnes"", Float) = 0



        _AMOUNTMASK_Tile(""_AMOUNTMASK_Tile"", Float) = 0.1//Range(0, 2)
			_AMOUNTMASK_Amount(""_AMOUNTMASK_Amount"", Float) = 3//Range(0, 5)
			_AMOUNTMASK_min(""_AMOUNTMASK_min"", Float) = 0.66//Range(0, 0.5)
			_AMOUNTMASK_max(""_AMOUNTMASK_max"", Float) = 1 //Range(0.5, 10)

			_TILINGMASK_Tile(""_TILINGMASK_Tile"", Float) = 0.1//Range(0, 2)
			_TILINGMASK_Amount(""_TILINGMASK_Amount"", Float) = 3//Range(0, 5)
			_TILINGMASK_min(""_TILINGMASK_min"", Float) = 0.66//Range(0, 0.5)
			_TILINGMASK_max(""_TILINGMASK_max"", Float) = 1 //Range(0.5, 10)

			_MAINTEXMASK_Tile(""_MAINTEXMASK_Tile"", Float) = 0.1//Range(0, 2)
			_MAINTEXMASK_Amount(""_MAINTEXMASK_Amount"", Float) = 3//Range(0, 5)
			_MAINTEXMASK_min(""_MAINTEXMASK_min"", Float) = 0.66//Range(0, 0.5)
			_MAINTEXMASK_max(""_MAINTEXMASK_max"", Float) = 1 //Range(0.5, 10)
			_MAINTEXMASK_Blend(""_MAINTEXMASK_Blend"", Float) = 0.5 //Range(0.5, 10)

			_3Dwaves_BORDER_FACE(""_3Dwaves_BORDER_FACE"", Float) = 0
			_FixHLClamp(""_FixHLClamp"", Float) = 0.8
			_FastFresnelAmount(""_FastFresnelAmount"", Float) = 10
			_RefrDeepAmount(""_RefrDeepAmount"", Float) = 1
			_RefrTopAmount(""_RefrTopAmount"", Float) = 1
			_TexRefrDistortFix(""_TexRefrDistortFix"", Float) = 0
			_SpecularGlowAmount(""_SpecularGlowAmount"", Float) = 0.2
			_RefractionBLendAmount(""_RefractionBLendAmount"", Float) = 0.5

			_TILINGMASK_factor(""_TILINGMASK_factor"", Float) = 5 //Range(0.5, 10)
			_AMOUNTMASK_offset(""_AMOUNTMASK_offset"", Vector) = (0,0,0,0)
			_TILINGMASK_offset(""_TILINGMASK_offset"", Vector) = (0,0,0,0)
			_MAINTEXMASK_offset(""_MAINTEXMASK_offset"", Vector) = (0,0,0,0)
			_ReflectionMask_offset(""_ReflectionMask_offset"", Vector) = (0,0,0,0)
			_REFR_MASK_offset(""_REFR_MASK_offset"", Vector) = (0,0,0,0)
			_ReplaceColor(""_ReplaceColor"", COLOR) = (0.5, 0.5, 0.5, 1)


		[Header(Refraction)]//////////////////////////////////////////////////

        _RefrDistortion(""_RefrDistortion"", Float  ) = 100//Range(0, 600)
		_RefrAmount(""_RefrAmount"", Float  ) = 1//Range(0, 4)
		_RefrZColor(""_RefrZColor"", COLOR) = (0.29, 0.53, 0.608, 1)
			_RefrTopZColor(""_RefrTopZColor"", COLOR) = (0.89, 0.95, 1, 1)
			_RefrTextureFog(""_RefrTextureFog"", Float) = 0 //Range(0, 1)

			_RefrZOffset(""_RefrZOffset"", Float  ) = 1.8//Range(0, 2)
		_RefrZFallOff(""_RefrZFallOff"", Float ) = 10 //Range(1, 20)
		_RefrBlur(""_RefrBlur"", Float) = 0.25 //Range(0, 1)
		_RefrRecover(""_RefrRecover"", Float ) = 0.0 //Range(0, 1)
			_RefrDeepFactor(""_RefrDeepFactor"", Float) = 64 //Range(0, 1)
			_3dwanamnt(""_3dwanamnt"", Float) = 1 //Range(0, 1)



		[Header(Other)]//////////////////////////////////////////////////
        _LightDir(""_LightDir"", Vector) = (-0.5,0.46,0.88,0)
		[NoScaleOffset] _GradTexture(""_GradTexture "", 2D) = ""black"" {}
		

		_ObjecAngle(""_ObjecAngle"", Float) = 0 //Range(0, 1)



		[NoScaleOffset] _ReflectionTex(""_ReflectionTex"", 2D) = ""black"" {}
		_ReflectionTex_size(""_ReflectionTex_size"", Float) = 256.0 //Range(0, 1)
		[NoScaleOffset] _ReflectionTex_temp(""_ReflectionTex_temp"", 2D) = ""black"" {}
		_ReflectionTex_temp_size(""_ReflectionTex_temp_size"", Float) = 128.0 //Range(0, 1)

		[NoScaleOffset] _RefractionTex(""_RefractionTex"", 2D) = ""black"" {}
		_RefractionTex_size(""_RefractionTex_size"", Float) = 512.0 //Range(0, 1)
		[NoScaleOffset] _RefractionTex_temp(""_RefractionTex_temp"", 2D) = ""black"" {}
		_RefractionTex_temp_size(""_RefractionTex_temp_size"", Float) = 512.0 //Range(0, 1)

		[NoScaleOffset] _BakedData(""_BakedData "", 2D) = ""black"" {}
		_BakedData_size(""_BakedData_size"", Float) = 256.0 //Range(0, 1)
		[NoScaleOffset] _BakedData_temp(""_BakedData_temp "", 2D) = ""black"" {}
		_BakedData_temp_size(""_BakedData_temp_size"", Float) = 256.0 //Range(0, 1)

			_MTDistortion(""_MTDistortion"", Float) = 0 //Range(1, 256)

		[NoScaleOffset] _RimGradient(""_RimGradient"", 2D) = ""black"" {}

			RIM_Minus(""RIM_Minus"", Float) = 0.4 //Range(1, 256)
			RIM_Plus(""RIM_Plis"", Float) =50 //Range(1, 256)
			POSTRIZE_Colors(""POSTRIZE_Colors"", Float) = 12 //Range(1, 24)

			_MyNearClipPlane(""_MyNearClipPlane"", Float) = 0//Range(1, 256)
			_MyFarClipPlane(""_MyFarClipPlane"", Float) = 1000//Range(1, 256)

			_MultyOctaveNormals(""_MultyOctaveNormals"", Float) = 5//Range(1, 256)

			_SurfaceFoamAmount(""_SurfaceFoamAmount"", Float) = 10//Range(1, 256)
			_SurfaceFoamContrast(""_SurfaceFoamContrast"", Float) = 200//Range(1, 256)
			_SUrfaceFoamVector(""_SUrfaceFoamVector"", Vector) = (0.12065329648999294186660041025867, 0.98533525466827569191057001711249, 0.12065329648999294186660041025867, 0)


			_ReflectionBakeLayers(""_ReflectionBakeLayers"", Vector) = (255, 255, 255, 255)
			_RefractionBakeLayers(""_RefractionBakeLayers"", Vector) = (255, 255, 255, 255)
			_ZDepthBakeLayers(""_ZDepthBakeLayers"", Vector) = (255, 255, 255, 255)

			_DetileAmount(""_DetileAmount"", Float) = 0.15//Range(1, 256)
			_DetileFriq(""_DetileFriq"", Float) = 5//Range(1, 256)
			_VERTEX_ANIM_DETILEFRIQ(""_VERTEX_ANIM_DETILEFRIQ"", Float) = 5//Range(1, 256)
			MainTexSpeed(""MainTexSpeed"", Float) = 1//Range(1, 256)
			_ReflectionDesaturate(""_ReflectionDesaturate"", Float) = 0.5//Range(1, 256)
			_RefractionDesaturate(""_RefractionDesaturate"", Float) = 0.5//Range(1, 256)

			_sinFriq(""_sinFriq"", Float) = 7.28//Range(1, 256)
			_sinAmount(""_sinAmount"", Float) = 0.1//Range(1, 256)


			_APPLY_REFR_TO_SPECULAR_DISSOLVE(""_APPLY_REFR_TO_SPECULAR_DISSOLVE"", Float) = 1
			_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST(""_APPLY_REFR_TO_SPECULAR_DISSOLVE_FAST"", Float) = 1



  _UFSHORE_Amount_1(""_UFSHORE_Amount_1"", Float ) = 5 
  _UFSHORE_Amount_2(""_UFSHORE_Amount_2"", Float ) = 5 
  _UFSHORE_Length_1(""_UFSHORE_Length_1"", Float ) = 10 
  _UFSHORE_Length_2(""_UFSHORE_Length_2"", Float ) = 10 
  _UFSHORE_Color_1(""_UFSHORE_Color_1"", Color ) = (1,1,1,1)
  _UFSHORE_Color_2(""_UFSHORE_Color_2"", Color ) = (1,1,1,1)
  _UFSHORE_Distortion_1(""_UFSHORE_Distortion_1"", Float ) = 0.8
  _UFSHORE_Distortion_2(""_UFSHORE_Distortion_2"", Float ) = 0.8
  _UFSHORE_Tile_1(""_UFSHORE_Tile_1"", Vector ) = (1,1,1,1)
  _UFSHORE_Tile_2(""_UFSHORE_Tile_2"", Vector ) = (1,1,1,1)
  _UFSHORE_Speed_1(""_UFSHORE_Speed_1"", Float ) = 1
  _UFSHORE_Speed_2(""_UFSHORE_Speed_2"", Float ) = 1
  _UFSHORE_LowDistortion_1(""_UFSHORE_LowDistortion_1"", Float ) = 1
  _UFSHORE_LowDistortion_2(""_UFSHORE_LowDistortion_2"", Float ) = 1
  _UFSHORE_AlphaAmount_1(""_UFSHORE_AlphaAmount_1"", Float ) = 0.25 
  _UFSHORE_AlphaAmount_2(""_UFSHORE_AlphaAmount_2"", Float ) = 0.25 
  _UFSHORE_AlphaMax_1(""_UFSHORE_AlphaMax_1"", Float ) = 1 
  _UFSHORE_AlphaMax_2(""_UFSHORE_AlphaMax_2"", Float ) = 1 
  _UFSHORE_ShadowV1_1(""_UFSHORE_ShadowV1_1"", Float ) = 1 
  _UFSHORE_ShadowV2_1(""_UFSHORE_ShadowV2_1"", Float ) = 1 
  _UFSHORE_ShadowV1_2(""_UFSHORE_ShadowV1_2"", Float ) = 1 
  _UFSHORE_ShadowV2_2(""_UFSHORE_ShadowV2_2"", Float ) = 1 
  MAIN_TEX_FoamGradWavesSpeed_1(""MAIN_TEX_FoamGradWavesSpeed_1"", Float ) = 0.1
  MAIN_TEX_FoamGradWavesSpeed_2(""MAIN_TEX_FoamGradWavesSpeed_2"", Float ) = 0.1 
  MAIN_TEX_FoamGradDirection_1(""MAIN_TEX_FoamGradDirection_1"", Float ) = 0.01
  MAIN_TEX_FoamGradDirection_2(""MAIN_TEX_FoamGradDirection_2"", Float ) = 0.01 
  MAIN_TEX_FoamGradTile_1(""MAIN_TEX_FoamGradTile_1"", Float ) = 0.1
  MAIN_TEX_FoamGradTileYYY_1(""MAIN_TEX_FoamGradTileYYY_1"", Float ) = 0
  MAIN_TEX_FoamGradTile_2(""MAIN_TEX_FoamGradTile_2"", Float ) = 0.1
  _UFSHORE_ADDITIONAL_Length_1(""_UFSHORE_ADDITIONAL_Length_1"", Float ) = 1
  _UFSHORE_ADDITIONAL_Length_2(""_UFSHORE_ADDITIONAL_Length_2"", Float ) = 1

  _UFSHORE_ADD_Amount_1(""_UFSHORE_ADD_Amount_1"", Float ) = 1
  _UFSHORE_ADD_Amount_2(""_UFSHORE_ADD_Amount_2"", Float ) = 1
  _UFSHORE_ADD_Tile_1(""_UFSHORE_ADD_Tile_1"", Float ) = 0.1
  _UFSHORE_ADD_Tile_2(""_UFSHORE_ADD_Tile_2"", Float ) = 0.1
  _UFSHORE_ADD_Distortion_1(""_UFSHORE_ADD_Distortion_1"", Float ) = 1
  _UFSHORE_ADD_Distortion_2(""_UFSHORE_ADD_Distortion_2"", Float ) = 1
  _UFSHORE_ADD_LowDistortion_1(""_UFSHORE_ADD_LowDistortion_1"", Float ) = 0.1
  _UFSHORE_ADD_LowDistortion_2(""_UFSHORE_ADD_LowDistortion_2"", Float ) = 0.1
  _UFSHORE_ADD_Color_1(""_UFSHORE_ADD_Color_1"", Color ) = (1,1,1,1)
  _UFSHORE_ADD_Color_2(""_UFSHORE_ADD_Color_2"", Color ) = (1,1,1,1)
  SHORE_USE_ADDITIONALCONTUR_POW_Amount_1(""SHORE_USE_ADDITIONALCONTUR_POW_Amount_1"", Float ) = 10
  SHORE_USE_ADDITIONALCONTUR_POW_Amount_2(""SHORE_USE_ADDITIONALCONTUR_POW_Amount_2"", Float ) = 10
  _UFSHORE_UNWRAP_LowDistortion_1(""_UFSHORE_UNWRAP_LowDistortion_1"", Float ) = 0

  _LOW_DISTOR_Tile(""_LOW_DISTOR_Tile"", Float ) =20
  _LOW_DISTOR_Speed(""_LOW_DISTOR_Speed"", Float ) = 8
  _LOW_DISTOR_Amount(""_LOW_DISTOR_Amount"", Float ) = 1
  //_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount(""_BAKED_DEPTH_EXTERNAL_TEXTURE_Amount"", Float ) = 1

  _LOW_DISTOR_MAINTEX_Tile(""_LOW_DISTOR_MAINTEX_Tile"", Float ) =20
  _LOW_DISTOR_MAINTEX_Speed(""_LOW_DISTOR_MAINTEX_Speed"", Float ) = 8
  MAIN_TEX_MixOffset(""MAIN_TEX_MixOffset"", Float ) = 0


  _FoamAmount_SW(""_FoamAmount_SW"", Float ) = 5 //Range(0,10)
	_FoamLength_SW(""_FoamLength_SW"", Float ) = 10 //Range(0,20)
	_FoamColor_SW(""_FoamColor_SW"", COLOR) = (1,1,1,1)
		_FoamDistortion_SW(""_FoamDistortion_SW"", Float ) = 1 //Range(0,10)
		_FoamDistortionFade_SW(""_FoamDistortionFade_SW"", Float) = 0.8 //Range(0,1)
		_FoamDirection_SW(""_FoamDirection_SW"", Float) = 0.05 //Range(0,1)
_WaterfrontFade_SW(""_WaterfrontFade_SW"", Float ) = 0//Range(0,30) 
_LightNormalsFactor(""_LightNormalsFactor"", Float ) = 1//Range(0,30) 


	_FoamTextureTiling_SW(""_FoamTextureTiling_SW"", Float ) = 1 //Range(0.01,2)
	_FoamWavesSpeed_SW(""_FoamWavesSpeed_SW"", Float ) = 0.15//Range(0,2 )
		[NoScaleOffset] _ShoreWavesGrad(""_ShoreWavesGrad"",  2D) = ""white"" {}
	_ShoreWavesRadius(""_ShoreWavesRadius"", Float ) = 0.1//Range(0,2 )
	_ShoreWavesQual(""_ShoreWavesQual"", Float ) = 2//Range(0,2 )
	_FoamLShoreWavesTileY_SW(""_FoamLShoreWavesTileY_SW"", Float ) = 1//Range(0,2 )
	_FoamLShoreWavesTileX_SW(""_FoamLShoreWavesTileX_SW"", Float ) = 1//Range(0,2 )
	_FoamMaskOffset_SW(""_FoamMaskOffset_SW"", Float ) = 0.0//Range(0,2 )

	_BumpZOffset(""_BumpZOffset"", Float ) = 1.0//Range(0,2 )
	_BumpZFade(""_BumpZFade"", Float ) = 0//Range(0,2 )
	_RefractionBlendOffset(""_RefractionBlendOffset"", Float ) = 0.0//Range(0,2 )
	_RefractionBlendFade(""_RefractionBlendFade"", Float ) = 1//Range(0,2 )
		[NoScaleOffset] _FoamTexture_SW(""_FoamTexture_SW"",  2D) = ""white"" {}

	_RefrBled_Fres_Amount(""_RefrBled_Fres_Amount"", Float ) = 1//Range(0,2 )
	_RefrBled_Fres_Pow(""_RefrBled_Fres_Pow"", Float ) = 1//Range(0,2 )


	_LutAmount(""_LutAmount"", Float ) = 1.0//Range(0,2 )
	[NoScaleOffset]	 _Lut2D(""_Lut2D"",  2D) = ""defaulttexture"" {}
		_Lut2D_params(""_Lut2D_params"", Vector) = (0,0,0,0)

	_GAmplitude(""Wave Amplitude"", Vector) = (0.3 ,0.35, 0.25, 0.25)
	_GFrequency(""Wave Frequency"", Vector) = (1.3, 1.35, 1.25, 1.25)
	_GSteepness(""Wave Steepness"", Vector) = (1.0, 1.0, 1.0, 1.0)
	_GSpeed(""Wave Speed"", Vector) = (1.2, 1.375, 1.1, 1.5)
	_GDirectionAB(""Wave Direction"", Vector) = (0.3 ,0.85, 0.85, 0.25)
	_GDirectionCD(""Wave Direction"", Vector) = (0.1 ,0.9, 0.5, 0.5)
//_GAmplitude(""Wave Amplitude"", Vector) = 1
//	_GFrequency(""Wave Frequency"", Vector) = 1
//	_GSteepness(""Wave Steepness"", Vector) = 1
//	_GSpeed(""Wave Speed"", Vector) = 1
//	_GDirectionAB(""Wave Direction"", Vector) =(0.3 ,0.85, 0.85, 0.25)
//
		//	_CameraFarClipPlane(""_CameraFarClipPlane"", FLOAT) = 4
		//_LightPos(""_LightPos"", Vector) = (2001,1009,3274,0)


	_FoamDistortionTexture(""_FoamDistortionTexture"", Float ) = 1
	_ShoreDistortionTexture(""_ShoreDistortionTexture"", Float ) = 1

	_MOR_Offset(""_MOR_Offset"", Float ) = 1
	_MOR_Base(""_MOR_Base"", Float ) = 1
	_C_BLUR_R(""_C_BLUR_R"", Float ) = 0.05
	_C_BLUR_S(""_C_BLUR_S"", Float ) = 0.05
	_MOR_Tile(""_MOR_Tile"", Float ) = 82

	_FracTimeX(""_FracTimeX"", Float ) = 0
	_FracTimeW(""_FracTimeW"", Float ) = 0

	_WaveGrad0(""_WaveGrad0"", COLOR) = (1,1,1,1)
	_WaveGrad1(""_WaveGrad1"", COLOR) = (1,1,1,1)
	_WaveGrad2(""_WaveGrad2"", COLOR) = (1,1,1,1)
		_WaveGradMidOffset(""_WaveGradMidOffset"", Float) = 0.5
		_WaveGradTopOffset(""_WaveGradTopOffset"", Float) = 1


	_SFW_Tile(""_SFW_Tile"", Vector) = (1,1,1,1)
		_SFW_Speed(""_SFW_Speed"", Vector) = (1,1,1,1)
		_SFW_Amount(""_SFW_Amount"", Float) = 1
		_SFW_NrmAmount(""_SFW_NrmAmount"", Float) = 1
		_SFW_Dir(""_SFW_Dir"", Vector) = (1,0,0,0)
		_SFW_Dir1(""_SFW_Dir1"", Vector) = (0.9,0,0.1,0)
		_SFW_Distort(""_SFW_Distort"", Float) = 1




	[NoScaleOffset] _CAUSTIC_MAP(""_CAUSTIC_MAP "", 2D) = ""black"" {}
_CAUSTIC_FOG_Amount(""_CAUSTIC_FOG_Amount"", Float ) = 1 //Range(0.1,32)
_CAUSTIC_FOG_Pow(""_CAUSTIC_FOG_Pow"", Float ) = 4 //Range(0.1,32)
	_CAUSTIC_Speed(""_CAUSTIC_Speed"", Float ) = 1 //Range(0.1,32)
	//_CAUSTIC_Tiling(""_CAUSTIC_Tiling"", Float ) = 1 //Range(0.1,32)
	_CAUSTIC_Tiling(""_CAUSTIC_Tiling"", Vector ) = (1,1,0,0)
	_CAUSTIC_Offset(""_CAUSTIC_Offset"", Vector ) = (1,2,1,0)

	_CAUSTIC_PROC_Tiling(""_CAUSTIC_PROC_Tiling"", Vector ) = (1,1,1,1)
	_CAUSTIC_PROC_GlareSpeed(""_CAUSTIC_PROC_GlareSpeed"", Float ) = 1 //Range(0.1,32)
	_CAUSTIC_PROC_Contrast(""_CAUSTIC_PROC_Contrast"", Float ) = 1 //Range(0.1,32)
	_CAUSTIC_PROC_BlackOffset(""_CAUSTIC_PROC_BlackOffset"", Float ) = 1 //Range(0.1,32)


	DOWNSAMPLING_SAMPLE_SIZE(""DOWNSAMPLING_SAMPLE_SIZE"", Float ) = 0.33




	_CN_DISTANCE(""_CN_DISTANCE"", Float ) = 200
	_CN_TEXEL(""_CN_TEXEL"", Float ) = 0.01
	_CLASNOISE_PW(""_CLASNOISE_PW"", Float ) = 0.35
	_CN_AMOUNT(""_CN_AMOUNT"", Float ) = 5
	_CN_SPEED(""_CN_SPEED"", Float ) = 2
	_CN_TILING(""_CN_TILING"", Vector) = (5,5,1,1)


	}

    SubShader{
";
    //OPT-OPQ// Tags{ ""Queue"" = ""Geometry"" ""RenderType"" = ""Opaque"" ""LightMode"" = ""ForwardBase"" ""PreviewType"" = ""Plane""  ""ForceNoShadowCasting"" = ""False"" ""IgnoreProjector"" = ""False"" }
    //Tags{ ""LIGHTMODE"" = ""ForwardBase"" ""QUEUE"" = ""Transparent-5"" ""IGNOREPROJECTOR"" = ""true"" ""RenderType"" = ""Transparent"" }//OPT-TRANSP//
    
    // ""LightMode"" = ""ForwardBase""
    //
    
    // ""LightMode"" = ""Always""
    //""PreviewType"" = ""Plane""
    public static string prekeywords = @"
        Cull Off								
        
Pass{
CGPROGRAM

            " +
                                       @"#pragma vertex vert
            " +
                                       @"#pragma fragment frag
            " +
                                       @"#pragma multi_compile_fog
            " +
                                       @"#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
            " +
                                       @"";
                                       
    //multi_compile_fwdbase noforwardadd
    public static string postkeywords = /*@"
            " +
@"#if !defined(USE_LIGHTMAPS)//?not sure do this work
            " +
@"#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap  novertexlight
            " +
@"#else
            " +
@"#pragma multi_compile_fwdbase  //nolightmap nodirlightmap nodynlightmap novertexlight
            " +
@"#endif
            " +*/
        @"#pragma fragmentoption ARB_precision_hint_fastest
            " +
        @"#pragma multi_compile ORTO_CAMERA_ON _
            " +
        @"#pragma multi_compile WATER_DOWNSAMPLING WATER_DOWNSAMPLING_HARD _
           " +

        /*@" #pragma glsl
        " +*/
        
        @"";
    public static string closeshader = @"
				ENDCG
    }
    //UsePass ""Legacy Shaders/VertexLit/SHADOWCASTER""
    //#if SHADER_TARGET < 30
    //#else
    //#endif
}
//Fallback ""Mobile/VertexLit""
}
";
}
#endif
}