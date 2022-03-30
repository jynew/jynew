Talk(7, "没事就快走吧，别在此逗留。", "talkname7", 0);
Talk(0, "在下有个请求，请何掌门能放了那位可怜的大夫。", "talkname0", 1);
Talk(7, "这个庸医，还妄称是陕、甘一带最有名的大夫，连是什么病都说不出来。若是他医不好五姑的病，我就一刀把他砍了，免得留在世上招摇撞骗，荼害世人。", "talkname7", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "说的也是，在下告辞了。", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, 139, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "可是他实在是已尽了力，况且那女人长的这么丑，不要也罢。", "talkname0", 1);
    Talk(7, "你说什么！", "talkname7", 0);
    Talk(0, "糟了，说溜了嘴。", "talkname0", 1);
    if TryBattle(18) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(7, "…………", "talkname7", 0);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/yisheng", "");--医生逃跑
        ModifyEvent(-2, -2, -2, -2, 165, -1, -1, -2, -2, -2, -2, -2, -2);
        AddEthics(2);
        AddRepute(3);
do return end;
