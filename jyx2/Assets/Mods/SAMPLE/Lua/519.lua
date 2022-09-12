Talk(158, "小兄弟还想打听什么消息，只需要100两即可获得一条独家消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "讲讲重点。");
        Talk(158, "钟鸣阁位于地图西侧，渡城四大门派中近年来新崛起的一方，年轻的阁主萨擎苍和死去的莫桥山庄掌门莫穿林号称渡城双骏，是渡城最有潜力的两位青年，也是武林盟主的有力竞争者。");
        AddItemWithoutHint(174, -100);
        ModifyEvent(-2, -2, -2, -2, 520, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
