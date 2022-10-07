if GetFlagInt("玉镯奇缘") == 1 then goto label0 end;
    Talk(180, "江湖险恶，你们资历尚浅，可要万事小心。");
    Talk(0, "好的，师父放心！");
    do return end;
::label0::
    if InTeam(80) == true then goto label1 end;
        Talk(180, "江湖险恶，你们资历尚浅，可要万事小心。");
        Talk(0, "好的，师父放心！");
        do return end;
::label1::
        Talk(180, "江湖险恶，你们资历尚浅，可要万事小心。");
        Talk(80, "感觉也不险恶嘛，听说罂粟谷是最险恶的地方，但那里的那位阿姨可好了，还送我一只玉镯子，让我凑成了一对。");
        Talk(180, "什么？你说这只玉镯子是罂粟谷的人给你的？");
        Talk(80, "是啊师父，有什么奇怪吗？");
        Talk(180, "这个，这对玉镯子有一个大秘密！");
        Talk(0, "什么秘密？");
        Talk(180, "<color=orange>这个秘密只有茶恩寺的寿眉大师能够破解，</color>你们可以去请教请教他。");
        ModifyEvent(-2, -2, -2, -2, 85, -1, -1, -2, -2, -2, -2, -2, -2);
        SetFlagInt("徐谦提示", 1);
do return end;
