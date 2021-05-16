Talk(2, "小子，還有事嗎？", "talkname2", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒事，沒事．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "靈姑娘用毒，醫術都極為高明，有你陪伴闖盪江湖，旅程將會十分安穩，不知姑娘是否肯隨我們四處走走呢？", "talkname0", 1);
    if InTeam(1) == false then goto label1 end;
        Talk(1, "是啊，姑娘一個人住這裡，悶也悶慌了，就隨我們到處走走吧．", "talkname1", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(2, "你的隊伍已滿，我無法加入．", "talkname2", 0);
            do return end;
::label2::
            Talk(2, "看在胡公子的面子上，我就陪你們到處玩一玩．", "talkname2", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
            LightScence();
            Join(2);
            AddEthics(1);
            do return end;
::label1::
            Talk(2, "你臭美啊！跟你在一起一定很無聊．", "talkname2", 0);
do return end;
