using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : Jyx2_UIBase
{
    public Text name_text;
    public Text date_text;
    public Text note_text;
    

    protected override void OnCreate()
    {
        IsBlockControl = true;
        InitUi();
        name_text.text = GameRuntimeData.Instance.Player.Name;
        DateTime dt = DateTime.Now;
        date_text.text = dt.Subtract(GameRuntimeData.Instance.startDate).Days.ToString()+"天前";
        note_text.text = dt.ToLongDateString().ToString()+"\n在地球某处\n当地失踪人口又增加了\n一例……";
    }

    public void InitUi()
    {
        name_text=transform.Find("image/name").GetComponent<Text>();
        date_text=transform.Find("image/date").GetComponent<Text>();
        note_text=transform.Find("note").GetComponent<Text>();
        (transform.Find("operationArea/SavePanel").GetComponent<SavePanel>()).Show();
    }
    
    public async void BackToMainMenu()
    {
        LoadingPanel.Create(null).Forget();
    }
}
