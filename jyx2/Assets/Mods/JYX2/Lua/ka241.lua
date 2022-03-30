if UseItem(126) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(126, -1);
    PlayAnimation(3, 5722, 5748);
    ModifyEvent(-2, -2, -2, -2, 237, 242, -1, 5722, 5748, 5722, -2, -2, -2);
    jyx2_PlayTimeline("[Timeline]ka238_悦来客栈_令狐冲喝酒", 0, false, "NPC/令狐冲");
    jyx2_Wait(0.9);
    jyx2_StopTimeline("[Timeline]ka238_悦来客栈_令狐冲喝酒");
    Talk(35, "嗯！好一坛梨花酒！曾闻白乐天杭州喜望诗云：“红袖织绫夸柿叶，青旗沽酒趁梨花”。这坛梨花酒酒味是极好，只可惜少了股芳冽之气，若能以翡翠杯盛之而饮，那更是醇美无比。", "talkname35", 0);
do return end;
