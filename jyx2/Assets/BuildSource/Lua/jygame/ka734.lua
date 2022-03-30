if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "五岳剑派近来好不兴旺。其盟主左冷禅极有野心，一心想称霸武林。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
