if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "不起眼的门徒有时也会知道些重要消息，所以，有空可以多找他们聊聊。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
