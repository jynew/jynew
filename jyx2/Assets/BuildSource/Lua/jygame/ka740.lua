if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "嵩山大会何时召开？或许等你拜访完五岳剑派其它四派掌门后就召开了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
