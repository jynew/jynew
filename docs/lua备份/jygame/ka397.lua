Talk(58, "少俠別來無恙，旅程一切還好吧．", "talkname58", 0);
Talk(0, "一切還好．", "talkname0", 1);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "不知楊兄是否有意與我為伴雲遊各地，一覽這五岳三川的風貌．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(58, "你的隊伍已滿，我無法加入．", "talkname58", 0);
        do return end;
::label1::
        Talk(58, "好啊！或許旅途中會有龍兒的下落也說不定．", "talkname58", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杨过加入队伍。场景07-编号06
        LightScence();
        Join(58);
        AddEthics(3);
do return end;
