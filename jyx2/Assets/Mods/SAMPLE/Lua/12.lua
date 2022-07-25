Talk(1, "又是你，还是想尝尝玉石榴酒吗？");
if AskBattle() == true then goto label0 end;
    Talk(0, "我还是不喝了吧……");
    ModifyEvent(-2, -2, -2, -2, 12, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "那就来比试比试吧！");
    if TryBattle(0) == false then goto label1 end;
        LightScence();
        Talk(1, "小子，你还有两下子，我何某说到做到，这瓶子酒你拿去，地地道道的塞外玉石榴。");
        ModifyEvent(-2, -2, -2, -2, 13, -1, -1, -2, -2, -2, -2, -2, -2);
        AddItem(126, 1);
        do return end;
::label1::
        LightScence();
        Talk(1, "看来你还没有口福，哈哈哈哈哈。");
        ModifyEvent(-2, -2, -2, -2, 12, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
