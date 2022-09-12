Talk(158, "小兄弟，忘了告诉你了，只需要500两即可获得所有消息！");
if ShowYesOrNoSelectPanel("是否购买消息？") == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(500) == true then goto label1 end;
        Talk(158, "走，走，走，没钱就不要再待在这！");
        do return end;
::label1::
        Talk(0, "你，你不早说！");
        Talk(158, "鸽子楼表面上是楼主徐谦收留了一帮孤儿帮他养鸽子，其实是通过鸽子来传递情报，实际上是渡城最大的情报中心，如果你有疑点可以去鸽子楼找找。");
        AddItemWithoutHint(174, -500);
        ModifyEvent(-2, -2, -2, -2, 524, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
