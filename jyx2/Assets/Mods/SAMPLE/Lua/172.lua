Talk(1170, "哈哈，少侠，来张饼吗？十两银子一张。");
if ShowYesOrNoSelectPanel("是否购买罗大饼？") == true then goto label0 end;
    Talk(0, "今天先不吃了。");
    do return end;
::label0::
    if JudgeMoney(10) == true then goto label1 end;
        Talk(1170, "走，走，走，没钱就不要妨碍我做生意！");
        do return end;
::label1::
        Talk(0, "来一张吧。");
        AddItemWithoutHint(174, -10);
        AddItem(171, 1);
do return end;
