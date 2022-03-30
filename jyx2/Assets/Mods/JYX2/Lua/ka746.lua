if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "要拿到《笑傲江湖》一书，你必需先拿到梅庄四友所喜欢的东西，拿给这四人。然后在梅庄地牢中得到“黑木令牌”以便进入黑木崖，因为书是在黑木崖上的。不过梅庄四友所喜欢的东西中，有一样是在嵩山派内，所以你必需参加嵩山大会。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
