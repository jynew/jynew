Talk(45, "公子別來無恙？", "talkname45", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了薛先生的奇妙醫術，一路上難免病痛煩身，是否可以再請薛先生幫忙呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(45, "你的隊伍已滿，我無法加入．", "talkname45", 0);
        do return end;
::label1::
        Talk(45, "公子有需，薛某自當效力．", "talkname45", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(45);
do return end;
