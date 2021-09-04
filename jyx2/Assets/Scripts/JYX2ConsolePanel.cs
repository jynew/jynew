/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JYX2ConsolePanel : MonoBehaviour
{
    public InputField inputField;
    public Button confirmButton;
    public Button helpButton;
    public Button cmdListButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        helpButton.onClick.AddListener(OnHelp);
        cmdListButton.onClick.AddListener(OnCmdList);
    }

    void OnHelp()
    {
        Application.OpenURL("https://github.com/jynew/jynew/wiki/2.2%E6%8E%A7%E5%88%B6%E5%8F%B0");
    }

    void OnCmdList()
    {
        Application.OpenURL("https://github.com/jynew/jynew/wiki/%E6%B8%B8%E6%88%8F%E4%BA%8B%E4%BB%B6%E6%8C%87%E4%BB%A4");
    }
    
    void OnConfirm()
    {
        string cmd = inputField.text.Trim();

        if (string.IsNullOrEmpty(cmd))
            return;

        string[] paras = cmd.Split(' ');
        switch (paras[0])
        {
            case "map":
            case "scene":
                {
                    string id = paras[1];
                    foreach(var map in ConfigTable.GetAll<GameMap>())
                    {
                        if(map.Jyx2MapId == id)
                        {
                            //SceneManager.LoadScene(map.Key);
                            LevelLoader.LoadGameMap(map.Key);
                            break;
                        }
                    }
                    break;
                }
            case "event":
                {
                    string eventId = paras[1];
                    var eventLuaPath = "jygame/ka" + eventId;
                    Jyx2.LuaExecutor.Execute(eventLuaPath);

                    //停止导航
                    var levelMaster = LevelMaster.Instance;
                    if (levelMaster != null)
                    {
                        levelMaster.StopPlayerNavigation();
                    }
                    break;
                }
            case "item":
                {
                    int itemId = int.Parse(paras[1]);
                    int count = 1;
                    if (paras.Length > 2)
                    {
                        count = int.Parse(paras[2]);
                    }
                    Jyx2LuaBridge.AddItem(itemId, count);
                    break;
                }
            case "set_attack":
                {
                    int attack = int.Parse(paras[1]);
                    GameRuntimeData.Instance.Team[0].Attack = attack;
                    break;
                }
            case "quickbattle":
                {
                    int isOn = int.Parse(paras[1]);
                    Jyx2LuaBridge.isQuickBattle = (isOn == 1);
                    break;
                }
                //开启无敌
            case "whosyourdad":
                {
                    if(paras.Length > 1)
                    {
                        BattleManager.Whosyourdad = int.Parse(paras[1]) == 1;
                    }
                    else
                    {
                        BattleManager.Whosyourdad = true;
                    }
                    
                    break;
                }
            default:
                Debug.Log("没有识别的指令，将执行lua替代：" + cmd);
                string luaContent = cmd;
                Jyx2.LuaExecutor.ExecuteLua(luaContent);
                break;
        }
    }
}
