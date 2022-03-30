if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "武当派的张真人，对于提携后进一向不遗余力，你若有空，可常向他讨教，肯定获益良多。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
