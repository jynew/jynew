Talk(0, "田伯光，平一指叫我来杀你。", "talkname0", 1);
Talk(29, "这老小子，还想不通，他女儿被我玩玩应该高兴才对呀。怎么，你想杀我吗？还是要跟我一起玩女人，我教你几招高招，包你整得女人死去活来。", "talkname29", 0);
if AskBattle() == false then goto label0 end;
    Talk(0, "你这采花淫贼，死到临头还不觉悟。你受死吧！", "talkname0", 1);
    if TryBattle(53) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除田伯光 场景59-0
		jyx2_ReplaceSceneObject("", "NPC/田伯光", "");
        ModifyEvent(30, 0, -2, -2, 303, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本303 场景30-0
        SetScenceMap(-2, 1, 17, 15, 2674);
        LightScence();
        AddRepute(4);
        do return end;
::label0::
        ModifyEvent(-2, -2, -2, -2, 308, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本308 场景59-0
        if AskJoin () == false then goto label2 end;
            Talk(0, "这可是你说的，我们就一起走吧，到时可得传授小弟几招。", "talkname0", 1);
            if TeamIsFull() == false then goto label3 end;
                Talk(29, "你的队伍已满，我无法加入。", "talkname29", 0);
                do return end;
::label3::
                DarkScence();
                ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 田伯光加入队伍 场景59-0
                jyx2_ReplaceSceneObject("", "NPC/田伯光", "");--田伯光加入队伍
                SetScenceMap(-2, 1, 17, 15, 2674);--by fanyu 改变贴图，移除田伯光 场景59-1
                LightScence();
                Join(29);
                AddEthics(-6);
                do return end;
::label2::
                Talk(0, "你们俩的事，我不想管。", "talkname0", 1);
do return end;
