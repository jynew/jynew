Talk(158, "小兄弟还想打听什么消息，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "讲讲重点。");
        Talk(158, "滨湖茅庐里面住着一位秦婆婆，听说能通鬼神之事，或许能召唤阴间使者也说不定。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 523, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
