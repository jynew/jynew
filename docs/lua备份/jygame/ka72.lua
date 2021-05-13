Talk(9, "你能帶我去找義父嗎？", "talkname9", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "這恐怕不行，我還有許多要事在身，無法帶你去找他．", "talkname0", 1);
    Talk(9, "．．．．．", "talkname9", 0);
    do return end;
::label0::
    Talk(0, "好啊，我就帶你去找他．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(9, "你的隊伍已滿，我無法加入．", "talkname9", 0);
        do return end;
::label1::
        Talk(9, "謝謝這位大哥．", "talkname9", 0);
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/张无忌", ""); 
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(9);
        AddEthics(2);
do return end;
