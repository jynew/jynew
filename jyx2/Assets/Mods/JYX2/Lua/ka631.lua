Talk(0, "金轮法王，快将可兰经交出来。", "talkname0", 1);
Talk(62, "看你有没有这个本事拿。", "talkname62", 0);
if TryBattle(100) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 3, -2, -2, 632, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本632。场景16-编号3
    ModifyEvent(-2, 5, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号5
    ModifyEvent(-2, 6, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号6
    ModifyEvent(-2, 7, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号7
    ModifyEvent(-2, 8, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号8
    ModifyEvent(-2, 9, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号9
    ModifyEvent(-2, 12, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号12
    ModifyEvent(-2, 13, -2, -2, 634, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本634。场景16-编号13
    LightScence();
    Talk(0, "老秃贼，遇到我算你倒霉。", "talkname0", 1);
    Talk(62, "哼！", "talkname62", 0);
    AddItem(159, 1);
    AddRepute(8);
do return end;
