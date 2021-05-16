Talk(7, "沒事就快走吧，別在此逗留．", "talkname7", 0);
Talk(0, "在下有個請求，請何掌門能放了那位可憐的大夫．", "talkname0", 1);
Talk(7, "這個庸醫，還妄稱是陝，甘一帶最有名的大夫，連是什麼病都說不出來．若是他醫不好五姑的病，我就一刀把他砍了，免得留在世上招搖撞騙，荼害世人．", "talkname7", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "說的也是，在下告辭了．", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, 139, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "可是他實在是已盡了力，況且那女人長的這麼醜，不要也罷．", "talkname0", 1);
    Talk(7, "你說什麼！", "talkname7", 0);
    Talk(0, "糟了，說溜了嘴．", "talkname0", 1);
    if TryBattle(18) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(7, "．．．．．", "talkname7", 0);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/yisheng", "");--医生逃跑
        ModifyEvent(-2, -2, -2, -2, 165, -1, -1, -2, -2, -2, -2, -2, -2);
        AddEthics(2);
        AddRepute(3);
do return end;
