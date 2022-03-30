Talk(29, "怎么？兄台回心转意了吗？", "talkname29", 0);
if AskJoin () == false then goto label0 end;
    Talk(0, "原来兄台有此雅好，与在下不谋而合。不妨咱俩一起结伴，在这江湖中好好的爽他一爽。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(29, "你的队伍已满，我无法加入。", "talkname29", 0);
        do return end;
::label1::
        Talk(29, "好！你这兄弟一点也不做作。不像其他假正经的家伙，只会以名门正派自居。要知“做那档事”是人心本能的欲望，何必刻意去掩饰呢？我喜欢你，我们就一起去游戏人间。", "talkname29", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/田伯光", "");  
        SetScenceMap(-2, 1, 17, 15, 2674);
        LightScence();
        Join(29);
        AddEthics(-6);
        do return end;
::label0::
        if AskBattle() == true then goto label2 end;
            Talk(0, "兄台自己玩好了，小弟没这份兴致。", "talkname0", 1);
            Talk(29, "少装了，难不成你有“断袖之癖”。", "talkname29", 0);
            ModifyEvent(-2, -2, -2, -2, 306, -1, -1, -2, -2, -2, -2, -2, -2);
            do return end;
::label2::
            Talk(0, "什么！你这采花淫贼，尽做这些伤天害理的勾当，今天让我撞见，算你倒霉。小侠我要为江湖除害。", "talkname0", 1);
            if TryBattle(53) == true then goto label3 end;
                Dead();
                do return end;
::label3::
                LightScence();
                Talk(0, "今天先饶你不死，希望你改过向善。否则日后再叫我撞见，就“去你的势”，让你去做太监。", "talkname0", 1);
                ModifyEvent(-2, -2, -2, -2, 305, -1, -1, -2, -2, -2, -2, -2, -2);
                AddRepute(1);
do return end;
