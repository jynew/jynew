Talk(29, "怎么，还想杀我吗？还是想跟我学几招对付女人呀？", "talkname29", 0);
if AskBattle() == false then goto label0 end;
    Talk(0, "你这采花淫贼，死到临头还不觉悟。你受死吧！", "talkname0", 1);
    if TryBattle(53) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/田伯光", "");
        ModifyEvent(30, 0, -2, -2, 303, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本303 场景30-0
        SetScenceMap(-2, 1, 17, 15, 2674);--田伯光死掉
        LightScence();
        AddRepute(4);
        do return end;
::label0::
        if AskJoin () == false then goto label2 end;
            Talk(0, "这可是你说的，我们就一起走吧，到时可得传授小弟几招。", "talkname0", 1);
            if TeamIsFull() == false then goto label3 end;
                Talk(29, "你的队伍已满，我无法加入。", "talkname29", 0);
                do return end;
::label3::
                DarkScence();
                ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 田伯光加入队伍 场景59-0
				jyx2_ReplaceSceneObject("", "NPC/田伯光", "");
                SetScenceMap(-2, 1, 17, 15, 2674);
                LightScence();
                Join(29);
                AddEthics(-6);
                do return end;
::label2::
                Talk(0, "你们俩的事，我不想管。", "talkname0", 1);
do return end;
