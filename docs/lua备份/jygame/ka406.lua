Talk(64, "來來來，來跟老頑童來玩兩招．", "talkname64", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "前輩別開玩笑了，晚輩怎是你的對手．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那晚輩就獻醜了．", "talkname0", 1);
    if TryBattle(67) == false then goto label1 end;
        LightScence();
        Talk(64, "小兄弟，你這是什麼功夫，教教我好不好．", "talkname64", 0);
        Talk(0, "那裡，前輩承讓了．晚輩功夫還差得遠．", "talkname0", 1);
        Talk(64, "這樣好了，我跟你磕八個響頭，拜你為師，你總肯教我了吧．", "talkname64", 0);
        Talk(0, "前輩別開玩笑了，晚輩擔當不起．", "talkname0", 1);
        do return end;
::label1::
        LightScence();
        Talk(64, "唉，你功夫還差的遠了，再去練練．", "talkname64", 0);
do return end;
