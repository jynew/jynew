if UseItem(195) == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "动手挖墓吧！希望别有僵尸跑出来吓人。", "talkname0", 1);
    PlayAnimation(-1, 6704, 6714);
    PlayAnimation(-1, 6704, 6714);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);

    --缺少timeline
    --jyx2_PlayTimeline("[Timeline]ka564_小虾米破庙挖坟", 0, false);
    jyx2_PlayTimelineSimple("[Timeline]ka564_小虾米破庙挖坟", true)
    jyx2_Wait(5.2)
    --jyx2_StopTimeline("[Timeline]ka564_小虾米破庙挖坟");

    DarkScence();
    ModifyEvent(-2, -2, -2, -2, 565, -1, -1, 6698, 6698, 6698, -2, -2, -2);
    jyx2_ReplaceSceneObject("","Dynamic/Tomb", "0")
    jyx2_ReplaceSceneObject("","Dynamic/Tomb_2", "1")
    PlayAnimation(-1, 6702, 6702);
    LightScence();

    Talk(0, "啊！真累，盗墓的工作真不轻松，好在有点收获．这是什么东西来着，全书尽是怪异的文字，封皮写着……“广陵散”……", "talkname0", 1);
    AddItem(177, 1);
    ModifyEvent(-2, -2, -2, -2, 565, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
