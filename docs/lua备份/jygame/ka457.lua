Talk(68, "閣下為何硬闖我全真教．", "talkname68", 0);
Talk(0, "前輩是丘掌門吧，我聽說先師王重陽武功天下第一，所以想看看他所創的全真教是否同他一樣厲害．", "talkname0", 1);
Talk(68, "廢話少說，動手吧．", "talkname68", 0);
if TryBattle(74) == true then goto label0 end;
    LightScence();
    Talk(68, "就這麼一點微末道行，也敢到我重陽宮撒野．", "talkname68", 0);
    Talk(0, "在下的確是自不量力，但人在江湖，身不由己啊！為了找那幾本書，我只有硬著頭皮四處踢館，看能不能有什麼收穫．", "talkname0", 1);
    if JudgeEthics(0, 50, 100) == true then goto label1 end;
        Talk(68, "就算功夫不行，也不能四處為惡，如果下次見到你時，你還陷入邪道之中，老道就要開殺戒了．", "talkname68", 0);
        ModifyEvent(-2, -2, -2, -2, 458, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动458脚本。场景19-编号00
        do return end;
::label1::
        Talk(68, "看你的本質還不壞，但武功太差了，這樣怎麼在江湖上混呢？我送你本秘笈，你拿去好好鑽研吧．", "talkname68", 0);
        Talk(0, "謝謝道長．", "talkname0", 1);
        GetItem(70, 1);
        ModifyEvent(-2, -2, -2, -2, 458, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动458脚本。场景19-编号00
        do return end;
::label0::
        ModifyEvent(-2, -2, -2, -2, 459, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动459脚本。场景19-编号00
        LightScence();
        AddRepute(5);
do return end;
