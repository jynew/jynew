Talk(9, "公子别来无恙？", "talkname9", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了张兄的九阳神功，一路上颇多凶险，是否可以再请张兄出马？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(9, "你的队伍已满，我无法加入。", "talkname9", 0);
        do return end;
::label1::
        Talk(9, "好的。", "talkname9", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/张无忌","");
        LightScence();
        Join(9);
do return end;
