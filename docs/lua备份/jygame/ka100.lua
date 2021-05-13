Talk(15, "你又想做什麼？", "talkname15", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "晚輩斗膽向前輩討教討教．", "talkname0", 1);
    Talk(15, "好，我們來玩玩．", "talkname15", 0);
    if TryBattle(132) == false then goto label1 end;
        LightScence();
        Talk(15, "小子，過些時候，我金花婆婆再向你討教．", "talkname15", 0);
        Talk(0, "我會等您的．", "talkname0", 1);
        do return end;
::label1::
        LightScence();
        Talk(15, "看你資質挺好的，老婆婆我不想殺你，你走吧．", "talkname15", 0);
do return end;
