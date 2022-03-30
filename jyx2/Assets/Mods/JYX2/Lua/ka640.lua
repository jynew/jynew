if JudgeAttack(0, 75, 1000) == true then goto label0 end;
    PlayAnimation(-1, 7864, 7912);
    jyx2_PlayTimeline("[Timeline]ka460_金蛇山洞_拔金蛇剑_失败", 1, true);
    Talk(0, "哇塞！我使尽了吃奶的力气还拔不出来，难道要请亚瑟王来才行吗？", "talkname0", 1);
    jyx2_StopTimeline("[Timeline]ka460_金蛇山洞_拔金蛇剑_失败");
    do return end;
::label0::
    ModifyEvent(-2, -2, 1, 1, -1, -1, -1, 4736, 4736, 4736, -2, -2, -2);
    PlayAnimation(-1, 7864, 7912);
    PlayAnimation(-1, 7864, 7964);
    jyx2_PlayTimeline("[Timeline]ka460_金蛇山洞_拔金蛇剑", 1, true);
    Talk(0, "终于让我给拔出来了！", "talkname0", 1);
    jyx2_ReplaceSceneObject("", "Bake/Static/jinshejian", ""); 
    jyx2_StopTimeline("[Timeline]ka460_金蛇山洞_拔金蛇剑");
    AddItem(110, 1);
do return end;
