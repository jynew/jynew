if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "侠客岛上的武功秘密几十年来无人解得出来，原因就是读过书的人太钻牛角尖了。试试看带没读过书的石破天去碰碰运气好了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
