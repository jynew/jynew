if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "在你参加一场战斗后，参与该场战斗而未昏迷之人，皆可平分该场战斗之经验。另外还有一点，就是击毙敌人的一瞬间，出手之人可获得额外的经验。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
