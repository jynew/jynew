Talk(71, "谷主吩咐，闲杂人等不得入谷。");
if AskBattle() == true then goto label0 end;
    Talk(100, "罂粟谷搞得这么神神秘秘干嘛。");
    do return end;
::label0::
    Talk(0, "我可不是什么闲杂人等。");
    if TryBattle(70) == false then goto label1 end;
        jyx2_FixMapObject("罂粟谷守门弟子让路", 1);
        Talk(71, "看你还挺帅，我们谷主应该喜欢。");
        ModifyEvent(-2, 5, -2, -2, 713, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 7, -2, -2, 713, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label1::
        Talk(71, "再练练吧，小子。");
do return end;
