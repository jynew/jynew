if UseItem(126) == true then goto label0 end;
    do return end;
::label0::
    AddItem(126, -1);
    PlayAnimation(3, 5722, 5748);
    ModifyEvent(-2, -2, -2, -2, 237, 242, -1, 5722, 5748, 5722, -2, -2, -2);
    Talk(35, "嗯！好一罈梨花酒！曾聞白樂天杭州喜望詩云：”紅袖織綾誇柿葉，  青旗沽酒趁梨花”．這罈梨花酒酒味是極好，只可惜少了股芳冽之氣，若能以翡翠杯盛之而飲，那更是醇美無比．", "talkname35", 0);
do return end;
