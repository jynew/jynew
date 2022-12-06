if UseItem(136) == true then goto label0 end;
    do return end;
::label0::
    AddEthics(4);
    ModifyEvent(-2, -2, -2, -2, 337, -2, -2, -2, -2, -2, -2, -2, -2);
    AddItemWithoutHint(136, -1);
    Talk(0, "小兄弟，快将这酒喝下去！", "talkname0", 1);
    Talk(38, "我不喜欢喝酒。", "talkname38", 0);
    Talk(0, "快喝了他吧！喝下去之后，你就不会忽冷忽热了。", "talkname0", 1);
    Talk(38, "真的？那我要喝。", "talkname38", 0);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, -2, -2, -2, 5152, 5152, 5152, -2, -2, -2);
	jyx2_FixMapObject("石破天痊愈",1);
    jyx2_SwitchRoleAnimation("NPC/石破天", "Assets/BuildSource/AnimationControllers/备份/ShipotianController.controller");
    ModifyEvent(-2, 1, -2, -2, -2, -2, 338, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(38, "哇！这酒怎么这么辛辣！不过好像真的挺有效的，谢谢你！既然老伯伯不在了，我要去找妈妈跟小黄了。", "talkname38", 0);
    if AskJoin () == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "你要找你妈妈？我正好在四处旅行，不妨我们结伴一起走，好吗？", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(38, "你的队伍已满，我无法加入。", "talkname38", 0);
            do return end;
::label2::
            Talk(38, "好啊！", "talkname38", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/石破天", "");--石破天加入队伍
            LightScence();
            Join(38);
            ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(40, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(40, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
do return end;
