Talk(68, "阁下为何硬闯我全真教。", "talkname68", 0);
Talk(0, "前辈是丘掌门吧，我听说先师王重阳武功天下第一，所以想看看他所创的全真教是否同他一样厉害。", "talkname0", 1);
Talk(68, "废话少说，动手吧。", "talkname68", 0);
if TryBattle(74) == true then goto label0 end;
    LightScence();
    Talk(68, "就这么一点微末道行，也敢到我重阳宫撒野。", "talkname68", 0);
    Talk(0, "在下的确是自不量力，但人在江湖，身不由己啊！为了找那几本书，我只有硬着头皮四处踢馆，看能不能有什么收获。", "talkname0", 1);
    if JudgeEthics(0, 50, 100) == true then goto label1 end;
        Talk(68, "就算功夫不行，也不能四处为恶，如果下次见到你时，你还陷入邪道之中，老道就要开杀戒了。", "talkname68", 0);
        ModifyEvent(-2, -2, -2, -2, 458, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动458脚本。场景19-编号00
        do return end;
::label1::
        Talk(68, "看你的本质还不坏，但武功太差了，这样怎么在江湖上混呢？我送你本秘笈，你拿去好好钻研吧。", "talkname68", 0);
        Talk(0, "谢谢道长。", "talkname0", 1);
        AddItem(70, 1);
        ModifyEvent(-2, -2, -2, -2, 458, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动458脚本。场景19-编号00
        do return end;
::label0::
        ModifyEvent(-2, -2, -2, -2, 459, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动459脚本。场景19-编号00
        LightScence();
        AddRepute(5);
do return end;
