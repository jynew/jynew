if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "通常在客栈或是你家中休息时都可恢复体力，生命，及内力。除非你是受伤不轻或是有中毒状况。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
