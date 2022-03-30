Talk(109, "公子近来无恙？", "talkname109", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托王姑娘的福，一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "还好，不过若有王姑娘在队中指导我们攻击与防御，我们会更好。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(109, "你的队伍已满，我无法加入。", "talkname109", 0);
        do return end;
::label1::
        Talk(109, "好吧，我加入你们。", "talkname109", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/王语嫣","");
        LightScence();
        Join(76);
do return end;
