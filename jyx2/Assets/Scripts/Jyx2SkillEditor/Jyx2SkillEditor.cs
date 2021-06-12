using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSFrameWork;
using Jyx2;
using HSFrameWork.ConfigTable;
using System.IO;
using System.Threading;
using HSFrameWork.Common;
using Jyx2.Middleware;
using Jyx2.Setup;

public class Jyx2SkillEditor : MonoBehaviour
{
    public MapRole player;

    public Jyx2SkillEditorEnemy[] enemys;
    // Start is called before the first frame update
    void Start()
    {
        FileSystemWatcher watcher;

        //监控excel文件夹
        watcher = new FileSystemWatcher();
        watcher.BeginInit();
        watcher.Path = "excel";
        watcher.EnableRaisingEvents = true;
        watcher.IncludeSubdirectories = true;
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size;
        watcher.Changed += Watcher_Changed;
        watcher.EndInit();


        player.IsInBattle = true;
        Container.TryResolve<IXLsReloader>()?.Do();

        Jyx2_UIManager.Instance.ShowUI("SkillEditorUIPanel",player,enemys);
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        updateExcel = true;
    }

    bool updateExcel = false;



    // Update is called once per frame
    void Update()
    {
      

        if (updateExcel)
        {
            updateExcel = false;
            Container.TryResolve<IXLsReloader>()?.Do();
        }

      
    }
}
