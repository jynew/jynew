if HaveItem(110) == true then goto label0 end;
    Talk(54, "完成前兩道考驗後，再回來浡泥島上．", "talkname54", 0);
    do return end;
::label0::
    Talk(0, "袁兄，我找到金蛇山洞了，而且將金蛇劍給拔了出來．我已經通過了前兩道考驗．", "talkname0", 1);
    Talk(54, "很好，接下來讓我看看你在江湖上的表現．", "talkname54", 0);
    if JudgeEthics(0, 80, 100) == false then goto label1 end;
        Talk(54, "很好，你在江湖中行走這麼久，還能保持在正道上，很好．”碧血劍”一書就拿去吧．", "talkname54", 0);
        GetItem(156, 1);
        ModifyEvent(-2, -2, -2, -2, 638, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label1::
        Talk(54, "可惜呀可惜．雖然有了智慧和勇氣，但是”仁義”方面還要加強．", "talkname54", 0);
        ModifyEvent(-2, -2, -2, -2, 637, -1, -1, -2, -2, -2, -2, -2, -2);
        if AskBattle() == true then goto label2 end;
            do return end;
::label2::
            Talk(0, "袁兄，我沒什麼時間去增加”仁義”點數了，只好得罪了．", "talkname0", 1);
            if TryBattle(101) == true then goto label3 end;
                ModifyEvent(-2, -2, -2, -2, 637, -1, -1, -2, -2, -2, -2, -2, -2);
                LightScence();
                Talk(54, "我還是勸你多做些俠義之事才是．", "talkname54", 0);
                do return end;
::label3::
                ModifyEvent(-2, -2, -2, -2, 639, -1, -1, -2, -2, -2, -2, -2, -2);
                LightScence();
                Talk(54, "唉！你又往邪道走近一步，武功這麼好，為什麼不用在正途上呢？", "talkname54", 0);
                GetItem(156, 1);
                AddRepute(2);
do return end;
