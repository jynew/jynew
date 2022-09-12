Talk(158, "小兄弟还想打听什么消息，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "讲讲重点。");
        Talk(158, "罂粟谷是渡城第五门派，但因为擅长用毒，一直被将军府视为邪教，入不了正派之流。教主佟雯是渡城用毒第一人。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
