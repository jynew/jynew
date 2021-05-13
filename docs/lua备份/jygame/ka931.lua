if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    Talk(0, "為了走更遠的路，適當的休息也是必須的．我就好好的睡一覺吧！", "talkname0", 1);
    DarkScence();
    Rest();
    SetRoleFace(3);
    LightScence();
    Talk(0, "一覺起來，精神十足．走吧，繼續冒險去了！", "talkname0", 1);
do return end;
