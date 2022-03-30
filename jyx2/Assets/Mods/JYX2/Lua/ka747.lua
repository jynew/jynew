if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "《天龙八部》一书是在乔峰手上。我希望你是用正当手段得到的。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
