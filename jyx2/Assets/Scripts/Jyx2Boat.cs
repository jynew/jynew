using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Jyx2Boat : MonoBehaviour
{
    Jyx2Player player
    {
        get
        {
            if (_player == null)
            {
                LevelMaster lm = LevelMaster.Instance;
                _player = lm.GetPlayer();
            }
            return _player;
        }
    }

    Jyx2Player _player;

    bool showingGetInBoat = false;

    void OnTriggerEnter(Collider other)
    {
        if (!player.IsOnBoat)
        {
            showingGetInBoat = true;
            Jyx2InteractiveButton.Show("上船", GetInBoat);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!player.IsOnBoat)
        {
            if(showingGetInBoat == false)
            {
                showingGetInBoat = true;
                Jyx2InteractiveButton.Show("上船", GetInBoat);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!player.IsOnBoat)
        {
            showingGetInBoat = false;
            Jyx2InteractiveButton.Hide();
        }
    }


    //上船
    public void GetInBoat()
    {
        player.GetInBoat(this);
        showingGetInBoat = false;
        Jyx2InteractiveButton.Show("上岸", GetOutBoat);
    }

    //下船
    public void GetOutBoat()
    {
        if (player.GetOutBoat())
        {
            Jyx2InteractiveButton.Hide();
        }
        else
        {
            GameUtil.DisplayPopinfo("此处不能靠岸！");
        }
    }
}
