/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using UnityEngine;
using Jyx2;
using Sirenix.OdinInspector;

public class Jyx2SkillEditor : MonoBehaviour
{
    public BattleRole player;

    public Jyx2SkillEditorEnemy[] enemys;

    public Transform[] faceTrans;
    public Transform[] lineTrans;
    public Transform[] crossTrans;

    [LabelText("是否测试左右互搏")]
    public bool TestZuoyouhubo = false;
    // Start is called before the first frame update
    async void Start()
    {
        /*
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
        */


        player.IsInBattle = true;
        //Container.TryResolve<IXLsReloader>()?.Do();

        await RuntimeEnvSetup.Setup();
        
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(SkillEditorUIPanel),player,enemys);
    }

    /*
    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        updateExcel = true;
    }

    bool updateExcel = false;
    */



    // Update is called once per frame
    void Update()
    {
      

        /*
        if (updateExcel)
        {
            updateExcel = false;
            Container.TryResolve<IXLsReloader>()?.Do();
        }
        */

      
    }

    /// <summary>
    /// 预览技能
    /// </summary>
    /// <param name="skillName"></param>
    public void PreviewSkill(string skillName)
    {
        var skillEditorUIPanel = FindObjectOfType<SkillEditorUIPanel>();
        skillEditorUIPanel.SwitchToSkill(skillName);
    }
}
