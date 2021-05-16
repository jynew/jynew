if UseItem(180) == true then goto label0 end;
    do return end;
::label0::
    Talk(31, "啊！這是北宋范寬的真跡”谿山行旅圖”，你．．．你是從何處得來的？", "talkname31", 0);
    Talk(0, "這個你不必管．我聽江湖上傳言，梅莊四莊主好酒，好畫，好劍，人稱三絕．那想必對我這幅畫定是”哈”死了！", "talkname0", 1);
    Talk(31, "你這小子，到底有什麼企圖？", "talkname31", 0);
    Talk(0, "”企圖”沒有，”行旅圖”倒是有一幅．", "talkname0", 1);
    Talk(31, "小子，少貧嘴，找死嘛？", "talkname31", 0);
    Talk(0, "就憑你？幫我搔癢還差不多．", "talkname0", 1);
    Talk(31, "啊！氣死我了！", "talkname31", 0);
    if TryBattle(43) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(31, "真是長江後浪推前浪，今日敗在你的手裡，我也沒話可說．", "talkname31", 0);
        Talk(0, "承讓了，四莊主．", "talkname0", 1);
        Talk(31, "小子，你等著，待我去請我三哥．", "talkname31", 0);
        DarkScence();
        ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 37, 42, 0);
        jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_027", "");--秃笔翁开门
        jyx2_ReplaceSceneObject("", "NPC/danqingsheng", "");--丹青生
        jyx2_ReplaceSceneObject("", "NPC/danqingsheng2", "1");--丹青生
        ModifyEvent(-2, 8, 1, 1, 251, -1, -1, 6048, 6048, 6048, -2, -2, -2);
        LightScence();
        AddRepute(2);
do return end;
