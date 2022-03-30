Talk(0, "袁兄，你看我现在符合“仁义”的资格了吗？", "talkname0", 1);
if JudgeEthics(0, 80, 100) == false then goto label0 end;
    Talk(54, "很好，你在江湖中行走这么久，还能保持在正道上，很好。《碧血剑》一书就拿去吧。", "talkname54", 0);
    AddItem(156, 1);
    ModifyEvent(-2, -2, -2, -2, 638, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(54, "可惜呀可惜。虽然有了智慧和勇气，但是“仁义”方面还要加强。", "talkname54", 0);
    if AskBattle() == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "袁兄，我没什么时间去增加“仁义”点数了，只好得罪了。", "talkname0", 1);
        if TryBattle(101) == true then goto label2 end;
            LightScence();
            Talk(54, "我还是劝你多做些侠义之事才是。", "talkname54", 0);
            do return end;
::label2::
            ModifyEvent(-2, -2, -2, -2, 639, -1, -1, -2, -2, -2, -2, -2, -2);
            LightScence();
            Talk(54, "唉！你又往邪道走近一步，武功这么好，为什么不用在正途上呢？", "talkname54", 0);
            AddItem(156, 1);
            AddRepute(2);
do return end;
