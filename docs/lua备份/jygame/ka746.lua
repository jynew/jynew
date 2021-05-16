if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "要拿到”笑傲江湖”一書，你必需先拿到梅莊四友所喜歡的東西，拿給這四人．然後在梅莊地牢中得到”黑木令牌”以便進入黑木崖，因為書是在黑木崖上的．不過梅莊四友所喜歡的東西中，有一樣是在嵩山派內，所以你必需參加嵩山大會．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
