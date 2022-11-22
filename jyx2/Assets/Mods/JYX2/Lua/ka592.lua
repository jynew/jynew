if UseItem(174) == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(106, "兄弟，１００就１００，我们可是不二价的。", "talkname106", 0);
        do return end;
::label1::
        AddItemWithoutHint(174, -100);
        Talk(106, "好，从这里一直往西南走，大概在澜沧江的源头就可以看到了。座标大约是在……（１６８，４２６）附近。祝你玩的愉快。", "talkname106", 0);
        Talk(0, "就这么简单？", "talkname0", 1);
        Talk(106, "招牌上写的清清楚楚，自助旅游，当然是你自己去，难不成还我带你去呀。", "talkname106", 0);
        Talk(0, "这样就要１００两，太离谱了吧。", "talkname0", 1);
        Talk(106, "你再吵，再吵我就将你的行为报告给全国的小二哥联谊会，看你以后还有没有小道消息可问。", "talkname106", 0);
        Talk(0, "这可不得了，万万不可．在下只是发发牢骚罢了，小二哥可别当真了。", "talkname0", 1);
        ModifyEvent(-2, 5, -2, -2, 571, -2, -2, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, -2, -2, -2, 481, -2, -2, -2, -2, -2, -2, -2, -2);
do return end;
