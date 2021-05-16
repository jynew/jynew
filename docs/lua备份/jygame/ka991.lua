Talk(58, "兄弟近來如何？", "talkname58", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托楊兄的福，一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "近日旅途有些不順，此次前來是想請楊兄加入，助我一臂之力．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(58, "你的隊伍已滿，我無法加入．", "talkname58", 0);
        do return end;
::label1::
        Talk(58, "那有什麼問題，別的沒有，就是有”一臂”．", "talkname58", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(58);
do return end;
