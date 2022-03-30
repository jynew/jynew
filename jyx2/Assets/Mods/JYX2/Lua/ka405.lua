Talk(0, "前辈好福气，竟住在如此奇妙的所在。", "talkname0", 1);
Talk(64, "你倒说说看，我这百花谷怎生好法。", "talkname64", 0);
Talk(0, "此处山谷向南，高山阻住了北风，想来地下又有硫磺，煤炭等类矿藏，地气特暖。因之阳春早临，百花先放。", "talkname0", 1);
Talk(64, "小兄弟还算有点见识。不过下次你再来时，又会有另一番风貌的。最近我正在驯养蜜蜂，虽然有点……有点不顺，但……但我一定会想办法让这些小东西乖乖驯服的。", "talkname64", 0);
Talk(0, "这阵子晚辈遍游江湖各地，道听途说了一些，没什么收获，倒是见闻增广了不少。", "talkname0", 1);
Talk(64, "你说你最近跑遍江湖，那你知不知道武林中有什么新功夫问世。", "talkname64", 0);
Talk(0, "晚辈是得到一些秘笈，不过功夫还不到家。", "talkname0", 1);
Talk(64, "来来来，跟我来玩两招。", "talkname64", 0);
ModifyEvent(-2, -2, -2, -2, 406, 407, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动406,407脚本。场景20-编号4
if AskBattle() == true then goto label0 end;
    Talk(0, "前辈别开玩笑了，晚辈怎是你的对手。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那晚辈就献丑了。", "talkname0", 1);
    if TryBattle(67) == false then goto label1 end;
        LightScence();
        Talk(64, "小兄弟，你这是什么功夫，教教我好不好。", "talkname64", 0);
        Talk(0, "哪里，前辈承让了。晚辈功夫还差得远。", "talkname0", 1);
        Talk(64, "这样好了，我跟你磕八个响头，拜你为师，你总肯教我了吧。", "talkname64", 0);
        Talk(0, "前辈别开玩笑了，晚辈担当不起。", "talkname0", 1);
        AddRepute(8);
        do return end;
::label1::
        LightScence();
        Talk(64, "唉，你功夫还差的远了，再去练练。", "talkname64", 0);
do return end;
