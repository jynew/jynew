Talk(0, "前輩好福氣，竟住在如此奇妙的所在．", "talkname0", 1);
Talk(64, "你倒說說看，我這百花谷怎生好法．", "talkname64", 0);
Talk(0, "此處山谷向南，高山阻住了北風，想來地下又有硫磺，煤炭等類礦藏，地氣特暖．因之陽春早臨，百花先放．", "talkname0", 1);
Talk(64, "小兄弟還算有點見識．不過下次你再來時，又會有另一番風貌的．最近我正在馴養蜜蜂，雖然有點．．有點不順，但．．但我一定會想辦法讓這些小東西乖乖馴服的．", "talkname64", 0);
Talk(0, "這陣子晚輩遍遊江湖各地，道聽途說了一些，沒什麼收穫，倒是見聞增廣了不少．", "talkname0", 1);
Talk(64, "你說你最近跑遍江湖，那你知不知道武林中有什麼新功夫問世．", "talkname64", 0);
Talk(0, "晚輩是得到一些秘笈，不過功夫還不到家．", "talkname0", 1);
Talk(64, "來來來，跟我來玩兩招．", "talkname64", 0);
ModifyEvent(-2, -2, -2, -2, 406, 407, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动406,407脚本。场景20-编号4
if AskBattle() == true then goto label0 end;
    Talk(0, "前輩別開玩笑了，晚輩怎是你的對手．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "那晚輩就獻醜了．", "talkname0", 1);
    if TryBattle(67) == false then goto label1 end;
        LightScence();
        Talk(64, "小兄弟，你這是什麼功夫，教教我好不好．", "talkname64", 0);
        Talk(0, "那裡，前輩承讓了．晚輩功夫還差得遠．", "talkname0", 1);
        Talk(64, "這樣好了，我跟你磕八個響頭，拜你為師，你總肯教我了吧．", "talkname64", 0);
        Talk(0, "前輩別開玩笑了，晚輩擔當不起．", "talkname0", 1);
        AddRepute(8);
        do return end;
::label1::
        LightScence();
        Talk(64, "唉，你功夫還差的遠了，再去練練．", "talkname64", 0);
do return end;
