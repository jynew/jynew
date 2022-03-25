Talk(0, "王姑娘你好。", "talkname0", 1);
if InTeam(53) == true then goto label0 end;
    do return end;
::label0::
    Talk(53, "神仙姊姊……神仙姊姊……", "talkname53", 1);
    Talk(109, "…………", "talkname109", 0);
    if InTeam(53) == true then goto label1 end;
        do return end;
::label1::
        ModifyEvent(-2, 0, -2, -2, -1, -1, 491, -1, -1, -1, -2, -2, -2);
do return end;
