Talk(29, "兄弟，一路上還爽吧？又搞了幾個女人呀？", "talkname29", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "田兄真愛開玩笑．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "沒有你同行，小弟一人怎麼玩得起來．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(29, "你的隊伍已滿，我無法加入．", "talkname29", 0);
        do return end;
::label1::
        Talk(29, "那就走吧．我一個人玩也沒什麼意思，團體的比較好玩．", "talkname29", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(29);
do return end;
