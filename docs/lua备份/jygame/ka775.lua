if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "在你參加一場戰鬥後，參與該場戰鬥而未昏迷之人，皆可平分該場戰鬥之經驗．另外還有一點，就是擊斃敵人的一瞬間，出手之人可獲得額外的經驗．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
