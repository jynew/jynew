if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "”聖堂”在那裡？從前是有個地方叫聖堂的，但不知是不是改了名字，現在沒有一個地方是叫做聖”堂”．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
