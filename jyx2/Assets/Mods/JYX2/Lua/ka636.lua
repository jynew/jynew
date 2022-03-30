if HaveItem(110) == true then goto label0 end;
    Talk(54, "完成前两道考验后，再回来浡泥岛上。", "talkname54", 0);
    do return end;
::label0::
    Talk(0, "袁兄，我找到金蛇山洞了，而且将金蛇剑给拔了出来。我已经通过了前两道考验。", "talkname0", 1);
    Talk(54, "很好，接下来让我看看你在江湖上的表现。", "talkname54", 0);
    if JudgeEthics(0, 80, 100) == false then goto label1 end;
        Talk(54, "很好，你在江湖中行走这么久，还能保持在正道上，很好。《碧血剑》一书就拿去吧。", "talkname54", 0);
        AddItem(156, 1);
        ModifyEvent(-2, -2, -2, -2, 638, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label1::
        Talk(54, "可惜呀可惜。虽然有了智慧和勇气，但是“仁义”方面还要加强。", "talkname54", 0);
        ModifyEvent(-2, -2, -2, -2, 637, -1, -1, -2, -2, -2, -2, -2, -2);
        if AskBattle() == true then goto label2 end;
            do return end;
::label2::
            Talk(0, "袁兄，我没什么时间去增加“仁义”点数了，只好得罪了。", "talkname0", 1);
            if TryBattle(101) == true then goto label3 end;
                ModifyEvent(-2, -2, -2, -2, 637, -1, -1, -2, -2, -2, -2, -2, -2);
                LightScence();
                Talk(54, "我还是劝你多做些侠义之事才是。", "talkname54", 0);
                do return end;
::label3::
                ModifyEvent(-2, -2, -2, -2, 639, -1, -1, -2, -2, -2, -2, -2, -2);
                LightScence();
                Talk(54, "唉！你又往邪道走近一步，武功这么好，为什么不用在正途上呢？", "talkname54", 0);
                AddItem(156, 1);
                AddRepute(2);
do return end;
