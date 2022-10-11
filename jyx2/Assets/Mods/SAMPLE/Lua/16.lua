Talk(10, "又是你，还是想尝尝玉石榴酒吗？");
if AskBattle() == true then goto label0 end;
    Talk(0, "我还是不喝了吧……");
    ModifyEvent(-2, -2, -2, -2, 16, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "那就来比试比试吧！");
    if TryBattle(10) == false then goto label1 end;
        Talk(10, "小子，你还有两下子，我何某说到做到，这瓶子酒你拿去，地地道道的塞外玉石榴。");
        ModifyEvent(-2, -2, -2, -2, 116, -1, -1, -2, -2, -2, -2, -2, -2);
        AddItem(10, 1);
        do return end;
::label1::
        Talk(10, "看来你还没有口福，哈哈哈哈哈。");
        ModifyEvent(-2, -2, -2, -2, 16, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
