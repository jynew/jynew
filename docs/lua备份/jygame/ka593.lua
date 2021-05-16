if InTeam(76) == false then goto label0 end;
    Talk(53, "大哥，讓我也加入你，好不好？", "talkname53", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "沒關係，我還應付得來．暫且不勞你費心．", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "我就知道你想跟著王姑娘，兄弟我當然不會拆散你們．", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的隊伍已滿，我無法加入．", "talkname53", 0);
            do return end;
::label2::
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(53);
            do return end;
::label0::
            Talk(0, "兄弟，你還真會享福．躲在洞中過著清幽的生活．那像我，還得在外東奔西走的．", "talkname0", 1);
            Talk(53, "大哥，近來一切可好吧？有沒有什麼我可以效勞的．", "talkname53", 0);
            if AskJoin () == true then goto label3 end;
                Talk(0, "沒什麼問題，我還應付得來．", "talkname0", 1);
                do return end;
::label3::
                Talk(0, "不瞞兄弟，此次我就是特地來找兄弟幫忙的．只是怕擾了兄弟的清靜．", "talkname0", 1);
                if TeamIsFull() == false then goto label4 end;
                    Talk(53, "你的隊伍已滿，我無法加入．", "talkname53", 0);
                    do return end;
::label4::
                    Talk(53, "那的話．兄弟能有今天，還不是靠大哥幫忙的嗎？今日大哥既然有難，兄弟我當然義不容辭的幫你了．", "talkname53", 0);
                    DarkScence();
                    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                    LightScence();
                    Join(53);
                    AddEthics(2);
do return end;
