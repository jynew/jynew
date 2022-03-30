if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "回族人近日大举出动，听说与金轮寺有关。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
