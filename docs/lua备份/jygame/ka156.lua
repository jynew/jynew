Talk(5, "小兄弟想要與老朽切磋武學的奧妙嗎？", "talkname5", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "還望前輩指導．", "talkname0", 1);
    if TryBattle(22) == false then goto label1 end;
        LightScence();
        Talk(5, "少俠武功已到如此境界，老朽也沒什麼好教你的了．", "talkname5", 0);
        ModifyEvent(-2, -2, -2, -2, 158, -1, -1, -2, -2, -2, -2, -2, -2);
        AddRepute(20);
        do return end;
::label1::
        LightScence();
        if JudgeEthics(0, 65, 100) == true then goto label2 end;
            Talk(5, "小兄弟，看來你還需再下一番努力才是．", "talkname5", 0);
            do return end;
::label2::
            if JudgeAttack(0, 40, 100) == true then goto label3 end;
                Talk(5, "小兄弟，看來你還需再下一番努力才是．", "talkname5", 0);
                do return end;
::label3::
                Talk(5, "小兄弟資質不錯，這是我最近研究出的一套拳法，名為太極拳．你拿去好好參詳吧我這套太極拳，和自來武學之道全然不同，講究以靜制動，後發制人．．．．", "talkname5", 0);
                Talk(0, "．．．這．．這．．．．．是了！這是以慢打快，以靜制動的上乘武學，想不到世間竟會有如此高明的功夫．謝謝前輩．", "talkname0", 1);
                GetItem(58, 1);
                ModifyEvent(-2, -2, -2, -2, 157, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
