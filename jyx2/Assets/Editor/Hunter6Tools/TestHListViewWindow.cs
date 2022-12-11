using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TestHListViewWindow : EditorWindow
{
    [MenuItem("Test/Hlistview")]
    public static  void Start()
    {
        GetWindow<TestHListViewWindow>();
    }

    private HListView hv = new HListView();



    private void OnGUI()
    {
        hv.DoGUI();
    }
}
