if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "梅莊三莊主禿筆翁鍾情於書法，如果你能找到名家的書法帖子，或許．．．．．．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
