Talk(63, "這位公子，不知來小女子程英家中所為何事？", "talkname63", 0);
Talk(0, "在下浪跡天涯尋找一些書，途經姑娘家門外，想說進來碰碰運氣．", "talkname0", 1);
Talk(63, "不知公子找些什麼東西？", "talkname63", 0);
Talk(0, "不瞞姑娘，在下找尋的是江湖中傳說的”十四天書”．", "talkname0", 1);
Talk(63, "”十四天書”？我也曾聽人提起過，聽說是很久以前的一位前輩所遺留下來的．不過我倒是不知這些書的下落．", "talkname63", 0);
Talk(0, "是啊，知道這些書下落的人也不多．姑娘家中就只一個人嗎？", "talkname0", 1);
Talk(63, "自從和家師黃島主分開後，我就一個人在此定居．", "talkname63", 0);
Talk(0, "原來姑娘是東邪黃藥師的高徒，那武功一定不錯了．", "talkname0", 1);
Talk(63, "那裡，我資質太差，沒能學到師父武功的精髓，只學會一些奇門五行之術．", "talkname63", 0);
Talk(0, "原來姑娘還懂奇門五行之術，這對闖盪江湖很有用呢．", "talkname0", 1);
Talk(63, "是有點用處，有些平常人看不出的佈局，我略懂一些．", "talkname63", 0);
ModifyEvent(-2, -2, -2, -2, 403, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "打擾姑娘多時，在下告退．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那程姑娘可否與在下一遊，幫忙在下尋找那十四天書？", "talkname0", 1);
    if JudgeEthics(0, 65, 100) == true then goto label1 end;
        Talk(63, "我看公子臉上泛有戾氣，公子還是多做些善事才是．", "talkname63", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(63, "你的隊伍已滿，我無法加入．", "talkname63", 0);
            do return end;
::label2::
            Talk(63, "嗯！好吧．反正我一人在此也是無聊，就隨公子一遊吧．", "talkname63", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/chengying", "");--程英加入队伍
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(63);
            AddEthics(2);
do return end;
