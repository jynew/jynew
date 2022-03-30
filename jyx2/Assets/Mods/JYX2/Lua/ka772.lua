if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "大轮寺关着一个苦命之人，去杀了他吧。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
