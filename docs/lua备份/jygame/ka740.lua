if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "嵩山大會何時召開？或許等你拜訪完五嶽劍派其它四派掌門後就召開了．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
