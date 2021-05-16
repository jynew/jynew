Talk(2, "公子別來無恙？", "talkname2", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "沒有靈姑娘這個大毒梟在，一路上都挺麻煩的，是否可請靈姑娘再出馬呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(2, "你的隊伍已滿，我無法加入．", "talkname2", 0);
        do return end;
::label1::
        Talk(2, "那有什麼問題．", "talkname2", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(2);
do return end;
