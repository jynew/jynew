Talk(0, "小和尚，借个位……", "talkname0", 1);
Talk(49, "佛观一钵水，八万四千虫，若不持此咒，如食众生肉。唵缚悉波罗摩尼莎诃", "talkname49", 0);
Talk(0, "小师父，你叽哩咕噜念什么咒啊？", "talkname0", 1);
Talk(49, "小僧念的是饮水咒。佛说每一碗水中，有八万四千条小虫，出家人戒杀，因此要念了饮水咒，这才喝得……", "talkname49", 0);
Talk(0, "这水干净得很，一条虫子也没有，小师父真会说笑。", "talkname0", 1);
Talk(49, "施主有所不知。我辈凡夫看来，水中自然无虫，但我佛以天眼看水，却看到水中小虫成千上万。", "talkname49", 0);
Talk(0, "那你念了饮水咒之后，将八万四千条小虫喝入肚中，那些小虫便不死了？", "talkname0", 1);
Talk(49, "这……这个……师父倒没教过。多半小虫便不死了。", "talkname49", 0);
Talk(0, "对了，小和尚怎么一个人在此，是要准备前往西方取经的吗？", "talkname0", 1);
Talk(49, "不是，我是和师父他们走丢了，正想办法该如何回少林寺去。", "talkname49", 0);
Talk(0, "哦，原来是少林寺的高僧，武功定是很高喽！", "talkname0", 1);
Talk(49, "哪里，小僧武功低微，在寺中是打杂的。", "talkname49", 0);
ModifyEvent(-2, -2, -2, -2, 499, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "小和尚，那改天我们少林寺再见。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "既然如此，小和尚要不要和我一起走，我知道回少林的路。", "talkname0", 1);
    if JudgeEthics(0, 75, 100) == true then goto label1 end;
        Talk(49, "不用了，小僧自己找路回少林即可。倒是施主眼神中充满戾气，恐入魔道，望施主能即时回头。", "talkname49", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(49, "你的队伍已满，我无法加入。", "talkname49", 0);
            do return end;
::label2::
            Talk(49, "好啊。", "talkname49", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/xuzhu", "");--xuzhu加入队伍
            ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(49);
            AddEthics(3);
do return end;
