if InTeam(47) == true then goto label0 end;
    Talk(48, "你別在這惹阿紫姑娘生氣．", "talkname48", 0);
    do return end;
::label0::
    Talk(48, "阿紫姑娘，你別丟下我一個人．求求少俠讓我加入，好讓我跟在阿紫姑娘身旁服侍她．", "talkname48", 0);
    if AskJoin () == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "也好．", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(48, "你的隊伍已滿，我無法加入．", "talkname48", 0);
            do return end;
::label2::
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/youtanzhi", "");--游坦之加入
            ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(48);
            AddEthics(-2);
do return end;
