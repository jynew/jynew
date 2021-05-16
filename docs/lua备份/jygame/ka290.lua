Talk(0, "林兄劍法不知練的怎樣了？", "talkname0", 1);
Talk(36, "我迫不及待想上四川青城，誅滅他全派為我雙親報仇．可是以我現在的功力，恐怕無法辦到．", "talkname36", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "林兄別急，凡事慢慢來．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不然這樣好了，我和林兄一起去，殺光他青城派．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(36, "你的隊伍已滿，我無法加入．", "talkname36", 0);
        do return end;
::label1::
        Talk(36, "真的，我們走吧．", "talkname36", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(36, 3, -2, -2, 315, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        Join(36);
        AddEthics(-4);
do return end;
