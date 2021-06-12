using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// A behaviour that is attached to a playable
public class TestPlayableBehaviour : PlayableBehaviour
{
    //显示文字的控件
    public Text ShowNumberText;
    public int StartNum;
    float time;
    int currentNum;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        Debug.Log("OnGraphStart Called");
        currentNum = StartNum;
        ShowNumberText.text = "Start Number Is " + StartNum;
        Debug.Log(ShowNumberText.text);
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        Debug.Log("OnGraphStop Called");
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        Debug.Log("OnBehaviourPlay Called");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        Debug.Log("OnBehaviourPause Called");
        ShowNumberText.text = "End Number Is " + currentNum;
        Debug.Log(ShowNumberText.text);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        Debug.Log("PrepareFrame Called");
        time += Time.deltaTime;
        if (time > 1.0f)
        {
            currentNum++;
            ShowNumberText.text = "Current Number Is " + currentNum;
            Debug.Log(ShowNumberText.text);
            time -= 1.0f;
        }
    }
}
