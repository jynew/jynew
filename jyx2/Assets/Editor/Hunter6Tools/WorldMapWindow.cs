/* reorderList的方法参考如下，还挺详细的代码：
 * https://blog.csdn.net/qq_35361471/article/details/84715930 *
 * 初代的 reorderlist 介绍，也是很好（但英文）
 * https://www.cnblogs.com/hont/p/5458021.html
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using WH.Editor;
/// <summary>
/// 想做一个短篇天龙Mods，本身缺框架的原因，代码还是挺不稳定的，中间关联的代码也不好提交；
/// 所以搞这-触发器管理，可同步场景的触发器和数据表（config/)
/// </summary>
public class WorldMapWindow : EditorWindow
{
    [MenuItem("项目快速导航/场景Teleporter数据管理",priority = 103)]
    public  static void Start()
    {
        GetWindow<WorldMapWindow>().Show();
    }
    [Serializable]
    public struct  TriggerInScene
    {
        public GameObject go;
        public string name;
    }
    private Rect paramArea; // 参数区域
    private Rect stateArea; // 状态区域
    private float percent_of_param = 0.4f; // 参数所占用的比例
    private Rect paramResizeArea;
    const float ResizeWidth = 10f;
    private bool isResizingParamArea = false; // 是否正在调整参数区域
   // private List<GraphLayer> graphLayers = new List<GraphLayer>();
   public static Color GridColor { get; } = new Color(0,0,0,0.2f);
   public static Color BackgroundColor { get; } = new Color(0, 0, 0, 0.25f);
   public static Color SelctionColor { get; } = new Color(100,200,255,255)/255;
   public static Color TransitionColor { get; } = new Color(0,0.78f,1,1);
   public static Color ParaBackground { get; } = new Color(48,48,48,255)/255.0f;
   private Vector2 scrollView;
   private ReorderableList reorderableList;
   private HListView _lvCfg;
   private List<string> lstTriggersName=new List<string>();

  // private List<GameObject> lstTriggers = new List<GameObject>();
  private List<TriggerInScene> lstTriggers = new List<TriggerInScene>();
   // private void Awake()
    // {
    //     titleContent = new GUIContent("场景触发器管理");
    //     DoLoadSceneTriggers();
    //     _lvCfg = new HListView(); 
    //     _lvCfg.SetData(lstTriggers.Count, OnItem2GUI);
    // }

    private void OnEnable()
    {
        titleContent = new GUIContent("场景触发器管理");
        DoLoadSceneTriggers();
        _lvCfg = new HListView(); 
        _lvCfg.SetData(lstTriggers.Count, OnItem2GUI);
        
    }

    void OnItem2GUI(HListViewItem item)
    {
       item.text = lstTriggers[item.row].name;

    }

    void DoLoadSceneTriggers()
    {
        lstTriggers.Clear();
        var root = GameObject.Find("Level/Triggers");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var child = root.transform.GetChild(i);
            lstTriggersName.Add(child.gameObject.name);
            lstTriggers.Add(new TriggerInScene(){name = child.name,go= child.gameObject});  
  
        }
        
    }

    private void Update()
    {
        if(isResizingParamArea)
            Repaint();

        // if (graphLayers != null)
        // {
        //     foreach (var item in graphLayers)
        //     {
        //         item.Update();
        //     }
        // }

    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新"))
        {
            DoLoadSceneTriggers();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("定位选择"))
        {
            var obj = lstTriggers[reorderableList.index];
            EditorGUIUtility.PingObject(obj.go);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        float y = 30f;
        paramArea.Set(0, y, this.position.width * percent_of_param /*- ResizeWidth / 2*/, this.position.height-60);
        stateArea.Set(paramArea.width /*+ ResizeWidth*/, y, this.position.width * (1 - percent_of_param) /*- ResizeWidth / 2*/, this.position.height);
        //拖动的逻辑计算（拖动的 Repaint()需要在 Update()处理，OnGUI())
        ParamAreaResize();
        
        /////////////////// 多余的 （暂时 )///////////////////////
        // if (graphLayers.Count == 0)
        // {
        //     InitLayers();
        // }
        //
        // foreach (var item in graphLayers)
        // {
        //     item.OnGUI(stateArea);
        // }
        /////////////////////////////////////////////////////////
        
        //画左边背景色
        EditorGUI.DrawRect(paramArea, BackgroundColor);
        OnTriggersGUI(paramArea);
        OnCfgGUi(stateArea);
    }



    private void InitLayers() {
       // graphLayers.Add(new BackgroundLayer(null));
     //   graphLayers.Add(new TransitionLayer(null));
       // graphLayers.Add(new StateNodeLayer(null));
    }
    /// <summary>
    /// 调整参数区域
    /// </summary>
    private void ParamAreaResize() {
        paramResizeArea.Set(paramArea.width - ResizeWidth / 2, 0, ResizeWidth, position.height);

        EditorGUIUtility.AddCursorRect(paramResizeArea, MouseCursor.ResizeHorizontal);

        if ( Event.current.type == EventType.MouseDown && paramResizeArea.Contains(Event.current.mousePosition)) {
            isResizingParamArea = true;
            Event.current.Use();
        }

        if ( isResizingParamArea ) {
            percent_of_param = Mathf.Clamp(  Event.current.mousePosition.x / position.width,0.1f,0.5f);
            //Event.current.Use();
        }

        if (Event.current.type == EventType.MouseUp  ) {
            isResizingParamArea = false;
            if (paramResizeArea.Contains(Event.current.mousePosition)) { 
                Event.current.Use();
            }
        }

    }

    void OnCfgGUi(Rect rect)
    {
        GUILayout.BeginArea(rect);
        //scrollView = GUILayout.BeginScrollView(scrollView);
        //reorderableList.DoLayoutList();
        //GUILayout.EndScrollView();
        _lvCfg.DoGUI();
        GUILayout.EndArea();
    }

    void OnTriggersGUI(Rect rect)
    {
        GUI.Box(rect, string.Empty, GUI.skin.GetStyle("CN Box"));

        List<string> ss = new List<string>();
        if (lstTriggers!=null&&lstTriggers.Count ==0)
        {
            ss.Add("11111");
            ss.Add("没有读到数字");
            ss.Add("11111");
        }
        else
        {
             ss = lstTriggersName;
        }
      //  List<TriggerInScene> ss = lstTriggers;
        if (reorderableList == null)
        {

            //ReorderList 的初始化逻辑所在
            reorderableList =
                new ReorderableList(ss, typeof(List<string>), true, true, true, true) {headerHeight = 0};


            //reorderableList.onAddDropdownCallback += OnAddDropdownCallback;
            reorderableList.onRemoveCallback += OnRemoveCallback;
            //reorderableList.drawElementCallback += DrawElementCallback;

            //reorderableList.onCanRemoveCallback += onCanRemoveCallback;
            //reorderableList.onCanAddCallback += onCanRemoveCallback;
           // reorderableList.onSelectCallback += OnSelectedCallback;
            reorderableList.displayAdd = false;
        }
        else
        {

            //  reorderableList.list = this.Context.RuntimeFSMController.parameters;
          //  reorderableList.list = ss;
        }

        GUILayout.BeginArea(rect);
        scrollView = GUILayout.BeginScrollView(scrollView);
        reorderableList.DoLayoutList();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    // void OnSelectedCallback(ReorderableList list)
    // {
    //     list.index
    // }

    void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        ///// 这个方法好像要配合使用才行 //////////
        /// reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("colors"), true, true,
        //true,
       // true);
       // Debug.Log(reorderableList);
       // Debug.Log(reorderableList.serializedProperty);
        // var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 100;
        // EditorGUI.PropertyField(
        //     new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
        //     element.FindPropertyRelative("name"), GUIContent.none);
        // EditorGUI.PropertyField(
        //     new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
        //     element.FindPropertyRelative("go"), GUIContent.none);
        // // EditorGUI.PropertyField(
        // //     new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
        // //     element.FindPropertyRelative("Count"), GUIContent.none);
        GUILayout.BeginArea(rect);
        var obj = lstTriggers[index];
        GUILayout.Label(obj.name);
        GUILayout.Label("-");
        if (GUILayout.Button("Ping"))
        {
            Debug.LogError("Click i=" + index);
        }
        GUILayout.EndArea();
    }

    void OnRemoveCallback(ReorderableList list)
    {
        Debug.LogError("list count="+list.count);
        if (EditorUtility.DisplayDialog("", "会改变地图或场景触发器，确实是否删除？请先做好备份。", "确认删除","取消"))
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }
    }
}
