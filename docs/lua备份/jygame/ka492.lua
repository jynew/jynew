Talk(0, "段兄，你在這過的還好吧？", "talkname0", 1);
if InTeam(76) == true then goto label0 end;
    Talk(53, "能天天在這陪著神仙姊姊，要我做牛做馬我都願意．", "talkname53", 0);
    do return end;
::label0::
    Talk(53, "兄弟，讓我加入你吧，我想跟王姑娘在一起．", "talkname53", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "很抱歉，段兄．王姑娘的風采也挺令小弟著迷的．", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "段兄你真是個癡情種子，我們當然是一起走嘍！", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的隊伍已滿，我無法加入．", "talkname53", 0);
            do return end;
::label2::
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/duanyu", "");--段誉
            jyx2_ReplaceSceneObject("", "NPC/wangyuyan", "");--段誉
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(53);
do return end;
