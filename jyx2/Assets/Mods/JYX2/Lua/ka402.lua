Talk(63, "这位公子，不知来小女子程英家中所为何事？", "talkname63", 0);
Talk(0, "在下浪迹天涯寻找一些书，途经姑娘家门外，想说进来碰碰运气。", "talkname0", 1);
Talk(63, "不知公子找些什么东西？", "talkname63", 0);
Talk(0, "不瞒姑娘，在下找寻的是江湖中传说的“十四天书”。", "talkname0", 1);
Talk(63, "“十四天书”？我也曾听人提起过，听说是很久以前的一位前辈所遗留下来的。不过我倒是不知这些书的下落。", "talkname63", 0);
Talk(0, "是啊，知道这些书下落的人也不多。姑娘家中就只一个人吗？", "talkname0", 1);
Talk(63, "自从和家师黄岛主分开后，我就一个人在此定居。", "talkname63", 0);
Talk(0, "原来姑娘是东邪黄药师的高徒，那武功一定不错了。", "talkname0", 1);
Talk(63, "哪里，我资质太差，没能学到师父武功的精髓，只学会一些奇门五行之术。", "talkname63", 0);
Talk(0, "原来姑娘还懂奇门五行之术，这对闯荡江湖很有用呢。", "talkname0", 1);
Talk(63, "是有点用处，有些平常人看不出的布局，我略懂一些。", "talkname63", 0);
ModifyEvent(-2, -2, -2, -2, 403, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "打扰姑娘多时，在下告退。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那程姑娘可否与在下一游，帮忙在下寻找那十四天书？", "talkname0", 1);
    if JudgeEthics(0, 65, 100) == true then goto label1 end;
        Talk(63, "我看公子脸上泛有戾气，公子还是多做些善事才是。", "talkname63", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(63, "你的队伍已满，我无法加入。", "talkname63", 0);
            do return end;
::label2::
            Talk(63, "嗯！好吧。反正我一人在此也是无聊，就随公子一游吧。", "talkname63", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/程英", "");--程英加入队伍
            LightScence();
            Join(63);
            AddEthics(2);
do return end;
