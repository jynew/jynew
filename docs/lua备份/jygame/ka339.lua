Talk(90, "來者何人，可知這裡是凌霄城．", "talkname90", 0);
Talk(0, "小弟有事想求見貴派掌門．", "talkname0", 1);
Talk(90, "掌門師叔現下不見客．", "talkname90", 0);
Talk(0, "在下並無惡意，煩請這位大哥通報一聲．", "talkname0", 1);
Talk(90, "本派今有大事要辦，快快離去，別在這囉嗦．", "talkname90", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "實在對不起，在下一定得見見貴派掌門．", "talkname0", 1);
    Talk(90, "好個傢伙！想硬闖是不是？", "talkname90", 0);
    if TryBattle(58) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
        LightScence();
        AddRepute(1);
do return end;
