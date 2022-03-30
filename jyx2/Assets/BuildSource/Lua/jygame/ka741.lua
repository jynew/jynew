if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "六大派何时攻上光明顶？或许等你进入明教地道，发现一些秘密后，六大派的人正好也攻上去了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
