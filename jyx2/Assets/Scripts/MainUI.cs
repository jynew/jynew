using System.Collections;
using System.Collections.Generic;
using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Button m_XiakeButton;
    public Button m_BagButton;
    public Button m_BackButton; // for test
    public void OnXiakeButtonClicked()
    {
        XiakePanelUI.Create(GameRuntimeData.Instance.Player, GameRuntimeData.Instance.Team, this.transform);
    }

    public void OnBagButtonClicked()
    {
        BagPanel.Create(this.transform, GameRuntimeData.Instance.Items, OnUseItem);
    }

    public void OnBackButtonClicked()
    {
        var levelMaster = LevelMaster.Instance;
        if (!levelMaster.GetCurrentGameMap().Tags.Contains("WORLDMAP"))
        {
            levelMaster.PlayLeaveMusic(levelMaster.GetCurrentGameMap());
            //退出到大地图
            LevelLoader.LoadGameMap("0_BigMap");
        }
    }

    void OnUseItem(int id)
    {
        if (id == -1) return;

        var item = ConfigTable.Get<Jyx2Item>(id);
        if(item == null)
        {
            Debug.LogError("调用了错误的物品id=" + id);
            return;
        }

        //可使用道具
        if (item.ItemType != 3)
            return;

        var runtime = GameRuntimeData.Instance;
        GameUtil.SelectRole(runtime.Team, (selectRole) => {
            if (selectRole == null) return;

            if (selectRole.CanUseItem(id))
            {
                selectRole.UseItem(item);
                runtime.AddItem(id, -1);
                GameUtil.DisplayPopinfo($"{selectRole.Name}使用了{item.Name}");
            }
        });
    }
}
