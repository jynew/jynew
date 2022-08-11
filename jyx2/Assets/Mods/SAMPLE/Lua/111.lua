Talk(1110, "小哥，要来点槟榔吗，正宗海南果，烟熏究脑壳，嚼一颗提神醒脑。");
if ShowYesOrNoSelectPanel("是否购买槟榔？") == true then goto label0 end;
    Talk(0, "不用了，长期过量咀嚼，有害身体健康。");
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(1110, "走，走，走，没钱就不要妨碍我做生意！");
        do return end;
::label1::
        Talk(0, "来几颗尝尝。");
        AddItemWithoutHint(174, -100);
        AddItem(110, 10);
do return end;
