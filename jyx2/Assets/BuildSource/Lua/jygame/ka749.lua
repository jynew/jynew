if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "“圣堂”在哪里？从前是有个地方叫圣堂的，但不知是不是改了名字，现在没有一个地方是叫做“圣堂”。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
