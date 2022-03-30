if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "你若装备了特殊武器，则在使用特定功夫时会产生额外的攻击力。血刀搭配血刀大法，冷月宝刀搭配胡家刀法，霹雳狂刀搭配霹雳刀法，玄铁剑搭配玄铁剑法，君子剑或淑女剑搭配玉女素心剑法，金蛇剑搭配金蛇剑法。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
