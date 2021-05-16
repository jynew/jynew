Talk(109, "公子近來無恙？", "talkname109", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托王姑娘的福，一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "還好，不過若有王姑娘在隊　中指導我們攻擊與防禦，我　們會更好．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(109, "你的隊伍已滿，我無法加入．", "talkname109", 0);
        do return end;
::label1::
        Talk(109, "好吧，我加入你們．", "talkname109", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(76);
do return end;
