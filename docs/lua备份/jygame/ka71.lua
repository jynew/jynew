if UseItem(181) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 72, -2, -2, -2, -2, -2, -2, -2, -2);
    AddItem(181, -1);
    Talk(9, "這．．．這一撮金毛是．．", "talkname9", 0);
    Talk(0, "你義父是不是長的一頭金髮？", "talkname0", 1);
    Talk(9, "是啊．義父人稱金毛獅王，當然是滿頭的金髮．", "talkname9", 0);
    Talk(0, "那沒錯了，我曾在北方的一個荒島上見過他．", "talkname0", 1);
    Talk(9, "真的？你知道荒島的位置嗎，快帶我去找他．", "talkname9", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "這恐怕不行，我還有許多要事在身，無法帶你去找他．", "talkname0", 1);
        Talk(9, "．．．．．", "talkname9", 0);
        do return end;
::label1::
        Talk(0, "好啊，我就帶你去找他．", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(9, "你的隊伍已滿，我無法加入．", "talkname9", 0);
            do return end;
::label2::
            Talk(9, "謝謝這位大哥．", "talkname9", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);  --by fanyu|加入队伍人物消失。场景04-编号01
            jyx2_ReplaceSceneObject("", "NPC/张无忌", ""); 
            LightScence();
            Join(9);
            AddEthics(2);
do return end;
