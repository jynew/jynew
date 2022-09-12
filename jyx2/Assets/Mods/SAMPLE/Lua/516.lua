Talk(158, "在渡城就没有我不知道的事，我自号“江湖百晓生”，可为你解惑，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "且听你说说。");
        Talk(158, "在这渡城有四大门派，莫桥山庄位于地图东侧，渡城四大门派中实力最强的帮派根据地，掌门人莫穿林近日身亡，可能和这怡麟楼有关。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 517, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
