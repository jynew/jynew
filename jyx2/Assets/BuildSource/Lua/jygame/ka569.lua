Talk(0, "徒儿，师父来看你了。", "talkname0", 1);
Talk(44, "……", "talkname44", 0);
Talk(0, "叫师父啊。", "talkname0", 1);
Talk(44, "……师……师父。", "talkname44", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "记得要乖哦！", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "徒儿，你就跟为师的走吧。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(44, "你的队伍已满，我无法加入。", "talkname44", 0);
        do return end;
::label1::
        Talk(44, "是。", "talkname44", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/NanHaiEShen",""); 
        LightScence();
        Join(44);
do return end;
