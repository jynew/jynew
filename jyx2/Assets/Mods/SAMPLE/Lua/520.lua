Talk(158, "小兄弟还想打听什么消息，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "讲讲重点。");
        Talk(158, "将军府是中原边塞之城，由于当朝对北方外族抱有主和思想，只派驻了王远将军带少量部队镇守，将军府是王远的栖息之所。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 521, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
