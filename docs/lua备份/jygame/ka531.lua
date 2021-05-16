if UseItem(183) == true then goto label0 end;
    do return end;
::label0::
    AddEthics(-14);
    AddItem(183, -1);
    Talk(0, "喬峰，你看這是什麼？", "talkname0", 1);
    Talk(50, "是什麼？", "talkname50", 0);
    Talk(0, "你看了便知道．", "talkname0", 1);
    DarkScence();
    PlayMusic(9);
    LightScence();
    Talk(50, "這．．這是真的嗎？", "talkname50", 0);
    Talk(0, "少林方丈親筆寫的，會是假的嗎！", "talkname0", 1);
    Talk(50, "我．．我．．我不是漢人．．．．我是契丹人．．．．", "talkname50", 0);
    Talk(0, "喬峰，你是契丹人，非我漢族人士，怎可擔任這丐幫幫主一職，保管”天龍八部”一書呢！", "talkname0", 1);
    Talk(50, "你要我怎麼做！", "talkname50", 0);
    Talk(0, "辭去丐幫幫主之位，交出”天龍八部”一書．", "talkname0", 1);
    Talk(50, "好！我今天就辭去這丐幫幫主之位，”天龍八部”一書你拿去吧．", "talkname50", 0);
    jyx2_ReplaceSceneObject("", "NPC/qiaofeng", "");--战斗结束，乔峰离开
    DarkScence();
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，乔峰离开 场景51-14
    LightScence();
    GetItem(147, 1);
    Talk(0, "得來真是全不費功夫．", "talkname0", 1);
    Talk(93, "閣下來此，揭發喬峰的真實身份，讓我丐幫不致誤奉一契丹人為幫主，很是感激．", "talkname93", 0);
    Talk(0, "沒什麼，這是中原武林的大事，小弟應該做的．", "talkname0", 1);
    Talk(93, "但是，拜你所賜，我丐幫也將因此事貽笑武林．你拿的”天龍八部”是丐幫鎮幫之寶，還請閣下留下．", "talkname93", 0);
    Talk(0, "怎麼可以，我好不容易才拿到手的．", "talkname0", 1);
    Talk(93, "那只好得罪了．", "talkname93", 0);
    if TryBattle(84) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        ModifyEvent(-2, 2, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-2
        ModifyEvent(-2, 3, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-3
        ModifyEvent(-2, 4, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-4
        ModifyEvent(-2, 6, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-6
        ModifyEvent(-2, 7, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-7
        ModifyEvent(-2, 8, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-8
        ModifyEvent(-2, 9, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-9
        ModifyEvent(-2, 10, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-10
        ModifyEvent(-2, 11, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-11
        ModifyEvent(-2, 12, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-12
        ModifyEvent(-2, 13, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-13
        ModifyEvent(-2, 15, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-15
        ModifyEvent(-2, 16, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-16
        ModifyEvent(-2, 17, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-17
        ModifyEvent(-2, 18, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-18
        ModifyEvent(-2, 19, -2, -2, 532, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本532 场景51-19
        AddRepute(5);
do return end;
