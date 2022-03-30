Talk(64, "来来来，跟老顽童来玩两招。", "talkname64", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "前辈别开玩笑了，晚辈怎是你的对手。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那晚辈就献丑了。", "talkname0", 1);
    if TryBattle(135) == false then goto label1 end;
        LightScence();
        Talk(64, "小兄弟，你这是什么功夫，教教我好不好。", "talkname64", 0);
        Talk(0, "哪里，前辈承让了。晚辈功夫还差得远。", "talkname0", 1);
        Talk(64, "这样好了，我跟你磕八个响头，拜你为师，你总肯教我了吧。", "talkname64", 0);
        Talk(0, "前辈别开玩笑了，晚辈担当不起。", "talkname0", 1);
        do return end;
::label1::
        LightScence();
        Talk(64, "唉，你功夫还差的远了，再去练练。", "talkname64", 0);
do return end;
