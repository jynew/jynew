if JudgeEthics(0, 50, 100) == false then goto label0 end;
    Talk(68, "最近江湖上对你的风评还挺不错的，希望你继续保持下去。", "talkname68", 0);
    do return end;
::label0::
    Talk(68, "你这作恶多端的小子，老道饶不了你。", "talkname68", 0);
    if TryBattle(75) == true then goto label1 end;
        LightScence();
        do return end;
::label1::
        ModifyEvent(-2, -2, -2, -2, 459, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动459脚本。场景19-编号00
        LightScence();
do return end;
