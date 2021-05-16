Talk(0, "前輩是崆峒派掌門吧．晚輩雲遊江湖，聽聞崆峒派七傷拳神乎奇技，不知是否有榮幸見識見識．", "talkname0", 1);
Talk(8, "小兄弟，還算有點見識，知道我七傷拳的奧妙．要知我七傷拳中拳力剛中有柔，柔中有剛，七般拳勁各不相同，吞吐閃爍，變化百端，敵手委實難防難擋．．．小兄弟還是別見識的好．", "talkname8", 0);
if InTeam(9) == true then goto label0 end;
    if AskBattle() == true then goto label1 end;
        ModifyEvent(-2, -2, -2, -2, 127, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label1::
        Talk(0, "該不會是掌門您七傷拳練的火侯不夠，才不敢拿出來見人吧？", "talkname0", 1);
        Talk(8, "小子，你自尋死路．", "talkname8", 0);
        if TryBattle(16) == true then goto label2 end;
            Dead();
            do return end;
::label2::
            LightScence();
            Talk(0, "我說你火侯不夠還不相信，再多練個幾年吧．", "talkname0", 1);
            Talk(8, "哼！小兄弟留下個字號，他日我唐文亮再向你請教．", "talkname8", 0);
            Talk(0, "字號？好吧，記著，”軟世派河洛分舵”金庸堂堂主是也．", "talkname0", 1);
            Talk(8, "”軟世派河洛分舵”？沒聽過．", "talkname8", 0);
            ModifyEvent(-2, -2, -2, -2, 128, -1, -1, -2, -2, -2, -2, -2, -2);
            AddRepute(3);
            do return end;
::label0::
            Talk(9, "大哥，我聽我義父說過，這七傷拳若是內力修為不足之人練之，練之反而有害．我看這崆峒掌門顯然已受了內傷，七傷拳想必修練的也不怎麼樣，不看也罷．", "talkname9", 1);
            Talk(8, "不知這位兄弟的義父是誰，竟能對我崆峒派的絕技有高明意見．", "talkname8", 0);
            Talk(0, "他義父是金毛獅王謝．．．", "talkname0", 1);
            Talk(8, "魔教的謝遜在那裡？你是他的義子，先拿下再說．", "talkname8", 0);
            Talk(0, "完了，說溜了嘴．", "talkname0", 1);
            Talk(9, "大哥，打就打吧．", "talkname9", 1);
            if TryBattle(16) == true then goto label3 end;
                Dead();
                do return end;
::label3::
                LightScence();
                Talk(0, "賢弟，果真不怎麼樣．", "talkname0", 1);
                Talk(8, "哼！你們現在不殺我，到時我六大派聯手，非把你魔教剿滅不可．", "talkname8", 0);
                ModifyEvent(-2, -2, -2, -2, 128, -1, -1, -2, -2, -2, -2, -2, -2);
                AddRepute(3);
do return end;
