Talk(9, "公子別來無恙？", "talkname9", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了張兄的九陽神功，一路上頗多兇險，是否可以再請張兄出馬？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(9, "你的隊伍已滿，我無法加入．", "talkname9", 0);
        do return end;
::label1::
        Talk(9, "好的．", "talkname9", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(9);
do return end;
