if UseItem(179) == true then goto label0 end;
    do return end;
::label0::
    Talk(32, "这……这是真迹！真是……真是唐朝……唐朝张旭的“率意帖”……假不了，假不了的！", "talkname32", 0);
    Talk(0, "三庄主果然是行家。", "talkname0", 1);
    Talk(32, "少侠，可否再借老夫一看？", "talkname32", 0);
    Talk(0, "秃老头，要看可以，先打赢我再说。", "talkname0", 1);
    Talk(32, "说什么？我最痛恨人家叫我秃子，你这小子太不知死活了。", "talkname32", 0);
    Talk(0, "秃头、秃头，下雨不愁，人家有伞，我有秃头。", "talkname0", 1);
    Talk(32, "好小子，我瞧你是活得不耐烦了，看看老夫怎么收拾你。", "talkname32", 0);
    if TryBattle(44) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(32, "小伙子，果然有两下子，可是那“率意帖”我是要定了。", "talkname32", 0);
        Talk(0, "三庄主，别自不量力了。我看这梅庄之中也没什么能手了，真是害我白走一趟。", "talkname0", 1);
        Talk(32, "臭小子！别这么嚣张！", "talkname32", 0);
        Talk(0, "这样好了，贵庄中只要有人能够胜得了我，连同先前那幅“溪山行旅图”和这“率意帖”一并送上。", "talkname0", 1);
        Talk(32, "小子，此话当真？", "talkname32", 0);
        Talk(0, "小爷我从来不说假话。", "talkname0", 1);
        Talk(32, "好！四弟，咱们去求二哥帮忙。", "talkname32", 0);
        DarkScence();
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 7, -2, -2, -1, -1, -1, 2908, 2908, 2908, -2, -2, -2);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 34, 0);
        jyx2_ReplaceSceneObject("", "Dynamic/Door_028", "");--黑白子开门
        ModifyEvent(-2, 10, 1, 1, 254, -1, -1, 6054, 6054, 6054, -2, -2, -2);
        ModifyEvent(-2, 11, 1, 1, 254, -1, -1, 6050, 6050, 6050, -2, -2, -2);
		jyx2_SwitchRoleAnimation("NPC/tubiweng", "Assets/BuildSource/AnimationControllers/StandController.controller");
		jyx2_FixMapObject("梅庄求助黑白子",1);
        LightScence();
        AddRepute(2);
do return end;
