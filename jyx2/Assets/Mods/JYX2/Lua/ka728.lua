if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "洞庭湖畔有座药王庄，里面住着一位很会用毒的奇人，叫做“毒手药王”。不过药王庄四周种有奇特的树，使人难以进入。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
