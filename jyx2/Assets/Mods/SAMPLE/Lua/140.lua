Talk(0, "糟糕！有野狼！");
if TryBattle(140) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "糟糕！有野狼！");
    if TryBattle(141) == true then goto label1 end;
        Dead();
::label1::
        LightScence();
        Talk(0, "咦？这群狼似乎在围着一匹死去的野狼，没想到狼这么凶残的动物也还是这么重感情，再想到我自己，还真是倍感凄凉啊。。。");
        if TryBattle(142) == true then goto label2 end;
            Dead();
        do return end;
::label2::
            LightScence();
            Talk(0, "这匹死去的野狼肚子全都腐烂了，咦！这是什么？一个断裂的小针头！是十字形状！");
            AddItem(126, 1);
            AddItem(126, 1);
            ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
