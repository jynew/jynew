Talk(29, "怎麼？兄台回心轉意了嗎？", "talkname29", 0);
if AskJoin () == false then goto label0 end;
    Talk(0, "原來兄台有此雅好，與在下不謀而合．不妨咱倆一起結伴，在這江湖中好好的爽他一爽．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(29, "你的隊伍已滿，我無法加入．", "talkname29", 0);
        do return end;
::label1::
        Talk(29, "好！你這兄弟一點也不做作．不像其他假正經的傢伙，只會以名門正派自居．要知”做那檔事”是人心本能的慾望，何必刻意去掩飾呢？我喜歡你，我們就一起去遊戲人間．", "talkname29", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 17, 15, 2674);
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
