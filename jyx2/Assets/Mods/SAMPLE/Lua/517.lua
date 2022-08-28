Talk(158, "小兄弟还想打听什么消息，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再呆在这！");
        do return end;
::label1::
        Talk(0, "讲讲重点。");
        Talk(158, "万烛书苑位于地图南侧，渡城四大门派之一，掌门人刘灯剑以轻盈诡异的醉灯剑法长期稳固着个人和帮派在渡城中的江湖地位。二当家童岿然和掌门刘灯剑似乎貌合神离。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 518, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
