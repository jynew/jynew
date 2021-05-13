Talk(8, "不知少俠來我崆峒山有何貴事？", "talkname8", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "我是來叮嚀你的，以後要跟明教和平相處哦！不要再互相殘殺了．", "talkname0", 1);
    Talk(8, "哼！", "talkname8", 0);
    do return end;
::label0::
    Talk(0, "我想找你練練功，賺些經驗點數．", "talkname0", 1);
    Talk(8, "哼！那就來吧．", "talkname8", 0);
    if TryBattle(17) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "嗯，這經驗點數還真好賺．", "talkname0", 1);
        AddEthics(-1);
do return end;
