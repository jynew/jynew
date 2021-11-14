using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Reflection;

public class DebugTools : EditorWindow
{
    [MenuItem("Tools/[OPEN DEBUG PANEL]", false, 1)]
    static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        DebugTools window = EditorWindow.GetWindow<DebugTools>();
    }

    public void Init()
    {
        this.titleContent = new GUIContent("Debug Console");
        StreamReader sr = new StreamReader("DebugCommands.txt");
        string content = sr.ReadToEnd();
        sr.Close();
        cmds.Clear();
        foreach (var line in content.Split('\n'))
        {
            if (!line.Contains(":")) continue;
            cmds.Add(line.Split(':')[0].Replace("\n", "").Replace("\r", ""), line.Split(':')[1].Replace("\n", "").Replace("\r", ""));
        }
    }

    static private Dictionary<string, string> cmds = new Dictionary<string, string>();

    private string text;

    void Awake()
    {
        Init();
        Show();
        UnityEngine.Object.DontDestroyOnLoad(this);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("指令");
        text = EditorGUILayout.TextArea(text, GUILayout.Width(200));
        if (GUILayout.Button("执行", GUILayout.Width(100)))
        {
            ExecuteCommand(text);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("刷新指令集", GUILayout.Width(100)))
        {
            this.Init();
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        int count = 0;
        foreach (var cmd in cmds)
        {
            string key = cmd.Key;
            string value = cmd.Value;
            if (count % 4 == 0)
            {
                GUILayout.BeginHorizontal();
            }
            if (GUILayout.Button(key, GUILayout.Width(60), GUILayout.Height(30)))
            {
                ExecuteCommand(value);
            }

            count++;
            if (count % 4 == 0)
            {
                GUILayout.EndHorizontal();
            }
        }

    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    void ExecuteCommand(string cmd)
    {
        Jyx2Console.RunConsoleCommand(cmd);
    }

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        var getGroup = sizesType.GetMethod("GetGroup");
        var gameViewSizesInstance = instanceProp.GetValue(null, null);
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    public static string GetParentName(Transform tf)
    {
        if (tf == null)
            return "";
        Transform parent = tf.parent;
        if (parent == null)
            return tf.name;
        return GetParentName(parent) + "/" + tf.name;
    }
}
