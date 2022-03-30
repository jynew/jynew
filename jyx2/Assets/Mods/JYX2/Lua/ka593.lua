if InTeam(76) == false then goto label0 end;
    Talk(53, "大哥，让我也加入你，好不好？", "talkname53", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "没关系，我还应付得来。暂且不劳你费心。", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "我就知道你想跟着王姑娘，兄弟我当然不会拆散你们。", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的队伍已满，我无法加入。", "talkname53", 0);
            do return end;
::label2::
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/段誉", "");--段誉加入队伍
            LightScence();
            Join(53);
            do return end;
::label0::
            Talk(0, "兄弟，你还真会享福。躲在洞中过着清幽的生活。哪像我，还得在外东奔西走的。", "talkname0", 1);
            Talk(53, "大哥，近来一切可好吧？有没有什么我可以效劳的。", "talkname53", 0);
            if AskJoin () == true then goto label3 end;
                Talk(0, "没什么问题，我还应付得来。", "talkname0", 1);
                do return end;
::label3::
                Talk(0, "不瞒兄弟，此次我就是特地来找兄弟帮忙的。只是怕扰了兄弟的清静。", "talkname0", 1);
                if TeamIsFull() == false then goto label4 end;
                    Talk(53, "你的队伍已满，我无法加入。", "talkname53", 0);
                    do return end;
::label4::
                    Talk(53, "哪的话。兄弟能有今天，还不是靠大哥帮忙的吗？今日大哥既然有难，兄弟我当然义不容辞的帮你了。", "talkname53", 0);
                    DarkScence();
                    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
					jyx2_ReplaceSceneObject("", "NPC/段誉", "");--段誉加入队伍
                    LightScence();
                    Join(53);
                    AddEthics(2);
do return end;
