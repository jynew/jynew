if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "不过经过了这么久，大部分的古墓也被盗得差不多了。你要找的话就尽量往偏僻一点的地方去找。如野外，破庙等。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
