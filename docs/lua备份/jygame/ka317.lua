if UseItem(125) == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "這位弟兄，我有要事稟告東方教主，麻煩借過一下．請看，這是”黑木令牌”．", "talkname0", 1);
    DarkScence();
    ModifyEvent(-2, 82, 1, 1, 318, -1, -1, 5890, 5890, 5890, 0, 54, 28);--by fanyu 启动脚本318，角色移到坐标处 场景26，编号82
    jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
    LightScence();
do return end;
