Talk(15, "你又想做什么？", "talkname15", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "晚辈斗胆向前辈讨教讨教。", "talkname0", 1);
    Talk(15, "好，咱们来玩玩。", "talkname15", 0);
    if TryBattle(14) == false then goto label1 end;
        ModifyEvent(-2, -2, -2, -2, 100, -1, -1, -2, -2, -2, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 17, 0); --开门
        jyx2_FixMapObject("灵蛇岛开门",1); 

        LightScence();
        Talk(15, "好小子，有你的。真是长江后浪推前浪。你是来救王难姑的吧，既然打输了你，老婆婆我就改天再寻他们的晦气。", "talkname15", 0);
        Talk(0, "＜什么救不救人的？我都搞糊涂了。＞", "talkname0", 1);
        AddRepute(3);
        do return end;
::label1::
        LightScence();
        Talk(15, "看你资质挺好的，老婆婆我不想杀你，你走吧。", "talkname15", 0);
do return end;
