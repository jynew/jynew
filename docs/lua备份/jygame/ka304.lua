Talk(0, "兄台，你在幹什麼，”那兒”不會痛嗎？", "talkname0", 1);
Talk(29, "我在練門神功，痛也得忍．要知練完之後，我就是武林中”最強最猛”之人．", "talkname29", 0);
Talk(0, "原來如此？還有，你家怎麼有張這麼大的床？", "talkname0", 1);
Talk(29, "床大好辦事啊．", "talkname29", 0);
Talk(0, "辦事，辦什麼事？", "talkname0", 1);
Talk(29, "我田伯光會辦什麼事，當然是神仙做的事．有這麼大的床，才可以同時搞五，六個妞，真爽．過些時後，我要再去抓些俏妞，到時兄弟想不想一起來爽啊？", "talkname29", 0);
ModifyEvent(-2, -2, -2, -2, 306, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == false then goto label0 end;
    Talk(0, "原來兄台有此雅好，與在下不謀而合．不妨咱倆一起結伴，在這江湖中好好的爽他一爽．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(29, "你的隊伍已滿，我無法加入．", "talkname29", 0);
        do return end;
::label1::
        Talk(29, "好！你這兄弟一點也不做作．不像其他假正經的傢伙，只會以名門正派自居．要知”做那檔事”是人心本能的慾望，何必刻意去掩飾呢？我喜歡你，我們就一起去遊戲人間．", "talkname29", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 17, 15, 2674); --隐藏田伯光
        jyx2_ReplaceSceneObject("", "NPC/田伯光", "");         
        LightScence();
        Join(29);
        AddEthics(-6);
        do return end;
::label0::
        if AskBattle() == true then goto label2 end;
            Talk(0, "兄台自己玩好了，小弟沒這份興緻．", "talkname0", 1);
            Talk(29, "少裝了，難不成你有”斷袖之癖”．", "talkname29", 0);
            ModifyEvent(-2, -2, -2, -2, 306, -1, -1, -2, -2, -2, -2, -2, -2);
            do return end;
::label2::
            Talk(0, "什麼！你這採花淫賊，儘做這些傷天害理的勾當，今天讓我撞見，算你倒霉．小俠我要為江湖除害．", "talkname0", 1);
            if TryBattle(53) == true then goto label3 end;
                Dead();
                do return end;
::label3::
                LightScence();
                Talk(0, "今天先饒你不死，希望你改過向善．否則日後再叫我撞見，就”去你的勢”，讓你去做太監．", "talkname0", 1);
                ModifyEvent(-2, -2, -2, -2, 305, -1, -1, -2, -2, -2, -2, -2, -2);
                AddRepute(1);
do return end;
