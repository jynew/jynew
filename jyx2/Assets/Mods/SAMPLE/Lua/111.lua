Talk(1110, "小哥，要来点雪梨吗，正宗赵州贡品，鲜美多汁，尝一颗润肺提神。");
if ShowYesOrNoSelectPanel("是否购买雪梨？") == true then goto label0 end;
    Talk(0, "不用了。");
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
