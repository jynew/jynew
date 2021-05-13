Talk(0, "賢弟，我們走了吧．", "talkname0", 1);
if TeamIsFull() == false then goto label0 end;
    Talk(49, "你的隊伍已滿，我無法加入．", "talkname49", 0);
    do return end;
::label0::
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/xuzhu2", "");--虚竹加入
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    AddMP(49, 300);
    AddHP(49, 200);
    AddAttack(49, 30);
    AddSpeed(49, 20);
    LearnMagic2(49, 15, 0);
    SetPersonMPPro(49,2);
    Join(49);
do return end;
