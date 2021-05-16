Talk(0, "小和尚，借個位．．．", "talkname0", 1);
Talk(49, "佛觀一缽水，八萬四千蟲，若不持此咒，如食眾生肉．唵縛悉波羅摩尼莎訶", "talkname49", 0);
Talk(0, "小師父，你嘰哩咕嚕唸什麼咒啊？", "talkname0", 1);
Talk(49, "小僧念的是飲水咒．佛說每一碗水中，有八萬四千條小蟲，出家人戒殺，因此要念了飲水咒，這才喝得．．．．", "talkname49", 0);
Talk(0, "這水乾淨得很，一條蟲子也沒有，小師父真會說笑．", "talkname0", 1);
Talk(49, "施主有所不知．我輩凡夫看來，水中自然無蟲，但我佛以天眼看水，卻看到水中小蟲成千上萬．", "talkname49", 0);
Talk(0, "那你唸了飲水咒之後，將八萬四千條小蟲喝入肚中，那些小蟲便不死了？", "talkname0", 1);
Talk(49, "這．．．這個．．．師父倒沒教過．多半小蟲便不死了．", "talkname49", 0);
Talk(0, "對了，小和尚怎麼一個人在此，是要準備前往西方取經的嗎？", "talkname0", 1);
Talk(49, "不是，我是和師父他們走丟了，正想辦法該如何回少林寺去．", "talkname49", 0);
Talk(0, "哦，原來是少林寺的高僧，武功定是很高嘍！", "talkname0", 1);
Talk(49, "那裡，小僧武功低微，在寺中是打雜的．", "talkname49", 0);
ModifyEvent(-2, -2, -2, -2, 499, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "小和尚，那改天我們少林寺再見．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "既然如此，小和尚要不要和我一起走，我知道回少林的路．", "talkname0", 1);
    if JudgeEthics(0, 75, 100) == true then goto label1 end;
        Talk(49, "不用了，小僧自己找路回少林即可．倒是施主眼神中充滿戾氣，恐入魔道，望施主能即時回頭．", "talkname49", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(49, "你的隊伍已滿，我無法加入．", "talkname49", 0);
            do return end;
::label2::
            Talk(49, "好啊．", "talkname49", 0);
            DarkScence();
            ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(49);
            AddEthics(3);
do return end;
