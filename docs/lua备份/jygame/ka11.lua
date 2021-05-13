Talk(1, "有什麼要我幫忙的，儘管說出來．", "talkname1", 0);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "胡大哥肯隨我闖盪江湖否？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(1, "你的隊伍已滿，我無法加入．", "talkname1", 0);
        do return end;
::label1::
        Talk(1, "好．我就隨你一走．", "talkname1", 0);
        Talk(0, "胡大哥肯隨我闖盪江湖幫這個忙，那再好也不過了．", "talkname0", 1);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        jyx2_ReplaceSceneObject("","NPC/胡斐","");
        LightScence();
        Join(1);
        AddEthics(1);
do return end;
