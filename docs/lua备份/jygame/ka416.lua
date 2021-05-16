if InTeam(63) == true then goto label0 end;
    do return end;
::label0::
    Talk(63, "這黑水潭似乎是有人特別佈局過，但卻也難不倒我．", "talkname63", 1);
    DarkScence();
    ChangeScencePic(-2, 0, 994, 990);
    jyx2_ReplaceSceneObject("", "Bake/Static/hidenBridges", "1"); 
    jyx2_ReplaceSceneObject("", "NPC/yinggu", "1"); 
    LightScence();
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 5, -2, -2, -1, -1, 434, -1, -1, -1, -2, -2, -2);--by fanyu 启动脚本434 场景21-编号5
do return end;
